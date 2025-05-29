using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using TracePca.Data;
using TracePca.Dto.Audit;
using TracePca.Interface;
using TracePca.Interface.Audit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace TracePca.Service.Audit
{
    public class DashboardAndSchedule : AuditAndDashboardInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public DashboardAndSchedule(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }


        public async Task<List<DashboardAndScheduleDto>> GetDashboardAuditAsync(
      int? id, int? customerId, int? compId, int? financialYearId, int? loginUserId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query;
            object parameters;
            string sSql = @"
    SELECT Usr_ID 
    FROM sad_userdetails 
    WHERE usr_compID = @CompId 
      AND USR_Partner = 1 
      AND (usr_DelFlag = 'A' OR usr_DelFlag = 'B' OR usr_DelFlag = 'L') 
      AND Usr_ID = @UserId";

             parameters = new
            {
                CompId = compId,   // <-- Make sure iCompId is defined with the company ID
                UserId = loginUserId   // <-- Assuming this is the correct user ID to check
             };

            // Execute the query and check if any records exist
            var Partner = await connection.QueryFirstOrDefaultAsync<int?>(sSql, parameters);

            // Convert result to a boolean flag
            bool loginUserIsPartner = Partner.HasValue;

            if (id.HasValue)
            {
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

                parameters = new { Id = id.Value, CompId = compId };
            }
            else
            {
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
            ELSE '-' 
        END AS AuditStatus,
        Partner = ISNULL(STUFF((
            SELECT DISTINCT '; ' + CAST(usr_FullName AS VARCHAR(MAX))
            FROM Sad_UserDetails 
            WHERE usr_id IN (
                SELECT value 
                FROM STRING_SPLIT(
                    (
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
    {(financialYearId > 0 ? "AND b.SA_YearID = @financialYearId AND LOE_YearId = @financialYearId" : "")}
    {(customerId > 0 ? "AND b.SA_CustID = @customerId AND LOE_CustomerId = @customerId" : "")}
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
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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


        public async Task<List<AuditTypeCustomerDto>> GetAuditTypesByCustomerAsync(int compId, string sType)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"
        SELECT CMM_ID AS PKID, CMM_Desc AS Name
        FROM Content_Management_Master
        WHERE CMM_Category = @SType
          AND CMM_CompID = @CompId
          AND CMM_Delflag = 'A'
          AND CMS_KeyComponent = 0
          ORDER BY CMM_Desc ASC";

            var result = await connection.QueryAsync<AuditTypeCustomerDto>(query, new
            {
                CompId = compId,
                SType = sType
            });

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
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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
                    AND ACM_Heading = @Heading
                    AND ACM_ID in (
                        SELECT SAC_CheckPointID 
                        FROM StandardAudit_ScheduleCheckPointList 
                        WHERE SAC_SA_ID = @AuditId 
                          AND SAC_CompID = @CompId
                    )
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
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            try
            {
                var sql = @"
                DELETE FROM StandardAudit_Checklist_Details
                WHERE SACD_AuditId = @AuditId
                  AND SACD_CustID = @CustomerId
                  AND SACD_ID = @PkId
                  AND SACD_CompID = @CompanyId";

                var affectedRows = await connection.ExecuteAsync(sql, new
                {
                    dto.AuditId,
                    dto.CustomerId,
                    dto.PkId,
                    dto.CompId
                });

                return affectedRows > 0;
            }
            catch (Exception)
            {
                throw; // Or log and return false depending on your logging approach
            }
        }
        public async Task<IEnumerable<AuditChecklistDto>> GetChecklistAsync(int auditId, int companyId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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
            var sql = @"SELECT CUST_CODE FROM sad_Customer_master 
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

                    return Regex.Replace(result ?? string.Empty, @"\s", "").Trim();
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

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
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

                    string formattedId = maxId.ToString("D5"); // e.g., 00001

                    // If GetCustomerCode accesses DB, modify it to accept a transaction or connection
                    // Otherwise, it can be called without transaction if it's safe
                    string customerPrefix = await GetCustomerCode(connectionString, dto.SA_CompID, dto.SA_CustID);

                    string jobCode = $"{customerPrefix}/AUD/{dto.SA_YearID}/{formattedId}";

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
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(sSql, parameters);
            }
        }

        // Helper method to execute scalar SQL command
        private async Task<T> ExecuteScalarAsync<T>(string sAC, string sSql, object parameters)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                return await connection.ExecuteScalarAsync<T>(sSql, parameters);
            }
        }

        public async Task<string[]> SaveUpdateStandardAuditChecklistDetailsAsync(StandardAuditChecklistDetailsDto objSACLD)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
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
                // Construct SQL delete query
                string sSql = "DELETE FROM StandardAudit_ScheduleCheckPointList " +
                              "WHERE SAC_SA_ID = @AuditId " +
                              "AND SAC_CheckPointID IN (@SelectedCheckPointsPKID) " +
                              "AND SAC_CompID = @CustRegAccessCodeId";

                using var cmdDelete = new SqlCommand(sSql, connection)
                {
                    CommandType = CommandType.Text
                };

                // Add parameters to avoid SQL injection
                cmdDelete.Parameters.AddWithValue("@AuditId", objSACLD.SACD_AuditId);
                cmdDelete.Parameters.AddWithValue("@SelectedCheckPointsPKID", objSACLD.SelectedCheckPointsPKID);
                cmdDelete.Parameters.AddWithValue("@CustRegAccessCodeId", objSACLD.SACD_CompId);

                // Execute the command
                await cmdDelete.ExecuteNonQueryAsync();
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
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection3")))
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

                    // Add output parameters
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
                    var query = @"UPDATE SAD_CUSTOMER_MASTER 
      SET CUST_DelFlg = 'A'
      WHERE CUST_CompID = @CompId AND CUST_ID = @Cust_Id";

                    await connection.ExecuteAsync(query, new { CompId = iCompId, Cust_Id = dto.CUST_ID }); // assuming custId is defined

                    try
                    {
                        await command.ExecuteNonQueryAsync();

                        int updateOrSave = (int)updateOrSaveParam.Value;
                        int oper = (int)operParam.Value;

                        return new int[] { updateOrSave, oper };
                    }
                    catch (Exception ex)
                    {
                        // Handle or log the error
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
            var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return await connection.QueryAsync<AuditChecklistDto>(sql, parameters);

        }
        public async Task<List<AssignedCheckpointDto>> GetAssignedCheckpointsAsync(
      int auditId, int custId, string heading)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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

            // Append heading filter condition only if valid
            if (!string.IsNullOrWhiteSpace(heading))
            {
                sql += " AND SACD_Heading = @Heading";
            }

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
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Save/Update Audit Schedule via Stored Procedure
                        var parameters = new DynamicParameters();
                        parameters.Add("@SA_ID", dto.SA_ID);
                        parameters.Add("@SA_AuditNo", dto.AuditNo);
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
                        parameters.Add("@iUpdateOrSave", 2, direction: ParameterDirection.InputOutput);
                        parameters.Add("@iOper", 1, direction: ParameterDirection.InputOutput);
                        parameters.Add("@Out_Message", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
                        parameters.Add("@Out_AuditScheduleID", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        await connection.ExecuteAsync("spStandardAudit_Schedule", parameters, transaction, commandType: CommandType.StoredProcedure);
                        int newAuditScheduleId = parameters.Get<int>("@SA_ID");

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
                        string checkpointQuery = "SELECT SAC_ID FROM StandardAudit_ScheduleCheckPointList WHERE SAC_SA_ID = @AuditId AND SAC_CompID = @CompId";
                        var checkpointRows = (await connection.QueryAsync<int>(checkpointQuery, new
                        {
                            AuditId = dto.SA_ID,
                            CompId = dto.CompID
                        }, transaction)).ToList();

                        foreach (var sacId in checkpointRows)
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
    }
}

    
    



