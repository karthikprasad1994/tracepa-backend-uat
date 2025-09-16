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
                        new MasterDropDownListData { ID = "ASF", Name = "AuditCompletionCheckPoint" },
                        new MasterDropDownListData { ID = "AT", Name = "AuditTaskOrAssignmentTask" },
                        new MasterDropDownListData { ID = "OE", Name = "EngagementFees" },
                        new MasterDropDownListData { ID = "FRE", Name = "Frequency" },
                        new MasterDropDownListData { ID = "MR", Name = "ManagementRepresentations" },
                        new MasterDropDownListData { ID = "TOR", Name = "TypeofReport" },
                        new MasterDropDownListData { ID = "TOT", Name = "TypeofTest" },
                        new MasterDropDownListData { ID = "WCM", Name = "WorkpaperChecklist" },
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
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            try
            {
                var query = @"
                SELECT CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMS_Remarks, CMS_KeyComponent, CMS_Module, CMM_Delflag, CMM_Status, CMM_ApprovedBy, CMM_ApprovedOn, CMM_IPAddress, CMM_CompID, 
                CMM_RiskCategory, CMM_CrBy, CMM_CrOn, CMM_UpdatedBy, CMM_UpdatedOn, CMM_Rate, CMM_Act, CMM_HSNSAC, CMM_AudrptType FROM Content_Management_Master
                WHERE CMM_Category = @CMM_Category And CMM_Status = @Status AND CMM_CompID = @CompID AND CMM_Delflag = 'A' ORDER BY CMM_Desc;";

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
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            try
            {
                var query = @"
                SELECT CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMS_Remarks, CMS_KeyComponent, CMS_Module, CMM_Delflag, CMM_Status, CMM_ApprovedBy, CMM_ApprovedOn, CMM_IPAddress, CMM_CompID, 
                CMM_RiskCategory, CMM_CrBy, CMM_CrOn, CMM_UpdatedBy, CMM_UpdatedOn, CMM_Rate, CMM_Act, CMM_HSNSAC, CMM_AudrptType FROM Content_Management_Master
                WHERE CMM_ID = @Id AND CMM_CompID = @CompID AND CMM_Delflag = 'A';";

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

        public async Task<(int Id, string Message, List<DropDownListData> MasterList)> SaveOrUpdateMasterDataAndGetRecordsAsync(ContentManagementMasterDTO dto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                bool isUpdate = (dto.CMM_ID ?? 0) > 0;

                var duplicateCount = await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1) FROM Content_Management_Master WHERE CMM_Desc = @CMM_Desc AND CMM_Category = @CMM_Category AND CMM_CompID = @CMM_CompID AND (CMM_ID <> @CMM_ID);",
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
                    return (0, "A record with the same description already exists.", new List<DropDownListData>());
                }

                if (isUpdate)
                {
                    await connection.ExecuteAsync(
                        @"UPDATE Content_Management_Master SET CMM_Desc = @CMM_Desc, CMS_Remarks = @CMS_Remarks, CMM_UpdatedBy = @CMM_UpdatedBy, CMM_UpdatedOn = GETDATE(), CMM_IPAddress = @CMM_IPAddress 
                        WHERE CMM_ID = @CMM_ID;", dto, transaction);
                }
                else
                {
                    var maxId = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(MAX(cmm_ID), 0) + 1 FROM Content_Management_Master WHERE CMM_CompID = @CompId;", new { CompId = dto.CMM_CompID }, transaction);
                    dto.CMM_Code = $"{dto.CMM_Category}_{maxId}";
                    dto.CMM_ID = await connection.ExecuteScalarAsync<int>(
                        @"DECLARE @NewId INT = ISNULL((SELECT MAX(CMM_ID) FROM Content_Management_Master), 0) + 1;
                        INSERT INTO Content_Management_Master (CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMS_Remarks, CMS_KeyComponent, CMS_Module, CMM_Delflag, CMM_Status, CMM_ApprovedBy, CMM_ApprovedOn, 
                        CMM_IPAddress, CMM_CompID, CMM_RiskCategory, CMM_CrBy, CMM_CrOn, CMM_Rate, CMM_Act, CMM_HSNSAC, CMM_AudrptType)
                        VALUES (@NewId, @CMM_Code, @CMM_Desc, @CMM_Category, @CMS_Remarks, @CMS_KeyComponent, @CMS_Module, 'A', 'W', @CMM_CrBy, GETDATE(), @CMM_IPAddress, @CMM_CompID, @CMM_RiskCategory, 
                        @CMM_CrBy, GETDATE(), @CMM_Rate, @CMM_Act, @CMM_HSNSAC, @CMM_AudrptType);
                        SELECT @NewId;", dto, transaction);
                }

                var masterList = (await connection.QueryAsync<DropDownListData>(@"SELECT CMM_ID AS ID, CMM_Desc AS Name FROM Content_Management_Master WHERE CMM_Category = @Category AND CMM_Delflag = 'A' AND 
                    CMM_CompID = @CompID ORDER BY CMM_Desc;", new { Category = dto.CMM_Category, CompID = dto.CMM_CompID }, transaction)).ToList();

                await transaction.CommitAsync();

                return ((dto.CMM_ID ?? 0), isUpdate ? "Updated successfully." : "Saved successfully.", masterList);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (0, "An error occurred while saving or updating the master data: " + ex.Message, new List<DropDownListData>());
            }
        }

        public async Task<(bool Success, string Message)> UpdateRecordsStatusAsync(List<int> ids, string action, int compId, int updatedBy, string ipAddress)
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
                        query = @"
                    UPDATE Content_Management_Master
                    SET CMM_Status = 'A', CMM_UpdatedBy = @UpdatedBy, CMM_UpdatedOn = GETDATE(), CMM_IPAddress = @IpAddress
                    WHERE CMM_ID IN @Ids AND CMM_CompID = @CompID AND CMM_Delflag = 'A';";
                        break;

                    case "revert":
                        query = @"
                    UPDATE Content_Management_Master
                    SET CMM_Status = 'R', CMM_UpdatedBy = @UpdatedBy, CMM_UpdatedOn = GETDATE(), CMM_IPAddress = @IpAddress
                    WHERE CMM_ID IN @Ids AND CMM_CompID = @CompID AND CMM_Delflag = 'A';";
                        break;

                    case "delete":
                        query = @"
                    UPDATE Content_Management_Master
                    SET CMM_Delflag = 'D', CMM_UpdatedBy = @UpdatedBy, CMM_UpdatedOn = GETDATE(), CMM_IPAddress = @IpAddress
                    WHERE CMM_ID IN @Ids AND CMM_CompID = @CompID AND CMM_Delflag = 'A';";
                        break;

                    default:
                        return (false, "Invalid action. Allowed actions: approve, revert, delete.");
                }

                var rowsAffected = await connection.ExecuteAsync(query, new
                {
                    UpdatedBy = updatedBy,
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
    }
}
