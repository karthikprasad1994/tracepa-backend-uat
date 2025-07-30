using Dapper;

using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SkiaSharp;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using System.Data;
using System.Data.Common;
using System.Text;
using TracePca.Data;
using TracePca.Dto.Audit;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface;
using TracePca.Interface.Audit;
using TracePca.Interface.FIN_Statement;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static TracePca.Dto.FIN_Statement.ScheduleMappingDto;
using static TracePca.Service.FIN_statement.ScheduleMappingService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
namespace TracePca.Service.FIN_statement
{
    public class ScheduleMappingService : ScheduleMappingInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IWebHostEnvironment _env;
        private readonly SqlConnection _db;

        public ScheduleMappingService(Trdmyus1Context dbcontext, IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }  

        //GetScheduleHeading
        public async Task<IEnumerable<ScheduleHeadingDto>> GetScheduleHeadingAsync(int CompId, int CustId, int ScheduleTypeId)
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

            var query = @"
        SELECT DISTINCT 
            b.ASH_Name AS ASH_Name, 
            b.ASH_ID AS ASH_ID
        FROM ACC_ScheduleTemplates a
        LEFT JOIN ACC_ScheduleHeading b 
            ON b.ASH_ID = a.AST_HeadingID AND b.ASH_STATUS <> 'D'
        WHERE a.AST_CompId = @compId
            AND a.AST_Companytype = @custId
            AND a.AST_Schedule_type = @scheduleTypeId
            AND b.ASH_Name IS NOT NULL 
            AND b.ASH_ID IS NOT NULL";

            return await connection.QueryAsync<ScheduleHeadingDto>(query, new { CompId, CustId, ScheduleTypeId });
        }

        //GetScheduleSub-Heading
        public async Task<IEnumerable<ScheduleSubHeadingDto>> GetScheduleSubHeadingAsync(int CompId, int CustId, int ScheduleTypeId)
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
            var query = @"
        SELECT DISTINCT 
            b.AsSH_Name AS ASSH_Name,
            b.AsSH_ID AS ASSH_ID
        FROM ACC_ScheduleTemplates a
        LEFT JOIN ACC_ScheduleSubHeading b 
            ON b.AsSH_ID = a.AST_SubHeadingID AND b.AsSH_STATUS <> 'D'
        WHERE a.AST_CompId = @compId
          AND a.AST_Companytype = @custId
          AND a.AST_Schedule_type = @scheduleTypeId
          AND b.AsSH_Name IS NOT NULL 
          AND b.AsSH_ID IS NOT NULL";

            return await connection.QueryAsync<ScheduleSubHeadingDto>(query, new { CompId, CustId, ScheduleTypeId });
        }

        //GetScheduleItem
        public async Task<IEnumerable<ScheduleItemDto>> GetScheduleItemAsync(int CompId, int CustId, int ScheduleTypeId)
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
            var query = @"
        SELECT DISTINCT 
            b.ASI_ID AS ASI_ID,
            b.ASI_Name AS ASI_Name
        FROM ACC_ScheduleTemplates a
        LEFT JOIN ACC_ScheduleItems b 
            ON b.ASI_ID = a.AST_ItemID AND b.ASI_STATUS <> 'D'
        WHERE a.AST_CompId = @compId
            AND a.AST_Companytype = @custId
            AND a.AST_Schedule_type = @scheduleTypeId
            AND b.ASI_Name IS NOT NULL 
            AND b.ASI_ID IS NOT NULL";

            return await connection.QueryAsync<ScheduleItemDto>(query, new { CompId, CustId, ScheduleTypeId });
        }

        //GetScheduleSub-Item
        public async Task<IEnumerable<ScheduleSubItemDto>> GetScheduleSubItemAsync(int CompId, int CustId, int ScheduleTypeId)
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
            var query = @"
        SELECT DISTINCT 
            b.ASSI_ID AS ASSI_ID,
            b.ASSI_Name AS ASSI_Name
        FROM ACC_ScheduleTemplates a
        LEFT JOIN ACC_ScheduleSubItems b 
            ON b.ASSI_ID = a.AST_SubItemID AND b.ASSI_STATUS <> 'D'
        WHERE a.AST_CompId = @compId
            AND a.AST_Companytype = @custId
            AND a.AST_Schedule_type = @scheduleTypeId
            AND b.ASSI_Name IS NOT NULL 
            AND b.ASSI_ID IS NOT NULL";

            return await connection.QueryAsync<ScheduleSubItemDto>(query, new {CompId, CustId, ScheduleTypeId });
        }

        //GetTotalAmount
        public async Task<IEnumerable<CustCOASummaryDto>>  GetCustCOAMasterDetailsAsync(int CompId, int CustId, int YearId, int BranchId, int DurationId)
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
            var query = @"
SELECT 
    ROUND(SUM(a.ATBU_Opening_Debit_Amount), 0) AS OpeningDebit,
    ROUND(SUM(a.ATBU_Opening_Credit_Amount), 0) AS OpeningCredit,
    ROUND(SUM(a.ATBU_TR_Debit_Amount + ISNULL(g.TotalDebit, 0) + ISNULL(i.TotalDebit, 0)), 0) AS TrDebit,
    ROUND(SUM(a.ATBU_TR_Credit_Amount + ISNULL(g.TotalCredit, 0) + ISNULL(i.TotalCredit, 0)), 0) AS TrCredit,
    ROUND(SUM(a.ATBU_Closing_TotalCredit_Amount), 0) AS ClosingCredit,
    ROUND(SUM(a.ATBU_Closing_TotalDebit_Amount), 0) AS ClosingDebit
FROM Acc_TrailBalance_Upload a
LEFT JOIN (
    SELECT 
        AJTB_DescName,
        SUM(AJTB_Debit) AS TotalDebit,
        SUM(AJTB_Credit) AS TotalCredit
    FROM Acc_JETransactions_Details
    WHERE AJTB_Status = 'A'
        AND AJTB_CustId = @custId
        AND AJTB_YearID = @yearId
        AND AJTB_QuarterId = @durationId
        AND AJTB_BranchId = @branchId
    GROUP BY AJTB_DescName
) g ON g.AJTB_DescName = a.ATBU_Description
LEFT JOIN (
    SELECT 
        AJTB_DescName,
        SUM(AJTB_Debit) AS TotalDebit,
        SUM(AJTB_Credit) AS TotalCredit
    FROM Acc_LedgerTransactions_Details
    WHERE AJTB_Status = 'A'
        AND AJTB_CustId = @custId
        AND AJTB_YearID = @yearId
        AND AJTB_QuarterId = @durationId
        AND AJTB_BranchId = @branchId
    GROUP BY AJTB_DescName
) i ON i.AJTB_DescName = a.ATBU_Description
WHERE 
    a.ATBU_CustId = @custId
    AND a.ATBU_CompId = @compId
    AND a.ATBU_YearId = @yearId
    AND a.ATBU_BranchId = @branchId
    AND a.ATBU_QuarterId = @durationId
    AND a.ATBU_Description <> 'Net income';";

            return await connection.QueryAsync<CustCOASummaryDto>(query, new{ CompId, CustId, YearId, BranchId, DurationId});
        }

        //GetTrailBalance(Grid)
        public async Task<IEnumerable<CustCOADetailsDto>> GetCustCOADetailsAsync(int CompId, int CustId, int YearId, int ScheduleTypeId, int Unmapped, int BranchId, int DurationId)
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
            var query = @"
SELECT 
    ROW_NUMBER() OVER (ORDER BY ATBU_ID ASC) AS SrNo,
    Atbu_id AS DescDetailsID,
    b.atbud_progress AS Status,
    b.ATBUD_SChedule_Type AS ScheduleType,
    b.ATBUD_ID AS DescID,
    ATBU_Code AS DescriptionCode,
    ATBU_Description AS Description,
    CAST(ATBU_Opening_Debit_Amount AS DECIMAL(19, 2)) AS OpeningDebit,
    CAST(ATBU_Opening_Credit_Amount AS DECIMAL(19, 2)) AS OpeningCredit,
    CAST(SUM(ATBU_TR_Debit_Amount + ISNULL(g.TotalDebit, 0) + ISNULL(i.TotalDebit, 0)) AS DECIMAL(19, 2)) AS TrDebit,
    CAST(SUM(ATBU_TR_Credit_Amount + ISNULL(h.TotalCredit, 0) + ISNULL(i.TotalCredit, 0)) AS DECIMAL(19, 2)) AS TrCredit,
    CAST(ATBU_Closing_TotalDebit_Amount AS DECIMAL(19, 2)) AS ClosingDebit,
    CAST(ATBU_Closing_TotalCredit_Amount AS DECIMAL(19, 2)) AS ClosingCredit,
    ISNULL(b.ATBUD_SubItemId, 0) AS SubItemID,
    ASSI_Name,
    ISNULL(b.ATBUD_ItemId, 0) AS ItemID,
    ASI_Name,
    ISNULL(b.ATBUD_SubHeading, 0) AS SubHeadingID,
    ASSH_Name,
    ISNULL(b.ATBUD_HeadingId, 0) AS HeadingID,
    ASH_Name,
    CAST(ATBU_TR_Debit_Amount AS DECIMAL(19, 2)) AS TrDebittrUploaded,
    CAST(ATBU_TR_Credit_Amount AS DECIMAL(19, 2)) AS TrCredittrUploaded
FROM Acc_TrailBalance_Upload a
LEFT JOIN Acc_TrailBalance_Upload_details b
    ON b.ATBUD_Description = a.ATBU_Description
    AND b.ATBUD_CustId = @custId
    AND b.ATBUD_YEARId = @yearId
    AND b.ATBUD_Branchnameid = @branchId
    AND b.ATBUD_QuarterId = @durationId
LEFT JOIN ACC_ScheduleHeading c ON c.ASH_ID = b.ATBUD_HeadingId
LEFT JOIN ACC_ScheduleSubHeading d ON d.ASSH_ID = b.ATBUD_SubHeading
LEFT JOIN ACC_ScheduleItems e ON e.ASI_ID = b.ATBUD_ItemId
LEFT JOIN ACC_ScheduleSubItems f ON f.ASSI_ID = b.ATBUD_SubItemId
LEFT JOIN (
    SELECT AJTB_DescName, SUM(AJTB_Debit) AS TotalDebit
    FROM Acc_JETransactions_Details
    WHERE AJTB_Status = 'A'
        AND AJTB_CustId = @custId
        AND AJTB_YearID = @yearId
        AND AJTB_QuarterId = @durationId
        AND AJTB_BranchId = @branchId
        AND AJTB_Credit = 0
    GROUP BY AJTB_DescName
) g ON g.AJTB_DescName = a.ATBU_Description
LEFT JOIN (
    SELECT AJTB_DescName, SUM(AJTB_Credit) AS TotalCredit
    FROM Acc_JETransactions_Details
    WHERE AJTB_Status = 'A'
        AND AJTB_CustId = @custId
        AND AJTB_YearID = @yearId
        AND AJTB_QuarterId = @durationId
        AND AJTB_BranchId = @branchId
        AND AJTB_Debit = 0
    GROUP BY AJTB_DescName
) h ON h.AJTB_DescName = a.ATBU_Description
LEFT JOIN (
    SELECT AJTB_DescName, SUM(AJTB_Debit) AS TotalDebit, SUM(AJTB_Credit) AS TotalCredit
    FROM Acc_LedgerTransactions_Details
    WHERE AJTB_Status = 'A'
        AND AJTB_CustId = @custId
        AND AJTB_YearID = @yearId
        AND AJTB_QuarterId = @durationId
        AND AJTB_BranchId = @branchId
    GROUP BY AJTB_DescName
) i ON i.AJTB_DescName = a.ATBU_Description
WHERE a.ATBU_CustId = @custId
    AND a.ATBU_CompId = @compId
    AND a.ATBU_YEARId = @yearId
    AND a.ATBU_BranchId = @branchId
    AND a.ATBU_QuarterId = @durationId
    " + (Unmapped != 0 ? "AND ATBUD_Headingid = 0 AND ATBUD_Subheading = 0 AND ATBUD_itemid = 0 AND ATBUD_SubItemId = 0" : "") + @"
GROUP BY b.ATBUD_ID, a.ATBU_ID, a.ATBU_Code, a.ATBU_CustId, a.ATBU_Description, a.ATBU_Opening_Debit_Amount,
         a.ATBU_Opening_Credit_Amount, a.ATBU_TR_Debit_Amount, a.ATBU_TR_Credit_Amount,
         a.ATBU_Closing_TotalDebit_Amount, a.ATBU_Closing_TotalCredit_Amount,
         b.ATBUD_SubItemId, b.ATBUD_ItemId, ASI_Name, b.ATBUD_SubHeading,
         ASSH_Name, b.atbud_progress, b.ATBUD_HeadingId, ASH_Name,
         b.ATBUD_SChedule_Type, ASSI_Name
ORDER BY ATBU_ID;";

            return await connection.QueryAsync<CustCOADetailsDto>(query, new
            {CompId, CustId, YearId, ScheduleTypeId, Unmapped, BranchId, DurationId });
        }

        //FreezeForPreviousDuration
        public async Task<int[]> FreezePreviousYearTrialBalanceAsync(FreezePreviousDurationRequestDto input)
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

            using var transaction = connection.BeginTransaction();
            var detailIds = new List<int>();

            try
            {
                var previousYearId = input.YearId - 1;

                foreach (var item in input.ScheduleItems)
                {
                    // Step 1: Get ATBU_ID
                    var query = @"
SELECT ISNULL(atbu_id, 0)
FROM Acc_TrailBalance_Upload
WHERE ATBU_Description = @AtbuDescription
  AND ATBU_CustId = @AtbuCustId
  AND ATBU_Branchid = @AtbuBranchId
  AND ATBU_CompId = @AtbuCompId
  AND ATBU_YEARId = @AtbuYearId
  AND (@AtbuQuarterId = 0 OR ATBU_QuarterId = @AtbuQuarterId)";

                    int atbuId = await connection.ExecuteScalarAsync<int>(
                        query,
                        new
                        {
                            AtbuDescription = input.AtbuDescription,
                            AtbuCustId = input.AtbuCustId,
                            AtbuBranchId = input.AtbuBranchId,
                            AtbuCompId = input.AtbuCompId,
                            AtbuYearId = previousYearId,
                            AtbuQuarterId = input.AtbuQuarterId
                        },
                        transaction);

                    // Step 2: Get ATBUD_ID
                    var detailQuery = @"
SELECT ISNULL(atbud_id, 0)
FROM Acc_TrailBalance_Upload_Details
WHERE ATBUD_Description = @AtbudDescription
  AND ATBUD_CustId = @AtbudCustId
  AND Atbud_Branchnameid = @AtbudBranchId
  AND ATBUD_CompId = @AtbudCompId
  AND ATBUD_YEARId = @AtbudYearId
  AND (@AtbudQuarterId = 0 OR ATBUD_QuarterId = @AtbudQuarterId)";

                    int atbudId = await connection.ExecuteScalarAsync<int>(
                        detailQuery,
                        new
                        {
                            AtbudDescription = item.AtbudDescription,
                            AtbudCustId = item.AtbudCustId,
                            AtbudBranchId = item.AtbudBranchId,
                            AtbudCompId = item.AtbudCompId,
                            AtbudYearId = previousYearId,
                            AtbudQuarterId = item.AtbudQuarterId
                        },
                        transaction);

                    // Step 3: Call master stored procedure
                    var masterParams = new DynamicParameters();
                    masterParams.Add("@ATBU_ID", atbuId);
                    masterParams.Add("@ATBU_CODE", input.AtbuCode ?? string.Empty);
                    masterParams.Add("@ATBU_Description", input.AtbuDescription ?? string.Empty);
                    masterParams.Add("@ATBU_CustId", input.AtbuCustId);
                    masterParams.Add("@ATBU_Opening_Debit_Amount", input.OpeningDebitAmount);
                    masterParams.Add("@ATBU_Opening_Credit_Amount", input.OpeningCreditAmount);
                    masterParams.Add("@ATBU_TR_Debit_Amount", input.TrDebitAmount);
                    masterParams.Add("@ATBU_TR_Credit_Amount", input.TrCreditAmount);
                    masterParams.Add("@ATBU_Closing_Debit_Amount", input.ClosingDebitAmount);
                    masterParams.Add("@ATBU_Closing_Credit_Amount", input.ClosingCreditAmount);
                    masterParams.Add("@ATBU_DELFLG", input.AtbuDelflg ?? string.Empty);
                    masterParams.Add("@ATBU_CRBY", input.AtbuCrBy);
                    masterParams.Add("@ATBU_STATUS", input.AtbuStatus ?? string.Empty);
                    masterParams.Add("@ATBU_UPDATEDBY", input.AtbuUpdatedBy);
                    masterParams.Add("@ATBU_IPAddress", input.AtbuIpAddress ?? string.Empty);
                    masterParams.Add("@ATBU_CompId", input.AtbuCompId);
                    masterParams.Add("@ATBU_YEARId", previousYearId);
                    masterParams.Add("@ATBU_Branchid", input.AtbuBranchId);
                    masterParams.Add("@ATBU_QuarterId", input.AtbuQuarterId);
                    masterParams.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    masterParams.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync("spAcc_TrailBalance_Upload", masterParams, transaction, commandType: CommandType.StoredProcedure);

                    // Step 4: Call detail stored procedure
                    // Step 4: Call detail stored procedure
                    var detailParams = new DynamicParameters();
                    detailParams.Add("@ATBUD_ID", atbudId);
                    detailParams.Add("@ATBUD_Masid", item.AtbudMasid);
                    detailParams.Add("@ATBUD_CODE", item.AtbudCode ?? string.Empty);
                    detailParams.Add("@ATBUD_Description", item.AtbudDescription ?? string.Empty);
                    detailParams.Add("@ATBUD_CustId", item.AtbudCustId);
                    detailParams.Add("@ATBUD_SChedule_Type", item.AtbudScheduleType);
                    detailParams.Add("@ATBUD_Branchid", item.AtbudBranchId); // ✅ fixed casing
                    detailParams.Add("@ATBUD_QuarterId", item.AtbudQuarterId);
                    detailParams.Add("@ATBUD_Company_Type", item.AtbudCompanyType);
                    detailParams.Add("@ATBUD_Headingid", item.AtbudHeadingId);
                    detailParams.Add("@ATBUD_Subheading", item.AtbudSubheadingId);
                    detailParams.Add("@ATBUD_itemid", item.AtbudItemId);
                    detailParams.Add("@ATBUD_SubItemId", item.AtbudSubItemId);
                    detailParams.Add("@ATBUD_DELFLG", item.AtbudDelflg ?? string.Empty); // expects string, not bool
                    detailParams.Add("@ATBUD_CRBY", item.AtbudCrBy);
                    detailParams.Add("@ATBUD_UPDATEDBY", item.AtbudUpdatedBy);
                    detailParams.Add("@ATBUD_STATUS", item.AtbudStatus ?? string.Empty);
                    detailParams.Add("@ATBUD_Progress", item.AtbudProgress ?? string.Empty);
                    detailParams.Add("@ATBUD_IPAddress", item.AtbudIpAddress ?? string.Empty);
                    detailParams.Add("@ATBUD_CompId", item.AtbudCompId);
                    detailParams.Add("@ATBUD_YEARId", previousYearId);
                    detailParams.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    detailParams.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync(
                        "spAcc_TrailBalance_Upload_Details",
                        detailParams,
                        transaction,
                        commandType: CommandType.StoredProcedure
                    );


                    int resultId = detailParams.Get<int>("@iOper");
                    detailIds.Add(resultId);
                }
                transaction.Commit();
                return detailIds.ToArray();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //FreezeForNextDuration
        public async Task<int[]> FreezeNextDurationrialBalanceAsync(FreezeNextDurationRequestDto input)
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

            using var transaction = connection.BeginTransaction();
            var detailIds = new List<int>();

            try
            {
                var previousYearId = input.YearId + 1;

                foreach (var item in input.ScheduleItems)
                {
                    // Step 1: Get ATBU_ID
                    var query = @"
SELECT ISNULL(atbu_id, 0)
FROM Acc_TrailBalance_Upload
WHERE ATBU_Description = @AtbuDescription
  AND ATBU_CustId = @AtbuCustId
  AND ATBU_Branchid = @AtbuBranchId
  AND ATBU_CompId = @AtbuCompId
  AND ATBU_YEARId = @AtbuYearId
  AND (@AtbuQuarterId = 0 OR ATBU_QuarterId = @AtbuQuarterId)";

                    int atbuId = await connection.ExecuteScalarAsync<int>(
                        query,
                        new
                        {
                            AtbuDescription = input.AtbuDescription,
                            AtbuCustId = input.AtbuCustId,
                            AtbuBranchId = input.AtbuBranchId,
                            AtbuCompId = input.AtbuCompId,
                            AtbuYearId = previousYearId,
                            AtbuQuarterId = input.AtbuQuarterId
                        },
                        transaction);

                    // Step 2: Get ATBUD_ID
                    var detailQuery = @"
SELECT ISNULL(atbud_id, 0)
FROM Acc_TrailBalance_Upload_Details
WHERE ATBUD_Description = @AtbudDescription
  AND ATBUD_CustId = @AtbudCustId
  AND Atbud_Branchnameid = @AtbudBranchId
  AND ATBUD_CompId = @AtbudCompId
  AND ATBUD_YEARId = @AtbudYearId
  AND (@AtbudQuarterId = 0 OR ATBUD_QuarterId = @AtbudQuarterId)";

                    int atbudId = await connection.ExecuteScalarAsync<int>(
                        detailQuery,
                        new
                        {
                            AtbudDescription = item.AtbudDescription,
                            AtbudCustId = item.AtbudCustId,
                            AtbudBranchId = item.AtbudBranchId,
                            AtbudCompId = item.AtbudCompId,
                            AtbudYearId = previousYearId,
                            AtbudQuarterId = item.AtbudQuarterId
                        },
                        transaction);

                    // Step 3: Call master stored procedure
                    var masterParams = new DynamicParameters();
                    masterParams.Add("@ATBU_ID", atbuId);
                    masterParams.Add("@ATBU_CODE", input.AtbuCode ?? string.Empty);
                    masterParams.Add("@ATBU_Description", input.AtbuDescription ?? string.Empty);
                    masterParams.Add("@ATBU_CustId", input.AtbuCustId);
                    masterParams.Add("@ATBU_Opening_Debit_Amount", input.OpeningDebitAmount);
                    masterParams.Add("@ATBU_Opening_Credit_Amount", input.OpeningCreditAmount);
                    masterParams.Add("@ATBU_TR_Debit_Amount", input.TrDebitAmount);
                    masterParams.Add("@ATBU_TR_Credit_Amount", input.TrCreditAmount);
                    masterParams.Add("@ATBU_Closing_Debit_Amount", input.ClosingDebitAmount);
                    masterParams.Add("@ATBU_Closing_Credit_Amount", input.ClosingCreditAmount);
                    masterParams.Add("@ATBU_DELFLG", input.AtbuDelflg ?? string.Empty);
                    masterParams.Add("@ATBU_CRBY", input.AtbuCrBy);
                    masterParams.Add("@ATBU_STATUS", input.AtbuStatus ?? string.Empty);
                    masterParams.Add("@ATBU_UPDATEDBY", input.AtbuUpdatedBy);
                    masterParams.Add("@ATBU_IPAddress", input.AtbuIpAddress ?? string.Empty);
                    masterParams.Add("@ATBU_CompId", input.AtbuCompId);
                    masterParams.Add("@ATBU_YEARId", previousYearId);
                    masterParams.Add("@ATBU_Branchid", input.AtbuBranchId);
                    masterParams.Add("@ATBU_QuarterId", input.AtbuQuarterId);
                    masterParams.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    masterParams.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync("spAcc_TrailBalance_Upload", masterParams, transaction, commandType: CommandType.StoredProcedure);

                    // Step 4: Call detail stored procedure
                    // Step 4: Call detail stored procedure
                    var detailParams = new DynamicParameters();
                    detailParams.Add("@ATBUD_ID", atbudId);
                    detailParams.Add("@ATBUD_Masid", item.AtbudMasid);
                    detailParams.Add("@ATBUD_CODE", item.AtbudCode ?? string.Empty);
                    detailParams.Add("@ATBUD_Description", item.AtbudDescription ?? string.Empty);
                    detailParams.Add("@ATBUD_CustId", item.AtbudCustId);
                    detailParams.Add("@ATBUD_SChedule_Type", item.AtbudScheduleType);
                    detailParams.Add("@ATBUD_Branchid", item.AtbudBranchId); // ✅ fixed casing
                    detailParams.Add("@ATBUD_QuarterId", item.AtbudQuarterId);
                    detailParams.Add("@ATBUD_Company_Type", item.AtbudCompanyType);
                    detailParams.Add("@ATBUD_Headingid", item.AtbudHeadingId);
                    detailParams.Add("@ATBUD_Subheading", item.AtbudSubheadingId);
                    detailParams.Add("@ATBUD_itemid", item.AtbudItemId);
                    detailParams.Add("@ATBUD_SubItemId", item.AtbudSubItemId);
                    detailParams.Add("@ATBUD_DELFLG", item.AtbudDelflg ?? string.Empty); // expects string, not bool
                    detailParams.Add("@ATBUD_CRBY", item.AtbudCrBy);
                    detailParams.Add("@ATBUD_UPDATEDBY", item.AtbudUpdatedBy);
                    detailParams.Add("@ATBUD_STATUS", item.AtbudStatus ?? string.Empty);
                    detailParams.Add("@ATBUD_Progress", item.AtbudProgress ?? string.Empty);
                    detailParams.Add("@ATBUD_IPAddress", item.AtbudIpAddress ?? string.Empty);
                    detailParams.Add("@ATBUD_CompId", item.AtbudCompId);
                    detailParams.Add("@ATBUD_YEARId", previousYearId);
                    detailParams.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    detailParams.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync(
                        "spAcc_TrailBalance_Upload_Details",
                        detailParams,
                        transaction,
                        commandType: CommandType.StoredProcedure
                    );


                    int resultId = detailParams.Get<int>("@iOper");
                    detailIds.Add(resultId);
                }

                transaction.Commit();
                return detailIds.ToArray();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //DownloadUploadableExcelAndTemplate
        public FileDownloadResult GetExcelTemplate()
        {
            var filePath = "C:\\Users\\SSD\\Desktop\\TracePa\\tracepa-dotnet-core\\SampleExcels\\SampleTrailBalExcel.xlsx";

            if (!File.Exists(filePath))
                return new FileDownloadResult();

            var bytes = File.ReadAllBytes(filePath);
            var fileName = "SampleTrailBalExcel.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return new FileDownloadResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        //SaveTrailBalnceDetails
        public async Task<int[]> SaveTrailBalanceDetailsAsync(int CompId, List<TrailBalanceDetailsDto> dtos)
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
            using var transaction = connection.BeginTransaction();

            var insertedIds = new List<int>();

            try
            {
                foreach (var dto in dtos)
                {
                    int updateOrSave = 0, oper = 0;

                    // Step 1: Resolve schedule mapping IDs from names
                    int subItemId = 0, itemId = 0, subHeadingId = 0, headingId = 0, scheduleType = 0;

                    if (!string.IsNullOrWhiteSpace(dto.Excel_SubItem))
                        subItemId = await GetIdFromNameAsync(connection, transaction, "ACC_ScheduleSubItems", "ASSI_ID", dto.Excel_SubItem, dto.ATBU_CustId);

                    if (!string.IsNullOrWhiteSpace(dto.Excel_Item))
                        itemId = await GetIdFromNameAsync(connection, transaction, "ACC_ScheduleItems", "ASI_ID", dto.Excel_Item, dto.ATBU_CustId);

                    if (!string.IsNullOrWhiteSpace(dto.Excel_SubHeading))
                        subHeadingId = await GetIdFromNameAsync(connection, transaction, "ACC_ScheduleSubHeading", "ASSH_ID", dto.Excel_SubHeading, dto.ATBU_CustId);

                    if (!string.IsNullOrWhiteSpace(dto.Excel_Heading))
                        headingId = await GetIdFromNameAsync(connection, transaction, "ACC_ScheduleHeading", "ASH_ID", dto.Excel_Heading, dto.ATBU_CustId);

                    // Optional: Fetch ScheduleType from template
                    scheduleType = await GetScheduleTypeFromTemplateAsync(connection, transaction, subItemId, itemId, subHeadingId, headingId, dto.ATBUD_Company_Type);

                    // --- Master Insert ---
                    using (var cmdMaster = new SqlCommand("spAcc_TrailBalance_Upload", connection, transaction))
                    {
                        cmdMaster.CommandType = CommandType.StoredProcedure;

                        cmdMaster.Parameters.AddWithValue("@ATBU_ID", dto.ATBU_ID);
                        cmdMaster.Parameters.AddWithValue("@ATBU_CODE", dto.ATBU_CODE ?? string.Empty);
                        cmdMaster.Parameters.AddWithValue("@ATBU_Description", dto.ATBU_Description ?? string.Empty);
                        cmdMaster.Parameters.AddWithValue("@ATBU_CustId", dto.ATBU_CustId);
                        cmdMaster.Parameters.AddWithValue("@ATBU_Opening_Debit_Amount", dto.ATBU_Opening_Debit_Amount);
                        cmdMaster.Parameters.AddWithValue("@ATBU_Opening_Credit_Amount", dto.ATBU_Opening_Credit_Amount);
                        cmdMaster.Parameters.AddWithValue("@ATBU_TR_Debit_Amount", dto.ATBU_TR_Debit_Amount);
                        cmdMaster.Parameters.AddWithValue("@ATBU_TR_Credit_Amount", dto.ATBU_TR_Credit_Amount);
                        cmdMaster.Parameters.AddWithValue("@ATBU_Closing_Debit_Amount", dto.ATBU_Closing_Debit_Amount);
                        cmdMaster.Parameters.AddWithValue("@ATBU_Closing_Credit_Amount", dto.ATBU_Closing_Credit_Amount);
                        cmdMaster.Parameters.AddWithValue("@ATBU_DELFLG", dto.ATBU_DELFLG ?? "A");
                        cmdMaster.Parameters.AddWithValue("@ATBU_CRBY", dto.ATBU_CRBY);
                        cmdMaster.Parameters.AddWithValue("@ATBU_STATUS", dto.ATBU_STATUS ?? "C");
                        cmdMaster.Parameters.AddWithValue("@ATBU_UPDATEDBY", dto.ATBU_UPDATEDBY);
                        cmdMaster.Parameters.AddWithValue("@ATBU_IPAddress", dto.ATBU_IPAddress ?? "127.0.0.1");
                        cmdMaster.Parameters.AddWithValue("@ATBU_CompId", dto.ATBU_CompId);
                        cmdMaster.Parameters.AddWithValue("@ATBU_YEARId", dto.ATBU_YEARId);
                        cmdMaster.Parameters.AddWithValue("@ATBU_Branchid", dto.ATBU_Branchid);
                        cmdMaster.Parameters.AddWithValue("@ATBU_QuarterId", dto.ATBU_QuarterId);

                        var output1 = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var output2 = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        cmdMaster.Parameters.Add(output1);
                        cmdMaster.Parameters.Add(output2);

                        await cmdMaster.ExecuteNonQueryAsync();

                        updateOrSave = (int)(output1.Value ?? 0);
                        oper = (int)(output2.Value ?? 0);
                    }

                    // --- Detail Insert ---
                    using (var cmdDetail = new SqlCommand("spAcc_TrailBalance_Upload_Details", connection, transaction))
                    {
                        cmdDetail.CommandType = CommandType.StoredProcedure;

                        cmdDetail.Parameters.AddWithValue("@ATBUD_ID", dto.ATBUD_ID);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Masid", oper);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_CODE", dto.ATBUD_CODE ?? string.Empty);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Description", dto.ATBUD_Description ?? string.Empty);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_CustId", dto.ATBUD_CustId);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_SChedule_Type", scheduleType);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Branchid", dto.ATBUD_Branchid);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_QuarterId", dto.ATBUD_QuarterId);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Company_Type", dto.ATBUD_Company_Type);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Headingid", headingId);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Subheading", subHeadingId);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_itemid", itemId);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_SubItemid", subItemId);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_DELFLG", dto.ATBUD_DELFLG ?? "A");
                        cmdDetail.Parameters.AddWithValue("@ATBUD_CRBY", dto.ATBUD_CRBY);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_UPDATEDBY", dto.ATBUD_UPDATEDBY);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_STATUS", dto.ATBUD_STATUS ?? "C");
                        cmdDetail.Parameters.AddWithValue("@ATBUD_Progress", dto.ATBUD_Progress ?? "Uploaded");
                        cmdDetail.Parameters.AddWithValue("@ATBUD_IPAddress", dto.ATBUD_IPAddress ?? "127.0.0.1");
                        cmdDetail.Parameters.AddWithValue("@ATBUD_CompId", dto.ATBUD_CompId);
                        cmdDetail.Parameters.AddWithValue("@ATBUD_YEARId", dto.ATBU_YEARId);

                        var output1 = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var output2 = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        cmdDetail.Parameters.Add(output1);
                        cmdDetail.Parameters.Add(output2);

                        await cmdDetail.ExecuteNonQueryAsync();
                        insertedIds.Add((int)(output2.Value ?? 0));
                    }
                }
                transaction.Commit();
                return insertedIds.ToArray();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        private async Task<int> GetIdFromNameAsync(SqlConnection conn, SqlTransaction tran, string table, string column, string name, int orgType)
        {
            string nameCol = column.Replace("_ID", "_Name");
            string orgCol = column.Replace("_ID", "_Orgtype");

            var query = $"SELECT ISNULL({column}, 0) FROM {table} WHERE {nameCol} = @name AND {orgCol} = @orgType";
            using var cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@name", name.Trim());
            cmd.Parameters.AddWithValue("@orgType", orgType);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0);
        }
        private async Task<int> GetScheduleTypeFromTemplateAsync(SqlConnection conn, SqlTransaction tran, int subItemId, int itemId, int subHeadingId, int headingId, int orgType)
        {
            string query = "";

            if (subItemId > 0)
                query = "SELECT ISNULL(AST_Schedule_type, 0) FROM ACC_ScheduleTemplates WHERE AST_SubItemID = @id AND AST_Companytype = @orgType";
            else if (itemId > 0)
                query = "SELECT ISNULL(AST_Schedule_type, 0) FROM ACC_ScheduleTemplates WHERE AST_ItemID = @id AND AST_SubItemID = 0 AND AST_Companytype = @orgType";
            else if (subHeadingId > 0)
                query = "SELECT ISNULL(AST_Schedule_type, 0) FROM ACC_ScheduleTemplates WHERE AST_SubHeadingID = @id AND AST_Companytype = @orgType";
            else if (headingId > 0)
                query = "SELECT ISNULL(AST_Schedule_type, 0) FROM ACC_ScheduleTemplates WHERE AST_HeadingID = @id AND AST_Companytype = @orgType";
            else
                return 0;

            using var cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@id", subItemId > 0 ? subItemId : itemId > 0 ? itemId : subHeadingId > 0 ? subHeadingId : headingId);
            cmd.Parameters.AddWithValue("@orgType", orgType);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0);
        }

        //UpdateTrailBalance
        public async Task<List<int>> UpdateTrailBalanceAsync(List<UpdateTrailBalanceDto> dtos)
        {
            var resultIds = new List<int>();
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var dto in dtos)
                {
                    int iPKId = dto.ATBUD_ID;
                    int updateOrSave, oper;

                    using (var detailsCommand = new SqlCommand("spAcc_TrailBalance_Upload_Details", connection, transaction))
                    {
                        detailsCommand.CommandType = CommandType.StoredProcedure;

                        detailsCommand.Parameters.AddWithValue("@ATBUD_ID", dto.ATBUD_ID);
                        detailsCommand.Parameters.AddWithValue("@ATBUD_Masid", dto.ATBUD_Masid);
                        detailsCommand.Parameters.AddWithValue("@ATBUD_CODE", dto.ATBUD_CODE ?? string.Empty);
                        detailsCommand.Parameters.AddWithValue("@ATBUD_Description", dto.ATBUD_Description ?? string.Empty);
                        detailsCommand.Parameters.AddWithValue("@ATBUD_CustId", dto.ATBUD_CustId);
                        detailsCommand.Parameters.AddWithValue("@ATBUD_SChedule_Type", dto.ATBUD_SChedule_Type);
                        detailsCommand.Parameters.AddWithValue("@ATBUD_Branchid", dto.ATBUD_Branchid);
                        detailsCommand.Parameters.AddWithValue("@ATBUD_QuarterId", dto.ATBUD_QuarterId);
                        detailsCommand.Parameters.AddWithValue("@ATBUD_Company_Type", dto.ATBUD_Company_Type);
                        detailsCommand.Parameters.AddWithValue("@ATBUD_Headingid", dto.ATBUD_Headingid);
                        detailsCommand.Parameters.AddWithValue("@ATBUD_Subheading", dto.ATBUD_Subheading);
                        detailsCommand.Parameters.AddWithValue("@ATBUD_itemid", dto.ATBUD_itemid);
                        detailsCommand.Parameters.AddWithValue("@ATBUD_Subitemid", dto.ATBUD_Subitemid);
                        detailsCommand.Parameters.AddWithValue("@ATBUD_DELFLG", dto.ATBUD_DELFLG ?? "A");
                        detailsCommand.Parameters.AddWithValue("@ATBUD_CRBY", dto.ATBUD_CRBY);
                        detailsCommand.Parameters.AddWithValue("@ATBUD_UPDATEDBY", dto.ATBUD_UPDATEDBY);
                        detailsCommand.Parameters.AddWithValue("@ATBUD_STATUS", dto.ATBUD_STATUS ?? "C");
                        detailsCommand.Parameters.AddWithValue("@ATBUD_Progress", dto.ATBUD_Progress ?? "Uploaded");
                        detailsCommand.Parameters.AddWithValue("@ATBUD_IPAddress", dto.ATBUD_IPAddress ?? string.Empty);
                        detailsCommand.Parameters.AddWithValue("@ATBUD_CompId", dto.ATBUD_CompId);
                        detailsCommand.Parameters.AddWithValue("@ATBUD_YEARId", dto.ATBUD_YEARId);

                        var detailOutput1 = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var detailOutput2 = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        detailsCommand.Parameters.Add(detailOutput1);
                        detailsCommand.Parameters.Add(detailOutput2);

                        await detailsCommand.ExecuteNonQueryAsync();
                    }
                }
                transaction.Commit();
                return resultIds;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error while saving Trail Balance data: " + ex.Message, ex);
            }
        }

        //LoadSubHeadingByHeading
        public async Task<IEnumerable<LoadSubHeadingByHeadingDto>> GetSubHeadingsByHeadingIdAsync(int headingId, int orgType)
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

            var query = @"
SELECT ASSH_ID AS Id, ASSH_Name AS Name 
FROM ACC_ScheduleSubHeading 
WHERE ASSH_HeadingID = @HeadingId 
  AND ASSH_OrgType = @OrgType 
  AND ISNULL(ASSH_DELFLG, 'A') = 'A'";

            var result = await connection.QueryAsync<LoadSubHeadingByHeadingDto>(query, new { HeadingId = headingId, OrgType = orgType });
            return result;
        }

        //LoadItemBySubHeading
        public async Task<IEnumerable<LoadItemBySubHeadingDto>> GetItemsBySubHeadingIdAsync(int subHeadingId, int orgType)
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
            var query = @"
SELECT ASI_ID AS Id, ASI_Name AS Name 
FROM ACC_ScheduleItems 
WHERE ASI_SubHeadingID = @SubHeadingId 
  AND ASI_OrgType = @OrgType 
  AND ISNULL(ASI_DELFLG, 'A') = 'A'";

            var result = await connection.QueryAsync<LoadItemBySubHeadingDto>(query, new { SubHeadingId = subHeadingId, OrgType = orgType });
            return result;
        }

        //LoadSubItemByItem
        public async Task<IEnumerable<LoadSubItemByItemDto>> GetSubItemsByItemIdAsync(int itemId, int orgType)
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
            var query = @"
SELECT ASSI_ID AS Id, ASSI_Name AS Name 
FROM ACC_ScheduleSubItems 
WHERE ASSI_ItemsID = @ItemId 
  AND ASSI_OrgType = @OrgType 
  AND ISNULL(ASSI_DELFLG, 'A') = 'A'";

            var result = await connection.QueryAsync<LoadSubItemByItemDto>(query, new { ItemId = itemId, OrgType = orgType });
            return result;
        }

        //GetPreviousLoadId
        public async Task<(int? HeadingId, int? SubHeadingId, int? ItemId)> GetPreviousLoadIdAsync(int? subItemId = null, int? itemId = null, int? subHeadingId = null)
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
            using var transaction = connection.BeginTransaction();

            try
            {
                int? finalHeadingId = null;
                int? finalSubHeadingId = null;
                int? finalItemId = null;
                int? finalSubItemId = null;

                // If sub-item is selected, get item > sub-heading > heading
                if (subItemId.HasValue && subItemId > 0)
                {
                    var query = @"
              SELECT 
                  si.ASSI_ItemsID AS ItemId,
                  i.ASI_SubHeadingID AS SubHeadingId,
                  sh.ASSH_HeadingID AS HeadingId
              FROM ACC_ScheduleSubItems si
              JOIN ACC_ScheduleItems i ON si.ASSI_ItemsID = i.ASI_ID
              JOIN ACC_ScheduleSubHeading sh ON i.ASI_SubHeadingID = sh.ASSH_ID
              WHERE si.ASSI_ID = @SubItemId";

                    var result = await connection.QueryFirstOrDefaultAsync<(int?, int?, int?)>(
                        query, new { SubItemId = subItemId }, transaction);

                    finalItemId = result.Item1;
                    finalSubHeadingId = result.Item2;
                    finalHeadingId = result.Item3;
                    finalSubItemId = subItemId;
                }
                // If item is selected, get sub-heading > heading
                else if (itemId.HasValue && itemId > 0)
                {
                    var query = @"
              SELECT 
                  ASI_SubHeadingID AS SubHeadingId,
                  (SELECT ASSH_HeadingID FROM ACC_ScheduleSubHeading WHERE ASSH_ID = ASI_SubHeadingID) AS HeadingId
              FROM ACC_ScheduleItems
              WHERE ASI_ID = @ItemId";

                    var result = await connection.QueryFirstOrDefaultAsync<(int?, int?)>(
                        query, new { ItemId = itemId }, transaction);

                    finalSubHeadingId = result.Item1;
                    finalHeadingId = result.Item2;
                    finalItemId = itemId;
                }
                // If sub-heading is selected, get heading
                else if (subHeadingId.HasValue && subHeadingId > 0)
                {
                    var query = @"SELECT ASSH_HeadingID FROM ACC_ScheduleSubHeading WHERE ASSH_ID = @SubHeadingId";

                    finalHeadingId = await connection.ExecuteScalarAsync<int?>(
                        query, new { SubHeadingId = subHeadingId }, transaction);

                    finalSubHeadingId = subHeadingId;
                }
                transaction.Commit();

                return (finalHeadingId, finalSubHeadingId, finalItemId);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}



