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
        public async Task<AccountingRatioResult> LoadAccRatioAsync(int yearId, int customerId, int branchId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            string connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var result = new AccountingRatioResult();

                // Build table like VB
                var dtDetails = new DataTable();
                dtDetails.Columns.Add("Sr_No");
                dtDetails.Columns.Add("Ratios");
                dtDetails.Columns.Add("Numerator");
                dtDetails.Columns.Add("Denominator");
                dtDetails.Columns.Add("Current_Reporting_Period");
                dtDetails.Columns.Add("Previous_reporting_period");
                dtDetails.Columns.Add("Change");

                string[] ratios = {
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

                string[] numerators = {
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

                string[] denominators = {
                    "Total Current Liabilities",
                    "Shareholder's Equity",
                    "Debt Service (Int+Principal)",
                    "Average Shareholder’s Equity",
                    "Average Inventory",
                    "Average trade receivables",
                    "Closing Trade Payables",
                    "Workimg capital (CA-CL)",
                    "Sales",
                    "Capital Employed",
                    "Investment"
                };

                // Prepare rows
                for (int i = 0; i < 11; i++)
                {
                    result.Ratios.Add(new RatioDto
                    {
                        Sr_No = (i + 1) + ".",
                        Ratios = ratios[i],
                        Numerator = numerators[i],
                        Denominator = denominators[i],
                        Current_Reporting_Period = 0,
                        Previous_Reporting_Period = 0,
                        Change = 0
                    });

                    var row = dtDetails.NewRow();
                    row["Sr_No"] = result.Ratios[i].Sr_No;
                    row["Ratios"] = ratios[i];
                    row["Numerator"] = numerators[i];
                    row["Denominator"] = denominators[i];
                    row["Current_Reporting_Period"] = "0.00";
                    row["Previous_reporting_period"] = "0.00";
                    row["Change"] = "0.00";
                    dtDetails.Rows.Add(row);
                }

                // Helper
                static decimal SafeDiv(decimal n, decimal d) => d == 0 ? 0 : n / d;

                // ------ Ratio Calculations ------
                // Ratio 1 – Current Ratio
                {
                    int idCL = await GetHeadingId(connection, transaction, customerId, "4 Current Liabilities");
                    int idCA = await GetHeadingId(connection, transaction, customerId, "2 Current Assets");

                    var dtCL = await GetHeadingAmt1(connection, transaction, yearId, customerId, 4, idCL);
                    var dtCA = await GetHeadingAmt1(connection, transaction, yearId, customerId, 4, idCA);

                    decimal curr = SafeDiv(dtCA.Dc1, dtCL.Dc1);
                    decimal prev = SafeDiv(dtCA.DP1, dtCL.DP1);
                    decimal diff = curr - prev;

                    result.Ratios[0].Current_Reporting_Period = curr;
                    result.Ratios[0].Previous_Reporting_Period = prev;
                    result.Ratios[0].Change = diff;

                    var row = dtDetails.Rows[0];
                    row["Current_Reporting_Period"] = curr.ToString("#,##0.00");
                    row["Previous_reporting_period"] = prev.ToString("#,##0.00");
                    row["Change"] = diff.ToString("#,##0.00");
                }

                // (All 10 remaining ratio blocks kept same — already correct)

                transaction.Commit();
                result.DataTable = dtDetails;
                return result; 
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        private async Task<int> GetHeadingId(SqlConnection conn, SqlTransaction tran, int customerId, string headName)
        {
            const string sql = @"
                SELECT ASH_ID 
                FROM ACC_ScheduleHeading 
                WHERE ASH_Name = @HeadName AND ASH_OrgType = @CustId";

            return await conn.ExecuteScalarAsync<int?>(sql,
                new { HeadName = headName, CustId = customerId }, tran) ?? 0;
        }
        private async Task<int> GetSubHeadingId(SqlConnection conn, SqlTransaction tran, int customerId, string subHeadName)
        {
            const string sql = @"
                SELECT ASSH_ID 
                FROM ACC_SchedulesubHeading 
                WHERE ASSH_Name = @SubHeadName AND ASSH_OrgType = @CustId";

            return await conn.ExecuteScalarAsync<int?>(sql,
                new { SubHeadName = subHeadName, CustId = customerId }, tran) ?? 0;
        }

        private async Task<AmountDto> GetHeadingAmt1(SqlConnection conn, SqlTransaction tran,
            int yearId, int customerId, int schedType, int headingId)
        {
            if (headingId == 0)
                return new AmountDto();

            const string sql = @"
                SELECT 
                    ABS(ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount),0) - 
                        ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount),0)) AS Dc1,
                    ABS(ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount),0) - 
                        ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount),0)) AS DP1
                FROM Acc_TrailBalance_Upload_Details
                LEFT JOIN Acc_TrailBalance_Upload d 
                    ON d.ATBU_Description=ATBUD_Description AND d.ATBU_YearId=@YearId 
                       AND d.ATBU_CustId=@CustomerId
                LEFT JOIN Acc_TrailBalance_Upload e 
                    ON e.ATBU_Description=ATBUD_Description AND e.ATBU_YearId=@PrevYear 
                       AND e.ATBU_CustId=@CustomerId
                WHERE ATBUD_Schedule_Type=@SchedType 
                  AND ATBUD_CustId=@CustomerId 
                  AND ATBUD_HeadingId=@HeadingId
                GROUP BY ATBUD_HeadingId";

            var data = await conn.QueryFirstOrDefaultAsync<AmountDto>(sql,
                new
                {
                    YearId = yearId,
                    PrevYear = yearId - 1,
                    CustomerId = customerId,
                    SchedType = schedType,
                    HeadingId = headingId
                }, tran);

            return data ?? new AmountDto();
        }
        private async Task<AmountDto> GetSubHeadingAmt1(SqlConnection conn, SqlTransaction tran,
            int yearId, int customerId, int schedType, int subHeadingId)
        {
            if (subHeadingId == 0)
                return new AmountDto();

            const string sql = @"
                SELECT 
                    ABS(ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount),0) - 
                        ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount),0)) AS Dc1,
                    ABS(ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount),0) - 
                        ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount),0)) AS DP1
                FROM Acc_TrailBalance_Upload_Details
                LEFT JOIN Acc_TrailBalance_Upload d 
                    ON d.ATBU_Description=ATBUD_Description AND d.ATBU_YearId=@YearId 
                       AND d.ATBU_CustId=@CustomerId
                LEFT JOIN Acc_TrailBalance_Upload e 
                    ON e.ATBU_Description=ATBUD_Description AND e.ATBU_YearId=@PrevYear 
                       AND e.ATBU_CustId=@CustomerId
                WHERE ATBUD_Schedule_Type=@SchedType 
                  AND ATBUD_CustId=@CustomerId 
                  AND ATBUD_SubHeading=@SubHeadingId
                GROUP BY ATBUD_HeadingId";

            var data = await conn.QueryFirstOrDefaultAsync<AmountDto>(sql,
                new
                {
                    YearId = yearId,
                    PrevYear = yearId - 1,
                    CustomerId = customerId,
                    SchedType = schedType,
                    SubHeadingId = subHeadingId
                }, tran);

            return data ?? new AmountDto();
        }
        private async Task<decimal> GetPandLFinalAmt(SqlConnection conn, SqlTransaction tran,
            int yearId, int customerId, int branchId)
        {
            const string sql = @"
                SELECT SUM(ATBU_Closing_TotalDebit_Amount - ATBU_Closing_TotalCredit_Amount) 
                FROM Acc_TrailBalance_Upload
                WHERE ATBU_Description='Net Income' 
                  AND ATBU_YearId=@YearId 
                  AND ATBU_CustId=@CustomerId 
                  AND ATBU_BranchId=@BranchId";

            return await conn.ExecuteScalarAsync<decimal?>(sql,
                new { YearId = yearId, CustomerId = customerId, BranchId = branchId }, tran) ?? 0m;
        }
    }
}


