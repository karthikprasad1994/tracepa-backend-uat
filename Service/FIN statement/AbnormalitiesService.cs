using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.AbnormalitiesDto;
using static TracePca.Dto.FIN_Statement.SelectedPartiesDto;

namespace TracePca.Service.FIN_statement
{
    public class AbnormalitiesService : AbnormalitiesInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AbnormalitiesService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
        }

        //GetAbnormalTransactions
        public async Task<IEnumerable<AbnormalTransactionsDto>> GetAbnormalTransactionsAsync(int iCustId, int iBranchId, int iYearID, int iAbnormalType, string sAmount)
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
            string sql = string.Empty;

            //Greater Than Average Amount Ratio
            if (iAbnormalType == 1)
            {
                sql = @"
      WITH AvgValues AS (SELECT AJTB_DescName, AVG(ajtb_credit) AS AvgCreditAmt,
                 AVG(ajtb_debit) AS AvgDebitAmt,
                 AVG(ajtb_credit * @sAmount) AS AvgCreditAmtRatio,
                 AVG(ajtb_debit * @sAmount) AS AvgDebitAmtRatio
          FROM Acc_JETransactions_Details
          WHERE Ajtb_Custid = @iCustId AND AJTB_BranchId = @iBranchId AND AJTB_YearID = @iYearID
          GROUP BY AJTB_DescName        )
      SELECT A.AJTB_DescName,AJTB_CreatedOn,AJTB_TranscNo, A.ajtb_credit AS creditAmt, A.ajtb_debit AS DebitAmt,
             V.AvgCreditAmt, V.AvgDebitAmt, V.AvgCreditAmtRatio, V.AvgDebitAmtRatio,
AJTB_AEStatus as status, AJTB_BillType,AJTB_TranscNo,AJTB_CreatedBy,ajtb_id
      FROM Acc_JETransactions_Details A
      JOIN AvgValues V ON A.AJTB_DescName = V.AJTB_DescName
      WHERE A.Ajtb_Custid = @iCustId AND A.AJTB_BranchId = @iBranchId AND A.AJTB_YearID = @iYearID
      AND (A.ajtb_credit > V.AvgCreditAmtRatio OR A.ajtb_debit > V.AvgDebitAmtRatio)";
            }

            //Lesser Than Average Amount Ratio
            else if (iAbnormalType == 2)
            {
                sql = @"
      WITH AvgValues AS ( SELECT AJTB_DescName, AVG(ajtb_credit) AS AvgCreditAmt,
                 AVG(ajtb_debit) AS AvgDebitAmt,
                 AVG(ajtb_credit * @sAmount) AS AvgCreditAmtRatio,
                 AVG(ajtb_debit * @sAmount) AS AvgDebitAmtRatio
          FROM Acc_JETransactions_Details
          WHERE Ajtb_Custid = @iCustId AND AJTB_BranchId = @iBranchId AND AJTB_YearID = @iYearID
          GROUP BY AJTB_DescName     )
      SELECT A.AJTB_DescName, A.ajtb_credit AS creditAmt, A.ajtb_debit AS DebitAmt,
             V.AvgCreditAmt, V.AvgDebitAmt, V.AvgCreditAmtRatio, V.AvgDebitAmtRatio,a.AJTB_status
      FROM Acc_JETransactions_Details A
      JOIN AvgValues V ON A.AJTB_DescName = V.AJTB_DescName
      WHERE A.Ajtb_Custid = @iCustId AND A.AJTB_BranchId = @iBranchId AND A.AJTB_YearID = @iYearID
      AND (A.ajtb_credit < V.AvgCreditAmtRatio OR A.ajtb_debit < V.AvgDebitAmtRatio)";
            }
            else if (iAbnormalType == 3)
            {
                sql = @"  SELECT AJTB_DescName, ajtb_credit, ajtb_debit,AJTB_status
      FROM Acc_JETransactions_Details
      WHERE Ajtb_Custid = @iCustId AND AJTB_BranchId = @iBranchId AND AJTB_YearID = @iYearID
      AND (AJTB_Credit >= @sAmount OR ajtb_debit >= @sAmount)";
            }
            else if (iAbnormalType == 4)
            {
                sql = @" SELECT AJTB_DescName, ajtb_credit, ajtb_debit
      FROM Acc_JETransactions_Details
      WHERE Ajtb_Custid = @iCustId AND AJTB_BranchId = @iBranchId AND AJTB_YearID = @iYearID
      AND ((AJTB_Credit <= @sAmount AND ajtb_credit <> 0) OR (ajtb_debit <= @sAmount AND ajtb_debit <> 0))";
            }

            // ✅ Execute query once
            var result = await connection.QueryAsync<AbnormalTransactionsDto>(sql, new
            {
                iCustId,
                iBranchId,
                iYearID,
                sAmount
            });

            return result;
        }

        //UpdateAEStatus
        public async Task<int> UpdateJournalEntrySeqRefAsync(List<UpdateJournalEntrySeqRef1Dto> dtoList)
        {
            if (dtoList == null || !dtoList.Any())
                return 0; // Nothing to update

            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Step 3: Build dynamic CASE statement
                var caseStatements = dtoList
                    .Select((d, i) => $"WHEN @Id{i} THEN @Status{i}")
                    .ToList();

                var sql = $@"
        UPDATE Acc_JETransactions_Details
        SET AJTB_AEStatus = CASE AJTB_ID
            {string.Join(" ", caseStatements)}
        END
        WHERE AJTB_ID IN ({string.Join(",", dtoList.Select((d, i) => $"@Id{i}"))});";

                // Step 4: Prepare parameters
                var parameters = new DynamicParameters();
                for (int i = 0; i < dtoList.Count; i++)
                {
                    parameters.Add($"Id{i}", dtoList[i].Id);
                    parameters.Add($"Status{i}", dtoList[i].Status);
                }

                // Step 5: Execute query
                var updatedCount = await connection.ExecuteAsync(sql, parameters, transaction);

                transaction.Commit();
                return updatedCount;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}

