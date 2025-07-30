using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using TracePca.Data;
using TracePca.Dto.Audit;
using TracePca.Interface;
using TracePca.Interface.Audit;
using TracePca.Models.CustomerRegistration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace TracePca.Service.Audit
{
    public class DashboardAndSchedule : AuditAndDashboardInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public DashboardAndSchedule(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbcontext = dbcontext;
            _configuration = configuration;
        }


        public async Task<List<DashboardAndScheduleDto>> GetDashboardAuditAsync(
       int? id, int? customerId, int? compId, int? financialYearId, int? loginUserId)
        
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            // Step 1: Check if user is a Partner
            var checkPartnerSql = @"
        SELECT Usr_ID 
        FROM sad_userdetails 
        WHERE usr_compID = @CompId 
          AND USR_Partner = 1 
          AND (usr_DelFlag = 'A' OR usr_DelFlag = 'B' OR usr_DelFlag = 'L') 
          AND Usr_ID = @UserId";

            var partnerParams = new
            {
                CompId = compId,
                UserId = loginUserId
            };

            var Partner = await connection.QueryFirstOrDefaultAsync<int?>(checkPartnerSql, partnerParams);
            bool loginUserIsPartner = Partner.HasValue;

            string query;
            object parameters;

            if (id.HasValue)
            {
                // Get full audit details by ID
                query = @"
SELECT 
    SA_ID,
    SA_YearID,
    YMS_Id AS FY,
    SA_AuditNo,
    SA_CustID,
    Cust_Name AS CustomerName,
    SA_IntervalId,
    SA_AuditTypeID,
    CMM_Desc AS AuditType,
    SA_EngagementPartnerID,
    SA_ReviewPartnerID,
    SA_PartnerID,
    SA_AdditionalSupportEmployeeID,
    ISNULL(SA_ScopeOfAudit, '') AS SA_ScopeOfAudit,
    ISNULL(CONVERT(VARCHAR(10), SA_StartDate, 103), '') + ' - ' + ISNULL(CONVERT(VARCHAR(10), SA_ExpCompDate, 103), '') AS AuditDate,
    ISNULL(CONVERT(VARCHAR(10), SA_RptRvDate, 103), '') AS SA_RptRvDate,
    ISNULL(CONVERT(VARCHAR(10), SA_RptFilDate, 103), '') AS SA_RptFilDate,
    ISNULL(CONVERT(VARCHAR(10), SA_MRSDate, 103), '') AS SA_MRSDate,
    ISNULL(CONVERT(VARCHAR(10), SA_StartDate, 103), '') AS SA_StartDate,
    ISNULL(CONVERT(VARCHAR(10), SA_ExpCompDate, 103), '') AS SA_ExpCompDate,
    ISNULL(CONVERT(VARCHAR(10), SA_AuditOpinionDate, 103), '') AS SA_AuditOpinionDate,
    ISNULL(CONVERT(VARCHAR(10), SA_FilingDateSEC, 103), '') AS SA_FilingDateSEC,
    ISNULL(CONVERT(VARCHAR(10), SA_MRLDate, 103), '') AS SA_MRLDate,
    ISNULL(CONVERT(VARCHAR(10), SA_FilingDatePCAOB, 103), '') AS SA_FilingDatePCAOB,
    ISNULL(CONVERT(VARCHAR(10), SA_BinderCompletedDate, 103), '') AS SA_BinderCompletedDate,
    ISNULL(SA_SignedBy, 0) AS SA_SignedBy,
    ISNULL(SA_UDIN, '') AS SA_UDIN,
    ISNULL(CONVERT(VARCHAR(10), SA_UDINdate, 103), '') AS SA_UDINdate,
    SA_Status AS StatusID
FROM StandardAudit_Schedule
JOIN Year_Master ON SA_YearID = YMS_YearID
JOIN Content_Management_Master ON CMM_ID = SA_AuditTypeID
JOIN SAD_CUSTOMER_MASTER ON Cust_Id = SA_CustID
WHERE SA_ID = @Id AND SA_CompID = @CompId";

                parameters = new
                {
                    Id = id.Value,
                    CompId = compId,
                    LoginUserID = loginUserId
                };
            }
            else
            {
                // Dashboard style list with optional filters
                query = @$"
WITH RankedAudit AS (
    SELECT DISTINCT 
        b.SA_ID AS SrNo,
        Cust_ID AS CustID,
        Cust_Name AS CustomerName,
        CONCAT(SUBSTRING(Cust_Name, 0, 25), '....') AS CustomerShortName,
        ISNULL(CMM_ID, 0) AS CheckPointID,
        CMM_Desc AS AuditType,
        ISNULL(b.SA_ID, 0) AS AuditID,
        ISNULL(b.SA_AuditNo, '-') AS AuditNo,
        ISNULL(b.SA_Status, '-') AS StatusID,
        ISNULL(FORMAT(b.SA_StartDate, 'dd-MMM-yyyy'), '') + ' - ' + ISNULL(FORMAT(b.SA_ExpCompDate, 'dd-MMM-yyyy'), '') AS AuditDate,
        CASE 
            WHEN b.SA_Status = 1 THEN 'Scheduled'
            WHEN b.SA_Status = 2 THEN 'Communication with Client'
            WHEN b.SA_Status = 3 THEN 'TBR'
            WHEN b.SA_Status = 4 THEN 'Conduct Audit' 
            WHEN b.SA_Status = 5 THEN 'Report'
            WHEN b.SA_Status = 10 THEN 'Completed'
            ELSE 'Audit Started' 
        END AS AuditStatus,
        Partner = ISNULL(STUFF((
            SELECT DISTINCT '; ' + CAST(usr_FullName AS VARCHAR(MAX))
            FROM Sad_UserDetails 
            WHERE usr_id IN (
                SELECT value 
                FROM STRING_SPLIT((
                    SELECT STUFF(
                        LEFT(a.SA_PartnerID, LEN(a.SA_PartnerID) - PATINDEX('%[^,]%', REVERSE(a.SA_PartnerID)) + 1),
                        1,
                        PATINDEX('%[^,]%', a.SA_PartnerID) - 1,
                        ''
                    )
                    FROM StandardAudit_Schedule a 
                    WHERE SA_ID = b.SA_ID
                ), ',')
            )
        FOR XML PATH('')), 1, 1, ''), '-') 
    FROM SAD_CUST_LOE
    JOIN SAD_CUSTOMER_MASTER ON Cust_Id = LOE_CustomerId
    JOIN Content_Management_Master ON CMM_ID = LOE_ServiceTypeId
    JOIN StandardAudit_Schedule b ON CMM_ID = b.SA_AuditTypeID 
        AND Cust_Id = b.SA_CustID 
        AND b.SA_CompID = @CompId
    WHERE LOE_CompID = @CompId
    {(financialYearId > 0 ? "AND b.SA_YearID = @FinancialYearID AND LOE_YearId = @FinancialYearID" : "")}
    {(customerId > 0 ? "AND b.SA_CustID = @CustomerId AND LOE_CustomerId = @CustomerId" : "")}
    {(loginUserIsPartner == false
                ? @"AND (
            CONCAT(',', b.SA_AdditionalSupportEmployeeID, ',') LIKE ('%,' + CAST(@LoginUserID AS VARCHAR) + ',%') 
            OR CONCAT(',', b.SA_EngagementPartnerID, ',') LIKE ('%,' + CAST(@LoginUserID AS VARCHAR) + ',%') 
            OR CONCAT(',', b.SA_ReviewPartnerID, ',') LIKE ('%,' + CAST(@LoginUserID AS VARCHAR) + ',%') 
            OR CONCAT(',', b.SA_PartnerID, ',') LIKE ('%,' + CAST(@LoginUserID AS VARCHAR) + ',%')
        )"
                : "")}
)
SELECT 
    ROW_NUMBER() OVER (ORDER BY SrNo DESC) AS SrNo,
    AuditID,
    CustID,
    CustomerName,
    CustomerShortName,
    CheckPointID,
    AuditType,
    AuditNo,
    StatusID,
    AuditDate,
    AuditStatus,
    Partner
FROM RankedAudit
ORDER BY SrNo";

                parameters = new
                {
                    CompId = compId,
                    FinancialYearID = financialYearId ?? 0,
                    CustomerId = customerId ?? 0,
                    LoginUserID = loginUserId
                };
            }

            var result = await connection.QueryAsync<DashboardAndScheduleDto>(query, parameters);
            return result.ToList();
        }


        public async Task<List<UserDto>> GetUsersByRoleAsync(int compId, string role)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));
            string query;
            object parameters;

            if (role.Equals("Assigned", StringComparison.OrdinalIgnoreCase))
            {
                query = @"
            SELECT Usr_ID AS UserId,
                   (Usr_FullName + ' - ' + Usr_Code) AS FullName
            FROM sad_userdetails
            WHERE Usr_CompID = @CompId
              AND Usr_Node > 0
              AND Usr_OrgnID > 0
              AND usr_DelFlag IN ('A', 'B', 'L')
            ORDER BY FullName";

                parameters = new { CompId = compId };
            }
            else
            {
                query = @"
            SELECT Usr_ID AS UserId, USr_FullName AS FullName
            FROM sad_userdetails
            WHERE usr_compID = @CompId
              AND Usr_Role IN (
                  SELECT Mas_ID
                  FROM SAD_GrpOrLvl_General_Master
                  WHERE Mas_Delflag = 'A'
                    AND Mas_CompID = @CompId
                    AND Mas_Description = @Role
              )
              AND usr_DelFlag IN ('A', 'B', 'L')
            ORDER BY USr_FullName";

                parameters = new { CompId = compId, Role = role };
            }

            var result = await connection.QueryAsync<UserDto>(query, parameters);
            return result.ToList();
        }


        public async Task<List<AuditTypeCustomerDto>> LoadAuditTypeComplianceDetailsAsync(AuditTypeRequestDto req)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            var sql = new StringBuilder();
            sql.Append(@"
        SELECT CMM_ID AS PKID, CMM_Desc AS Name 
        FROM Content_Management_Master 
        WHERE CMM_Category = @Type 
        AND CMM_CompID = @CompanyId 
        AND CMM_Delflag = 'A' 
        AND CMS_KeyComponent = 0 
        AND CMM_ID IN (
            SELECT LOE_ServiceTypeId 
            FROM SAD_CUST_LOE 
            WHERE LOE_CustomerId = @CustomerId 
            AND LOE_YearId = @FinancialYearId
        )
        AND EXISTS (
            SELECT 1 
            FROM AuditType_Checklist_Master 
            WHERE ACM_AuditTypeID = CMM_ID
        ");

            if (req.AuditTypeId > 0)
                sql.Append(" OR CMM_ID = @AuditTypeId ");

            sql.Append(") ORDER BY CMM_Desc ASC");

            var parameters = new
            {
                req.Type,
                req.CompanyId,
                req.FinancialYearId,
                req.CustomerId,
                req.AuditTypeId
            };

            var result = await connection.QueryAsync<AuditTypeCustomerDto>(sql.ToString(), parameters);
            return result.ToList();
        }


        public List<QuarterDto> GenerateQuarters(DateTime? fromDate)
        {
            var quarters = new List<QuarterDto>();

            if (fromDate.HasValue)
            {
                var yearlyToDate = fromDate.Value.AddYears(1).AddDays(-1);
                quarters.Add(new QuarterDto
                {
                    Id = 0,
                    Description = "K Yearly",
                    FromDate = fromDate.Value.ToString("dd/MM/yyyy"),
                    ToDate = yearlyToDate.ToString("dd/MM/yyyy")
                });

                var quarterFrom = fromDate.Value;

                for (int i = 1; i <= 4; i++)
                {
                    var quarterTo = quarterFrom.AddMonths(3).AddDays(-1);

                    quarters.Add(new QuarterDto
                    {
                        Id = i,
                        Description = $"Q{i}",
                        FromDate = quarterFrom.ToString("dd/MM/yyyy"),
                        ToDate = quarterTo.ToString("dd/MM/yyyy")
                    });

                    quarterFrom = quarterTo.AddDays(1);
                }
            }
            else
            {
                quarters.Add(new QuarterDto { Id = 0, Description = "K Yearly", FromDate = "", ToDate = "-" });
                for (int i = 1; i <= 4; i++)
                {
                    quarters.Add(new QuarterDto { Id = i, Description = $"Q{i}", FromDate = "-", ToDate = "-" });
                }
            }

            return quarters;
        }


        public async Task<List<AuditTypeHeadingDto>> LoadAllAuditTypeHeadingsAsync(int compId, int auditTypeId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            string query = @"
            SELECT DISTINCT(ACM_Heading),
                   DENSE_RANK() OVER (ORDER BY ACM_Heading DESC) AS ACM_ID
            FROM AuditType_Checklist_Master
            WHERE ACM_AuditTypeID = @AuditTypeID
              AND ACM_CompId = @CompID
              AND ACM_Heading <> ''
              AND ACM_Heading <> 'NULL'";

            var result = await connection.QueryAsync<AuditTypeHeadingDto>(query, new { AuditTypeID = auditTypeId, CompID = compId });
            return result.ToList();
        }
        public async Task<IEnumerable<UserDto>> GetUsersAsync(int companyId, string userIds)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));
            var sql = @"
    SELECT Usr_ID, (Usr_FullName + ' - ' + Usr_Code) AS FullName
    FROM sad_userdetails
    WHERE Usr_CompID = @CompanyId 
      AND (usr_DelFlag = 'A' OR usr_DelFlag = 'B' OR usr_DelFlag = 'L')";

            var parameters = new DynamicParameters();
            parameters.Add("CompanyId", companyId);

            if (!string.IsNullOrWhiteSpace(userIds))
            {
                var idList = userIds.Split(',')
                                    .Select(id => int.TryParse(id.Trim(), out var i) ? i : (int?)null)
                                    .Where(id => id.HasValue)
                                    .Select(id => id.Value)
                                    .ToList();

                if (idList.Any())
                {
                    sql += " AND Usr_ID IN @UserIds";
                    parameters.Add("UserIds", idList);
                }
            }

            sql += " ORDER BY FullName";

            var result = await connection.QueryAsync<UserDto>(sql, parameters);
            return result;
        }

        public async Task<List<AssignedCheckpointDto>> GetAssignedCheckpointsAndTeamMembersAsync(
       int compId, int auditId, int auditTypeId, int custId,
       string sType, string heading, string? sCheckPoints)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            string sql;
            object parameters;

            switch (sType.ToLower())
            {
                case "heading":
                    // Filter by heading only
                    sql = @"
                SELECT 
                    DENSE_RANK() OVER (ORDER BY ACM_ID) AS SlNo,
                    ACM_Heading,
                    ACM_ID,
                    ACM_Checkpoint
                FROM AuditType_Checklist_Master
                WHERE 
                    ACM_CompId = @CompId
                    AND ACM_DELFLG = 'A'
                    AND ACM_AuditTypeID = @AuditTypeId
                    And ACM_Heading = @Heading
                    ORDER BY ACM_Heading, ACM_ID";

                    parameters = new
                    {
                        CompId = compId,
                        AuditTypeId = auditTypeId,
                        AuditId = auditId,
                        Heading = heading
                    };
                    break;

                case "checkpoint":
                    // Filter by checkpoints only
                    sql = @"
                SELECT 
                    DENSE_RANK() OVER (ORDER BY ACM_ID) AS SlNo,
                    ACM_Heading,
                    ACM_ID,
                    ACM_Checkpoint
                FROM AuditType_Checklist_Master
                WHERE 
                    ACM_CompId = @CompId
                    AND ACM_DELFLG = 'A'
                    AND ACM_AuditTypeID = @AuditTypeId
                    AND ACM_ID IN (
                        SELECT value FROM STRING_SPLIT(@CheckPoints, ',')
                    )
                ORDER BY ACM_Heading, ACM_ID";

                    parameters = new
                    {
                        CompId = compId,
                        AuditTypeId = auditTypeId,
                        CheckPoints = sCheckPoints
                    };
                    break;

                case "assigned":
                default:
                    // Load assigned checkpoint details
                    sql = @"
                SELECT 
                    SACD_ID,
                    SACD_CheckpointId,
                    SACD_Heading,
                    ISNULL(usr_FullName, '') AS Employee,
                    SUM(LEN(SACD_CheckpointId) - LEN(REPLACE(SACD_CheckpointId, ',', '')) + 1) AS NoCheckpoints,
                    CASE WHEN SACD_EmpId > 0 THEN 1 ELSE 0 END AS NoEmployee,
                    SACD_TotalHr AS Working_Hours,
                    CASE 
                        WHEN CONVERT(VARCHAR(10), SACD_EndDate, 103) = '01/01/1900' THEN '' 
                        ELSE ISNULL(CONVERT(VARCHAR(10), SACD_EndDate, 103), '') 
                    END AS Timeline
                FROM StandardAudit_Checklist_Details
                LEFT JOIN sad_userdetails a ON SACD_EmpId = usr_Id
                WHERE 
                    SACD_AuditId = @AuditId 
                    AND SACD_CustID = @CustId
                    AND (@Heading = '' OR SACD_Heading = @Heading)
                GROUP BY 
                    SACD_ID, SACD_Heading, SACD_EndDate, SACD_TotalHr, SACD_CheckpointId, usr_FullName, SACD_EmpId";

                    parameters = new
                    {
                        AuditId = auditId,
                        CustId = custId,
                        Heading = heading ?? ""
                    };
                    break;
            }

            var result = await connection.QueryAsync<AssignedCheckpointDto>(sql, parameters);
            return result.ToList();
        }



        public async Task<bool> DeleteSelectedCheckpointsAndTeamMembersAsync(DeleteCheckpointDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            const string deleteChecklistDetailsSql = @"
        DELETE FROM StandardAudit_Checklist_Details
        WHERE SACD_AuditId = @AuditId
          AND SACD_CustID = @CustomerId
          AND SACD_ID = @PkId
          AND SACD_CompID = @CompId";

            const string deleteScheduleCheckpointSql = @"
        DELETE FROM StandardAudit_ScheduleCheckPointList
        WHERE SAC_SA_ID = @AuditId
          AND SAC_CheckPointID IN @CheckpointIds
          AND SAC_CompID = @CompId";

            try
            {
                await connection.OpenAsync();
                using var transaction = connection.BeginTransaction();

                var deleteChecklist = await connection.ExecuteAsync(deleteChecklistDetailsSql, new
                {
                    AuditId = dto.AuditId,
                    CustomerId = dto.CustomerId,
                    PkId = dto.PkId,
                    CompId = dto.CompId
                }, transaction);

                var deleteSchedule = await connection.ExecuteAsync(deleteScheduleCheckpointSql, new
                {
                    AuditId = dto.AuditId,
                    CheckpointIds = dto.CheckpointIds.Split(',').Select(int.Parse).ToArray(),
                    CompId = dto.CompId
                }, transaction);

                // Optional: also delete from trace_cust DB if needed with a second connection string

                await transaction.CommitAsync();

                return deleteChecklist > 0 || deleteSchedule > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<AuditChecklistDto>> GetChecklistAsync(int auditId, int companyId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            var sql = @"
        SELECT ACM_Heading, ACM_ID, ACM_Checkpoint,
               CASE WHEN SAC_Mandatory = 1 THEN 'Yes' ELSE 'No' END AS SAC_Mandatory
        FROM AuditType_Checklist_Master
        JOIN StandardAudit_ScheduleCheckPointList ON ACM_ID = SAC_CheckPointID
        WHERE SAC_SA_ID = @AuditId AND SAC_CompID = @CompanyId AND ACM_CompId = @CompanyId";

            var parameters = new { AuditId = auditId, CompanyId = companyId };
            var result = await connection.QueryAsync<AuditChecklistDto>(sql, parameters);
            return result;
        }
        private async Task<string> GetCustomerCode(string connectionString, int companyId, int customerId)
        {
            var sql = @"SELECT CUST_NAME FROM sad_Customer_master 
                WHERE Cust_ID = @CustomerId AND Cust_CompID = @CompanyId";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var result = await connection.ExecuteScalarAsync<string>(sql, new
                    {
                        CustomerId = customerId,
                        CompanyId = companyId
                    });

                    // Remove whitespace, trim, and return first 3 uppercase characters
                    var cleaned = Regex.Replace(result ?? string.Empty, @"\s", "").Trim();
                    return cleaned.Length >= 3 ? cleaned.Substring(0, 3).ToUpper() : cleaned.ToUpper();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve customer code", ex);
            }
        }


        public async Task<int> InsertStandardAuditScheduleWithQuartersAsync(
         string sAC, StandardAuditScheduleDto dto, int custRegAccessCodeId, int iUserID, string sIPAddress,
         string sModule, string sForm, string sEvent, int iMasterID, string sMasterName, int iSubMasterID, string sSubMasterName)
        {
            int insertedId = 0;
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);
            var builder = new SqlConnectionStringBuilder(connectionString);
            sAC = builder.InitialCatalog;

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                if (dto.SA_ID == 0)
                {
                    // Pass transaction here!
                    int maxId = await connection.ExecuteScalarAsync<int>(
                        $"SELECT COUNT(*) + 1 FROM StandardAudit_Schedule WHERE SA_YearID = @YearID",
                        new { YearID = dto.SA_YearID },
                        transaction // assign transaction here
                    );

                    string formattedId = maxId.ToString(); // e.g., 00001

                    // If GetCustomerCode accesses DB, modify it to accept a transaction or connection
                    // Otherwise, it can be called without transaction if it's safe
                    string customerPrefix = await GetCustomerCode(connectionString, dto.SA_CompID, dto.SA_CustID);

                    string jobCode = $"{customerPrefix}{dto.SA_YearID}-{formattedId}";

                    dto.SA_AuditNo = jobCode;
                }
                else
                {
                    var auditNo = await connection.ExecuteScalarAsync<string>(
                        "SELECT SA_AuditNo FROM StandardAudit_Schedule WHERE SA_ID = @SA_ID",
                        new { dto.SA_ID },
                        transaction // assign transaction here
                    );

                    dto.SA_AuditNo = auditNo;
                }

                // Prepare and execute your stored procedure with transaction
                using var command = new SqlCommand("spStandardAudit_Schedule", connection, transaction)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@SA_ID", dto.SA_ID != null ? (object)dto.SA_ID : DBNull.Value);
                command.Parameters.AddWithValue("@SA_AuditNo", dto.SA_AuditNo ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@SA_CustID", dto.SA_CustID != null ? (object)dto.SA_CustID : DBNull.Value);
                command.Parameters.AddWithValue("@SA_YearID", dto.SA_YearID != null ? (object)dto.SA_YearID : DBNull.Value);
                command.Parameters.AddWithValue("@SA_AuditTypeID", dto.SA_AuditTypeID != null ? (object)dto.SA_AuditTypeID : DBNull.Value);
                command.Parameters.AddWithValue("@SA_EngagementPartnerID", dto.SA_EngagementPartnerID != null ? (object)dto.SA_EngagementPartnerID : DBNull.Value);
                command.Parameters.AddWithValue("@SA_ReviewPartnerID", dto.SA_ReviewPartnerID != null ? (object)dto.SA_ReviewPartnerID : DBNull.Value);
                command.Parameters.AddWithValue("@SA_PartnerID", dto.SA_PartnerID != null ? (object)dto.SA_PartnerID : DBNull.Value);
                command.Parameters.AddWithValue("@SA_AdditionalSupportEmployeeID", dto.SA_AdditionalSupportEmployeeID != null ? (object)dto.SA_AdditionalSupportEmployeeID : DBNull.Value);
                command.Parameters.AddWithValue("@SA_ScopeOfAudit", dto.SA_ScopeOfAudit ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@SA_Status", dto.SA_Status != null ? (object)dto.SA_Status : DBNull.Value);
                command.Parameters.AddWithValue("@SA_AttachID", dto.SA_AttachID != null ? (object)dto.SA_AttachID : DBNull.Value);
                command.Parameters.AddWithValue("@SA_StartDate", dto.SA_StartDate != null ? (object)dto.SA_StartDate : DBNull.Value);
                command.Parameters.AddWithValue("@SA_ExpCompDate", dto.SA_ExpCompDate != null ? (object)dto.SA_ExpCompDate : DBNull.Value);

                // Newly added parameters
                command.Parameters.AddWithValue("@SA_RptRvDate", dto.SA_RptRvDate != null ? (object)dto.SA_RptRvDate : DBNull.Value);
                command.Parameters.AddWithValue("@SA_RptFilDate", dto.SA_RptFilDate != null ? (object)dto.SA_RptFilDate : DBNull.Value);
                command.Parameters.AddWithValue("@SA_MRSDate", dto.SA_MRSDate != null ? (object)dto.SA_MRSDate : DBNull.Value);

                command.Parameters.AddWithValue("@SA_AuditOpinionDate", dto.SA_AuditOpinionDate != null ? (object)dto.SA_AuditOpinionDate : DBNull.Value);
                command.Parameters.AddWithValue("@SA_FilingDateSEC", dto.SA_FilingDateSEC != null ? (object)dto.SA_FilingDateSEC : DBNull.Value);
                command.Parameters.AddWithValue("@SA_MRLDate", dto.SA_MRLDate != null ? (object)dto.SA_MRLDate : DBNull.Value);
                command.Parameters.AddWithValue("@SA_FilingDatePCAOB", dto.SA_FilingDatePCAOB != null ? (object)dto.SA_FilingDatePCAOB : DBNull.Value);
                command.Parameters.AddWithValue("@SA_BinderCompletedDate", dto.SA_BinderCompletedDate != null ? (object)dto.SA_BinderCompletedDate : DBNull.Value);
                command.Parameters.AddWithValue("@SA_IntervalId", dto.SA_IntervalId != null ? (object)dto.SA_IntervalId : DBNull.Value);
                command.Parameters.AddWithValue("@SA_CrBy", dto.SA_CrBy != null ? (object)dto.SA_CrBy : DBNull.Value);
                command.Parameters.AddWithValue("@SA_UpdatedBy", dto.SA_UpdatedBy != null ? (object)dto.SA_UpdatedBy : DBNull.Value);
                command.Parameters.AddWithValue("@SA_IPAddress", dto.SA_IPAddress ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@SA_CompID", dto.SA_CompID != null ? (object)dto.SA_CompID : DBNull.Value);

                // Output Parameters
                command.Parameters.Add("@Out_Message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                command.Parameters.Add("@Out_AuditScheduleID", SqlDbType.Int).Direction = ParameterDirection.Output;


                var paramUpdateOrSave = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var paramOper = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(paramUpdateOrSave);
                command.Parameters.Add(paramOper);

                await command.ExecuteNonQueryAsync();
                insertedId = (int)paramOper.Value;

                // Audit Logging
                using var logCommand = new SqlCommand("spAudit_Log_Form_Operations", connection, transaction)
                {
                    CommandType = CommandType.StoredProcedure
                };

                logCommand.Parameters.AddWithValue("@ALFO_UserID", dto.SA_CrBy);
                logCommand.Parameters.AddWithValue("@ALFO_Module", sModule);
                logCommand.Parameters.AddWithValue("@ALFO_Form", sForm);
                logCommand.Parameters.AddWithValue("@ALFO_Event", sEvent);
                logCommand.Parameters.AddWithValue("@ALFO_MasterID", insertedId);
                logCommand.Parameters.AddWithValue("@ALFO_MasterName", sMasterName);
                logCommand.Parameters.AddWithValue("@ALFO_SubMasterID", iSubMasterID);
                logCommand.Parameters.AddWithValue("@ALFO_SubMasterName", sSubMasterName);
                logCommand.Parameters.AddWithValue("@ALFO_IPAddress", sIPAddress);
                logCommand.Parameters.AddWithValue("@ALFO_CompID", dto.SA_CompID);

                await logCommand.ExecuteNonQueryAsync();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            // Process quarters after transaction commit
            foreach (var quarter in dto.Quarters ?? Enumerable.Empty<QuarterAuditDto>())
            {
                await UpdateStandardAuditStartEndDate(sAC, dto.SA_CompID, insertedId, quarter.StartDate, quarter.EndDate, custRegAccessCodeId);
                await SaveUpdateAuditScheduleIntervalForCust(sAC, dto.SA_CompID, insertedId, quarter.IntervalID, quarter.SubIntervalID, quarter.StartDate, quarter.EndDate, iUserID, sIPAddress);
            }

            return insertedId;
        }



        private async Task UpdateStandardAuditStartEndDate(string sAC, int iAcID, int iAuditID, DateTime dStartDate, DateTime dExpCompDate, int iCustRegAccessCodeId)
        {
            try
            {
                // Update the first record
                string sSql = "UPDATE StandardAudit_Schedule SET SA_StartDate = @StartDate, SA_ExpCompDate = @ExpCompDate WHERE SA_ID = @AuditID AND SA_CompID = @CompID";
                var parameters = new
                {
                    StartDate = dStartDate,
                    ExpCompDate = dExpCompDate,
                    AuditID = iAuditID,
                    CompID = iAcID
                };

                await ExecuteCommandAsync(sAC, sSql, parameters);

            }
            catch (Exception ex)
            {
                // Log the error for debugging
                throw new Exception("Error updating StandardAuditStartEndDate", ex);
            }
        }


        private async Task<int> SaveUpdateAuditScheduleIntervalForCust(string sAC, int iACID, int iAuditId, int iIntervalID, int iIntervalSubID, DateTime dStartDate, DateTime dEndDate, int iUserID, string sIPAddress)
        {
            try
            {
                // First, check if the interval already exists
                string sSql = "SELECT SAI_ID FROM StandardAudit_ScheduleIntervals WHERE SAI_SA_ID = @AuditId AND SAI_IntervalID = @IntervalID AND SAI_IntervalSubID = @IntervalSubID AND SAI_CompID = @CompID";
                var parameters = new
                {
                    AuditId = iAuditId,
                    IntervalID = iIntervalID,
                    IntervalSubID = iIntervalSubID,
                    CompID = iACID
                };
                var iSAI_ID = await ExecuteScalarAsync<int>(sAC, sSql, parameters);

                if (iSAI_ID > 0)
                {
                    // If the record exists, update it
                    string sUpdateSql = "UPDATE StandardAudit_ScheduleIntervals SET SAI_StartDate = @StartDate, SAI_EndDate = @EndDate WHERE SAI_ID = @SAI_ID";
                    var updateParameters = new
                    {
                        StartDate = dStartDate,
                        EndDate = dEndDate,
                        SAI_ID = iSAI_ID
                    };

                    await ExecuteCommandAsync(sAC, sUpdateSql, updateParameters);

                    // Return the updated ID
                    return iSAI_ID;
                }
                else
                {
                    // If no existing record, insert a new one
                    string sqlQuery = "SELECT ISNULL(MAX(SAI_ID), 0) + 1 FROM StandardAudit_ScheduleIntervals";
                    int iMaxId = await ExecuteScalarAsync<int>(sAC, sqlQuery, parameters);
                    string sInsertSql = @"
                INSERT INTO StandardAudit_ScheduleIntervals (SAI_ID, SAI_SA_ID, SAI_IntervalID, SAI_IntervalSubID, SAI_StartDate, SAI_EndDate, SAI_CrBy, SAI_CrOn, SAI_IPAddress, SAI_CompID) 
                VALUES (@MaxID, @AuditId, @IntervalID, @IntervalSubID, @StartDate, @EndDate, @UserID, GETDATE(), @IPAddress, @CompID)";

                    var insertParameters = new
                    {
                        MaxID = iMaxId,
                        AuditId = iAuditId,
                        IntervalID = iIntervalID,
                        IntervalSubID = iIntervalSubID,
                        StartDate = dStartDate,
                        EndDate = dEndDate,
                        UserID = iUserID,
                        IPAddress = sIPAddress,
                        CompID = iACID
                    };

                    await ExecuteCommandAsync(sAC, sInsertSql, insertParameters);

                    // Return the inserted ID (iMaxId)
                    return iMaxId;
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Error in SaveUpdateAuditScheduleIntervalForCust: {ex.Message}");
                throw new Exception("Error saving or updating audit schedule interval", ex);
            }
        }



        // Helper method to execute non-query SQL command
        // Helper method to execute non-query SQL command
        private async Task ExecuteCommandAsync(string sAC, string sSql, object parameters)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using (var connection = new SqlConnection(_configuration.GetConnectionString(dbName)))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(sSql, parameters);
            }
        }

        // Helper method to execute scalar SQL command
        private async Task<T> ExecuteScalarAsync<T>(string sAC, string sSql, object parameters)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using (var connection = new SqlConnection(_configuration.GetConnectionString(dbName)))
            {
                await connection.OpenAsync();
                return await connection.ExecuteScalarAsync<T>(sSql, parameters);
            }
        }

        public async Task<string[]> SaveUpdateStandardAuditChecklistDetailsAsync(StandardAuditChecklistDetailsDto objSACLD)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            var connectionString = _configuration.GetConnectionString(dbName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Save or Update Checklist Details
            using var cmdChecklist = new SqlCommand("spStandardAudit_Checklist_Details", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmdChecklist.Parameters.AddWithValue("@SACD_ID", objSACLD.SACD_ID);
            cmdChecklist.Parameters.AddWithValue("@SACD_CustId", objSACLD.SACD_CustId);
            cmdChecklist.Parameters.AddWithValue("@SACD_AuditId", objSACLD.SACD_AuditId);
            cmdChecklist.Parameters.AddWithValue("@SACD_AuditType", objSACLD.SACD_AuditType);
            cmdChecklist.Parameters.AddWithValue("@SACD_Heading", objSACLD.SACD_Heading ?? (object)DBNull.Value);
            cmdChecklist.Parameters.AddWithValue("@SACD_CheckpointId", objSACLD.SACD_CheckpointId ?? (object)DBNull.Value);
            cmdChecklist.Parameters.AddWithValue("@SACD_EmpId", objSACLD.SACD_EmpId);
            cmdChecklist.Parameters.AddWithValue("@SACD_WorkType", objSACLD.SACD_WorkType);
            cmdChecklist.Parameters.AddWithValue("@SACD_HrPrDay", objSACLD.SACD_HrPrDay ?? (object)DBNull.Value);
            cmdChecklist.Parameters.AddWithValue("@SACD_StartDate", objSACLD.SACD_StartDate);
            cmdChecklist.Parameters.AddWithValue("@SACD_EndDate", objSACLD.SACD_EndDate);
            cmdChecklist.Parameters.AddWithValue("@SACD_TotalHr", objSACLD.SACD_TotalHr ?? (object)DBNull.Value);
            cmdChecklist.Parameters.AddWithValue("@SACD_CrBy", objSACLD.SACD_CRBY);
            cmdChecklist.Parameters.AddWithValue("@SACD_UpdatedBy", objSACLD.SACD_UPDATEDBY);
            cmdChecklist.Parameters.AddWithValue("@SACD_IPAddress", objSACLD.SACD_IPAddress ?? (object)DBNull.Value);
            cmdChecklist.Parameters.AddWithValue("@SACD_CompID", objSACLD.SACD_CompId);

            var paramUpdateOrSave = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var paramOper = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmdChecklist.Parameters.Add(paramUpdateOrSave);
            cmdChecklist.Parameters.Add(paramOper);

            await cmdChecklist.ExecuteNonQueryAsync();

            // Delete Checkpoints if AuditStatus = 1
            if (objSACLD.AuditStatusID == 1 && objSACLD.SACD_ID > 0 && !string.IsNullOrEmpty(objSACLD.SelectedCheckPointsPKID))
            {
                // Split and validate checkpoint IDs
                var ids = objSACLD.SelectedCheckPointsPKID
                           .Split(',')
                           .Where(x => int.TryParse(x, out _))
                           .ToList();

                if (ids.Count > 0)
                {
                    var idParams = ids.Select((id, index) => $"@cp{index}").ToList();
                    var sSql = $@"
            DELETE FROM StandardAudit_ScheduleCheckPointList
            WHERE SAC_SA_ID = @AuditId
            AND SAC_CompID = @CustRegAccessCodeId
            AND SAC_CheckPointID IN ({string.Join(",", idParams)})";

                    using var cmdDelete = new SqlCommand(sSql, connection)
                    {
                        CommandType = CommandType.Text
                    };

                    // Add base parameters
                    cmdDelete.Parameters.AddWithValue("@AuditId", objSACLD.SACD_AuditId);
                    cmdDelete.Parameters.AddWithValue("@CustRegAccessCodeId", objSACLD.SACD_CompId);

                    // Add dynamic checkpoint parameters
                    for (int i = 0; i < ids.Count; i++)
                    {
                        cmdDelete.Parameters.AddWithValue($"@cp{i}", int.Parse(ids[i]));
                    }

                    await cmdDelete.ExecuteNonQueryAsync();
                }
            }


            // Save Checkpoints
            if (objSACLD.Checkpoints != null && objSACLD.Checkpoints.Count > 0)
            {
                foreach (var chk in objSACLD.Checkpoints)
                {
                    using var cmdChk = new SqlCommand("spStandardAudit_ScheduleCheckPointList", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Existing parameters for the stored procedure
                    cmdChk.Parameters.AddWithValue("@SAC_ID", chk.SAC_ID);
                    cmdChk.Parameters.AddWithValue("@SAC_SA_ID", chk.SAC_SA_ID);
                    cmdChk.Parameters.AddWithValue("@SAC_CheckPointID", chk.SAC_CheckPointID);
                    cmdChk.Parameters.AddWithValue("@SAC_Mandatory", chk.SAC_Mandatory);
                    cmdChk.Parameters.AddWithValue("@SAC_Status", chk.SAC_Status);
                    cmdChk.Parameters.AddWithValue("@SAC_AttachID", chk.SAC_AttachID);
                    cmdChk.Parameters.AddWithValue("@SAC_CrBy", chk.SAC_CrBy);
                    cmdChk.Parameters.AddWithValue("@SAC_IPAddress", chk.SAC_IPAddress ?? (object)DBNull.Value);
                    cmdChk.Parameters.AddWithValue("@SAC_CompID", chk.SAC_CompID);

                    // Missing parameters
                    cmdChk.Parameters.AddWithValue("@iUpdateOrSave", chk.SAC_ID > 0 ? 1 : 0); // 1 = Update, 0 = Save
                    cmdChk.Parameters.AddWithValue("@iOper", DBNull.Value); // Assuming this is an output parameter, you can adjust it as needed

                    // Execute the stored procedure
                    await cmdChk.ExecuteNonQueryAsync();
                }
            }


            return new string[] { paramUpdateOrSave.Value.ToString(), paramOper.Value.ToString() };
        }
        public async Task<int[]> SaveCustomerMasterAsync(int iCompId, AuditCustomerDetailsDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            using (var connection = new SqlConnection(_configuration.GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("spSAD_CUSTOMER_MASTER", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add input parameters
                    command.Parameters.AddWithValue("@CUST_ID", dto.CUST_ID);
                    command.Parameters.AddWithValue("@CUST_NAME", dto.CUST_NAME ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_CODE", dto.CUST_CODE ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_WEBSITE", dto.CUST_WEBSITE ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_EMAIL", dto.CUST_EMAIL ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_GROUPNAME", dto.CUST_GROUPNAME ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_GROUPINDIVIDUAL", dto.CUST_GROUPINDIVIDUAL);
                    command.Parameters.AddWithValue("@CUST_ORGTYPEID", dto.CUST_ORGTYPEID);
                    command.Parameters.AddWithValue("@CUST_INDTYPEID", dto.CUST_INDTYPEID);
                    command.Parameters.AddWithValue("@CUST_MGMTTYPEID", dto.CUST_MGMTTYPEID);
                    command.Parameters.AddWithValue("@CUST_CommitmentDate", dto.CUST_CommitmentDate);
                    command.Parameters.AddWithValue("@CUSt_BranchId", dto.CUSt_BranchId ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_COMM_ADDRESS", dto.CUST_COMM_ADDRESS ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_COMM_CITY", dto.CUST_COMM_CITY ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_COMM_PIN", dto.CUST_COMM_PIN ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_COMM_STATE", dto.CUST_COMM_STATE ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_COMM_COUNTRY", dto.CUST_COMM_COUNTRY ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_COMM_FAX", dto.CUST_COMM_FAX ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_COMM_TEL", dto.CUST_COMM_TEL ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_COMM_Email", dto.CUST_COMM_Email ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_ADDRESS", dto.CUST_ADDRESS ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_CITY", dto.CUST_CITY ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_PIN", dto.CUST_PIN ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_STATE", dto.CUST_STATE ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_COUNTRY", dto.CUST_COUNTRY ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_FAX", dto.CUST_FAX ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_TELPHONE", dto.CUST_TELPHONE ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_ConEmailID", dto.CUST_ConEmailID ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_LOCATIONID", dto.CUST_LOCATIONID ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_TASKS", dto.CUST_TASKS ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_ORGID", dto.CUST_ORGID);
                    command.Parameters.AddWithValue("@CUST_CRBY", dto.CUST_CRBY);
                    command.Parameters.AddWithValue("@CUST_UpdatedBy", dto.CUST_UpdatedBy);
                    command.Parameters.AddWithValue("@CUST_BOARDOFDIRECTORS", dto.CUST_BOARDOFDIRECTORS ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_DEPMETHOD", dto.CUST_DEPMETHOD);
                    command.Parameters.AddWithValue("@CUST_IPAddress", dto.CUST_IPAddress ?? string.Empty);
                    command.Parameters.AddWithValue("@CUST_CompID", dto.CUST_CompID);
                    command.Parameters.AddWithValue("@CUST_Amount_Type", dto.CUST_Amount_Type);
                    command.Parameters.AddWithValue("@CUST_RoundOff", dto.CUST_RoundOff);
                    command.Parameters.AddWithValue("@Cust_FY", dto.Cust_FY);
                    command.Parameters.AddWithValue("@Cust_DurtnId", dto.Cust_DurtnId);

                    // Output parameters
                    var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var operParam = new SqlParameter("@iOper", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    command.Parameters.Add(updateOrSaveParam);
                    command.Parameters.Add(operParam);

                    try
                    {
                        // Execute stored procedure first
                        await command.ExecuteNonQueryAsync();

                        // Now you can safely read output values
                        int updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                        int oper = (int)(operParam.Value ?? 0);

                        // Only after getting oper value, run your update query
                        var query = @"UPDATE SAD_CUSTOMER_MASTER 
                              SET CUST_DelFlg = 'A'
                              WHERE CUST_CompID = @CompId AND CUST_ID = @Cust_Id";

                        await connection.ExecuteAsync(query, new { CompId = iCompId, Cust_Id = oper });

                        return new int[] { updateOrSave, oper };
                    }
                    catch (Exception ex)
                    {
                        // Log or rethrow as needed
                        throw;
                    }
                }
            }
        }

        public async Task<IEnumerable<AuditChecklistDto>> LoadAuditTypeCheckListAsync(
       int compId,
       int auditId,
       int auditTypeId,
       string heading)
        {
            var sql = @"
            SELECT 
                DENSE_RANK() OVER (ORDER BY ACM_ID) AS SlNo,
                ACM_Heading,
                ACM_ID,
                ACM_Checkpoint
            FROM AuditType_Checklist_Master
            WHERE 
                ACM_CompId = @CompId
                AND ACM_DELFLG = 'A'
                AND ACM_AuditTypeID = @AuditTypeID
                AND ACM_Heading = @Heading
                AND ACM_ID NOT IN (
                    SELECT SAC_CheckPointID 
                    FROM StandardAudit_ScheduleCheckPointList 
                    WHERE SAC_SA_ID = @AuditID 
                      AND SAC_CompID = @CompId
                )
            ORDER BY ACM_Heading, ACM_ID";

            var parameters = new
            {
                CompId = compId,
                AuditTypeID = auditTypeId,
                Heading = heading,
                AuditID = auditId
            };
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            var connection = new SqlConnection(_configuration.GetConnectionString(dbName));
            return await connection.QueryAsync<AuditChecklistDto>(sql, parameters);

        }
        public async Task<List<AssignedCheckpointDto>> GetAssignedCheckpointsAsync(
      int auditId, int custId, string heading)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            // Normalize heading input
            if (heading == "Select Heading")
            {
                heading = "";
            }

            // Start building the base SQL
            var sql = @"
    SELECT 
        SACD_ID,
        SACD_CheckpointId,
        SACD_Heading,
        ISNULL(usr_FullName, '') AS Employee,
        SUM(LEN(SACD_CheckpointId) - LEN(REPLACE(SACD_CheckpointId, ',', '')) + 1) AS NoCheckpoints,
        CASE WHEN SACD_EmpId > 0 THEN 1 ELSE 0 END AS NoEmployee,
        SACD_TotalHr AS Working_Hours,
        CASE 
            WHEN CONVERT(VARCHAR(10), SACD_EndDate, 103) = '01/01/1900' THEN '' 
            ELSE ISNULL(CONVERT(VARCHAR(10), SACD_EndDate, 103), '') 
        END AS Timeline
    FROM StandardAudit_Checklist_Details
    LEFT JOIN sad_userdetails a ON SACD_EmpId = usr_Id
    WHERE 
        SACD_AuditId = @AuditId 
        AND SACD_CustID = @CustId";



            sql += @"
    GROUP BY 
        SACD_ID, SACD_Heading, SACD_EndDate, SACD_TotalHr, SACD_CheckpointId, usr_FullName, SACD_EmpId";

            // Prepare parameters
            var parameters = new
            {
                AuditId = auditId,
                CustId = custId,
                Heading = heading
            };

            var result = await connection.QueryAsync<AssignedCheckpointDto>(sql, parameters);
            return result.ToList();
        }


        public async Task<int> SaveOrUpdateFullAuditSchedule(StandardAuditScheduleDTO dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using (var connection = new SqlConnection(_configuration.GetConnectionString(dbName)))
            {
                string AuditNo = dto.AuditNo;
                int SA_AuditNo = int.Parse(AuditNo);

                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var auditNo = await connection.ExecuteScalarAsync<string>(
                       "SELECT SA_AuditNo FROM StandardAudit_Schedule WHERE SA_ID = @SA_AuditNo",
                       new { SA_AuditNo },
                       transaction // assign transaction here
                   );

                        dto.AuditNo = auditNo;
                        // 1. Save/Update Audit Schedule via Stored Procedure
                        var parameters = new DynamicParameters();
                        parameters.Add("@SA_ID", dto.SA_ID);
                        auditNo = dto.AuditNo;
                        if (auditNo.EndsWith("KY"))
                        {
                            auditNo = auditNo.Substring(0, auditNo.Length - 2); // Remove "KY"
                        }

                        parameters.Add("@SA_AuditNo", auditNo + "Q" + dto.IntervalId);

                        parameters.Add("@SA_CustID", dto.CustID);
                        parameters.Add("@SA_YearID", dto.YearID);
                        parameters.Add("@SA_AuditTypeID", dto.AuditTypeID);
                        parameters.Add("@SA_EngagementPartnerID", dto.EngagementPartnerIDs);
                        parameters.Add("@SA_ReviewPartnerID", dto.ReviewPartnerIDs);
                        parameters.Add("@SA_PartnerID", dto.PartnerIDs);
                        parameters.Add("@SA_AdditionalSupportEmployeeID", dto.SupportEmployeeIDs);
                        parameters.Add("@SA_ScopeOfAudit", dto.ScopeOfAudit);
                        parameters.Add("@SA_Status", dto.Status);
                        parameters.Add("@SA_AttachID", dto.AttachID);
                        parameters.Add("@SA_StartDate", dto.FromDate);
                        parameters.Add("@SA_ExpCompDate", dto.ToDate);
                        parameters.Add("@SA_RptRvDate", dto.ReportReviewDate);         // MISSING
                        parameters.Add("@SA_RptFilDate", dto.ReportFilingDate);         // MISSING
                        parameters.Add("@SA_MRSDate", dto.DateForMRS);                  // MISSING
                        parameters.Add("@SA_AuditOpinionDate", dto.ReportReviewDate);
                        parameters.Add("@SA_FilingDateSEC", dto.ReportFilingDate);
                        parameters.Add("@SA_MRLDate", dto.DateForMRS);
                        parameters.Add("@SA_FilingDatePCAOB", dto.ReportFilingDatePCAOB);
                        parameters.Add("@SA_BinderCompletedDate", dto.BinderCompletedDate);
                        parameters.Add("@SA_IntervalId", dto.IntervalId);
                        parameters.Add("@SA_CrBy", dto.UserID);
                        parameters.Add("@SA_UpdatedBy", dto.UserID);
                        parameters.Add("@SA_IPAddress", dto.IPAddress);
                        parameters.Add("@SA_CompID", dto.CompID);

                        // OUTPUT parameters
                        parameters.Add("@iUpdateOrSave", 2, direction: ParameterDirection.InputOutput);
                        parameters.Add("@iOper", 1, direction: ParameterDirection.InputOutput);
                        parameters.Add("@Out_Message", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
                        parameters.Add("@Out_AuditScheduleID", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        await connection.ExecuteAsync("spStandardAudit_Schedule", parameters, transaction, commandType: CommandType.StoredProcedure);
                        int newAuditScheduleId = dto.SAI_SA_ID; // Gets 1 from "AUD2025-001"


                        // 2. Update SA_StartDate and SA_ExpCompDate
                        string updateScheduleSql = "UPDATE StandardAudit_Schedule SET SA_StartDate = @StartDate, SA_ExpCompDate = @ExpCompDate WHERE SA_ID = @AuditID AND SA_CompID = @CompID";
                        await connection.ExecuteAsync(updateScheduleSql, new
                        {
                            StartDate = dto.SACD_StartDate,
                            ExpCompDate = dto.SACD_EndDate,
                            AuditID = newAuditScheduleId,
                            CompID = dto.CompID
                        }, transaction);

                        // 3. Insert/Update Schedule Intervals
                        string intervalCheckSql = "SELECT SAI_ID FROM StandardAudit_ScheduleIntervals WHERE SAI_SA_ID = @AuditId AND SAI_IntervalID = @IntervalID AND SAI_IntervalSubID = @IntervalSubID AND SAI_CompID = @CompID";
                        int existingIntervalId = await connection.ExecuteScalarAsync<int?>(intervalCheckSql, new
                        {
                            AuditId = newAuditScheduleId,
                            IntervalID = dto.IntervalId,
                            IntervalSubID = dto.SAI_IntervalSubID,
                            CompID = dto.CompID
                        }, transaction) ?? 0;

                        if (existingIntervalId > 0)
                        {
                            string updateIntervalSql = "UPDATE StandardAudit_ScheduleIntervals SET SAI_StartDate = @StartDate, SAI_EndDate = @EndDate WHERE SAI_ID = @SAI_ID AND SAI_SA_ID = @AuditId AND SAI_IntervalID = @IntervalID AND SAI_IntervalSubID = @IntervalSubID AND SAI_CompID = @CompID";
                            await connection.ExecuteAsync(updateIntervalSql, new
                            {
                                StartDate = dto.SAI_StartDate,
                                EndDate = dto.SAI_EndDate,
                                SAI_ID = existingIntervalId,
                                AuditId = newAuditScheduleId,
                                IntervalID = dto.IntervalId,
                                IntervalSubID = dto.SAI_IntervalSubID,
                                CompID = dto.CompID
                            }, transaction);
                        }
                        else
                        {
                            int maxIntervalId = await connection.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(SAI_ID) + 1, 1) FROM StandardAudit_ScheduleIntervals", transaction: transaction);
                            string insertIntervalSql = "INSERT INTO StandardAudit_ScheduleIntervals (SAI_ID, SAI_SA_ID, SAI_IntervalID, SAI_IntervalSubID, SAI_StartDate, SAI_EndDate, SAI_CrBy, SAI_CrOn, SAI_IPAddress, SAI_CompID) VALUES (@ID, @AuditId, @IntervalID, @IntervalSubID, @StartDate, @EndDate, @CRBY, GETDATE(), @IPAddress, @CompID)";
                            await connection.ExecuteAsync(insertIntervalSql, new
                            {
                                ID = maxIntervalId,
                                AuditId = newAuditScheduleId,
                                IntervalID = dto.IntervalId,
                                IntervalSubID = dto.SAI_IntervalSubID,
                                StartDate = dto.SAI_StartDate,
                                EndDate = dto.SAI_EndDate,
                                CRBY = dto.UserID,
                                IPAddress = dto.IPAddress,
                                CompID = dto.CompID
                            }, transaction);
                        }

                        // 4. Checklist Sync
                        string checklistSql = "SELECT SACD_ID FROM StandardAudit_Checklist_Details WHERE SACD_CustId = @CustId AND SACD_AuditId = @AuditId AND SACD_CompId = @CompId";
                        var checklistIds = (await connection.QueryAsync<int>(checklistSql, new
                        {
                            CustId = dto.SACD_CustId,
                            AuditId = dto.SA_ID,
                            CompId = dto.CompID
                        }, transaction)).ToList();

                        foreach (var oldId in checklistIds)
                        {
                            int newId = await connection.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(SACD_ID) + 1, 1) FROM StandardAudit_Checklist_Details", transaction: transaction);

                            string insertChecklistSql = @"
                        INSERT INTO StandardAudit_Checklist_Details 
                        (SACD_ID, SACD_CustId, SACD_AuditId, SACD_AuditType, SACD_Heading, SACD_CheckpointId, SACD_EmpId, SACD_WorkType, SACD_HrPrDay, SACD_StartDate, SACD_EndDate, SACD_TotalHr, SACD_CRON, SACD_CRBY, SACD_IPAddress, SACD_CompId)
                        SELECT @NewID, @CustId, @NewAuditId, SACD_AuditType, SACD_Heading, SACD_CheckpointId, SACD_EmpId, SACD_WorkType, SACD_HrPrDay, SACD_StartDate, SACD_EndDate, SACD_TotalHr, GETDATE(), @CRBY, @IPAddress, SACD_CompId
                        FROM StandardAudit_Checklist_Details 
                        WHERE SACD_ID = @OldID AND SACD_CustId = @CustId AND SACD_AuditId = @AuditId AND SACD_CompId = @CompId";

                            await connection.ExecuteAsync(insertChecklistSql, new
                            {
                                NewID = newId,
                                CustId = dto.SACD_CustId,
                                NewAuditId = newAuditScheduleId,
                                OldID = oldId,
                                AuditId = dto.SA_ID,
                                CompId = dto.CompID,
                                CRBY = dto.UserID,
                                IPAddress = dto.IPAddress
                            }, transaction);
                        }

                        // 5. Checkpoint Sync
                        string checklistQuery = @"
    SELECT SACD_ID 
    FROM StandardAudit_Checklist_Details 
    WHERE SACD_CustId = @CustId 
      AND SACD_AuditId = @AuditScheduleId 
      AND SACD_CompId = @CompId";

                        var checklistRows = (await connection.QueryAsync<int>(checklistQuery, new
                        {
                            CustId = dto.CustID,
                            AuditScheduleId = newAuditScheduleId,
                            CompId = dto.CompID
                        }, transaction)).ToList();


                        foreach (var sacId in checklistRows)
                        {
                            int newSAC_ID = await connection.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(SAC_ID) + 1, 1) FROM StandardAudit_ScheduleCheckPointList", transaction: transaction);

                            string insertSql1 = @"
                        INSERT INTO StandardAudit_ScheduleCheckPointList 
                        (SAC_ID, SAC_SA_ID, SAC_CheckPointID, SAC_Mandatory, SAC_Status, SAC_AttachID, SAC_CrBy, SAC_CrOn, SAC_IPAddress, SAC_CompID)
                        SELECT @NewID, @NewAuditReviewId, SAC_CheckPointID, SAC_Mandatory, SAC_Status, 0, @CRBY, GETDATE(), @IPAddress, SAC_CompID
                        FROM StandardAudit_ScheduleCheckPointList 
                        WHERE SAC_ID = @OldID AND SAC_SA_ID = @AuditId AND SAC_CompID = @CompId";

                            await connection.ExecuteAsync(insertSql1, new
                            {
                                NewID = newSAC_ID,
                                NewAuditReviewId = newAuditScheduleId,
                                CRBY = dto.UserID,
                                IPAddress = dto.IPAddress,
                                OldID = sacId,
                                AuditId = dto.SA_ID,
                                CompId = dto.CompID
                            }, transaction);

                        }

                        transaction.Commit();
                        return newAuditScheduleId;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public async Task<CustomerDetailsDto> GetCustomerDetailsAsync(int iACID, int iCustId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            var result = new CustomerDetailsDto();

            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));
            await connection.OpenAsync();

            // 1. Financial Year
            var fyQuery = @"
            SELECT CASE 
                WHEN ISNULL(CUST_FY, '0') = 1 THEN 'Jan 1st to Dec 31st'
                WHEN ISNULL(CUST_FY, '0') = 2 THEN 'Feb 1st to Jan 31st'
                WHEN ISNULL(CUST_FY, '0') = 3 THEN 'Mar 1st to Feb 28th'
                WHEN ISNULL(CUST_FY, '0') = 4 THEN 'Apr 1st to May 31st'
                WHEN ISNULL(CUST_FY, '0') = 5 THEN 'May 1st to Apr 30th'
                WHEN ISNULL(CUST_FY, '0') = 6 THEN 'Jun 1st to May 31st'
                WHEN ISNULL(CUST_FY, '0') = 7 THEN 'Jul 1st to Jun 30th'
                WHEN ISNULL(CUST_FY, '0') = 8 THEN 'Aug 1st to Jul 31st'
                WHEN ISNULL(CUST_FY, '0') = 9 THEN 'Sep 1st to Aug 31st'
                WHEN ISNULL(CUST_FY, '0') = 10 THEN 'Oct 1st to Sep 30th'
                WHEN ISNULL(CUST_FY, '0') = 11 THEN 'Nov 1st to Oct 31st'
                WHEN ISNULL(CUST_FY, '0') = 12 THEN 'Dec 1st to Jan 31st'
                ELSE '-' END AS CUST_FY_Text
            FROM SAD_CUSTOMER_MASTER
            WHERE CUST_ID = @CustID AND CUST_CompID = @CompID";

            using var cmdFY = new SqlCommand(fyQuery, connection);
            cmdFY.Parameters.AddWithValue("@CustID", iCustId);
            cmdFY.Parameters.AddWithValue("@CompID", iACID);
            result.FinancialYear = (await cmdFY.ExecuteScalarAsync())?.ToString();

            // 2. CIK Registration No
            var cikQuery = "SELECT ISNULL(CUSt_BranchID, '') FROM SAD_CUSTOMER_MASTER WHERE CUST_ID = @CustID AND CUST_CompID = @CompID";
            using var cmdCIK = new SqlCommand(cikQuery, connection);
            cmdCIK.Parameters.AddWithValue("@CustID", iCustId);
            cmdCIK.Parameters.AddWithValue("@CompID", iACID);
            result.CIKRegistrationNo = (await cmdCIK.ExecuteScalarAsync())?.ToString();

            // 3. Address
            var addressQuery = "SELECT CUST_ADDRESS, CUST_CITY, CUST_STATE, CUST_PIN FROM SAD_CUSTOMER_MASTER WHERE CUST_ID = @CustID AND CUST_CompID = @CompID";
            using var cmdAddr = new SqlCommand(addressQuery, connection);
            cmdAddr.Parameters.AddWithValue("@CustID", iCustId);
            cmdAddr.Parameters.AddWithValue("@CompID", iACID);

            using var reader = await cmdAddr.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var addressParts = new List<string>();
                if (!reader.IsDBNull(0)) addressParts.Add(reader.GetString(0));
                if (!reader.IsDBNull(1)) addressParts.Add(reader.GetString(1));
                if (!reader.IsDBNull(2)) addressParts.Add(reader.GetString(2));
                if (!reader.IsDBNull(3)) addressParts.Add(reader.GetString(3));
                result.Address = string.Join(", ", addressParts);
            }

            return result;
        }
        public async Task<IEnumerable<GeneralMasterDto>> LoadGeneralMastersAsync(int iACID, string sType)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));
            var query = @"SELECT cmm_ID, cmm_Desc 
                      FROM Content_Management_Master 
                      WHERE CMM_CompID = @CompID 
                      AND cmm_Category = @Category 
                      AND cmm_Delflag = 'A' 
                      ORDER BY cmm_Desc ASC";

            return await connection.QueryAsync<GeneralMasterDto>(query, new
            {
                CompID = iACID,
                Category = sType
            });
        }
        public async Task<AuditStatusDto> GetAuditStatusAsync(int SA_Id, int companyId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));
            var query = "SELECT SA_Status FROM StandardAudit_Schedule WHERE SA_ID = @SA_ID AND SA_CompID = @SA_CompID";

            var result = await connection.QueryFirstOrDefaultAsync<AuditStatusDto>(query, new { SA_ID = SA_Id, SA_CompID = companyId });

            // If no record found, return a default object with SA_Status = 0
            return result ?? new AuditStatusDto { SA_Status = "0" };
        }
        public async Task<bool> IsCustomerLoeApprovedAsync(int customerId, int yearId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            var sql = @"SELECT 1 
                FROM SAD_CUST_LOE 
                WHERE LOE_CustomerId = @CustomerId 
                  AND LOE_YearId = @YearId 
                  AND LOE_STATUS = 'A'";

            var result = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { CustomerId = customerId, YearId = yearId });
            return result.HasValue;
        }
        public async Task<bool> CheckScheduleQuarterDetailsAsync(ScheduleQuarterCheckDto dto)
        {
            try
            {
                // ✅ Step 1: Get DB name from session
                string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

                if (string.IsNullOrEmpty(dbName))
                    throw new Exception("CustomerCode is missing in session. Please log in again.");
                using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));
                await connection.OpenAsync();

                // 1. Get the original AuditNo from DB using the input AuditNo as SA_ID (assuming AuditNo is SA_ID)
                var auditNoFromDb = await connection.ExecuteScalarAsync<string>(
                    "SELECT SA_AuditNo FROM StandardAudit_Schedule WHERE SA_Id = @AuditNo",
                    new { AuditNo = dto.AuditNo }
                );

                if (string.IsNullOrEmpty(auditNoFromDb))
                {
                    return false; // No such audit found
                }

                // 2. Remove "KY" if present
                var auditNoClean = auditNoFromDb.EndsWith("KY")
                    ? auditNoFromDb.Substring(0, auditNoFromDb.Length - 2)
                    : auditNoFromDb;

                // 3. Create job code
                var jobCode = auditNoClean + "Q" + dto.QuarterID;

                // 4. Query with job code and company ID
                string sql = @"
            SELECT SA_IntervalId 
            FROM StandardAudit_Schedule 
            WHERE SA_AuditNo = @JobCode AND SA_CompID = @CompID";

                var result = await connection.QuerySingleOrDefaultAsync<int?>(
                    sql,
                    new { JobCode = jobCode, CompID = dto.CompId }
                );

                return result.HasValue;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> GetLOESignedOnAsync(int compid, int auditTypeId, int customerId, int yearId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            var connectionString = _configuration.GetConnectionString(dbName);
            string sql = $@"
            SELECT LOET_ApprovedOn AS LOET_ApprovedOn
            FROM LOE_Template
            WHERE LOET_CustomerId = @CustomerId
              AND LOET_FunctionId = @AuditTypeId
              AND LOET_CompID = @AccessCodeId
              AND LOET_LOEID IN (
                  SELECT LOE_Id FROM SAD_CUST_LOE 
                  WHERE LOE_YearId = @YearId 
                    AND LOE_CustomerId = @CustomerId
              )";

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@CustomerId", customerId);
            command.Parameters.AddWithValue("@AuditTypeId", auditTypeId);
            command.Parameters.AddWithValue("@AccessCodeId", compid);
            command.Parameters.AddWithValue("@YearId", yearId);

            var result = await command.ExecuteScalarAsync();
            return result?.ToString() ?? "";
        }

        public async Task<string> GetLOEStatusAsync(int compid, int auditTypeId, int customerId, int yearId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            var connectionString = _configuration.GetConnectionString(dbName);
            string sql = @"
            SELECT CASE 
                       WHEN ISNULL(LOET_STATUS, 'N') = 'A' THEN 'A' 
                       ELSE 'N' 
                   END AS LOET_STATUS
            FROM LOE_Template
            WHERE LOET_CustomerId = @CustomerId
              AND LOET_FunctionId = @AuditTypeId
              AND LOET_CompID = @AccessCodeId
              AND LOET_LOEID IN (
                  SELECT LOE_Id FROM SAD_CUST_LOE 
                  WHERE LOE_YearId = @YearId 
                    AND LOE_CustomerId = @CustomerId
              )";

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@CustomerId", customerId);
            command.Parameters.AddWithValue("@AuditTypeId", auditTypeId);
            command.Parameters.AddWithValue("@AccessCodeId", compid);
            command.Parameters.AddWithValue("@YearId", yearId);

            var result = await command.ExecuteScalarAsync();
            return result?.ToString() ?? "";
        }


        public async Task<(DateTime? StartDate, DateTime? EndDate)> GetScheduleQuarterDateDetailsAsync(int iAcID, int iAuditID, int iQuarterID)
        {
            try
            {
                // ✅ Step 1: Get DB name from session
                string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

                if (string.IsNullOrEmpty(dbName))
                    throw new Exception("CustomerCode is missing in session. Please log in again.");
                var connectionString = _configuration.GetConnectionString(dbName);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"
                    SELECT SAI_StartDate, SAI_EndDate 
                    FROM StandardAudit_ScheduleIntervals 
                    WHERE SAI_SA_ID = @AuditID 
                      AND SAI_IntervalSubID = @QuarterID 
                      AND SAI_CompID = @AcID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@AuditID", iAuditID);
                        cmd.Parameters.AddWithValue("@QuarterID", iQuarterID);
                        cmd.Parameters.AddWithValue("@AcID", iAcID);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var startDate = reader["SAI_StartDate"] as DateTime?;
                                var endDate = reader["SAI_EndDate"] as DateTime?;
                                return (startDate, endDate);
                            }
                            return (null, null);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<dynamic?> LoadCustomerMasterAsync(int companyId, int customerId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            string sql = $@"SELECT * FROM SAD_CUSTOMER_MASTER 
                    WHERE CUST_ID = {customerId} AND CUST_COMPID = {companyId}";

            var result = await connection.QueryFirstOrDefaultAsync(sql);
            return result;
        }

        public async Task<string[]> SaveEmployeeDetailsAsync(EmployeeDto emp)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));
            await connection.OpenAsync();
            using var connectionMMCS = new SqlConnection(_configuration.GetConnectionString("CustomerRegistrationConnection"));
            await connectionMMCS.OpenAsync();

            // Step 1: Get auto-generated employee code if missing
            if (string.IsNullOrWhiteSpace(emp.UsrCode))
            {
                string empCode = await GetMaxEmployeeCodeAsync(
                    _configuration.GetConnectionString(dbName),
                    emp.UsrCompID
                );
                emp.UsrCode = empCode;
            }

            var parameters = new DynamicParameters();

            parameters.Add("@Usr_ID", emp.UserID);
            parameters.Add("@Usr_Node", emp.UsrNode);
            parameters.Add("@Usr_Code", emp.UsrCode);
            parameters.Add("@Usr_FullName", emp.UsrFullName);
            parameters.Add("@Usr_LoginName", emp.UsrEmail);
            parameters.Add("@Usr_Password", EncryptPassword(emp.UsrPassword));
            parameters.Add("@Usr_Email", emp.UsrEmail);
            parameters.Add("@Usr_Category", emp.UsrSentMail);
            parameters.Add("@Usr_Suggetions", emp.UsrSuggetions);
            parameters.Add("@usr_partner", emp.UsrPartner);
            parameters.Add("@Usr_LevelGrp", emp.UsrRole);
            parameters.Add("@Usr_DutyStatus", 'A');
            parameters.Add("@Usr_PhoneNo", emp.UsrPhoneNo);
            parameters.Add("@Usr_MobileNo", emp.UsrMobileNo);
            parameters.Add("@Usr_OfficePhone", emp.UsrOfficePhone);
            parameters.Add("@Usr_OffPhExtn", emp.UsrOffPhExtn);
            parameters.Add("@Usr_Designation", emp.UsrDesignation);
            parameters.Add("@Usr_CompanyID", emp.UsrCompanyID);
            parameters.Add("@Usr_OrgnID", emp.UsrOrgID);
            parameters.Add("@Usr_GrpOrUserLvlPerm", emp.UsrGrpOrUserLvlPerm);
            parameters.Add("@Usr_Role", emp.UsrRole);
            parameters.Add("@Usr_MasterModule", emp.UsrMasterModule);
            parameters.Add("@Usr_AuditModule", emp.UsrAuditModule);
            parameters.Add("@Usr_RiskModule", emp.UsrRiskModule);
            parameters.Add("@Usr_ComplianceModule", emp.UsrComplianceModule);
            parameters.Add("@Usr_BCMModule", emp.UsrBCMmodule);
            parameters.Add("@Usr_DigitalOfficeModule", emp.UsrDigitalOfficeModule);
            parameters.Add("@Usr_MasterRole", emp.UsrMasterRole);
            parameters.Add("@Usr_AuditRole", emp.UsrAuditRole);
            parameters.Add("@Usr_RiskRole", emp.UsrRiskRole);
            parameters.Add("@Usr_ComplianceRole", emp.UsrComplianceRole);
            parameters.Add("@Usr_BCMRole", emp.UsrBCMRole);
            parameters.Add("@Usr_DigitalOfficeRole", emp.UsrDigitalOfficeRole);
            parameters.Add("@Usr_CreatedBy", emp.UsrCreatedBy);
            parameters.Add("@Usr_UpdatedBy", emp.UsrCreatedBy);
            parameters.Add("@Usr_DelFlag", 'A');
            parameters.Add("@Usr_Status", 'A');
            parameters.Add("@Usr_IPAddress", emp.UsrIPAddress);
            parameters.Add("@Usr_CompId", emp.UsrCompID);
            parameters.Add("@Usr_Type", emp.UsrType);
            parameters.Add("@usr_IsSuperuser", emp.IsSuperuser);
            parameters.Add("@USR_DeptID", emp.DeptID);
            parameters.Add("@USR_MemberType", 0);
            parameters.Add("@USR_Levelcode", 0);

            parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

            // Call the stored procedure
            await connection.ExecuteAsync("spEmployeeMaster", parameters, commandType: CommandType.StoredProcedure);

            // Step 2: Update MMCS_CustomerRegistration.MCR_emails by appending emp.UsrEmail
            string customerCode = connection.Database;

            string existingEmails = await connectionMMCS.ExecuteScalarAsync<string>(
                "SELECT MCR_emails FROM [dbo].[MMCS_CustomerRegistration] WHERE MCR_CustomerCode = @CustomerCode",
                new { CustomerCode = customerCode }
            );

            // Append email only if not already present
            if (!string.IsNullOrWhiteSpace(emp.UsrEmail) && (existingEmails == null || !existingEmails.Split(',').Contains(emp.UsrEmail)))
            {
                var updatedEmails = string.IsNullOrWhiteSpace(existingEmails)
                    ? emp.UsrEmail
                    : existingEmails + "," + emp.UsrEmail;

                await connectionMMCS.ExecuteAsync(
                    "UPDATE [dbo].[MMCS_CustomerRegistration] SET MCR_emails = @Emails WHERE MCR_CustomerCode = @CustomerCode",
                    new { Emails = updatedEmails, CustomerCode = customerCode } // Replace 'trdm' if needed
                );
            }

            return new string[]
            {
        parameters.Get<int>("@iUpdateOrSave").ToString(),
        parameters.Get<int>("@iOper").ToString()
            };
        }
        private string EncryptPassword(string plainText)
        {
            string encryptionKey = "ML736@mmcs";
            byte[] salt = new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D,
                               0x65, 0x64, 0x76, 0x65, 0x64, 0x65,
                               0x76 };

            byte[] plainBytes = Encoding.Unicode.GetBytes(plainText);

            using var aes = Aes.Create();
            var pdb = new Rfc2898DeriveBytes(encryptionKey, salt);
            aes.Key = pdb.GetBytes(32);
            aes.IV = pdb.GetBytes(16);

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(plainBytes, 0, plainBytes.Length);
            cs.Close();

            return Convert.ToBase64String(ms.ToArray());
        }

        public async Task<string> GetMaxEmployeeCodeAsync(string connectionString, int companyId)
        {
            try
            {
                // Query to get MAX(Usr_ID) + 1
                var query = "SELECT ISNULL(MAX(Usr_ID) + 1, 1) FROM Sad_UserDetails WHERE Usr_CompId = @CompanyId";

                int maxId = 0;
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CompanyId", companyId);
                        var result = await command.ExecuteScalarAsync();
                        maxId = Convert.ToInt32(result);
                    }
                }

                // Format the ID like EMP001, EMP010, etc.
                string empCode = maxId switch
                {
                    < 10 => $"EMP00{maxId}",
                    < 100 => $"EMP0{maxId}",
                    _ => $"EMP{maxId}"
                };

                return empCode;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<object> LoadExistingEmployeeDetailsAsync(int companyId, int userId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));
            await connection.OpenAsync();

            string sql = @"
            SELECT 
                Usr_FullName,
                Usr_Code,
                Usr_LoginName,
                Usr_Email,
                Usr_MobileNo,
                Usr_PassWord,
                Usr_CompId,
                Usr_Role
            FROM Sad_UserDetails
            WHERE Usr_ID = @UserId AND Usr_CompId = @CompanyId";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@CompanyId", companyId);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new
                {
                    usr_FullName = reader["Usr_FullName"]?.ToString(),
                    usr_Code = reader["Usr_Code"]?.ToString(),
                    usr_LoginName = reader["Usr_LoginName"]?.ToString(),
                    usr_Email = reader["Usr_Email"]?.ToString(),
                    usr_MobileNo = reader["Usr_MobileNo"]?.ToString(),
                    usr_PassWord = DecryptPassword(reader["Usr_PassWord"]?.ToString() ?? ""),
                    usr_CompanyId = reader["Usr_CompId"]?.ToString(),
                    Usr_Role = reader["Usr_Role"]
                };
            }

            return null;
        }
        public string DecryptPassword(string encryptedValue)
        {
            string decryptionKey = "ML736@mmcs";
            byte[] cipherBytes = Convert.FromBase64String(encryptedValue);

            using (Aes aes = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(decryptionKey, new byte[]
                {
                0x49, 0x76, 0x61, 0x6E,
                0x20, 0x4D, 0x65, 0x64,
                0x76, 0x65, 0x64, 0x65, 0x76
                });

                aes.Key = pdb.GetBytes(32);
                aes.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }

                    return Encoding.Unicode.GetString(ms.ToArray());
                }
            }
        }
        public async Task<IEnumerable<dynamic>> LoadActiveRoleAsync(int companyId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            string sql = @"
            SELECT Mas_ID, Mas_Description 
            FROM SAD_GrpOrLvl_General_Master 
            WHERE Mas_Delflag = 'A' AND Mas_CompID = @CompanyId 
            ORDER BY Mas_Description";

            var result = await connection.QueryAsync(sql, new { CompanyId = companyId });
            return result;
        }

        public async Task<IEnumerable<dynamic>> GetUsersByCompanyAndRoleAsync(int companyId, int usrRole)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            var sql = @"
            SELECT Usr_ID, 
                   (Usr_FullName + ' - ' + Usr_Code) AS FullName 
            FROM Sad_UserDetails 
            WHERE Usr_CompId = @CompanyId AND usr_role = @UsrRole";

            var result = await connection.QueryAsync(sql, new { CompanyId = companyId, UsrRole = usrRole });

            return result;
        }

        public async Task<DataTable> LoadAuditScheduleIntervalAsync(int accessCodeId, int auditId, string format)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var conn = new SqlConnection(_configuration.GetConnectionString(dbName));

            var query = $@"
            SELECT 
                SAI_IntervalSubID As ID,
                SAI_StartDate FromDate,
                SAI_EndDate AS ToDate,
                CASE 
                    WHEN SAI_IntervalSubID = 0 THEN 'K Yearly'
                    WHEN SAI_IntervalSubID = 1 THEN 'Q1'
                    WHEN SAI_IntervalSubID = 2 THEN 'Q2'
                    WHEN SAI_IntervalSubID = 3 THEN 'Q3'
                    ELSE '' 
                END AS Description
            FROM StandardAudit_ScheduleIntervals
            WHERE SAI_SA_ID = @AuditId AND SAI_CompID = @CompanyId
            ORDER BY SAI_IntervalSubID";

            var reader = await conn.ExecuteReaderAsync(query, new { Format = format, AuditId = auditId, CompanyId = accessCodeId });

            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        public async Task<DataTable> LoadAssignedCheckPointsAndTeamMembersAsync(int accessCodeId, int auditId, int customerId, string heading, string format)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var conn = new SqlConnection(_configuration.GetConnectionString(dbName));

            var query = $@"
            SELECT 
                SACD_ID,
                SACD_CheckpointId,
                SACD_Heading,
                ISNULL(usr_FullName, '') AS Employee,
                SUM(LEN(SACD_CheckpointId) - LEN(REPLACE(SACD_CheckpointId, ',', '')) + 1) AS NoCheckpoints,
                CASE WHEN SACD_EmpId > 0 THEN 1 ELSE 0 END AS NoEmployee,
                SACD_TotalHr AS Working_Hours,
                CASE 
                    WHEN CONVERT(VARCHAR(10), SACD_EndDate, 103) = '01/01/1900' THEN '' 
                    ELSE SACD_EndDate
                END AS Timeline
            FROM StandardAudit_Checklist_Details
            LEFT JOIN sad_userdetails a ON SACD_EmpId = usr_Id
            WHERE SACD_AuditId = @AuditId AND SACD_CustID = @CustomerId
            {(string.IsNullOrEmpty(heading) ? "" : " AND SACD_Heading = @Heading")}
            GROUP BY 
                SACD_ID, SACD_Heading, SACD_EndDate, SACD_TotalHr, 
                SACD_CheckpointId, usr_FullName, SACD_EmpId";

            var reader = await conn.ExecuteReaderAsync(query, new
            {
                AuditId = auditId,
                CustomerId = customerId,
                Format = format,
                Heading = heading
            });

            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        public async Task<DataTable> GetFinalAuditTypeHeadingsAsync(int accessCodeId, int auditId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var conn = new SqlConnection(_configuration.GetConnectionString(dbName));

            var query = @"
            SELECT 
                ACM_Heading, 
                ACM_ID,
                ACM_Checkpoint,
                CASE WHEN SAC_Mandatory = 1 THEN 'Yes' ELSE 'No' END AS SAC_Mandatory
            FROM AuditType_Checklist_Master 
            JOIN StandardAudit_ScheduleCheckPointList 
                ON ACM_ID = SAC_CheckPointID
            WHERE SAC_SA_ID = @AuditId 
              AND SAC_CompID = @CompanyId 
              AND ACM_CompId = @CompanyId";

            var reader = await conn.ExecuteReaderAsync(query, new { AuditId = auditId, CompanyId = accessCodeId });

            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        public async Task<string> GetUserNamesAsync(int accessCodeId, List<int> engagementPartnerIds)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var conn = new SqlConnection(_configuration.GetConnectionString(dbName));

            var sql = @"
        SELECT STRING_AGG(usr_fullname, ', ') AS FullNames
        FROM SAD_Userdetails
        WHERE usr_id IN @UserIds";

            var result = await conn.QuerySingleOrDefaultAsync<string>(sql, new { UserIds = engagementPartnerIds });

            return result ?? "N/A";
        }

        public async Task<string> GetUserNames1Async(int accessCodeId, List<int> engagementPartnerIds)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var conn = new SqlConnection(_configuration.GetConnectionString(dbName));

            var sql = @"
        SELECT STRING_AGG(usr_fullname, ', ') AS FullNames
        FROM SAD_Userdetails
        WHERE usr_id IN @UserIds";

            var result = await conn.QuerySingleOrDefaultAsync<string>(sql, new { UserIds = engagementPartnerIds });

            return result ?? "N/A";
        }

        public async Task<string> GetUserNames2Async(int accessCodeId, List<int> engagementPartnerIds)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var conn = new SqlConnection(_configuration.GetConnectionString(dbName));

            var sql = @"
        SELECT STRING_AGG(usr_fullname, ', ') AS FullNames
        FROM SAD_Userdetails
        WHERE usr_id IN @UserIds";

            var result = await conn.QuerySingleOrDefaultAsync<string>(sql, new { UserIds = engagementPartnerIds });

            return result ?? "N/A";
        }

        public async Task<string> GetUserNames3Async(int accessCodeId, List<int> engagementPartnerIds)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var conn = new SqlConnection(_configuration.GetConnectionString(dbName));

            var sql = @"
        SELECT STRING_AGG(usr_fullname, ', ') AS FullNames
        FROM SAD_Userdetails
        WHERE usr_id IN @UserIds";

            var result = await conn.QuerySingleOrDefaultAsync<string>(sql, new { UserIds = engagementPartnerIds });

            return result ?? "N/A";
        }

        public string GetFormattedDate(string accessCode, int accessCodeId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

            var query = @"SELECT SAD_Config_Value 
                      FROM SAD_Config_Settings 
                      WHERE SAD_Config_Key = @Key AND SAD_CompId = @AccessCodeId";

            var configValue = connection.QueryFirstOrDefault<string>(query, new
            {
                Key = "DateFormat",
                AccessCodeId = accessCodeId
            });

            if (string.IsNullOrWhiteSpace(configValue))
                return DateTime.Now.ToString("dd-MMM-yyyy"); // default format

            return FormatDate(DateTime.Now, configValue);
        }

        private string FormatDate(DateTime dt, string code)
        {
            return code.Trim() switch
            {
                "1" => dt.ToString("dd-MMM-yy"),
                "2" => dt.ToString("dd/MM/yyyy"),
                "3" => dt.ToString("MM/dd/yyyy"),
                "4" => dt.ToString("yyyy/MM/dd"),
                "5" => dt.ToString("MMM-dd-yy"),
                "6" => dt.ToString("MMM/dd/yy"),
                "7" => dt.ToString("MM-dd-yyyy"),
                "8" => dt.ToString("MMM/dd/yyyy"),
                _ => dt.ToString("dd-MMM-yyyy") // fallback
            };
        }
        public async Task<IEnumerable<CustomerDto1>> GetCustomersAsync(int companyId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");
            using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));
            string query = "SELECT CUST_ID AS CustId, CUST_NAME AS CustName, CUST_CompID AS CompanyId FROM SAD_CUSTOMER_MASTER WHERE CUST_STATUS <> 'D' AND CUST_CompID = @CompanyId";

            return await connection.QueryAsync<CustomerDto1>(query, new { CompanyId = companyId });
        }
        //public async Task SaveGraceFormOperations(int accessCodeId, int userId, string module, string action, string operation, int yearId, string yearName, string auditNo, string remarks, string ipAddress)
        //{
        //    using var conn = new SqlConnection(_configuration.GetConnectionString(dbName));

        //    var query = @"
        //    INSERT INTO GraceForm_Operations 
        //        ( AccessCodeID, UserID, Module, Action, Operation, YearID, YearName, AuditNo, Remarks, IPAddress, ActionDate)
        //    VALUES 
        //        (@AccessCodeID, @UserID, @Module, @Action, @Operation, @YearID, @YearName, @AuditNo, @Remarks, @IPAddress, GETDATE())";

        //    await conn.ExecuteAsync(query, new
        //    {
        //        AccessCodeID = accessCodeId,
        //        UserID = userId,
        //        Module = module,
        //        Action = action,
        //        Operation = operation,
        //        YearID = yearId,
        //        YearName = yearName,
        //        AuditNo = auditNo,
        //        Remarks = remarks,
        //        IPAddress = ipAddress
        //    });
        //}
    }
}






