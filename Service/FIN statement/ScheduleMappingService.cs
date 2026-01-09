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
using StackExchange.Redis;
using DocumentFormat.OpenXml.Bibliography;
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

            return await connection.QueryAsync<ScheduleSubItemDto>(query, new { CompId, CustId, ScheduleTypeId });
        }

        //GetTotalAmount
        public async Task<IEnumerable<CustCOASummaryDto>> GetCustCOAMasterDetailsAsync(int CompId, int CustId, int YearId, int BranchId, int DurationId)
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
    ROUND(SUM(a.ATBU_TR_Debit_Amount + ISNULL(g.TotalDebit, 0) ), 0) AS TrDebit,
    ROUND(SUM(a.ATBU_TR_Credit_Amount + ISNULL(g.TotalCredit, 0) ), 0) AS TrCredit,
    ROUND(SUM(a.ATBU_Closing_TotalCredit_Amount), 0) AS ClosingCredit,
    ROUND(SUM(a.ATBU_Closing_TotalDebit_Amount), 0) AS ClosingDebit
FROM Acc_TrailBalance_Upload a
LEFT JOIN (
    SELECT 
        AJTB_DescName,
        SUM(AJTB_Debit) AS TotalDebit,
        SUM(AJTB_Credit) AS TotalCredit
    FROM Acc_JETransactions_Details
    WHERE  AJTB_CustId = @custId
        AND AJTB_YearID = @yearId
        AND AJTB_QuarterId = @durationId
        AND AJTB_BranchId = @branchId and AJTB_Status<> 'D'
    GROUP BY AJTB_DescName
) g ON g.AJTB_DescName = a.ATBU_Description
WHERE 
    a.ATBU_CustId = @custId
    AND a.ATBU_CompId = @compId
    AND a.ATBU_YearId = @yearId
    AND a.ATBU_BranchId = @branchId
    AND a.ATBU_Description<>'Net Income' 
    AND a.ATBU_QuarterId = @durationId;";

            return await connection.QueryAsync<CustCOASummaryDto>(query, new { CompId, CustId, YearId, BranchId, DurationId });
        }

        //GetTrailBalance(Grid)
        public async Task<IEnumerable<CustCOADetailsDto>> GetCustCOADetailsAsync(
      int CompId, int CustId, int YearId, int ScheduleTypeId, int Unmapped, int BranchId, int DurationId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

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
    CAST(SUM(ATBU_TR_Debit_Amount + ISNULL(g.TotalDebit, 0) ) AS DECIMAL(19, 2)) AS TrDebit,
    CAST(SUM(ATBU_TR_Credit_Amount + ISNULL(h.TotalCredit, 0) ) AS DECIMAL(19, 2)) AS TrCredit,
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
    WHERE  AJTB_CustId = @custId
        AND AJTB_YearID = @yearId
        AND AJTB_QuarterId = @durationId
        AND AJTB_BranchId = @branchId
        AND AJTB_Credit = 0 and AJTB_Status<> 'D'
    GROUP BY AJTB_DescName
) g ON g.AJTB_DescName = a.ATBU_Description
LEFT JOIN (
    SELECT AJTB_DescName, SUM(AJTB_Credit) AS TotalCredit
    FROM Acc_JETransactions_Details
    WHERE  AJTB_CustId = @custId
        AND AJTB_YearID = @yearId
        AND AJTB_QuarterId = @durationId
        AND AJTB_BranchId = @branchId
        AND AJTB_Debit = 0 and AJTB_Status<> 'D'
    GROUP BY AJTB_DescName
) h ON h.AJTB_DescName = a.ATBU_Description
WHERE a.ATBU_CustId = @custId
    AND a.ATBU_CompId = @compId
    AND a.ATBU_YEARId = @yearId
    AND a.ATBU_BranchId = @branchId
    AND a.ATBU_QuarterId = @durationId
" + (Unmapped == 1 ? @"AND (
        ISNULL(ATBUD_Headingid, 0) = 0 and  
        ISNULL(ATBUD_Subheading, 0) = 0 and  
        ISNULL(ATBUD_itemid, 0) = 0 and 
        ISNULL(ATBUD_SubItemId, 0) = 0
    )" : "") + @"
GROUP BY b.ATBUD_ID, a.ATBU_ID, a.ATBU_Code, a.ATBU_CustId, a.ATBU_Description, a.ATBU_Opening_Debit_Amount,
         a.ATBU_Opening_Credit_Amount, a.ATBU_TR_Debit_Amount, a.ATBU_TR_Credit_Amount,
         a.ATBU_Closing_TotalDebit_Amount, a.ATBU_Closing_TotalCredit_Amount,
         b.ATBUD_SubItemId, b.ATBUD_ItemId, ASI_Name, b.ATBUD_SubHeading,
         ASSH_Name, b.atbud_progress, b.ATBUD_HeadingId, ASH_Name,
         b.ATBUD_SChedule_Type, ASSI_Name
ORDER BY ATBU_ID;";

            return await connection.QueryAsync<CustCOADetailsDto>(query, new
            {
                CompId,
                CustId,
                YearId,
                ScheduleTypeId,
                Unmapped,
                BranchId,
                DurationId
            });

        }

        //FreezeForPreviousDuration
        public async Task<int[]> FreezePreviousDurationTrialBalanceAsync(List<FreezePreviousYearTrialBalanceDto> inputList)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            var detailIds = new List<int>();

            try
            {
                foreach (var record in inputList)
                {
                    int nextYearId = record.AtbU_YEARId + 1;
                    var AtbuPkId = await connection.ExecuteScalarAsync<int>(
                        @"SELECT ISNULL(ATBU_ID, 0)
FROM Acc_TrailBalance_Upload
WHERE ATBU_Custid = @CustId
  AND ATBU_YEARId = @YearId
  AND ISNULL(ATBU_Description, '') = @Description
  AND ISNULL(ATBU_QuarterId, 0) = @QuarterId",
                        new
                        {
                            CustId = record.AtbU_CustId,
                            YearId = nextYearId,
                            Description = record.AtbU_Description ?? string.Empty,
                            QuarterId = record.AtbU_QuarterId
                        },
                        transaction
                    );

                    var AtbudPkId = await connection.ExecuteScalarAsync<int>(
                        @"SELECT ISNULL(ATBUD_ID, 0)
FROM Acc_TrailBalance_Upload_Details
WHERE ATBUD_Custid = @CustId
  AND ATBUD_YEARId = @YearId
  AND ISNULL(ATBUD_Description, '') = @Description
  AND ISNULL(ATBUD_QuarterId, 0) = @QuarterId",
                        new
                        {
                            CustId = record.AtbuD_CustId,
                            YearId = nextYearId,
                            Description = record.AtbuD_Description ?? string.Empty,
                            QuarterId = record.AtbuD_QuarterId
                        },
                        transaction
                    );



                    // ✅ Step 1: Handle ATBU record (master)
                    var atbuParams = new DynamicParameters();
                    atbuParams.Add("@ATBU_ID", AtbuPkId);
                    atbuParams.Add("@ATBU_CODE", record.AtbU_CODE ?? string.Empty);
                    atbuParams.Add("@ATBU_Description", record.AtbU_Description ?? string.Empty);
                    atbuParams.Add("@ATBU_CustId", record.AtbU_CustId);

                    // ✅ Include your original condition logic here
                    if (record.AtbuD_SChedule_Type == 4)
                    {
                        atbuParams.Add("@ATBU_Opening_Debit_Amount", record.AtbU_Closing_Debit_Amount);
                        atbuParams.Add("@ATBU_Opening_Credit_Amount", record.AtbU_Closing_Credit_Amount);
                        atbuParams.Add("@ATBU_TR_Debit_Amount", 0);
                        atbuParams.Add("@ATBU_TR_Credit_Amount", 0);
                        atbuParams.Add("@ATBU_Closing_Debit_Amount", record.AtbU_Closing_Debit_Amount);
                        atbuParams.Add("@ATBU_Closing_Credit_Amount", record.AtbU_Closing_Credit_Amount);
                    }
                    else
                    {
                        atbuParams.Add("@ATBU_Opening_Debit_Amount", 0);
                        atbuParams.Add("@ATBU_Opening_Credit_Amount", 0);
                        atbuParams.Add("@ATBU_TR_Debit_Amount", 0);
                        atbuParams.Add("@ATBU_TR_Credit_Amount", 0);
                        atbuParams.Add("@ATBU_Closing_Debit_Amount", 0);
                        atbuParams.Add("@ATBU_Closing_Credit_Amount", 0);
                    }

                    atbuParams.Add("@ATBU_DELFLG", record.AtbU_DELFLG ?? "N");
                    atbuParams.Add("@ATBU_CRBY", record.AtbU_CRBY);
                    atbuParams.Add("@ATBU_STATUS", record.AtbU_STATUS ?? "Active");
                    atbuParams.Add("@ATBU_UPDATEDBY", record.AtbU_UPDATEDBY);
                    atbuParams.Add("@ATBU_IPAddress", record.AtbU_IPAddress ?? string.Empty);
                    atbuParams.Add("@ATBU_CompId", record.AtbU_CompId);
                    atbuParams.Add("@ATBU_YEARId", nextYearId);
                    atbuParams.Add("@ATBU_Branchid", record.AtbU_Branchid);
                    atbuParams.Add("@ATBU_QuarterId", record.AtbU_QuarterId);
                    atbuParams.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    atbuParams.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync(
                        "spAcc_TrailBalance_Upload",
                        atbuParams,
                        transaction,
                        commandType: CommandType.StoredProcedure
                    );
                    int masId = atbuParams.Get<int>("@iOper");


                    // ✅ Step 2: Handle ATBUD record (detail)
                    var atbudParams = new DynamicParameters();
                    atbudParams.Add("@ATBUD_ID", AtbudPkId);
                    atbudParams.Add("@ATBUD_Masid", masId);
                    atbudParams.Add("@ATBUD_CODE", record.AtbuD_CODE ?? string.Empty);
                    atbudParams.Add("@ATBUD_Description", record.AtbuD_Description ?? string.Empty);
                    atbudParams.Add("@ATBUD_CustId", record.AtbuD_CustId);
                    atbudParams.Add("@ATBUD_SChedule_Type", record.AtbuD_SChedule_Type);
                    atbudParams.Add("@ATBUD_Branchid", record.AtbuD_Branchid);
                    atbudParams.Add("@ATBUD_QuarterId", record.AtbuD_QuarterId);
                    atbudParams.Add("@ATBUD_Company_Type", record.AtbuD_Company_Type);
                    atbudParams.Add("@ATBUD_Headingid", record.AtbuD_Headingid);
                    atbudParams.Add("@ATBUD_Subheading", record.AtbuD_Subheading);
                    atbudParams.Add("@ATBUD_itemid", record.AtbuD_itemid);
                    atbudParams.Add("@ATBUD_SubItemId", record.AtbuD_SubItemid);
                    atbudParams.Add("@ATBUD_DELFLG", record.AtbuD_DELFLG ?? "N");
                    atbudParams.Add("@ATBUD_CRBY", record.AtbuD_CRBY);
                    atbudParams.Add("@ATBUD_UPDATEDBY", record.AtbuD_UPDATEDBY);
                    atbudParams.Add("@ATBUD_STATUS", record.AtbuD_STATUS ?? "Active");
                    atbudParams.Add("@ATBUD_Progress", record.AtbuD_Progress ?? string.Empty);
                    atbudParams.Add("@ATBUD_IPAddress", record.AtbuD_IPAddress ?? string.Empty);
                    atbudParams.Add("@ATBUD_CompId", record.AtbuD_CompId);
                    atbudParams.Add("@ATBUD_YEARId", nextYearId);
                    atbudParams.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    atbudParams.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync(
                        "spAcc_TrailBalance_Upload_Details",
                        atbudParams,
                        transaction,
                        commandType: CommandType.StoredProcedure
                    );

                    int detailResultId = atbudParams.Get<int>("@iOper");
                    detailIds.Add(detailResultId);
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
        //        public async Task<int[]> FreezeNextDurationTrialBalanceAsync(List<FreezeNextYearTrialBalanceDto> inputList)
        //        {
        //            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //            if (string.IsNullOrEmpty(dbName))
        //                throw new Exception("CustomerCode is missing in session. Please log in again.");

        //            var connectionString = _configuration.GetConnectionString(dbName);

        //            using var connection = new SqlConnection(connectionString);
        //            await connection.OpenAsync();

        //            using var transaction = connection.BeginTransaction();
        //            var detailIds = new List<int>();

        //            try
        //            {
        //                // First, find or create the SURPLUS record to accumulate the Net Income
        //                var surplusRecord = inputList.FirstOrDefault(x =>
        //                    x.AtbU_Description?.Trim().Equals("SURPLUS", StringComparison.OrdinalIgnoreCase) == true);

        //                // Find or create the Opening Stock record
        //                var openingStockRecord = inputList.FirstOrDefault(x =>
        //                    x.AtbU_Description?.Trim().Equals("Opening Stock", StringComparison.OrdinalIgnoreCase) == true);

        //                // Collect all Net Income amounts
        //                decimal netIncomeClosingDebit = 0;
        //                decimal netIncomeClosingCredit = 0;

        //                // Collect all Closing Stock amounts
        //                decimal closingStockDebit = 0;
        //                decimal closingStockCredit = 0;

        //                foreach (var record in inputList)
        //                {
        //                    if (record.AtbU_Description?.Trim().Equals("Net Income", StringComparison.OrdinalIgnoreCase) == true)
        //                    {
        //                        netIncomeClosingDebit += record.AtbU_Closing_Debit_Amount;
        //                        netIncomeClosingCredit += record.AtbU_Closing_Credit_Amount;
        //                    }

        //                    if (record.AtbU_Description?.Trim().Equals("Closing Stock", StringComparison.OrdinalIgnoreCase) == true)
        //                    {
        //                        closingStockDebit += record.AtbU_Closing_Debit_Amount;
        //                        closingStockCredit += record.AtbU_Closing_Credit_Amount;
        //                    }
        //                }

        //                foreach (var record in inputList)
        //                {
        //                    int nextYearId = record.AtbU_YEARId + 1;

        //                    var AtbuPkId = await connection.ExecuteScalarAsync<int>(
        //                        @"SELECT ISNULL(ATBU_ID, 0)
        //FROM Acc_TrailBalance_Upload
        //WHERE ATBU_Custid = @CustId
        //  AND ATBU_YEARId = @YearId
        //  AND ISNULL(ATBU_Description, '') = @Description
        //  AND ISNULL(ATBU_QuarterId, 0) = @QuarterId",
        //                        new
        //                        {
        //                            CustId = record.AtbU_CustId,
        //                            YearId = nextYearId,
        //                            Description = record.AtbU_Description ?? string.Empty,
        //                            QuarterId = record.AtbU_QuarterId
        //                        },
        //                        transaction
        //                    );

        //                    var AtbudPkId = await connection.ExecuteScalarAsync<int>(
        //                        @"SELECT ISNULL(ATBUD_ID, 0)
        //FROM Acc_TrailBalance_Upload_Details
        //WHERE ATBUD_Custid = @CustId
        //  AND ATBUD_YEARId = @YearId
        //  AND ISNULL(ATBUD_Description, '') = @Description
        //  AND ISNULL(ATBUD_QuarterId, 0) = @QuarterId",
        //                        new
        //                        {
        //                            CustId = record.AtbuD_CustId,
        //                            YearId = nextYearId,
        //                            Description = record.AtbuD_Description ?? string.Empty,
        //                            QuarterId = record.AtbuD_QuarterId
        //                        },
        //                        transaction
        //                    );

        //                    // ✅ Step 1: Handle ATBU record (master)
        //                    var atbuParams = new DynamicParameters();
        //                    atbuParams.Add("@ATBU_ID", AtbuPkId);
        //                    atbuParams.Add("@ATBU_CODE", record.AtbU_CODE ?? string.Empty);
        //                    atbuParams.Add("@ATBU_Description", record.AtbU_Description ?? string.Empty);
        //                    atbuParams.Add("@ATBU_CustId", record.AtbU_CustId);

        //                    // Check if this is the SURPLUS record
        //                    bool isSurplusRecord = record.AtbU_Description?.Trim().Equals("SURPLUS", StringComparison.OrdinalIgnoreCase) == true;

        //                    // Check if this is the Opening Stock record
        //                    bool isOpeningStockRecord = record.AtbU_Description?.Trim().Equals("Opening Stock", StringComparison.OrdinalIgnoreCase) == true;

        //                    // Check if this is a Net Income record
        //                    bool isNetIncomeRecord = record.AtbU_Description?.Trim().Equals("Net Income", StringComparison.OrdinalIgnoreCase) == true;

        //                    // Check if this is a Closing Stock record
        //                    bool isClosingStockRecord = record.AtbU_Description?.Trim().Equals("Closing Stock", StringComparison.OrdinalIgnoreCase) == true;

        //                    // ✅ Include your original condition logic here
        //                    if (record.AtbuD_SChedule_Type == 4)
        //                    {
        //                        // For Net Income record - save with 0 amounts
        //                        if (isNetIncomeRecord)
        //                        {
        //                            atbuParams.Add("@ATBU_Opening_Debit_Amount", 0);
        //                            atbuParams.Add("@ATBU_Opening_Credit_Amount", 0);
        //                            atbuParams.Add("@ATBU_TR_Debit_Amount", 0);
        //                            atbuParams.Add("@ATBU_TR_Credit_Amount", 0);
        //                            atbuParams.Add("@ATBU_Closing_Debit_Amount", 0);
        //                            atbuParams.Add("@ATBU_Closing_Credit_Amount", 0);
        //                        }
        //                        // For Closing Stock record - save with 0 amounts
        //                        else if (isClosingStockRecord)
        //                        {
        //                            atbuParams.Add("@ATBU_Opening_Debit_Amount", 0);
        //                            atbuParams.Add("@ATBU_Opening_Credit_Amount", 0);
        //                            atbuParams.Add("@ATBU_TR_Debit_Amount", 0);
        //                            atbuParams.Add("@ATBU_TR_Credit_Amount", 0);
        //                            atbuParams.Add("@ATBU_Closing_Debit_Amount", 0);
        //                            atbuParams.Add("@ATBU_Closing_Credit_Amount", 0);
        //                        }
        //                        // For SURPLUS record, add the Net Income amounts to opening balances
        //                        else if (isSurplusRecord)
        //                        {
        //                            atbuParams.Add("@ATBU_Opening_Debit_Amount", record.AtbU_Closing_Debit_Amount + netIncomeClosingDebit);
        //                            atbuParams.Add("@ATBU_Opening_Credit_Amount", record.AtbU_Closing_Credit_Amount + netIncomeClosingCredit);
        //                            atbuParams.Add("@ATBU_TR_Debit_Amount", 0);
        //                            atbuParams.Add("@ATBU_TR_Credit_Amount", 0);
        //                            atbuParams.Add("@ATBU_Closing_Debit_Amount", record.AtbU_Closing_Debit_Amount + netIncomeClosingDebit);
        //                            atbuParams.Add("@ATBU_Closing_Credit_Amount", record.AtbU_Closing_Credit_Amount + netIncomeClosingCredit);
        //                        }
        //                        // For Opening Stock record, add the Closing Stock amounts to opening balances
        //                        else if (isOpeningStockRecord)
        //                        {
        //                            atbuParams.Add("@ATBU_Opening_Debit_Amount", record.AtbU_Closing_Debit_Amount + closingStockDebit);
        //                            atbuParams.Add("@ATBU_Opening_Credit_Amount", record.AtbU_Closing_Credit_Amount + closingStockCredit);
        //                            atbuParams.Add("@ATBU_TR_Debit_Amount", 0);
        //                            atbuParams.Add("@ATBU_TR_Credit_Amount", 0);
        //                            atbuParams.Add("@ATBU_Closing_Debit_Amount", record.AtbU_Closing_Debit_Amount + closingStockDebit);
        //                            atbuParams.Add("@ATBU_Closing_Credit_Amount", record.AtbU_Closing_Credit_Amount + closingStockCredit);
        //                        }
        //                        else
        //                        {
        //                            atbuParams.Add("@ATBU_Opening_Debit_Amount", record.AtbU_Closing_Debit_Amount);
        //                            atbuParams.Add("@ATBU_Opening_Credit_Amount", record.AtbU_Closing_Credit_Amount);
        //                            atbuParams.Add("@ATBU_TR_Debit_Amount", 0);
        //                            atbuParams.Add("@ATBU_TR_Credit_Amount", 0);
        //                            atbuParams.Add("@ATBU_Closing_Debit_Amount", record.AtbU_Closing_Debit_Amount);
        //                            atbuParams.Add("@ATBU_Closing_Credit_Amount", record.AtbU_Closing_Credit_Amount);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        atbuParams.Add("@ATBU_Opening_Debit_Amount", 0);
        //                        atbuParams.Add("@ATBU_Opening_Credit_Amount", 0);
        //                        atbuParams.Add("@ATBU_TR_Debit_Amount", 0);
        //                        atbuParams.Add("@ATBU_TR_Credit_Amount", 0);
        //                        atbuParams.Add("@ATBU_Closing_Debit_Amount", 0);
        //                        atbuParams.Add("@ATBU_Closing_Credit_Amount", 0);
        //                    }

        //                    atbuParams.Add("@ATBU_DELFLG", record.AtbU_DELFLG ?? "N");
        //                    atbuParams.Add("@ATBU_CRBY", record.AtbU_CRBY);
        //                    atbuParams.Add("@ATBU_STATUS", record.AtbU_STATUS ?? "Active");
        //                    atbuParams.Add("@ATBU_UPDATEDBY", record.AtbU_UPDATEDBY);
        //                    atbuParams.Add("@ATBU_IPAddress", record.AtbU_IPAddress ?? string.Empty);
        //                    atbuParams.Add("@ATBU_CompId", record.AtbU_CompId);
        //                    atbuParams.Add("@ATBU_YEARId", nextYearId);
        //                    atbuParams.Add("@ATBU_Branchid", record.AtbU_Branchid);
        //                    atbuParams.Add("@ATBU_QuarterId", record.AtbU_QuarterId);
        //                    atbuParams.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
        //                    atbuParams.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //                    await connection.ExecuteAsync(
        //                        "spAcc_TrailBalance_Upload",
        //                        atbuParams,
        //                        transaction,
        //                        commandType: CommandType.StoredProcedure
        //                    );
        //                    int masId = atbuParams.Get<int>("@iOper");

        //                    // ✅ Step 2: Handle ATBUD record (detail)
        //                    var atbudParams = new DynamicParameters();
        //                    atbudParams.Add("@ATBUD_ID", AtbudPkId);
        //                    atbudParams.Add("@ATBUD_Masid", masId);
        //                    atbudParams.Add("@ATBUD_CODE", record.AtbuD_CODE ?? string.Empty);
        //                    atbudParams.Add("@ATBUD_Description", record.AtbuD_Description ?? string.Empty);
        //                    atbudParams.Add("@ATBUD_CustId", record.AtbuD_CustId);
        //                    atbudParams.Add("@ATBUD_SChedule_Type", record.AtbuD_SChedule_Type);
        //                    atbudParams.Add("@ATBUD_Branchid", record.AtbuD_Branchid);
        //                    atbudParams.Add("@ATBUD_QuarterId", record.AtbuD_QuarterId);
        //                    atbudParams.Add("@ATBUD_Company_Type", record.AtbuD_Company_Type);
        //                    atbudParams.Add("@ATBUD_Headingid", record.AtbuD_Headingid);
        //                    atbudParams.Add("@ATBUD_Subheading", record.AtbuD_Subheading);
        //                    atbudParams.Add("@ATBUD_itemid", record.AtbuD_itemid);
        //                    atbudParams.Add("@ATBUD_SubItemId", record.AtbuD_SubItemid);
        //                    atbudParams.Add("@ATBUD_DELFLG", record.AtbuD_DELFLG ?? "N");
        //                    atbudParams.Add("@ATBUD_CRBY", record.AtbuD_CRBY);
        //                    atbudParams.Add("@ATBUD_UPDATEDBY", record.AtbuD_UPDATEDBY);
        //                    atbudParams.Add("@ATBUD_STATUS", record.AtbuD_STATUS ?? "Active");
        //                    atbudParams.Add("@ATBUD_Progress", record.AtbuD_Progress ?? string.Empty);
        //                    atbudParams.Add("@ATBUD_IPAddress", record.AtbuD_IPAddress ?? string.Empty);
        //                    atbudParams.Add("@ATBUD_CompId", record.AtbuD_CompId);
        //                    atbudParams.Add("@ATBUD_YEARId", nextYearId);
        //                    atbudParams.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
        //                    atbudParams.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //                    await connection.ExecuteAsync(
        //                        "spAcc_TrailBalance_Upload_Details",
        //                        atbudParams,
        //                        transaction,
        //                        commandType: CommandType.StoredProcedure
        //                    );

        //                    int detailResultId = atbudParams.Get<int>("@iOper");
        //                    detailIds.Add(detailResultId);
        //                }

        //                transaction.Commit();
        //                return detailIds.ToArray();
        //            }
        //            catch
        //            {
        //                transaction.Rollback();
        //                throw;
        //            }
        //        }

        public async Task<int[]> FreezeNextDurationTrialBalanceAsync(List<FreezeNextYearTrialBalanceDto> inputList)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            var detailIds = new List<int>();

            try
            {
                // Collect all Net Income amounts
                decimal netIncomeClosingDebit = 0;
                decimal netIncomeClosingCredit = 0;

                // Collect all Closing Stock amounts
                decimal closingStockDebit = 0;
                decimal closingStockCredit = 0;

                // Find existing records
                FreezeNextYearTrialBalanceDto? surplusRecord = null;
                FreezeNextYearTrialBalanceDto? openingStockRecord = null;

                foreach (var record in inputList)
                {
                    if (record.AtbU_Description?.Trim().Equals("Net Income", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        netIncomeClosingDebit += record.AtbU_Closing_Debit_Amount;
                        netIncomeClosingCredit += record.AtbU_Closing_Credit_Amount;
                    }

                    if (record.AtbU_Description?.Trim().Equals("Closing Stock", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        closingStockDebit += record.AtbU_Closing_Debit_Amount;
                        closingStockCredit += record.AtbU_Closing_Credit_Amount;
                    }

                    if (record.AtbU_Description?.Trim().Equals("SURPLUS", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        surplusRecord = record;
                    }

                    if (record.AtbU_Description?.Trim().Equals("Opening Stock", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        openingStockRecord = record;
                    }
                }

                // Create SURPLUS record if it doesn't exist
                if (surplusRecord == null)
                {
                    surplusRecord = new FreezeNextYearTrialBalanceDto
                    {
                        AtbU_CODE = "SURPLUS",
                        AtbU_Description = "SURPLUS",
                        AtbU_CustId = inputList.First().AtbU_CustId, // Use first record's customer ID
                        AtbU_YEARId = inputList.First().AtbU_YEARId,
                        AtbU_Closing_Debit_Amount = 0,
                        AtbU_Closing_Credit_Amount = 0,
                        AtbU_DELFLG = "A",
                        AtbU_CRBY = inputList.First().AtbU_CRBY,
                        AtbU_STATUS = "A",
                        AtbU_UPDATEDBY = inputList.First().AtbU_UPDATEDBY,
                        AtbU_IPAddress = inputList.First().AtbU_IPAddress,
                        AtbU_CompId = inputList.First().AtbU_CompId,
                        AtbU_Branchid = inputList.First().AtbU_Branchid,
                        AtbU_QuarterId = inputList.First().AtbU_QuarterId,
                        AtbuD_CODE = "SURPLUS",
                        AtbuD_Description = "SURPLUS",
                        AtbuD_CustId = inputList.First().AtbuD_CustId,
                        AtbuD_SChedule_Type = 4, // Assuming schedule type 4 for surplus
                        AtbuD_Branchid = inputList.First().AtbuD_Branchid,
                        AtbuD_QuarterId = inputList.First().AtbuD_QuarterId,
                        AtbuD_Company_Type = inputList.First().AtbuD_Company_Type,
                        AtbuD_Headingid = 0, // Default or get from config
                        AtbuD_Subheading = 0,
                        AtbuD_itemid = 0,
                        AtbuD_SubItemid = 0,
                        AtbuD_DELFLG = "A",
                        AtbuD_CRBY = inputList.First().AtbuD_CRBY,
                        AtbuD_UPDATEDBY = inputList.First().AtbuD_UPDATEDBY,
                        AtbuD_STATUS = "A",
                        AtbuD_Progress = "",
                        AtbuD_IPAddress = inputList.First().AtbuD_IPAddress,
                        AtbuD_CompId = inputList.First().AtbuD_CompId,
                        HeadingName = "SURPLUS"
                    };
                    inputList.Add(surplusRecord);
                }

                // Create Opening Stock record if it doesn't exist
                if (openingStockRecord == null)
                {
                    var first = inputList.First();

                    // Schedule Type = 3 (Stock)
                    int scheduleType = 3;

                    // 🔹 Resolve hierarchy correctly
                    var headingId = await GetScheduleIdAsync(scheduleType, first.AtbuD_CustId, 1, 3);
                    var subHeadingId = headingId.HasValue
                        ? await GetScheduleIdAsync(scheduleType, first.AtbuD_CustId, 2, headingId.Value)
                        : null;

                    //var itemId = subHeadingId.HasValue
                    //    ? await GetScheduleIdAsync(scheduleType, first.AtbuD_CustId,         -+  3, subHeadingId.Value)
                    //    : null;

                    //var subItemId = itemId.HasValue
                    //    ? await GetScheduleIdAsync(scheduleType, first.AtbuD_CustId, 4, itemId.Value)
                    //    : null;

                    openingStockRecord = new FreezeNextYearTrialBalanceDto
                    {
                        AtbU_CODE = "Opening Stock",
                        AtbU_Description = "Opening Stock",
                        AtbU_CustId = first.AtbU_CustId,
                        AtbU_YEARId = first.AtbU_YEARId,
                        AtbU_Closing_Debit_Amount = 0,
                        AtbU_Closing_Credit_Amount = 0,
                        AtbU_DELFLG = "A",
                        AtbU_CRBY = first.AtbU_CRBY,
                        AtbU_STATUS = "A",
                        AtbU_UPDATEDBY = first.AtbU_UPDATEDBY,
                        AtbU_IPAddress = first.AtbU_IPAddress,
                        AtbU_CompId = first.AtbU_CompId,
                        AtbU_Branchid = first.AtbU_Branchid,
                        AtbU_QuarterId = first.AtbU_QuarterId,

                        AtbuD_CODE = "Opening Stock",
                        AtbuD_Description = "Opening Stock",
                        AtbuD_CustId = first.AtbuD_CustId,
                        AtbuD_SChedule_Type = scheduleType,
                        AtbuD_Branchid = first.AtbuD_Branchid,
                        AtbuD_QuarterId = first.AtbuD_QuarterId,
                        AtbuD_Company_Type = first.AtbuD_Company_Type,

                        AtbuD_Headingid = headingId ?? 0,
                        AtbuD_Subheading = subHeadingId ?? 0,
                        AtbuD_itemid = 0,
                        AtbuD_SubItemid = 0,

                        AtbuD_DELFLG = "A",
                        AtbuD_CRBY = first.AtbuD_CRBY,
                        AtbuD_UPDATEDBY = first.AtbuD_UPDATEDBY,
                        AtbuD_STATUS = "A",
                        AtbuD_Progress = "",
                        AtbuD_IPAddress = first.AtbuD_IPAddress,
                        AtbuD_CompId = first.AtbuD_CompId,

                        HeadingName = "Opening Stock"
                    };

                    inputList.Add(openingStockRecord);
                }


                foreach (var record in inputList)
                {
                    int nextYearId = record.AtbU_YEARId + 1;

                    var AtbuPkId = await connection.ExecuteScalarAsync<int>(
                        @"SELECT ISNULL(ATBU_ID, 0)
FROM Acc_TrailBalance_Upload
WHERE ATBU_Custid = @CustId
  AND ATBU_YEARId = @YearId
  AND ISNULL(ATBU_Description, '') = @Description
  AND ISNULL(ATBU_QuarterId, 0) = @QuarterId",
                        new
                        {
                            CustId = record.AtbU_CustId,
                            YearId = nextYearId,
                            Description = record.AtbU_Description ?? string.Empty,
                            QuarterId = record.AtbU_QuarterId
                        },
                        transaction
                    );

                    var AtbudPkId = await connection.ExecuteScalarAsync<int>(
                        @"SELECT ISNULL(ATBUD_ID, 0)
FROM Acc_TrailBalance_Upload_Details
WHERE ATBUD_Custid = @CustId
  AND ATBUD_YEARId = @YearId
  AND ISNULL(ATBUD_Description, '') = @Description
  AND ISNULL(ATBUD_QuarterId, 0) = @QuarterId",
                        new
                        {
                            CustId = record.AtbuD_CustId,
                            YearId = nextYearId,
                            Description = record.AtbuD_Description ?? string.Empty,
                            QuarterId = record.AtbuD_QuarterId
                        },
                        transaction
                    );

                    // ✅ Step 1: Handle ATBU record (master)
                    var atbuParams = new DynamicParameters();
                    atbuParams.Add("@ATBU_ID", AtbuPkId);
                    atbuParams.Add("@ATBU_CODE", record.AtbU_CODE ?? string.Empty);
                    atbuParams.Add("@ATBU_Description", record.AtbU_Description ?? string.Empty);
                    atbuParams.Add("@ATBU_CustId", record.AtbU_CustId);

                    // Check record types
                    bool isNetIncomeRecord = record.AtbU_Description?.Trim().Equals("Net Income", StringComparison.OrdinalIgnoreCase) == true;
                    bool isClosingStockRecord = record.AtbU_Description?.Trim().Equals("Closing Stock", StringComparison.OrdinalIgnoreCase) == true;
                    bool isOpeningStockRecord = record.AtbU_Description?.Trim().Equals("Opening Stock", StringComparison.OrdinalIgnoreCase) == true;
                    bool isSurplusRecord = record.AtbU_Description?.Trim().Equals("SURPLUS", StringComparison.OrdinalIgnoreCase) == true;

                    // ✅ Include your original condition logic here
                    if (record.AtbuD_SChedule_Type == 4)
                    {
                        // For Net Income record - save with 0 amounts
                        if (isNetIncomeRecord)
                        {
                            atbuParams.Add("@ATBU_Opening_Debit_Amount", 0);
                            atbuParams.Add("@ATBU_Opening_Credit_Amount", 0);
                            atbuParams.Add("@ATBU_TR_Debit_Amount", 0);
                            atbuParams.Add("@ATBU_TR_Credit_Amount", 0);
                            atbuParams.Add("@ATBU_Closing_Debit_Amount", 0);
                            atbuParams.Add("@ATBU_Closing_Credit_Amount", 0);
                        }
                        // For Closing Stock record - save with 0 amounts
                        else if (isClosingStockRecord)
                        {
                            atbuParams.Add("@ATBU_Opening_Debit_Amount", 0);
                            atbuParams.Add("@ATBU_Opening_Credit_Amount", 0);
                            atbuParams.Add("@ATBU_TR_Debit_Amount", 0);
                            atbuParams.Add("@ATBU_TR_Credit_Amount", 0);
                            atbuParams.Add("@ATBU_Closing_Debit_Amount", 0);
                            atbuParams.Add("@ATBU_Closing_Credit_Amount", 0);
                        }
                        // For SURPLUS record, add the Net Income amounts to opening balances
                        else if (isSurplusRecord)
                        {
                            atbuParams.Add("@ATBU_Opening_Debit_Amount", record.AtbU_Closing_Debit_Amount + netIncomeClosingDebit);
                            atbuParams.Add("@ATBU_Opening_Credit_Amount", record.AtbU_Closing_Credit_Amount + netIncomeClosingCredit);
                            atbuParams.Add("@ATBU_TR_Debit_Amount", 0);
                            atbuParams.Add("@ATBU_TR_Credit_Amount", 0);
                            atbuParams.Add("@ATBU_Closing_Debit_Amount", record.AtbU_Closing_Debit_Amount + netIncomeClosingDebit);
                            atbuParams.Add("@ATBU_Closing_Credit_Amount", record.AtbU_Closing_Credit_Amount + netIncomeClosingCredit);
                        }
                        // For Opening Stock record, add the Closing Stock amounts to opening balances
                        else if (isOpeningStockRecord)
                        {
                            atbuParams.Add("@ATBU_Opening_Debit_Amount", record.AtbU_Closing_Debit_Amount + closingStockDebit);
                            atbuParams.Add("@ATBU_Opening_Credit_Amount", record.AtbU_Closing_Credit_Amount + closingStockCredit);
                            atbuParams.Add("@ATBU_TR_Debit_Amount", 0);
                            atbuParams.Add("@ATBU_TR_Credit_Amount", 0);
                            atbuParams.Add("@ATBU_Closing_Debit_Amount", record.AtbU_Closing_Debit_Amount + closingStockDebit);
                            atbuParams.Add("@ATBU_Closing_Credit_Amount", record.AtbU_Closing_Credit_Amount + closingStockCredit);
                        }
                        else
                        {
                            atbuParams.Add("@ATBU_Opening_Debit_Amount", record.AtbU_Closing_Debit_Amount);
                            atbuParams.Add("@ATBU_Opening_Credit_Amount", record.AtbU_Closing_Credit_Amount);
                            atbuParams.Add("@ATBU_TR_Debit_Amount", 0);
                            atbuParams.Add("@ATBU_TR_Credit_Amount", 0);
                            atbuParams.Add("@ATBU_Closing_Debit_Amount", record.AtbU_Closing_Debit_Amount);
                            atbuParams.Add("@ATBU_Closing_Credit_Amount", record.AtbU_Closing_Credit_Amount);
                        }
                    }
                    else
                    {
                        atbuParams.Add("@ATBU_Opening_Debit_Amount", 0);
                        atbuParams.Add("@ATBU_Opening_Credit_Amount", 0);
                        atbuParams.Add("@ATBU_TR_Debit_Amount", 0);
                        atbuParams.Add("@ATBU_TR_Credit_Amount", 0);
                        atbuParams.Add("@ATBU_Closing_Debit_Amount", 0);
                        atbuParams.Add("@ATBU_Closing_Credit_Amount", 0);

                        if (isOpeningStockRecord)
                        {
                            atbuParams.Add("@ATBU_Opening_Debit_Amount", record.AtbU_Closing_Debit_Amount + closingStockDebit);
                            atbuParams.Add("@ATBU_Opening_Credit_Amount", record.AtbU_Closing_Credit_Amount + closingStockCredit);
                            atbuParams.Add("@ATBU_TR_Debit_Amount", 0);
                            atbuParams.Add("@ATBU_TR_Credit_Amount", 0);
                            atbuParams.Add("@ATBU_Closing_Debit_Amount", record.AtbU_Closing_Debit_Amount + closingStockDebit);
                            atbuParams.Add("@ATBU_Closing_Credit_Amount", record.AtbU_Closing_Credit_Amount + closingStockCredit);
                        }
                    }

                    atbuParams.Add("@ATBU_DELFLG", record.AtbU_DELFLG ?? "N");
                    atbuParams.Add("@ATBU_CRBY", record.AtbU_CRBY);
                    atbuParams.Add("@ATBU_STATUS", record.AtbU_STATUS ?? "Active");
                    atbuParams.Add("@ATBU_UPDATEDBY", record.AtbU_UPDATEDBY);
                    atbuParams.Add("@ATBU_IPAddress", record.AtbU_IPAddress ?? string.Empty);
                    atbuParams.Add("@ATBU_CompId", record.AtbU_CompId);
                    atbuParams.Add("@ATBU_YEARId", nextYearId);
                    atbuParams.Add("@ATBU_Branchid", record.AtbU_Branchid);
                    atbuParams.Add("@ATBU_QuarterId", record.AtbU_QuarterId);
                    atbuParams.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    atbuParams.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync(
                        "spAcc_TrailBalance_Upload",
                        atbuParams,
                        transaction,
                        commandType: CommandType.StoredProcedure
                    );
                    int masId = atbuParams.Get<int>("@iOper");

                    // ✅ Step 2: Handle ATBUD record (detail)
                    var atbudParams = new DynamicParameters();
                    atbudParams.Add("@ATBUD_ID", AtbudPkId);
                    atbudParams.Add("@ATBUD_Masid", masId);
                    atbudParams.Add("@ATBUD_CODE", record.AtbuD_CODE ?? string.Empty);
                    atbudParams.Add("@ATBUD_Description", record.AtbuD_Description ?? string.Empty);
                    atbudParams.Add("@ATBUD_CustId", record.AtbuD_CustId);
                    atbudParams.Add("@ATBUD_SChedule_Type", record.AtbuD_SChedule_Type);
                    atbudParams.Add("@ATBUD_Branchid", record.AtbuD_Branchid);
                    atbudParams.Add("@ATBUD_QuarterId", record.AtbuD_QuarterId);
                    atbudParams.Add("@ATBUD_Company_Type", record.AtbuD_Company_Type);
                    atbudParams.Add("@ATBUD_Headingid", record.AtbuD_Headingid);
                    atbudParams.Add("@ATBUD_Subheading", record.AtbuD_Subheading);
                    atbudParams.Add("@ATBUD_itemid", record.AtbuD_itemid);
                    atbudParams.Add("@ATBUD_SubItemId", record.AtbuD_SubItemid);
                    atbudParams.Add("@ATBUD_DELFLG", record.AtbuD_DELFLG ?? "N");
                    atbudParams.Add("@ATBUD_CRBY", record.AtbuD_CRBY);
                    atbudParams.Add("@ATBUD_UPDATEDBY", record.AtbuD_UPDATEDBY);
                    atbudParams.Add("@ATBUD_STATUS", record.AtbuD_STATUS ?? "Active");
                    atbudParams.Add("@ATBUD_Progress", record.AtbuD_Progress ?? string.Empty);
                    atbudParams.Add("@ATBUD_IPAddress", record.AtbuD_IPAddress ?? string.Empty);
                    atbudParams.Add("@ATBUD_CompId", record.AtbuD_CompId);
                    atbudParams.Add("@ATBUD_YEARId", nextYearId);
                    atbudParams.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    atbudParams.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync(
                        "spAcc_TrailBalance_Upload_Details",
                        atbudParams,
                        transaction,
                        commandType: CommandType.StoredProcedure
                    );

                    int detailResultId = atbudParams.Get<int>("@iOper");
                    detailIds.Add(detailResultId);
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

        private async Task<int?> GetScheduleIdAsync(
    int scheduleType,
    int custId,
    int level,          // 1=Heading, 2=SubHeading, 3=Item, 4=SubItem
    int parentId
)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));
            await connection.OpenAsync();

            return level switch
            {
                // 🔹 HEADING
                1 => await connection.ExecuteScalarAsync<int?>(@"
                SELECT TOP 1 ASH_ID
                FROM ACC_ScheduleHeading
                WHERE ASH_scheduletype = @scheduleType
                  AND ASH_Orgtype = @custId
                  AND ASH_Name = 'Expenses'
                ORDER BY ASH_ID",
                        new { scheduleType, custId, parentId }),

                // 🔹 SUB HEADING
                2 => await connection.ExecuteScalarAsync<int?>(@"
                SELECT TOP 1 ASSH_ID
                FROM ACC_ScheduleSubHeading
                WHERE ASSH_scheduletype = @scheduleType
                  AND ASSH_Orgtype = @custId
                  AND ASSH_Name = '(a) Cost Of Materials Consumed'
                ORDER BY ASSH_ID",
                        new { scheduleType, custId, parentId }),

                // 🔹 ITEM
                3 => await connection.ExecuteScalarAsync<int?>(@"
                SELECT TOP 1 ASI_ID
                FROM ACC_ScheduleItems
                WHERE ASI_scheduletype = @scheduleType
                  AND ASI_Orgtype = @custId
                  AND ASI_SubHeadingID = @parentId
                ORDER BY ASI_ID",
                        new { scheduleType, custId, parentId }),

                // 🔹 SUB ITEM
                4 => await connection.ExecuteScalarAsync<int?>(@"
                SELECT TOP 1 ASSI_ID
                FROM ACC_ScheduleSubItems
                WHERE ASSI_scheduletype = @scheduleType
                  AND ASSI_Orgtype = @custId
                  AND ASSI_ItemsID = @parentId
                ORDER BY ASSI_ID",
                        new { scheduleType, custId, parentId }),

                _ => throw new ArgumentException("Invalid schedule level")
            };
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

        //CheckTrailBalanceRecordExists
        public async Task<bool> CheckTrailBalanceRecordExistsAsync(int CompId, int CustId, int YearId, int BranchId, int QuarterId)
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

            string checkQuery = @"
        SELECT ATBU_ID 
        FROM Acc_TrailBalance_Upload 
        WHERE ATBU_CustId = @CustId 
          AND ATBU_YEARId = @YearId 
          AND ATBU_BranchId = @BranchId 
          AND ISNULL(ATBU_QuarterId, 0) = @QuarterId
          AND ATBU_CompId = @CompId";

            var existingRecord = await connection.QueryFirstOrDefaultAsync<int?>(checkQuery, new
            {
                CustId,
                YearId,
                BranchId,
                QuarterId,
                CompId
            });

            return existingRecord.HasValue; // ✅ true if exists, false if not
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
            int CustId = 0;

            // ✅ Step 1: Check if record already exists
            string checkQuery = @"
SELECT ATBU_ID 
FROM Acc_TrailBalance_Upload 
WHERE ATBU_Description = @Description
  AND ATBU_CustId = @CustId 
  AND ATBU_YEARId = @YearId 
  AND ATBU_BranchId = @BranchId 
  AND ISNULL(ATBU_QuarterId, 0) = @DurationId
";

            var existingRecord = await connection.QueryFirstOrDefaultAsync<int?>(checkQuery, new
            {
                Description = dtos[0].ATBU_Description,
                CustId = dtos[0].ATBU_CustId,
                YearId = dtos[0].ATBU_YEARId,
                BranchId = dtos[0].ATBU_Branchid,
                DurationId = dtos[0].ATBU_QuarterId
            }, transaction);

            // ✅ Step 2: If record exists → delete it before inserting new data
            if (existingRecord.HasValue && dtos[0].FlagUpdate != 0)
            {
                // Delete details using all relevant parameters
                string deleteDetailsQuery = @"
DELETE FROM Acc_TrailBalance_Upload_Details
  where ATBUD_CustId = @CustId
  AND ATBUD_YEARId = @YearId
  AND Atbud_Branchnameid = @BranchId
  AND ATBUd_QuarterId = @DurationId
  AND ATBUD_CompId = @CompId
";

                await connection.ExecuteAsync(deleteDetailsQuery, new
                {
                    CustId = dtos[0].ATBU_CustId,
                    YearId = dtos[0].ATBU_YEARId,
                    BranchId = dtos[0].ATBU_Branchid,
                    DurationId = dtos[0].ATBU_QuarterId,
                    CompId = dtos[0].ATBU_CompId
                }, transaction);

                // Delete master using all relevant parameters
                string deleteMasterQuery = @"
DELETE FROM Acc_TrailBalance_Upload
WHERE 
   ATBU_CustId = @CustId 
  AND ATBU_YEARId = @YearId 
  AND ATBU_BranchId = @BranchId 
  AND ATBU_QuarterId = @DurationId
  AND ATBU_CompId = @CompId
";

                await connection.ExecuteAsync(deleteMasterQuery, new
                {
                    CustId = dtos[0].ATBU_CustId,
                    YearId = dtos[0].ATBU_YEARId,
                    BranchId = dtos[0].ATBU_Branchid,
                    DurationId = dtos[0].ATBU_QuarterId,
                    CompId = dtos[0].ATBU_CompId
                }, transaction);
            }
            try
            {
                var query = @"
                select CUST_ORGTYPEID as OrgId 
                from SAD_CUSTOMER_MASTER 
                where CUST_ID = @CompanyId and CUST_DELFLG = 'A'";
                int OrgId = await connection.QueryFirstOrDefaultAsync<int>(query, new
                {
                    CompanyId = dtos[0].ATBU_CustId
                }, transaction);

                foreach (var dto in dtos)
                {
                    // 🔹 STEP A: Check if record exists (for update vs insert)
                    var existingRecordForUpdate = await connection.QueryFirstOrDefaultAsync<int>(@"
                SELECT ISNULL(ATBU_ID, 0)
                FROM Acc_TrailBalance_Upload
                WHERE ATBU_Custid = @CustId
                  AND ATBU_YEARId = @YearId
                  AND ISNULL(ATBU_Description, '') = @Description
                  AND ATBU_BranchId = @BranchId
                  AND ISNULL(ATBU_QuarterId, 0) = @QuarterId
            ", new
                    {
                        CustId = dto.ATBU_CustId,
                        YearId = dto.ATBU_YEARId,
                        Description = dto.ATBU_Description ?? string.Empty,
                        BranchId = dto.ATBU_Branchid,
                        QuarterId = dto.ATBU_QuarterId
                    }, transaction);

                    if (existingRecordForUpdate > 0)
                    {
                        // 🔹 Record found → perform update
                        dto.ATBU_ID = existingRecordForUpdate;
                    }
                    else
                    {
                        // 🔹 No existing record → new insert
                        dto.ATBU_ID = 0;
                    }

                    var AtbuPkId = await connection.ExecuteScalarAsync<int>(
                       @"SELECT ISNULL(ATBU_ID, 0)
                       FROM Acc_TrailBalance_Upload
                       WHERE ATBU_Custid = @CustId
                          AND ATBU_YEARId = @YearId
                          AND ISNULL(ATBU_Description, '') = @Description
                          AND ISNULL(ATBU_QuarterId, 0) = @QuarterId",
                       new
                       {
                           CustId = dto.ATBU_CustId,
                           YearId = dto.ATBU_YEARId,
                           Description = dto.ATBU_Description ?? string.Empty,
                           QuarterId = dto.ATBU_QuarterId
                       },
                       transaction
                    );

                    var AtbudPkId = await connection.ExecuteScalarAsync<int>(
                        @"SELECT ISNULL(ATBUD_ID, 0)
                        FROM Acc_TrailBalance_Upload_Details
                        WHERE ATBUD_Custid = @CustId
                           AND ATBUD_YEARId = @YearId
                           AND ISNULL(ATBUD_Description, '') = @Description
                           AND ISNULL(ATBUD_QuarterId, 0) = @QuarterId",
                        new
                        {
                            CustId = dto.ATBUD_CustId,
                            YearId = dto.ATBUD_YEARId,
                            Description = dto.ATBUD_Description ?? string.Empty,
                            QuarterId = dto.ATBUD_QuarterId
                        },
                        transaction
                    );

                    if (dto.ATBU_Description != "")
                    {
                        if (dto.ATBU_Opening_Debit_Amount != 0 || dto.ATBU_Opening_Credit_Amount != 0 || dto.ATBU_TR_Debit_Amount != 0 || dto.ATBU_TR_Credit_Amount != 0 || dto.ATBU_Closing_Debit_Amount != 0 || dto.ATBU_Closing_Credit_Amount != 0)
                        {
                            int updateOrSave = 0, oper = 0;
                            int subItemId = 0, itemId = 0, subHeadingId = 0, headingId = 0, scheduleType = 0, LevelId = 0;
                            DataTable templateIds;

                            //// 🧠 STEP 1: AI Prediction if values are missing
                            //if (string.IsNullOrWhiteSpace(dto.Excel_Heading)
                            //    && string.IsNullOrWhiteSpace(dto.Excel_SubHeading)
                            //    && string.IsNullOrWhiteSpace(dto.Excel_Item)
                            //    && string.IsNullOrWhiteSpace(dto.Excel_SubItem))
                            //{
                            //    var aiResult = await _aiMappingService.PredictMappingAsync(new MappingPredictionRequest
                            //    {
                            //        Description = dto.ATBU_Description,
                            //        Amount = dto.ATBU_TR_Debit_Amount + dto.ATBU_TR_Credit_Amount,
                            //        CustId = dto.ATBU_CustId,
                            //        OrgType = dto.ATBUD_Company_Type
                            //    });

                            //    dto.Excel_Heading = aiResult.Heading;
                            //    dto.Excel_SubHeading = aiResult.SubHeading;
                            //    dto.Excel_Item = aiResult.Item;
                            //    dto.Excel_SubItem = aiResult.SubItem;
                            //}
                            //else
                            //{
                            //    // ✅ If mapping is already provided (e.g. from Excel), train the AI for next time
                            //    await _aiMappingService.AddTrainingDataAsync(new MappingTrainingRequest
                            //    {
                            //        Description = dto.ATBU_Description?.Trim(),
                            //        OrgType = dto.ATBUD_Company_Type,
                            //        Heading = dto.Excel_Heading?.Trim(),
                            //        SubHeading = string.IsNullOrWhiteSpace(dto.Excel_SubHeading) ? "-" : dto.Excel_SubHeading.Trim(),
                            //        Item = string.IsNullOrWhiteSpace(dto.Excel_Item) ? "-" : dto.Excel_Item.Trim(),
                            //        SubItem = string.IsNullOrWhiteSpace(dto.Excel_SubItem) ? "-" : dto.Excel_SubItem.Trim()
                            //    });
                            //}

                            // 🧠 STEP 2: Resolve Name to IDs from database
                            if (!string.IsNullOrWhiteSpace(dto.Excel_SubItem))
                            {
                                subItemId = await GetIdFromNameAsync(connection, transaction, "ACC_ScheduleSubItems", "ASSI_ID", dto.Excel_SubItem, dto.ATBU_CustId);
                                if (subItemId == 0)
                                {
                                    var subIds = await GetGroupIdFromAliasAsync(connection, transaction, dto.Excel_SubItem, dto.ATBU_CompId, dto.ATBU_CustId, 4, OrgId);
                                    if (subIds.Rows.Count > 0)
                                    {
                                        DataRow row = subIds.Rows[0];
                                        subItemId = Convert.ToInt32(row["ID"]);
                                        LevelId = Convert.ToInt32(row["Level"]);
                                        templateIds = await GetScheduleIDs(connection, transaction, subItemId, dto.ATBU_CustId, LevelId);
                                        if (templateIds.Rows.Count > 0)
                                        {
                                            row = templateIds.Rows[0];
                                            headingId = Convert.ToInt32(row["ASH_ID"]);
                                            subHeadingId = Convert.ToInt32(row["ASSH_ID"]);
                                            itemId = Convert.ToInt32(row["ASI_ID"]);
                                            subItemId = Convert.ToInt32(row["ASSI_ID"]);
                                        }
                                        scheduleType = await GetScheduleTypeFromTemplateAsync(connection, transaction, subItemId, itemId, subHeadingId, headingId, dto.ATBU_CustId);
                                        goto saveFunction;
                                    }
                                }
                                else
                                {
                                    templateIds = await GetScheduleIDs(connection, transaction, subItemId, dto.ATBU_CustId, 4);
                                    if (templateIds.Rows.Count > 0)
                                    {
                                        DataRow row = templateIds.Rows[0];
                                        headingId = Convert.ToInt32(row["ASH_ID"]);
                                        subHeadingId = Convert.ToInt32(row["ASSH_ID"]);
                                        itemId = Convert.ToInt32(row["ASI_ID"]);
                                        subItemId = Convert.ToInt32(row["ASSI_ID"]);
                                    }
                                    scheduleType = await GetScheduleTypeFromTemplateAsync(connection, transaction, subItemId, itemId, subHeadingId, headingId, dto.ATBU_CustId);
                                    goto saveFunction;
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(dto.Excel_Item))
                            {
                                itemId = await GetIdFromNameAsync(connection, transaction, "ACC_ScheduleItems", "ASI_ID", dto.Excel_Item, dto.ATBU_CustId);
                                if (itemId == 0)
                                {
                                    var subIds = await GetGroupIdFromAliasAsync(connection, transaction, dto.Excel_Item, dto.ATBU_CompId, dto.ATBU_CustId, 3, OrgId);
                                    if (subIds.Rows.Count > 0)
                                    {
                                        DataRow row = subIds.Rows[0];
                                        itemId = Convert.ToInt32(row["ID"]);
                                        LevelId = Convert.ToInt32(row["Level"]);
                                        templateIds = await GetScheduleIDs(connection, transaction, itemId, dto.ATBU_CustId, LevelId);
                                        if (templateIds.Rows.Count > 0)
                                        {
                                            row = templateIds.Rows[0];
                                            headingId = Convert.ToInt32(row["ASH_ID"]);
                                            subHeadingId = Convert.ToInt32(row["ASSH_ID"]);
                                            itemId = Convert.ToInt32(row["ASI_ID"]);
                                            subItemId = Convert.ToInt32(row["ASSI_ID"]);
                                        }
                                        scheduleType = await GetScheduleTypeFromTemplateAsync(connection, transaction, subItemId, itemId, subHeadingId, headingId, dto.ATBU_CustId);
                                        goto saveFunction;

                                    }

                                }
                                else
                                {
                                    templateIds = await GetScheduleIDs(connection, transaction, itemId, dto.ATBU_CustId, 3);
                                    if (templateIds.Rows.Count > 0)
                                    {
                                        DataRow row = templateIds.Rows[0];
                                        headingId = Convert.ToInt32(row["ASH_ID"]);
                                        subHeadingId = Convert.ToInt32(row["ASSH_ID"]);
                                        itemId = Convert.ToInt32(row["ASI_ID"]);
                                        subItemId = Convert.ToInt32(row["ASSI_ID"]);
                                    }
                                    scheduleType = await GetScheduleTypeFromTemplateAsync(connection, transaction, subItemId, itemId, subHeadingId, headingId, dto.ATBU_CustId);
                                    goto saveFunction;
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(dto.Excel_SubHeading))
                            {
                                subHeadingId = await GetIdFromNameAsync(connection, transaction, "ACC_ScheduleSubHeading", "ASSH_ID", dto.Excel_SubHeading, dto.ATBU_CustId);
                                if (subHeadingId == 0)
                                {
                                    var subIds = await GetGroupIdFromAliasAsync(connection, transaction, dto.Excel_SubHeading, dto.ATBU_CompId, dto.ATBU_CustId, 2, OrgId);
                                    if (subIds.Rows.Count > 0)
                                    {
                                        DataRow row = subIds.Rows[0];
                                        subHeadingId = Convert.ToInt32(row["ID"]);
                                        LevelId = Convert.ToInt32(row["Level"]);
                                        templateIds = await GetScheduleIDs(connection, transaction, subHeadingId, dto.ATBU_CustId, LevelId);
                                        if (templateIds.Rows.Count > 0)
                                        {
                                            row = templateIds.Rows[0];
                                            headingId = Convert.ToInt32(row["ASH_ID"]);
                                            subHeadingId = Convert.ToInt32(row["ASSH_ID"]);
                                            itemId = Convert.ToInt32(row["ASI_ID"]);
                                            subItemId = Convert.ToInt32(row["ASSI_ID"]);
                                        }
                                        scheduleType = await GetScheduleTypeFromTemplateAsync(connection, transaction, subItemId, itemId, subHeadingId, headingId, dto.ATBU_CustId);
                                        goto saveFunction;
                                    }
                                }
                                else
                                {
                                    templateIds = await GetScheduleIDs(connection, transaction, subHeadingId, dto.ATBU_CustId, 2);
                                    if (templateIds.Rows.Count > 0)
                                    {
                                        DataRow row = templateIds.Rows[0];
                                        headingId = Convert.ToInt32(row["ASH_ID"]);
                                        subHeadingId = Convert.ToInt32(row["ASSH_ID"]);
                                        itemId = Convert.ToInt32(row["ASI_ID"]);
                                        subItemId = Convert.ToInt32(row["ASSI_ID"]);
                                    }
                                    scheduleType = await GetScheduleTypeFromTemplateAsync(connection, transaction, subItemId, itemId, subHeadingId, headingId, dto.ATBU_CustId);
                                    goto saveFunction;
                                }
                            }
                          

                            saveFunction:
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
                    }
                }
                transaction.Commit();

                // ✅ Call UpdateNetIncomeAsync once with common values (from first dto)
                var firstDto = dtos.FirstOrDefault();
                if (firstDto != null)
                {
                    await UpdateNetIncomeAsync(
                        firstDto.ATBUD_CompId,
                        firstDto.ATBUD_CustId,
                        firstDto.ATBUD_CRBY,
                        firstDto.ATBUD_YEARId,
                        firstDto.ATBUD_Branchid.ToString(),
                        firstDto.ATBUD_QuarterId
                    );
                }
                return insertedIds.ToArray();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        private async Task<DataTable> GetScheduleIDs(SqlConnection conn, SqlTransaction tran, int Id, int orgType, int LevelId)
        {
            string query = string.Empty;
            int idValue = 0;
            if (LevelId == 4)
            {

                query = @"SELECT AST_HeadingID, a.ASH_ID, a.ASH_Name, ISNULL(AST_Schedule_type,0) AS AST_Schedule_type,
                      ISNULL(b.ASSH_ID,0) AS ASSH_ID, ISNULL(b.ASSH_Name,'') AS ASSH_Name, 
                      ISNULL(c.ASI_ID,0) AS ASI_ID, ISNULL(c.ASI_Name,'') AS ASI_Name,
                      ISNULL(d.ASSI_ID,0) AS ASSi_ID, ISNULL(d.ASSI_Name,'') AS ASSI_Name 
               FROM ACC_ScheduleTemplates 
               LEFT JOIN ACC_ScheduleSubItems d ON d.ASSI_ID = AST_SubItemID
               LEFT JOIN ACC_ScheduleItems c ON c.ASI_ID = AST_ItemID
               LEFT JOIN ACC_ScheduleSubHeading b ON b.ASSH_ID = AST_SubHeadingID
               LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = AST_HeadingID
               WHERE AST_SubItemID = @ID AND AST_Companytype = @OrgType";
            }
            else if (LevelId == 3)
            {
                query = @"SELECT AST_HeadingID, a.ASH_ID, a.ASH_Name, ISNULL(AST_Schedule_type,0) AS AST_Schedule_type,
                      ISNULL(b.ASSH_ID,0) AS ASSH_ID, ISNULL(b.ASSH_Name,'') AS ASSH_Name, 
                      ISNULL(c.ASI_ID,0) AS ASI_ID, ISNULL(c.ASI_Name,'') AS ASI_Name,
                      0 AS ASSi_ID, '' AS ASSI_Name 
               FROM ACC_ScheduleTemplates 
               LEFT JOIN ACC_ScheduleSubItems d ON d.ASSI_ID = AST_SubItemID
               LEFT JOIN ACC_ScheduleItems c ON c.ASI_ID = AST_ItemID
               LEFT JOIN ACC_ScheduleSubHeading b ON b.ASSH_ID = AST_SubHeadingID
               LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = AST_HeadingID
               WHERE AST_ItemID = @ID AND AST_Companytype = @OrgType AND AST_SubItemID = 0";
            }
            else if (LevelId == 2)
            {
                query = @"SELECT AST_HeadingID, a.ASH_ID, a.ASH_Name, ISNULL(AST_Schedule_type,0) AS AST_Schedule_type,
                      ISNULL(b.ASSH_ID,0) AS ASSH_ID, ISNULL(b.ASSH_Name,'') AS ASSH_Name,
                      0 AS ASI_ID, '' AS ASI_Name,
                      0 AS ASSi_ID, '' AS ASSI_Name 
               FROM ACC_ScheduleTemplates 
               LEFT JOIN ACC_ScheduleSubItems d ON d.ASSI_ID = AST_SubItemID
               LEFT JOIN ACC_ScheduleItems c ON c.ASI_ID = AST_ItemID
               LEFT JOIN ACC_ScheduleSubHeading b ON b.ASSH_ID = AST_SubHeadingID
               LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = AST_HeadingID
               WHERE AST_SubHeadingID = @ID AND AST_Companytype = @OrgType";
            }
            else if (LevelId == 1)
            {
                query = @"SELECT AST_HeadingID, a.ASH_ID, a.ASH_Name, ISNULL(AST_Schedule_type,0) AS AST_Schedule_type,
                      0 AS ASSH_ID, '' AS ASSH_Name,
                      0 AS ASI_ID, '' AS ASI_Name,
                      0 AS ASSi_ID, '' AS ASSI_Name 
               FROM ACC_ScheduleTemplates 
               LEFT JOIN ACC_ScheduleSubItems d ON d.ASSI_ID = AST_SubItemID
               LEFT JOIN ACC_ScheduleItems c ON c.ASI_ID = AST_ItemID
               LEFT JOIN ACC_ScheduleSubHeading b ON b.ASSH_ID = AST_SubHeadingID
               LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = AST_HeadingID
               WHERE AST_HeadingID = @ID AND AST_Companytype = @OrgType";
            }
            else
            {
                return new DataTable(); // return empty if nothing is matched
            }

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                // Determine which ID to pass     
                cmd.Parameters.AddWithValue("@ID", Id);
                cmd.Parameters.AddWithValue("@OrgType", orgType);

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable result = new DataTable();
                    await Task.Run(() => adapter.Fill(result));
                    return result;
                }
            }
        }
        private async Task<DataTable> GetGroupIdFromAliasAsync(SqlConnection conn, SqlTransaction tran, string desc, int acId, int custId, int grpLevel, int orgId)
        {
            DataTable dtGroup = new DataTable();

            try
            {
                string query = @"SELECT ISNULL(AGA_GLID, 0) AS ID, AGA_GrpLevel AS Level, AGA_GLDESC 
                         FROM Acc_GroupingAlias 
                         WHERE AGA_Description = @Desc AND AGA_Orgtype = @OrgId AND AGA_Compid = @AcId";

                using (SqlCommand cmd = new SqlCommand(query, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@Desc", desc);
                    cmd.Parameters.AddWithValue("@OrgId", orgId);
                    cmd.Parameters.AddWithValue("@AcId", acId);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        await Task.Run(() => adapter.Fill(dtGroup));
                    }
                }

                // If alias not found
                if (dtGroup.Rows.Count == 0)
                {
                    if (grpLevel == 4)
                    {
                        dtGroup = await TryScheduleLevelAsync(conn, tran, "ACC_ScheduleItems", "ASI", desc, custId, acId, 3);
                        if (dtGroup.Rows.Count == 0)
                            dtGroup = await TryScheduleLevelAsync(conn, tran, "ACC_ScheduleSubHeading", "ASSH", desc, custId, acId, 2);
                        if (dtGroup.Rows.Count == 0)
                            dtGroup = await TryScheduleLevelAsync(conn, tran, "ACC_ScheduleHeading", "ASH", desc, custId, acId, 1);
                    }
                    else if (grpLevel == 3)
                    {
                        dtGroup = await TryScheduleLevelAsync(conn, tran, "ACC_ScheduleSubHeading", "ASSH", desc, custId, acId, 2);
                        if (dtGroup.Rows.Count == 0)
                            dtGroup = await TryScheduleLevelAsync(conn, tran, "ACC_ScheduleHeading", "ASH", desc, custId, acId, 1);
                    }
                    else if (grpLevel == 2)
                    {
                        dtGroup = await TryScheduleLevelAsync(conn, tran, "ACC_ScheduleHeading", "ASH", desc, custId, acId, 1);
                    }
                    else if (grpLevel == 1)
                    {
                        dtGroup = await TryScheduleLevelAsync(conn, tran, "ACC_ScheduleHeading", "ASH", desc, custId, acId, 1);
                    }
                }
                else // If alias found, use AGA_GLDESC for further lookup
                {
                    string resolvedDesc = dtGroup.Rows[0]["AGA_GLDESC"].ToString();

                    if (grpLevel == 1)
                    {
                        dtGroup = await TryScheduleLevelAsync(conn, tran, "ACC_ScheduleHeading", "ASH", resolvedDesc, custId, acId, 1);
                        if (dtGroup.Rows.Count == 0)
                            dtGroup = await TryScheduleLevelAsync(conn, tran, "ACC_ScheduleSubHeading", "ASSH", resolvedDesc, custId, acId, 2);
                        if (dtGroup.Rows.Count == 0)
                            dtGroup = await TryScheduleLevelAsync(conn, tran, "ACC_ScheduleItems", "ASI", resolvedDesc, custId, acId, 3);
                        if (dtGroup.Rows.Count == 0)
                            dtGroup = await TryScheduleLevelAsync(conn, tran, "ACC_SchedulesubItems", "ASSI", resolvedDesc, custId, acId, 4);
                    }
                    else if (grpLevel == 2)
                    {
                        dtGroup = await TryScheduleLevelAsync(conn, tran, "ACC_ScheduleSubHeading", "ASSH", resolvedDesc, custId, acId, 2);
                        if (dtGroup.Rows.Count == 0)
                            dtGroup = await TryScheduleLevelAsync(conn, tran, "ACC_ScheduleItems", "ASI", resolvedDesc, custId, acId, 3);
                        if (dtGroup.Rows.Count == 0)
                            dtGroup = await TryScheduleLevelAsync(conn, tran, "ACC_SchedulesubItems", "ASSI", resolvedDesc, custId, acId, 4);
                    }
                    else if (grpLevel == 3)
                    {
                        dtGroup = await TryScheduleLevelAsync(conn, tran, "ACC_ScheduleItems", "ASI", resolvedDesc, custId, acId, 3);
                        if (dtGroup.Rows.Count == 0)
                            dtGroup = await TryScheduleLevelAsync(conn, tran, "ACC_SchedulesubItems", "ASSI", resolvedDesc, custId, acId, 4);
                    }
                    else if (grpLevel == 4)
                    {
                        dtGroup = await TryScheduleLevelAsync(conn, tran, "ACC_SchedulesubItems", "ASSI", resolvedDesc, custId, acId, 4);
                    }
                }

                return dtGroup;
            }
            catch
            {
                throw;
            }
        }
        private async Task<DataTable> TryScheduleLevelAsync(SqlConnection conn, SqlTransaction tran, string table, string prefix, string desc, int orgType, int compId, int level)
        {
            string columnId = $"{prefix}_ID";
            string columnName = $"{prefix}_Name";
            string columnOrg = $"{prefix}_Orgtype";
            string columnComp = $"{prefix}_Compid";

            string query = $@"
        SELECT ISNULL({columnId}, 0) AS ID, {level} AS Level 
        FROM {table} 
        WHERE {columnName} = @Desc AND {columnOrg} = @OrgType AND {columnComp} = @CompId";

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@Desc", desc);
                cmd.Parameters.AddWithValue("@OrgType", orgType);
                cmd.Parameters.AddWithValue("@CompId", compId);

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable result = new DataTable();
                    await Task.Run(() => adapter.Fill(result));
                    return result;
                }
            }
        }

        //private async Task<DataTable> GetScheduleIDs(SqlConnection conn, SqlTransaction tran, int subItemId, int itemId, int subHeadingId, int headingId, int orgType)
        //{
        //    string sSql = "";


        //    if (subItemId > 0)
        //    {
        //        sSql = $@"SELECT AST_HeadingID,a.ASH_ID,a.ASH_Name,ISNULL(AST_Schedule_type,0) AS AST_Schedule_type,

        //            sSql = $@"SELECT AST_HeadingID,a.ASH_ID,a.ASH_Name,ISNULL(AST_Schedule_type,0) AS AST_Schedule_type,
        //            ISNULL(b.ASSH_ID,0) AS ASSH_ID, ISNULL(b.ASSH_Name,'') AS ASSH_Name, 
        //            ISNULL(c.ASI_ID,0) AS ASI_ID, ISNULL(c.ASI_Name,'') AS ASI_Name,
        //            ISNULL(d.ASSI_ID,0) AS ASSi_ID, ISNULL(d.ASSI_Name,'') AS ASSI_Name 
        //            FROM ACC_ScheduleTemplates 
        //            LEFT JOIN ACC_ScheduleSubItems d ON d.ASSI_ID=AST_SubItemID
        //            LEFT JOIN ACC_ScheduleItems c ON c.ASI_ID=AST_ItemID
        //            LEFT JOIN ACC_ScheduleSubHeading b ON b.ASSH_ID=AST_SubHeadingID
        //            LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID=AST_HeadingID
        //            WHERE AST_SubItemID={subItemId} AND AST_Companytype={orgType}";

        //    }
        //    else if (itemId > 0)
        //    {
        //        sSql = $@"SELECT AST_HeadingID,a.ASH_ID,a.ASH_Name,ISNULL(AST_Schedule_type,0) AS AST_Schedule_type,

        //        }
        //    else if (itemId > 0)
        //    {
        //            sSql = $@"SELECT AST_HeadingID,a.ASH_ID,a.ASH_Name,ISNULL(AST_Schedule_type,0) AS AST_Schedule_type,
        //            ISNULL(b.ASSH_ID,0) AS ASSH_ID, ISNULL(b.ASSH_Name,'') AS ASSH_Name, 
        //            ISNULL(c.ASI_ID,0) AS ASI_ID, ISNULL(c.ASI_Name,'') AS ASI_Name,
        //            0 AS ASSi_ID, '' AS ASSI_Name 
        //            FROM ACC_ScheduleTemplates 
        //            LEFT JOIN ACC_ScheduleSubItems d ON d.ASSI_ID=AST_SubItemID
        //            LEFT JOIN ACC_ScheduleItems c ON c.ASI_ID=AST_ItemID
        //            LEFT JOIN ACC_ScheduleSubHeading b ON b.ASSH_ID=AST_SubHeadingID
        //            LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID=AST_HeadingID
        //            WHERE AST_ItemID={itemId} AND AST_Companytype={orgType} AND AST_SubItemID=0";

        //    }
        //    else if (subHeadingId > 0)
        //    {
        //        sSql = $@"SELECT AST_HeadingID,a.ASH_ID,a.ASH_Name,ISNULL(AST_Schedule_type,0) AS AST_Schedule_type,

        //        }
        //    else if (subHeadingId > 0)
        //    {
        //            sSql = $@"SELECT AST_HeadingID,a.ASH_ID,a.ASH_Name,ISNULL(AST_Schedule_type,0) AS AST_Schedule_type,

        //            ISNULL(b.ASSH_ID,0) AS ASSH_ID, ISNULL(b.ASSH_Name,'') AS ASSH_Name,
        //            0 AS ASI_ID, '' AS ASI_Name,
        //            0 AS ASSi_ID, '' AS ASSI_Name 
        //            FROM ACC_ScheduleTemplates 
        //            LEFT JOIN ACC_ScheduleSubItems d ON d.ASSI_ID=AST_SubItemID
        //            LEFT JOIN ACC_ScheduleItems c ON c.ASI_ID=AST_ItemID
        //            LEFT JOIN ACC_ScheduleSubHeading b ON b.ASSH_ID=AST_SubHeadingID
        //            LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID=AST_HeadingID
        //            WHERE AST_SubHeadingID={subHeadingId} AND AST_Companytype={orgType}";

        //    }
        //    else if (headingId > 0)
        //    {
        //        sSql = $@"SELECT AST_HeadingID,a.ASH_ID,a.ASH_Name,ISNULL(AST_Schedule_type,0) AS AST_Schedule_type,

        //        }
        //    else if (headingId > 0)
        //    {
        //            sSql = $@"SELECT AST_HeadingID,a.ASH_ID,a.ASH_Name,ISNULL(AST_Schedule_type,0) AS AST_Schedule_type,

        //            0 AS ASSH_ID, '' AS ASSH_Name,
        //            0 AS ASI_ID, '' AS ASI_Name,
        //            0 AS ASSi_ID, '' AS ASSI_Name 
        //            FROM ACC_ScheduleTemplates 
        //            LEFT JOIN ACC_ScheduleSubItems d ON d.ASSI_ID=AST_SubItemID
        //            LEFT JOIN ACC_ScheduleItems c ON c.ASI_ID=AST_ItemID
        //            LEFT JOIN ACC_ScheduleSubHeading b ON b.ASSH_ID=AST_SubHeadingID
        //            LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID=AST_HeadingID
        //            WHERE AST_HeadingID={headingId} AND AST_Companytype={orgType}";
        //    }
        //    return await ExecuteSqlToAsync(conn, sSql);

        //        }  
        //    return await ExecuteSqlToDataTableAsync(conn, sSql);

        //}

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

        //UploadNetIncome
        public async Task<bool> UpdateNetIncomeAsync(int compId, int custId, int userId, int yearId, string branchId, int durationId)
        {
            // Step 1: Get DB name from session
            var dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Step 3: Check if 'Net income' record already exists
            var checkQuery = @"
      SELECT * 
      FROM Acc_TrailBalance_Upload 
      WHERE ATBU_Description = 'Net income' 
        AND ATBU_CustId = @CustId 
        AND ATBU_YEARId = @YearId 
        AND ATBU_Branchid = @BranchId 
        AND ATBU_QuarterId = @DurationId";

            var existingRecord = await connection.QueryFirstOrDefaultAsync(checkQuery, new
            {
                CustId = custId,
                YearId = yearId,
                BranchId = branchId,
                DurationId = durationId
            });

            if (existingRecord != null)
            {
                // Record already exists; no insert required
                return false;
            }

            // Step 4: Get new ID
            var maxIdQuery = @"
      SELECT ISNULL(MAX(ATBU_ID), 0) + 1 
      FROM Acc_TrailBalance_Upload ";

            int newId = await connection.ExecuteScalarAsync<int>(maxIdQuery, new
            {
                CustId = custId,
                YearId = yearId,
                BranchId = branchId,
                DurationId = durationId
            });

            // Step 5: Insert into Acc_TrailBalance_Upload
            var insertUploadQuery = @"
      INSERT INTO Acc_TrailBalance_Upload
      (
          ATBU_ID, ATBU_Description, ATBU_CODE, ATBU_CustId, ATBU_Branchid, ATBU_QuarterId, ATBU_YEARId,
          ATBU_Opening_Debit_Amount, ATBU_Opening_Credit_Amount, ATBU_TR_Debit_Amount, ATBU_TR_Credit_Amount,
          ATBU_Closing_Debit_Amount, ATBU_Closing_Credit_Amount, ATBU_Closing_TotalDebit_Amount, ATBU_Closing_TotalCredit_Amount,
          ATBU_CRBY, Atbu_CrOn, ATBU_CompId
      )
      VALUES
      (
          @NewId, 'Net Income', @NewId, @CustId, @BranchId, @DurationId, @YearId,
          0, 0, 0, 0, 0, 0, 0, 0,
          @UserId, GETDATE(), @CompId
      );";

            await connection.ExecuteAsync(insertUploadQuery, new
            {
                NewId = newId,
                CustId = custId,
                BranchId = branchId,
                DurationId = durationId,
                YearId = yearId,
                UserId = userId,
                CompId = compId
            });

            maxIdQuery = @"
      SELECT ISNULL(MAX(ATBUD_ID), 0) + 1 
      FROM Acc_TrailBalance_Upload_Details ";

            newId = await connection.ExecuteScalarAsync<int>(maxIdQuery, new
            {
                CustId = custId,
                YearId = yearId,
                BranchId = branchId,
                DurationId = durationId
            });

            // Step 6: Insert into Acc_TrailBalance_Upload_details
            var insertDetailQuery = @"
      INSERT INTO Acc_TrailBalance_Upload_details
      (
          ATBUD_ID, ATBUD_Masid, ATBUD_Description, ATBUD_CODE, ATBUD_CustId, Atbud_Branchnameid,
          ATBUD_QuarterId, ATBUD_YEARId, ATBUD_CRBY, AtbuD_CrOn, ATBUD_CompId
      )
      VALUES
      (
          @NewId, @NewId, 'Net Income', @NewId, @CustId, @BranchId,
          @DurationId, @YearId, @UserId, GETDATE(), @CompId
      );";

            await connection.ExecuteAsync(insertDetailQuery, new
            {
                NewId = newId,
                CustId = custId,
                BranchId = branchId,
                DurationId = durationId,
                YearId = yearId,
                UserId = userId,
                CompId = compId
            });

            return true;
        }


        //SaveMappingTransactionDetails
        public async Task<int[]> SaveMappingTransactionDetailsAsync(SaveMappingTransactionDetailsDto dto)
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
            string Transaction;
            int updateOrSave = 0, oper = 0;
            try
            {
                // --- Save Journal Entry Master ---
                dto.Acc_JE_TransactionNo = await GenerateTransactionNoAsync(dto.Acc_JE_YearID, dto.Acc_JE_Party, dto.Acc_JE_QuarterId, dto.acc_JE_BranchId);
                Transaction = dto.Acc_JE_TransactionNo;
                int iPKId = dto.Acc_JE_ID;

                using (var cmdMaster = new SqlCommand("spAcc_JE_Master", connection, transaction))
                {
                    cmdMaster.CommandType = CommandType.StoredProcedure;

                    cmdMaster.Parameters.AddWithValue("@Acc_JE_ID", dto.Acc_JE_ID);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_TransactionNo", dto.Acc_JE_TransactionNo ?? string.Empty);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_Party", dto.Acc_JE_Party);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_Location", dto.Acc_JE_Location);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_BillType", dto.Acc_JE_BillType);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_BillNo", dto.Acc_JE_BillNo ?? string.Empty);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_BillDate", dto.Acc_JE_BillDate);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_BillAmount", dto.Acc_JE_BillAmount);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_AdvanceAmount", dto.Acc_JE_AdvanceAmount);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_AdvanceNaration", dto.Acc_JE_AdvanceNaration ?? string.Empty);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_BalanceAmount", dto.Acc_JE_BalanceAmount);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_NetAmount", dto.Acc_JE_NetAmount);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_PaymentNarration", dto.Acc_JE_PaymentNarration ?? string.Empty);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_ChequeNo", dto.Acc_JE_ChequeNo ?? string.Empty);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_ChequeDate", dto.Acc_JE_ChequeDate);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_IFSCCode", dto.Acc_JE_IFSCCode ?? string.Empty);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_BankName", dto.Acc_JE_BankName ?? string.Empty);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_BranchName", dto.Acc_JE_BranchName ?? string.Empty);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_CreatedBy", dto.Acc_JE_CreatedBy);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_YearID", dto.Acc_JE_YearID);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_CompID", dto.Acc_JE_CompID);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_Status", dto.Acc_JE_Status ?? string.Empty);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_Operation", dto.Acc_JE_Operation ?? string.Empty);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_IPAddress", dto.Acc_JE_IPAddress ?? string.Empty);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_BillCreatedDate", dto.Acc_JE_BillCreatedDate);
                    cmdMaster.Parameters.AddWithValue("@acc_JE_BranchId", dto.acc_JE_BranchId);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_QuarterId", dto.Acc_JE_QuarterId);
                    cmdMaster.Parameters.AddWithValue("@Acc_JE_Comnments", dto.Acc_JE_Comments ?? string.Empty);

                    var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    cmdMaster.Parameters.Add(updateOrSaveParam);
                    cmdMaster.Parameters.Add(operParam);

                    await cmdMaster.ExecuteNonQueryAsync();

                    updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                    oper = (int)(operParam.Value ?? 0);
                }

                // ✅ Handle Transactions for the single DTO
                var t = dto.Transactions; // assumes frontend always sends one transaction

                using (var cmdDetail = new SqlCommand("spAcc_JETransactions_Details", connection, transaction))
                {
                    cmdDetail.CommandType = CommandType.StoredProcedure;

                    cmdDetail.Parameters.AddWithValue("@AJTB_ID", t.AJTB_ID);
                    cmdDetail.Parameters.AddWithValue("@AJTB_MasID", oper);
                    cmdDetail.Parameters.AddWithValue("@AJTB_TranscNo", Transaction ?? string.Empty);
                    cmdDetail.Parameters.AddWithValue("@AJTB_CustId", t.AJTB_CustId);
                    cmdDetail.Parameters.AddWithValue("@AJTB_ScheduleTypeid", t.AJTB_ScheduleTypeid);
                    cmdDetail.Parameters.AddWithValue("@AJTB_Deschead", t.AJTB_Deschead);
                    cmdDetail.Parameters.AddWithValue("@AJTB_Desc", t.AJTB_Desc);
                    cmdDetail.Parameters.AddWithValue("@AJTB_Debit", t.AJTB_Debit);
                    cmdDetail.Parameters.AddWithValue("@AJTB_Credit", t.AJTB_Credit);
                    cmdDetail.Parameters.AddWithValue("@AJTB_CreatedBy", t.AJTB_CreatedBy);
                    cmdDetail.Parameters.AddWithValue("@AJTB_UpdatedBy", t.AJTB_UpdatedBy);
                    cmdDetail.Parameters.AddWithValue("@AJTB_Status", t.AJTB_Status ?? string.Empty);
                    cmdDetail.Parameters.AddWithValue("@AJTB_IPAddress", t.AJTB_IPAddress ?? string.Empty);
                    cmdDetail.Parameters.AddWithValue("@AJTB_CompID", t.AJTB_CompID);
                    cmdDetail.Parameters.AddWithValue("@AJTB_YearID", t.AJTB_YearID);
                    cmdDetail.Parameters.AddWithValue("@AJTB_BillType", t.AJTB_BillType);
                    cmdDetail.Parameters.AddWithValue("@AJTB_DescName", t.AJTB_DescName ?? string.Empty);
                    cmdDetail.Parameters.AddWithValue("@AJTB_BranchId", t.AJTB_BranchId);
                    cmdDetail.Parameters.AddWithValue("@AJTB_QuarterId", t.AJTB_QuarterId);

                    var updateOrSaveDetail = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var operDetail = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    cmdDetail.Parameters.Add(updateOrSaveDetail);
                    cmdDetail.Parameters.Add(operDetail);

                    await cmdDetail.ExecuteNonQueryAsync();

                    if (dto.Acc_JE_Status == "A")
                    {
                        await UpdateJeDetAsync(
                            t.AJTB_CompID,
                            t.AJTB_YearID,
                            oper,                  // ✅ use new Master ID
                            t.AJTB_CustId,
                            (t.AJTB_Debit > 0 ? 0 : 1),  // 0=Debit, 1=Credit
                            (t.AJTB_Debit > 0 ? t.AJTB_Debit : t.AJTB_Credit),
                            t.AJTB_BranchId,
                            t.AJTB_Debit,
                            t.AJTB_Credit,
                            t.AJTB_QuarterId,
                            t.AJTB_Deschead,
                            t.AJTB_Desc,
                            t.AJTB_DescName
                        );
                    }
                }


                transaction.Commit();
                return new int[] { updateOrSave, oper };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task UpdateJeDetAsync(
   int compId,
   int yearId,
   int id,                  // JE Master ID
   int custId,
   int transId,             // 0 = Debit, 1 = Credit
   decimal transAmt,        // Net Transaction Amount
   int branchId,
   decimal transDbAmt,      // Transaction Debit
   decimal transCrAmt,      // Transaction Credit
   int durtnId,             // Quarter Id
   int deschead,            // ATBU_ID → goes to AJTB_Deschead
   int descId,              // Duplicate of ATBU_ID → goes to AJTB_Desc
   string descName)        // ATBU_Description → goes to AJTB_DescName

        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            // 🔹 Select the Trail Balance row for given parameters
            string sql = @"
        SELECT TOP 1 *
        FROM Acc_TrailBalance_Upload
        WHERE ATBU_CustId = @CustId
          AND ATBU_CompID = @CompId
          AND ATBU_QuarterId = @DurtnId
          AND ATBU_ID = @Deschead
          AND ATBU_BranchId = @BranchId";

            var row = await conn.QueryFirstOrDefaultAsync(sql, new
            {
                CustId = custId,
                CompId = compId,
                DurtnId = durtnId,
                BranchId = branchId,
                Deschead = deschead
            });

            if (row == null) return;

            // 🔹 Safe cast of values
            decimal debitAmt = (decimal?)row.ATBU_Closing_TotalDebit_Amount ?? 0m;
            decimal creditAmt = (decimal?)row.ATBU_Closing_TotalCredit_Amount ?? 0m;

            string updateSql = string.Empty;

            // 🔹 Debit Transaction (transId = 0)
            if (transId == 0)
            {
                if (debitAmt != 0)
                {
                    // Case 1: Existing Debit ≠ 0 → Add incoming Debit
                    debitAmt += transDbAmt;

                    if (debitAmt >= 0)
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalDebit_Amount = @DebitAmt,
                        ATBU_Closing_TotalCredit_Amount = 0
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalCredit_Amount = @DebitAmt,
                        ATBU_Closing_TotalDebit_Amount = 0
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                        debitAmt = Math.Abs(debitAmt);
                    }
                }
                else if (creditAmt != 0)
                {
                    // Case 2: Existing Credit ≠ 0 → Subtract incoming Debit
                    debitAmt = creditAmt - transDbAmt;

                    if (debitAmt >= 0)
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalCredit_Amount = @DebitAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalDebit_Amount = @DebitAmt,
                        ATBU_Closing_TotalCredit_Amount = 0
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                        debitAmt = Math.Abs(debitAmt);
                    }
                }
                else
                {
                    // Case 3: Both Debit & Credit = 0 → Just add to Debit
                    debitAmt += transDbAmt;

                    if (debitAmt >= 0)
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalDebit_Amount = @DebitAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalCredit_Amount = @DebitAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                        debitAmt = Math.Abs(debitAmt);
                    }
                }
            }
            // 🔹 Credit Transaction (transId = 1)
            else if (transId == 1)
            {
                if (creditAmt != 0)
                {
                    // Case 1: Existing Credit ≠ 0 → Add incoming Credit
                    creditAmt += transCrAmt;

                    if (creditAmt >= 0)
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalCredit_Amount = @CreditAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalDebit_Amount = @CreditAmt,
                        ATBU_Closing_TotalCredit_Amount = 0.00
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                        creditAmt = Math.Abs(creditAmt);
                    }
                }
                else if (debitAmt != 0)
                {
                    // Case 2: Existing Debit ≠ 0 → Subtract incoming Credit
                    creditAmt = debitAmt - transCrAmt;

                    if (creditAmt >= 0)
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalDebit_Amount = @CreditAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalCredit_Amount = @CreditAmt,
                        ATBU_Closing_TotalDebit_Amount = 0.00
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                        creditAmt = Math.Abs(creditAmt);
                    }
                }
                else
                {
                    // Case 3: Both Debit & Credit = 0 → Just add to Credit
                    creditAmt += transCrAmt;

                    if (creditAmt >= 0)
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalCredit_Amount = @CreditAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                    }
                    else
                    {
                        updateSql = @"
                    UPDATE Acc_TrailBalance_Upload
                    SET ATBU_Closing_TotalDebit_Amount = @CreditAmt
                    WHERE ATBU_CustId = @CustId
                      AND ATBU_CompID = @CompId
                      AND ATBU_QuarterId = @DurtnId
                      AND ATBU_ID = @Deschead
                      AND ATBU_BranchId = @BranchId";
                        creditAmt = Math.Abs(creditAmt);
                    }
                }
            }

            // 🔹 Execute Update if SQL was built
            if (!string.IsNullOrEmpty(updateSql))
            {
                await conn.ExecuteAsync(updateSql, new
                {
                    CustId = custId,
                    CompId = compId,
                    DurtnId = durtnId,
                    BranchId = branchId,
                    DebitAmt = debitAmt,
                    CreditAmt = creditAmt,
                    Deschead = deschead,
                    DescId = descId,
                    DescName = descName
                });
            }
        }
        public async Task<string> GenerateTransactionNoAsync(int Yearid, int Custid, int duration, int branchid)
        {
            try
            {
                string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

                if (string.IsNullOrEmpty(dbName))
                    throw new Exception("CustomerCode is missing in session. Please log in again.");

                // ✅ Step 2: Get the connection string
                var connectionString = _configuration.GetConnectionString(dbName);

                // ✅ Step 3: Use SqlConnection
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string sql = $@"
                SELECT ISNULL(MAX(Acc_JE_ID) + 1, 1) 
                FROM Acc_JE_Master 
                WHERE Acc_JE_YearID = {Yearid} 
                  AND Acc_JE_Party = {Custid} 
                  AND Acc_JE_QuarterId = {duration} 
                  AND acc_JE_BranchId = {branchid}";

                int iMax = await connection.ExecuteScalarAsync<int>(sql);

                string prefix = $"JE00-{iMax}";

                return prefix;
            }
            catch (Exception ex)
            {
                throw; // or handle logging
            }
        }
    }
}