using Dapper;
using DocumentFormat.OpenXml.Office.CoverPageProps;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Layout.Element;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using OfficeOpenXml.Table.PivotTable;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StackExchange.Redis;
using System.Data;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TracePca.Data;
using TracePca.Dto.Audit;
using TracePca.Interface.Audit;
using TracePca.Service.DigitalFilling;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Body = DocumentFormat.OpenXml.Wordprocessing.Body;
using Bold = DocumentFormat.OpenXml.Wordprocessing.Bold;
using Colors = QuestPDF.Helpers.Colors;
using Document = DocumentFormat.OpenXml.Wordprocessing.Document;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
using WordDoc = DocumentFormat.OpenXml.Wordprocessing.Document;
using WordprocessingDocument = DocumentFormat.OpenXml.Packaging.WordprocessingDocument;

namespace TracePca.Service.Audit
{
    public class AuditCompletionService : AuditCompletionInterface
    {
        private readonly IConfiguration _configuration;
        private readonly EngagementPlanInterface _engagementPlanInterface;
        private readonly AuditSummaryInterface _auditSummaryInterface;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;

        public AuditCompletionService(IConfiguration configuration, EngagementPlanInterface engagementPlanInterface, AuditSummaryInterface auditSummaryInterface, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _engagementPlanInterface = engagementPlanInterface ?? throw new ArgumentNullException(nameof(engagementPlanInterface));
            _auditSummaryInterface = auditSummaryInterface ?? throw new ArgumentNullException(nameof(auditSummaryInterface));
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

        public async Task<AuditDropDownListDataDTO> LoadAllAuditDDLDataAsync(int compId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var parameters = new { CompId = compId };

                var currentYear = connection.QueryFirstOrDefaultAsync<DropDownListData>(@"SELECT YMS_YEARID AS ID, YMS_ID AS Name FROM YEAR_MASTER
                WHERE YMS_Default = 1 AND YMS_CompId = @CompId", parameters);

                var customerList = connection.QueryAsync<DropDownListData>(@"SELECT CUST_ID AS ID, CUST_NAME AS Name FROM SAD_CUSTOMER_MASTER
                WHERE CUST_DELFLG = 'A' AND CUST_CompID = @CompId ORDER BY CUST_NAME ASC", parameters);

                var auditCompletionCheckPointList = connection.QueryAsync<DropDownListData>(@"SELECT cmm_ID AS ID, cmm_Desc AS Name FROM Content_Management_Master
                WHERE CMM_Category = 'ASF' AND CMS_Keycomponent = 1 AND CMM_Delflag = 'A' AND CMM_CompID = @CompId ORDER BY cmm_Desc ASC", parameters);

                var auditClosureCheckPointList = connection.QueryAsync<DropDownListData>(@"SELECT cmm_ID AS ID, cmm_Desc AS Name FROM Content_Management_Master
                WHERE CMM_Category = 'ASF' AND CMS_Keycomponent = 0 AND CMM_Delflag = 'A' AND CMM_CompID = @CompId ORDER BY cmm_Desc ASC", parameters);

                var signedByList = connection.QueryAsync<DropDownListData>(@"SELECT Usr_ID AS ID, USr_FullName AS Name FROM sad_userdetails
                WHERE usr_compID = @CompId AND Usr_Role IN (SELECT Mas_ID FROM SAD_GrpOrLvl_General_Master WHERE Mas_Delflag = 'A'
                AND Mas_CompID = @CompId AND Mas_Description = 'Partner') AND usr_DelFlag IN ('A', 'B', 'L') ORDER BY USr_FullName", parameters);

                await Task.WhenAll(currentYear, customerList, auditCompletionCheckPointList);

                return new AuditDropDownListDataDTO
                {
                    CurrentYear = await currentYear,
                    CustomerList = customerList.Result.ToList(),
                    AuditCompletionCheckPointList = auditCompletionCheckPointList.Result.ToList(),
                    AuditClosureCheckPointList = auditClosureCheckPointList.Result.ToList(),
                    SignedByList = signedByList.Result.ToList()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading all DDL data", ex);
            }
        }

        public async Task<IEnumerable<ReportTypeDetailsDTO>> GetReportTypeDetails(int compId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
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

                var result = await connection.QueryAsync<ReportTypeDetailsDTO>(query, parameters);
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting report type details", ex);
            }
        }

        public async Task<IEnumerable<ReportTypeDetailsDTO>> GetReportTypeDetailsByAuditId(int compId, int auditId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var frameworkId = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT SA_AuditFrameworkId FROM StandardAudit_Schedule WHERE SA_ID = @AuditId AND SA_CompID = @CompId", new { AuditId = auditId, CompId = compId });

                int reportTypeId = (frameworkId == 1) ? 16 : 34;

                var contentIds = await connection.QueryAsync<int>(@"SELECT TEM_ContentId FROM SAD_Finalisation_Report_Template WHERE TEM_FunctionId = @ReportTypeId AND TEM_Delflag = 'A' AND TEM_CompID = @CompId",
                    new { CompId = compId, ReportTypeId = reportTypeId });

                string query;
                object parameters;
                if (contentIds.Count() == 1)
                {
                    query = @"SELECT RCM_Id, RCM_ReportName, RCM_Heading, RCM_Description FROM SAD_ReportContentMaster WHERE RCM_Id = @ContentId AND RCM_ReportId = @ReportTypeId AND RCM_CompID = @CompId AND RCM_Delflag = 'A' ORDER BY RCM_Id";
                    parameters = new { ContentId = contentIds.First(), CompId = compId, ReportTypeId = reportTypeId };
                }
                else
                {
                    query = @"SELECT RCM_Id, RCM_ReportName, RCM_Heading, RCM_Description FROM SAD_ReportContentMaster WHERE RCM_ReportId = @ReportTypeId AND RCM_CompID = @CompId AND RCM_Delflag = 'A' ORDER BY RCM_Id";
                    parameters = new { CompId = compId, ReportTypeId = reportTypeId };
                }

                var result = await connection.QueryAsync<ReportTypeDetailsDTO>(query, parameters);
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
				using var connection = new SqlConnection(_connectionString);
				await connection.OpenAsync();

				var sql = @"SELECT SA.SA_ID AS ID, SA.SA_AuditNo + ' - ' + CMM.CMM_Desc AS Name,
                    CASE WHEN ',' + ISNULL(SA.SA_PartnerID, '') + ',' LIKE '%,' + CAST(@UserId AS VARCHAR) + ',%' OR ',' + ISNULL(SA.SA_EngagementPartnerID, '') + ',' LIKE '%,' + CAST(@UserId AS VARCHAR) + ',%' THEN 1 ELSE 0 END AS isPartner,
                    CASE WHEN ',' + ISNULL(SA.SA_ReviewPartnerID, '') + ',' LIKE '%,' + CAST(@UserId AS VARCHAR) + ',%' THEN 1 ELSE 0 END AS isReviewer,
                    CASE WHEN ',' + ISNULL(SA.SA_AdditionalSupportEmployeeID, '') + ',' LIKE '%,' + CAST(@UserId AS VARCHAR) + ',%' THEN 1 ELSE 0 END AS isAuditor, 
                    CASE WHEN ISNULL(SA.SA_IsArchived, 0) = 1 THEN 1 ELSE 0 END AS IsArchived,
                    SA_Status As Status, SA_AuditFrameworkId As AuditFrameworkId
                    FROM StandardAudit_Schedule SA LEFT JOIN Content_Management_Master CMM ON CMM.CMM_ID = SA.SA_AuditTypeID
                    WHERE SA.SA_CompID = @CompId AND SA.SA_YearID = @YearId ";

				if (custId > 0) { sql += " AND SA.SA_CustID = @CustId "; }

				sql += @" AND (EXISTS (SELECT 1 FROM sad_userdetails WHERE usr_CompID = @CompId AND usr_ID = @UserId AND usr_Partner = 1 AND usr_DelFlag IN ('A','B','L'))
                OR (',' + ISNULL(SA.SA_AdditionalSupportEmployeeID, '') + ',' LIKE '%,' + CAST(@UserId AS VARCHAR) + ',%' OR ',' + ISNULL(SA.SA_EngagementPartnerID, '') + ',' LIKE '%,' + CAST(@UserId AS VARCHAR) + ',%' 
                OR ',' + ISNULL(SA.SA_ReviewPartnerID, '') + ',' LIKE '%,' + CAST(@UserId AS VARCHAR) + ',%' OR ',' + ISNULL(SA.SA_PartnerID, '') + ',' LIKE '%,' + CAST(@UserId AS VARCHAR) + ',%')) ORDER BY SA.SA_ID DESC;";

				var parameters = new
				{
					CompId = compId,
					YearId = yearId,
					CustId = custId,
					UserId = userId
				};

				var auditNoList = await connection.QueryAsync<AuditDropDownListData>(sql, parameters);
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
                using var connection = new SqlConnection(_connectionString);
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
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"SELECT ISNULL(SAC_ID, 0) AS SAC_ID, ASM_ID AS SAC_SubPointId, ASM_CheckpointID AS SAC_CheckPointId, CMM.cmm_Desc AS SAC_CheckPointName, 
                ASM_SubPoint AS SAC_SubPointName, SAC_Remarks AS SAC_Remarks, ISNULL(SAC_WorkPaperId, 0) AS SAC_WorkPaperId, WP.SSW_WorkpaperRef AS SAC_WorkPaperName, ISNULL(SAC_AttachmentId, 0) AS SAC_AttachmentId 
                FROM AuditCompletion_SubPoint_Master ASM
                LEFT JOIN StandardAudit_Audit_Completion SAC ON SAC.SAC_CheckPointId = ASM.ASM_CheckpointID AND SAC.SAC_SubPointId = ASM.ASM_ID AND SAC.SAC_AuditID = @AuditID
                LEFT JOIN Content_Management_Master CMM ON CMM.cmm_ID = ASM.ASM_CheckpointID AND CMM.CMM_Category = 'ASF' AND CMM.CMM_CompID = @CompId AND
                    CMS_KeyComponent IN (SELECT SA_AuditFrameworkId FROM StandardAudit_Schedule WHERE SA_ID = @AuditId AND SA_CompID = @CompId)
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

        public async Task<List<AuditCompletionSubPointDetailsDTO>> GetAuditClosureSubPointDetailsAsync(int compId, int auditId, int checkPointId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"SELECT ISNULL(SAC_ID, 0) AS SAC_ID, ASM_ID AS SAC_SubPointId, ASM_CheckpointID AS SAC_CheckPointId, CMM.cmm_Desc AS SAC_CheckPointName, 
                ASM_SubPoint AS SAC_SubPointName, SAC_Remarks AS SAC_Remarks, ISNULL(SAC_WorkPaperId, 0) AS SAC_WorkPaperId, WP.SSW_WorkpaperRef AS SAC_WorkPaperName, ISNULL(SAC_AttachmentId, 0) AS SAC_AttachmentId 
                FROM AuditCompletion_SubPoint_Master ASM
                LEFT JOIN StandardAudit_Audit_Completion SAC ON SAC.SAC_CheckPointId = ASM.ASM_CheckpointID AND SAC.SAC_SubPointId = ASM.ASM_ID AND SAC.SAC_AuditID = @AuditID
                LEFT JOIN Content_Management_Master CMM ON CMM.cmm_ID = ASM.ASM_CheckpointID AND CMM.CMM_Category = 'ACP' AND CMM.CMM_CompID = @CompId 
                LEFT JOIN StandardAudit_ScheduleConduct_WorkPaper WP ON WP.SSW_ID = SAC.SAC_WorkPaperId AND WP.SSW_CompID = @CompId
                WHERE ASM.ASM_CheckpointID = @CheckPointId And ASM.ASM_CompId = @CompId";

                var subPointDetails = await connection.QueryAsync<AuditCompletionSubPointDetailsDTO>(query, new { CompId = compId, AuditID = auditId, CheckPointId = checkPointId });
                return subPointDetails.ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting the audit closure subpoints by ID", ex);
            }
        }

        public async Task<AuditCompletionDTO> GetAuditCompletionDetailsByIdAsync(int compId, int auditId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var dto = await connection.QueryFirstOrDefaultAsync<AuditCompletionDTO>(
                    @"SELECT SAC_CustID, SAC_YearID, SAC_AuditID, SAC_CreatedBy, SAC_CreatedOn, SAC_UpdatedBy, SAC_UpdatedOn, SAC_CompID, SAC_IPAddress FROM StandardAudit_Audit_Completion WHERE SAC_AuditID = @AuditId AND SAC_CompID = @CompId;",
                    new { CompId = compId, AuditId = auditId });

                if (dto == null)
                {
                    dto = await connection.QueryFirstOrDefaultAsync<AuditCompletionDTO>(@"SELECT SA_CustID AS SAC_CustID, SA_YearID AS SAC_YearID, SA_ID AS SAC_AuditID, SA_CompID As SAC_CompID FROM StandardAudit_Schedule WHERE SA_ID = @AuditId AND SA_CompID = @CompId;",
                        new { CompId = compId, AuditId = auditId });

                    if (dto == null)
                    {
                        dto = new AuditCompletionDTO
                        {
                            SAC_AuditID = auditId,
                            SAC_CompID = compId
                        };
                    }
                }

                var templateDetails = (await connection.QueryAsync<AuditCompletionTemplateDetailsDTO>(@"SELECT LTD_ID, LTD_LOE_ID, LTD_ReportTypeID, LTD_HeadingID, LTD_Heading, LTD_Decription, 
                     LTD_FormName, LTD_CrBy, LTD_CrOn, LTD_IPAddress, LTD_CompID FROM LOE_Template_Details WHERE LTD_FormName = 'AC' AND LTD_LOE_ID = @LOEId AND LTD_CompID = @CompId;",
                    new { CompId = compId, LOEId = auditId })).ToList();

                if (!templateDetails.Any())
                {
                    var reportTypeList = await GetReportTypeDetailsByAuditId(compId, auditId);
                    foreach (var report in reportTypeList)
                    {
                        var fallbackTemplate = new AuditCompletionTemplateDetailsDTO
                        {
                            LTD_ID = 0,
                            LTD_LOE_ID = auditId,
                            LTD_ReportTypeID = 16,
                            LTD_HeadingID = report.RCM_Id,
                            LTD_Heading = report.RCM_Heading ?? "",
                            LTD_Decription = report.RCM_Description ?? "",
                            LTD_FormName = "AC",
                            LTD_CrBy = 0,
                            LTD_CrOn = DateTime.Now,
                            LTD_IPAddress = "",
                            LTD_CompID = compId,
                        };
                        templateDetails.Add(fallbackTemplate);
                    }
                }

                dto.AuditCompletionTemplateDetails = templateDetails;
                return dto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting audit completion by ID", ex);
            }
        }

        public async Task<int> SaveOrUpdateAuditCompletionDataAsync(AuditCompletionDTO dto)
        {
            using var connection = new SqlConnection(_connectionString);
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
                             @"UPDATE StandardAudit_Audit_Completion SET SAC_Remarks = @SAC_Remarks, SAC_WorkPaperId = @SAC_WorkPaperId, SAC_AttachmentId = @SAC_AttachmentId, SAC_UpdatedBy = @SAC_UpdatedBy, SAC_UpdatedOn = GetDate()
                                WHERE SAC_AuditID = @SAC_AuditID AND SAC_CheckPointId = @SAC_CheckPointId AND SAC_SubPointId = @SAC_SubPointId And SAC_CompID = @SAC_CompID;",
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
                              (SAC_ID, SAC_CustID, SAC_YearID, SAC_AuditID, SAC_CheckPointId, SAC_SubPointId, SAC_Remarks, SAC_WorkPaperId, SAC_AttachmentId, SAC_CreatedBy, SAC_CreatedOn, SAC_IPAddress, SAC_CompID)
                              VALUES (@NewSubId, @SAC_CustID, @SAC_YearID, @SAC_AuditID, @SAC_CheckPointId, @SAC_SubPointId, @SAC_Remarks, @SAC_WorkPaperId, @SAC_AttachmentId, @SAC_CreatedBy, GetDate(), @SAC_IPAddress, @SAC_CompID);
                              SELECT @NewSubId;",
                            new
                            {
                                SAC_CustID = dto.SAC_CustID,
                                SAC_YearID = dto.SAC_YearID,
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

        public async Task<int> SaveOrUpdateAuditCompletionSubPointDataAsync(AuditCompletionSingleDTO dto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                bool anyExisting = false;
                var existing = await connection.ExecuteScalarAsync<int>(@"SELECT COUNT(*) FROM StandardAudit_Audit_Completion 
                        WHERE SAC_AuditID = @SAC_AuditID AND SAC_CheckPointId = @SAC_CheckPointId AND SAC_SubPointId = @SAC_SubPointId AND SAC_CompID = @SAC_CompID;",
                        new
                        {
                            SAC_AuditID = dto.SAC_AuditID,
                            SAC_CheckPointId = dto.AuditCompletionSubPointDetails.SAC_CheckPointId,
                            SAC_SubPointId = dto.AuditCompletionSubPointDetails.SAC_SubPointId,
                            SAC_CompID = dto.SAC_CompID
                        }, transaction);

                if (existing > 0)
                {
                    anyExisting = true;
                    await connection.ExecuteAsync(
                        @"UPDATE StandardAudit_Audit_Completion SET SAC_Remarks = @SAC_Remarks, SAC_WorkPaperId = @SAC_WorkPaperId, SAC_AttachmentId = @SAC_AttachmentId, SAC_UpdatedBy = @SAC_UpdatedBy, SAC_UpdatedOn = GetDate()
                                WHERE SAC_AuditID = @SAC_AuditID AND SAC_CheckPointId = @SAC_CheckPointId AND SAC_SubPointId = @SAC_SubPointId And SAC_CompID = @SAC_CompID;",
                        new
                        {
                            SAC_AuditID = dto.SAC_AuditID,
                            SAC_CheckPointId = dto.AuditCompletionSubPointDetails.SAC_CheckPointId,
                            SAC_SubPointId= dto.AuditCompletionSubPointDetails.SAC_SubPointId,
                            SAC_Remarks = dto.AuditCompletionSubPointDetails.SAC_Remarks,
                            SAC_WorkPaperId = dto.AuditCompletionSubPointDetails.SAC_WorkPaperId,
                            SAC_UpdatedBy = dto.SAC_UpdatedBy,
                            SAC_AttachmentId = dto.AuditCompletionSubPointDetails.SAC_AttachmentId,
                            SAC_CompID = dto.SAC_CompID
                        }, transaction);
                }
                else
                {
                    dto.AuditCompletionSubPointDetails.SAC_ID = await connection.ExecuteScalarAsync<int>(
                        @"DECLARE @NewSubId INT; SELECT @NewSubId = ISNULL(MAX(SAC_ID), 0) + 1 FROM StandardAudit_Audit_Completion;
                              INSERT INTO StandardAudit_Audit_Completion 
                              (SAC_ID, SAC_CustID, SAC_YearID, SAC_AuditID, SAC_CheckPointId, SAC_SubPointId, SAC_Remarks, SAC_WorkPaperId, SAC_AttachmentId, SAC_CreatedBy, SAC_CreatedOn, SAC_IPAddress, SAC_CompID)
                              VALUES (@NewSubId, @SAC_CustID, @SAC_YearID, @SAC_AuditID, @SAC_CheckPointId, @SAC_SubPointId, @SAC_Remarks, @SAC_WorkPaperId, @SAC_AttachmentId, @SAC_CreatedBy, GetDate(), @SAC_IPAddress, @SAC_CompID);
                              SELECT @NewSubId;",
                        new
                        {
                            SAC_CustID = dto.SAC_CustID,
                            SAC_YearID = dto.SAC_YearID,
                            SAC_AuditID = dto.SAC_AuditID,
                            SAC_CheckPointId = dto.AuditCompletionSubPointDetails.SAC_CheckPointId,
                            SAC_SubPointId = dto.AuditCompletionSubPointDetails.SAC_SubPointId,
                            SAC_Remarks = dto.AuditCompletionSubPointDetails.SAC_Remarks,
                            SAC_WorkPaperId = dto.AuditCompletionSubPointDetails.SAC_WorkPaperId,
                            SAC_AttachmentId = dto.AuditCompletionSubPointDetails.SAC_AttachmentId,
                            SAC_CreatedBy = dto.SAC_CreatedBy,
                            SAC_IPAddress = dto.SAC_IPAddress,
                            SAC_CompID = dto.SAC_CompID
                        }, transaction);
                }

                await transaction.CommitAsync();
                return anyExisting ? 1 : 0;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while saving or updating the audit completion subpoint data.", ex);
            }
        }

        public async Task<int> UpdateSignedByUDINInAuditAsync(AuditSignedByUDINRequestDTO dto)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"UPDATE StandardAudit_Schedule SET SA_SignedBy = @SignedBy, SA_UDIN = @UDIN, SA_UDINdate = @UDINDate WHERE SA_ID = @AuditID AND SA_CompID = @ACID";

                var parameters = new { SignedBy = dto.SA_SignedBy, UDIN = dto.SA_UDIN, UDINDate = dto.SA_UDINdate, AuditID = dto.SA_ID, ACID = dto.SA_CompID };
                var rowsAffected = await connection.ExecuteAsync(query, parameters);

                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating the SignedBy and UDIN in the audit.", ex);
            }
        }

        public async Task<int> UpdateAuditCompletionStatusAsync(int compId, int auditId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"UPDATE StandardAudit_Schedule SET SA_Status = 10 WHERE SA_ID = @AuditID AND SA_CompID = @ACID";

                var parameters = new { AuditID = auditId, ACID = compId };
                var rowsAffected = await connection.ExecuteAsync(query, parameters);

                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating the SignedBy and UDIN in the audit.", ex);
            }
        }

        public async Task<int> GetAuditFrameworkIdAsync(int compId, int auditId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var frameworkId = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT SA_AuditFrameworkId FROM StandardAudit_Schedule WHERE SA_ID = @AuditId AND SA_CompID = @CompId", new { AuditId = auditId, CompId = compId });
                return frameworkId;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred whilegettting Audit Framework Id.", ex);
            }
        }

        public async Task<int> CheckCAEIndependentAuditorsReportSavedAsync(int compId, int auditId)
        {            
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var frameworkId = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT SA_AuditFrameworkId FROM StandardAudit_Schedule WHERE SA_ID = @AuditId AND SA_CompID = @CompId", new { AuditId = auditId, CompId = compId });
                int reportTypeId = (frameworkId == 1) ? 2 : 33;
                const string CAEQuery = @"SELECT COUNT(*) FROM LOE_Template_Details WHERE LTD_FormName = 'CAE' AND LTD_ReportTypeID = @ReportTypeId AND LTD_LOE_ID = @LOEId";
                const string checkExistQuery = @"SELECT COUNT(*) FROM StandardAudit_ScheduleCheckPointList WHERE SAC_SA_ID = @SAC_SA_ID AND SAC_CompID = @SAC_CompID";
                const string incompleteMandatoryQuery = @"SELECT COUNT(*) FROM StandardAudit_ScheduleCheckPointList WHERE SAC_SA_ID = @SAC_SA_ID AND SAC_Mandatory = 1 AND SAC_TestResult IS NULL AND SAC_CompID = @SAC_CompID";

                int caeCount = await connection.ExecuteScalarAsync<int>(CAEQuery, new { LOEId = auditId, ReportTypeId = reportTypeId });
                if (caeCount == 0)
                {
                    return 0; // CAE Report not saved
                }

                var parameters = new { SAC_CompID = compId, SAC_SA_ID = auditId };
                int totalCheckpoints = await connection.ExecuteScalarAsync<int>(checkExistQuery, parameters);
                if (totalCheckpoints == 0)
                {
                    return 3; // No checkpoints
                }

                int incompleteMandatoryCount = await connection.ExecuteScalarAsync<int>(incompleteMandatoryQuery, parameters);
                if (incompleteMandatoryCount > 0)
                {
                    return 1; // Incomplete mandatory checkpoints
                }

                return 2;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while checking Audit status.", ex);
            }
        }

        public async Task<AuditSignedByUDINRequestDTO> GetSignedByUDINInAuditAsync(int compId, int auditId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"SELECT SA_SignedBy, SA_UDIN, SA_UDINdate, SA_ID, SA_CompID FROM StandardAudit_Schedule WHERE SA_ID = @AuditID AND SA_CompID = @ACID";

                var parameters = new { AuditID = auditId, ACID = compId };
                var result = await connection.QueryFirstOrDefaultAsync(query, parameters);

                if (result == null)
                {
                    return new AuditSignedByUDINRequestDTO();
                }

                return new AuditSignedByUDINRequestDTO
                {
                    SA_ID = result.SA_ID ?? 0,
                    SA_SignedBy = result.SA_SignedBy ?? 0,
                    SA_UDIN = result.SA_UDIN ?? string.Empty,
                    SA_UDINdate = result.SA_UDINdate ?? DateTime.MinValue,
                    SA_CompID = result.SA_CompID ?? 0
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching the SignedBy and UDIN in the audit.", ex);
            }
        }

        public async Task<(byte[] fileBytes, string contentType, string fileName)> GenerateAndDownloadReportAsync(int compId, int auditId, string format)
        {
            try
            {
                byte[] fileBytes;
                string contentType;
                string fileName = "Audit_Completion";

                if (format.ToLower() == "pdf")
                {
                    fileBytes = await GeneratePdfAsync(compId, auditId);
                    contentType = "application/pdf";
                    fileName += ".pdf";
                }
                else
                {
                    throw new ApplicationException("Unsupported format. Only PDF is currently supported.");
                }

                return (fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating the report.", ex);
            }
        }

        public async Task<string> GenerateReportAndGetURLPathAsync(int compId, int auditId, string format)
        {
            try
            {
                byte[] fileBytes;
                string contentType;
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"Audit_Completion_{timestamp}";

                if (format.ToLower() == "pdf")
                {
                    fileBytes = await GeneratePdfAsync(compId, auditId);
                    contentType = "application/pdf";
                    fileName += ".pdf";
                }
                else
                {
                    throw new ApplicationException("Unsupported format. Only PDF is currently supported.");
                }

                string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Tempfolder", compId.ToString());
                Directory.CreateDirectory(tempFolder);

                var filePath = Path.Combine(tempFolder, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                await File.WriteAllBytesAsync(filePath, fileBytes);

                string baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
                string downloadUrl = $"{baseUrl}/Tempfolder/{compId}/{fileName}";

                return downloadUrl;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating the report.", ex);
            }
        }

        public async Task<string> GenerateReportAndGetTempPathAsync(int compId, int auditId, string format)
        {
            try
            {
                byte[] fileBytes;
                string contentType;
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"Audit_Completion_{timestamp}";

                if (format.ToLower() == "pdf")
                {
                    fileBytes = await GeneratePdfAsync(compId, auditId);
                    contentType = "application/pdf";
                    fileName += ".pdf";
                }
                else
                {
                    throw new ApplicationException("Unsupported format. Only PDF is currently supported.");
                }

                string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Tempfolder", compId.ToString());
                Directory.CreateDirectory(tempFolder);

                var filePath = Path.Combine(tempFolder, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                await File.WriteAllBytesAsync(filePath, fileBytes);
                string downloadUrl = $"{tempFolder}/{fileName}";
                return downloadUrl;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating the report.", ex);
            }
        }

        public async Task<string> GenerateACSubPointsReportAndGetURLPathAsync(int compId, int auditId, int userId, string format)
        {
            try
            {
                byte[] fileBytes;
                string contentType;
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"Audit_Completion_SubPoints_Report_{timestamp}";

                if (format.ToLower() == "pdf")
                {
                    fileBytes = await GenerateACSubPointsPdfAsync(compId, auditId, userId);
                    contentType = "application/pdf";
                    fileName += ".pdf";
                }
                else
                {
                    throw new ApplicationException("Unsupported format. Only PDF is currently supported.");
                }

                string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Tempfolder", compId.ToString());
                Directory.CreateDirectory(tempFolder);

                var filePath = Path.Combine(tempFolder, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                await File.WriteAllBytesAsync(filePath, fileBytes);

                string baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
                string downloadUrl = $"{baseUrl}/Tempfolder/{compId}/{fileName}";

                return downloadUrl;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating the report.", ex);
            }
        }

        public async Task<string> GenerateACSubPointsReportAndGetTempPathAsync(int compId, int auditId, int userId, string format)
        {
            try
            {
                byte[] fileBytes;
                string contentType;
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"Audit_Completion_SubPoints_Report_{timestamp}";

                if (format.ToLower() == "pdf")
                {
                    fileBytes = await GenerateACSubPointsPdfAsync(compId, auditId, userId);
                    contentType = "application/pdf";
                    fileName += ".pdf";
                }
                else
                {
                    throw new ApplicationException("Unsupported format. Only PDF is currently supported.");
                }

                string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Tempfolder", compId.ToString());
                Directory.CreateDirectory(tempFolder);

                var filePath = Path.Combine(tempFolder, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                await File.WriteAllBytesAsync(filePath, fileBytes);
                string downloadUrl = $"{tempFolder}/{fileName}";
                return downloadUrl;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating the report.", ex);
            }
        }

        private async Task<byte[]> GenerateACSubPointsPdfAsync(int compId, int auditId, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string companyName = await connection.ExecuteScalarAsync<string>(@"SELECT STUFF((SELECT DISTINCT '; ' + CAST(Company_Name AS VARCHAR(MAX)) FROM Trace_CompanyDetails WHERE Company_CompID = @CompId FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '')", new { CompId = compId });
            string userName = await connection.ExecuteScalarAsync<string>(@"SELECT Usr_FullName FROM Sad_Userdetails WHERE Usr_ID = @UserId And Usr_CompId = @CompId", new { CompId = compId, UserId = userId });

            var templateDetails = await connection.QueryAsync<EngagementPlanTemplateReportDetailsDTO>(
                @"SELECT LTD_ReportTypeID, LTD_Heading, LTD_Decription FROM LOE_Template_Details WHERE LTD_FormName = 'AC' AND LTD_LOE_ID = @LOEId AND LTD_CompID = @CompId;", new { CompId = compId, LOEId = auditId });

            var checkpoints = await GetAuditCompletionCheckPointDetailsForReportAsync(compId, auditId);

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;

            return await Task.Run(() =>
            {
                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.Size(PageSizes.A4);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Content().Column(column =>
                        {
                            column.Item().PaddingBottom(10).AlignCenter().PaddingBottom(10).Text("Audit Completion Document").FontSize(16).Bold();
                            column.Item().PaddingBottom(5).Text($"Company Name: {companyName}").FontSize(10);
                            column.Item().PaddingBottom(5).Text($"Balance Sheet Date: {DateTime.Now.ToString("dd MMM yyyy")}").FontSize(10);
                            column.Item().PaddingBottom(5).Text($"Completed By: {userName}").FontSize(10);
                            column.Item().PaddingBottom(10).Text($"Date: {DateTime.Now.ToString("dd MMM yyyy")}").FontSize(10);

                            if (templateDetails?.Any() == true)
                            {
                                var firstItem = templateDetails.First();
                                if (!string.IsNullOrWhiteSpace(firstItem.LTD_Heading) && !firstItem.LTD_Heading.ToString().StartsWith("NH", StringComparison.OrdinalIgnoreCase))
                                    column.Item().Text(firstItem.LTD_Heading).FontSize(11).Bold();
                                if (!string.IsNullOrWhiteSpace(firstItem.LTD_Decription))
                                    column.Item().Text(firstItem.LTD_Decription).FontSize(10);
                                column.Item().PaddingBottom(5);
                            }

                            if (checkpoints?.Rows.Count > 0)
                            {
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(1.5f);
                                        columns.RelativeColumn(1.5f);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("SubPoint").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Remarks").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Workpaper Ref").FontSize(10).Bold();
                                    });

                                    foreach (DataRow row in checkpoints.Rows)
                                    {
                                        table.Cell().Element(CellStyle).Text(row["SubPoint"]?.ToString() ?? "");
                                        table.Cell().Element(CellStyle).Text(row["Remarks"]?.ToString() ?? "");
                                        table.Cell().Element(CellStyle).Text(row["WorkpaperRef"]?.ToString() ?? "");
                                    }

                                    static IContainer CellStyle(IContainer container) => container.Border(0.5f).PaddingVertical(3).PaddingHorizontal(4);
                                });
                            }

                            if (templateDetails?.Any() == true)
                            {                                
                                foreach (var item in templateDetails.Skip(1))
                                {
                                    column.Item().PaddingTop(5);
                                    //column.Item().Text(item.LTD_Heading).FontSize(11).Bold();
                                    if (!string.IsNullOrWhiteSpace(item.LTD_Decription))
                                        column.Item().Text(item.LTD_Decription).FontSize(10);
                                }
                            }
                        });
                    });
                });

                using var ms = new MemoryStream();
                document.GeneratePdf(ms);
                return ms.ToArray();
            });
        }

        public async Task<DataTable> GetAuditCompletionCheckPointDetailsForReportAsync(int iAcID, int iAuditID)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var dt = new DataTable();
                dt.Columns.Add("SubPoint");
                dt.Columns.Add("Remarks");
                dt.Columns.Add("WorkpaperRef");

                string queryHeading = $@"SELECT CMM_ID, CMM_Desc AS Name FROM Content_Management_Master WHERE CMM_Category='ASF' AND CMM_CompID={iAcID} AND CMM_Delflag='A' 
                    AND CMS_KeyComponent IN (SELECT SA_AuditFrameworkId FROM StandardAudit_Schedule WHERE SA_ID = {iAuditID} AND SA_CompID = {iAcID}) ORDER BY CMM_Desc ASC";
                var dtTab = await GetDataTableAsync(connection, queryHeading);

                foreach (DataRow row in dtTab.Rows)
                {
                    int checkpointId = Convert.ToInt32(row["CMM_ID"]);

                    var headingRow = dt.NewRow();
                    headingRow["SubPoint"] = row["Name"].ToString();
                    headingRow["Remarks"] = "";
                    headingRow["WorkpaperRef"] = "";
                    dt.Rows.Add(headingRow);

                    string querySubPoint = $@"SELECT ASM_SubPoint, SAC_Remarks, SSW_WorkpaperRef FROM AuditCompletion_SubPoint_Master 
                                      LEFT JOIN StandardAudit_Audit_Completion ON SAC_CheckPointId=ASM_CheckpointID AND SAC_SubPointId=ASM_ID AND SAC_AuditID={iAuditID} AND SAC_CheckPointId={checkpointId} 
                                      LEFT JOIN Content_Management_Master ON CMM_ID=ASM_CheckpointID AND CMM_ID={checkpointId} 
                                      LEFT JOIN StandardAudit_ScheduleConduct_WorkPaper ON SAC_WorkPaperId=SSW_ID 
                                      WHERE ASM_CheckpointID={checkpointId} AND ASM_CompId={iAcID} 
                                      ORDER BY ASM_ID";

                    var dtAC = await GetDataTableAsync(connection, querySubPoint);
                    foreach (DataRow subRow in dtAC.Rows)
                    {
                        var detailRow = dt.NewRow();
                        detailRow["SubPoint"] = subRow["ASM_SubPoint"].ToString();
                        detailRow["Remarks"] = subRow["SAC_Remarks"].ToString();
                        detailRow["WorkpaperRef"] = subRow["SSW_WorkpaperRef"].ToString();
                        dt.Rows.Add(detailRow);
                    }

                    var emptyRow = dt.NewRow();
                    emptyRow["SubPoint"] = "";
                    emptyRow["Remarks"] = "";
                    emptyRow["WorkpaperRef"] = "";
                    dt.Rows.Add(emptyRow);
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting Audit Completion CheckPoint Details", ex);
            }
        }

        public async Task<string> GetCustAllLocationLOEDetailsAsync(int compId, int customerId)
        {
            string result = string.Empty;

            string sql = @"SELECT STUFF((SELECT DISTINCT '; ' + CONVERT(varchar(10), LOE_TimeSchedule, 103) + ' to ' + CONVERT(varchar(10), LOE_ReportDueDate, 103)
                FROM SAD_CUST_LOE
                WHERE LOE_CustomerId = @CustomerId AND LOE_CompID = @CompId
                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')";

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);
                command.Parameters.AddWithValue("@CompId", compId);

                var resultObj = await command.ExecuteScalarAsync();
                result = resultObj != null ? resultObj.ToString() : string.Empty;
            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            return result;
        }

        public async Task<string> GetCustAllLocationDetailsAsync(int compId, int customerId, string columnType)
        {
            string result = string.Empty;

            string sql = $@"SELECT STUFF((SELECT DISTINCT '; ' + CAST(Mas_Description + ' : ' + {columnType} AS VARCHAR(MAX))
                FROM SAD_CUST_LOCATION
                WHERE Mas_CustID = @CustomerId AND Mas_CompID = @CompId
                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')";

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);
                command.Parameters.AddWithValue("@CompId", compId);

                var resultObj = await command.ExecuteScalarAsync();
                result = resultObj?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            return result;
        }

        public async Task<string> GetCustAllLocationStatutoryRefDetailsAsync(int compId, int customerId, string statutoryType)
        {
            string result = string.Empty;

            string sql = @"SELECT STUFF((SELECT DISTINCT'; ' + CAST(Mas_Description + ' : ' + Cust_Value AS VARCHAR(MAX))
                FROM SAD_CUST_Accounting_Template
                INNER JOIN SAD_CUST_LOCATION ON Mas_Id = Cust_LocationId
                WHERE Cust_Desc = @StatutoryType AND Cust_Id = @CustomerId AND Mas_CustID = @CustomerId AND Mas_CompID = @CompId
                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')";

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);
                command.Parameters.AddWithValue("@CompId", compId);
                command.Parameters.AddWithValue("@StatutoryType", statutoryType);

                var resultObj = await command.ExecuteScalarAsync();
                result = resultObj?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            return result;
        }

        public async Task<string> GetCustomerDetailsByColumnAsync(int compId, int customerId, string columnType)
        {
            string result = string.Empty;
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @" SELECT @ColumnType FROM SAD_CUSTOMER_MASTER WHERE CUST_ID = @CustomerId AND CUST_CompID = @CompId";
                var customer = await connection.QueryFirstOrDefaultAsync(sql, new
                {
                    CustomerId = customerId,
                    CompId = compId
                });

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);
                command.Parameters.AddWithValue("@CompId", compId);
                command.Parameters.AddWithValue("@ColumnType", columnType);

                var resultObj = await command.ExecuteScalarAsync();
                result = resultObj?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            return result;
        }

        public async Task<string> GetCompanyDetailsByColumnAsync(int compId, string columnType)
        {
            string result = string.Empty;
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @" Select @ColumnType  From Trace_CompanyDetails where Company_ID = @CompId";
                var customer = await connection.QueryFirstOrDefaultAsync(sql, new
                {
                    CompId = compId
                });

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@CompId", compId);
                command.Parameters.AddWithValue("@ColumnType", columnType);

                var resultObj = await command.ExecuteScalarAsync();
                result = resultObj?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            return result;
        }

        public async Task<(string CustomerName, string OrgTypeDesc, string CommitmentDate, string ProductsManufactured)> GetCustReportForCustAuditTypeAsync(int customerId)
        {
            string sql = @"SELECT  ISNULL(CUST_NAME, '') AS CUST_NAME, ISNULL(cmm_Desc, '') AS cmm_Desc, ISNULL(CONVERT(varchar(10), CUST_CommitmentDate, 103), '') AS CUST_CommitmentDate, ISNULL(CDET_PRODUCTSMANUFACTURED, '') AS CDET_PRODUCTSMANUFACTURED FROM SAD_CUSTOMER_MASTER
                LEFT JOIN SAD_CUSTOMER_DETAILS ON CDET_CUSTID = CUST_ID
                LEFT JOIN Content_Management_Master ON cmm_ID = CUST_ORGTYPEID
                WHERE CUST_ID = @CustomerId";

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    string custName = reader["CUST_NAME"]?.ToString() ?? string.Empty;
                    string orgType = reader["cmm_Desc"]?.ToString() ?? string.Empty;
                    string commitmentDate = reader["CUST_CommitmentDate"]?.ToString() ?? string.Empty;
                    string productsManufactured = reader["CDET_PRODUCTSMANUFACTURED"]?.ToString() ?? string.Empty;

                    return (custName, orgType, commitmentDate, productsManufactured);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return (string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public async Task<List<AuditReportCustInfoAuditeeDetailDTO>> GetAuditeeDetails(int compId, int customerId, string yearName)
        {
            var report = new List<AuditReportCustInfoAuditeeDetailDTO>();
            int sl = 1;

            void Add(string title, string value)
            {
                report.Add(new AuditReportCustInfoAuditeeDetailDTO
                {
                    SlNo = sl++,
                    Particulars = title,
                    Details = value ?? string.Empty
                });
            }

            var (customerName, orgTypeDesc, commitmentDate, productsManufactured) = await GetCustReportForCustAuditTypeAsync(customerId);

            Add("Name of the auditee", customerName);
            Add("Financial year of Audit", yearName);
            Add("Period of Audit (i.e. From dd/mm/yyyy to dd/mm/yyyy)", await GetCustAllLocationLOEDetailsAsync(compId, customerId));
            Add("Constitution", orgTypeDesc);
            Add("Changes in constitution during the year", "");
            Add("Nature of Audit to be conducted –\n• Statutory Audit\n• Tax Audit\n• Charitable/Religious Trust Audit\n• Special Audit\n• Internal/Other", "");
            Add("Address(es) of places of Business", await GetCustAllLocationDetailsAsync(compId, customerId, "Mas_Loc_Address"));
            Add("Audit scope (whole/specific unit)", "");
            Add("Phone numbers of all places of business", await GetCustAllLocationDetailsAsync(compId, customerId, "Mas_Contact_LandLineNo"));
            Add("Fax numbers of all places of business", "");
            Add("E-mail addresses of all places of business", await GetCustAllLocationDetailsAsync(compId, customerId, "Mas_Contact_Email"));
            Add("Date of Incorporation/Formation", commitmentDate);
            Add("Company (CIN)/Firm Registration Number", await GetCustAllLocationStatutoryRefDetailsAsync(compId, customerId, "CIN"));
            Add("Income Tax PAN", await GetCustAllLocationStatutoryRefDetailsAsync(compId, customerId, "PAN"));
            Add("TAN of all units", await GetCustAllLocationStatutoryRefDetailsAsync(compId, customerId, "TAN"));
            Add("Central Excise Registration Numbers", await GetCustAllLocationStatutoryRefDetailsAsync(compId, customerId, "Central Excise Registration Numbers"));
            Add("Service Tax Registration Numbers", await GetCustAllLocationStatutoryRefDetailsAsync(compId, customerId, "Service Tax Registration Numbers"));
            Add("VAT Registration Numbers", await GetCustAllLocationStatutoryRefDetailsAsync(compId, customerId, "VAT Registration Numbers"));
            Add("GST Registration Numbers", await GetCustAllLocationStatutoryRefDetailsAsync(compId, customerId, "GSTIN"));
            Add("Import Export Code Number (IEC)", await GetCustAllLocationStatutoryRefDetailsAsync(compId, customerId, "Import Export Code Number (IEC)"));
            Add("Bank Account Details", "");
            Add("Key persons for audit interaction", "");
            Add("Contact person/ Coordinator", "");
            Add("Nature of Business / Core Activity", "");
            Add("Brief note on the manufacturing process", productsManufactured);
            Add("Main products / By-products", "");
            Add("Main Raw materials used", "");
            Add("Method of Accounting", "");
            Add("Method of Book keeping", "");
            Add("Accounting package used & generated reports", "");
            Add("List of books (computerized/manual)", "");
            Add("Covered by Internal Audit", "");
            Add("Other entities where directors are interested", "");
            Add("Nature of such interest", "");
            Add("DIN of all Directors", "");
            return report;
        }

        public async Task<List<ConductAuditReportDetailDTO>> GetConductAuditReportAsync(int compId, int auditId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"SELECT DENSE_RANK() OVER (ORDER BY SAC_CheckPointID) AS SlNo, ACM_Heading AS Heading, ACM_Checkpoint AS CheckPoints, ISNULL(SAC_Remarks, '') AS Comments,
                    CASE WHEN SAC_Annexure = 1 THEN 'Yes' WHEN SAC_Annexure = 0 THEN 'No' ELSE '' END AS Annexure ,
                    CASE WHEN ISNULL(SAC_Mandatory, 0) = 1 THEN 'Yes' ELSE 'No' END AS Mandatory, ISNULL(SSW_WorkpaperRef, '') AS WorkpaperRef,
                    CASE WHEN ISNULL(SAC_TestResult, 0) = 1 THEN 'Yes' WHEN ISNULL(SAC_TestResult, 0) = 2 THEN 'No' WHEN ISNULL(SAC_TestResult, 0) = 3 THEN 'NA' ELSE '' END AS TestResult,
                    ISNULL(a.Usr_FullName, '') AS ConductedBy, ISNULL(SAC_LastUpdatedOn,'') AS ConductedOn
                    FROM StandardAudit_ScheduleCheckPointList
                    LEFT JOIN AuditType_Checklist_Master ON ACM_ID = SAC_CheckPointID
					LEFT JOIN sad_userdetails a ON a.Usr_ID = SAC_ConductedBy
                    LEFT JOIN StandardAudit_ScheduleConduct_WorkPaper ON SSW_SA_ID = @AuditID AND SSW_ID = SAC_WorkpaperID
                    WHERE SAC_SA_ID = @AuditID AND SAC_CompID = @CompID ORDER BY SAC_CheckPointID";

                var parameters = new
                {
                    CompID = compId,
                    AuditID = auditId
                };

                var result = await connection.QueryAsync<ConductAuditReportDetailDTO>(query, parameters);
                return result.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting Audit or Review - Testing Report details.", ex);
            }
        }

        public async Task<List<ConductAuditRemarksReportDTO>> GetConductAuditRemarksReportAsync(int compId, int auditId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"SELECT a.SCR_CheckPointID,ISNULL(x.ACM_Checkpoint, '') AS ACM_Checkpoint,a.SCR_Date,a.SCR_Remarks As SCR_Remarks,
                    c.USr_FullName As SCR_RemarksByName,e.Mas_Description As SCR_RemarksByRole,ISNULL(d.USr_FullName,'') + ' - ' + ISNULL(b.SCR_Remarks,'') As SCR_ClientRemarks
                    FROM StandardAudit_ConductAudit_RemarksHistory a 
					JOIN AuditType_Checklist_Master x ON x.ACM_ID = a.SCR_CheckPointID
                    Left Join sad_userdetails c on c.Usr_ID = a.SCR_RemarksBy 
                    Left Join StandardAudit_ConductAudit_RemarksHistory b on a.SCR_ID=b.SCR_IsIssueRaised And b.SCR_IsIssueRaised > 1
                    Left Join sad_userdetails d on d.Usr_ID=b.SCR_RemarksBy 
                    Left Join SAD_GrpOrLvl_General_Master e on e.Mas_ID=c.Usr_Role 
                    Where a.SCR_SA_ID = @AuditID And a.SCR_CompID = @CompID And a.SCR_IsIssueRaised <= 1
                    Order by a.SCR_CheckPointID,a.SCR_ID Desc";
                var parameters = new { AuditID = auditId, CompID = compId };

                var rawData = (await connection.QueryAsync(query, parameters)).ToList();

                var result = new List<ConductAuditRemarksReportDTO>();
                int srNo = 0;
                int? lastCheckpointId = null;

                foreach (var row in rawData)
                {
                    int currentCheckpointId = row.SCR_CheckPointID;
                    string checkpointText = row.ACM_Checkpoint?.ToString();
                    string remarkText = row.SCR_Remarks?.ToString();

                    var dto = new ConductAuditRemarksReportDTO();

                    if (lastCheckpointId != currentCheckpointId)
                    {
                        srNo++;
                        dto.SrNo = srNo.ToString();
                        dto.CheckPoint = checkpointText;
                        dto.Observations = $"{remarkText}";
                    }
                    else
                    {
                        dto.SrNo = "";
                        dto.CheckPoint = "";
                        dto.Observations = $"{remarkText}";
                    }

                    dto.Date = row.SCR_Date;
                    dto.RemarksBy = row.SCR_RemarksByName;
                    dto.RemarksByRole = row.SCR_RemarksByRole;
                    dto.ClientRemarks = row.SCR_ClientRemarks;

                    lastCheckpointId = currentCheckpointId;
                    result.Add(dto);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading Audit or Review - Testing remarks report", ex);
            }
        }

        public async Task<List<ConductAuditObservationDTO>> GetConductAuditObservationsAsync(int compId, int auditId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                string query = @"SELECT SSO_SAC_CheckPointID, ACM_Checkpoint, SSO_Observations FROM StandardAudit_ScheduleObservations
                    LEFT JOIN AuditType_Checklist_Master ON ACM_ID = SSO_SAC_CheckPointID WHERE SSO_SA_ID = @AuditID AND SSO_CompID = @CompID
                    ORDER BY SSO_SAC_CheckPointID, SSO_Observations DESC";

                var parameters = new { AuditID = auditId, CompID = compId };

                var rawData = (await connection.QueryAsync(query, parameters)).ToList();

                var result = new List<ConductAuditObservationDTO>();
                int srNo = 0;
                int observationCount = 0;
                int? lastCheckpointId = null;

                foreach (var row in rawData)
                {
                    int currentCheckpointId = row.SSO_SAC_CheckPointID;
                    string checkpointText = row.ACM_Checkpoint?.ToString();
                    string observationText = row.SSO_Observations?.ToString();

                    var dto = new ConductAuditObservationDTO();

                    if (lastCheckpointId != currentCheckpointId)
                    {
                        srNo++;
                        observationCount = 1;
                        dto.SrNo = srNo.ToString();
                        dto.CheckPoint = checkpointText;
                        dto.Observations = $"Observation {observationCount}: {observationText}";
                    }
                    else
                    {
                        observationCount++;
                        dto.SrNo = "";
                        dto.CheckPoint = "";
                        dto.Observations = $"Observation {observationCount}: {observationText}";
                    }

                    lastCheckpointId = currentCheckpointId;
                    result.Add(dto);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading audit observations report", ex);
            }
        }

        public async Task<List<ConductAuditWorkPaperDTO>> LoadConductAuditWorkPapersAsync(int compId, int auditId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                string query = @"SELECT SSW_ID AS PKID, SSW_WorkpaperNo AS WorkpaperNo, SSW_WorkpaperRef AS WorkpaperRef, SSW_Observation AS Deviations, SSW_Conclusion AS Conclusion, 
                    SSW_NotesSteps As Notes, SSW_ReviewerComments AS ReviewerComments, SSW_CriticalAuditMatter As CriticalAuditMatter, SSW_AttachID AS AttachID, b.usr_FullName AS CreatedBy,
                    CONVERT(VARCHAR(10), SSW_CrOn, 103) AS CreatedOn, c.usr_FullName AS ReviewedBy, ISNULL(CONVERT(VARCHAR(10), SSW_ReviewedOn, 103), '') AS ReviewedOn,
                    ISNULL(STUFF((SELECT ', ' + cmm.CMM_Desc FROM STRING_SPLIT(CAST(a.SSW_TypeOfTest AS VARCHAR(MAX)), ',') AS s JOIN Content_Management_Master cmm ON TRY_CAST(s.value AS INT) = cmm.CMM_ID
                    WHERE cmm.CMM_Category = 'TOT' FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''),'') AS TypeOfTest, SSW_AuditorHoursSpent As AuditorHoursSpent,
                    CASE WHEN a.SSW_ExceededMateriality = 1 THEN 'Yes' WHEN a.SSW_ExceededMateriality = 2 THEN 'No' WHEN a.SSW_ExceededMateriality = 3 THEN 'NA' ELSE NULL END AS ExceededMateriality,                   
                    CASE WHEN a.SSW_Status = 1 THEN 'Open' WHEN a.SSW_Status = 2 THEN 'WIP' WHEN a.SSW_Status = 3 THEN 'Closed' ELSE '' END AS Status,
                    ISNULL((SELECT Count(*) FROM edt_attachments WHERE ATCH_CompID = @CompId AND ATCH_ID in (SELECT SSW_AttachID FROM StandardAudit_ScheduleConduct_WorkPaper SSW WHERE SSW.SSW_SA_ID = @AuditId AND SSW.SSW_CompID = @CompId And SSW.SSW_ID = a.SSW_ID)), 0) As AttachmentCount
                    FROM StandardAudit_ScheduleConduct_WorkPaper a
                    LEFT JOIN sad_userdetails b ON b.Usr_ID = a.SSW_CrBy
                    LEFT JOIN sad_userdetails c ON c.Usr_ID = a.SSW_ReviewedBy
                    WHERE a.SSW_SA_ID = @AuditID AND a.SSW_CompID = @CompID ORDER BY a.SSW_WorkpaperNo DESC";

                var parameters = new { AuditID = auditId, CompID = compId };

                var result = await connection.QueryAsync<ConductAuditWorkPaperDTO>(query, parameters);
                return result.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading Audit or Review - Testing Work Papers", ex);
            }
        }

        private async Task<byte[]> GeneratePdfAsync(int compId, int auditId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryFirstOrDefaultAsync<(int EpPkId, int CustID, string YearName, string AuditNo)>(
                @"SELECT LOE.LOE_ID AS EpPkId, SA.SA_CustID As CustID, YMS.YMS_ID AS YearName, SA_AuditNo + ' - ' + CMA.CMM_Desc As AuditNo FROM StandardAudit_Schedule AS SA
                  LEFT JOIN SAD_CUST_LOE AS LOE ON LOE.LOE_CustomerId = SA.SA_CustID AND LOE.LOE_YearId = SA.SA_YearID AND LOE.LOE_ServiceTypeId = SA.SA_AuditTyPeId
                  LEFT JOIN YEAR_MASTER AS YMS ON YMS.YMS_YEARID = SA.SA_YearID
                  LEFT JOIN Content_Management_Master CMA On CMA.cmm_ID = SA.SA_AuditTypeID
                  WHERE LOE.LOE_CompID = @CompId AND SA.SA_ID = @AuditId;", new { CompId = compId, AuditId = auditId });

            EngagementPlanReportDetailsDTO dtoEP = await _engagementPlanInterface.GetEngagementPlanReportDetailsByIdAsync(compId, result.EpPkId);
            List<AuditReportCustInfoAuditeeDetailDTO> dtoAD = await GetAuditeeDetails(compId, result.CustID, result.YearName);
            List<ConductAuditWorkPaperDTO> dtoCAWP = await LoadConductAuditWorkPapersAsync(compId, auditId);
            List<ConductAuditReportDetailDTO> dtoCA = await GetConductAuditReportAsync(compId, auditId);
            List<ConductAuditRemarksReportDTO> dtoCAO = await GetConductAuditRemarksReportAsync(compId, auditId);
            IEnumerable<CMADto> CAMresult = await _auditSummaryInterface.GetCAMDetailsAsync(compId, auditId);
            List<CMADto> dtoCAM = CAMresult.ToList();

            var address = await GetCustomerDetailsByColumnAsync(compId, result.CustID, "CUST_ADDRESS");
            var city = await GetCustomerDetailsByColumnAsync(compId, result.CustID, "CUST_CITY");
            var state = await GetCustomerDetailsByColumnAsync(compId, result.CustID, "CUST_STATE");
            var pin = await GetCustomerDetailsByColumnAsync(compId, result.CustID, "CUST_PIN");
            var email = await GetCustomerDetailsByColumnAsync(compId, result.CustID, "CUST_EMAIL");
            var telephone = await GetCustomerDetailsByColumnAsync(compId, result.CustID, "CUST_TELPHONE");


            var reportTypeList = await connection.QueryAsync<DropDownListData>(@"SELECT RTM_Id AS ID, RTM_ReportTypeName As Name FROM SAD_ReportTypeMaster
                    WHERE RTM_TemplateId = 4 And RTM_AudrptType = 3 And RTM_DelFlag = 'A' AND RTM_CompID = @CompId ORDER BY RTM_ReportTypeName", new { CompId = compId });

            var allDtoCAEs = new Dictionary<int, List<CommunicationWithClientTemplateReportDetailsDTO>>();
            foreach (var reportType in reportTypeList)
            {
                var dtoCAE = (await connection.QueryAsync<CommunicationWithClientTemplateReportDetailsDTO>(
                    @"SELECT LTD_ReportTypeID, LTD_Heading, LTD_Decription FROM LOE_Template_Details WHERE LTD_FormName = 'CAE' AND LTD_ReportTypeID = @ReportTypeID AND LTD_LOE_ID = @LOEId AND LTD_CompID = @CompId;",
                    new { CompId = compId, ReportTypeID = reportType.ID, LOEId = auditId })).ToList();
                if (dtoCAE.Count > 0)
                {
                    allDtoCAEs[reportType.ID] = dtoCAE;
                }
            }

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;

            return await Task.Run(() =>
            {
                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.Size(PageSizes.A4);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Content().Column(column =>
                        {
                            column.Item().AlignCenter().PaddingBottom(10).Text("Letter of Engagement").FontSize(16).Bold();
                            column.Item().PaddingBottom(5).Text($"Ref.No.: {dtoEP.EngagementPlanNo}").FontSize(12).Bold();
                            column.Item().PaddingBottom(5).Text($"Date: {dtoEP.CurrentDate:dd MMM yyyy}").FontSize(10);
                            column.Item().PaddingBottom(10).Text(dtoEP.Customer ?? "N/A").FontSize(10);


                            column.Item().PaddingBottom(10).Text($"Dear: {dtoEP.Customer ?? "Client"}").FontSize(10);
                            column.Item().PaddingBottom(10).Text($"{dtoEP.Subject ?? "Sub: Engagement Letter"}").FontSize(10);

                            if (dtoEP.EngagementTemplateDetails?.Any() == true)
                            {
                                int count = 1;
                                foreach (var item in dtoEP.EngagementTemplateDetails)
                                {
                                    column.Item().Text($"{count}. {item.LTD_Heading}").FontSize(11).Bold();
                                    if (!string.IsNullOrWhiteSpace(item.LTD_Decription))
                                        column.Item().Text(item.LTD_Decription).FontSize(10);

                                    column.Item().PaddingBottom(5);
                                    count++;
                                }
                            }

                            if (dtoEP.EngagementAdditionalFees?.Any() == true)
                            {
                                column.Item().PaddingTop(10).Text("Additional Fees").FontSize(12).Bold().Underline();
                                column.Item().PaddingBottom(10).Text($"Details of Engagement Estimate for the Letter of Engagement in {dtoEP.AuditType} to {dtoEP.Customer}").FontSize(10);
                                
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(0.5f);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(1);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("Sl No").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Expense Name").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).AlignRight().Text($"Charges In {dtoEP.CurrencyType}").FontSize(10).Bold();
                                    });

                                    int slNo = 1;
                                    decimal totalCharges = 0;
                                    foreach (var fee in dtoEP.EngagementAdditionalFees)
                                    {
                                        table.Cell().Element(CellStyle).Text(slNo.ToString()).FontSize(10);
                                        table.Cell().Element(CellStyle).Text(fee.LAF_OtherExpensesName ?? "-").FontSize(10);
                                        var displayCharge = decimal.TryParse(fee.LAF_Charges, out var charge) ? charge.ToString("F2") : "0.00";
                                        table.Cell().Element(CellStyle).AlignRight().Text(displayCharge).FontSize(10);
                                        totalCharges += charge;
                                        slNo++;
                                    }

                                    table.Cell().Element(CellStyle).Text("");
                                    table.Cell().Element(CellStyle).Text("Total").FontSize(10).Bold();
                                    table.Cell().Element(CellStyle).AlignRight().Text(totalCharges.ToString("F2")).FontSize(10).Bold();

                                    static IContainer CellStyle(IContainer container) => container.Border(0.5f).PaddingVertical(3).PaddingHorizontal(4);
                                });
                            }

                            column.Item().PaddingTop(20).Text("Very truly yours,").FontSize(10);
                            column.Item().Text(dtoEP.CompanyName ?? "[Company Name]").FontSize(10).Bold();
                            column.Item().Text("[Firm Name]").FontSize(10);
                            column.Item().PaddingBottom(30);
                            column.Item().PaddingTop(10).PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Black);
                            column.Item().PaddingBottom(20).Text("We agree to the terms of the engagement described in this letter.").FontSize(10);
                            column.Item().Text(dtoEP.Customer ?? "[Client Name]").FontSize(10).Bold();
                            column.Item().Text("[Client Name]").FontSize(10);
                            column.Item().PaddingBottom(20);
                            column.Item().Text("[Signature]").FontSize(10);
                            column.Item().PaddingBottom(20);
                            column.Item().Text("[Date]").FontSize(10);
                        });
                    });

                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.Size(PageSizes.A4);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));
                        page.Content().Column(column =>
                        {
                            column.Item().AlignCenter().PaddingBottom(10).Text("Profile/Information about the Auditee").FontSize(16).Bold();
                            if (dtoAD.Any() == true)
                            {
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(0.5f);
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(2);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("Sl No").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Particulars").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Details").FontSize(10).Bold();
                                    });

                                    int slNo = 1;
                                    foreach (var details in dtoAD)
                                    {
                                        table.Cell().Element(CellStyle).Text(slNo.ToString()).FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.Particulars ?? "-").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.Details ?? "-").FontSize(10);
                                        slNo++;
                                    }
                                    static IContainer CellStyle(IContainer container) => container.Border(0.5f).PaddingVertical(3).PaddingHorizontal(4);
                                });
                            }

                            column.Item().PaddingTop(20).Text("Very truly yours,").FontSize(10);
                            column.Item().PaddingBottom(5).Text("For " + dtoEP.CompanyName ?? "").FontSize(10).Bold();
                            column.Item().PaddingBottom(5).Text("[Designation]").FontSize(10);
                            column.Item().PaddingBottom(5).Text("Place : ").FontSize(10);
                            column.Item().PaddingBottom(5).Text("Date :").FontSize(10);
                        });
                    });

                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.Size(PageSizes.A4);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Content().Column(column =>
                        {
                            column.Item().AlignCenter().PaddingBottom(10).Text("Audit or Review - Testing Workpaper Report").FontSize(16).Bold();
                            column.Item().Text(text =>
                            {
                                text.Span("Client Name: ").FontSize(10).Bold();
                                text.Span(dtoEP.Customer).FontSize(10);
                            });
                            column.Item().Text(text =>
                            {
                                text.Span("Audit No: ").FontSize(10).Bold();
                                text.Span(result.AuditNo).FontSize(10);
                            });

                            column.Item().PaddingBottom(10);

                            if (dtoCAWP.Any() == true)
                            {
                                column.Item().AlignCenter().PaddingBottom(10).Text("Work Paper Details").FontSize(14).Bold();
                                column.Item().PaddingBottom(15).Table(table =>
                                {
                                    foreach (var details in dtoCAWP)
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(1);
                                            columns.RelativeColumn(2);
                                        });

                                        table.Cell().Element(CellStyle).Text("Workpaper Name:").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text(details.WorkpaperRef).FontSize(10);

                                        table.Cell().Element(CellStyle).Text("Created By and Date:").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text($"{details.CreatedBy}, {details.CreatedOn}").FontSize(10);

                                        table.Cell().Element(CellStyle).Text("Workpaper No:").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text(details.WorkpaperNo).FontSize(10);

                                        table.Cell().Element(CellStyle).Text("Reviewed By and Date:").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text($"{details.ReviewedBy}, {details.ReviewedOn}").FontSize(10);

                                        table.Cell().Element(CellStyle).Text("Type of Test:").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text(details.TypeOfTest).FontSize(10);

                                        table.Cell().Element(CellStyle).Text("Status:").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text(details.Status).FontSize(10);

                                        table.Cell().Element(CellStyle).Text("Exceeded Materiality:").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text(details.ExceededMateriality).FontSize(10);

                                        table.Cell().Element(CellStyle).Text("Auditor Hours Spent:").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text(details.AuditorHoursSpent).FontSize(10);

                                        table.Cell().Element(CellStyle).Text("Notes/Steps:").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text(details.Notes).FontSize(10);

                                        table.Cell().Element(CellStyle).Text("Deviations/Exceptions Noted:").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text(details.Deviations).FontSize(10);

                                        table.Cell().Element(CellStyle).Text("Critical Audit Matter(CAM):").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text(details.CriticalAuditMatter).FontSize(10);

                                        table.Cell().Element(CellStyle).Text("Conclusion:").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text(details.Conclusion).FontSize(10);

                                        table.Cell().Element(CellStyle).Text("Attachments:").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text(details.AttachmentCount).FontSize(10);

                                        table.Cell().Element(CellStyle).Text("").FontSize(10).Bold();
                                        table.Cell().Element(CellStyle).Text("").FontSize(10);
                                    }
                                    static IContainer CellStyle(IContainer container) => container.Border(0.5f).PaddingVertical(3).PaddingHorizontal(4);
                                });
                            }

                            column.Item().PaddingBottom(10);

                            if (dtoCA.Any() == true)
                            {
                                column.Item().AlignCenter().PaddingBottom(10).Text("Audit or Review - Testing Heading wise Checkpoints Report").FontSize(14).Bold();
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(0.5f);
                                        columns.RelativeColumn(1.75f);
                                        columns.RelativeColumn(1.75f);
                                        columns.RelativeColumn(1.5f);
                                        columns.RelativeColumn(2);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("Sl No").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Heading").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Check Point").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Assertions").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Workpaper Ref/Index").FontSize(10).Bold();
                                    });

                                    int slNo = 1;
                                    foreach (var details in dtoCA)
                                    {
                                        table.Cell().Element(CellStyle).Text(slNo.ToString()).FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.Heading).FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.CheckPoints).FontSize(10);
                                        table.Cell().Element(CellStyle).Text($"Mandatory: {details.Mandatory}\nTest Result: {details.TestResult}\nAnnexure: {details.Annexure}").FontSize(10);
                                        table.Cell().Element(CellStyle).Text($"Workpaper Ref: {details.WorkpaperRef}\nComments: {details.Comments}\nBy: {details.ConductedBy}\nOn: {details.ConductedOn}").FontSize(10);
                                        slNo++;
                                    }
                                    static IContainer CellStyle(IContainer container) =>
                                        container.Border(0.5f).PaddingVertical(3).PaddingHorizontal(4);
                                });
                            }

                            column.Item().PaddingBottom(10);

                            if (dtoCAO.Any() == true)
                            {
                                column.Item().AlignCenter().PaddingBottom(10).Text("Audit or Review - Testing Observation Details").FontSize(14).Bold();
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(0.5f);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("Sl No").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Check Point").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Observations").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Remarks By").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Client Remarks").FontSize(10).Bold();
                                    });

                                    foreach (var details in dtoCAO)
                                    {
                                        table.Cell().Element(CellStyle).Text(details.SrNo.ToString()).FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.CheckPoint.ToString()).FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.Observations.ToString()).FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.RemarksBy.ToString() + "(" + details.RemarksByRole + ")").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.ClientRemarks.ToString()).FontSize(10);
                                    }
                                    static IContainer CellStyle(IContainer container) => container.Border(0.5f).PaddingVertical(3).PaddingHorizontal(4);
                                });
                            }
                        });
                    });

                    foreach (var reportType in reportTypeList)
                    {
                        var dtoCAE = allDtoCAEs.ContainsKey(reportType.ID) ? allDtoCAEs[reportType.ID] : new List<CommunicationWithClientTemplateReportDetailsDTO>();
                        if (dtoCAE.Count > 0)
                        {
                            container.Page(page =>
                            {
                                page.Margin(30);
                                page.Size(PageSizes.A4);
                                page.PageColor(Colors.White);
                                page.DefaultTextStyle(x => x.FontSize(12));

                                page.Content().Column(column =>
                                {
                                    column.Item().AlignCenter().PaddingBottom(10).Text(reportType.Name).FontSize(16).Bold();
                                    column.Item().Text($"Ref.No.: {result.AuditNo}").FontSize(12).Bold();
                                    column.Item().Text($"Date: {dtoEP.CurrentDate:dd MMM yyyy}").FontSize(10);
                                    column.Item().Text(dtoEP.Customer ?? "N/A").FontSize(10);
                                    if (!string.IsNullOrWhiteSpace(address)) column.Item().Text(address).FontSize(10);
                                    if (!string.IsNullOrWhiteSpace(city)) column.Item().Text(city).FontSize(10);

                                    var statePin = string.Join(", ", new[] { state, pin }.Where(s => !string.IsNullOrWhiteSpace(s)));
                                    if (!string.IsNullOrWhiteSpace(statePin)) column.Item().Text(statePin).FontSize(10);

                                    if (!string.IsNullOrWhiteSpace(email)) column.Item().Text($"Email: {email}").FontSize(10);
                                    if (!string.IsNullOrWhiteSpace(telephone)) column.Item().Text($"Telephone: {telephone}").FontSize(10);

                                    column.Item().PaddingBottom(10);

                                    if (dtoCAE.Any())
                                    {
                                        int count = 1;
                                        foreach (var item in dtoCAE)
                                        {
                                            //column.Item().Text($"{count}. {item.LTD_Heading}").FontSize(11).Bold();
                                            if (!string.IsNullOrWhiteSpace(item.LTD_Heading) && !item.LTD_Heading.ToString().StartsWith("NH", StringComparison.OrdinalIgnoreCase))
                                                column.Item().Text($"{count}. {item.LTD_Heading}").FontSize(11).Bold();
                                            if (!string.IsNullOrWhiteSpace(item.LTD_Decription))
                                                column.Item().Text(item.LTD_Decription).FontSize(10);

                                            column.Item().PaddingBottom(5);
                                            count++;
                                        }
                                    }

                                    column.Item().PaddingTop(20).Text("Very truly yours,").FontSize(10);
                                    column.Item().Text(dtoEP.CompanyName ?? "[Company Name]").FontSize(10).Bold();
                                    column.Item().Text("Chartered Accountant").FontSize(10);
                                });
                            });
                        }
                    }

                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.Size(PageSizes.A4);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Content().Column(column =>
                        {
                            column.Item().AlignCenter().PaddingBottom(10).Text("Audit Issues and Closure").FontSize(16).Bold();
                            column.Item().Text(text =>
                            {
                                text.Span("Client Name: ").FontSize(10).Bold();
                                text.Span(dtoEP.Customer).FontSize(10);
                            });
                            column.Item().Text(text =>
                            {
                                text.Span("Audit No: ").FontSize(10).Bold();
                                text.Span(result.AuditNo).FontSize(10);
                            });


                            column.Item().PaddingBottom(10);

                            if (dtoCAM.Any() == true)
                            {
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(0.5f);
                                        columns.RelativeColumn(1.5f);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(1.5f);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1.5f);
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(3);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text(" ").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Workpaper Ref").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("CAM").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Exceeded Materiality").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Deviations/Exceptions Noted").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Conclusion").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Type of Test").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Status").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Description & Reason for selection as CAM").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Audit Procedure undertaken to address the CAM").FontSize(10).Bold();
                                    });

                                    int slNo = 1;
                                    foreach (var details in dtoCAM)
                                    {
                                        table.Cell().Element(CellStyle).Text(slNo.ToString()).FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.WorkpaperRef ?? "").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.CAM ?? "").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.ExceededMateriality ?? "").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.Observation ?? "").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.Conclusion ?? "").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.TypeOfTest ?? "").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.Status ?? "").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.DescriptionOrReasonForSelectionAsCAM ?? "").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.AuditProcedureUndertakenToAddressTheCAM ?? "").FontSize(10);
                                        slNo++;
                                    }

                                    static IContainer CellStyle(IContainer container) =>
                                        container.Border(0.5f).PaddingVertical(3).PaddingHorizontal(4);
                                });
                            }
                        });
                    });
                });
                using var ms = new MemoryStream();
                document.GeneratePdf(ms);
                return ms.ToArray();
            });
        }

        public async Task<StandardAuditAllAttachmentsDTO> LoadAllAuditAttachmentsByAuditIdAsync(int compId, int auditId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            try
            {
                var result = new StandardAuditAllAttachmentsDTO
                {
                    AuditPlanAttachments = new List<AttachmentGroupDTO>(),
                    BeginningAuditAttachments = new List<AttachmentGroupDTO>(),
                    NearEndAuditAttachments = new List<AttachmentGroupDTO>(),
                    DuringAuditAttachments = new List<AttachmentGroupDTO>(),
                    WorkpaperAttachments = new List<AttachmentGroupDTO>(),
                    ConductAuditAttachments = new List<AttachmentGroupDTO>()
                };

                // 1. Audit Plan/EngagementPlan
                var auditPlanTypes = await connection.QueryAsync<(int TypeId, string TypeName, string AttachIds)>(
                @"SELECT TOP 1 RTM.RTM_Id AS TypeId, RTM.RTM_ReportTypeName AS TypeName,
                STUFF((SELECT DISTINCT ',' + CAST(LOET2.LOE_AttachID AS VARCHAR) FROM StandardAudit_Schedule SA2
                           JOIN SAD_CUST_LOE LOE2 ON LOE2.LOE_CustomerId=SA2.SA_CustID AND LOE2.LOE_YearId=SA2.SA_YearID AND LOE2.LOE_ServiceTypeId=SA2.SA_AuditTypeID
                           JOIN LOE_Template LOET2 ON LOET2.LOET_LOEID=LOE2.LOE_ID WHERE SA2.SA_ID=@AuditID AND SA2.SA_CompID=@CompId AND LOET2.LOE_AttachID>0 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'') AS AttachIds
                  FROM StandardAudit_Schedule SA
                  JOIN SAD_CUST_LOE LOE ON LOE.LOE_CustomerId=SA.SA_CustID AND LOE.LOE_YearId=SA.SA_YearID AND LOE.LOE_ServiceTypeId=SA.SA_AuditTypeID
                  JOIN LOE_Template LOET ON LOET.LOET_LOEID=LOE.LOE_ID
                  JOIN LOE_Template_Details LTD ON LTD.LTD_LOE_ID=LOET.LOET_LOEID AND LTD.LTD_FormName='LOE' AND LTD.LTD_CompID=SA.SA_CompID
                  JOIN SAD_ReportTypeMaster RTM ON RTM.RTM_Id=LTD.LTD_ReportTypeID
                  WHERE SA.SA_ID=@AuditID AND SA.SA_CompID=@CompId",
                new { CompId = compId, AuditID = auditId });

                foreach (var item in auditPlanTypes)
                {
                    var attachments = await LoadAttachmentsByIdsAsync(item.AttachIds, compId, connection);
                    if (attachments.Any())
                        result.AuditPlanAttachments.Add(new AttachmentGroupDTO { TypeId = item.TypeId, TypeName = item.TypeName, Attachments = attachments });
                }

                // 2. Audit Schedule
                result.AuditScheduleAttachments.Add(new AttachmentGroupDTO { TypeId = 0, TypeName = "Audit Schedule Report", Attachments = null });

                // 3. Beginning of the Audit Communication
                var beginningTypes = await connection.QueryAsync<(int TypeId, string TypeName, string AttachIds)>(
                 @"SELECT RTM_Id AS TypeId, RTM_ReportTypeName AS TypeName,
                  STUFF((SELECT DISTINCT ',' + CAST(SAR_AttchId AS VARCHAR) FROM StandardAudit_Audit_DRLLog_RemarksHistory WHERE SAR_SA_ID=@AuditID AND SAR_ReportType=RTM_Id AND SAR_AttchId>0 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'') AS AttachIds
                  FROM SAD_ReportTypeMaster
                  WHERE RTM_CompID=@CompId AND RTM_TemplateId IN(2) AND RTM_DelFlag='A' AND EXISTS(SELECT 1 FROM StandardAudit_Audit_DRLLog_RemarksHistory H WHERE H.SAR_SA_ID=@AuditID AND H.SAR_ReportType=RTM_Id AND H.SAR_AttchId>0)",
                 new { CompId = compId, AuditID = auditId });

                foreach (var item in beginningTypes)
                {
                    var attachments = await LoadAttachmentsByIdsAsync(item.AttachIds, compId, connection);
                    if (attachments.Any())
                        result.BeginningAuditAttachments.Add(new AttachmentGroupDTO { TypeId = item.TypeId, TypeName = item.TypeName, Attachments = attachments });
                }

                // 4. During the Audit Requests
                var duringTypes = await connection.QueryAsync<(int TypeId, string TypeName, string AttachIds)>(
                @"SELECT CMM_ID AS TypeId, CMM_Desc AS TypeName,
                  STUFF((SELECT DISTINCT ',' + CAST(ADRL_AttachID AS VARCHAR) FROM Audit_DRLLog WHERE ADRL_AuditNo=@AuditID AND ADRL_RequestedListID=CMM_ID AND ADRL_AttachID>0 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'') AS AttachIds
                  FROM Content_Management_Master WHERE CMM_Category='DRL' AND CMS_Keycomponent=0 AND EXISTS(SELECT 1 FROM Audit_DRLLog WHERE ADRL_AuditNo=@AuditID AND ADRL_RequestedListID=CMM_ID AND ADRL_AttachID>0)",
                new { AuditID = auditId });

                foreach (var item in duringTypes)
                {
                    var attachments = await LoadAttachmentsByIdsAsync(item.AttachIds, compId, connection);
                    if (attachments.Any())
                        result.DuringAuditAttachments.Add(new AttachmentGroupDTO { TypeId = item.TypeId, TypeName = item.TypeName, Attachments = attachments });
                }

                // 5. Workpapers and Attachments
                var workpaperTypes = await connection.QueryAsync<(int TypeId, string TypeName, string AttachIds)>(
                @"SELECT wp.SSW_ID AS TypeId, wp.SSW_WorkpaperRef AS TypeName,
                  STUFF((SELECT DISTINCT ',' + CAST(SSW_AttachID AS VARCHAR) FROM StandardAudit_ScheduleConduct_WorkPaper WHERE SSW_SA_ID=@AuditID AND SSW_ID=wp.SSW_ID AND SSW_AttachID>0 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'') AS AttachIds
                  FROM StandardAudit_ScheduleConduct_WorkPaper wp WHERE SSW_SA_ID=@AuditID AND EXISTS(SELECT 1 FROM StandardAudit_ScheduleConduct_WorkPaper W WHERE W.SSW_SA_ID=@AuditID AND W.SSW_ID=wp.SSW_ID AND W.SSW_AttachID>0)",
                new { AuditID = auditId });

                foreach (var item in workpaperTypes)
                {
                    var attachments = await LoadAttachmentsByIdsAsync(item.AttachIds, compId, connection);
                    if (attachments.Any())
                        result.WorkpaperAttachments.Add(new AttachmentGroupDTO { TypeId = item.TypeId, TypeName = item.TypeName, Attachments = attachments });
                }

                // 6. Conduct Audit Checkpoint wise Testing details and Annexures
                var conductTypes = await connection.QueryAsync<(int TypeId, string TypeName, string AttachIds)>(
                @"SELECT DISTINCT ACM_ID AS TypeId, ACM_CheckPoint AS TypeName,
                  STUFF((SELECT DISTINCT ',' + CAST(SAC_AttachID AS VARCHAR) FROM StandardAudit_ScheduleCheckPointList WHERE SAC_SA_ID=@AuditID AND SAC_CheckPointID=cp.SAC_CheckPointID AND SAC_AttachID>0 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'') AS AttachIds
                  FROM StandardAudit_ScheduleCheckPointList cp
                  INNER JOIN AuditType_Checklist_Master ON ACM_ID=cp.SAC_CheckPointID
                  WHERE cp.SAC_SA_ID=@AuditID AND EXISTS(SELECT 1 FROM StandardAudit_ScheduleCheckPointList X WHERE X.SAC_SA_ID=@AuditID AND X.SAC_CheckPointID=cp.SAC_CheckPointID AND X.SAC_AttachID>0)",
                new { AuditID = auditId });

                foreach (var item in conductTypes)
                {
                    var attachments = await LoadAttachmentsByIdsAsync(item.AttachIds, compId, connection);
                    if (attachments.Any())
                        result.ConductAuditAttachments.Add(new AttachmentGroupDTO { TypeId = item.TypeId, TypeName = item.TypeName, Attachments = attachments });
                }

                // 7. Near End of the Audit
                var nearEndTypes = await connection.QueryAsync<(int TypeId, string TypeName, string AttachIds)>(
                 @"SELECT RTM_Id AS TypeId, RTM_ReportTypeName AS TypeName,
                  STUFF((SELECT DISTINCT ',' + CAST(SAR_AttchId AS VARCHAR) FROM StandardAudit_Audit_DRLLog_RemarksHistory WHERE SAR_SA_ID=@AuditID AND SAR_ReportType=RTM_Id AND SAR_AttchId>0 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'') AS AttachIds
                  FROM SAD_ReportTypeMaster
                  WHERE RTM_CompID=@CompId AND RTM_TemplateId IN(4) AND RTM_DelFlag='A' AND EXISTS(SELECT 1 FROM StandardAudit_Audit_DRLLog_RemarksHistory H WHERE H.SAR_SA_ID=@AuditID AND H.SAR_ReportType=RTM_Id AND H.SAR_AttchId>0)",
                 new { CompId = compId, AuditID = auditId });

                foreach (var item in nearEndTypes)
                {
                    var attachments = await LoadAttachmentsByIdsAsync(item.AttachIds, compId, connection);
                    if (attachments.Any())
                        result.NearEndAuditAttachments.Add(new AttachmentGroupDTO { TypeId = item.TypeId, TypeName = item.TypeName, Attachments = attachments });
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading all audit attachments.", ex);
            }
        }

        public async Task<List<AttachmentDetailsDTO>> LoadAttachmentsByIdsAsync(string ids, int compId, IDbConnection connection)
        {
            if (string.IsNullOrEmpty(ids) || ids == "0") return new();

            var query = @"SELECT A.ATCH_ID, A.ATCH_DOCID, A.ATCH_FNAME, A.ATCH_EXT, A.ATCH_DESC, A.ATCH_CREATEDBY, U.Usr_FullName AS ATCH_CREATEDBYNAME, A.ATCH_CREATEDON, A.ATCH_SIZE FROM edt_attachments A
                  LEFT JOIN Sad_Userdetails U ON A.ATCH_CREATEDBY = U.Usr_ID AND A.ATCH_COMPID = U.Usr_CompId
                  WHERE A.ATCH_CompID = @CompId AND A.ATCH_ID IN (SELECT value FROM STRING_SPLIT(@Ids, ',')) AND A.ATCH_Status <> 'D' ORDER BY A.ATCH_CREATEDON";

            var result = await connection.QueryAsync<AttachmentDetailsDTO>(query, new { CompId = compId, Ids = ids });
            return result.ToList();
        }

        private string SanitizeName(string name)
        {
            var invalidChars = System.IO.Path.GetInvalidFileNameChars().Concat(new[] { ',', ':', '"', '<', '>', '|', '*', '?' }).Distinct();
            foreach (char ch in invalidChars)
            {
                name = name.Replace(ch, '_');
            }
            name = name.TrimEnd(' ', '.');
            if (name.Length > 200)
            {
                name = name.Substring(0, 200);
            }
            return name;
        }

        private async Task<string> GenerateAuditeeInfoTempPathAsync(int compId, int auditId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryFirstOrDefaultAsync<(int EpPkId, int CustID, string YearName, string AuditNo)>(
                @"SELECT LOE.LOE_ID AS EpPkId, SA.SA_CustID As CustID, YMS.YMS_ID AS YearName, SA_AuditNo + ' - ' + CMA.CMM_Desc As AuditNo FROM StandardAudit_Schedule AS SA
                  LEFT JOIN SAD_CUST_LOE AS LOE ON LOE.LOE_CustomerId = SA.SA_CustID AND LOE.LOE_YearId = SA.SA_YearID AND LOE.LOE_ServiceTypeId = SA.SA_AuditTyPeId
                  LEFT JOIN YEAR_MASTER AS YMS ON YMS.YMS_YEARID = SA.SA_YearID
                  LEFT JOIN Content_Management_Master CMA On CMA.cmm_ID = SA.SA_AuditTypeID
                  WHERE LOE.LOE_CompID = @CompId AND SA.SA_ID = @AuditId;", new { CompId = compId, AuditId = auditId });

            EngagementPlanReportDetailsDTO dtoEP = await _engagementPlanInterface
                .GetEngagementPlanReportDetailsByIdAsync(compId, result.EpPkId);

            List<AuditReportCustInfoAuditeeDetailDTO> dtoAD = await GetAuditeeDetails(compId, result.CustID, result.YearName);

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;

            byte[] fileBytes;
            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Content().Column(column =>
                    {
                        column.Item().AlignCenter().PaddingBottom(10)
                              .Text("Profile/Information about the Auditee").FontSize(16).Bold();

                        if (dtoAD.Any())
                        {
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(0.5f);
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Sl No").FontSize(10).Bold();
                                    header.Cell().Element(CellStyle).Text("Particulars").FontSize(10).Bold();
                                    header.Cell().Element(CellStyle).Text("Details").FontSize(10).Bold();
                                });

                                int slNo = 1;
                                foreach (var details in dtoAD)
                                {
                                    table.Cell().Element(CellStyle).Text(slNo.ToString()).FontSize(10);
                                    table.Cell().Element(CellStyle).Text(details.Particulars ?? "-").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(details.Details ?? "-").FontSize(10);
                                    slNo++;
                                }

                                static IContainer CellStyle(IContainer container) =>
                                    container.Border(0.5f).PaddingVertical(3).PaddingHorizontal(4);
                            });
                        }

                        column.Item().PaddingTop(20).Text("Very truly yours,").FontSize(10);
                        column.Item().PaddingBottom(5).Text("For " + (dtoEP.CompanyName ?? "")).FontSize(10).Bold();
                        column.Item().PaddingBottom(5).Text("[Designation]").FontSize(10);
                        column.Item().PaddingBottom(5).Text("Place : ").FontSize(10);
                        column.Item().PaddingBottom(5).Text("Date :").FontSize(10);
                    });
                });
            });

            using (var ms = new MemoryStream())
            {
                document.GeneratePdf(ms);
                fileBytes = ms.ToArray();
            }

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"Information_About_Auditee.pdf";

            string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Tempfolder", compId.ToString());
            Directory.CreateDirectory(tempFolder);

            var filePath = Path.Combine(tempFolder, fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            await File.WriteAllBytesAsync(filePath, fileBytes);
            string downloadUrl = $"{tempFolder}/{fileName}";
            return downloadUrl;
        }

        public async Task<(bool, string)> DownloadAllAuditAttachmentsByAuditIdAsync(int compId, int auditId, int userId, string ipAddress)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                var result = await connection.QueryFirstOrDefaultAsync<(string SubCabinet, string CustCode, string CustName, string YearName, string UserName, DateTime DocumentExpiryDate, int ReminderDay)>(
                    @"Select SA_AuditNo + ' - ' + CMM.CMM_Desc, CUST_CODE, CUST_NAME, YMS_ID, usr_FullName, ISNULL(SA_ExpiryDate, DATEADD(YEAR, 7, GETDATE())) AS SA_ExpiryDate, ISNULL(SA_RetentionPeriod, 7) AS SA_RetentionPeriod
		            from StandardAudit_Schedule JOIN SAD_CUSTOMER_MASTER On CUST_ID=SA_CustID JOIN Year_Master On YMS_YEARID = SA_YearID Join Sad_Userdetails On Usr_Id = @UserId 
		            JOIN Content_Management_Master CMM ON CMM.CMM_ID = SA_AuditTypeID Where SA_ID= @AuditId;",
                    new { CompId = compId, AuditId = auditId, UserId = userId });

                String OrgName = result.CustName;
                String Cabinet = result.CustName + "_" + result.YearName;
                string downloadDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Tempfolder", compId.ToString(), SanitizeName(Cabinet));
 
                if (Directory.Exists(downloadDirectoryPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(downloadDirectoryPath);

                    foreach (FileInfo file in dir.GetFiles())
                    {
                        try { file.Delete(); } catch { continue; }
                    }

                    foreach (DirectoryInfo subDir in dir.GetDirectories())
                    {
                        try { subDir.Delete(true); } catch { continue; }
                    }
                }
                else
                {
                    Directory.CreateDirectory(downloadDirectoryPath);
                }               

                int orgNode = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(Org_Node, 0) FROM sad_org_structure WHERE Org_Name = @Org_Name AND Org_CompID = @Org_CompID;",
                    new { Org_Name = OrgName, Org_CompID = compId });
                if (orgNode == 0)
                {
                    orgNode = await connection.ExecuteScalarAsync<int>(@"DECLARE @Org_Node INT = (SELECT ISNULL(MAX(Org_Node), 0) + 1 FROM sad_org_structure WHERE Org_CompID = @Org_CompID);
		                INSERT INTO sad_org_structure (Org_Node, Org_Code, Org_Name, Org_Parent, Org_Delflag, Org_Note, Org_AppStrength, Org_CreatedBy, Org_CreatedOn, Org_Status, Org_LevelCode, Org_IPAddress, Org_CompID, Org_SalesUnitCode, Org_BranchCode) 
		                VALUES (@Org_Node, @Org_Code, @Org_Name, 3, 'A', @Org_Name, 0, @Org_CreatedBy, GETDATE(), 'A', 3, @Org_IPAddress, @Org_CompID, '', '');
		                SELECT @Org_Node;",
                        new { Org_Code = result.CustCode, Org_Name = OrgName, Org_CreatedBy = userId, Org_IPAddress = ipAddress, Org_CompID = compId });
                }

                int cabinetId = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(CBN_ID, 0) FROM edt_cabinet WHERE CBN_Name = @CBN_Name AND CBN_CompID = @CBN_CompID;",
                    new { CBN_Name = Cabinet, CBN_CompID = compId });
                if (cabinetId == 0)
                {
                    cabinetId = await connection.ExecuteScalarAsync<int>(@"DECLARE @CBN_ID INT = (SELECT ISNULL(MAX(CBN_ID), 0) + 1 FROM edt_cabinet);
		                INSERT INTO edt_cabinet (CBN_ID, CBN_NAME, CBN_Note, CBN_PARENT, CBN_UserID, CBN_Department, CBN_SubCabCount, CBN_FolderCount, CBN_Status, CBN_DelFlag, CBN_CreatedBy, CBN_CreatedOn, CBN_CompID, CBN_DocumentExpiryDate, CBN_ReminderDay, CBN_AuditID)
		                VALUES (@CBN_ID, @CBN_NAME, @CBN_NAME, -1, @CBN_UserID, 0, 0, 0, 'A', 'A', @CBN_CreatedBy, GETDATE(), @CBN_CompID, @CBN_DocumentExpiryDate, @CBN_ReminderDay, @AuditId);
		                SELECT @CBN_ID;",
                        new { CBN_NAME = Cabinet, CBN_UserID = userId, CBN_CreatedBy = userId, CBN_CompID = compId, CBN_DocumentExpiryDate = result.DocumentExpiryDate, CBN_ReminderDay = result.ReminderDay, AuditId = auditId });
                }

                int subCabinetId = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(CBN_ID, 0) FROM edt_cabinet WHERE CBN_Name = @Name AND CBN_Parent = @ParentId AND CBN_CompID = @CompId",
                    new { Name = result.SubCabinet, ParentId = cabinetId, CompId = compId });
                if (subCabinetId == 0)
                {
                    subCabinetId = await connection.ExecuteScalarAsync<int>(@"DECLARE @NewId INT = (SELECT ISNULL(MAX(CBN_ID), 0) + 1 FROM edt_cabinet);
		                INSERT INTO edt_cabinet (CBN_ID, CBN_NAME, CBN_NOTE, CBN_PARENT, CBN_UserID, CBN_Department, CBN_SubCabCount, CBN_FolderCount, CBN_Status, CBN_DelFlag, CBN_CreatedBy, CBN_CreatedOn, CBN_CompID)
		                VALUES (@NewId, @Name, @Name, @ParentId, @UserId, 0, 0, 0, 'A', 'A', @UserId, GETDATE(), @CompId);
		                SELECT @NewId;",
                        new { Name = result.SubCabinet, ParentId = cabinetId, UserId = userId, CompId = compId });

                    await connection.ExecuteAsync(@"UPDATE edt_cabinet SET CBN_SubCabCount = (SELECT COUNT(*) FROM edt_cabinet WHERE CBN_Parent = @ParentId AND CBN_DelFlag = 'A') WHERE CBN_ID = @ParentId AND CBN_CompID = @CompId",
                        new { ParentId = cabinetId, CompId = compId });

                    await connection.ExecuteAsync(@"UPDATE edt_cabinet SET CBN_FolderCount = ( SELECT COUNT(*) FROM edt_folder WHERE FOL_CABINET IN ( SELECT CBN_ID FROM edt_cabinet WHERE CBN_Parent = @ParentId AND CBN_DelFlag = 'A')) WHERE CBN_ID = @ParentId AND CBN_CompID = @CompId",
                        new { ParentId = cabinetId, CompId = compId });
                }

                String mainFolder = SanitizeName(result.SubCabinet);

                // 1. Audit Plan
                var auditPlanTypes =
                "SELECT TOP 1 LOE_ID AS LOEID, RTM.RTM_Id AS TypeId,'LOE - ' + RTM.RTM_ReportTypeName AS TypeName," +
                " STUFF((SELECT ',' + CAST(LOET2.LOE_AttachID AS VARCHAR) FROM StandardAudit_Schedule SA2" +
                " JOIN SAD_CUST_LOE LOE2 ON LOE2.LOE_CustomerId=SA2.SA_CustID AND LOE2.LOE_YearId=SA2.SA_YearID AND LOE2.LOE_ServiceTypeId=SA2.SA_AuditTypeID" +
                " JOIN LOE_Template LOET2 ON LOET2.LOET_LOEID=LOE2.LOE_ID" +
                " WHERE SA2.SA_ID=" + auditId + " AND SA2.SA_CompID=" + compId + " AND LOET2.LOE_AttachID>0" +
                " FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'') AttachIds" +
                " FROM StandardAudit_Schedule SA" +
                " JOIN SAD_CUST_LOE LOE ON LOE.LOE_CustomerId=SA.SA_CustID AND LOE.LOE_YearId=SA.SA_YearID AND LOE.LOE_ServiceTypeId=SA.SA_AuditTypeID" +
                " JOIN LOE_Template LOET ON LOET.LOET_LOEID=LOE.LOE_ID" +
                " JOIN LOE_Template_Details LTD ON LTD.LTD_LOE_ID=LOET.LOET_LOEID AND LTD.LTD_FormName='LOE' AND LTD.LTD_CompID=SA.SA_CompID" +
                " JOIN SAD_ReportTypeMaster RTM ON RTM.RTM_Id=LTD.LTD_ReportTypeID" +
                " WHERE SA.SA_ID=" + auditId + " AND SA.SA_CompID=" + compId +
                " AND EXISTS(SELECT 1 FROM LOE_Template t2 WHERE t2.LOET_LOEID=LOE.LOE_ID AND t2.LOE_AttachID>0)";

                // 3. Beginning of the Audit Communication
                var beginningTypes =
                "SELECT RTM_Id AS TypeId,1 As CheckReportType,'Beginning of the Audit - ' + RTM_ReportTypeName AS TypeName," +
                " STUFF((SELECT DISTINCT ',' + CAST(SAR_AttchId AS VARCHAR) FROM StandardAudit_Audit_DRLLog_RemarksHistory" +
                " WHERE SAR_SA_ID=" + auditId + " AND SAR_ReportType=RTM_Id AND SAR_AttchId>0 FOR XML PATH('')),1,1,'') AS AttachIds" +
                " FROM SAD_ReportTypeMaster" +
                " WHERE RTM_CompID=" + compId + " AND RTM_TemplateId=2 AND RTM_DelFlag='A'" +
                " AND EXISTS(SELECT 1 FROM StandardAudit_Audit_DRLLog_RemarksHistory WHERE SAR_SA_ID=" + auditId + " AND SAR_ReportType=RTM_Id AND SAR_AttchId>0)";

                // 4. During the Audit Requests
                var duringTypes =
                "SELECT CMM_ID AS TypeId,0 As CheckReportType,'DRL - ' + CMM_Desc AS TypeName," +
                " STUFF((SELECT ',' + CAST(ADRL_AttachID AS VARCHAR) FROM Audit_DRLLog" +
                " WHERE ADRL_AuditNo=" + auditId + " AND ADRL_RequestedListID=CMM_ID AND ADRL_AttachID>0 FOR XML PATH('')),1,1,'') AS AttachIds" +
                " FROM Content_Management_Master" +
                " WHERE CMM_Category='DRL' AND CMS_Keycomponent=0" +
                " AND EXISTS(SELECT 1 FROM Audit_DRLLog WHERE ADRL_AuditNo=" + auditId + " AND ADRL_RequestedListID=CMM_ID AND ADRL_AttachID>0)";

                // 5. Workpapers and Attachments
                var workpaperTypes =
                "SELECT wp.SSW_ID AS TypeId,0 As CheckReportType,'WP - ' + wp.SSW_WorkpaperRef AS TypeName," +
                " STUFF((SELECT ',' + CAST(SSW_AttachID AS VARCHAR) FROM StandardAudit_ScheduleConduct_WorkPaper" +
                " WHERE SSW_SA_ID=" + auditId + " AND SSW_ID=wp.SSW_ID AND SSW_AttachID>0 FOR XML PATH('')),1,1,'') AS AttachIds" +
                " FROM StandardAudit_ScheduleConduct_WorkPaper wp" +
                " WHERE SSW_SA_ID=" + auditId +
                " AND EXISTS(SELECT 1 FROM StandardAudit_ScheduleConduct_WorkPaper WHERE SSW_SA_ID=" + auditId + " AND SSW_ID=wp.SSW_ID AND SSW_AttachID>0)";

                // 6. Conduct Audit Checkpoint wise Testing details and Annexures
                var conductTypes =
                "SELECT DISTINCT ACM_ID AS TypeId,0 As CheckReportType,CAST('CheckPoint_' + CAST(ACM_ID AS VARCHAR(10)) + ' - ' + CAST(ACM_Heading AS VARCHAR(MAX)) AS VARCHAR(MAX)) AS TypeName," +
                " STUFF((SELECT ',' + CAST(SAC_AttachID AS VARCHAR) FROM StandardAudit_ScheduleCheckPointList" +
                " WHERE SAC_SA_ID=" + auditId + " AND SAC_CheckPointID=cp.SAC_CheckPointID AND SAC_AttachID>0 FOR XML PATH('')),1,1,'') AS AttachIds" +
                " FROM StandardAudit_ScheduleCheckPointList cp" +
                " INNER JOIN AuditType_Checklist_Master ON ACM_ID=cp.SAC_CheckPointID" +
                " WHERE cp.SAC_SA_ID=" + auditId +
                " AND EXISTS(SELECT 1 FROM StandardAudit_ScheduleCheckPointList WHERE SAC_SA_ID=" + auditId + " AND SAC_CheckPointID=cp.SAC_CheckPointID AND SAC_AttachID>0)";

                // 7. Near End of the Audit
                var nearingEndTypes =
                "SELECT RTM_Id AS TypeId,1 As CheckReportType,'Near end of the Audit - ' + RTM_ReportTypeName AS TypeName," +
                " STUFF((SELECT DISTINCT ',' + CAST(SAR_AttchId AS VARCHAR) FROM StandardAudit_Audit_DRLLog_RemarksHistory" +
                " WHERE SAR_SA_ID=" + auditId + " AND SAR_ReportType=RTM_Id AND SAR_AttchId>0 FOR XML PATH('')),1,1,'') AS AttachIds" +
                " FROM SAD_ReportTypeMaster" +
                " WHERE RTM_CompID=" + compId + " AND RTM_TemplateId=4 AND RTM_DelFlag='A'" +
                " AND EXISTS(SELECT 1 FROM StandardAudit_Audit_DRLLog_RemarksHistory WHERE SAR_SA_ID=" + auditId + " AND SAR_ReportType=RTM_Id AND SAR_AttchId>0)";

                // 8. Audit Completion Checkpoint Reports

                // 9. Account Finalisation Reports

                await ProcessGenericAttachmentsAsync(connection, compId, downloadDirectoryPath, "StandardAudit", mainFolder, auditId, userId, cabinetId, subCabinetId, ipAddress, auditPlanTypes);
                //@"SELECT DISTINCT ISNULL(CAST(SAR_AttchId AS VARCHAR), '0') AS SAR_AttchId FROM StandardAudit_Audit_DRLLog_RemarksHistory WHERE SAR_SA_ID = " + auditId + " AND SAR_ReportType IN (Select RTM_Id from SAD_ReportTypeMaster where RTM_CompID = " + compId + " And RTM_TemplateId = 2) And RTM_DelFlag='A'",
                //folderNameField: null, attachIdField: "SAR_AttchId");

                await ProcessGenericAttachmentsAsync(connection, compId, downloadDirectoryPath, "SamplingCU", mainFolder, auditId, userId, cabinetId, subCabinetId, ipAddress, beginningTypes);
                //@"SELECT DISTINCT ISNULL(CMM_DESC, 'Audit') AS FolderName, ISNULL(CAST(ADRL_AttachID AS VARCHAR), '0') AS ADRL_AttachID FROM Audit_DRLLog LEFT JOIN Content_Management_Master ON ADRL_RequestedListID = CMM_ID And cmm_Category = 'DRL' WHERE ADRL_AuditNo = " + auditId,
                //folderNameField: "FolderName", attachIdField: "ADRL_AttachID");

                await ProcessGenericAttachmentsAsync(connection, compId, downloadDirectoryPath, "SamplingCU", mainFolder, auditId, userId, cabinetId, subCabinetId, ipAddress, duringTypes);
                //@"SELECT DISTINCT ISNULL(CAST(SAR_AttchId AS VARCHAR), '0') AS SAR_AttchId FROM StandardAudit_Audit_DRLLog_RemarksHistory WHERE SAR_SA_ID = " + auditId + " AND SAR_ReportType IN (Select RTM_Id from SAD_ReportTypeMaster where RTM_CompID = " + compId + " And RTM_TemplateId = 4) And RTM_DelFlag='A'",
                //folderNameField: null, attachIdField: "SAR_AttchId");

                await ProcessGenericAttachmentsAsync(connection, compId, downloadDirectoryPath, "SamplingCU", mainFolder, auditId, userId, cabinetId, subCabinetId, ipAddress, nearingEndTypes);
                //@"SELECT DISTINCT ISNULL(CMM_DESC, 'Audit') AS FolderName, ISNULL(CAST(ADRL_AttachID AS VARCHAR), '0') AS ADRL_AttachID FROM Audit_DRLLog LEFT JOIN Content_Management_Master ON ADRL_RequestedListID = CMM_ID And cmm_Category = 'DRL' WHERE ADRL_AuditNo = " + auditId,
                //folderNameField: "FolderName", attachIdField: "ADRL_AttachID");

                await ProcessGenericAttachmentsAsync(connection, compId, downloadDirectoryPath, "StandardAudit", mainFolder, auditId, userId, cabinetId, subCabinetId, ipAddress, workpaperTypes);
                //@"SELECT SSW_WorkpaperNo, ISNULL(CAST(SSW_AttachID AS VARCHAR), '0') AS SSW_AttachID FROM StandardAudit_ScheduleConduct_WorkPaper WHERE SSW_SA_ID = " + auditId,
                //folderNameField: "SSW_WorkpaperNo", attachIdField: "SSW_AttachID");

                await ProcessGenericAttachmentsAsync(connection, compId, downloadDirectoryPath, "StandardAudit", mainFolder, auditId, userId, cabinetId, subCabinetId, ipAddress, conductTypes);
                //@"SELECT DISTINCT ISNULL(CAST(SAC_AttachID AS VARCHAR), '0') AS SAC_AttachID FROM StandardAudit_ScheduleCheckPointList WHERE SAC_SA_ID = " + auditId,
                //folderNameField: null, attachIdField: "SAC_AttachID");


                // Audit Plan/EngagementPlan Report
                var dt = await GetDataTableAsync(connection, auditPlanTypes);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    string folderName = row["TypeName"] == null ? "StandardAudit" : row["TypeName"]?.ToString() ?? "StandardAudit";
                    int epPKid = row["LOEID"] == DBNull.Value ? 0 : Convert.ToInt32(row["LOEID"]);
                    string folderPath = Path.Combine(downloadDirectoryPath, mainFolder, SanitizeName(folderName));
                    var savedAudiPlanFilePath = await _engagementPlanInterface.GenerateReportAndGetTempPathAsync(compId, epPKid, "pdf");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    await ProcessReportAttachmentsAsync(connection, compId, downloadDirectoryPath, mainFolder, userId, cabinetId, subCabinetId, folderName, "LOE_Final.pdf", savedAudiPlanFilePath);
                }

                // Information about the Auditee Report
                var savedAuditeeFilePath = await GenerateAuditeeInfoTempPathAsync(compId, auditId);
                await ProcessReportAttachmentsAsync(connection, compId, downloadDirectoryPath, mainFolder, userId, cabinetId, subCabinetId, "Information about the Auditee", "Information_About_Auditee_Report.pdf", savedAuditeeFilePath);

                // Audit Completion SubPoint Report
                var savedAuditCompletionSubPointFilePath = await GenerateACSubPointsReportAndGetTempPathAsync(compId, auditId, userId, "pdf");
                await ProcessReportAttachmentsAsync(connection, compId, downloadDirectoryPath, mainFolder, userId, cabinetId, subCabinetId, "Audit Completion", "Audit_Completion_SubPoint_Report.pdf", savedAuditCompletionSubPointFilePath);

                // Audit Completion Report
                var savedAuditCompletionFilePath = await GenerateReportAndGetTempPathAsync(compId, auditId, "pdf");
                await ProcessReportAttachmentsAsync(connection, compId, downloadDirectoryPath, mainFolder, userId, cabinetId, subCabinetId, "Audit Completion", "Audit_Completion_Report.pdf", savedAuditCompletionFilePath);

                string cleanedPath = downloadDirectoryPath.TrimEnd('\\');
                string zipFilePath = cleanedPath + ".zip";

                if (File.Exists(zipFilePath))
                    File.Delete(zipFilePath);

                ZipFile.CreateFromDirectory(cleanedPath, zipFilePath);

                var request = _httpContextAccessor.HttpContext.Request;
                string baseUrl = $"{request.Scheme}://{request.Host}";
                string downloadUrl = $"{baseUrl}/Tempfolder/{compId}/{SanitizeName(Cabinet) + ".zip"}";

                return (true, downloadUrl);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to upload the attachment document.", ex);
            }
        }

        private async Task<DataTable> GetDataTableAsync(SqlConnection connection, string query)
        {
            try
            {
                using var cmd = new SqlCommand(query, connection);
                using var reader = await cmd.ExecuteReaderAsync();
                var dt = new DataTable();
                dt.Load(reader);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to Get attachment data Table details.", ex);
            }
        }

        public async Task ProcessGenericAttachmentsAsync(SqlConnection connection, int compId, string downloadDirectoryPath, string module, string mainFolder, int auditId, int userId, int cabinetId, int subCabinetId, string ipAddress, string query)
        {
            try
            {
                var dt = await GetDataTableAsync(connection, query);

                foreach (DataRow row in dt.Rows)
                {
                    bool folderExists = true;
                    int checkReportType = row["CheckReportType"] == DBNull.Value ? 0 : Convert.ToInt32(row["CheckReportType"]);
                    int typeId = checkReportType == 1 ? Convert.ToInt32(row["TypeId"]) : 0;
                    string folderName = row["TypeName"] == null ? "StandardAudit" : row["TypeName"]?.ToString() ?? "StandardAudit";
                    string folderPath = System.IO.Path.Combine(downloadDirectoryPath, mainFolder, SanitizeName(folderName));

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    int folderId = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(FOL_FOLID, 0) FROM edt_folder WHERE FOL_NAME = @Name AND FOL_CABINET = @SubCabinetId AND FOL_CompID = @CompId", new { Name = folderName, SubCabinetId = subCabinetId, CompId = compId });
                    if (folderId == 0)
                    {
                        folderExists = false;

                        folderId = await connection.ExecuteScalarAsync<int>(@"DECLARE @NewFolId INT = (SELECT ISNULL(MAX(FOL_FOLID), 0) + 1 FROM edt_folder);
                          INSERT INTO edt_folder (FOL_FOLID, FOL_NAME, FOL_NOTE, FOL_CABINET, FOL_CREATEDBY, FOL_CREATEDON, FOL_STATUS, FOL_DELFLAG, FOL_COMPID)
                          VALUES (@NewFolId, @Name, @Name, @SubCabinet, @UserId, GETDATE(), 'A', 'A', @CompId);

                          UPDATE edt_cabinet SET CBN_FolderCount = (SELECT COUNT(FOL_FOLID) FROM edt_folder WHERE FOL_CABINET IN (SELECT CBN_ID FROM edt_cabinet WHERE CBN_PARENT = @Cabinet AND CBN_DELFLAG = 'A')) 
                          WHERE CBN_ID = @Cabinet AND CBN_CompID = @CompId;

                          UPDATE edt_cabinet SET CBN_FolderCount = (SELECT COUNT(FOL_FOLID) FROM edt_folder WHERE FOL_CABINET = @SubCabinet AND FOL_DELFLAG = 'A') 
                          WHERE CBN_ID = @SubCabinet AND CBN_CompID = @CompId;

                          SELECT @NewFolId;", new { Name = folderName, Cabinet = cabinetId, SubCabinet = subCabinetId, UserId = userId, CompId = compId });
                    }

                    string attachIds = row["AttachIds"]?.ToString() ?? "0";
                    var uniqueAttachIds = attachIds.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(id => int.TryParse(id, out var num) ? num : 0).Where(id => id > 0).Distinct();
                    foreach (var attachId in uniqueAttachIds)
                    {
                        if (attachId > 0)
                        {
                            await HandleFileProcessingAsync(connection, compId, userId, cabinetId, subCabinetId, folderId, module, folderPath, attachId.ToString(), folderExists, typeId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to Process folder & attachment.", ex);
            }
        }

        public async Task ProcessReportAttachmentsAsync(SqlConnection connection, int compId, string downloadDirectoryPath, string mainFolder, int userId, int cabinetId, int subCabinetId, string folderName, string fileName, string savedFilePath)
        {
            try
            {
                bool folderExists = true;
                string folderPath = System.IO.Path.Combine(downloadDirectoryPath, mainFolder, SanitizeName(folderName));

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                int folderId = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(FOL_FOLID, 0) FROM edt_folder WHERE FOL_NAME = @Name AND FOL_CABINET = @SubCabinetId AND FOL_CompID = @CompId", new { Name = folderName, SubCabinetId = subCabinetId, CompId = compId });
                if (folderId == 0)
                {
                    folderExists = false;

                    folderId = await connection.ExecuteScalarAsync<int>(@"DECLARE @NewFolId INT = (SELECT ISNULL(MAX(FOL_FOLID), 0) + 1 FROM edt_folder);
                          INSERT INTO edt_folder (FOL_FOLID, FOL_NAME, FOL_NOTE, FOL_CABINET, FOL_CREATEDBY, FOL_CREATEDON, FOL_STATUS, FOL_DELFLAG, FOL_COMPID)
                          VALUES (@NewFolId, @Name, @Name, @SubCabinet, @UserId, GETDATE(), 'A', 'A', @CompId);

                          UPDATE edt_cabinet SET CBN_FolderCount = (SELECT COUNT(FOL_FOLID) FROM edt_folder WHERE FOL_CABINET IN (SELECT CBN_ID FROM edt_cabinet WHERE CBN_PARENT = @Cabinet AND CBN_DELFLAG = 'A')) 
                          WHERE CBN_ID = @Cabinet AND CBN_CompID = @CompId;

                          UPDATE edt_cabinet SET CBN_FolderCount = (SELECT COUNT(FOL_FOLID) FROM edt_folder WHERE FOL_CABINET = @SubCabinet AND FOL_DELFLAG = 'A') 
                          WHERE CBN_ID = @SubCabinet AND CBN_CompID = @CompId;

                          SELECT @NewFolId;", new { Name = folderName, Cabinet = cabinetId, SubCabinet = subCabinetId, UserId = userId, CompId = compId });
                }

                string destFilePath = System.IO.Path.Combine(folderPath, fileName);

                if (File.Exists(savedFilePath))
                {
                    try
                    {
                        FileDecrypt(savedFilePath, destFilePath);
                    }
                    catch
                    {
                        File.Copy(savedFilePath, destFilePath, true);
                    }
                }

                if (!folderExists)
                    IndexingFileAsync(connection, cabinetId, subCabinetId, folderId, userId, compId, destFilePath, "FALSE");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to Process folder & attachment.", ex);
            }
        }

        public string GetTRACeConfigValue(string key)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT SAD_Config_Value FROM [dbo].[Sad_Config_Settings] WHERE SAD_Config_Key = @Key";
                return connection.QueryFirstOrDefault<string>(query, new { Key = key });
            }
        }

        public async Task HandleFileProcessingAsync(SqlConnection connection, int compId, int userId, int cabinetId, int subCabinetId, int folderId, string module, string folderPath, object attachId, bool shouldIndexFile, int typeId)
        {
            try
            {
                string isBlobData = (await connection.ExecuteScalarAsync<string>(@"SELECT Sad_Config_Value FROM Sad_Config_Settings WHERE Sad_Config_Key = 'FilesInDB' AND Sad_CompID = @CompId", new { CompId = compId }));

                var attachments = new List<AttachmentDetailsDTO>();
                var query = @"SELECT A.ATCH_ID, A.ATCH_DOCID, A.ATCH_FNAME, A.ATCH_EXT, A.ATCH_DESC, A.ATCH_CREATEDBY, U.Usr_FullName AS ATCH_CREATEDBYNAME, A.ATCH_CREATEDON, A.ATCH_SIZE FROM edt_attachments A 
                LEFT JOIN Sad_Userdetails U ON A.ATCH_CREATEDBY = U.Usr_ID AND A.ATCH_COMPID = U.Usr_CompId WHERE A.ATCH_CompID = @CompId AND A.ATCH_ID = @AttachId AND A.ATCH_Status <> 'D' AND 
                (@CheckReportType <= 0 OR A.ATCH_ReportType = @CheckReportType) ORDER BY A.ATCH_CREATEDON;";
                attachments = (await connection.QueryAsync<AttachmentDetailsDTO>(query, new { CompId = compId, AttachId = attachId, CheckReportType = typeId })).ToList();

                foreach (var item in attachments)
                {
                    string fileName = $"{item.ATCH_FNAME}.{item.ATCH_EXT}";
                    string destFilePath = System.IO.Path.Combine(folderPath, fileName);

                    if (File.Exists(destFilePath))
                        File.Delete(destFilePath);

                    if (isBlobData.ToUpper() == "TRUE")
                    {
                        byte[] blobData = (await connection.QueryAsync<byte[]>(@"SELECT ATCH_ole FROM EDT_ATTACHMENTS WHERE ATCH_ID = @AttachId AND ATCH_CompID = @CompId", new { AttachId = attachId, CompId = compId })).FirstOrDefault();
                        if (blobData != null)
                        {
                            await File.WriteAllBytesAsync(destFilePath, blobData);
                        }
                    }
                    else
                    {
                        string fileExt = string.IsNullOrWhiteSpace(item.ATCH_EXT) ? ".unk" : (item.ATCH_EXT.StartsWith(".") ? item.ATCH_EXT.ToLower() : "." + item.ATCH_EXT.ToLower());

                        // Determine file type
                        string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tif", ".tiff", ".svg", ".psd", ".ai", ".eps", ".ico", ".webp", ".raw", ".heic", ".heif", ".exr", ".dng", ".jp2", ".j2k", ".cr2", ".nef", ".orf", ".arw", ".raf", ".rw2", ".mp4", ".avi", ".mov", ".wmv", ".mkv", ".flv", ".webm", ".m4v", ".mpg", ".mpeg", ".3gp", ".ts", ".m2ts", ".vob", ".mts", ".divx", ".ogv" };
                        string[] documentExtensions = { ".pdf", ".doc", ".docx", ".txt", ".xls", ".xlsx", ".ppt", ".ppsx", ".pptx", ".odt", ".ods", ".odp", ".rtf", ".csv", ".pptm", ".xlsm", ".docm", ".xml", ".json", ".yaml", ".key", ".numbers", ".pages", ".tar", ".zip", ".rar" };
                        string fileType = imageExtensions.Contains(fileExt) ? "Images" : documentExtensions.Contains(fileExt) ? "Documents" : "Others";

                        // Build source file path
                        string basePath = GetTRACeConfigValue("ImgPath"); // Or Directory.GetCurrentDirectory()
                        string folderChunk = (item.ATCH_DOCID / 301).ToString();
                        string savedFilePath = Path.Combine(basePath, module, fileType, folderChunk, $"{item.ATCH_DOCID}{fileExt}");

                        if (File.Exists(savedFilePath))
                        {
                            try
                            {
                                FileDecrypt(savedFilePath, destFilePath);
                            }
                            catch
                            {
                                File.Copy(savedFilePath, destFilePath, true);
                            }
                        }


						await connection.ExecuteAsync(
							 @"UPDATE edt_attachments SET Atch_FolderId = @Atch_FolderId, Atch_Path = @Atch_Path 
                                WHERE ATCH_ID = @ATCH_ID AND Atch_DocID = @Atch_DocID",
							new
							{
								Atch_FolderId = folderId,
								Atch_Path = savedFilePath,
								ATCH_ID = item.ATCH_ID,
								Atch_DocID = item.ATCH_DOCID
							});
					}

                    if (!shouldIndexFile)
                        IndexingFileAsync(connection, cabinetId, subCabinetId, folderId, userId, compId, destFilePath, isBlobData);
                     
				}
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to Handle File Processing details.", ex);
            }
        }

        public async void IndexingFileAsync(SqlConnection connection, int cabinetId,  int subCabinetId, int folderId, int userId, int compId, string filePath, string isBlobData)
        {
            try
            {
                string fileName = System.IO.Path.GetFileName(filePath);
                string fileExt = System.IO.Path.GetExtension(filePath)?.TrimStart('.') ?? "";
                string objectType = GetObjectType(fileExt);

                int newBaseName = await connection.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(PGE_BASENAME), 0) + 1 FROM EDT_PAGE");
                int newPageNo = await connection.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(PGE_PAGENO), 0) + 1 FROM EDT_PAGE");

                await connection.ExecuteAsync(
                    @"INSERT INTO EDT_PAGE (PGE_BASENAME, PGE_CABINET, PGE_FOLDER, PGE_DOCUMENT_TYPE, PGE_TITLE, PGE_DATE, PGE_DETAILS_ID, PGE_CreatedBy, PGE_CreatedOn, PGE_OBJECT, PGE_PAGENO, PGE_EXT, PGE_KEYWORD, PGE_OCRText, 
                    PGE_SIZE, PGE_CURRENT_VER, PGE_STATUS, PGE_SubCabinet, PGE_QC_UsrGrpId, PGE_FTPStatus, PGE_batch_name, PGE_OrignalFileName, PGE_BatchID, PGE_OCRDelFlag, PGE_CompID, pge_Delflag, PGE_RFID) VALUES (
                    @BaseName, @CabinetId, @FolderId, 0, @Title, GETDATE(), @DetailsId, @CreatedBy, GETDATE(), @Object, @PageNo, @Ext, '', '', 0, 0, 'A', @SubCabinetId, 0, 'F', @BatchName, @OriginalFileName, 0, 0, @CompId, 'A', '')",
                    new
                    {
                        BaseName = newBaseName,
                        CabinetId = cabinetId,
                        FolderId = folderId,
                        Title = fileName,
                        DetailsId = newBaseName,
                        CreatedBy = userId,
                        Object = objectType,
                        PageNo = newPageNo,
                        Ext = fileExt,
                        SubCabinetId = subCabinetId,
                        BatchName = newBaseName,
                        OriginalFileName = fileName,
                        CompId = compId
                    }
                );

                string fileGeneratedPath = await Urlenp(connection, compId, userId, newBaseName, filePath);
                if (isBlobData.ToUpper() == "TRUE")
                {
                    FilePageInEdictAsync(connection, compId, newBaseName, fileGeneratedPath);
                }
            }
            catch (Exception ex)
            {                
            }
        }

        private string GetObjectType(string ext)
        {
            string[] imageExtensions = new[] { "TIF", "TIFF", "JPG", "JPEG", "BMP", "BRK", "CAL", "CLP", "DCX", "EPS", "ICO", "IFF", "IMT", "ICA", "PCT", "PCX", "PNG", "PSD", "RAS", "SGI", "TGA", "XBM", "XPM", "XWD" };
            return imageExtensions.Contains(ext.ToUpper()) ? "IMAGE" : "OLE";
        }

        public async Task<string> Urlenp(SqlConnection connection, int compId, int userId, long baseName, string filePath)
        {
            try
            {
                string baseImgPath = await connection.ExecuteScalarAsync<string>(@"SELECT SAD_Config_Value FROM sad_config_settings WHERE SAD_Config_Key = 'ImgPath' AND SAD_CompID = @CompId", new { CompId = compId });
                string tempFolder = System.IO.Path.Combine(baseImgPath, "Tempfolder", "UrlUpload", userId.ToString());
                Directory.CreateDirectory(tempFolder);

                string extension = System.IO.Path.GetExtension(filePath);
                string tempDownloadedPath = System.IO.Path.Combine(tempFolder, $"{baseName}_ed{extension}");
                string encryptedFilePath = System.IO.Path.Combine(tempFolder, $"{baseName}{extension}");

                using (var client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(new Uri(filePath), tempDownloadedPath);
                }

                FileEncrypt(tempDownloadedPath, encryptedFilePath);
                return encryptedFilePath;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async void FilePageInEdictAsync(SqlConnection connection, int compId, long baseName, string filePath)
        {
            string configPath = await connection.ExecuteScalarAsync<string>("SELECT SAD_Config_Value FROM sad_config_settings WHERE SAD_Config_Key = 'ImgPath' AND SAD_CompID = @CompId", new { CompId = compId });
            if (string.IsNullOrEmpty(configPath))
                return;

            string imageFolder = System.IO.Path.Combine(configPath, "BITMAPS", (baseName / 301).ToString());

            if (!Directory.Exists(imageFolder))
                Directory.CreateDirectory(imageFolder);

            string extension = System.IO.Path.GetExtension(filePath);
            string destinationPath = System.IO.Path.Combine(imageFolder, $"{baseName}{extension}");

            if (!File.Exists(destinationPath))
            {
                File.Copy(filePath, destinationPath);
                return;
            }
        }

        public void FileEncrypt(string inputFilePath, string outputFilePath)
        {
            string encryptionKey = "MAKV2SPBNI99212";
            byte[] salt = new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };

            using (Aes aes = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptionKey, salt);
                aes.Key = pdb.GetBytes(32);
                aes.IV = pdb.GetBytes(16);

                using (FileStream fsOutput = new FileStream(outputFilePath, FileMode.Create))
                using (CryptoStream cs = new CryptoStream(fsOutput, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (FileStream fsInput = new FileStream(inputFilePath, FileMode.Open))
                {
                    int data;
                    while ((data = fsInput.ReadByte()) != -1)
                    {
                        cs.WriteByte((byte)data);
                    }
                }
            }

            File.Delete(inputFilePath);
        }

        public static void FileDecrypt(string inputFilePath, string outputFilePath)
        {
            string encryptionKey = "MAKV2SPBNI99212";
            byte[] salt = new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };

            using (Aes aes = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptionKey, salt);
                aes.Key = pdb.GetBytes(32);
                aes.IV = pdb.GetBytes(16);

                using (FileStream fsInput = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                using (CryptoStream cs = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (FileStream fsOutput = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                {
                    int data;
                    while ((data = cs.ReadByte()) != -1)
                    {
                        fsOutput.WriteByte((byte)data);
                    }
                }
            }
        }

        public async Task<int> UpdateArchiveInAuditAsync(AuditArchiveDTO dto)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"UPDATE StandardAudit_Schedule SET SA_RetentionPeriod = @RetentionPeriod, SA_ExpiryDate = DATEADD(YEAR, @RetentionPeriod, @ExpiryDate), SA_ForCompleteAudit = @ForCompleteAudit, SA_IsArchived = 1 WHERE SA_ID = @AuditID AND SA_CompID = @ACID";

                var parameters = new { RetentionPeriod = dto.SA_RetentionPeriod, ExpiryDate = dto.SA_ExpiryDate, ForCompleteAudit = dto.SA_ForCompleteAudit, AuditID = dto.SA_ID, ACID = dto.SA_CompID };
                var rowsAffected = await connection.ExecuteAsync(query, parameters);

                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating the Archive data in the audit.", ex);
            }
        }
    }
}
