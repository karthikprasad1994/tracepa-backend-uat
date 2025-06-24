using System.Data;
using System.Data.Common;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TracePca.Data;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.JournalEntryDto;
using static TracePca.Service.FIN_statement.JournalEntryService;

namespace TracePca.Service.FIN_statement
{
    public class JournalEntryService : JournalEntryInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
      

        public JournalEntryService(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }

        //GetCustomersName
        public async Task<IEnumerable<CustDto>> GetCustomerNameAsync(int icompId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            Cust_Id,
            Cust_Name 
        FROM SAD_CUSTOMER_MASTER
        WHERE cust_Compid = @CompID";

            await connection.OpenAsync();

            return await connection.QueryAsync<CustDto>(query, new { CompID = icompId });
        }

        //GetFinancialYear
        public async Task<IEnumerable<FinancialYearDto>> GetFinancialYearAsync(int icompId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            YMS_YEARID AS YearId,
            YMS_ID AS Id
        FROM YEAR_MASTER 
        WHERE YMS_FROMDATE < DATEADD(year, +1, GETDATE()) 
          AND YMS_CompId = @CompID 
        ORDER BY YMS_ID DESC";

            await connection.OpenAsync();

            return await connection.QueryAsync<FinancialYearDto>(query, new { CompID = icompId });
        }

        //GetDuration
        public async Task<int?> GetCustomerDurationIdAsync(int compId, int custId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var query = "SELECT Cust_DurtnId FROM SAD_CUSTOMER_MASTER WHERE CUST_CompID = @CompId AND CUST_ID = @CustId";

            var parameters = new { CompId = compId, CustId = custId };
            var result = await connection.QueryFirstOrDefaultAsync<int?>(query, parameters);

            return result;
        }

        //GetBranchName
        public async Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(int compId, int custId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            Mas_Id AS Branchid, 
            Mas_Description AS BranchName 
        FROM SAD_CUST_LOCATION 
        WHERE Mas_CompID = @compId AND Mas_CustID = @custId";

            await connection.OpenAsync();

            return await connection.QueryAsync<CustBranchDto>(query, new { compId, custId });
        }

        //GetJournalEntryInformation
        public async Task<IEnumerable<JournalEntryInformationDto>> GetJournalEntryInformationAsync(
            int compId, int userId, string status, int custId, int yearId, int branchId, string dateFormat, int durationId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var statusFilter = status switch
            {
                "0" => "A",
                "1" => "D",
                "2" => "W",
                _ => null
            };

            var sql = new StringBuilder();
            sql.Append(@"
        SELECT 
            Acc_JE_ID AS Id,
            Acc_JE_TransactionNo AS TransactionNo,
            acc_JE_BranchId AS BranchID,
            '' AS BillNo,
            FORMAT(Acc_JE_BillDate, @dateFormat) AS BillDate,
            Acc_JE_BillType,
            Acc_JE_Party AS PartyID,
            Acc_JE_Status AS Status
        FROM Acc_JE_Master
        WHERE Acc_JE_Party = @custId 
            AND Acc_JE_CompID = @compId 
            AND Acc_JE_YearId = @yearId");

            if (!string.IsNullOrEmpty(statusFilter))
                sql.Append(" AND Acc_JE_Status = @statusFilter");

            if (branchId != 0)
                sql.Append(" AND acc_je_BranchID = @branchId");

            if (durationId != 0)
                sql.Append(" AND Acc_JE_QuarterId = @durationId");

            sql.Append(" ORDER BY Acc_JE_ID ASC");

            var entries = (await connection.QueryAsync<JournalEntryInformationDto>(
                sql.ToString(),
                new { compId, custId, yearId, branchId, durationId, statusFilter, dateFormat }
            )).ToList();

            foreach (var entry in entries)
            {

                // Get Debit/Credit details
                var detailQuery = @"
            SELECT 
                SUM(AJTB_Debit) AS Debit,
                SUM(AJTB_Credit) AS Credit,
                AJTB_DescName,
                AJTB_Debit AS LineDebit
            FROM Acc_JETransactions_Details 
            WHERE Ajtb_Masid = @entryId AND AJTB_CustId = @custId
            GROUP BY AJTB_DescName, AJTB_Debit";

                var details = await connection.QueryAsync(detailQuery, new { entryId = entry.Id, custId });

                var debDescriptions = new List<string>();
                var credDescriptions = new List<string>();
                decimal totalDebit = 0, totalCredit = 0;

                foreach (var row in details)
                {
                    if ((decimal)row.LineDebit != 0)
                    {
                        totalDebit += row.Debit ?? 0;
                        debDescriptions.Add(row.AJTB_DescName);
                    }
                    else
                    {
                        totalCredit += row.Credit ?? 0;
                        credDescriptions.Add(row.AJTB_DescName);
                    }
                }

                entry.Debit = totalDebit;
                entry.Credit = totalCredit;
                entry.DebDescription = string.Join(", ", debDescriptions);
                entry.CredDescription = string.Join(", ", credDescriptions);

                // Map Bill Type
                entry.BillType = entry.BillType switch
                {
                    "1" => "Payment",
                    "2" => "Receipt",
                    "3" => "Petty Cash",
                    "4" => "Purchase",
                    "5" => "Sales",
                    "6" => "Others",
                    _ => ""
                };

                // Map Status
                entry.Status = entry.Status switch
                {
                    "W" => "Waiting For Approval",
                    "A" => "Activated",
                    "D" => "De-Activated",
                    _ => ""
                };
            }

            return entries;
        }
    }
}

