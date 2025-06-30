using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TracePca.Data;
using TracePca.Dto;
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

        public async Task<(int Id, string Message)> SaveOrUpdateAuditFrequencyDataAsync(ContentManagementMasterDTO dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                bool isUpdate = (dto.CMM_ID ?? 0) > 0;

                var duplicateCount = await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1) FROM Content_Management_Master WHERE CMM_Desc = @CMM_Desc AND CMM_Category = 'FRE' AND CMM_CompID = @CMM_CompID AND (CMM_ID <> @CMM_ID);",
                    new
                    {
                        dto.CMM_Desc,
                        dto.CMM_CompID,
                        dto.CMM_ID
                    }, transaction);

                if (duplicateCount > 0)
                {
                    await transaction.RollbackAsync();
                    return (0, "A record with the same description already exists.");
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
                    dto.CMM_Code = $"FRE_{maxId}";
                    dto.CMM_ID = await connection.ExecuteScalarAsync<int>(
                        @"DECLARE @NewId INT = ISNULL((SELECT MAX(CMM_ID) FROM Content_Management_Master), 0) + 1;
                        INSERT INTO Content_Management_Master (CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMS_Remarks, CMS_KeyComponent, CMS_Module, CMM_Delflag, CMM_Status, CMM_ApprovedBy, CMM_ApprovedOn, CMM_IPAddress, CMM_CompID, CMM_RiskCategory, CMM_CrBy, CMM_CrOn, CMM_Rate, CMM_Act, CMM_HSNSAC)
                        VALUES (@NewId, @CMM_Code, @CMM_Desc, 'FRE', @CMS_Remarks, 0, 'A', 'A', 'A', @CMM_CrBy, GETDATE(), @CMM_IPAddress, @CMM_CompID, 0, @CMM_CrBy, GETDATE(), 0.00, '', '');
                        SELECT @NewId;", dto, transaction);
                }

                await transaction.CommitAsync();
                return ((dto.CMM_ID ?? 0), isUpdate ? "Updated successfully." : "Saved successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (0, "An error occurred while saving or updating the master data: " + ex.Message);
            }
        }
        public async Task<(int Id, string Message)> SaveOrUpdateEngagementFeesDataAsync(ContentManagementMasterDTO dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                bool isUpdate = (dto.CMM_ID ?? 0) > 0;

                var duplicateCount = await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1) FROM Content_Management_Master WHERE CMM_Desc = @CMM_Desc AND CMM_Category = 'OE' AND CMM_CompID = @CMM_CompID AND (CMM_ID <> @CMM_ID);",
                    new
                    {
                        dto.CMM_Desc,
                        dto.CMM_CompID,
                        dto.CMM_ID
                    }, transaction);

                if (duplicateCount > 0)
                {
                    await transaction.RollbackAsync();
                    return (0, "A record with the same description already exists.");
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
                    dto.CMM_Code = $"OE_{maxId}";
                    dto.CMM_ID = await connection.ExecuteScalarAsync<int>(
                        @"DECLARE @NewId INT = ISNULL((SELECT MAX(CMM_ID) FROM Content_Management_Master), 0) + 1;
                        INSERT INTO Content_Management_Master (CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMS_Remarks, CMS_KeyComponent, CMS_Module, CMM_Delflag, CMM_Status, CMM_ApprovedBy, CMM_ApprovedOn, CMM_IPAddress, CMM_CompID, CMM_RiskCategory, CMM_CrBy, CMM_CrOn, CMM_Rate, CMM_Act, CMM_HSNSAC)
                        VALUES (@NewId, @CMM_Code, @CMM_Desc, 'OE', @CMS_Remarks, 0, 'A', 'A', 'A', @CMM_CrBy, GETDATE(), @CMM_IPAddress, @CMM_CompID, 0, @CMM_CrBy, GETDATE(), 0.00, '', '');
                        SELECT @NewId;", dto, transaction);
                }

                await transaction.CommitAsync();
                return ((dto.CMM_ID ?? 0), isUpdate ? "Updated successfully." : "Saved successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (0, "An error occurred while saving or updating the master data: " + ex.Message);
            }
        }
        public async Task<(int Id, string Message)> SaveOrUpdateTypeofReportDataAsync(ContentManagementMasterDTO dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                bool isUpdate = (dto.CMM_ID ?? 0) > 0;

                var duplicateCount = await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1) FROM Content_Management_Master WHERE CMM_Desc = @CMM_Desc AND CMM_Category = 'TOR' AND CMM_CompID = @CMM_CompID AND (CMM_ID <> @CMM_ID);",
                    new
                    {
                        dto.CMM_Desc,
                        dto.CMM_CompID,
                        dto.CMM_ID
                    }, transaction);

                if (duplicateCount > 0)
                {
                    await transaction.RollbackAsync();
                    return (0, "A record with the same description already exists.");
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
                    dto.CMM_Code = $"TOR_{maxId}";
                    dto.CMM_ID = await connection.ExecuteScalarAsync<int>(
                        @"DECLARE @NewId INT = ISNULL((SELECT MAX(CMM_ID) FROM Content_Management_Master), 0) + 1;
                        INSERT INTO Content_Management_Master (CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMS_Remarks, CMS_KeyComponent, CMS_Module, CMM_Delflag, CMM_Status, CMM_ApprovedBy, CMM_ApprovedOn, CMM_IPAddress, CMM_CompID, CMM_RiskCategory, CMM_CrBy, CMM_CrOn, CMM_Rate, CMM_Act, CMM_HSNSAC)
                        VALUES (@NewId, @CMM_Code, @CMM_Desc, 'TOR', @CMS_Remarks, 0, 'A', 'A', 'A', @CMM_CrBy, GETDATE(), @CMM_IPAddress, @CMM_CompID, 0, @CMM_CrBy, GETDATE(), 0.00, '', '');
                        SELECT @NewId;", dto, transaction);
                }

                await transaction.CommitAsync();
                return ((dto.CMM_ID ?? 0), isUpdate ? "Updated successfully." : "Saved successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (0, "An error occurred while saving or updating the master data: " + ex.Message);
            }
        }
        public async Task<(int Id, string Message)> SaveOrUpdateTypeofTestDataAsync(ContentManagementMasterDTO dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                bool isUpdate = (dto.CMM_ID ?? 0) > 0;

                var duplicateCount = await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1) FROM Content_Management_Master WHERE CMM_Desc = @CMM_Desc AND CMM_Category = 'TOT' AND CMM_CompID = @CMM_CompID AND (CMM_ID <> @CMM_ID);",
                    new
                    {
                        dto.CMM_Desc,
                        dto.CMM_CompID,
                        dto.CMM_ID
                    }, transaction);

                if (duplicateCount > 0)
                {
                    await transaction.RollbackAsync();
                    return (0, "A record with the same description already exists.");
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
                    dto.CMM_Code = $"TOT_{maxId}";
                    dto.CMM_ID = await connection.ExecuteScalarAsync<int>(
                        @"DECLARE @NewId INT = ISNULL((SELECT MAX(CMM_ID) FROM Content_Management_Master), 0) + 1;
                        INSERT INTO Content_Management_Master (CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMS_Remarks, CMS_KeyComponent, CMS_Module, CMM_Delflag, CMM_Status, CMM_ApprovedBy, CMM_ApprovedOn, CMM_IPAddress, CMM_CompID, CMM_RiskCategory, CMM_CrBy, CMM_CrOn, CMM_Rate, CMM_Act, CMM_HSNSAC)
                        VALUES (@NewId, @CMM_Code, @CMM_Desc, 'TOT', @CMS_Remarks, 0, 'A', 'A', 'A', @CMM_CrBy, GETDATE(), @CMM_IPAddress, @CMM_CompID, 0, @CMM_CrBy, GETDATE(), 0.00, '', '');
                        SELECT @NewId;", dto, transaction);
                }

                await transaction.CommitAsync();
                return ((dto.CMM_ID ?? 0), isUpdate ? "Updated successfully." : "Saved successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (0, "An error occurred while saving or updating the master data: " + ex.Message);
            }
        }
        public async Task<(int Id, string Message)> SaveOrUpdateManagementRepresentationsDataAsync(ContentManagementMasterDTO dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                bool isUpdate = (dto.CMM_ID ?? 0) > 0;

                var duplicateCount = await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1) FROM Content_Management_Master WHERE CMM_Desc = @CMM_Desc AND CMM_Category = 'MR' AND CMM_CompID = @CMM_CompID AND (CMM_ID <> @CMM_ID);",
                    new
                    {
                        dto.CMM_Desc,
                        dto.CMM_CompID,
                        dto.CMM_ID
                    }, transaction);

                if (duplicateCount > 0)
                {
                    await transaction.RollbackAsync();
                    return (0, "A record with the same description already exists.");
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
                    dto.CMM_Code = $"MR_{maxId}";
                    dto.CMM_ID = await connection.ExecuteScalarAsync<int>(
                        @"DECLARE @NewId INT = ISNULL((SELECT MAX(CMM_ID) FROM Content_Management_Master), 0) + 1;
                        INSERT INTO Content_Management_Master (CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMS_Remarks, CMS_KeyComponent, CMS_Module, CMM_Delflag, CMM_Status, CMM_ApprovedBy, CMM_ApprovedOn, CMM_IPAddress, CMM_CompID, CMM_RiskCategory, CMM_CrBy, CMM_CrOn, CMM_Rate, CMM_Act, CMM_HSNSAC)
                        VALUES (@NewId, @CMM_Code, @CMM_Desc, 'MR', @CMS_Remarks, 0, 'A', 'A', 'A', @CMM_CrBy, GETDATE(), @CMM_IPAddress, @CMM_CompID, 0, @CMM_CrBy, GETDATE(), 0.00, '', '');
                        SELECT @NewId;", dto, transaction);
                }

                await transaction.CommitAsync();
                return ((dto.CMM_ID ?? 0), isUpdate ? "Updated successfully." : "Saved successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (0, "An error occurred while saving or updating the master data: " + ex.Message);
            }
        }
        public async Task<(int Id, string Message)> SaveOrUpdateAuditTaskOrAssignmentTaskDataAsync(ContentManagementMasterDTO dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                bool isUpdate = (dto.CMM_ID ?? 0) > 0;

                var duplicateCount = await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1) FROM Content_Management_Master WHERE CMM_Desc = @CMM_Desc AND CMM_Category = 'AT' AND CMM_CompID = @CMM_CompID AND (CMM_ID <> @CMM_ID);",
                    new
                    {
                        dto.CMM_Desc,
                        dto.CMM_CompID,
                        dto.CMM_ID
                    }, transaction);

                if (duplicateCount > 0)
                {
                    await transaction.RollbackAsync();
                    return (0, "A record with the same description already exists.");
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
                    dto.CMM_Code = $"AT_{maxId}";
                    dto.CMM_ID = await connection.ExecuteScalarAsync<int>(
                        @"DECLARE @NewId INT = ISNULL((SELECT MAX(CMM_ID) FROM Content_Management_Master), 0) + 1;
                        INSERT INTO Content_Management_Master (CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMS_Remarks, CMS_KeyComponent, CMS_Module, CMM_Delflag, CMM_Status, CMM_ApprovedBy, CMM_ApprovedOn, CMM_IPAddress, CMM_CompID, CMM_RiskCategory, CMM_CrBy, CMM_CrOn, CMM_Rate, CMM_Act, CMM_HSNSAC)
                        VALUES (@NewId, @CMM_Code, @CMM_Desc, 'AT', @CMS_Remarks, @CMS_KeyComponent, 'A', 'A', 'A', @CMM_CrBy, GETDATE(), @CMM_IPAddress, @CMM_CompID, 0, @CMM_CrBy, GETDATE(), 0.00, @CMM_Act, '');
                        SELECT @NewId;", dto, transaction);
                }

                await transaction.CommitAsync();
                return ((dto.CMM_ID ?? 0), isUpdate ? "Updated successfully." : "Saved successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (0, "An error occurred while saving or updating the master data: " + ex.Message);
            }
        }
        public async Task<(int Id, string Message)> SaveOrUpdateWorkpaperChecklistMasterDataAsync(ContentManagementMasterDTO dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                bool isUpdate = (dto.CMM_ID ?? 0) > 0;

                var duplicateCount = await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1) FROM Content_Management_Master WHERE CMM_Desc = @CMM_Desc AND CMM_Category = 'WCM' AND CMM_CompID = @CMM_CompID AND (CMM_ID <> @CMM_ID);",
                    new
                    {
                        dto.CMM_Desc,
                        dto.CMM_CompID,
                        dto.CMM_ID
                    }, transaction);

                if (duplicateCount > 0)
                {
                    await transaction.RollbackAsync();
                    return (0, "A record with the same description already exists.");
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
                    dto.CMM_Code = $"WCM_{maxId}";
                    dto.CMM_ID = await connection.ExecuteScalarAsync<int>(
                        @"DECLARE @NewId INT = ISNULL((SELECT MAX(CMM_ID) FROM Content_Management_Master), 0) + 1;
                        INSERT INTO Content_Management_Master (CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMS_Remarks, CMS_KeyComponent, CMS_Module, CMM_Delflag, CMM_Status, CMM_ApprovedBy, CMM_ApprovedOn, CMM_IPAddress, CMM_CompID, CMM_RiskCategory, CMM_CrBy, CMM_CrOn, CMM_Rate, CMM_Act, CMM_HSNSAC)
                        VALUES (@NewId, @CMM_Code, @CMM_Desc, 'WCM', @CMS_Remarks, 0, 'A', 'A', 'A', @CMM_CrBy, GETDATE(), @CMM_IPAddress, @CMM_CompID, 0, @CMM_CrBy, GETDATE(), 0.00, '', '');
                        SELECT @NewId;", dto, transaction);
                }

                await transaction.CommitAsync();
                return ((dto.CMM_ID ?? 0), isUpdate ? "Updated successfully." : "Saved successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (0, "An error occurred while saving or updating the master data: " + ex.Message);
            }
        }
        public async Task<(int Id, string Message)> SaveOrUpdateAuditCompletionCheckPointsDataAsync(ContentManagementMasterDTO dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                bool isUpdate = (dto.CMM_ID ?? 0) > 0;

                var duplicateCount = await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1) FROM Content_Management_Master WHERE CMM_Desc = @CMM_Desc AND CMM_Category = 'ASF' AND CMM_CompID = @CMM_CompID AND (CMM_ID <> @CMM_ID);",
                    new
                    {
                        dto.CMM_Desc,
                        dto.CMM_CompID,
                        dto.CMM_ID
                    }, transaction);

                if (duplicateCount > 0)
                {
                    await transaction.RollbackAsync();
                    return (0, "A record with the same description already exists.");
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
                    dto.CMM_Code = $"ASF_{maxId}";
                    dto.CMM_ID = await connection.ExecuteScalarAsync<int>(
                        @"DECLARE @NewId INT = ISNULL((SELECT MAX(CMM_ID) FROM Content_Management_Master), 0) + 1;
                        INSERT INTO Content_Management_Master (CMM_ID, CMM_Code, CMM_Desc, CMM_Category, CMS_Remarks, CMS_KeyComponent, CMS_Module, CMM_Delflag, CMM_Status, CMM_ApprovedBy, CMM_ApprovedOn, CMM_IPAddress, CMM_CompID, CMM_RiskCategory, CMM_CrBy, CMM_CrOn, CMM_Rate, CMM_Act, CMM_HSNSAC)
                        VALUES (@NewId, @CMM_Code, @CMM_Desc, 'ASF', @CMS_Remarks, 0, 'A', 'A', 'A', @CMM_CrBy, GETDATE(), @CMM_IPAddress, @CMM_CompID, 0, @CMM_CrBy, GETDATE(), 0.00, '', '');
                        SELECT @NewId;", dto, transaction);
                }

                await transaction.CommitAsync();
                return ((dto.CMM_ID ?? 0), isUpdate ? "Updated successfully." : "Saved successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (0, "An error occurred while saving or updating the master data: " + ex.Message);
            }
        }
    }
}
