using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Dto.Audit;
using TracePca.Interface.Audit;

namespace TracePca.Service.Audit
{
    public class AuditCompletionService : AuditCompletionInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public AuditCompletionService(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }

        public async Task<AuditDropDownListDataDTO> LoadAllAuditDDLDataAsync(int compId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var parameters = new { CompId = compId };

                var currentYear = connection.QueryFirstOrDefaultAsync<DropDownListData>(@"SELECT YMS_YEARID AS ID, YMS_ID AS Name FROM YEAR_MASTER
                WHERE YMS_Default = 1 AND YMS_CompId = @CompId", parameters);

                var customerList = connection.QueryAsync<DropDownListData>(@"SELECT CUST_ID AS ID, CUST_NAME AS Name FROM SAD_CUSTOMER_MASTER
                WHERE CUST_DELFLG = 'A' AND CUST_CompID = @CompId ORDER BY CUST_NAME ASC", parameters);

                var auditCompletionCheckPointList = connection.QueryAsync<DropDownListData>(@"SELECT cmm_ID AS ID, cmm_Desc AS Name FROM Content_Management_Master
                WHERE CMM_Category = 'ASF' AND CMM_Delflag = 'A' AND CMM_CompID = @CompId ORDER BY cmm_Desc ASC", parameters);

                var signedByList = connection.QueryAsync<DropDownListData>(@"SELECT Usr_ID AS ID, USr_FullName AS Name from sad_userdetails 
                WHERE usr_compID = @CompId And USR_Partner = 1 And(usr_DelFlag = 'A' or usr_DelFlag = 'B' or usr_DelFlag = 'L') order by USr_FullName ASC", parameters);

                await Task.WhenAll(currentYear, customerList, auditCompletionCheckPointList);

                return new AuditDropDownListDataDTO
                {
                    CurrentYear = await currentYear,
                    CustomerList = customerList.Result.ToList(),
                    AuditCompletionCheckPointList = auditCompletionCheckPointList.Result.ToList(),
                    SignedByList = signedByList.Result.ToList()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading all DDL data", ex);
            }
        }

        public async Task<IEnumerable<ReportTypeDetails>> GetReportTypeDetails(int compId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var contentIds = await connection.QueryAsync<int>(@"SELECT TEM_ContentId FROM SAD_Finalisation_Report_Template WHERE TEM_FunctionId = @ReportTypeId And TEM_Delflag = 'A' And TEM_CompID = @CompId",
                    new { CompId = compId, ReportTypeId = 16 });

                string query;
                object parameters;
                if (contentIds.Count() == 1)
                {
                    query = @"SELECT RCM_Id, RCM_ReportName, RCM_Heading, RCM_Description FROM SAD_ReportContentMaster WHERE RCM_Id IN (@ContentId) AND RCM_ReportId = @ReportTypeId AND RCM_CompID = @CompId AND RCM_Delflag = 'A' ORDER BY RCM_Id";
                    parameters = new { ContentId = contentIds.First(), CompId = compId, ReportTypeId = 16 };
                }
                else
                {
                    query = @"SELECT RCM_Id, RCM_ReportName, RCM_Heading, RCM_Description FROM SAD_ReportContentMaster WHERE RCM_ReportId = @ReportTypeId AND RCM_CompID = @CompId AND RCM_Delflag = 'A' ORDER BY RCM_Id";
                    parameters = new { CompId = compId, ReportTypeId = 16 };
                }

                var result = await connection.QueryAsync<ReportTypeDetails>(query, parameters);
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting report type details", ex);
            }
        }

        public async Task<AuditDropDownListDataDTO> LoadAuditNoDDLAsync(int compId, int yearId, int custId, int userId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var sql = @"SELECT SA.SA_ID As ID, SA.SA_AuditNo + ' - ' + CMM.CMM_Desc AS Name FROM StandardAudit_Schedule SA
                LEFT JOIN Content_Management_Master CMM ON CMM.CMM_ID = SA.SA_AuditTypeID WHERE SA.SA_CompID = @CompId AND SA.SA_YearID = @YearId";
                if (custId > 0)
                {
                    sql += " AND SA.SA_CustID = @CustId ";
                }
                sql += @"AND (EXISTS (SELECT 1 FROM sad_userdetails WHERE usr_CompID = @CompId AND usr_ID = @UserId AND usr_Partner = 1 AND usr_DelFlag IN ('A','B','L'))
                OR (',' + ISNULL(SA.SA_AdditionalSupportEmployeeID, '') + ',' LIKE '%,' + CAST(@UserId AS VARCHAR) + ',%' OR ',' + ISNULL(SA.SA_EngagementPartnerID, '') + ',' LIKE '%,' + CAST(@UserId AS VARCHAR) + ',%' OR
                    ',' + ISNULL(SA.SA_ReviewPartnerID, '') + ',' LIKE '%,' + CAST(@UserId AS VARCHAR) + ',%' OR ',' + ISNULL(SA.SA_PartnerID, '') + ',' LIKE '%,' + CAST(@UserId AS VARCHAR) + ',%')) ORDER BY SA.SA_ID DESC";

                var parameters = new
                {
                    CompId = compId,
                    YearId = yearId,
                    CustId = custId,
                    UserId = userId
                };

                var auditNoList = await connection.QueryAsync<DropDownListData>(sql, parameters);
                return new AuditDropDownListDataDTO
                {
                    ExistingAuditNoList = auditNoList.ToList()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading audit no DDL data", ex);
            }
        }

        public async Task<AuditDropDownListDataDTO> LoadAuditWorkPaperDDLAsync(int compId, int auditId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var auditWorkPaper = await connection.QueryAsync<DropDownListData>(@"Select SSW_ID As ID, SSW_WorkpaperRef As Name From StandardAudit_ScheduleConduct_WorkPaper Where SSW_SA_ID = @AuditId And SSW_CompID = @CompId", new { CompId = compId, AuditId = auditId });

                return new AuditDropDownListDataDTO
                {
                    AuditWorkpaperList = auditWorkPaper.ToList()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting audit workpaper details", ex);
            }
        }

        public async Task<List<AuditCompletionSubPointDetailsDTO>> GetAuditCompletionSubPointDetailsAsync(int compId, int auditId, int checkPointId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var query = @"SELECT ISNULL(SAC_ID, 0) AS SAC_ID, ASM_ID AS SAC_SubPointId, ASM_CheckpointID AS SAC_CheckPointId, CMM.cmm_Desc AS SAC_CheckPointName, 
                ASM_SubPoint AS SAC_SubPointName, SAC_Remarks AS SAC_Remarks, ISNULL(SAC_WorkPaperId, 0) AS SAC_WorkPaperId, WP.SSW_WorkpaperRef AS SAC_WorkPaperName, ISNULL(SAC_AttachmentId, 0) AS SAC_AttachmentId 
                FROM AuditCompletion_SubPoint_Master ASM
                LEFT JOIN StandardAudit_Audit_Completion SAC ON SAC.SAC_CheckPointId = ASM.ASM_CheckpointID AND SAC.SAC_SubPointId = ASM.ASM_ID AND SAC.SAC_AuditID = @AuditID
                LEFT JOIN Content_Management_Master CMM ON CMM.cmm_ID = ASM.ASM_CheckpointID AND CMM.CMM_Category = 'ASF' AND CMM.CMM_CompID = @CompId 
                LEFT JOIN StandardAudit_ScheduleConduct_WorkPaper WP ON WP.SSW_ID = SAC.SAC_WorkPaperId AND WP.SSW_CompID = @CompId
                WHERE ASM.ASM_CheckpointID = @CheckPointId And ASM.ASM_CompId = @CompId";

                var subPointDetails = await connection.QueryAsync<AuditCompletionSubPointDetailsDTO>(query, new { CompId = compId, AuditID = auditId, CheckPointId = checkPointId });
                return subPointDetails.ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting the audit completion subpoints by ID", ex);
            }
        }

        public async Task<AuditCompletionDTO> GetAuditCompletionDetailsByIdAsync(int compId, int auditId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var dto = await connection.QueryFirstOrDefaultAsync<AuditCompletionDTO>(
                    @"SELECT SAC_CustID, SAC_YearID, SAC_AuditID, SAC_CreatedBy, SAC_CreatedOn, SAC_UpdatedBy, SAC_UpdatedOn, SAC_CompID, SAC_IPAddress FROM StandardAudit_Audit_Completion
                WHERE SAC_AuditID = @AuditId AND SAC_CompID = @CompId;",
                    new { CompId = compId, AuditId = auditId });

                if (dto == null)
                    return new AuditCompletionDTO();

                var templateDetails = await connection.QueryAsync<AuditCompletionTemplateDetailsDTO>(
                    @"SELECT LTD_ID, LTD_LOE_ID, LTD_ReportTypeID, LTD_HeadingID, LTD_Heading, LTD_Decription, LTD_FormName, LTD_CrBy, LTD_CrOn, LTD_IPAddress, LTD_CompID 
                FROM LOE_Template_Details WHERE LTD_FormName = 'AC' And LTD_LOE_ID = @LOEId And LTD_CompID = @CompId;",
                    new { CompId = compId, LOEId = auditId });

                dto.AuditCompletionTemplateDetails = templateDetails.ToList();
                return dto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting audit completion by ID", ex);
            }
        }

        public async Task<int> SaveOrUpdateAuditCompletionDataAsync(AuditCompletionDTO dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                bool anyExisting = false;
                foreach (var sub in dto.AuditCompletionSubPointDetails)
                {
                    var existing = await connection.ExecuteScalarAsync<int>(@"SELECT COUNT(*) FROM StandardAudit_Audit_Completion 
                        WHERE SAC_AuditID = @SAC_AuditID AND SAC_CheckPointId = @SAC_CheckPointId AND SAC_SubPointId = @SAC_SubPointId AND SAC_CompID = @SAC_CompID;",
                        new
                        {
                            SAC_AuditID = dto.SAC_AuditID,
                            sub.SAC_CheckPointId,
                            sub.SAC_SubPointId,
                            SAC_CompID = dto.SAC_CompID
                        }, transaction);

                    if (existing > 0)
                    {
                        anyExisting = true;
                        await connection.ExecuteAsync(
                            @"UPDATE StandardAudit_Audit_Completion SET SAC_Remarks = @SAC_Remarks, SAC_WorkPaperId = @SAC_WorkPaperId, SAC_AttachmentId = @SAC_AttachmentId 
                                WHERE SAC_AuditID = @SAC_AuditID AND SAC_CheckPointId = @SAC_CheckPointId AND SAC_SubPointId = @SAC_SubPointId And SAC_UpdatedBy = @SAC_UpdatedBy 
                                And SAC_UpdatedOn = GetDate() AND SAC_CompID = @SAC_CompID;",
                            new
                            {
                                SAC_AuditID = dto.SAC_AuditID,
                                sub.SAC_CheckPointId,
                                sub.SAC_SubPointId,
                                sub.SAC_Remarks,
                                sub.SAC_WorkPaperId,
                                SAC_UpdatedBy = dto.SAC_UpdatedBy,
                                sub.SAC_AttachmentId,
                                SAC_CompID = dto.SAC_CompID
                            }, transaction);
                    }
                    else
                    {
                        sub.SAC_ID = await connection.ExecuteScalarAsync<int>(
                            @"DECLARE @NewSubId INT; SELECT @NewSubId = ISNULL(MAX(SAC_ID), 0) + 1 FROM StandardAudit_Audit_Completion;
                              INSERT INTO StandardAudit_Audit_Completion 
                              (SAC_ID, SAC_AuditID, SAC_CheckPointId, SAC_SubPointId, SAC_Remarks, SAC_WorkPaperId, SAC_AttachmentId, SAC_CreatedBy, SAC_CreatedOn, SAC_IPAddress, SAC_CompID)
                              VALUES (@NewSubId, @SAC_AuditID, @SAC_CheckPointId, @SAC_SubPointId, @SAC_Remarks, @SAC_WorkPaperId, @SAC_AttachmentId, @SAC_CreatedBy, GetDate(), @SAC_IPAddress, @SAC_CompID);
                              SELECT @NewSubId;",
                            new
                            {
                                SAC_AuditID = dto.SAC_AuditID,
                                sub.SAC_CheckPointId,
                                sub.SAC_SubPointId,
                                sub.SAC_Remarks,
                                sub.SAC_WorkPaperId,
                                sub.SAC_AttachmentId,
                                SAC_CreatedBy = dto.SAC_CreatedBy,
                                SAC_IPAddress = dto.SAC_IPAddress,
                                SAC_CompID = dto.SAC_CompID
                            }, transaction);
                    }
                }

                await connection.ExecuteAsync("DELETE FROM LOE_Template_Details WHERE LTD_FormName = 'AC' And LTD_LOE_ID = @LOE_Id;", new { LOE_Id = dto.SAC_AuditID }, transaction);
                foreach (var item in dto.AuditCompletionTemplateDetails)
                {
                    item.LTD_LOE_ID = dto.SAC_AuditID;
                    await connection.ExecuteAsync(
                        @"DECLARE @NewTemplateDetailId INT; SELECT @NewTemplateDetailId = ISNULL(MAX(LTD_ID), 0) + 1 FROM LOE_Template_Details;
                          INSERT INTO LOE_Template_Details (LTD_ID, LTD_LOE_ID, LTD_ReportTypeID, LTD_HeadingID, LTD_Heading, LTD_Decription, LTD_FormName, LTD_CrBy, LTD_CrOn, LTD_IPAddress, LTD_CompID)
                          VALUES (@NewTemplateDetailId, @LTD_LOE_ID, @LTD_ReportTypeID, @LTD_HeadingID, @LTD_Heading, @LTD_Decription, @LTD_FormName, @LTD_CrBy, GETDATE(), @LTD_IPAddress, @LTD_CompID);",
                        item, transaction);
                }

                await transaction.CommitAsync();
                return anyExisting ? 1 : 0;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while saving or updating the audit completion data.", ex);
            }
        }

        public async Task<int> UpdateSignedByUDINInAuditAsync(AuditSignedByUDINRequestDTO dto)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var query = @"UPDATE StandardAudit_Schedule SET SA_Status = 10, SA_SignedBy = @SignedBy, SA_UDIN = @UDIN, SA_UDINdate = @UDINDate WHERE SA_ID = @AuditID AND SA_CompID = @ACID";

                var parameters = new { SignedBy = dto.SA_SignedBy, UDIN = dto.SA_UDIN, UDINDate = dto.SA_UDINdate, AuditID = dto.SA_ID, ACID = dto.SA_CompID };
                var rowsAffected = await connection.ExecuteAsync(query, parameters);

                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating the SignedBy and UDIN in the audit.", ex);
            }
        }
    }
}
