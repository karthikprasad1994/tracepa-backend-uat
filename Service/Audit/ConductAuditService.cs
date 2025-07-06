using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Dto;
using TracePca.Dto.Audit;
using TracePca.Interface.Audit;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
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
    public class ConductAuditService : ConductAuditInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly AuditCompletionInterface _auditCompletionInterface;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConductAuditService(Trdmyus1Context dbcontext, IConfiguration configuration, AuditCompletionInterface auditCompletionInterface, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _auditCompletionInterface = auditCompletionInterface;
            _httpContextAccessor = httpContextAccessor;
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

                var workpaperCheckList = connection.QueryAsync<DropDownListData>(@"SELECT cmm_ID As ID, cmm_Desc As Name FROM Content_Management_Master WHERE cmm_Delflag = 'A' 
                AND cmm_Category = 'WCM' AND cmm_CompID = @CompId ORDER BY cmm_Desc ASC", parameters);

                var typeofTestList = connection.QueryAsync<DropDownListData>(@"SELECT cmm_ID As ID, cmm_Desc As Name FROM Content_Management_Master WHERE cmm_Delflag = 'A' 
                AND cmm_Category = 'TOT' AND cmm_CompID = @CompId ORDER BY cmm_Desc ASC", parameters);

                //var typeofTestList = new List<DropDownListData>
                //{
                //    new DropDownListData { ID = 1, Name = "Inquiry" },
                //    new DropDownListData { ID = 2, Name = "Observation" },
                //    new DropDownListData { ID = 3, Name = "Examination" },
                //    new DropDownListData { ID = 4, Name = "Inspection" },
                //    new DropDownListData { ID = 5, Name = "Substantive Testing" }
                //};

                var exceededMaterialityList = new List<DropDownListData>
                {
                    new DropDownListData { ID = 1, Name = "Yes" },
                    new DropDownListData { ID = 2, Name = "No" },
                    new DropDownListData { ID = 3, Name = "NA" }
                };

                var workPaperStatusList = new List<DropDownListData>
                {
                    new DropDownListData { ID = 1, Name = "Open" },
                    new DropDownListData { ID = 2, Name = "WIP" },
                    new DropDownListData { ID = 3, Name = "Closed" }
                };

                var testResultList = new List<DropDownListData>
                {
                    new DropDownListData { ID = 1, Name = "Yes" },
                    new DropDownListData { ID = 2, Name = "No" },
                    new DropDownListData { ID = 3, Name = "NA" }
                };

                await Task.WhenAll(currentYear, customerList, workpaperCheckList);

                return new AuditDropDownListDataDTO
                {
                    CurrentYear = await currentYear,
                    CustomerList = customerList.Result.ToList(),
                    WorkpaperCheckList = workpaperCheckList.Result.ToList(),
                    TypeofTestList = typeofTestList.Result.ToList(),
                    ExceededMaterialityList = exceededMaterialityList.ToList(),
                    WorkPaperStatusList = workPaperStatusList.ToList(),
                    TestResultList = testResultList.ToList()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading all DDL data", ex);
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

        public async Task<string> GetCustomerFinancialYearAsync(int compId, int custId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                const string query = @"
                    SELECT 
                        CASE ISNULL(CUST_FY, 0)
                            WHEN 1 THEN 'Jan 1st to Dec 31st'
                            WHEN 2 THEN 'Feb 1st to Jan 31st'
                            WHEN 3 THEN 'Mar 1st to Feb 28th'
                            WHEN 4 THEN 'Apr 1st to May 31st'
                            WHEN 5 THEN 'May 1st to Apr 30th'
                            WHEN 6 THEN 'Jun 1st to May 31st'
                            WHEN 7 THEN 'Jul 1st to Jun 30th'
                            WHEN 8 THEN 'Aug 1st to Jul 31st'
                            WHEN 9 THEN 'Sep 1st to Aug 31st'
                            WHEN 10 THEN 'Oct 1st to Sep 30th'
                            WHEN 11 THEN 'Nov 1st to Oct 31st'
                            WHEN 12 THEN 'Dec 1st to Jan 31st'
                            ELSE '-' 
                        END AS CUST_FY_Text
                    FROM SAD_CUSTOMER_MASTER
                    WHERE CUST_ID = @custId AND CUST_CompID = @compId";
                return await connection.ExecuteScalarAsync<string>(query, new { custId, compId }) ?? "-";
            }
            catch (Exception ex)
            {
                return "-";
            }
        }

        public async Task<string> GenerateWorkpaperNoync(int compId, int auditId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var nextSerialNo = await connection.QueryFirstOrDefaultAsync<string>(@"SELECT RIGHT('000' + CAST(COUNT(*) + 1 AS VARCHAR(3)), 3) FROM StandardAudit_ScheduleConduct_WorkPaper WHERE SSW_SA_ID = @AuditId", new { AuditId = auditId });
                var auditNo = await connection.QueryFirstOrDefaultAsync<string>(@"SELECT SA_AuditNo FROM StandardAudit_Schedule WHERE SA_ID = @AuditId", new { AuditId = auditId });

                if (string.IsNullOrEmpty(auditNo))
                    throw new Exception("Audit number not found for the given Audit ID.");

                string wpName = $"{auditNo}/WP{nextSerialNo}";
                return wpName;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating Workpaper No.", ex);
            }
        }

        public async Task<AuditDropDownListDataDTO> LoadDRLWithAttachmentsDDLAsync(int compId, int auditId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var sql = @"SELECT CMM_ID AS ID, CMM_Desc AS Name FROM Content_Management_Master WHERE CMM_CompID = @compId AND cmm_delflag = 'A' 
                AND CMM_ID IN (SELECT ADRL_RequestedListID FROM Audit_DRLLog WHERE ADRL_AuditNo = @AuditId AND ADRL_AttachID > 0 AND ADRL_CompID = @compId) ORDER BY CMM_Desc ASC";

                var result = await connection.QueryAsync<DropDownListData>(sql, new { CompId = compId, AuditId = auditId });
                return new AuditDropDownListDataDTO
                {
                    DRLWithAttachmentsList = result.ToList()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading DRL list with attachments.", ex);
            }
        }

        public async Task<bool> IsDuplicateWorkpaperRefAsync(int auditId, string workpaperRef, int? workpaperId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                const string query = @"SELECT COUNT(1) FROM StandardAudit_ScheduleConduct_WorkPaper WHERE SSW_SA_ID = @AuditId AND SSW_WorkpaperRef = @WorkpaperRef AND (@WorkpaperId = 0 OR SSW_ID != @WorkpaperId)";
                int count = await connection.ExecuteScalarAsync<int>(query, new
                {
                    AuditId = auditId,
                    WorkpaperRef = workpaperRef,
                    WorkpaperId = workpaperId ?? 0
                });

                return count > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while checking for duplicate Workpaper Reference.", ex);
            }
        }

        public async Task<int> SaveOrUpdateConductAuditWorkpaperAsync(ConductAuditWorkPaperDetailsDTO dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                bool isUpdate = dto.SSW_ID.HasValue && dto.SSW_ID.Value > 0;

                if (isUpdate)
                {
                    await connection.ExecuteAsync(@"UPDATE StandardAudit_ScheduleConduct_WorkPaper SET SSW_WorkpaperRef = @SSW_WorkpaperRef, SSW_TypeOfTest = @SSW_TypeOfTest, SSW_WPCheckListID = @SSW_WPCheckListID, 
                        SSW_DRLID = @SSW_DRLID, SSW_ExceededMateriality = @SSW_ExceededMateriality, SSW_AuditorHoursSpent = @SSW_AuditorHoursSpent, SSW_Observation = @SSW_Observation,
                        SSW_NotesSteps = @SSW_NotesSteps, SSW_CriticalAuditMatter = @SSW_CriticalAuditMatter, SSW_Conclusion = @SSW_Conclusion, SSW_Status = @SSW_Status, SSW_AttachID = @SSW_AttachID,
                        SSW_UpdatedBy = @SSW_UpdatedBy, SSW_UpdatedOn = GETDATE(), SSW_IPAddress = @SSW_IPAddress WHERE SSW_ID = @SSW_ID And SSW_SA_ID = @SSW_SA_ID And SSW_CompID = @SSW_CompID;", new
                    {
                        dto.SSW_ID,
                        dto.SSW_SA_ID,
                        dto.SSW_WorkpaperRef,
                        dto.SSW_TypeOfTest,
                        dto.SSW_WPCheckListID,
                        dto.SSW_DRLID,
                        dto.SSW_ExceededMateriality,
                        dto.SSW_AuditorHoursSpent,
                        dto.SSW_Observation,
                        dto.SSW_NotesSteps,
                        dto.SSW_CriticalAuditMatter,
                        dto.SSW_Conclusion,
                        dto.SSW_Status,
                        dto.SSW_AttachID,
                        dto.SSW_UpdatedBy,
                        dto.SSW_IPAddress,
                        dto.SSW_CompID
                    }, transaction);
                }
                else
                {
                    dto.SSW_WorkpaperNo = await GenerateWorkpaperNoync(dto.SSW_CompID ?? 0, dto.SSW_SA_ID ?? 0);
                    var newId = await connection.QuerySingleAsync<int>(@"
                    DECLARE @SSW_ID INT;
                    SELECT @SSW_ID = ISNULL(MAX(SSW_ID), 0) + 1 FROM StandardAudit_ScheduleConduct_WorkPaper;

                    INSERT INTO StandardAudit_ScheduleConduct_WorkPaper(
                        SSW_ID, SSW_SA_ID, SSW_WorkpaperNo, SSW_WorkpaperRef, SSW_TypeOfTest, SSW_WPCheckListID, SSW_DRLID,
                        SSW_ExceededMateriality, SSW_AuditorHoursSpent, SSW_Observation, SSW_NotesSteps, SSW_CriticalAuditMatter, SSW_Conclusion,
                        SSW_Status, SSW_AttachID, SSW_CrBy, SSW_CrOn, SSW_IPAddress, SSW_CompID
                    ) VALUES (
                        @SSW_ID, @SSW_SA_ID, @SSW_WorkpaperNo, @SSW_WorkpaperRef, @SSW_TypeOfTest, @SSW_WPCheckListID, @SSW_DRLID,
                        @SSW_ExceededMateriality, @SSW_AuditorHoursSpent, @SSW_Observation, @SSW_NotesSteps, @SSW_CriticalAuditMatter, @SSW_Conclusion,
                        @SSW_Status, @SSW_AttachID, @SSW_CrBy, GETDATE(), @SSW_IPAddress, @SSW_CompID
                    );

                    SELECT @SSW_ID;", new
                    {
                        dto.SSW_SA_ID,
                        dto.SSW_WorkpaperNo,
                        dto.SSW_WorkpaperRef,
                        dto.SSW_TypeOfTest,
                        dto.SSW_WPCheckListID,
                        dto.SSW_DRLID,
                        dto.SSW_ExceededMateriality,
                        dto.SSW_AuditorHoursSpent,
                        dto.SSW_Observation,
                        dto.SSW_NotesSteps,
                        dto.SSW_CriticalAuditMatter,
                        dto.SSW_Conclusion,
                        dto.SSW_Status,
                        dto.SSW_AttachID,
                        dto.SSW_CrBy,
                        dto.SSW_IPAddress,
                        dto.SSW_CompID
                    }, transaction);

                    dto.SSW_ID = newId;
                }

                await transaction.CommitAsync();
                return dto.SSW_ID ?? 0;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while saving or updating the workpaper", ex);
            }
        }

        public async Task<List<ConductAuditWorkPaperDetailsDTO>> GetConductAuditWorkPapersByAuditIdAsync(int compId, int auditId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var result = new List<ConductAuditWorkPaperDetailsDTO>();
                var query = @"SELECT a.SSW_ID, a.SSW_SA_ID, a.SSW_WorkpaperNo, a.SSW_WorkpaperRef, a.SSW_TypeOfTest,
                    ISNULL(STUFF((SELECT ', ' + cmm.CMM_Desc FROM STRING_SPLIT(CAST(a.SSW_TypeOfTest AS VARCHAR(MAX)), ',') AS s JOIN Content_Management_Master cmm ON TRY_CAST(s.value AS INT) = cmm.CMM_ID
                    WHERE cmm.CMM_Category = 'TOT' FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''),'') AS SSW_TypeOfTestName, 
                    a.SSW_Observation, a.SSW_Conclusion, a.SSW_ReviewerComments, a.SSW_ExceededMateriality,
                    CASE WHEN a.SSW_ExceededMateriality = 1 THEN 'Yes' WHEN a.SSW_ExceededMateriality = 2 THEN 'No' WHEN a.SSW_ExceededMateriality = 3 THEN 'NA' ELSE NULL END AS SSW_ExceededMaterialityName,
                    a.SSW_AuditorHoursSpent, a.SSW_NotesSteps, a.SSW_CriticalAuditMatter, a.SSW_WPCheckListID, a.SSW_Status, a.SSW_DRLID, 
                    ISNULL((SELECT Count(*) FROM edt_attachments WHERE ATCH_CompID = @CompId AND ATCH_ID in (SELECT ADRL_AttachID FROM Audit_DRLLog WHERE ADRL_AuditNo = @AuditId AND ADRL_CompID = @CompId And ADRL_RequestedListID > 0 And ADRL_RequestedListID = a.SSW_DRLID)), 0) As SSW_DRLAttachmentCount,
                    ISNULL((SELECT ADRL_AttachID FROM Audit_DRLLog WHERE ADRL_AuditNo = @AuditId AND ADRL_CompID = @CompId And ADRL_RequestedListID > 0 And ADRL_RequestedListID = a.SSW_DRLID), 0) As SSW_DRLAttachmentID,
                    CASE WHEN a.SSW_Status = 1 THEN 'Open' WHEN a.SSW_Status = 2 THEN 'WIP' WHEN a.SSW_Status = 3 THEN 'Closed' ELSE NULL END AS SSW_StatusName, a.SSW_AttachID,
					ISNULL((SELECT Count(*) FROM edt_attachments WHERE ATCH_CompID = @CompId AND ATCH_ID = a.SSW_AttachID), 0) As SSW_AttachCount,
                    a.SSW_CrBy,b.usr_FullName AS SSW_CrByName, a.SSW_CrOn, ISNULL(a.SSW_ReviewedBy, 0) AS SSW_ReviewedBy,ISNULL(d.usr_FullName, '') AS SSW_ReviewedByName, ISNULL(a.SSW_ReviewedOn, '') AS SSW_ReviewedOn, ISNULL(a.SSW_UpdatedBy, 0) AS SSW_UpdatedBy, ISNULL(c.usr_FullName, '') AS SSW_UpdatedByName, ISNULL(a.SSW_UpdatedOn, '') AS SSW_UpdatedOn, 
                    a.SSW_IPAddress, a.SSW_CompID FROM StandardAudit_ScheduleConduct_WorkPaper a 
                    LEFT JOIN sad_userdetails b ON b.Usr_ID = a.SSW_CrBy
                    LEFT JOIN sad_userdetails c ON c.Usr_ID = a.SSW_UpdatedBy
                    LEFT JOIN sad_userdetails d ON d.Usr_ID = a.SSW_ReviewedBy
                    WHERE a.SSW_CompID = @CompId AND a.SSW_SA_ID = @AuditId ORDER BY a.SSW_WorkpaperNo DESC";

                result = (await connection.QueryAsync<ConductAuditWorkPaperDetailsDTO>(query, new { CompId = compId, AuditId = auditId })).ToList();
                return result ?? new List<ConductAuditWorkPaperDetailsDTO>();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting workpaper list by audit ID", ex);
            }
        }

        public async Task<ConductAuditWorkPaperDetailsDTO> GetConductAuditWorkPaperByWPIdAsync(int compId, int auditId, int workpaperId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var result = new ConductAuditWorkPaperDetailsDTO();
                var query = @"SELECT a.SSW_ID, a.SSW_SA_ID, a.SSW_WorkpaperNo, a.SSW_WorkpaperRef, a.SSW_TypeOfTest,
                    ISNULL(STUFF((SELECT ', ' + cmm.CMM_Desc FROM STRING_SPLIT(CAST(a.SSW_TypeOfTest AS VARCHAR(MAX)), ',') AS s JOIN Content_Management_Master cmm ON TRY_CAST(s.value AS INT) = cmm.CMM_ID
                    WHERE cmm.CMM_Category = 'TOT' FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''),'') AS SSW_TypeOfTestName, 
                    a.SSW_Observation, a.SSW_Conclusion, a.SSW_ReviewerComments, a.SSW_ExceededMateriality,
                    CASE WHEN a.SSW_ExceededMateriality = 1 THEN 'Yes' WHEN a.SSW_ExceededMateriality = 2 THEN 'No' WHEN a.SSW_ExceededMateriality = 3 THEN 'NA' ELSE NULL END AS SSW_ExceededMaterialityName,
                    a.SSW_AuditorHoursSpent, a.SSW_NotesSteps, a.SSW_CriticalAuditMatter, a.SSW_WPCheckListID, a.SSW_Status, a.SSW_DRLID, 
                    ISNULL((SELECT Count(*) FROM edt_attachments WHERE ATCH_CompID = @CompId AND ATCH_ID in (SELECT ADRL_AttachID FROM Audit_DRLLog WHERE ADRL_AuditNo = @AuditId AND ADRL_CompID = @CompId And ADRL_RequestedListID > 0 And ADRL_RequestedListID = a.SSW_DRLID)), 0) As SSW_DRLAttachmentCount,
                    ISNULL((SELECT ADRL_AttachID FROM Audit_DRLLog WHERE ADRL_AuditNo = @AuditId AND ADRL_CompID = @CompId And ADRL_RequestedListID > 0 And ADRL_RequestedListID = a.SSW_DRLID), 0) As SSW_DRLAttachmentID,
                    CASE WHEN a.SSW_Status = 1 THEN 'Open' WHEN a.SSW_Status = 2 THEN 'WIP' WHEN a.SSW_Status = 3 THEN 'Closed' ELSE NULL END AS SSW_StatusName, a.SSW_AttachID,
                    ISNULL((SELECT Count(*) FROM edt_attachments WHERE ATCH_CompID = @CompId AND ATCH_ID = a.SSW_AttachID), 0) As SSW_AttachCount,
                    a.SSW_CrBy,b.usr_FullName AS SSW_CrByName, a.SSW_CrOn, ISNULL(a.SSW_ReviewedBy, 0) AS SSW_ReviewedBy,ISNULL(d.usr_FullName, '') AS SSW_ReviewedByName, ISNULL(a.SSW_ReviewedOn, '') AS SSW_ReviewedOn, ISNULL(a.SSW_UpdatedBy, 0) AS SSW_UpdatedBy, ISNULL(c.usr_FullName, '') AS SSW_UpdatedByName, ISNULL(a.SSW_UpdatedOn, '') AS SSW_UpdatedOn, 
                    a.SSW_IPAddress, a.SSW_CompID FROM StandardAudit_ScheduleConduct_WorkPaper a
                    LEFT JOIN sad_userdetails b ON b.Usr_ID = a.SSW_CrBy
                    LEFT JOIN sad_userdetails c ON c.Usr_ID = a.SSW_UpdatedBy
                    LEFT JOIN sad_userdetails d ON d.Usr_ID = a.SSW_ReviewedBy
                    WHERE a.SSW_CompID = @CompId AND a.SSW_SA_ID = @AuditId AND a.SSW_ID = @WorkpaperId";

                result = await connection.QueryFirstOrDefaultAsync<ConductAuditWorkPaperDetailsDTO>(query, new { CompId = compId, AuditId = auditId, WorkpaperId = workpaperId });
                return result ?? new ConductAuditWorkPaperDetailsDTO();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting the selected workpaper by ID", ex);
            }
        }

        public async Task<AuditDropDownListDataDTO> LoadConductAuditCheckPointHeadingsAsync(int compId, int auditId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var query = @"SELECT DENSE_RANK() OVER (ORDER BY ACM_Heading DESC) AS ID, ACM_Heading AS Name FROM AuditType_Checklist_Master WHERE ACM_ID IN 
                    (SELECT SAC_CheckPointID FROM StandardAudit_ScheduleCheckPointList WHERE SAC_SA_ID = @AuditId AND SAC_CompID = @CompId)
                    AND ACM_CompId = @CompId AND ACM_Heading <> '' AND ACM_Heading <> 'NULL' GROUP BY ACM_Heading ORDER BY ACM_Heading DESC";

                var parameters = new
                {
                    CompId = compId,
                    AuditId = auditId
                };

                var headingList = await connection.QueryAsync<DropDownListData>(query, parameters);
                return new AuditDropDownListDataDTO
                {
                    AuditCheckPointHeadingList = headingList.ToList()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading conduct audit checkpoint headings", ex);
            }
        }

        public async Task<bool> AssignWorkpapersToCheckPointsAsync(List<ConductAuditDetailsDTO> dtos)
        {
            if (dtos == null)
                throw new ArgumentNullException(nameof(dtos));

            const string query = @"UPDATE StandardAudit_ScheduleCheckPointList SET SAC_WorkpaperID = @SAC_WorkpaperID, SAC_ConductedBy = @SAC_ConductedBy, SAC_LastUpdatedOn = GETDATE()
                WHERE SAC_ID = @SAC_ID AND SAC_SA_ID = @SAC_SA_ID AND SAC_CompID = @SAC_CompID AND SAC_CheckPointID = @SAC_CheckPointID";

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                int totalRows = await connection.ExecuteAsync(query, dtos, transaction);
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while assigning multiple workpapers to checkpoints", ex);
            }
        }

        public async Task<bool> UpdateConductAuditCheckPointRemarksAndAnnexureAsync(ConductAuditDetailsDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            const string query = @"UPDATE StandardAudit_ScheduleCheckPointList SET SAC_ConductedBy = @SAC_ConductedBy, SAC_LastUpdatedOn = GETDATE(), SAC_Annexure = @SAC_Annexure, SAC_Remarks = @SAC_Remarks,
                    SAC_TestResult = @SAC_TestResult WHERE SAC_ID = @SAC_ID AND SAC_SA_ID = @SAC_SA_ID AND SAC_CheckPointID = @SAC_CheckPointID AND SAC_CompID = @SAC_CompID;";
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();
                int rows = await connection.ExecuteAsync(query, dto);
                return rows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error updating checkpoint remarks/annexure", ex);
            }
        }

        public async Task<(int attachmentId, string relativeFilePath)> UploadAndSaveCheckPointAttachmentAsync(FileAttachmentDTO dto, int auditId, int checkPointId)
        {
            try
            {
                if (dto.File == null || dto.File.Length == 0)
                    throw new ArgumentException("Invalid file.");

                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                int attachId = dto.ATCH_ID == 0 ? await connection.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(ATCH_ID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_COMPID = @CompId", new { CompId = dto.ATCH_COMPID }) : dto.ATCH_ID;
                int docId = await connection.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(ATCH_DOCID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_COMPID = @CompId", new { CompId = dto.ATCH_COMPID });

                string originalFileName = Path.GetFileNameWithoutExtension(dto.File.FileName) ?? "unknown";
                string safeFileName = originalFileName.Replace("&", " and");
                safeFileName = safeFileName.Length > 95 ? safeFileName.Substring(0, 95) : safeFileName;

                string fileExt = Path.GetExtension(dto.File.FileName)?.TrimStart('.').ToLower() ?? "unk";
                long fileSize = dto.File.Length;

                string relativeFolder = Path.Combine("Uploads", "Audit", (docId / 301).ToString());
                string absoluteFolder = Path.Combine(Directory.GetCurrentDirectory(), relativeFolder);

                if (!Directory.Exists(absoluteFolder))
                    Directory.CreateDirectory(absoluteFolder);

                string uniqueFileName = $"{docId}.{fileExt}";
                string fullFilePath = Path.Combine(absoluteFolder, uniqueFileName);

                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                var insertQuery = @"
                INSERT INTO EDT_ATTACHMENTS (ATCH_ID, ATCH_DOCID, ATCH_FNAME, ATCH_EXT, ATCH_CREATEDBY, ATCH_VERSION, ATCH_FLAG, ATCH_SIZE, ATCH_FROM, ATCH_Basename, ATCH_CREATEDON, 
                ATCH_Status, ATCH_CompID, Atch_Vstatus, ATCH_REPORTTYPE, ATCH_DRLID)
                VALUES (@AtchId, @DocId, @FileName, @FileExt, @CreatedBy, 1, 0, @Size, 0, 0, GETDATE(), 'X', @CompId, 'A', 0, 0);";

                await connection.ExecuteAsync(insertQuery, new
                {
                    AtchId = attachId,
                    DocId = docId,
                    FileName = safeFileName,
                    FileExt = fileExt,
                    CreatedBy = dto.ATCH_CREATEDBY,
                    Size = fileSize,
                    CompId = dto.ATCH_COMPID
                });

                const string attachQuery = @"UPDATE StandardAudit_ScheduleCheckPointList SET SAC_AttachID = @SAC_AttachID WHERE SAC_SA_ID = @SAC_SA_ID AND SAC_CheckPointID = @SAC_CheckPointID AND SAC_CompID = @SAC_CompID;";
                await connection.ExecuteAsync(attachQuery, new
                {
                    SAC_AttachID = attachId,
                    SAC_SA_ID = auditId,
                    SAC_CheckPointID = checkPointId,
                    SAC_CompID = dto.ATCH_COMPID
                });

                return (attachId, fullFilePath.Replace("\\", "/"));
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to upload the attachment document.", ex);
            }
        }

        public async Task<List<ConductAuditDetailsDTO>> LoadConductAuditCheckPointDetailsAsync(int compId, int auditId, int userId, string? heading)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            string checkpointIds = "";

            const string sql = @"SELECT usr_Partner FROM sad_userdetails WHERE usr_CompID = @CompId AND usr_ID = @UserId AND usr_DelFlag IN ('A', 'B', 'L')";
            var isPartner = await connection.ExecuteScalarAsync<int>(sql, new { CompId = compId, UserId = userId });

            if (isPartner == 0)
            {
                var sqlCheckpointIds = @"SELECT ISNULL(STUFF((SELECT ',' + CAST(SACD_CheckpointId AS VARCHAR(MAX)) FROM StandardAudit_Checklist_Details WHERE SACD_EmpId = @UserId AND SACD_AuditId = @AuditId FOR XML PATH('')), 1, 1, ''), '')";
                checkpointIds = await connection.ExecuteScalarAsync<string>(sqlCheckpointIds, new { UserId = userId, AuditId = auditId });
            }

            var sqlCA = @"SELECT @AuditId AS SAC_SA_ID, ISNULL(SAC_ID, 0) AS SAC_ID, ISNULL(SAC_CheckPointID, 0) AS SAC_CheckPointID, ISNULL(ACM_Checkpoint, '') AS SAC_CheckPointName,
                ISNULL(ACM_Heading, '') AS SAC_HeadingName, ISNULL(ACM_Assertions, '') AS SAC_AssertionName, ISNULL(SAC_Mandatory, 0) AS SAC_Mandatory,
                CASE WHEN ISNULL(SAC_Mandatory, 0) = 1 THEN 'Yes' ELSE 'No' END AS SAC_MandatoryName, ISNULL(SAC_Annexure, 0) AS SAC_Annexure,
                CASE WHEN ISNULL(SAC_Annexure, 0) = 1 THEN 'TRUE' ELSE 'FALSE' END AS SAC_AnnexureName, ISNULL(SAC_Remarks, '') AS SAC_Remarks,
                ISNULL(SAC_ReviewerRemarks, '') AS SAC_ReviewerRemarks, ISNULL(SAC_Status, 0) AS SAC_Status, ISNULL(SAC_TestResult, 0) AS SAC_TestResult,
                CASE WHEN ISNULL(SAC_TestResult, 0) = 1 THEN 'Yes' WHEN ISNULL(SAC_TestResult, 0) = 2 THEN 'No' WHEN ISNULL(SAC_TestResult, 0) = 3 THEN 'NA' ELSE '' END AS SAC_TestResultName,
                ISNULL(SAC_AttachID, 0) AS SAC_AttachID, ISNULL(SAC_ConductedBy, 0) AS SAC_ConductedBy, ISNULL(a.Usr_FullName, '') AS SAC_ConductedByName, ISNULL(SAC_LastUpdatedOn,'') AS SAC_LastUpdatedOn,
                ISNULL(SSW_ID, 0) AS SAC_WorkpaperID, ISNULL(SSW_WorkpaperNo, '') AS SAC_WorkpaperNo, ISNULL(SSW_WorkpaperRef, '') AS SAC_WorkpaperRef FROM StandardAudit_ScheduleCheckPointList
                JOIN AuditType_Checklist_Master ON ACM_ID = SAC_CheckPointID LEFT JOIN sad_userdetails a ON a.Usr_ID = SAC_ConductedBy 
                LEFT JOIN StandardAudit_ScheduleConduct_WorkPaper ON SSW_SA_ID = @AuditId AND SSW_ID = SAC_WorkpaperID
                WHERE SAC_SA_ID = @AuditId AND SAC_CompID = @CompId";
            if (!string.IsNullOrWhiteSpace(heading))
            {
                sqlCA += @" AND ACM_ID IN (SELECT ACM_ID FROM AuditType_Checklist_Master WHERE ACM_Heading = @Heading AND ACM_CompId = @CompId AND ACM_DELFLG = 'A')";
            }
            if (isPartner == 0 && !string.IsNullOrWhiteSpace(checkpointIds))
            {
                sqlCA += $" AND SAC_CheckPointID IN ({checkpointIds})";
            }
            sqlCA += " ORDER BY SAC_CheckPointID";

            var results = new List<ConductAuditDetailsDTO>();
            results = (await connection.QueryAsync<ConductAuditDetailsDTO>(sqlCA, new { AuditId = auditId, CompId = compId, Heading = heading })).ToList();
            return results;
        }

        public async Task<(byte[] fileBytes, string contentType, string fileName)> GenerateAndDownloadWorkpapersReportAsync(int compId, int auditId, string format)
        {
            try
            {
                byte[] fileBytes;
                string contentType;
                string fileName = "Audit_Workpaper";

                if (format.ToLower() == "pdf")
                {
                    fileBytes = await GenerateWorkpapersPdfAsync(compId, auditId);
                    contentType = "application/pdf";
                    fileName += ".pdf";
                }
                else
                {
                    throw new ApplicationException("Unsupported format. Only PDF is currently supported.");
                }

                string relativeFolder = Path.Combine("Uploads", "Audit").ToString();
                string absoluteFolder = Path.Combine(Directory.GetCurrentDirectory(), relativeFolder);
                string fullFilePath = Path.Combine(absoluteFolder, "Audit_Workpaper.pdf");
                await File.WriteAllBytesAsync(fullFilePath, fileBytes);

                return (fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating the report.", ex);
            }
        }

        public async Task<(byte[] fileBytes, string contentType, string fileName)> GenerateAndDownloadCheckPointsReportAsync(int compId, int auditId, string format)
        {
            try
            {
                byte[] fileBytes;
                string contentType;
                string fileName = "Conduct_Audit";

                if (format.ToLower() == "pdf")
                {
                    fileBytes = await GenerateCheckPointsPdfAsync(compId, auditId);
                    contentType = "application/pdf";
                    fileName += ".pdf";
                }
                else
                {
                    throw new ApplicationException("Unsupported format. Only PDF is currently supported.");
                }

                string relativeFolder = Path.Combine("Uploads", "Audit").ToString();
                string absoluteFolder = Path.Combine(Directory.GetCurrentDirectory(), relativeFolder);
                string fullFilePath = Path.Combine(absoluteFolder, "Conduct_Audit.pdf");
                await File.WriteAllBytesAsync(fullFilePath, fileBytes);

                return (fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating the report.", ex);
            }
        }

        public async Task<string> GenerateWorkpapersReportAndGetURLPathAsync(int compId, int auditId, string format)
        {
            try
            {
                byte[] fileBytes;
                string contentType;
                string fileName = "Conduct_Audit_Workpaper";

                if (format.ToLower() == "pdf")
                {
                    fileBytes = await GenerateCheckPointsPdfAsync(compId, auditId);
                    contentType = "application/pdf";
                    fileName += ".pdf";
                }
                else
                {
                    throw new ApplicationException("Unsupported format. Only PDF is currently supported.");
                }

                var tempPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp");
                if (!Directory.Exists(tempPath))
                    Directory.CreateDirectory(tempPath);

                var filePath = Path.Combine(tempPath, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                await File.WriteAllBytesAsync(filePath, fileBytes);

                string baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
                string downloadUrl = $"{baseUrl}/temp/{fileName}";

                return downloadUrl;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating the report.", ex);
            }
        }

        public async Task<string> GenerateCheckPointsReportAndGetURLPathAsync(int compId, int auditId, string format)
        {
            try
            {
                byte[] fileBytes;
                string contentType;
                string fileName = "Conduct_Audit_CheckPoints";

                if (format.ToLower() == "pdf")
                {
                    fileBytes = await GenerateCheckPointsPdfAsync(compId, auditId);
                    contentType = "application/pdf";
                    fileName += ".pdf";
                }
                else
                {
                    throw new ApplicationException("Unsupported format. Only PDF is currently supported.");
                }

                var tempPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp");
                if (!Directory.Exists(tempPath))
                    Directory.CreateDirectory(tempPath);

                var filePath = Path.Combine(tempPath, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                await File.WriteAllBytesAsync(filePath, fileBytes);

                string baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
                string downloadUrl = $"{baseUrl}/temp/{fileName}";

                return downloadUrl;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating the report.", ex);
            }
        }

        private async Task<byte[]> GenerateWorkpapersPdfAsync(int compId, int auditId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var result = await connection.QueryFirstOrDefaultAsync<(int EpPkId, int CustID, string CustName, string YearName, string AuditNo)>(
                @"SELECT LOE.LOE_ID AS EpPkId, SA.SA_CustID As CustID, CUST.CUST_NAME As CustName, YMS.YMS_ID AS YearName, SA_AuditNo + ' - ' + CMA.CMM_Desc As AuditNo FROM StandardAudit_Schedule AS SA
                  LEFT JOIN SAD_CUST_LOE AS LOE ON LOE.LOE_CustomerId = SA.SA_CustID AND LOE.LOE_YearId = SA.SA_YearID AND LOE.LOE_ServiceTypeId = SA.SA_AuditTyPeId
                  LEFT JOIN SAD_CUSTOMER_MASTER AS CUST ON SA.SA_CustID = CUST_ID 
                  LEFT JOIN YEAR_MASTER AS YMS ON YMS.YMS_YEARID = SA.SA_YearID
                  LEFT JOIN Content_Management_Master CMA On CMA.cmm_ID = SA.SA_AuditTypeID
                  WHERE LOE.LOE_CompID = @CompId AND SA.SA_ID = @AuditId;", new { CompId = compId, AuditId = auditId });

            List<ConductAuditWorkPaperDTO> dtoCAWP = await _auditCompletionInterface.LoadConductAuditWorkPapersAsync(compId, auditId);

            var reportTypeList = await connection.QueryAsync<DropDownListData>(@"SELECT RTM_Id AS ID, RTM_ReportTypeName As Name FROM SAD_ReportTypeMaster
                    WHERE RTM_TemplateId = 4 And RTM_DelFlag = 'A' AND RTM_CompID = @CompId ORDER BY RTM_ReportTypeName", new { CompId = compId }); //RTM_TemplateId = 4 And RTM_AudrptType = 3

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
                            column.Item().AlignCenter().PaddingBottom(10).Text("Conduct Audit Work Paper Report").FontSize(16).Bold();
                            column.Item().Text(text =>
                            {
                                text.Span("Client Name: ").FontSize(10).Bold();
                                text.Span(result.CustName).FontSize(10);
                            });
                            column.Item().Text(text =>
                            {
                                text.Span("Audit No: ").FontSize(10).Bold();
                                text.Span(result.AuditNo).FontSize(10);
                            });

                            column.Item().PaddingBottom(10);

                            if (dtoCAWP.Any() == true)
                            {
                                column.Item().PaddingBottom(15).Table(table =>
                                {
                                    foreach (var details in dtoCAWP)
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(1);
                                            columns.RelativeColumn(2);
                                        });

                                        table.Cell().Element(CellStyle).Text("Workpaper Name:").Bold();
                                        table.Cell().Element(CellStyle).Text(details.WorkpaperRef);

                                        table.Cell().Element(CellStyle).Text("Created By and Date:").Bold();
                                        table.Cell().Element(CellStyle).Text($"{details.CreatedBy}, {details.CreatedOn}");

                                        table.Cell().Element(CellStyle).Text("Workpaper No:").Bold();
                                        table.Cell().Element(CellStyle).Text(details.WorkpaperNo);

                                        table.Cell().Element(CellStyle).Text("Reviewed By and Date:").Bold();
                                        table.Cell().Element(CellStyle).Text($"{details.ReviewedBy}, {details.ReviewedOn}");

                                        table.Cell().Element(CellStyle).Text("Type of Test:").Bold();
                                        table.Cell().Element(CellStyle).Text(details.TypeOfTest);

                                        table.Cell().Element(CellStyle).Text("Status:").Bold();
                                        table.Cell().Element(CellStyle).Text(details.Status);

                                        table.Cell().Element(CellStyle).Text("Notes/Steps:").Bold();
                                        table.Cell().Element(CellStyle).Text(details.Notes);

                                        table.Cell().Element(CellStyle).Text("Deviations/Exceptions Noted:").Bold();
                                        table.Cell().Element(CellStyle).Text(details.Deviations);

                                        table.Cell().Element(CellStyle).Text("Critical Audit Matter(CAM):").Bold();
                                        table.Cell().Element(CellStyle).Text(details.CriticalAuditMatter);

                                        table.Cell().Element(CellStyle).Text("Conclusion:").Bold();
                                        table.Cell().Element(CellStyle).Text(details.Conclusion);

                                        table.Cell().Element(CellStyle).Text("Attachments:").Bold();
                                        table.Cell().Element(CellStyle).Text(details.AttachNames);

                                        table.Cell().Element(CellStyle).Text("").Bold();
                                        table.Cell().Element(CellStyle).Text("");
                                    }
                                    static IContainer CellStyle(IContainer container) => container.Border(0.5f).PaddingVertical(3).PaddingHorizontal(4);
                                });
                            }

                            column.Item().PaddingBottom(10);
                        });
                    });
                });
                using var ms = new MemoryStream();
                document.GeneratePdf(ms);
                return ms.ToArray();
            });
        }

        private async Task<byte[]> GenerateCheckPointsPdfAsync(int compId, int auditId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var result = await connection.QueryFirstOrDefaultAsync<(int EpPkId, int CustID, string CustName, string YearName, string AuditNo)>(
                @"SELECT LOE.LOE_ID AS EpPkId, SA.SA_CustID As CustID, CUST.CUST_NAME As CustName, YMS.YMS_ID AS YearName, SA_AuditNo + ' - ' + CMA.CMM_Desc As AuditNo FROM StandardAudit_Schedule AS SA
                  LEFT JOIN SAD_CUST_LOE AS LOE ON LOE.LOE_CustomerId = SA.SA_CustID AND LOE.LOE_YearId = SA.SA_YearID AND LOE.LOE_ServiceTypeId = SA.SA_AuditTyPeId
                  LEFT JOIN SAD_CUSTOMER_MASTER AS CUST ON SA.SA_CustID = CUST_ID 
                  LEFT JOIN YEAR_MASTER AS YMS ON YMS.YMS_YEARID = SA.SA_YearID
                  LEFT JOIN Content_Management_Master CMA On CMA.cmm_ID = SA.SA_AuditTypeID
                  WHERE LOE.LOE_CompID = @CompId AND SA.SA_ID = @AuditId;", new { CompId = compId, AuditId = auditId });

            List<ConductAuditReportDetailDTO> dtoCA = await _auditCompletionInterface.GetConductAuditReportAsync(compId, auditId);
            List<ConductAuditObservationDTO> dtoCAO = await _auditCompletionInterface.GetConductAuditObservationsAsync(compId, auditId);

            var reportTypeList = await connection.QueryAsync<DropDownListData>(@"SELECT RTM_Id AS ID, RTM_ReportTypeName As Name FROM SAD_ReportTypeMaster
                    WHERE RTM_TemplateId = 4 And RTM_DelFlag = 'A' AND RTM_CompID = @CompId ORDER BY RTM_ReportTypeName", new { CompId = compId }); //RTM_TemplateId = 4 And RTM_AudrptType = 3

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
                            column.Item().AlignCenter().PaddingBottom(10).Text("Conduct Audit Report").FontSize(16).Bold();
                            column.Item().Text(text =>
                            {
                                text.Span("Client Name: ").FontSize(10).Bold();
                                text.Span(result.CustName).FontSize(10);
                            });
                            column.Item().Text(text =>
                            {
                                text.Span("Audit No: ").FontSize(10).Bold();
                                text.Span(result.AuditNo).FontSize(10);
                            });

                            column.Item().PaddingBottom(10);

                            if (dtoCA.Any() == true)
                            {
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(0.5f);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(1);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("Sl No").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Heading").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Check Point").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Comments").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Annexures").FontSize(10).Bold();
                                    });

                                    int slNo = 1;
                                    foreach (var details in dtoCA)
                                    {
                                        table.Cell().Element(CellStyle).Text(slNo.ToString()).FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.Heading.ToString()).FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.CheckPoints.ToString()).FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.Comments.ToString()).FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.Annexures.ToString()).FontSize(10);
                                        slNo++;
                                    }
                                    static IContainer CellStyle(IContainer container) =>
                                        container.Border(0.5f).PaddingVertical(3).PaddingHorizontal(4);
                                });
                            }

                            column.Item().PaddingBottom(10);

                            if (dtoCAO.Any() == true)
                            {
                                column.Item().AlignCenter().PaddingBottom(10).Text("Conduct Audit Observation Details").FontSize(14).Bold();
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(0.5f);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("Sl No").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Check Point").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Observations").FontSize(10).Bold();
                                    });

                                    int slNo = 1;
                                    foreach (var details in dtoCAO)
                                    {
                                        table.Cell().Element(CellStyle).Text(slNo.ToString()).FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.CheckPoint.ToString()).FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.Observations.ToString()).FontSize(10);
                                        slNo++;
                                    }
                                    static IContainer CellStyle(IContainer container) => container.Border(0.5f).PaddingVertical(3).PaddingHorizontal(4);
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

        public async Task<AuditDropDownListDataDTO> LoadUsersByCustomerIdDDLAsync(int custId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var userList = await connection.QueryAsync<DropDownListData>(@"SELECT usr_FullName As Name,usr_Id As ID from Sad_UserDetails where usr_CompanyId = @CustId And (usr_DelFlag='A' or usr_DelFlag='B' or usr_DelFlag='L') And Usr_Email IS NOT NULL And Usr_Email<>'' order by usr_FullName", new { CustId = custId });
                return new AuditDropDownListDataDTO
                {
                    CustomerUserList = userList.ToList()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading customer user dropdown data.", ex);
            }
        }
    }
}
