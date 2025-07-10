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
namespace TracePca.Service.FIN_statement
{
    public class ScheduleMappingService : ScheduleMappingInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        private readonly IWebHostEnvironment _env;
        private readonly SqlConnection _db;

        public ScheduleMappingService(Trdmyus1Context dbcontext, IConfiguration configuration, IWebHostEnvironment env)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _env = env;
        }  

        public ScheduleMappingService(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;

        }

        //GetCustomersName
        public async Task<IEnumerable<CustDto>> GetCustomerNameAsync(int CompId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            Cust_Id,
            Cust_Name 
        FROM SAD_CUSTOMER_MASTER
        WHERE cust_Compid = @CompID";

            await connection.OpenAsync();

            return await connection.QueryAsync<CustDto>(query, new { CompID = CompId });
        }

        //GetFinancialYear
        public async Task<IEnumerable<FinancialYearDto>> GetFinancialYearAsync(int CompId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            YMS_YEARID,
            YMS_ID 
        FROM YEAR_MASTER 
        WHERE YMS_FROMDATE < DATEADD(year, +1, GETDATE()) 
          AND YMS_CompId = @CompID 
        ORDER BY YMS_ID DESC";

            await connection.OpenAsync();

            return await connection.QueryAsync<FinancialYearDto>(query, new { CompID = CompId });
        }

        //GetDuration
        public async Task<int?> GetCustomerDurationIdAsync(int CompId, int CustId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var query = "SELECT Cust_DurtnId FROM SAD_CUSTOMER_MASTER WHERE CUST_CompID = @CompId AND CUST_ID = @CustId";

            var parameters = new { CompId = CompId, CustId = CustId };
            var result = await connection.QueryFirstOrDefaultAsync<int?>(query, parameters);

            return result;
        }

        //GetBranchName
        public async Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(int CompId, int CustId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            Mas_Id AS Branchid, 
            Mas_Description AS BranchName 
        FROM SAD_CUST_LOCATION 
        WHERE Mas_CompID = @compId AND Mas_CustID = @custId";

            await connection.OpenAsync();

            return await connection.QueryAsync<CustBranchDto>(query, new { CompId, CustId });
        }

        //GetScheduleHeading
        public async Task<IEnumerable<ScheduleHeadingDto>> GetScheduleHeadingAsync(int CompId, int CustId, int ScheduleTypeId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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

            await connection.OpenAsync();

            return await connection.QueryAsync<ScheduleHeadingDto>(query, new { CompId, CustId, ScheduleTypeId });
        }

        //GetScheduleSub-Heading
        public async Task<IEnumerable<ScheduleSubHeadingDto>> GetScheduleSubHeadingAsync(int CompId, int CustId, int ScheduleTypeId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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

            await connection.OpenAsync();

            return await connection.QueryAsync<ScheduleSubHeadingDto>(query, new { CompId, CustId, ScheduleTypeId });
        }

        //GetScheduleItem
        public async Task<IEnumerable<ScheduleItemDto>> GetScheduleItemAsync(int CompId, int CustId, int ScheduleTypeId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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

            await connection.OpenAsync();

            return await connection.QueryAsync<ScheduleItemDto>(query, new { CompId, CustId, ScheduleTypeId });
        }

        //GetScheduleSub-Item
        public async Task<IEnumerable<ScheduleSubItemDto>> GetScheduleSubItemAsync(int CompId, int CustId, int ScheduleTypeId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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

            await connection.OpenAsync();
            return await connection.QueryAsync<ScheduleSubItemDto>(query, new { CompId, CustId, ScheduleTypeId });
        }

        ////SaveOrUpdateTrailBalanceUpload
        //public async Task<int[]> SaveTrailBalanceUploadAsync(int iCompId, TrailBalanceUploadDto dto)
        //{
        //    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        await connection.OpenAsync();

        //        using (var command = new SqlCommand("spAcc_TrailBalance_Upload", connection))
        //        {
        //            command.CommandType = CommandType.StoredProcedure;

        //            // Input parameters
        //            command.Parameters.AddWithValue("@ATBU_ID", dto.ATBU_ID);
        //            command.Parameters.AddWithValue("@ATBU_CODE", dto.ATBU_CODE ?? string.Empty);
        //            command.Parameters.AddWithValue("@ATBU_Description", dto.ATBU_Description ?? string.Empty);
        //            command.Parameters.AddWithValue("@ATBU_CustId", dto.ATBU_CustId);
        //            command.Parameters.AddWithValue("@ATBU_Opening_Debit_Amount", dto.ATBU_Opening_Debit_Amount);
        //            command.Parameters.AddWithValue("@ATBU_Opening_Credit_Amount", dto.ATBU_Opening_Credit_Amount);
        //            command.Parameters.AddWithValue("@ATBU_TR_Debit_Amount", dto.ATBU_TR_Debit_Amount);
        //            command.Parameters.AddWithValue("@ATBU_TR_Credit_Amount", dto.ATBU_TR_Credit_Amount);
        //            command.Parameters.AddWithValue("@ATBU_Closing_Debit_Amount", dto.ATBU_Closing_Debit_Amount);
        //            command.Parameters.AddWithValue("@ATBU_Closing_Credit_Amount", dto.ATBU_Closing_Credit_Amount);
        //            command.Parameters.AddWithValue("@ATBU_DELFLG", "A");
        //            command.Parameters.AddWithValue("@ATBU_CRBY", dto.ATBU_CRBY);
        //            command.Parameters.AddWithValue("@ATBU_STATUS", "C");
        //            command.Parameters.AddWithValue("@ATBU_UPDATEDBY", dto.ATBU_UPDATEDBY);
        //            command.Parameters.AddWithValue("@ATBU_IPAddress", dto.ATBU_IPAddress ?? string.Empty);
        //            command.Parameters.AddWithValue("@ATBU_CompId", dto.ATBU_CompId);
        //            command.Parameters.AddWithValue("@ATBU_YEARId", dto.ATBU_YEARId);
        //            command.Parameters.AddWithValue("@ATBU_Branchid", dto.ATBU_Branchid);
        //            command.Parameters.AddWithValue("@ATBU_QuarterId", dto.ATBU_QuarterId);

        //            // Output parameters
        //            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            var operParam = new SqlParameter("@iOper", SqlDbType.Int)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            command.Parameters.Add(updateOrSaveParam);
        //            command.Parameters.Add(operParam);

        //            // Optional: update flag if needed before SP call (as in your example)
        //            var query = @"
        //        UPDATE Acc_TrailBalance_Upload
        //        SET ATBU_DelFlg = 'A'
        //        WHERE ATBU_CompId = @CompId AND ATBU_ID = @ATBU_ID";

        //            await connection.ExecuteAsync(query, new { CompId = iCompId, ATBU_ID = dto.ATBU_ID });

        //            try
        //            {
        //                await command.ExecuteNonQueryAsync();

        //                int updateOrSave = (int)updateOrSaveParam.Value;
        //                int oper = (int)operParam.Value;

        //                return new int[] { updateOrSave, oper };
        //            }
        //            catch (Exception ex)
        //            {
        //                // Optional: log exception
        //                throw;
        //            }
        //        }
        //    }
        //}

        ////SaveOrUpdateTrailBalanceUploadDetails
        //public async Task<int[]> SaveTrailBalanceUploadDetailsAsync(int iCompId, TrailBalanceUploadDetailsDto dto)
        //{
        //    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        await connection.OpenAsync();

        //        using (var command = new SqlCommand("spAcc_TrailBalance_Upload_Details", connection))
        //        {
        //            command.CommandType = CommandType.StoredProcedure;

        //            // Input parameters
        //            command.Parameters.AddWithValue("@ATBUD_ID", dto.ATBUD_ID);
        //            command.Parameters.AddWithValue("@ATBUD_Masid", dto.ATBUD_Masid);
        //            command.Parameters.AddWithValue("@ATBUD_CODE", dto.ATBUD_CODE ?? string.Empty);
        //            command.Parameters.AddWithValue("@ATBUD_Description", dto.ATBUD_Description ?? string.Empty);
        //            command.Parameters.AddWithValue("@ATBUD_CustId", dto.ATBUD_CustId);
        //            command.Parameters.AddWithValue("@ATBUD_SChedule_Type", dto.ATBUD_SChedule_Type);
        //            command.Parameters.AddWithValue("@ATBUD_Branchid", dto.ATBUD_Branchid);
        //            command.Parameters.AddWithValue("@ATBUD_QuarterId", dto.ATBUD_QuarterId);
        //            command.Parameters.AddWithValue("@ATBUD_Company_Type", dto.ATBUD_Company_Type);
        //            command.Parameters.AddWithValue("@ATBUD_Headingid", dto.ATBUD_Headingid);
        //            command.Parameters.AddWithValue("@ATBUD_Subheading", dto.ATBUD_Subheading);
        //            command.Parameters.AddWithValue("@ATBUD_itemid", dto.ATBUD_itemid);
        //            command.Parameters.AddWithValue("@ATBUD_Subitemid", dto.ATBUD_Subitemid);
        //            command.Parameters.AddWithValue("@ATBUD_DELFLG", dto.ATBUD_DELFLG ?? string.Empty);
        //            command.Parameters.AddWithValue("@ATBUD_CRBY", dto.ATBUD_CRBY);
        //            command.Parameters.AddWithValue("@ATBUD_UPDATEDBY", dto.ATBUD_UPDATEDBY);
        //            command.Parameters.AddWithValue("@ATBUD_STATUS", dto.ATBUD_STATUS ?? string.Empty);
        //            command.Parameters.AddWithValue("@ATBUD_Progress", dto.ATBUD_Progress ?? string.Empty);
        //            command.Parameters.AddWithValue("@ATBUD_IPAddress", dto.ATBUD_IPAddress ?? string.Empty);
        //            command.Parameters.AddWithValue("@ATBUD_CompId", dto.ATBUD_CompId);
        //            command.Parameters.AddWithValue("@ATBUD_YEARId", dto.ATBUD_YEARId);

        //            // Output parameters
        //            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            var operParam = new SqlParameter("@iOper", SqlDbType.Int)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            command.Parameters.Add(updateOrSaveParam);
        //            command.Parameters.Add(operParam);

        //            // Optional: update flag if needed before SP call (as in your example)
        //            var query = @"
        //        UPDATE Acc_TrailBalance_Upload_Details
        //        SET ATBUD_DelFlg = 'A'
        //        WHERE ATBUD_CompId = @CompId AND ATBUD_ID = @ATBUD_ID";

        //            await connection.ExecuteAsync(query, new { CompId = iCompId, ATBUD_ID = dto.ATBUD_ID });

        //            try
        //            {
        //                await command.ExecuteNonQueryAsync();

        //                int updateOrSave = (int)updateOrSaveParam.Value;
        //                int oper = (int)operParam.Value;

        //                return new int[] { updateOrSave, oper };
        //            }
        //            catch (Exception ex)
        //            {
        //                // Optional: log exception
        //                throw;
        //            }
        //        }
        //    }
        //}

        //GetTotalAmount
        public async Task<IEnumerable<CustCOASummaryDto>>  GetCustCOAMasterDetailsAsync(int CompId, int CustId, int YearId, int BranchId, int DurationId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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

            await connection.OpenAsync();
            return await connection.QueryAsync<CustCOASummaryDto>(query, new{ CompId, CustId, YearId, BranchId, DurationId});
        }

        //GetTrailBalance(Grid)
        public async Task<IEnumerable<CustCOADetailsDto>> GetCustCOADetailsAsync(
    int CompId, int CustId, int YearId, int ScheduleTypeId, int Unmapped, int BranchId, int DurationId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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

            await connection.OpenAsync();
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

        //SaveScheduleTemplate
        public async Task<int[]> UploadTrialBalanceExcelAsync(int CompanyId, AccTrailBalanceUploadBatchDto dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Step 1: Insert master record using spAcc_TrailBalance_Upload
                var masterParams = new DynamicParameters();
                masterParams.Add("@ATBU_ID", dto.ATBU_ID); // Assuming new insert
                masterParams.Add("@ATBU_CODE", dto.ATBU_CODE ?? string.Empty);
                masterParams.Add("@ATBU_Description", dto.ATBU_Description ?? string.Empty);
                masterParams.Add("@ATBU_CustId", dto.ATBU_CustId);
                masterParams.Add("@ATBU_Opening_Debit_Amount", dto.ATBU_Opening_Debit_Amount);
                masterParams.Add("@ATBU_Opening_Credit_Amount", dto.ATBU_Opening_Credit_Amount);
                masterParams.Add("@ATBU_TR_Debit_Amount", dto.ATBU_TR_Debit_Amount);
                masterParams.Add("@ATBU_TR_Credit_Amount", dto.ATBU_TR_Credit_Amount);
                masterParams.Add("@ATBU_Closing_Debit_Amount", dto.ATBU_Closing_Debit_Amount);
                masterParams.Add("@ATBU_Closing_Credit_Amount", dto.ATBU_Closing_Credit_Amount);
                masterParams.Add("@ATBU_DELFLG", "A");
                masterParams.Add("@ATBU_CRBY", dto.ATBU_CRBY);
                masterParams.Add("@ATBU_STATUS", "C");
                masterParams.Add("@ATBU_UPDATEDBY", dto.ATBU_UPDATEDBY);
                masterParams.Add("@ATBU_IPAddress", dto.ATBU_IPAddress ?? string.Empty);
                masterParams.Add("@ATBU_CompId", dto.ATBU_CompId);
                masterParams.Add("@ATBU_YEARId", dto.ATBU_YEARId);
                masterParams.Add("@ATBU_Branchid", dto.ATBU_Branchid);
                masterParams.Add("@ATBU_QuarterId", dto.ATBU_QuarterId);
                // Output
                masterParams.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                masterParams.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("spAcc_TrailBalance_Upload", masterParams, transaction, commandType: CommandType.StoredProcedure);

                int masterId = masterParams.Get<int>("@iOper");

                // Step 2: Insert detail rows using spAcc_TrailBalance_Upload_Details
                var detailIds = new List<int>();

                foreach (var row in dto.Rows)
                {
                    var detailParams = new DynamicParameters();
                    detailParams.Add("@ATBUD_ID", dto.ATBUD_ID);
                    detailParams.Add("@ATBUD_Masid", dto.ATBUD_Masid);
                    detailParams.Add("@ATBUD_CODE", dto.ATBUD_CODE ?? string.Empty);
                    detailParams.Add("@ATBUD_Description", dto.ATBUD_Description ?? string.Empty);
                    detailParams.Add("@ATBUD_CustId", dto.ATBUD_CustId);
                    detailParams.Add("@ATBUD_SChedule_Type", dto.ATBUD_SChedule_Type);
                    detailParams.Add("@ATBUD_Branchid", dto.ATBUD_Branchid);
                    detailParams.Add("@ATBUD_QuarterId", dto.ATBUD_QuarterId);
                    detailParams.Add("@ATBUD_Company_Type", dto.ATBUD_Company_Type);
                    detailParams.Add("@ATBUD_Headingid", dto.ATBUD_Headingid);
                    detailParams.Add("@ATBUD_Subheading", dto.ATBUD_Subheading);
                    detailParams.Add("@ATBUD_itemid", dto.ATBUD_itemid);
                    detailParams.Add("@ATBUD_SubItemId", dto.ATBUD_SubItemId);
                    detailParams.Add("@ATBUD_DELFLG", "A");
                    detailParams.Add("@ATBUD_CRBY", dto.ATBUD_CRBY);
                    detailParams.Add("@ATBUD_STATUS", "C");
                    detailParams.Add("@ATBUD_Progress", "Uploaded");
                    detailParams.Add("@ATBUD_UPDATEDBY", dto.ATBUD_UPDATEDBY);
                    detailParams.Add("@ATBUD_IPAddress", dto.ATBUD_IPAddress ?? string.Empty);
                    detailParams.Add("@ATBUD_CompId", dto.ATBUD_CompId);
                    detailParams.Add("@ATBUD_YEARId", dto.ATBU_YEARId);
                    detailParams.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    detailParams.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync("spAcc_TrailBalance_Upload_Details", detailParams, transaction, commandType: CommandType.StoredProcedure);
                    int detailId = detailParams.Get<int>("@iOper");
                    detailIds.Add(row.ATBU_ID);
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

        //UploadExcelFile
        //public async Task<ExcelUploadResultDto> UploadScheduleExcelAsync(
        //IFormFile file,
        //int clientId,
        //int branchId,
        //int yearId,
        //int quarter,
        //string accessCode,
        //int accessCodeId,
        //string username)
        //       {
        //           var result = new ExcelUploadResultDto();

        //           if (file == null || file.Length == 0)
        //               throw new ArgumentException("No file uploaded.");

        //           var ext = Path.GetExtension(file.FileName).ToUpper();
        //           if (ext != ".XLS" && ext != ".XLSX")
        //               throw new ArgumentException("Invalid file type. Only .xls or .xlsx allowed.");

        //           if (string.IsNullOrWhiteSpace(_env.WebRootPath))
        //               throw new InvalidOperationException("WebRootPath is not set.");

        //           // Save file
        //           var uploadPath = Path.Combine(_env.WebRootPath, "Uploads", username);
        //           if (!Directory.Exists(uploadPath))
        //               Directory.CreateDirectory(uploadPath);

        //           var safeFileName = $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}{ext}";
        //           var filePath = Path.Combine(uploadPath, safeFileName);

        //           using (var stream = new FileStream(filePath, FileMode.Create))
        //           {
        //               await file.CopyToAsync(stream);
        //           }

        //           // Extract sheet names
        //           result.SheetNames = GetExcelSheetNames(filePath);

        //           // Check existing data via stored procedure
        //           var param = new DynamicParameters();
        //           param.Add("@CustId", clientId);
        //           param.Add("@BranchId", branchId);
        //           param.Add("@YearId", yearId);
        //           param.Add("@Quarter", quarter);
        //           param.Add("@AccessCodeId", accessCodeId);
        //           param.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //           await _db.ExecuteAsync("sp_CheckCustData", param, commandType: CommandType.StoredProcedure);

        //           var exists = param.Get<int>("@Result") != 0;
        //           result.IsExistingData = exists;
        //           result.Message = exists
        //               ? $"Data already exists for client in year {yearId}. Click Yes to replace."
        //               : "Excel file uploaded successfully. Select sheet to continue.";

        //           return result;
        //       }

        //       private List<string> GetExcelSheetNames(string filePath)
        //       {
        //           System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        //           var sheetNames = new List<string>();

        //           using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        //           {
        //               using var reader = ExcelReaderFactory.CreateReader(stream);
        //               var result = reader.AsDataSet();

        //               foreach (DataTable table in result.Tables)
        //               {
        //                   sheetNames.Add(table.TableName);
        //               }
        //           }

        //           return sheetNames;
        //       }

        //FreezeForPreviousDuration
        public async Task<int[]> FreezePreviousYearTrialBalanceAsync(FreezePreviousDurationRequestDto input)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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

        ////SaveTrailBalanceDetails
        //public async Task<int[]> SaveTrailBalanceDetailsAsync(int CompId, TrailBalanceDetailsDto dto)
        //{
        //    using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    await connection.OpenAsync();
        //    using var transaction = connection.BeginTransaction();

        //    try
        //    {
        //        int iPKId = dto.ATBU_ID;
        //        int updateOrSave, oper;

        //        // --- Save Trail Balance Upload (Main) ---
        //        using (var uploadCommand = new SqlCommand("spAcc_TrailBalance_Upload", connection, transaction))
        //        {
        //            uploadCommand.CommandType = CommandType.StoredProcedure;

        //            uploadCommand.Parameters.AddWithValue("@ATBU_ID", dto.ATBU_ID);
        //            uploadCommand.Parameters.AddWithValue("@ATBU_CODE", dto.ATBU_CODE ?? string.Empty);
        //            uploadCommand.Parameters.AddWithValue("@ATBU_Description", dto.ATBU_Description ?? string.Empty);
        //            uploadCommand.Parameters.AddWithValue("@ATBU_CustId", dto.ATBU_CustId);
        //            uploadCommand.Parameters.AddWithValue("@ATBU_Opening_Debit_Amount", dto.ATBU_Opening_Debit_Amount);
        //            uploadCommand.Parameters.AddWithValue("@ATBU_Opening_Credit_Amount", dto.ATBU_Opening_Credit_Amount);
        //            uploadCommand.Parameters.AddWithValue("@ATBU_TR_Debit_Amount", dto.ATBU_TR_Debit_Amount);
        //            uploadCommand.Parameters.AddWithValue("@ATBU_TR_Credit_Amount", dto.ATBU_TR_Credit_Amount);
        //            uploadCommand.Parameters.AddWithValue("@ATBU_Closing_Debit_Amount", dto.ATBU_Closing_Debit_Amount);
        //            uploadCommand.Parameters.AddWithValue("@ATBU_Closing_Credit_Amount", dto.ATBU_Closing_Credit_Amount);
        //            uploadCommand.Parameters.AddWithValue("@ATBU_DELFLG", "A");
        //            uploadCommand.Parameters.AddWithValue("@ATBU_CRBY", dto.ATBU_CRBY);
        //            uploadCommand.Parameters.AddWithValue("@ATBU_STATUS", "C");
        //            uploadCommand.Parameters.AddWithValue("@ATBU_UPDATEDBY", dto.ATBU_UPDATEDBY);
        //            uploadCommand.Parameters.AddWithValue("@ATBU_IPAddress", dto.ATBU_IPAddress ?? string.Empty);
        //            uploadCommand.Parameters.AddWithValue("@ATBU_CompId", dto.ATBU_CompId);
        //            uploadCommand.Parameters.AddWithValue("@ATBU_YEARId", dto.ATBU_YEARId);
        //            uploadCommand.Parameters.AddWithValue("@ATBU_Branchid", dto.ATBU_Branchid);
        //            uploadCommand.Parameters.AddWithValue("@ATBU_QuarterId", dto.ATBU_QuarterId);

        //            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
        //            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
        //            uploadCommand.Parameters.Add(updateOrSaveParam);
        //            uploadCommand.Parameters.Add(operParam);

        //            // Optional: soft update
        //            var updateQuery = @"
        //        UPDATE Acc_TrailBalance_Upload
        //        SET ATBU_DelFlg = 'A'
        //        WHERE ATBU_CompId = @CompId AND ATBU_ID = @ATBU_ID";
        //            await connection.ExecuteAsync(updateQuery, new { CompId = CompId, dto.ATBU_ID }, transaction);

        //            await uploadCommand.ExecuteNonQueryAsync();

        //            updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
        //            oper = (int)(operParam.Value ?? 0);
        //        }

        //        bool isNewUpload = dto.ATBU_ID == 0 || updateOrSave == 1;
        //        if (iPKId == 0)

        //            // --- Save Trail Balance Upload Details ---
        //            using (var detailsCommand = new SqlCommand("spAcc_TrailBalance_Upload_Details", connection, transaction))
        //        {
        //            detailsCommand.CommandType = CommandType.StoredProcedure;

        //            detailsCommand.Parameters.AddWithValue("@ATBUD_ID", dto.ATBUD_ID);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_Masid", oper); // Use main ATBU_ID as foreign key
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_CODE", dto.ATBUD_CODE ?? string.Empty);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_Description", dto.ATBUD_Description ?? string.Empty);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_CustId", dto.ATBUD_CustId);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_SChedule_Type", dto.ATBUD_SChedule_Type);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_Branchid", dto.ATBUD_Branchid);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_QuarterId", dto.ATBUD_QuarterId);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_Company_Type", dto.ATBUD_Company_Type);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_Headingid", dto.ATBUD_Headingid);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_Subheading", dto.ATBUD_Subheading);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_itemid", dto.ATBUD_itemid);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_Subitemid", dto.ATBUD_Subitemid);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_DELFLG", dto.ATBUD_DELFLG ?? string.Empty);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_CRBY", dto.ATBUD_CRBY);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_UPDATEDBY", dto.ATBUD_UPDATEDBY);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_STATUS", dto.ATBUD_STATUS ?? string.Empty);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_Progress", dto.ATBUD_Progress ?? string.Empty);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_IPAddress", dto.ATBUD_IPAddress ?? string.Empty);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_CompId", dto.ATBUD_CompId);
        //            detailsCommand.Parameters.AddWithValue("@ATBUD_YEARId", dto.ATBUD_YEARId);

        //            var updateOrSaveParamDet = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
        //            var operParamDet = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
        //            detailsCommand.Parameters.Add(updateOrSaveParamDet);
        //            detailsCommand.Parameters.Add(operParamDet);

        //            // Optional: soft update
        //            var updateDetailsQuery = @"
        //        UPDATE Acc_TrailBalance_Upload_Details
        //        SET ATBUD_DelFlg = 'A'
        //        WHERE ATBUD_CompId = @CompId AND ATBUD_ID = @ATBUD_ID";
        //            await connection.ExecuteAsync(updateDetailsQuery, new { CompId = CompId, dto.ATBUD_ID }, transaction);

        //            await detailsCommand.ExecuteNonQueryAsync();
        //        }

        //        transaction.Commit();
        //        return new int[] { updateOrSave, oper };
        //    }
        //    catch
        //    {
        //        transaction.Rollback();
        //        throw;
        //    }
        //}

        //SaveTrailBalnceDetails
        public async Task<int[]> SaveTrailBalanceDetailsAsync(int CompId, List<TrailBalanceDetailsDto> dtos)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
SELECT ASSH_ID AS Id, ASSH_Name AS Name 
FROM ACC_ScheduleSubHeading 
WHERE ASSH_HeadingID = @HeadingId 
  AND ASSH_OrgType = @OrgType 
  AND ISNULL(ASSH_DELFLG, 'A') = 'A'";

            await connection.OpenAsync();
            var result = await connection.QueryAsync<LoadSubHeadingByHeadingDto>(query, new { HeadingId = headingId, OrgType = orgType });
            return result;
        }

        //LoadItemBySubHeading
        public async Task<IEnumerable<LoadItemBySubHeadingDto>> GetItemsBySubHeadingIdAsync(int subHeadingId, int orgType)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
SELECT ASI_ID AS Id, ASI_Name AS Name 
FROM ACC_ScheduleItems 
WHERE ASI_SubHeadingID = @SubHeadingId 
  AND ASI_OrgType = @OrgType 
  AND ISNULL(ASI_DELFLG, 'A') = 'A'";

            await connection.OpenAsync();
            var result = await connection.QueryAsync<LoadItemBySubHeadingDto>(query, new { SubHeadingId = subHeadingId, OrgType = orgType });
            return result;
        }

        //LoadSubItemByItem
        public async Task<IEnumerable<LoadSubItemByItemDto>> GetSubItemsByItemIdAsync(int itemId, int orgType)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
SELECT ASSI_ID AS Id, ASSI_Name AS Name 
FROM ACC_ScheduleSubItems 
WHERE ASSI_ItemsID = @ItemId 
  AND ASSI_OrgType = @OrgType 
  AND ISNULL(ASSI_DELFLG, 'A') = 'A'";

            await connection.OpenAsync();
            var result = await connection.QueryAsync<LoadSubItemByItemDto>(query, new { ItemId = itemId, OrgType = orgType });
            return result;
        }

        //GetPreviousLoadId
        public async Task<(int? HeadingId, int? SubHeadingId, int? ItemId)> GetPreviousLoadIdAsync(
  int? subItemId = null, int? itemId = null, int? subHeadingId = null)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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



