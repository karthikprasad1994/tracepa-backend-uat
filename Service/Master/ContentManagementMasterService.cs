using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TracePca.Data;
using TracePca.Dto;
using TracePca.Dto.Audit;
using TracePca.Interface.Master;

namespace TracePca.Service.Master
{
    public class ContentManagementMasterService : ContentManagementMasterInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor HttpContextAccessor;

        public ContentManagementMasterService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            HttpContextAccessor = httpContextAccessor;
        }

        //Frequency - FRE
        //EngagementFees - OE
        //TypeofReport - TOR
        //TypeofTest - TOT
        //ManagementRepresentations - MR
        //AuditTaskOrAssignmentTask - AT
        //WorkpaperChecklist - WCM
        //AuditCompletionCheckPoint - ASF

        public async Task<(int Id, string Message, List<DropDownListData> MasterList)> SaveOrUpdateMasterDataAndGetRecordsAsync(ContentManagementMasterDTO dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
                        INSERT INTO Content_Management_Master (CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMS_Remarks, CMS_KeyComponent, CMS_Module, CMM_Delflag, CMM_Status, CMM_ApprovedBy, CMM_ApprovedOn, CMM_IPAddress, CMM_CompID, CMM_RiskCategory, CMM_CrBy, CMM_CrOn, CMM_Rate, CMM_Act, CMM_HSNSAC, CMM_AudrptType)
                        VALUES (@NewId, @CMM_Code, @CMM_Desc, @CMM_Category, @CMS_Remarks, @CMS_KeyComponent, @CMS_Module, 'A', 'A', @CMM_CrBy, GETDATE(), @CMM_IPAddress, @CMM_CompID, @CMM_RiskCategory, @CMM_CrBy, GETDATE(), @CMM_Rate, @CMM_Act, @CMM_HSNSAC, @CMM_AudrptType);
                        SELECT @NewId;", dto, transaction);
                }

                var masterList = (await connection.QueryAsync<DropDownListData>(@"SELECT CMM_ID AS ID, CMM_Desc AS Name FROM Content_Management_Master WHERE CMM_Category = @Category AND CMM_Delflag = 'A' 
                AND CMM_CompID = @CompID ORDER BY CMM_Desc;", new { Category = dto.CMM_Category, CompID = dto.CMM_CompID }, transaction)).ToList();

                await transaction.CommitAsync();

                return ((dto.CMM_ID ?? 0), isUpdate ? "Updated successfully." : "Saved successfully.", masterList);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (0, "An error occurred while saving or updating the master data: " + ex.Message, new List<DropDownListData>());
            }
        }
    }
}
