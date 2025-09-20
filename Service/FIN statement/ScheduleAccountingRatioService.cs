using System.Data;
using Dapper;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TracePca.Data;
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

        //
        //public async Task<List<RatioDto>> LoadAccRatioAsync(int yearId, int customerId, int branchId)
        //{
        //    // Get DB from session
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session.");

        //    var connectionString = _configuration.GetConnectionString(dbName);

        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();
        //    using var transaction = connection.BeginTransaction();

        //    try
        //    {
        //        var dtDetails = new DataTable();
        //        dtDetails.Columns.Add("Sr_No");
        //        dtDetails.Columns.Add("Ratios");
        //        dtDetails.Columns.Add("Numerator");
        //        dtDetails.Columns.Add("Denominator");
        //        dtDetails.Columns.Add("Current_Reporting_Period");
        //        dtDetails.Columns.Add("Previous_reporting_period");
        //        dtDetails.Columns.Add("Change");

        //        string[] ratios = new string[]
        //        {
        //    "Debt Equity Ratio",
        //    "Debt Capital",
        //    "Debt Service coverage ratio",
        //    "Return on Equity Ratio",
        //    "Inventory Turnover Ratio",
        //    "Trade Receivables turnover ratio",
        //    "Trade payables turnover ratio",
        //    "Net capital turnover ratio",
        //    "Net profit ratio",
        //    "Return on Capital employed",
        //    "Return on investment"
        //        };

        //        string[] numerators = new string[]
        //        {
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
        //        };

        //        string[] denominators = new string[]
        //        {
        //    "Total Current Liabilities",
        //    "Shareholder's Equity",
        //    "Debt Service (Int+Principal)",
        //    "Average Shareholder’s Equity",
        //    "Average Inventory",
        //    "Average trade receivables",
        //    "Closing Trade Payables",
        //    "Workimg capital (CA-CL)",
        //    "Sales",
        //    "Capital Employed",
        //    "Investment"
        //        };

        //        for (int i = 0; i < 11; i++)
        //        {
        //            var row = dtDetails.NewRow();
        //            row["Sr_No"] = (i + 1) + ".";
        //            row["Ratios"] = ratios[i];
        //            row["Numerator"] = numerators[i];
        //            row["Denominator"] = denominators[i];
        //            dtDetails.Rows.Add(row);
        //        }


        //        // --- Ratio 1 ---
        //        int headingIdCL = await GetHeadingId(connection, transaction, customerId, "4 Current Liabilities");
        //        int headingIdCA = await GetHeadingId(connection, transaction, customerId, "2 Current Assets");

        //        var dtCL = await GetHeadingAmt1(connection, transaction, yearId, customerId, 4, headingIdCL);
        //        var dtCA = await GetHeadingAmt1(connection, transaction, yearId, customerId, 4, headingIdCA);

        //        decimal dCValue = 0, dPValue = 0, dDiffVal = 0;

        //        // Safe division helper
        //        decimal SafeDivide(decimal numerator, decimal denominator) => denominator == 0 ? 0 : numerator / denominator;

        //        if (dtCA.Dc1 != 0 && dtCL.Dc1 != 0)
        //        {
        //            dCValue = SafeDivide(dtCA.Dc1, dtCL.Dc1);
        //            dPValue = SafeDivide(dtCA.DP1, dtCL.DP1);

        //            dDiffVal = dCValue - dPValue;
        //        }

        //        // --- Ratio 2 ---
        //        int subLongTermId = await GetSubHeadingId(connection, transaction, customerId, "(a) Long-term borrowings");
        //        int subShortTermId = await GetSubHeadingId(connection, transaction, customerId, "(a) Short Term Borrowings");
        //        int headingShareId = await GetHeadingId(connection, transaction, customerId, "1 Shareholders Funds");

        //        var dtLongTerm = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, 4, subLongTermId); // Get amounts
        //        var dtShortTerm = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, 4, subShortTermId);
        //        var dtShareHolder = await GetHeadingAmt1(connection, transaction, yearId, customerId, 4, headingShareId);

        //        decimal dPandL = await GetPandLFinalAmt(connection, transaction, yearId, customerId, branchId);
        //        decimal dPrevPandL = await GetPandLFinalAmt(connection, transaction, yearId - 1, customerId, branchId);

        //        decimal dCTotal1 = 0, dPTotal1 = 0, dCTotal2 = 0, dPTotal2 = 0, dDiff = 0;

        //        if (dtLongTerm.Dc1 != 0 || dtShortTerm.Dc1 != 0)  // Calculate total borrowings
        //            dCTotal1 = dtLongTerm.Dc1 + dtShortTerm.Dc1;

        //        if (dtLongTerm.DP1 != 0 || dtShortTerm.DP1 != 0)
        //            dPTotal1 = dtLongTerm.DP1 + dtShortTerm.DP1;

        //        if (dtShareHolder.Dc1 != 0 || dCTotal1 != 0) // Calculate ratio over shareholder funds
        //            dCTotal2 = dCTotal1 / (dtShareHolder.Dc1 + dPandL);

        //        if (dtShareHolder.DP1 != 0 || dPTotal1 != 0)
        //            dPTotal2 = dPTotal1 / (dtShareHolder.DP1 + dPrevPandL);

        //        dDiff = dCTotal2 - dPTotal2;

        //        // --- Ratio 3 ---
        //        int headingIncomeId = await GetHeadingId(connection, transaction, customerId, "Income");
        //        int headingExpensesId = await GetHeadingId(connection, transaction, customerId, "IV Expenses");
        //        int subDepreciationId = await GetSubHeadingId(connection, transaction, customerId, "(f) Depreciation and Amortisation Expenses");
        //        int subFinanceCostId = await GetSubHeadingId(connection, transaction, customerId, "(e) Finance Costs");
        //        int subLongTermBorrowId = await GetSubHeadingId(connection, transaction, customerId, "(a) Long-term borrowings");

        //        var dtIncome = await GetHeadingAmt1(connection, transaction, yearId, customerId, 3, headingIncomeId); // Get amounts
        //        var dtExpenses = await GetHeadingAmt1(connection, transaction, yearId, customerId, 3, headingExpensesId);
        //        var dtDepreciation = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, 3, subDepreciationId);
        //        var dtFinancialCosts = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, 3, subFinanceCostId);
        //        var dtLongTermBorrower = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, 4, subLongTermBorrowId);

        //        decimal dCYProfit = (dtIncome.Dc1 - dtExpenses.Dc1) + dtDepreciation.Dc1 + dtFinancialCosts.Dc1; // Calculate Profit Before Interest & Financial Costs
        //        decimal dPYProfit = (dtIncome.DP1 - dtExpenses.DP1) + dtDepreciation.DP1 + dtFinancialCosts.DP1;

        //        decimal dCYFinancialCost = dtFinancialCosts.Dc1 + dtLongTermBorrower.Dc1; // Calculate Financial Costs (Denominator)
        //        decimal dPYFinancialCost = dtFinancialCosts.DP1 + dtLongTermBorrower.DP1;

        //        dCYProfit = dCYFinancialCost != 0 ? dCYProfit / dCYFinancialCost : 0; // Avoid division by zero
        //        dPYProfit = dPYFinancialCost != 0 ? dPYProfit / dPYFinancialCost : 0;

        //        decimal dDiff = dCYProfit - dPYProfit;

        //        // --- Ratio 4: Return on Equity ---
        //        decimal dPandL = await GetPandLFinalAmt(connection, transaction, yearId, customerId, branchId);
        //        decimal dPrevPandL = await GetPandLFinalAmt(connection, transaction, yearId - 1, customerId, branchId);

        //        int headingShareId = await GetHeadingId(connection, transaction, customerId, "1 Shareholders Funds");  // Get Shareholder Funds heading ID

        //        var dtCurShareFund = await GetHeadingAmt1(connection, transaction, yearId, customerId, 4, headingShareId); // Get current and previous year shareholder funds
        //        var dtPrevShareFund = await GetHeadingAmt1(connection, transaction, yearId - 1, customerId, 4, headingShareId);

        //        decimal dCyShareAmt = 0, dPyShareAmt = 0;

        //        if (dtCurShareFund.Dc1 != 0 || dtCurShareFund.DP1 != 0 || dtPrevShareFund.Dc1 != 0 || dtPrevShareFund.DP1 != 0)
        //        {
        //            dCyShareAmt = ((dtCurShareFund.Dc1 + dPandL) + (dtCurShareFund.DP1 + dPrevPandL)) / 2;  // Average shareholder funds
        //            dPyShareAmt = ((dtPrevShareFund.Dc1 + dPrevPandL) + dtPrevShareFund.DP1) / 2;

        //            dCyShareAmt = dCyShareAmt != 0 ? dPandL / dCyShareAmt : 0; // Return on equity calculation
        //            dPyShareAmt = dPyShareAmt != 0 ? dPrevPandL / dPyShareAmt : 0;
        //        }

        //        if (decimal.IsInfinity(dCyShareAmt) || decimal.IsNaN(dCyShareAmt)) dCyShareAmt = 0; // Ensure no NaN or Infinity
        //        if (decimal.IsInfinity(dPyShareAmt) || decimal.IsNaN(dPyShareAmt)) dPyShareAmt = 0;

        //        decimal dDiff = dCyShareAmt - dPyShareAmt;

        //        // --- Ratio 5: Inventory Turnover ---
        //        int subCostMaterialId = await GetSubHeadingId(connection, transaction, customerId, "(a) Cost Of Materials Consumed");
        //        int subInventoriesId = await GetSubHeadingId(connection, transaction, customerId, "(b) Inventories");

        //        // Get amounts
        //        var dtCostSales = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, 3, subCostMaterialId);
        //        var dtCurInventory = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, 4, subInventoriesId);
        //        var dtPrevInventory = await GetSubHeadingAmt1(connection, transaction, yearId - 1, customerId, 4, subInventoriesId);

        //        decimal dCYInventory = 0m, dPYInventory = 0m;

        //        if ((dtCurInventory.Dc1 != 0 || dtPrevInventory.Dc1 != 0) && dtCostSales.Dc1 != 0)
        //        {
        //            decimal avgCurrentInventory = (dtCurInventory.Dc1 + dtPrevInventory.Dc1) / 2;
        //            decimal avgPreviousInventory = (dtCurInventory.DP1 + dtPrevInventory.DP1) / 2;

        //            dCYInventory = avgCurrentInventory != 0 ? dtCostSales.Dc1 / avgCurrentInventory : 0;
        //            dPYInventory = avgPreviousInventory != 0 ? dtCostSales.DP1 / avgPreviousInventory : 0;
        //        }

        //        // Ensure no NaN or Infinity
        //        if (decimal.IsInfinity(dCYInventory) || decimal.IsNaN(dCYInventory)) dCYInventory = 0;
        //        if (decimal.IsInfinity(dPYInventory) || decimal.IsNaN(dPYInventory)) dPYInventory = 0;

        //        decimal dDiff = dCYInventory - dPYInventory;

        //        // --- Ratio 6: Trade Receivables Turnover ---
        //        int subRevenueId = await GetSubHeadingId(connection, transaction, customerId, "I Revenue from operations");
        //        int subTradeReceivableId = await GetSubHeadingId(connection, transaction, customerId, "(c) Trade receivables");

        //        // Get amounts
        //        var dtRevenue = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, 3, subRevenueId);
        //        var dtCurReceivable = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, 4, subTradeReceivableId);
        //        var dtPrevReceivable = await GetSubHeadingAmt1(connection, transaction, yearId - 1, customerId, 4, subTradeReceivableId);

        //        decimal dCYTradeReceivable = 0m, dPYTradeReceivable = 0m;

        //        if ((dtCurReceivable.Dc1 != 0 || dtPrevReceivable.Dc1 != 0) && dtRevenue.Dc1 != 0)
        //        {
        //            decimal avgCurrentReceivable = (dtCurReceivable.Dc1 + dtPrevReceivable.Dc1) / 2;
        //            decimal avgPreviousReceivable = (dtCurReceivable.DP1 + dtPrevReceivable.DP1) / 2;

        //            dCYTradeReceivable = avgCurrentReceivable != 0 ? dtRevenue.Dc1 / avgCurrentReceivable : 0;
        //            dPYTradeReceivable = avgPreviousReceivable != 0 ? dtRevenue.DP1 / avgPreviousReceivable : 0;
        //        }

        //        if (decimal.IsInfinity(dCYTradeReceivable) || decimal.IsNaN(dCYTradeReceivable)) dCYTradeReceivable = 0; // Ensure no NaN or Infinity
        //        if (decimal.IsInfinity(dPYTradeReceivable) || decimal.IsNaN(dPYTradeReceivable)) dPYTradeReceivable = 0;

        //        decimal dDiff = dCYTradeReceivable - dPYTradeReceivable;

        //        // --- Ratio 7: Trade Payables Turnover ---
        //        int subCostOfGoodsId = await GetSubHeadingId(connection, transaction, customerId, "(a) Cost Of Materials Consumed");
        //        int subTradePayableId = await GetSubHeadingId(connection, transaction, customerId, "(b) Trade payables");

        //        // Get amounts
        //        var dtCostOfGoods = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, 3, subCostOfGoodsId);
        //        var dtTradePayable = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, 4, subTradePayableId);

        //        decimal dCYTradePayable = 0m, dPYTradePayable = 0m;

        //        if (dtCostOfGoods != null && dtTradePayable != null)
        //        {
        //            // Current Year
        //            if (dtCostOfGoods.Dc1 == 0 || dtTradePayable.DC1 == 0)
        //            {
        //                dCYTradePayable = dtCostOfGoods.Dc1 != 0 ? dtCostOfGoods.Dc1 : 0;
        //            }
        //            else
        //            {
        //                dCYTradePayable = dtCostOfGoods.Dc1 / dtTradePayable.DC1;
        //            }

        //            // Previous Year
        //            if (dtCostOfGoods.DP1 == 0 || dtTradePayable.DP1 == 0)
        //            {
        //                dPYTradePayable = dtTradePayable.DP1 != 0 ? dtTradePayable.DP1 : dtCostOfGoods.DP1;
        //            }
        //            else
        //            {
        //                dPYTradePayable = dtCostOfGoods.DP1 / dtTradePayable.DP1;
        //            }
        //        }

        //        // Ensure no NaN or Infinity
        //        if (decimal.IsInfinity(dCYTradePayable) || decimal.IsNaN(dCYTradePayable)) dCYTradePayable = 0;
        //        if (decimal.IsInfinity(dPYTradePayable) || decimal.IsNaN(dPYTradePayable)) dPYTradePayable = 0;

        //        decimal dDiff = dCYTradePayable - dPYTradePayable;

        //        // --- Ratio 8: Net Capital Turnover ---
        //        int headingCurrentLiabilitiesId = await GetHeadingId(connection, transaction, customerId, "4 Current Liabilities");
        //        int headingCurrentAssetsId = await GetHeadingId(connection, transaction, customerId, "2 Current Assets");

        //        // Get amounts
        //        var dtCurrentLiabilities = await GetHeadingAmt1(connection, transaction, yearId, customerId, 4, headingCurrentLiabilitiesId);
        //        var dtCurrentAssets = await GetHeadingAmt1(connection, transaction, yearId, customerId, 4, headingCurrentAssetsId);

        //        decimal dCTotalValue = 0m, dTotalPValue = 0m;

        //        if (dtCurrentAssets != null && dtCurrentLiabilities != null)
        //        {
        //            // Current year
        //            if (dtCurrentAssets.Dc1 != 0 || dtCurrentLiabilities.DC1 != 0)
        //            {
        //                dCTotalValue = dtCurrentLiabilities.Dc1 - dtCurrentAssets.DC1;
        //            }

        //            // Previous year
        //            if (dtCurrentAssets.DP1 != 0 || dtCurrentLiabilities.DP1 != 0)
        //            {
        //                dTotalPValue = dtCurrentLiabilities.DP1 - dtCurrentAssets.DP1;
        //            }
        //        }

        //        // Get Revenue
        //        int subRevenueId = await GetSubHeadingId(connection, transaction, customerId, "I Revenue from operations");
        //        var dtRevenue = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, 3, subRevenueId);

        //        if (dtRevenue != null)
        //        {
        //            if (dCTotalValue != 0) dCTotalValue = dtRevenue.DC1 / dCTotalValue;
        //            if (dTotalPValue != 0) dTotalPValue = dtRevenue.DP1 / dTotalPValue;
        //        }

        //        // Ensure no NaN or Infinity
        //        if (decimal.IsInfinity(dCTotalValue) || decimal.IsNaN(dCTotalValue)) dCTotalValue = 0;
        //        if (decimal.IsInfinity(dTotalPValue) || decimal.IsNaN(dTotalPValue)) dTotalPValue = 0;

        //        decimal dDiff = dCTotalValue - dTotalPValue;

        //        // --- Ratio 9: Net Profit Ratio ---
        //        decimal dPandLamt = await GetPandLFinalAmt(connection, transaction, yearId, customerId, branchId);
        //        decimal dPrevPandLamt = await GetPandLFinalAmt(connection, transaction, yearId - 1, customerId, branchId);

        //        // Get Revenue heading
        //        int subRevenueId = await GetSubHeadingId(connection, transaction, customerId, "I Revenue from operations");
        //        var dtRevenue = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, 3, subRevenueId);

        //        decimal dCYTotal9 = 0m, dPYTotal9 = 0m;

        //        if (dtRevenue != null)
        //        {
        //            if (dtRevenue.DC1 != 0)
        //            {
        //                dCYTotal9 = dPandLamt / dtRevenue.DC1;
        //            }

        //            if (dtRevenue.DP1 != 0 && dPrevPandLamt != 0)
        //            {
        //                dPYTotal9 = dPrevPandLamt / dtRevenue.DP1;
        //            }
        //        }

        //        // Handle NaN / Infinity
        //        if (decimal.IsInfinity(dCYTotal9) || decimal.IsNaN(dCYTotal9)) dCYTotal9 = 0;
        //        if (decimal.IsInfinity(dPYTotal9) || decimal.IsNaN(dPYTotal9)) dPYTotal9 = 0;

        //        decimal dDiff = dCYTotal9 - dPYTotal9;

        //        // --- Ratio 10: Return on Capital Employed ---
        //        int headingIncomeId = await GetHeadingId(connection, transaction, customerId, "Income");
        //        int headingExpensesId = await GetHeadingId(connection, transaction, customerId, "IV Expenses");
        //        int subLongTermBorrowingId = await GetSubHeadingId(connection, transaction, customerId, "(a) Long-term borrowings");

        //        var dtIncome = await GetHeadingAmt1(connection, transaction, yearId, customerId, 3, headingIncomeId);
        //        var dtExpenses = await GetHeadingAmt1(connection, transaction, yearId, customerId, 3, headingExpensesId);
        //        var dtFinancialCosts = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, 4, subLongTermBorrowingId);

        //        decimal dCYProfiTAmt = 0m, dPYProfiTAmt = 0m;

        //        if (dtIncome != null && dtExpenses != null)
        //        {
        //            dCYProfiTAmt = dtIncome.DC1 - dtExpenses.DC1;
        //            dPYProfiTAmt = dtIncome.DP1 - dtExpenses.DP1;
        //        }

        //        // Get Shareholders Funds & Long-term Borrowings
        //        decimal dPandLamt = await GetPandLFinalAmt(connection, transaction, yearId, customerId, branchId);
        //        decimal dPrevPandLamt = await GetPandLFinalAmt(connection, transaction, yearId - 1, customerId, branchId);

        //        int headingShareFundId = await GetHeadingId(connection, transaction, customerId, "1 Shareholders Funds");

        //        var dtShareFund = await GetHeadingAmt1(connection, transaction, yearId, customerId, 4, headingShareFundId);
        //        var dtLongTermBorro = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, 4, subLongTermBorrowingId);

        //        decimal dCYTotalAmt10 = dtShareFund != null ? dtShareFund.DC1 + dPandLamt : 0m;
        //        decimal dPYTotalAmt10 = dtShareFund != null ? dtShareFund.DP1 + dPrevPandLamt : 0m;

        //        if (dtLongTermBorro != null)
        //        {
        //            dCYTotalAmt10 += dtLongTermBorro.DC1;
        //        }

        //        if (dCYTotalAmt10 > 0)
        //        {
        //            dCYTotalAmt10 = dCYProfiTAmt / dCYTotalAmt10;
        //            dPYTotalAmt10 = dPYProfiTAmt != 0 ? dPYProfiTAmt / dPYTotalAmt10 : 0;
        //        }
        //        else
        //        {
        //            dCYTotalAmt10 = 0;
        //            dPYTotalAmt10 = 0;
        //        }

        //        // Handle NaN / Infinity
        //        if (decimal.IsInfinity(dCYTotalAmt10) || decimal.IsNaN(dCYTotalAmt10)) dCYTotalAmt10 = 0;
        //        if (decimal.IsInfinity(dPYTotalAmt10) || decimal.IsNaN(dPYTotalAmt10)) dPYTotalAmt10 = 0;

        //        decimal dDiff = dCYTotalAmt10 - dPYTotalAmt10;

        //        // --- Ratio 11: Return on Investment ---
        //        decimal dPandLamt = await GetPandLFinalAmt(connection, transaction, yearId, customerId, branchId);
        //        decimal dPrevPandLamt = await GetPandLFinalAmt(connection, transaction, yearId - 1, customerId, branchId);

        //        // Get Subheading IDs
        //        int subCurrentInvId = await GetSubHeadingId(connection, transaction, customerId, "(a) Current Investments");
        //        int subNonCurrentInvId = await GetSubHeadingId(connection, transaction, customerId, "(b) Non-current investments");

        //        // Get investment data
        //        var dtCurrInv = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, 4, subCurrentInvId);
        //        var dtNonInv = await GetSubHeadingAmt1(connection, transaction, yearId, customerId, 4, subNonCurrentInvId);

        //        decimal dCYTotal11 = 0m, dPYTotal11 = 0m;

        //        if (dtNonInv != null && dtCurrInv != null)
        //        {
        //            dCYTotal11 = dtRevenue.DC1 / dPandLamt;
        //            dPYTotal11 = dtRevenue.DP1 / dPrevPandLamt;
        //        }

        //        if (dCYTotal11 != 0)
        //        {
        //            dCYTotal11 = dPandLamt / dCYTotal11;
        //            dCYTotal11 = dPandLamt / dCYTotal11;
        //        }
        //        else
        //        {
        //            dCYTotal11 = 0;
        //        }

        //        if (dPYTotal11 != 0)
        //        {
        //            dPYTotal11 = dPrevPandLamt / dPYTotal11;
        //            dPYTotal11 = dPrevPandLamt / dPYTotal11;
        //        }

        //        if (decimal.IsNaN(dCYTotal11) || decimal.IsInfinity(dCYTotal11)) dCYTotal11 = 0;
        //        if (decimal.IsNaN(dPYTotal11) || decimal.IsInfinity(dPYTotal11)) dPYTotal11 = 0;

        //        decimal dDiff = dCYTotal11 - dPYTotal11;

        //        transaction.Commit();
        //        //return dtDetails;
        //    }
        //    catch
        //    {
        //        transaction.Rollback();
        //        throw;
        //    }
        //}

        //private async Task<int> GetHeadingId(SqlConnection conn, SqlTransaction tran, int customerId, string headName)
        //{
        //    const string sql = @"
        //    SELECT ASH_ID 
        //    FROM ACC_ScheduleHeading 
        //    WHERE ASH_Name=@HeadName AND ASH_OrgType=@CustId";
        //    return await conn.ExecuteScalarAsync<int?>(sql,
        //        new { HeadName = headName, CustId = customerId },
        //        tran) ?? 0;
        //}

        //private async Task<int> GetSubHeadingId(SqlConnection conn, SqlTransaction tran, int customerId, string subHeadName)
        //{
        //    const string sql = @"
        //    SELECT ASSH_ID 
        //    FROM ACC_SchedulesubHeading 
        //    WHERE ASSH_Name=@SubHeadName AND ASSH_OrgType=@CustId";
        //    return await conn.ExecuteScalarAsync<int?>(sql,
        //        new { SubHeadName = subHeadName, CustId = customerId },
        //        tran) ?? 0;
        //}

        //private async Task<(decimal Dc1, decimal DP1)> GetHeadingAmt1(SqlConnection conn, SqlTransaction tran,
        //    int yearId, int customerId, int schedType, int headingId)
        //{
        //    if (headingId == 0) return (0m, 0m);

        //    const string sql = @"
        //    SELECT 
        //        ABS(ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount),0)) AS Dc1,
        //        ABS(ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount),0)) AS DP1
        //    FROM Acc_TrailBalance_Upload_Details
        //    LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID=ATBUD_HeadingId
        //    LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description=ATBUD_Description AND d.ATBU_YearId=@YearId AND d.ATBU_CustId=@CustomerId AND ATBUD_YearId=@YearId
        //    LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description=ATBUD_Description AND e.ATBU_YearId=@PrevYear AND e.ATBU_CustId=@CustomerId AND ATBUD_YearId=@PrevYear
        //    WHERE ATBUD_Schedule_Type=@SchedType AND ATBUD_CustId=@CustomerId AND ATBUD_HeadingId=@HeadingId
        //    GROUP BY ATBUD_HeadingId
        //    ORDER BY ATBUD_HeadingId";

        //    return await conn.QueryFirstOrDefaultAsync<(decimal Dc1, decimal DP1)>(sql,
        //        new { YearId = yearId, PrevYear = yearId - 1, CustomerId = customerId, SchedType = schedType, HeadingId = headingId },
        //        tran);
        //}

        //private async Task<(decimal Dc1, decimal DP1)> GetSubHeadingAmt1(SqlConnection conn, SqlTransaction tran,
        //    int yearId, int customerId, int schedType, int subHeadingId)
        //{
        //    if (subHeadingId == 0) return (0m, 0m);

        //    const string sql = @"
        //    SELECT 
        //        ABS(ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount),0)) AS Dc1,
        //        ABS(ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount),0)) AS DP1
        //    FROM Acc_TrailBalance_Upload_Details
        //    LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID=ATBUD_SubHeading
        //    LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description=ATBUD_Description AND d.ATBU_YearId=@YearId AND d.ATBU_CustId=@CustomerId AND ATBUD_YearId=@YearId
        //    LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description=ATBUD_Description AND e.ATBU_YearId=@PrevYear AND e.ATBU_CustId=@CustomerId AND ATBUD_YearId=@PrevYear
        //    WHERE ATBUD_Schedule_Type=@SchedType AND ATBUD_CustId=@CustomerId AND ATBUD_SubHeading=@SubHeadingId
        //    GROUP BY ATBUD_HeadingId
        //    ORDER BY ATBUD_HeadingId";

        //    return await conn.QueryFirstOrDefaultAsync<(decimal Dc1, decimal DP1)>(sql,
        //        new { YearId = yearId, PrevYear = yearId - 1, CustomerId = customerId, SchedType = schedType, SubHeadingId = subHeadingId },
        //        tran);
        //}

        //private async Task<decimal> GetPandLFinalAmt(SqlConnection conn, SqlTransaction tran, int yearId, int customerId, int branchId)
        //{
        //    const string sql = @"SELECT SUM(ATBU_Closing_TotalDebit_Amount - ATBU_Closing_TotalCredit_Amount) as Amt
        //                     FROM Acc_TrailBalance_Upload
        //                     WHERE ATBU_Description='Net Income' AND ATBU_YearId=@YearId AND ATBU_CustId=@CustomerId AND ATBU_BranchId=@BranchId";

        //    return await conn.ExecuteScalarAsync<decimal?>(sql, new { YearId = yearId, CustomerId = customerId, BranchId = branchId }, tran) ?? 0m;
        //}
    }
}

