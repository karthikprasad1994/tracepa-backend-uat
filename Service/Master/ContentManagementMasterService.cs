using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TracePca.Data;
using TracePca.Dto;
using TracePca.Dto.Audit;
using TracePca.Interface.Audit;
using TracePca.Interface.Master;

namespace TracePca.Service.Master
{
    public class ContentManagementMasterService : ContentManagementMasterInterface
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;

        public ContentManagementMasterService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
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

        public async Task<(bool Success, string Message, MasterDropDownListDataDTO? Data)> LoadAllMasterDDLDataAsync(int compId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var parameters = new { CompId = compId };

                var MasterList = new List<MasterDropDownListData>
                    {
                        new MasterDropDownListData { ID = "ASF", Name = "Audit Completion CheckPoint" },
                        new MasterDropDownListData { ID = "AT", Name = "Audit Type" },
                        new MasterDropDownListData { ID = "ASGT", Name = "Task" },
                        new MasterDropDownListData { ID = "DRL", Name = "Document Request List" },
                        //new MasterDropDownListData { ID = "ROLE", Name = "Employee Role" },
                        new MasterDropDownListData { ID = "OE", Name = "Engagement Fees" },
                        new MasterDropDownListData { ID = "FRE", Name = "Frequency" },
                        new MasterDropDownListData { ID = "IND", Name = "Industry Type" },
                        new MasterDropDownListData { ID = "MNG", Name = "Management" },
                        //new MasterDropDownListData { ID = "MR", Name = "Management Representations" },
                        new MasterDropDownListData { ID = "MSN", Name = "Membership Type" },
                        new MasterDropDownListData { ID = "ORG", Name = "Organization Type" },
                        new MasterDropDownListData { ID = "PRO", Name = "Product/Project" },
                        new MasterDropDownListData { ID = "TM", Name = "Tax Master" },
                        //new MasterDropDownListData { ID = "TOR", Name = "Type of Report" },
                        new MasterDropDownListData { ID = "TOT", Name = "Type of Test" },
                        //new MasterDropDownListData { ID = "JE", Name = "Types of JE Transactions" },
                        //new MasterDropDownListData { ID = "LT", Name = "Types of LT Transactions" },
                        //new MasterDropDownListData { ID = "UM", Name = "Unit of Measurement" },
                        new MasterDropDownListData { ID = "WS", Name = "Work Status" },
                        new MasterDropDownListData { ID = "WCM", Name = "Workpaper Checklist" },
                    };

                var AuditFrameworkList = new List<DropDownListData>
                    {
                        new DropDownListData { ID = 0, Name = "ICAI" },
                        new DropDownListData { ID = 1, Name = "PCAOB" },
                    };

                var dto = new MasterDropDownListDataDTO
                {
                    MasterList = MasterList,
                    AuditFrameworkList = AuditFrameworkList,
                };

                return (true, "Master DDL data loaded successfully.", dto);
            }
            catch (Exception ex)
            {
                return (false, "An error occurred while loading all DDL data: " + ex.Message, null);
            }
        }

        public async Task<(bool Success, string Message, List<ContentManagementMasterDTO> Data)> GetMasterDataByStatusAsync(string type, string status, int compId)
        {
            using var connection = new SqlConnection(_connectionString);
            try
            {
                var query = @"SELECT CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMS_Remarks, CMS_KeyComponent, CMS_Module, CMM_Delflag, CMM_Status, CMM_ApprovedBy, CMM_ApprovedOn, CMM_IPAddress, CMM_CompID, CMM_RiskCategory, CMM_CrBy, CMM_CrOn, 
                CMM_UpdatedBy, CMM_UpdatedOn, CMM_Rate, CMM_Act, CMM_HSNSAC, CMM_AudrptType FROM Content_Management_Master WHERE CMM_Category = @CMM_Category And CMM_CompID = @CompID AND CMM_Delflag = @Status ORDER BY CMM_Desc;";

                var result = await connection.QueryAsync<ContentManagementMasterDTO>(query, new { CMM_Category = type, Status = status, CompID = compId });
                return (true, "Records fetched successfully.", result.ToList());
            }
            catch (Exception ex)
            {
                return (false, "Error while fetching records by status: " + ex.Message, new List<ContentManagementMasterDTO>());
            }
        }

        public async Task<(bool Success, string Message, ContentManagementMasterDTO? Data)> GetMasterDataByIdAsync(int id, int compId)
        {
            using var connection = new SqlConnection(_connectionString);
            try
            {
                var query = @"SELECT CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMS_Remarks, CMS_KeyComponent, CMS_Module, CMM_Delflag, CMM_Status, CMM_ApprovedBy, CMM_ApprovedOn, CMM_IPAddress, CMM_CompID, 
                CMM_RiskCategory, CMM_CrBy, CMM_CrOn, CMM_UpdatedBy, CMM_UpdatedOn, CMM_Rate, CMM_Act, CMM_HSNSAC, CMM_AudrptType FROM Content_Management_Master WHERE CMM_ID = @Id AND CMM_CompID = @CompID;";

                var result = await connection.QueryFirstOrDefaultAsync<ContentManagementMasterDTO>(query, new { Id = id, CompID = compId });

                if (result == null)
                    return (true, "No record found with the given Id.", null);

                return (true, "Record fetched successfully.", result);
            }
            catch (Exception ex)
            {
                return (false, "Error while fetching record by Id: " + ex.Message, null);
            }
        }

        public async Task<(bool Success, string Message, List<string>? Data)> GetActMasterDataAsync(int compId)
        {
            using var connection = new SqlConnection(_connectionString);
            try
            {
                var query = @"SELECT DISTINCT CMM_Act FROM Content_Management_Master WHERE CMM_CompID = @CompID AND CMM_Act IS NOT NULL AND CMM_Act <> '' AND CMM_Act <> 'NULL';";
                var result = (await connection.QueryAsync<string>(query, new { CompID = compId })).ToList();

                if (result == null || !result.Any())
                    return (true, "No records found.", null);

                return (true, "Act records fetched successfully.", result);
            }
            catch (Exception ex)
            {
                return (false, "Error while fetching Act records: " + ex.Message, null);
            }
        }

        public async Task<(int Id, string Message, List<ContentManagementMasterDTO> MasterList)> SaveOrUpdateMasterDataAndGetRecordsAsync(ContentManagementMasterDTO dto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                bool isUpdate = (dto.CMM_ID ?? 0) > 0;
                var duplicateCount = await connection.ExecuteScalarAsync<int>(@"SELECT COUNT(1) FROM Content_Management_Master WHERE CMM_Desc = @CMM_Desc AND CMM_Category = @CMM_Category AND CMM_CompID = @CMM_CompID AND (CMM_ID <> @CMM_ID);",
                    new
                    {
                        dto.CMM_Desc,
                        dto.CMM_Category,
                        dto.CMM_CompID,
                        dto.CMM_ID
                    }, transaction);

                if (duplicateCount > 0)
                {
                    await transaction.RollbackAsync();
                    return (0, "A record with the same Name/Description already exists.", new List<ContentManagementMasterDTO>());
                }

                if (isUpdate)
                {
                    await connection.ExecuteAsync(@"UPDATE Content_Management_Master SET CMM_Desc = @CMM_Desc, CMS_Remarks = @CMS_Remarks, CMM_UpdatedBy = @CMM_UpdatedBy, CMM_UpdatedOn = GETDATE(), CMM_Status = 'U', CMM_IPAddress = @CMM_IPAddress, 
                    CMM_Rate = @CMM_Rate, CMM_Act = @CMM_Act, CMM_HSNSAC = @CMM_HSNSAC, CMM_AudrptType = @CMM_AudrptType WHERE CMM_ID = @CMM_ID;", dto, transaction);
                }
                else
                {
                    var maxId = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(MAX(cmm_ID), 0) + 1 FROM Content_Management_Master WHERE CMM_CompID = @CompId;", new { CompId = dto.CMM_CompID }, transaction);
                    dto.CMM_Code = $"{dto.CMM_Category}_{maxId}";
                    dto.CMM_ID = await connection.ExecuteScalarAsync<int>(
                        @"DECLARE @NewId INT = ISNULL((SELECT MAX(CMM_ID) FROM Content_Management_Master), 0) + 1;
                        INSERT INTO Content_Management_Master (CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMS_Remarks, CMS_KeyComponent, CMS_Module, CMM_Delflag, CMM_Status, CMM_ApprovedBy, CMM_ApprovedOn, 
                        CMM_IPAddress, CMM_CompID, CMM_RiskCategory, CMM_CrBy, CMM_CrOn, CMM_Rate, CMM_Act, CMM_HSNSAC, CMM_AudrptType)
                        VALUES (@NewId, @CMM_Code, @CMM_Desc, @CMM_Category, @CMS_Remarks, @CMS_KeyComponent, @CMS_Module, 'A', 'A', @CMM_CrBy, GETDATE(), @CMM_IPAddress, @CMM_CompID, @CMM_RiskCategory, 
                        @CMM_CrBy, GETDATE(), @CMM_Rate, @CMM_Act, @CMM_HSNSAC, @CMM_AudrptType);
                        SELECT @NewId;", dto, transaction);
                }

                var masterList = (await connection.QueryAsync<ContentManagementMasterDTO>(@"SELECT CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMS_Remarks, CMS_KeyComponent, CMS_Module, CMM_Delflag, CMM_Status, CMM_ApprovedBy, CMM_ApprovedOn, 
                CMM_IPAddress, CMM_CompID, CMM_RiskCategory, CMM_CrBy, CMM_CrOn, CMM_UpdatedBy, CMM_UpdatedOn, CMM_Rate, CMM_Act, CMM_HSNSAC, CMM_AudrptType FROM Content_Management_Master
                WHERE CMM_Category = @Category And CMM_CompID = @CompID AND CMM_Delflag = 'A' ORDER BY CMM_Desc;", new { Category = dto.CMM_Category, CompID = dto.CMM_CompID }, transaction)).ToList();

                await transaction.CommitAsync();

                return ((dto.CMM_ID ?? 0), isUpdate ? "Updated successfully." : "Saved successfully.", masterList);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (0, "An error occurred while saving or updating the master data: " + ex.Message, new List<ContentManagementMasterDTO>());
            }
        }

        public async Task<(bool Success, string Message)> UpdateMasterRecordsStatusAsync(List<int> ids, string action, int compId, int updatedBy, string ipAddress)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                if (ids == null || ids.Count == 0)
                    return (false, "No IDs provided for update.");

                string query = string.Empty;
                switch (action.ToLower())
                {
                    case "approve":
                        query = @"UPDATE Content_Management_Master SET CMM_Status = 'A',  CMM_Delflag = 'A', CMM_ApprovedBy = @userId, CMM_ApprovedOn = GETDATE(), CMM_IPAddress = @IpAddress WHERE CMM_ID IN @Ids AND CMM_CompID = @CompID;";
                        break;

                    case "revert":
                        query = @"UPDATE Content_Management_Master SET CMM_Status = 'R', CMM_Delflag = 'A', CMM_RecallBy = @userId, CMM_RecallOn = GETDATE(), CMM_IPAddress = @IpAddress WHERE CMM_ID IN @Ids AND CMM_CompID = @CompID;";
                        break;

                    case "delete":
                        query = @"UPDATE Content_Management_Master SET CMM_Status = 'D', CMM_Delflag = 'D', CMM_DeletedBy = @userId, CMM_DeletedOn = GETDATE(), CMM_IPAddress = @IpAddress WHERE CMM_ID IN @Ids AND CMM_CompID = @CompID;";
                        break;

                    default:
                        return (false, "Invalid action. Allowed actions: approve, revert, delete.");
                }

                var rowsAffected = await connection.ExecuteAsync(query, new
                {
                    userId = updatedBy,
                    IpAddress = ipAddress,
                    Ids = ids,
                    CompID = compId
                }, transaction);

                await transaction.CommitAsync();

                return (true, $"{rowsAffected} record(s) updated successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, "Error while updating records: " + ex.Message);
            }
        }

        public async Task<(bool Success, string Message, List<string>? Data)> GetAuditTypeChecklistHeadingDataAsync(int compId)
        {
            using var connection = new SqlConnection(_connectionString);
            try
            {
                var query = @"SELECT DISTINCT ACM_Heading FROM AuditType_Checklist_Master WHERE ACM_CompId = @CompID AND ACM_Heading IS NOT NULL AND ACM_Heading <> '' AND ACM_Heading <> 'NULL';";
                var result = (await connection.QueryAsync<string>(query, new { CompID = compId })).ToList();

                if (result == null || !result.Any())
                    return (true, "No records found.", null);

                return (true, "Act records fetched successfully.", result);
            }
            catch (Exception ex)
            {
                return (false, "Error while fetching Checklist heading records: " + ex.Message, null);
            }
        }

        public async Task<(bool Success, string Message, List<AuditTypeChecklistMasterDTO> Data)> GetAuditTypeChecklistByStatusAsync(int typeId, string status, int compId)
        {
            using var connection = new SqlConnection(_connectionString);
            try
            {
                var query = @"SELECT * FROM AuditType_Checklist_Master WHERE ACM_AuditTypeID = @TypeId AND ACM_CompId = @CompID AND ACM_DELFLG = @Status ORDER BY ACM_Heading;";

                var result = await connection.QueryAsync<AuditTypeChecklistMasterDTO>(query, new { TypeId = typeId, Status = status, CompID = compId });
                return (true, "Records fetched successfully.", result.ToList());
            }
            catch (Exception ex)
            {
                return (false, "Error while fetching records by status: " + ex.Message, new List<AuditTypeChecklistMasterDTO>());
            }
        }

        public async Task<(bool Success, string Message, AuditTypeChecklistMasterDTO? Data)> GetAuditTypeChecklistByIdAsync(int id, int compId)
        {
            using var connection = new SqlConnection(_connectionString);
            try
            {
                var query = @"SELECT * FROM AuditType_Checklist_Master WHERE ACM_ID = @Id AND ACM_CompId = @CompID;";

                var result = await connection.QueryFirstOrDefaultAsync<AuditTypeChecklistMasterDTO>(query, new { Id = id, CompID = compId });

                if (result == null)
                    return (true, "No record found with the given Id.", null);

                return (true, "Record fetched successfully.", result);
            }
            catch (Exception ex)
            {
                return (false, "Error while fetching record by Id: " + ex.Message, null);
            }
        }

        public async Task<(int Id, string Message, List<AuditTypeChecklistMasterDTO> MasterList)> SaveOrUpdateAuditTypeChecklistAndGetRecordsAsync(AuditTypeChecklistMasterDTO dto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                bool isUpdate = (dto.ACM_ID ?? 0) > 0;

                var duplicateCount = await connection.ExecuteScalarAsync<int>(@"SELECT COUNT(1) FROM AuditType_Checklist_Master WHERE ACM_Checkpoint = @ACM_Checkpoint AND ACM_AuditTypeID = @ACM_AuditTypeID AND ACM_CompId = @ACM_CompId AND (ACM_ID <> @ACM_ID);",
                    new
                    {
                        dto.ACM_Checkpoint,
                        dto.ACM_AuditTypeID,
                        dto.ACM_CompId,
                        dto.ACM_ID
                    }, transaction);

                if (duplicateCount > 0)
                {
                    await transaction.RollbackAsync();
                    return (0, "A record with the same Name already exists.", new List<AuditTypeChecklistMasterDTO>());
                }

                if (isUpdate)
                {
                    await connection.ExecuteAsync(@"UPDATE AuditType_Checklist_Master SET ACM_Heading = @ACM_Heading, ACM_Checkpoint = @ACM_Checkpoint, ACM_Assertions = @ACM_Assertions, ACM_UpdatedBy = @ACM_UpdatedBy, 
                                                ACM_UpdatedOn = GETDATE(), ACM_Status = 'U', ACM_IPAddress = @ACM_IPAddress WHERE ACM_ID = @ACM_ID;", dto, transaction);
                }
                else
                {
                    var maxId = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(MAX(ACM_ID), 0) + 1 FROM AuditType_Checklist_Master WHERE ACM_CompId = @CompId;", new { CompId = dto.ACM_CompId }, transaction);
                    dto.ACM_Code = $"ACM_{maxId}";
                    dto.ACM_ID = await connection.ExecuteScalarAsync<int>(
                        @"DECLARE @NewId INT = ISNULL((SELECT MAX(ACM_ID) FROM AuditType_Checklist_Master), 0) + 1;
                          INSERT INTO AuditType_Checklist_Master (ACM_ID, ACM_Code, ACM_AuditTypeID, ACM_Heading, ACM_Checkpoint, ACM_Assertions, ACM_DELFLG, ACM_STATUS, ACM_CRBY, ACM_CRON, ACM_IPAddress, ACM_CompId)
                          VALUES (@NewId, @ACM_Code, @ACM_AuditTypeID, @ACM_Heading, @ACM_Checkpoint, @ACM_Assertions, 'A', 'W', @ACM_CRBY, GETDATE(), @ACM_IPAddress, @ACM_CompId);
                          SELECT @NewId;", dto, transaction);
                }

                var masterList = (await connection.QueryAsync<AuditTypeChecklistMasterDTO>(@"SELECT * FROM AuditType_Checklist_Master WHERE ACM_CompId = @CompID AND ACM_DELFLG = 'A' ORDER BY ACM_Heading;",
                    new { CompID = dto.ACM_CompId }, transaction)).ToList();

                await transaction.CommitAsync();

                return ((dto.ACM_ID ?? 0), isUpdate ? "Updated successfully." : "Saved successfully.", masterList);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (0, "Error while saving or updating: " + ex.Message, new List<AuditTypeChecklistMasterDTO>());
            }
        }

        public async Task<(bool Success, string Message)> UpdateAuditTypeChecklistStatusAsync(List<int> ids, string action, int compId, int userId, string ipAddress)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                if (ids == null || ids.Count == 0)
                    return (false, "No IDs provided for update.");

                string query = string.Empty;
                switch (action.ToLower())
                {
                    case "approve":
                        query = @"UPDATE AuditType_Checklist_Master SET ACM_Status = 'A', ACM_DELFLG = 'A', ACM_ApprovedBy = @UserId, ACM_ApprovedOn = GETDATE(), ACM_IPAddress = @IpAddress WHERE ACM_ID IN @Ids AND ACM_CompId = @CompID;";
                        break;

                    case "revert":
                        query = @"UPDATE AuditType_Checklist_Master SET ACM_Status = 'R', ACM_DELFLG = 'A', ACM_RecallBy = @UserId, ACM_RecallOn = GETDATE(), ACM_IPAddress = @IpAddress WHERE ACM_ID IN @Ids AND ACM_CompId = @CompID;";
                        break;

                    case "delete":
                        query = @"UPDATE AuditType_Checklist_Master SET ACM_Status = 'D', ACM_DELFLG = 'D', ACM_DeletedBy = @UserId, ACM_DeletedOn = GETDATE(), ACM_IPAddress = @IpAddress WHERE ACM_ID IN @Ids AND ACM_CompId = @CompID;";
                        break;

                    default:
                        return (false, "Invalid action. Allowed actions: approve, revert, delete.");
                }

                if (string.IsNullOrEmpty(query))
                    return (false, "Invalid action. Allowed: approve, revert, delete.");

                var rows = await connection.ExecuteAsync(query, new { UserId = userId, IpAddress = ipAddress, Ids = ids, CompID = compId }, transaction);
                await transaction.CommitAsync();
                return (true, $"{rows} record(s) updated successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, "Error while updating records: " + ex.Message);
            }
        }

        public async Task<(bool Success, string Message, List<string>? Data)> GetAssignmentTaskChecklistHeadingDataAsync(int compId)
        {
            using var connection = new SqlConnection(_connectionString);
            try
            {
                var query = @"SELECT DISTINCT ACM_Heading FROM AssignmentTask_Checklist_Master WHERE ACM_CompId = @CompID AND ACM_Heading IS NOT NULL AND ACM_Heading <> '' AND ACM_Heading <> 'NULL';";
                var result = (await connection.QueryAsync<string>(query, new { CompID = compId })).ToList();

                if (result == null || !result.Any())
                    return (true, "No records found.", null);

                return (true, "Act records fetched successfully.", result);
            }
            catch (Exception ex)
            {
                return (false, "Error while fetching Checklist heading records: " + ex.Message, null);
            }
        }

        public async Task<(bool Success, string Message, List<AssignmentTaskChecklistMasterDTO> Data)> GetAssignmentTaskChecklistByStatusAsync(int taskId, string status, int compId)
        {
            using var connection = new SqlConnection(_connectionString);
            try
            {
                var query = @"SELECT * FROM AssignmentTask_Checklist_Master WHERE ACM_AssignmentTaskID = @TaskId AND ACM_CompId = @CompID AND ACM_DELFLG = @Status ORDER BY ACM_Heading;";

                var result = await connection.QueryAsync<AssignmentTaskChecklistMasterDTO>(query, new { TaskId = taskId, Status = status, CompID = compId });
                return (true, "Records fetched successfully.", result.ToList());
            }
            catch (Exception ex)
            {
                return (false, "Error while fetching records by status: " + ex.Message, new List<AssignmentTaskChecklistMasterDTO>());
            }
        }

        public async Task<(bool Success, string Message, AssignmentTaskChecklistMasterDTO? Data)> GetAssignmentTaskChecklistByIdAsync(int id, int compId)
        {
            using var connection = new SqlConnection(_connectionString);
            try
            {
                var query = @"SELECT * FROM AssignmentTask_Checklist_Master WHERE ACM_ID = @Id AND ACM_CompId = @CompID;";

                var result = await connection.QueryFirstOrDefaultAsync<AssignmentTaskChecklistMasterDTO>(query, new { Id = id, CompID = compId });

                if (result == null)
                    return (true, "No record found with the given Id.", null);

                return (true, "Record fetched successfully.", result);
            }
            catch (Exception ex)
            {
                return (false, "Error while fetching record by Id: " + ex.Message, null);
            }
        }

        public async Task<(int Id, string Message, List<AssignmentTaskChecklistMasterDTO> MasterList)> SaveOrUpdateAssignmentTaskChecklistAndGetRecordsAsync(AssignmentTaskChecklistMasterDTO dto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                bool isUpdate = (dto.ACM_ID ?? 0) > 0;

                var duplicateCount = await connection.ExecuteScalarAsync<int>(@"SELECT COUNT(1) FROM AssignmentTask_Checklist_Master WHERE ACM_Checkpoint = @ACM_Checkpoint AND ACM_AssignmentTaskID = @ACM_AssignmentTaskID AND ACM_CompId = @ACM_CompId AND (ACM_ID <> @ACM_ID);",
                    new
                    {
                        dto.ACM_Checkpoint,
                        dto.ACM_AssignmentTaskID,
                        dto.ACM_CompId,
                        dto.ACM_ID
                    }, transaction);

                if (duplicateCount > 0)
                {
                    await transaction.RollbackAsync();
                    return (0, "A record with the same Name already exists.", new List<AssignmentTaskChecklistMasterDTO>());
                }

                if (isUpdate)
                {
                    await connection.ExecuteAsync(@"UPDATE AssignmentTask_Checklist_Master SET ACM_Heading = @ACM_Heading, ACM_Checkpoint = @ACM_Checkpoint, ACM_BillingType = @ACM_BillingType,
                    ACM_UpdatedBy = @ACM_UpdatedBy, ACM_UpdatedOn = GETDATE(), ACM_Status = 'U', ACM_IPAddress = @ACM_IPAddress WHERE ACM_ID = @ACM_ID;", dto, transaction);
                }
                else
                {
                    var maxId = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(MAX(ACM_ID), 0) + 1 FROM AssignmentTask_Checklist_Master WHERE ACM_CompId = @CompId;", new { CompId = dto.ACM_CompId }, transaction);
                    dto.ACM_Code = $"ACM_{maxId}";
                    dto.ACM_ID = await connection.ExecuteScalarAsync<int>(
                        @"DECLARE @NewId INT = ISNULL((SELECT MAX(ACM_ID) FROM AssignmentTask_Checklist_Master), 0) + 1;
                          INSERT INTO AssignmentTask_Checklist_Master (ACM_ID, ACM_Code, ACM_AssignmentTaskID, ACM_Heading, ACM_Checkpoint, ACM_BillingType, ACM_DELFLG, ACM_STATUS, ACM_CRBY, ACM_CRON, ACM_IPAddress, ACM_CompId)
                          VALUES (@NewId, @ACM_Code, @ACM_AssignmentTaskID, @ACM_Heading, @ACM_Checkpoint, @ACM_BillingType, 'A', 'W', @ACM_CRBY, GETDATE(), @ACM_IPAddress, @ACM_CompId);
                          SELECT @NewId;", dto, transaction);
                }

                var masterList = (await connection.QueryAsync<AssignmentTaskChecklistMasterDTO>(@"SELECT * FROM AssignmentTask_Checklist_Master WHERE ACM_CompId = @CompID AND ACM_DELFLG = 'A' ORDER BY ACM_Heading;",
                    new { CompID = dto.ACM_CompId }, transaction)).ToList();

                await transaction.CommitAsync();

                return ((dto.ACM_ID ?? 0), isUpdate ? "Updated successfully." : "Saved successfully.", masterList);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (0, "Error while saving or updating: " + ex.Message, new List<AssignmentTaskChecklistMasterDTO>());
            }
        }

        public async Task<(bool Success, string Message)> UpdateAssignmentTaskChecklistStatusAsync(List<int> ids, string action, int compId, int userId, string ipAddress)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                if (ids == null || ids.Count == 0)
                    return (false, "No IDs provided for update.");

                string query = string.Empty;
                switch (action.ToLower())
                {
                    case "approve":
                        query = @"UPDATE AssignmentTask_Checklist_Master SET ACM_Status = 'A', ACM_DELFLG = 'A', ACM_ApprovedBy = @UserId, ACM_ApprovedOn = GETDATE(), ACM_IPAddress = @IpAddress WHERE ACM_ID IN @Ids AND ACM_CompId = @CompID;";
                        break;

                    case "revert":
                        query = @"UPDATE AssignmentTask_Checklist_Master SET ACM_Status = 'R', ACM_DELFLG = 'A', ACM_RecallBy = @UserId, ACM_RecallOn = GETDATE(), ACM_IPAddress = @IpAddress WHERE ACM_ID IN @Ids AND ACM_CompId = @CompID;";
                        break;

                    case "delete":
                        query = @"UPDATE AssignmentTask_Checklist_Master SET ACM_Status = 'D', ACM_DELFLG = 'D', ACM_DeletedBy = @UserId, ACM_DeletedOn = GETDATE(), ACM_IPAddress = @IpAddress WHERE ACM_ID IN @Ids AND ACM_CompId = @CompID;";
                        break;

                    default:
                        return (false, "Invalid action. Allowed actions: approve, revert, delete.");
                }

                if (string.IsNullOrEmpty(query))
                    return (false, "Invalid action. Allowed: approve, revert, delete.");

                var rows = await connection.ExecuteAsync(query, new { UserId = userId, IpAddress = ipAddress, Ids = ids, CompID = compId }, transaction);
                await transaction.CommitAsync();
                return (true, $"{rows} record(s) updated successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, "Error while updating records: " + ex.Message);
            }
        }

        public async Task<(bool Success, string Message, List<AuditCompletionSubPointMasterDTO> Data)> GetAuditSubPointsByStatusAsync(int checkPointId, string status, int compId)
        {
            using var connection = new SqlConnection(_connectionString);
            try
            {
                var query = @"SELECT * FROM AuditCompletion_SubPoint_Master WHERE ASM_CheckpointID = @ASM_CheckpointID AND ASM_CompId = @CompID AND ASM_DELFLG = @Status ORDER BY ASM_SubPoint;";

                var result = await connection.QueryAsync<AuditCompletionSubPointMasterDTO>(query, new { ASM_CheckpointID = checkPointId, Status = status, CompID = compId });
                return (true, "Records fetched successfully.", result.ToList());
            }
            catch (Exception ex)
            {
                return (false, "Error while fetching records by status: " + ex.Message, new List<AuditCompletionSubPointMasterDTO>());
            }
        }

        public async Task<(bool Success, string Message, AuditCompletionSubPointMasterDTO? Data)> GetAuditSubPointByIdAsync(int id, int compId)
        {
            using var connection = new SqlConnection(_connectionString);
            try
            {
                var query = @"SELECT * FROM AuditCompletion_SubPoint_Master WHERE ASM_ID = @Id AND ASM_CompId = @CompID;";

                var result = await connection.QueryFirstOrDefaultAsync<AuditCompletionSubPointMasterDTO>(query, new { Id = id, CompID = compId });

                if (result == null)
                    return (true, "No record found with the given Id.", null);

                return (true, "Record fetched successfully.", result);
            }
            catch (Exception ex)
            {
                return (false, "Error while fetching record by Id: " + ex.Message, null);
            }
        }

        public async Task<(int Id, string Message, List<AuditCompletionSubPointMasterDTO> MasterList)> SaveOrUpdateAuditSubPointAndGetRecordsAsync(AuditCompletionSubPointMasterDTO dto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                bool isUpdate = (dto.ASM_ID ?? 0) > 0;

                var duplicateCount = await connection.ExecuteScalarAsync<int>(@"SELECT COUNT(1) FROM AuditCompletion_SubPoint_Master WHERE ASM_SubPoint = @ASM_SubPoint AND ASM_CheckpointID = @ASM_CheckpointID AND ASM_CompId = @ASM_CompId AND (ASM_ID <> @ASM_ID);",
                    new
                    {
                        dto.ASM_SubPoint,
                        dto.ASM_CheckpointID,
                        dto.ASM_CompId,
                        dto.ASM_ID
                    }, transaction);

                if (duplicateCount > 0)
                {
                    await transaction.RollbackAsync();
                    return (0, "A record with the same Name already exists..", new List<AuditCompletionSubPointMasterDTO>());
                }

                if (isUpdate)
                {
                    await connection.ExecuteAsync(@"UPDATE AuditCompletion_SubPoint_Master SET ASM_SubPoint = @ASM_SubPoint, ASM_Remarks = @ASM_Remarks, ASM_UpdatedBy = @ASM_UpdatedBy, ASM_UpdatedOn = GETDATE(), ASM_Status = 'U', ASM_IPAddress = @ASM_IPAddress WHERE ASM_ID = @ASM_ID;", dto, transaction);
                }
                else
                {
                    var maxId = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(MAX(ASM_ID), 0) + 1 FROM AuditCompletion_SubPoint_Master WHERE ASM_CompId = @CompId;", new { CompId = dto.ASM_CompId }, transaction);
                    dto.ASM_Code = $"ASM_{maxId}";
                    dto.ASM_ID = await connection.ExecuteScalarAsync<int>(@" DECLARE @NewId INT = ISNULL((SELECT MAX(ASM_ID) FROM AuditCompletion_SubPoint_Master), 0) + 1;
                    INSERT INTO AuditCompletion_SubPoint_Master (ASM_ID, ASM_Code, ASM_CheckpointID, ASM_SubPoint, ASM_Remarks, ASM_DELFLG, ASM_STATUS, ASM_CRBY, ASM_CRON, ASM_IPAddress, ASM_CompId)
                    VALUES (@NewId, @ASM_Code, @ASM_CheckpointID, @ASM_SubPoint, @ASM_Remarks, 'A', 'W', @ASM_CRBY, GETDATE(), @ASM_IPAddress, @ASM_CompId);
                    SELECT @NewId;", dto, transaction);
                }

                var masterList = (await connection.QueryAsync<AuditCompletionSubPointMasterDTO>(@"SELECT * FROM AuditCompletion_SubPoint_Master WHERE ASM_CompId = @CompID AND ASM_DELFLG = 'A' ORDER BY ASM_SubPoint;", new { CompID = dto.ASM_CompId }, transaction)).ToList();

                await transaction.CommitAsync();

                return ((dto.ASM_ID ?? 0), isUpdate ? "Updated successfully." : "Saved successfully.", masterList);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (0, "Error while saving or updating: " + ex.Message, new List<AuditCompletionSubPointMasterDTO>());
            }
        }

        public async Task<(bool Success, string Message)> UpdateAuditSubPointStatusAsync(List<int> ids, string action, int compId, int userId, string ipAddress)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                if (ids == null || ids.Count == 0)
                    return (false, "No IDs provided for update.");

                string query = string.Empty;

                switch (action.ToLower())
                {
                    case "approve":
                        query = @"UPDATE AuditCompletion_SubPoint_Master SET ASM_Status = 'A', ASM_DELFLG = 'A', ASM_ApprovedBy = @UserId, ASM_ApprovedOn = GETDATE(), ASM_IPAddress = @IpAddress WHERE ASM_ID IN @Ids AND ASM_CompId = @CompID;";
                        break;

                    case "revert":
                        query = @"UPDATE AuditCompletion_SubPoint_Master SET ASM_Status = 'R', ASM_DELFLG = 'A', ASM_RecallBy = @UserId, ASM_RecallOn = GETDATE(), ASM_IPAddress = @IpAddress WHERE ASM_ID IN @Ids AND ASM_CompId = @CompID;";
                        break;

                    case "delete":
                        query = @"UPDATE AuditCompletion_SubPoint_Master SET ASM_Status = 'D', ASM_DELFLG = 'D', ASM_DeletedBy = @UserId, ASM_DeletedOn = GETDATE(), ASM_IPAddress = @IpAddress WHERE ASM_ID IN @Ids AND ASM_CompId = @CompID;";
                        break;

                    default:
                        return (false, "Invalid action. Allowed actions: approve, revert, delete.");
                }

                var rows = await connection.ExecuteAsync(query, new { UserId = userId, IpAddress = ipAddress, Ids = ids, CompID = compId }, transaction);
                await transaction.CommitAsync();
                return (true, $"{rows} record(s) updated successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, "Error while updating records: " + ex.Message);
            }
        }


        public async Task<(bool Success, string Message, List<TRACeModuleMasterDTO> Data)> GetTRACeModuleByStatusAsync(int projectId, string status, int compId)
        {
            using var connection = new SqlConnection(_connectionString);
            try
            {
                var query = @"SELECT * FROM TRACe_Module_Master WHERE TMM_ProjectID = @ProjectID AND TMM_CompId = @CompID AND TMM_DELFLG = @Status ORDER BY TMM_ModuleName;";

                var result = await connection.QueryAsync<TRACeModuleMasterDTO>(query, new { ProjectID = projectId, Status = status, CompID = compId });
                return (true, "Records fetched successfully.", result.ToList());
            }
            catch (Exception ex)
            {
                return (false, "Error while fetching records by status: " + ex.Message, new List<TRACeModuleMasterDTO>());
            }
        }

        public async Task<(bool Success, string Message, TRACeModuleMasterDTO? Data)> GetTRACeModuleByIdAsync(int id, int compId)
        {
            using var connection = new SqlConnection(_connectionString);
            try
            {
                var query = @"SELECT * FROM TRACe_Module_Master WHERE TMM_ID = @Id AND TMM_CompId = @CompID;";

                var result = await connection.QueryFirstOrDefaultAsync<TRACeModuleMasterDTO>(query, new { Id = id, CompID = compId });

                if (result == null)
                    return (true, "No record found with the given Id.", null);

                return (true, "Record fetched successfully.", result);
            }
            catch (Exception ex)
            {
                return (false, "Error while fetching record by Id: " + ex.Message, null);
            }
        }

        public async Task<(int Id, string Message, List<TRACeModuleMasterDTO> MasterList)> SaveOrUpdateTRACeModuleAndGetRecordsAsync(TRACeModuleMasterDTO dto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                bool isUpdate = (dto.TMM_ID ?? 0) > 0;

                var duplicateCount = await connection.ExecuteScalarAsync<int>(@"SELECT COUNT(1) FROM TRACe_Module_Master WHERE TMM_ModuleName = @TMM_ModuleName AND TMM_ProjectID = @TMM_ProjectID AND TMM_CompId = @TMM_CompId AND (TMM_ID <> @TMM_ID);",
                    new
                    {
                        dto.TMM_ModuleName,
                        dto.TMM_ProjectID,
                        dto.TMM_CompId,
                        dto.TMM_ID
                    }, transaction);

                if (duplicateCount > 0)
                {
                    await transaction.RollbackAsync();
                    return (0, "A module with the same name already exists.", new List<TRACeModuleMasterDTO>());
                }

                if (isUpdate)
                {
                    await connection.ExecuteAsync(@"UPDATE TRACe_Module_Master SET TMM_ModuleName = @TMM_ModuleName, TMM_UPDATEDBY = @TMM_UPDATEDBY, TMM_UPDATEDON = GETDATE(), TMM_STATUS = @TMM_STATUS, TMM_IPAddress = @TMM_IPAddress WHERE TMM_ID = @TMM_ID;", dto, transaction);
                }
                else
                {
                    var maxId = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(MAX(TMM_ID), 0) + 1 FROM TRACe_Module_Master WHERE TMM_CompId = @CompId;", new { CompId = dto.TMM_CompId }, transaction);

                    dto.TMM_Code = $"TMM_{maxId}";
                    dto.TMM_ID = await connection.ExecuteScalarAsync<int>(@"
                        DECLARE @NewId INT = ISNULL((SELECT MAX(TMM_ID) FROM TRACe_Module_Master), 0) + 1;
                        INSERT INTO TRACe_Module_Master (TMM_ID, TMM_Code, TMM_ProjectID, TMM_ModuleName, TMM_DELFLG, TMM_STATUS, TMM_CRBY, TMM_CRON, TMM_IPAddress, TMM_CompId)
                        VALUES (@NewId, @TMM_Code, @TMM_ProjectID, @TMM_ModuleName, 'A', 'W', @TMM_CRBY, GETDATE(), @TMM_IPAddress, @TMM_CompId);
                        SELECT @NewId;",
                        dto, transaction);
                }

                var masterList = (await connection.QueryAsync<TRACeModuleMasterDTO>(@"SELECT * FROM TRACe_Module_Master WHERE TMM_CompId = @CompID AND TMM_DELFLG = 'A' ORDER BY TMM_ModuleName;", new { CompID = dto.TMM_CompId }, transaction)).ToList();

                await transaction.CommitAsync();

                return ((dto.TMM_ID ?? 0), isUpdate ? "Updated successfully." : "Saved successfully.", masterList);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (0, "Error while saving or updating: " + ex.Message, new List<TRACeModuleMasterDTO>());
            }
        }

        public async Task<(bool Success, string Message)> UpdateTRACeModuleStatusAsync(List<int> ids, string action, int compId, int userId, string ipAddress)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                if (ids == null || ids.Count == 0)
                    return (false, "No IDs provided for update.");

                string query = string.Empty;

                switch (action.ToLower())
                {
                    case "approve":
                        query = @"UPDATE TRACe_Module_Master SET TMM_STATUS = 'A', TMM_DELFLG = 'A', TMM_APPROVEDBY = @UserId, TMM_APPROVEDON = GETDATE(), TMM_IPAddress = @IpAddress WHERE TMM_ID IN @Ids AND TMM_CompId = @CompID;";
                        break;
                    case "revert":
                        query = @"UPDATE TRACe_Module_Master SET TMM_STATUS = 'R', TMM_DELFLG = 'A', TMM_RECALLBY = @UserId, TMM_RECALLON = GETDATE(), TMM_IPAddress = @IpAddress WHERE TMM_ID IN @Ids AND TMM_CompId = @CompID;";
                        break;
                    case "delete":
                        query = @"UPDATE TRACe_Module_Master SET TMM_STATUS = 'D', TMM_DELFLG = 'D', TMM_DELETEDBY = @UserId, TMM_DELETEDON = GETDATE(), TMM_IPAddress = @IpAddress WHERE TMM_ID IN @Ids AND TMM_CompId = @CompID;";
                        break;
                    default:
                        return (false, "Invalid action. Allowed actions: approve, revert, delete.");
                }

                var rows = await connection.ExecuteAsync(query, new { UserId = userId, IpAddress = ipAddress, Ids = ids, CompID = compId }, transaction);
                await transaction.CommitAsync();
                return (true, $"{rows} record(s) updated successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, "Error while updating records: " + ex.Message);
            }
        }
    }
}
