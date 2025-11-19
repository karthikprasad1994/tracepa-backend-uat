using Dapper;
using Microsoft.Data.SqlClient;
using Pipelines.Sockets.Unofficial.Arenas;
using System.Data;
using TracePca.Dto.Audit;
using TracePca.Dto.TaskManagement;
using TracePca.Interface.TaskManagement;

namespace TracePca.Service.TaskManagement
{
    public class TaskDashboardService : TaskDashboardInterface
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;

        public TaskDashboardService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _connectionString = GetConnectionStringFromSession();
        }

        private string GetConnectionStringFromSession()
        {
            var dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrWhiteSpace(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connStr = _configuration.GetConnectionString(dbName);
            if (string.IsNullOrWhiteSpace(connStr))
                throw new Exception($"Connection string for '{dbName}' not found in configuration.");

            return connStr;
        }

        public async Task<TaskDropDownListDataDTO> LoadAllDDLDataAsync(int compId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var parameters = new { CompId = compId };

                var currentYearTask = connection.QueryFirstOrDefaultAsync<DropDownListData>(
                    @"SELECT YMS_YEARID AS ID, YMS_ID AS Name FROM YEAR_MASTER WHERE YMS_Default = 1 AND YMS_CompId = @CompId", parameters);

                var yearList = connection.QueryAsync<DropDownListData>(
                    @"SELECT YMS_YEARID AS ID, YMS_ID AS Name FROM YEAR_MASTER WHERE YMS_CompId = 1 And YMS_YEARID > (SELECT YMS_YEARID FROM YEAR_MASTER WHERE YMS_Default = 1 AND YMS_CompId = @CompId) ORDER BY YMS_YEARID Asc", parameters);

                var customerListTask = connection.QueryAsync<DropDownListData>(
                    @"SELECT CUST_ID AS ID, CUST_NAME AS Name FROM SAD_CUSTOMER_MASTER WHERE CUST_DELFLG = 'A' AND CUST_CompID = @CompId ORDER BY CUST_NAME ASC", parameters);

                var projectListTask = connection.QueryAsync<DropDownListData>(
                    @"SELECT cmm_ID AS ID, cmm_Desc AS Name FROM Content_Management_Master WHERE CMM_Category = 'PRO' AND CMM_Delflag = 'A' AND CMM_CompID = @CompId ORDER BY cmm_Desc ASC", parameters);

                var taskListTask = connection.QueryAsync<DropDownListData>(
                    @"SELECT cmm_ID AS ID, cmm_Desc AS Name FROM Content_Management_Master WHERE CMM_Category = 'ASGT' AND CMM_Delflag = 'A' AND CMM_CompID = @CompId ORDER BY cmm_Desc ASC", parameters);

                var partnerListTask = connection.QueryAsync<DropDownListData>(
                    @"SELECT Usr_ID AS ID, USr_FullName AS Name FROM sad_userdetails WHERE usr_compID = @CompId AND Usr_Role IN (SELECT Mas_ID FROM SAD_GrpOrLvl_General_Master WHERE Mas_Delflag = 'A' AND Mas_CompID = @CompId AND Mas_Description = 'Partner') AND usr_DelFlag IN ('A', 'B', 'L') ORDER BY USr_FullName", parameters);

                var memberListTask = connection.QueryAsync<DropDownListData>(
                    @"SELECT Usr_ID AS ID, USr_FullName AS Name FROM sad_userdetails WHERE usr_compID = @CompId AND Usr_Role IN (SELECT Mas_ID FROM SAD_GrpOrLvl_General_Master WHERE Mas_Delflag = 'A' AND Mas_CompID = @CompId AND Mas_Description = 'Audit Assistant') AND usr_DelFlag IN ('A', 'B', 'L') ORDER BY USr_FullName", parameters);

                var frequencyListTask = connection.QueryAsync<DropDownListData>(
                    @"SELECT cmm_ID AS ID, cmm_Desc AS Name FROM Content_Management_Master WHERE CMM_Category = 'FRE' AND CMM_Delflag = 'A' AND CMM_CompID = @CompId ORDER BY cmm_Desc ASC", parameters);

                var workStatusListTask = connection.QueryAsync<DropDownListData>(
                    @"SELECT cmm_ID AS ID, cmm_Desc AS Name FROM Content_Management_Master WHERE CMM_Category = 'WS' AND CMM_Delflag = 'A' AND CMM_CompID = @CompId ORDER BY cmm_Desc ASC", parameters);

                await Task.WhenAll(currentYearTask, customerListTask, projectListTask, taskListTask, partnerListTask, memberListTask, frequencyListTask, workStatusListTask);

                return new TaskDropDownListDataDTO
                {
                    CurrentYear = await currentYearTask,
                    YearList = (await yearList).ToList(),
                    ClientList = (await customerListTask).ToList(),
                    ProjectList = (await projectListTask).ToList(),
                    TaskList = (await taskListTask).ToList(),
                    PartnerList = (await partnerListTask).ToList(),
                    TeamMemberList = (await memberListTask).ToList(),
                    FrequencyList = (await frequencyListTask).ToList(),
                    WorkStatusList = (await workStatusListTask).ToList()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading all DDL data.", ex);
            }
        }

        public async Task<TaskDashboardResponseDto> GetTaskScheduledDashboardDataAsync(TaskDashboardRequestDto dto)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string summarySql = @"
                    DECLARE @Today DATE = CAST(GETDATE() AS DATE);

                    SELECT SUM(CASE WHEN s.TMS_StatusId <> 2 AND s.TMS_DueDate < @Today THEN 1 ELSE 0 END) AS Overdue,
                        SUM(CASE WHEN CAST(s.TMS_CreatedOn AS DATE) = @Today THEN 1 ELSE 0 END) AS Today,
                        SUM(CASE WHEN s.TMS_StatusId <> 2 THEN 1 ELSE 0 END) AS OpenTasks,
                        SUM(CASE WHEN s.TMS_StatusId = 2 THEN 1 ELSE 0 END) AS ClosedTasks
                    FROM TaskManagement_Schedule s
                    WHERE s.TMS_CompId = @CompId
                      AND (@YearId = 0 OR s.TMS_FinancialYearId = @YearId)
                      AND (@ClientId = 0 OR s.TMS_CustomerId = @ClientId)
                      AND (@TaskId = 0 OR s.TMS_TaskId = @TaskId)
                      AND (@PartnerId = 0 OR EXISTS (SELECT 1 FROM STRING_SPLIT(s.TMS_PartnerIds, ',') x WHERE TRY_CAST(x.value AS INT) = @PartnerId));";

                const string listSql = @"
                    SELECT
                        s.TMS_Id AS ScheduleId,
                        s.TMS_TaskNo AS TaskNo,
                        ISNULL(c.CUST_NAME, '-') AS Client,
                        ISNULL(t.cmm_Desc, '-') AS Task,
                        CONVERT(varchar(10), s.TMS_StartDate, 103) AS StartDate,
                        CONVERT(varchar(10), s.TMS_DueDate, 103) AS ExpectedCompletion,
                        ISNULL(ws.cmm_Desc, '-') AS WorkStatus,
                        ISNULL(pu.PartnerNames, '-') AS Partner,
                        ISNULL(tmu.TeamMemberNames, '-') AS AssignedTo,
                        ISNULL((SELECT TOP 1 h.TMSH_Remarks FROM TaskManagement_SubTask_History h WHERE h.TMSH_SchedulePKId = s.TMS_Id ORDER BY h.TMSH_CreatedOn DESC), '-') AS Comments
                    FROM TaskManagement_Schedule s
                    LEFT JOIN SAD_CUSTOMER_MASTER c ON c.CUST_ID = s.TMS_CustomerId AND c.CUST_DELFLG = 'A'
                    LEFT JOIN Content_Management_Master t ON t.cmm_ID = s.TMS_TaskId AND t.CMM_Category = 'ASGT'
                    LEFT JOIN Content_Management_Master ws ON ws.cmm_ID = s.TMS_StatusId AND ws.CMM_Category = 'WS'
                    OUTER APPLY (SELECT STRING_AGG(u.USr_FullName, ', ') AS PartnerNames FROM sad_userdetails u WHERE CHARINDEX(',' + CAST(u.Usr_ID AS VARCHAR) + ',', ',' + s.TMS_PartnerIds + ',') > 0) pu
                    OUTER APPLY (SELECT STRING_AGG(u.USr_FullName, ', ') AS TeamMemberNames FROM sad_userdetails u WHERE CHARINDEX(',' + CAST(u.Usr_ID AS VARCHAR) + ',', ',' + s.TMS_TeamMemberIds + ',') > 0) tmu
                    WHERE s.TMS_CompId = @CompId
                      AND (@YearId = 0 OR s.TMS_FinancialYearId = @YearId)
                      AND (@ClientId = 0 OR s.TMS_CustomerId = @ClientId)
                      AND (@TaskId = 0 OR s.TMS_TaskId = @TaskId)
                      AND (@PartnerId = 0 OR EXISTS (SELECT 1 FROM STRING_SPLIT(s.TMS_PartnerIds, ',') x WHERE TRY_CAST(x.value AS INT) = @PartnerId))
                    ORDER BY s.TMS_Id DESC;";

                var parameters = new
                {
                    dto.CompId,
                    dto.YearId,
                    dto.ClientId,
                    dto.PartnerId,
                    dto.TaskId
                };

                var summary = await connection.QueryFirstOrDefaultAsync<TaskScheduledCountDto>(summarySql, parameters) ?? new TaskScheduledCountDto();
                var rows = (await connection.QueryAsync<TaskScheduledDataDto>(listSql, parameters)).ToList();
                return new TaskDashboardResponseDto
                {
                    TaskScheduledCount = summary,
                    TaskScheduledData = rows,
                    TotalTasks = rows.Count
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading Task Dashboard data.", ex);
            }
        }
    }
}