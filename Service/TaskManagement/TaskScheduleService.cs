using Dapper;
using Microsoft.Data.SqlClient;
using Pipelines.Sockets.Unofficial.Arenas;
using System.Data;
using TracePca.Dto.Audit;
using TracePca.Dto.TaskManagement;
using TracePca.Interface.TaskManagement;

namespace TracePca.Service.TaskManagement
{
    public class TaskScheduleService : TaskScheduleInterface
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;

        public TaskScheduleService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
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

        public async Task<IEnumerable<DropDownListData>> LoadAllModulesAsync(int compId, int projectId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var parameters = new { ProjectID = projectId, CompId = compId };

                var sql = @"SELECT TMM_ID AS ID, TMM_ModuleName AS Name FROM TRACe_Module_Master WHERE TMM_DELFLG = 'A' And TMM_ProjectID = @ProjectID AND TMM_CompId = @CompId ORDER BY TMM_ModuleName ASC";

                var result = await connection.QueryAsync<DropDownListData>(sql, parameters);
                return result.ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading Module/Phase data.", ex);
            }
        }

        public async Task<IEnumerable<DropDownListData>> loadSubTasksByTaskIdAsync(int compId, int taskId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var parameters = new { TaskId = taskId, CompId = compId };

                var sql = @"SELECT ACM_ID AS ID, ACM_Checkpoint AS Name FROM AssignmentTask_Checklist_Master WHERE ACM_DELFLG = 'A' AND ACM_AssignmentTaskID = @TaskId AND ACM_CompId = @CompId ORDER BY ACM_Checkpoint ASC";

                var result = await connection.QueryAsync<DropDownListData>(sql, parameters);
                return result.ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading SubTask(s) data.", ex);
            }
        }

        public async Task<string> GeneratTaskNoAsync(int compId, int yearId, int customerId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var nextSerialNo = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT COUNT(*) + 1 FROM TaskManagement_Schedule WHERE TMS_FinancialYearId = @YearId", new { YearId = yearId });
                var taskNo = await connection.QueryFirstOrDefaultAsync<string>(
                    @"SELECT CONCAT(cust.CUST_CODE, '/TASK/', yms.YMS_ID, '/', RIGHT('00000' + CAST(@SerialNo AS VARCHAR), 5)) AS LOE_Name
                    FROM SAD_CUSTOMER_MASTER cust, YEAR_MASTER yms WHERE cust.CUST_ID = @CustomerId AND yms.YMS_YEARID = @YearId",
                    new
                    {
                        CustomerId = customerId,
                        YearId = yearId,
                        SerialNo = nextSerialNo
                    });

                return taskNo ?? "";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating Task No.", ex);
            }
        }

        public async Task<int> SaveOrUpdateTaskScheduleAsync(TaskScheduleCreateDto dto)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var transaction = connection.BeginTransaction();
            try
            {
                if (dto is null) throw new ArgumentNullException(nameof(dto));
                if (dto.TMS_DueDate < dto.TMS_StartDate)
                    throw new ArgumentException("Due date cannot be before start date.");

                var isUpdate = dto.TMS_Id > 0;
                int scheduleId;

                if (isUpdate)
                {
                    const string updateSql =
                        "UPDATE TaskManagement_Schedule SET " +
                        "TMS_TaskNo = @TMS_TaskNo, " +
                        "TMS_FinancialYearId = @TMS_FinancialYearId, " +
                        "TMS_CustomerId = @TMS_CustomerId, " +
                        "TMS_ProjectId = @TMS_ProjectId, " +
                        "TMS_ModuleId = @TMS_ModuleId, " +
                        "TMS_TaskId = @TMS_TaskId, " +
                        "TMS_PartnerIds = @TMS_PartnerIds, " +
                        "TMS_TeamMemberIds = @TMS_TeamMemberIds, " +
                        "TMS_StartDate = @TMS_StartDate, " +
                        "TMS_DueDate = @TMS_DueDate, " +
                        "TMS_FrequencyId = @TMS_FrequencyId, " +
                        "TMS_Description = @TMS_Description, " +
                        "TMS_StatusId = @TMS_StatusId, " +
                        "TMS_ModifiedBy = @TMS_ModifiedBy, " +
                        "TMS_ModifiedOn = GETDATE(), " +
                        "TMS_AttachmentId = @TMS_AttachmentId, " +
                        "TMS_CompId = @TMS_CompId, " +
                        "TMS_IPAddress = @TMS_IPAddress " +
                        "WHERE TMS_ID = @TMS_Id;";

                    await connection.ExecuteAsync(updateSql, dto, transaction);
                    scheduleId = dto.TMS_Id;

                    const string deleteDetailsSql = "DELETE FROM TaskManagement_SubTask_Details WHERE TMSD_SchedulePKId = @TMS_Id;";
                    await connection.ExecuteAsync(deleteDetailsSql, new { dto.TMS_Id }, transaction);

                    const string deleteHistorySql = "DELETE FROM TaskManagement_SubTask_History WHERE TMSH_SchedulePKId = @TMS_Id;";
                    await connection.ExecuteAsync(deleteHistorySql, new { dto.TMS_Id }, transaction);
                }
                else
                {
                    const string nextIdSql = "SELECT ISNULL(MAX(TMS_ID), 0) + 1 FROM dbo.TaskManagement_Schedule;";
                    scheduleId = await connection.ExecuteScalarAsync<int>(nextIdSql, transaction: transaction);
                    var taskNo = await GeneratTaskNoAsync(dto.TMS_CompId, dto.TMS_FinancialYearId, dto.TMS_CustomerId);

                    const string insertSql =
                        "INSERT INTO dbo.TaskManagement_Schedule (TMS_ID, TMS_TaskNo, TMS_FinancialYearId, TMS_CustomerId, TMS_ProjectId, TMS_ModuleId, TMS_TaskId, " +
                        " TMS_PartnerIds, TMS_TeamMemberIds, TMS_StartDate, TMS_DueDate, TMS_FrequencyId, TMS_Description, TMS_StatusId, TMS_CreatedBy, TMS_CreatedOn, TMS_AttachmentId, TMS_CompId, TMS_IPAddress) " +
                        " VALUES (@TMS_ID, @TMS_TaskNo, @TMS_FinancialYearId, @TMS_CustomerId, @TMS_ProjectId, @TMS_ModuleId, @TMS_TaskId, @TMS_PartnerIds, @TMS_TeamMemberIds, @TMS_StartDate, @TMS_DueDate, " +
                        " @TMS_FrequencyId, @TMS_Description, @TMS_StatusId, @TMS_CreatedBy, GETDATE(), @TMS_AttachmentId, @TMS_CompId, @TMS_IPAddress);";

                    var insertParams = new
                    {
                        TMS_ID = scheduleId,
                        TMS_TaskNo = taskNo,
                        dto.TMS_FinancialYearId,
                        dto.TMS_CustomerId,
                        dto.TMS_ProjectId,
                        dto.TMS_ModuleId,
                        dto.TMS_TaskId,
                        dto.TMS_PartnerIds,
                        dto.TMS_TeamMemberIds,
                        dto.TMS_StartDate,
                        dto.TMS_DueDate,
                        dto.TMS_FrequencyId,
                        dto.TMS_Description,
                        dto.TMS_StatusId,
                        dto.TMS_CreatedBy,
                        dto.TMS_AttachmentId,
                        dto.TMS_CompId,
                        dto.TMS_IPAddress
                    };

                    await connection.ExecuteAsync(insertSql, insertParams, transaction);
                }

                var details = dto.TaskManagement_SubTask_Details?.Any() == true ? dto.TaskManagement_SubTask_Details : new List<TaskSubTaskDetailCreateDto>                    {
                new() { TMSD_SubtaskId = 0, TMSD_WorkStatusId = dto.TMS_StatusId, TMSD_CreatedBy = dto.TMS_CreatedBy, TMSD_CompId = dto.TMS_CompId, TMSD_IPAddress = dto.TMS_IPAddress }};

                const string nextDetailIdSql = "SELECT ISNULL(MAX(TMSD_Id), 0) + 1 FROM dbo.TaskManagement_SubTask_Details;";
                var nextDetailId = await connection.ExecuteScalarAsync<int>(nextDetailIdSql, transaction: transaction);

                const string insertDetailSql =
                    "INSERT INTO dbo.TaskManagement_SubTask_Details (TMSD_Id, TMSD_SchedulePKId, TMSD_SubtaskId, TMSD_WorkStatusId, TMSD_CreatedBy, TMSD_CreatedOn, " +
                    " TMSD_ApprovedBy, TMSD_ApprovedOn, TMSD_ModifiedBy, TMSD_ModifiedOn, TMSD_AttachmentId, TMSD_CompId, TMSD_IPAddress) " +
                    " VALUES (@TMSD_Id, @TMSD_SchedulePKId, @TMSD_SubtaskId, @TMSD_WorkStatusId, @TMSD_CreatedBy, GETDATE(), " +
                    " @TMSD_ApprovedBy, @TMSD_ApprovedOn, @TMSD_ModifiedBy, @TMSD_ModifiedOn, @TMSD_AttachmentId, @TMSD_CompId, @TMSD_IPAddress);";

                foreach (var d in details)
                {
                    var p = new
                    {
                        TMSD_Id = nextDetailId++,
                        TMSD_SchedulePKId = scheduleId,
                        d.TMSD_SubtaskId,
                        d.TMSD_WorkStatusId,
                        d.TMSD_CreatedBy,
                        d.TMSD_ApprovedBy,
                        d.TMSD_ApprovedOn,
                        d.TMSD_ModifiedBy,
                        d.TMSD_ModifiedOn,
                        d.TMSD_AttachmentId,
                        d.TMSD_CompId,
                        d.TMSD_IPAddress
                    };

                    await connection.ExecuteAsync(insertDetailSql, p, transaction);
                }

                await transaction.CommitAsync();
                return scheduleId;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while saviny/updaing Task schedule data", ex);
            }
        }

        public async Task<TaskDetailsResponseDto?> GetTaskScheduledDetailsByTaskIdAsync(int taskId)
        {
            using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            const string sqlHeader = @"
                SELECT TOP 1 s.TMS_Id, s.TMS_TaskNo, s.TMS_FinancialYearId, s.TMS_CustomerId, s.TMS_TaskId, s.TMS_PartnerIds, s.TMS_TeamMemberIds, s.TMS_Description, s.TMS_StatusId, s.TMS_ModifiedOn, 
                    ISNULL(c.CUST_NAME, '') AS ClientName, ISNULL(t.cmm_Desc, '') AS TaskName, ISNULL(y.YMS_ID, '') AS YearName
                FROM dbo.TaskManagement_Schedule s WITH (NOLOCK)
                LEFT JOIN dbo.SAD_CUSTOMER_MASTER c WITH (NOLOCK) ON c.CUST_ID = s.TMS_CustomerId
                LEFT JOIN dbo.Content_Management_Master t WITH (NOLOCK) ON t.cmm_ID = s.TMS_TaskId AND t.CMM_Category = 'ASGT'
                LEFT JOIN dbo.YEAR_MASTER y WITH (NOLOCK) ON y.YMS_YEARID = s.TMS_FinancialYearId
                WHERE s.TMS_Id = @TMS_Id
                ORDER BY s.TMS_Id DESC;";

            int? scheduleId = null;
            string partnerIdsCsv = "", teamIdsCsv = "";
            var header = new TaskScheduleDetailsDto();

            using (var cmd = new SqlCommand(sqlHeader, con))
            {
                cmd.Parameters.Add("@TMS_Id", SqlDbType.Int).Value = taskId;
                using var rdr = await cmd.ExecuteReaderAsync();
                if (!await rdr.ReadAsync()) return null;

                scheduleId = rdr.GetInt32(rdr.GetOrdinal("TMS_Id"));
                header.Year = rdr["YearName"]?.ToString() ?? "";
                header.TaskPkId = rdr.GetInt32(rdr.GetOrdinal("TMS_Id"));
                header.TaskId = rdr.GetInt32(rdr.GetOrdinal("TMS_TaskId"));
                header.TaskNo = rdr["TMS_TaskNo"]?.ToString() ?? "";
                header.ClientName = rdr["ClientName"]?.ToString() ?? "";
                header.TaskName = rdr["TaskName"]?.ToString() ?? "";
                header.StatusId = rdr.GetInt32(rdr.GetOrdinal("TMS_StatusId"));

                partnerIdsCsv = rdr["TMS_PartnerIds"]?.ToString() ?? "";
                teamIdsCsv = rdr["TMS_TeamMemberIds"]?.ToString() ?? "";

                header.PartnerNames = new List<string>();
                header.TeamMemberNames = new List<string>();
            }

            if (!string.IsNullOrWhiteSpace(partnerIdsCsv))
            {
                const string sqlPartners = @"WITH ids AS (SELECT TRY_CAST(value AS INT) AS Id FROM STRING_SPLIT(@Ids, ',') WHERE TRY_CAST(value AS INT) IS NOT NULL)
                    SELECT u.USr_FullName FROM dbo.sad_userdetails u WITH (NOLOCK) JOIN ids i ON i.Id = u.Usr_ID ORDER BY u.USr_FullName;";

                using var cmd = new SqlCommand(sqlPartners, con);
                cmd.Parameters.Add("@Ids", SqlDbType.VarChar, 4000).Value = partnerIdsCsv;
                using var rdr = await cmd.ExecuteReaderAsync();
                while (await rdr.ReadAsync())
                    header.PartnerNames.Add(rdr["USr_FullName"]?.ToString() ?? "");
            }

            if (!string.IsNullOrWhiteSpace(teamIdsCsv))
            {
                const string sqlTeams = @"WITH ids AS (SELECT TRY_CAST(value AS INT) AS Id FROM STRING_SPLIT(@Ids, ',') WHERE TRY_CAST(value AS INT) IS NOT NULL)
                    SELECT u.USr_FullName FROM dbo.sad_userdetails u WITH (NOLOCK) JOIN ids i ON i.Id = u.Usr_ID ORDER BY u.USr_FullName;";

                using var cmd = new SqlCommand(sqlTeams, con);
                cmd.Parameters.Add("@Ids", SqlDbType.VarChar, 4000).Value = teamIdsCsv;
                using var rdr = await cmd.ExecuteReaderAsync();
                while (await rdr.ReadAsync())
                    header.TeamMemberNames.Add(rdr["USr_FullName"]?.ToString() ?? "");
            }

            const string sqlRows = @"
                SELECT d.TMSD_Id, d.TMSD_SubtaskId, ISNULL(acm.ACM_Checkpoint, '') AS SubtaskName, d.TMSD_WorkStatusId, d.TMSD_Remarks
                FROM dbo.TaskManagement_SubTask_Details d WITH (NOLOCK)
                LEFT JOIN dbo.AssignmentTask_Checklist_Master acm WITH (NOLOCK) ON acm.ACM_ID = d.TMSD_SubtaskId
                WHERE d.TMSD_SchedulePKId = @ScheduleId
                ORDER BY d.TMSD_SubtaskId;
                ";

            var subTasks = new List<SubTaskListDto>();

            using (var cmd = new SqlCommand(sqlRows, con))
            {
                cmd.Parameters.Add("@ScheduleId", SqlDbType.Int).Value = scheduleId;

                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    var ordId = rdr.GetOrdinal("TMSD_Id");
                    var ordSubId = rdr.GetOrdinal("TMSD_SubtaskId");
                    var ordName = rdr.GetOrdinal("SubtaskName");
                    var ordWorkStatus = rdr.GetOrdinal("TMSD_WorkStatusId");
                    var ordRemarks = rdr.GetOrdinal("TMSD_Remarks");

                    while (await rdr.ReadAsync())
                    {
                        var pkId = !rdr.IsDBNull(ordId) ? rdr.GetInt32(ordId) : 0;
                        var subId = !rdr.IsDBNull(ordSubId) ? rdr.GetInt32(ordSubId) : 0;
                        var name = !rdr.IsDBNull(ordName) ? rdr.GetString(ordName) : string.Empty;
                        var workStatus = !rdr.IsDBNull(ordWorkStatus) ? rdr.GetInt32(ordWorkStatus) : (int?)null;
                        var remarks = !rdr.IsDBNull(ordRemarks) ? rdr.GetString(ordRemarks) : null;

                        subTasks.Add(new SubTaskListDto
                        {
                            SubtaskPkId = pkId,
                            SubtaskId = subId,
                            SubtaskName = name,
                            Comment = remarks,
                            StatusId = workStatus
                        });
                    }
                }
            }

            const string sqlHistory = @"
                SELECT h.TMSH_Id, h.TMSH_SchedulePKId, h.TMSH_SubtaskPKId, ISNULL(acm.ACM_Checkpoint, '') AS SubtaskName, h.TMSH_Remarks, h.TMSH_WorkStatusId, ud.USr_FullName As TMSH_CreatedBy, h.TMSH_CreatedOn
                FROM dbo.TaskManagement_SubTask_History h WITH (NOLOCK)
                LEFT JOIN dbo.AssignmentTask_Checklist_Master acm WITH (NOLOCK) ON acm.ACM_ID = h.TMSH_SubtaskPKId
                LEFT JOIN dbo.sad_userdetails ud WITH (NOLOCK) ON ud.Usr_ID = h.TMSH_CreatedBy
                WHERE h.TMSH_SchedulePKId = @ScheduleId
                ORDER BY h.TMSH_CreatedOn DESC;
            ";

            var historyList = new List<SubTaskListDto>();
            using (var cmd = new SqlCommand(sqlHistory, con))
            {
                cmd.Parameters.Add("@ScheduleId", SqlDbType.Int).Value = scheduleId;
                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    var ordSubTaskName = rdr.GetOrdinal("SubtaskName");
                    var ordHRemarks = rdr.GetOrdinal("TMSH_Remarks");
                    var ordHStatus = rdr.GetOrdinal("TMSH_WorkStatusId");
                    var ordHCreatedBy = rdr.GetOrdinal("TMSH_CreatedBy");
                    var ordHCreatedOn = rdr.GetOrdinal("TMSH_CreatedOn");

                    while (await rdr.ReadAsync())
                    {
                        historyList.Add(new SubTaskListDto
                        {
                            SubtaskName = !rdr.IsDBNull(ordSubTaskName) ? rdr.GetString(ordSubTaskName) : null,
                            Comment = !rdr.IsDBNull(ordHRemarks) ? rdr.GetString(ordHRemarks) : null,
                            StatusId = !rdr.IsDBNull(ordHStatus) ? rdr.GetInt32(ordHStatus) : (int?)null,
                            CreatedBy = !rdr.IsDBNull(ordHCreatedBy) ? rdr.GetString(ordHCreatedBy) : null,
                            CreatedOn = !rdr.IsDBNull(ordHCreatedOn) ? rdr.GetDateTime(ordHCreatedOn) : (DateTime?)null
                        });
                    }
                }
            }

            var response = new TaskDetailsResponseDto
            {
                TaskDetails = header,
                SubTaskList = subTasks,
                SubTaskHistory = historyList.Select(h => new SubTaskListDto
                {
                    SubtaskName = h.SubtaskName,
                    Comment = h.Comment,
                    StatusId = h.StatusId,
                    CreatedBy = h.CreatedBy,
                    CreatedOn = h.CreatedOn
                }).ToList()
            };
            return response;
        }

        public async Task<bool> UpdateTaskAndSubtasksAsync(TaskUpdateRequestDto dto)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        string updateTaskQuery = @"UPDATE TaskManagement_Schedule SET TMS_StatusId = @StatusId, TMS_Description = @Description, TMS_ModifiedBy = @ModifiedBy, TMS_ModifiedOn = GETDATE() 
                            WHERE TMS_Id = @TaskPKId And TMS_TaskId = @TaskId";

                        using (var cmd = new SqlCommand(updateTaskQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@TaskPKId", dto.TaskPKId);
                            cmd.Parameters.AddWithValue("@TaskId", dto.TaskId);
                            cmd.Parameters.AddWithValue("@StatusId", dto.StatusId ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Description", dto.Description ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ModifiedBy", dto.ModifiedBy);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        foreach (var subtask in dto.SubTasks)
                        {
                            string insertSubtaskDetailQuery = @"Update TaskManagement_SubTask_Details set TMSD_WorkStatusId = @StatusId,TMSD_Remarks = @Description, TMSD_ModifiedBy = @ModifiedBy, TMSD_ModifiedOn = GETDATE()
                                Where TMSD_Id = @SubtaskPkId And TMSD_SchedulePKId = @SchedulePKId And TMSD_SubtaskId = @SubtaskId";
                            int subtaskDetailId;
                            using (var cmd = new SqlCommand(insertSubtaskDetailQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@SchedulePKId", dto.TaskPKId);
                                cmd.Parameters.AddWithValue("@SubtaskPkId", subtask.SubtaskPkId);
                                cmd.Parameters.AddWithValue("@SubtaskId", subtask.SubtaskId);
                                cmd.Parameters.AddWithValue("@StatusId", subtask.StatusId ?? (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@Description", dto.Description ?? (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@ModifiedBy", dto.ModifiedBy);

                                subtaskDetailId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                            }

                            const string nextHistoryIdSql = "SELECT ISNULL(MAX(TMSH_Id), 0) + 1 FROM dbo.TaskManagement_SubTask_History;";
                            var nextDetailId = await connection.ExecuteScalarAsync<int>(nextHistoryIdSql, transaction: transaction);

                            string insertHistoryQuery = @"INSERT INTO TaskManagement_SubTask_History
                                (TMSH_Id, TMSH_SchedulePKId, TMSH_SubtaskPKId, TMSH_Remarks, TMSH_WorkStatusId, TMSH_CreatedBy, TMSH_CreatedOn, TMSD_CompId, TMSD_IPAddress)
                                VALUES (@PKId, @SchedulePKId, @SubtaskPKId, @Remarks, @WorkStatusId, @CreatedBy, GETDATE(), 1, @IPAddress)";

                            using (var cmd = new SqlCommand(insertHistoryQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@PKId", nextDetailId);
                                cmd.Parameters.AddWithValue("@SchedulePKId", dto.TaskPKId);
                                cmd.Parameters.AddWithValue("@SubtaskPKId", subtask.SubtaskPkId);
                                cmd.Parameters.AddWithValue("@Remarks", subtask.Comment ?? (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@WorkStatusId", subtask.StatusId ?? (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@CreatedBy", dto.ModifiedBy);
                                cmd.Parameters.AddWithValue("@IPAddress", "");
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}