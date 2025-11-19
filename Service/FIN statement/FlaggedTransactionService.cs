using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.FlaggedTransactionDto;
using static TracePca.Dto.FIN_Statement.ScheduleMastersDto;
using static TracePca.Dto.FIN_Statement.SelectedPartiesDto;

namespace TracePca.Service.FIN_statement
{
    public class FlaggedTransactionService : FlaggedTransactionInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FlaggedTransactionService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
        }

        //GetDiferenceAmountStatus
        public async Task<IEnumerable<GetDiferenceAmountStatusDto>> GetDiferenceAmountStatusAsync(int CompId, int CustId, int BranchId, int YearId)
        {
            //Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            //Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            //Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT a.ATBU_ID as ATBU_ID ,a.ATBU_STATUS as ATBU_STATUS,a.ATBU_Description as ATBU_Description,
          a.ATBU_Closing_TotalDebit_Amount as ATBU_Closing_Debit_Amount,a.ATBU_Closing_TotalCredit_Amount as ATBU_Closing_Credit_Amount,
          b.ATBU_Closing_TotalDebit_Amount as PrevATBU_Closing_Debit_Amount,b.ATBU_Closing_TotalCredit_Amount as PrevATBU_Closing_Credit_Amount
        FROM Acc_TrailBalance_Upload a
        Left Join Acc_TrailBalance_Upload b on b.ATBU_Description = a.ATBU_Description AND b.ATBU_CustId = @CustId AND b.ATBU_YEARId = @PrevYearId 
        WHERE a.ATBU_CompId = @CompId AND a.ATBU_CustId = @CustId AND a.ATBU_YEARId = @YearId  AND a.ATBU_STATUS = 'D'";

            return await connection.QueryAsync<GetDiferenceAmountStatusDto>(query, new { CompId = CompId, CustId = CustId, BranchId = BranchId, YearId = YearId, PrevYearId = YearId - 1 });
        }

        //GetAbnormalEntriesSeqReferenceNum
        public async Task<IEnumerable<GetGetAbnormalEntriesSeqReferenceNumDto>> GetAbnormalEntriesSeqReferenceNumAsync(int CompId, int CustId, int BranchId, int YearId)
        {
            //Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            //Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            //Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT 
            AJTB_ID,
            AJTB_Status,
            AJTB_DescName,
            AJTB_Debit,
            AJTB_Credit
        FROM Acc_JETransactions_Details
        WHERE AJTB_CompID = @CompId
              AND AJTB_CustId = @CustId
              AND AJTB_BranchId = @BranchId
              AND AJTB_YearID = @YearId
              AND AJTB_Status = 'E'";

            return await connection.QueryAsync<GetGetAbnormalEntriesSeqReferenceNumDto>(query, new { CompId = CompId, CustId = CustId, BranchId = BranchId, YearId = YearId });
        }

        //GetSelectedPartiesSeqReferenceNum
        public async Task<IEnumerable<GetSelectedPartiesSeqReferenceNumDto>> GetSelectedPartiesSeqReferenceNumAsync(int CompId, int CustId, int BranchId, int YearId)
        {
            //Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            //Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            //Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT 
            AJTB_ID,
            AJTB_SeqReferenceNum,
            AJTB_DescName,
            AJTB_Debit,
            AJTB_Credit
        FROM Acc_JETransactions_Details
        WHERE AJTB_CompID = @CompId
              AND AJTB_CustId = @CustId
              AND AJTB_BranchId = @BranchId
              AND AJTB_YearID = @YearId
              AND AJTB_SeqReferenceNum = 1";

            return await connection.QueryAsync<GetSelectedPartiesSeqReferenceNumDto>(query, new { CompId = CompId, CustId = CustId, BranchId = BranchId, YearId = YearId });
        }

        //GetSystemSamplingStatus
        public async Task<IEnumerable<GetSystemSamplingStatusDto>> GetSystemSamplingStatusAsync(int CompId, int CustId, int BranchId, int YearId)
        {
            //Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            //Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            //Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
 SELECT 
     AJTB_ID,
     AJTB_SyStatus,
     AJTB_DescName,
     AJTB_Debit,
     AJTB_Credit
 FROM Acc_JETransactions_Details
 WHERE AJTB_CompID = @CompId
       AND AJTB_CustId = @CustId
       AND AJTB_BranchId = @BranchId
       AND AJTB_YearID = @YearId
       AND AJTB_SyStatus = 'Sy'";

            return await connection.QueryAsync<GetSystemSamplingStatusDto>(query, new { CompId = CompId, CustId = CustId, BranchId = BranchId, YearId = YearId });
        }

        //GetStatifiedSampingStatus
        public async Task<IEnumerable<GetStatifiedSampingStatusDto>> GetStatifiedSampingStatusAsync(int CompId, int CustId, int BranchId, int YearId)
        {
            //Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            //Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            //Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
 SELECT 
     AJTB_ID,
     AJTB_SfStatus,
     AJTB_DescName,
     AJTB_Debit,
     AJTB_Credit
 FROM Acc_JETransactions_Details
 WHERE AJTB_CompID = @CompId
       AND AJTB_CustId = @CustId
       AND AJTB_BranchId = @BranchId
       AND AJTB_YearID = @YearId
       AND AJTB_SfStatus = 'Sf'";

            return await connection.QueryAsync<GetStatifiedSampingStatusDto>(query, new { CompId = CompId, CustId = CustId, BranchId = BranchId, YearId = YearId });
        }
    }
}
