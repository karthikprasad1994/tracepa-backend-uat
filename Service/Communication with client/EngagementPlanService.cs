using Dapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Word;
using DocumentFormat.OpenXml.Wordprocessing;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MimeKit;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Linq;
using TracePca.Data;
using TracePca.Dto.Audit;
using TracePca.Interface.Audit;
using Body = DocumentFormat.OpenXml.Wordprocessing.Body;
using Bold = DocumentFormat.OpenXml.Wordprocessing.Bold;
using Document = DocumentFormat.OpenXml.Wordprocessing.Document;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
using WordDoc = DocumentFormat.OpenXml.Wordprocessing.Document;
using WordprocessingDocument = DocumentFormat.OpenXml.Packaging.WordprocessingDocument;

namespace TracePca.Service.Audit
{
    public class EngagementPlanService : EngagementPlanInterface
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;

        public EngagementPlanService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
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

        public async Task<AuditDropDownListDataDTO> LoadAllDDLDataAsync(int compId)
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

                var auditTypeList = connection.QueryAsync<DropDownListData>(@"SELECT cmm_ID AS ID, cmm_Desc AS Name FROM Content_Management_Master
                    WHERE CMM_Category = 'AT' AND CMM_Delflag = 'A' AND CMM_CompID = @CompId ORDER BY cmm_Desc ASC", parameters);

                var reportTypeList = connection.QueryAsync<DropDownListData>(@"SELECT RTM_Id AS ID, RTM_ReportTypeName As Name FROM SAD_ReportTypeMaster
                    WHERE RTM_TemplateId = 1 And RTM_DelFlag = 'A' AND RTM_CompID = @CompId ORDER BY RTM_ReportTypeName ASC", parameters);

                var feeTypeList = connection.QueryAsync<DropDownListData>(@"SELECT cmm_ID AS ID, cmm_Desc AS Name FROM Content_Management_Master
                    WHERE cmm_Category = 'OE' AND CMM_Delflag = 'A' AND CMM_CompID = @CompId ORDER BY cmm_Desc ASC", parameters);

                await Task.WhenAll(currentYear, customerList, auditTypeList, reportTypeList, feeTypeList);

                return new AuditDropDownListDataDTO
                {
                    CurrentYear = await currentYear,
                    CustomerList = customerList.Result.ToList(),
                    AuditTypeList = auditTypeList.Result.ToList(),
                    ReportTypeList = reportTypeList.Result.ToList(),
                    FeeTypeList = feeTypeList.Result.ToList()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading all DDL data", ex);
            }
        }

        public async Task<AuditDropDownListDataDTO> LoadEngagementPlanDDLAsync(int compId, int yearId, int custId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                IEnumerable<LOEDropDownListData> loeList;
                if (custId > 0)
                {
                    loeList = await connection.QueryAsync<LOEDropDownListData>(@"SELECT LOE_ID AS ID, LOE_Name AS Name, LOE_ServiceTypeId As AuditTypeId FROM SAD_CUST_LOE WHERE LOE_YearId = @YearId AND LOE_CompID = @CompId AND LOE_CustomerId = @CustId  order by loe_id",
                        new { CompId = compId, YearId = yearId, CustId = custId });
                }
                else
                {
                    loeList = await connection.QueryAsync<LOEDropDownListData>(@"SELECT LOE_ID AS ID, LOE_Name AS Name, LOE_ServiceTypeId As AuditTypeId FROM SAD_CUST_LOE WHERE LOE_YearId = @YearId AND LOE_CompID = @CompId  order by loe_id",
                        new { CompId = compId, YearId = yearId });
                }

                return new AuditDropDownListDataDTO
                {
                    ExistingEngagementPlanNames = loeList.ToList()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading Engagement Plan DDL data", ex);
            }
        }

        public async Task<IEnumerable<ReportTypeDetailsDTO>> GetReportTypeDetails(int compId, int reportTypeId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var contentIds = await connection.QueryAsync<int>(@"SELECT TEM_ContentId FROM SAD_Finalisation_Report_Template WHERE TEM_FunctionId = @ReportTypeId And TEM_Delflag = 'A' And TEM_CompID = @CompId",
                    new { CompId = compId, ReportTypeId = reportTypeId });

                string query;
                object parameters;
                if (contentIds.Count() == 1)
                {
                    query = @"SELECT RCM_Id, RCM_ReportName, RCM_Heading, RCM_Description FROM SAD_ReportContentMaster WHERE RCM_Id IN (@ContentId) AND RCM_ReportId = @ReportTypeId AND RCM_CompID = @CompId AND RCM_Delflag = 'A' ORDER BY RCM_Id";
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

        public async Task<EngagementPlanDetailsDTO> CheckAndGetEngagementPlanByIdsAsync(int compId, int customerId, int yearId, int auditTypeId)
        {
            try
            {
                int epPKid = 0;
                using var connection = new SqlConnection(_connectionString);

                if (customerId > 0 && auditTypeId > 0)
                {
                    var parameters = new { CompId = compId, CustomerId = customerId, YearId = yearId, AuditTypeId = auditTypeId };
                    string query = @"SELECT LOE_ID FROM SAD_CUST_LOE WHERE LOE_CustomerId = @CustomerId And LOE_YearId = @YearId AND LOE_ServiceTypeId = @AuditTypeId And LOE_CompID = @CompId";
                    epPKid = await connection.QueryFirstOrDefaultAsync<int?>(query, parameters) ?? 0;
                }

                if (epPKid > 0)
                {
                    return await GetEngagementPlanByIdAsync(compId, epPKid);
                }
                else
                {
                    return new EngagementPlanDetailsDTO();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while checking and getting the engagement plan by ID", ex);
            }
        }

        public async Task<EngagementPlanDetailsDTO> GetEngagementPlanByIdAsync(int compId, int epPKid)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var dto = await connection.QueryFirstOrDefaultAsync<EngagementPlanDetailsDTO>(
                    @"SELECT LOE_Id, LOE_YearId, LOE_CustomerId, LOE_ServiceTypeId, LOE_FunctionId, LOE_NatureOfService, LOE_CrBy, LOE_CrOn, LOE_Total, LOE_Name, LOE_Frequency, LOE_STATUS, LOE_Delflag, LOE_IPAddress, LOE_CompID
                    FROM SAD_CUST_LOE WHERE LOE_Id = @LOEId And LOE_CompID = @CompId;", new { CompId = compId, LOEId = epPKid });
                if (dto == null)
                    return new EngagementPlanDetailsDTO();

                var template = await connection.QueryFirstOrDefaultAsync<EngagementPlanDetailsDTO>(
                    @"SELECT LOET_Id, LOET_LOEID AS LOE_Id, LOET_CustomerId, LOET_FunctionId, LOET_ScopeOfWork, LOET_Frequency AS LOET_Frequency, LOET_ProfessionalFees, LOET_CrBy, LOET_IPAddress, LOET_CompID, LOE_AttachID
                    FROM LOE_Template WHERE LOET_LOEID = @LOEId And LOET_CompID = @CompId;", new { CompId = compId, LOEId = epPKid });
                if (template != null)
                {
                    dto.LOET_Id = template.LOET_Id;
                    dto.LOET_ScopeOfWork = template.LOET_ScopeOfWork;
                    dto.LOET_Frequency = template.LOET_Frequency;
                    dto.LOET_ProfessionalFees = template.LOET_ProfessionalFees;
                    dto.LOE_AttachID = template.LOE_AttachID;
                }

                var templateDetails = await connection.QueryAsync<EngagementPlanTemplateDetailsDTO>(
                    @"SELECT LTD_ID, LTD_LOE_ID, LTD_ReportTypeID, LTD_HeadingID, LTD_Heading, LTD_Decription, LTD_FormName, LTD_CrBy, LTD_CrOn, LTD_IPAddress, LTD_CompID 
                    FROM LOE_Template_Details WHERE LTD_FormName = 'LOE' And LTD_LOE_ID = @LOEId And LTD_CompID = @CompId Order By LTD_ID;",
                    new { CompId = compId, LOEId = epPKid });
                dto.EngagementTemplateDetails = templateDetails.ToList();

                var additionalFees = await connection.QueryAsync<EngagementPlanAdditionalFeesDTO>(
                    @"SELECT LAF_ID, LAF_LOEID, LAF_OtherExpensesID, LAF_Charges, LAF_OtherExpensesName, LAF_Delflag, LAF_STATUS, LAF_CrBy, LAF_CrOn, LAF_IPAddress, LAF_CompID
                    FROM LOE_AdditionalFees WHERE LAF_LOEID = @LOEId And LAF_CompID = @CompId Order By LAF_ID;",
                    new { CompId = compId, LOEId = epPKid });
                dto.EngagementAdditionalFees = additionalFees.ToList();
                return dto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting engagement plan by ID", ex);
            }
        }

        public async Task<string> GenerateLOENameAsync(int compId, int yearId, int customerId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var nextSerialNo = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT COUNT(*) + 1 FROM SAD_CUST_LOE WHERE LOE_YearId = @YearId", new { YearId = yearId });
                var loeName = await connection.QueryFirstOrDefaultAsync<string>(
                    @"SELECT CONCAT(cust.CUST_CODE, '/LOE/', yms.YMS_ID, '/', RIGHT('00000' + CAST(@SerialNo AS VARCHAR), 5)) AS LOE_Name
                    FROM SAD_CUSTOMER_MASTER cust, YEAR_MASTER yms WHERE cust.CUST_ID = @CustomerId AND yms.YMS_YEARID = @YearId",
                    new
                    {
                        CustomerId = 2,
                        YearId = yearId,
                        SerialNo = nextSerialNo
                    });

                return loeName;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating LOE name", ex);
            }
        }

        public async Task<int> SaveOrUpdateEngagementPlanDataAsync(EngagementPlanDetailsDTO dto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                bool isUpdate = dto.LOE_Id > 0;
                if (isUpdate)
                {
                    await connection.ExecuteAsync(
                        @"UPDATE SAD_CUST_LOE SET LOE_NatureOfService = @LOE_NatureOfService, LOE_Total = @LOE_Total, LOE_Frequency = @LOE_Frequency, LOE_UpdatedBy = @LOE_UpdatedBy, LOE_UpdatedOn = GETDATE(),
                          LOE_IPAddress = @LOE_IPAddress WHERE LOE_Id = @LOE_Id;", dto, transaction);

                    await connection.ExecuteAsync("DELETE FROM LOE_Template_Details WHERE LTD_FormName = 'LOE' And LTD_LOE_ID = @LOE_Id;", dto, transaction);
                    await connection.ExecuteAsync("DELETE FROM LOE_AdditionalFees WHERE LAF_LOEID = @LOE_Id;", dto, transaction);
                }
                else
                {
                    dto.LOE_Name = await GenerateLOENameAsync(dto.LOE_CompID, dto.LOE_YearId, dto.LOE_CustomerId);
                    dto.LOE_Id = await connection.ExecuteScalarAsync<int>(
                        @"DECLARE @NewId INT; SELECT @NewId = ISNULL(MAX(LOE_Id), 0) + 1 FROM SAD_CUST_LOE;
                          INSERT INTO SAD_CUST_LOE (LOE_Id, LOE_YearId, LOE_CustomerId, LOE_ServiceTypeId, LOE_NatureOfService, LOE_LocationIds, LOE_TimeSchedule, LOE_ReportDueDate, LOE_ProfessionalFees, LOE_OtherFees,
                          LOE_ServiceTax, LOE_RembFilingFee, LOE_CrBy, LOE_CrOn, LOE_Total, LOE_Name, LOE_Frequency, LOE_FunctionId, LOE_SubFunctionId, LOE_STATUS, LOE_Delflag, LOE_IPAddress, LOE_CompID)
                          VALUES (@NewId, @LOE_YearId, @LOE_CustomerId, @LOE_ServiceTypeId, @LOE_NatureOfService, '0', NULL, NULL, 0, 0, 0, 0,
                          @LOE_CrBy, GETDATE(), @LOE_Total, @LOE_Name, @LOE_Frequency, @LOE_ServiceTypeId, 0, 'C', 'A', @LOE_IPAddress, @LOE_CompID); 
                          SELECT @NewId;", dto, transaction);
                }

                int existingTemplateCount = await connection.ExecuteScalarAsync<int>(@"SELECT COUNT(*) FROM LOE_Template WHERE LOET_LOEID = @LOE_Id;", new { dto.LOE_Id }, transaction);
                if (existingTemplateCount > 0)
                {
                    await connection.ExecuteAsync(
                        @"UPDATE LOE_Template SET LOET_CustomerId = @LOE_CustomerId, LOET_FunctionId = @LOE_ServiceTypeId, LOET_ScopeOfWork = @LOET_ScopeOfWork, LOET_Frequency = @LOET_Frequency,
                          LOET_ProfessionalFees = @LOET_ProfessionalFees, LOET_UpdatedBy = @LOE_UpdatedBy, LOET_UpdatedOn = GETDATE(), LOET_IPAddress = @LOE_IPAddress,
                          LOE_AttachID = @LOE_AttachID WHERE LOET_LOEID = @LOE_Id;",
                        new
                        {
                            dto.LOE_Id,
                            dto.LOE_CustomerId,
                            dto.LOE_ServiceTypeId,
                            dto.LOET_ScopeOfWork,
                            dto.LOET_Frequency,
                            dto.LOET_ProfessionalFees,
                            dto.LOE_IPAddress,
                            dto.LOE_CompID,
                            dto.LOE_UpdatedBy,
                            dto.LOE_AttachID
                        }, transaction);
                }
                else
                {
                    dto.LOET_Id = await connection.ExecuteScalarAsync<int>(
                        @"DECLARE @TemplateId INT; SELECT @TemplateId = ISNULL(MAX(LOET_Id), 0) + 1 FROM LOE_Template;
                          INSERT INTO LOE_Template (LOET_Id, LOET_LOEID, LOET_CustomerId, LOET_FunctionId, LOET_ScopeOfWork, LOET_Frequency, LOET_ProfessionalFees, LOET_Delflag, LOET_STATUS, 
                          LOET_CrOn, LOET_CrBy, LOET_IPAddress, LOET_CompID, LOE_AttachID)
                          VALUES ( @TemplateId, @LTD_LOE_ID, @LOE_CustomerId, @LOE_ServiceTypeId, @LOET_ScopeOfWork, @LOET_Frequency, @LOET_ProfessionalFees, 'A', 'C', GETDATE(), @LOE_CrBy, @LOE_IPAddress, @LOE_CompID, @LOE_AttachID);
                          SELECT @TemplateId;",
                        new
                        {
                            LTD_LOE_ID = dto.LOE_Id ?? 0,
                            dto.LOE_CustomerId,
                            dto.LOE_ServiceTypeId,
                            dto.LOET_ScopeOfWork,
                            dto.LOET_Frequency,
                            dto.LOET_ProfessionalFees,
                            dto.LOE_CrBy,
                            dto.LOE_IPAddress,
                            dto.LOE_CompID,
                            dto.LOE_AttachID
                        },
                        transaction
                    );
                }

                foreach (var item in dto.EngagementTemplateDetails)
                {
                    item.LTD_LOE_ID = dto.LOE_Id ?? 0;
                    await connection.ExecuteAsync(
                        @"DECLARE @NewTemplateDetailId INT; SELECT @NewTemplateDetailId = ISNULL(MAX(LTD_ID), 0) + 1 FROM LOE_Template_Details;
                          INSERT INTO LOE_Template_Details (LTD_ID, LTD_LOE_ID, LTD_ReportTypeID, LTD_HeadingID, LTD_Heading, LTD_Decription, LTD_FormName, LTD_CrBy, LTD_CrOn, LTD_IPAddress, LTD_CompID)
                          VALUES (@NewTemplateDetailId, @LTD_LOE_ID, @LTD_ReportTypeID, @LTD_HeadingID, @LTD_Heading, @LTD_Decription, @LTD_FormName, @LTD_CrBy, GETDATE(), @LTD_IPAddress, @LTD_CompID);",
                        item, transaction);
                }

                foreach (var fee in dto.EngagementAdditionalFees)
                {
                    fee.LAF_LOEID = dto.LOE_Id ?? 0;
                    await connection.ExecuteAsync(
                        @"DECLARE @NewFeesId INT; SELECT @NewFeesId = ISNULL(MAX(LAF_ID), 0) + 1 FROM LOE_AdditionalFees;
                          INSERT INTO LOE_AdditionalFees (LAF_ID, LAF_LOEID, LAF_OtherExpensesID, LAF_Charges, LAF_OtherExpensesName, LAF_Delflag, LAF_STATUS, LAF_CrBy, LAF_CrOn, LAF_IPAddress, LAF_CompID)
                          VALUES (@NewFeesId, @LAF_LOEID, @LAF_OtherExpensesID, @LAF_Charges, @LAF_OtherExpensesName, 'A', 'A', @LAF_CrBy, GETDATE(), @LAF_IPAddress, @LAF_CompID);",
                        fee, transaction);
                }

                await transaction.CommitAsync();
                return dto.LOE_Id ?? 0;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while saving or updating the engagement plan data", ex);
            }
        }

        public async Task<bool> ApproveEngagementPlanAsync(int compId, int epPKid, int approvedBy)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var parameters = new { ApprovedBy = approvedBy, LOEId = epPKid, CompId = compId };

                var query1 = @"UPDATE SAD_CUST_LOE SET LOE_STATUS = 'A', LOE_ApprovedBy = @ApprovedBy, LOE_ApprovedOn = GETDATE() WHERE LOE_Id = @LOEId AND LOE_CompID = @CompId";

                var query2 = @"UPDATE LOE_Template SET LOET_STATUS = 'A', LOET_ApprovedBy = @ApprovedBy, LOET_ApprovedOn = GETDATE() WHERE LOET_LOEID = @LOEId AND LOET_CompID = @CompId";

                var rowsAffected1 = await connection.ExecuteAsync(query1, parameters);
                var rowsAffected2 = await connection.ExecuteAsync(query2, parameters);

                return rowsAffected1 > 0 || rowsAffected2 > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while approving the engagement plan", ex);
            }
        }

        public async Task<EngagementPlanReportDetailsDTO> GetEngagementPlanReportDetailsByIdAsync(int compId, int epPKid)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var dto = await connection.QueryFirstOrDefaultAsync<EngagementPlanReportDetailsDTO>(
                    @"SELECT LOE_Name AS EngagementPlanNo, LOE_YearId AS Year, LOE_CustomerId AS Customer, LOE_ServiceTypeId AS AuditType, LOE_Total AS ProfessionalFees, 
                    LOE_Frequency AS Frequency FROM SAD_CUST_LOE WHERE LOE_Id = @LOEId AND LOE_CompID = @CompId;",
                    new { CompId = compId, LOEId = epPKid });

                if (dto == null)
                    return null;

                dto.Year = await connection.ExecuteScalarAsync<string>("SELECT YMS_ID FROM YEAR_MASTER WHERE YMS_YEARID = @YearId AND YMS_CompID = @CompId;", new { YearId = dto.Year, CompId = compId });

                dto.Customer = await connection.ExecuteScalarAsync<string>("SELECT CUST_NAME FROM SAD_CUSTOMER_MASTER WHERE CUST_ID = @CustId AND CUST_CompID = @CompId;", new { CustId = dto.Customer, CompId = compId });

                dto.CurrencyType = await connection.ExecuteScalarAsync<string>("SELECT CUR_CODE FROM SAD_Currency_Master WHERE CUR_CompID = @CompId;", new { CompId = compId });

                dto.AuditType = await connection.ExecuteScalarAsync<string>(@"SELECT CMM_Desc FROM Content_Management_Master WHERE CMM_Category = 'AT' AND CMM_ID = @AuditType AND CMM_CompID = @CompId;",
                    new { AuditType = dto.AuditType, CompId = compId });

                var reportTypeId = await connection.ExecuteScalarAsync<int?>(@"SELECT TOP 1 LTD_ReportTypeID FROM LOE_Template_Details WHERE LTD_FormName = 'LOE' AND LTD_LOE_ID = @LOEId AND LTD_CompID = @CompId;",
                    new { LOEId = epPKid, CompId = compId });

                if (reportTypeId.HasValue)
                {
                    dto.ReportType = await connection.ExecuteScalarAsync<string>("SELECT RTM_ReportTypeName FROM SAD_ReportTypeMaster WHERE RTM_Id = @ReportTypeId AND RTM_CompID = @CompId;",
                        new { ReportTypeId = reportTypeId.Value, CompId = compId });
                }

                dto.Frequency = dto.Frequency switch
                {
                    "1" => "Yearly",
                    "2" => "Quarterly",
                    _ => "Unknown"
                };

                dto.CompanyName = await connection.ExecuteScalarAsync<string>(@"SELECT STUFF((SELECT DISTINCT '; ' + CAST(Company_Name AS VARCHAR(MAX)) FROM Trace_CompanyDetails WHERE Company_CompID = @CompId FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '')",
                    new { CompId = compId });

                dto.AnnexureToLetterOfEngagement = $"Details of Engagement Estimate for the {dto.ReportType} to {dto.Customer}";
                dto.Subject = $"Sub: Engagement letter – {dto.ReportType} for the year ended {dto.Year}";
                dto.CurrentDate = DateTime.Now;

                var templateDetails = await connection.QueryAsync<EngagementPlanTemplateReportDetailsDTO>(
                    @"SELECT LTD_ReportTypeID, LTD_Heading, LTD_Decription FROM LOE_Template_Details WHERE LTD_FormName = 'LOE' AND LTD_LOE_ID = @LOEId AND LTD_CompID = @CompId;",
                    new { CompId = compId, LOEId = epPKid });

                dto.EngagementTemplateDetails = templateDetails.ToList();

                var additionalFees = await connection.QueryAsync<EngagementPlanAdditionalFeesReportDTO>(
                    @"SELECT LAF_OtherExpensesName, LAF_Charges FROM LOE_AdditionalFees WHERE LAF_LOEID = @LOEId AND LAF_CompID = @CompId;",
                    new { CompId = compId, LOEId = epPKid });

                dto.EngagementAdditionalFees = additionalFees.ToList();

                return dto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting engagement plan report details by ID", ex);
            }
        }

        public async Task<List<AttachmentDetailsDTO>> LoadAllAttachmentsByIdAsync(int compId, int attachId)
        {
            try
            {
                var result = new List<AttachmentDetailsDTO>();
                var query = @"SELECT A.ATCH_ID, A.ATCH_DOCID, A.ATCH_FNAME, A.ATCH_EXT, A.ATCH_DESC, A.ATCH_CREATEDBY, U.Usr_FullName AS ATCH_CREATEDBYNAME, A.ATCH_CREATEDON, A.ATCH_SIZE FROM edt_attachments A 
                LEFT JOIN Sad_Userdetails U ON A.ATCH_CREATEDBY = U.Usr_ID AND A.ATCH_COMPID = U.Usr_CompId WHERE A.ATCH_CompID = @CompId AND A.ATCH_ID = @AttachId AND A.ATCH_Status <> 'D' ORDER BY A.ATCH_CREATEDON;";

                using var connection = new SqlConnection(_connectionString);
                result = (await connection.QueryAsync<AttachmentDetailsDTO>(query, new { CompId = compId, AttachId = attachId })).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting the attachment details", ex);
            }
        }

        public string GetTRACeConfigValue(string key)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var query = "SELECT SAD_Config_Value FROM [dbo].[Sad_Config_Settings] WHERE SAD_Config_Key = @Key";
                return connection.QueryFirstOrDefault<string>(query, new { Key = key });
            }
        }

        public async Task<(int attachmentId, string relativeFilePath)> UploadAndSaveAttachmentAsync(FileAttachmentDTO dto, string module)
        {
            try
            {
                if (dto.File == null || dto.File.Length == 0)
                    throw new ArgumentException("Invalid file.");

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Generate attachment and document IDs
                int attachId = dto.ATCH_ID == 0 ? await connection.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(ATCH_ID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_COMPID = @CompId", new { CompId = dto.ATCH_COMPID }) : dto.ATCH_ID;
                int docId = await connection.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(ATCH_DOCID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_COMPID = @CompId", new { CompId = dto.ATCH_COMPID });

                // Prepare file metadata
                string originalName = Path.GetFileNameWithoutExtension(dto.File.FileName) ?? "unknown";
                string safeFileName = (originalName.Replace("&", " and")).Substring(0, Math.Min(95, originalName.Length));
                string fileExt = Path.GetExtension(dto.File.FileName)?.ToLower() ?? ".unk";
                long fileSize = dto.File.Length;

                // Determine file type
                string[] imageExtensions = {".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tif", ".tiff", ".svg", ".psd", ".ai", ".eps", ".ico", ".webp", ".raw", ".heic", ".heif", ".exr", ".dng", ".jp2", ".j2k", ".cr2", ".nef", ".orf", ".arw", ".raf", ".rw2", ".mp4", ".avi", ".mov", ".wmv", ".mkv", ".flv", ".webm", ".m4v", ".mpg", ".mpeg", ".3gp", ".ts", ".m2ts", ".vob", ".mts", ".divx", ".ogv"};
                string[] documentExtensions = {".pdf", ".doc", ".docx", ".txt", ".xls", ".xlsx", ".ppt", ".ppsx", ".pptx", ".odt", ".ods", ".odp", ".rtf", ".csv", ".pptm", ".xlsm", ".docm", ".xml", ".json", ".yaml", ".key", ".numbers", ".pages", ".tar", ".zip", ".rar"};
                string fileType = imageExtensions.Contains(fileExt) ? "Images" : documentExtensions.Contains(fileExt) ? "Documents" : "Others";

                // Build file path
                string basePath = GetTRACeConfigValue("ImgPath"); // Or Directory.GetCurrentDirectory()
                string folderChunk = (docId / 301).ToString();
                string savePath = Path.Combine(basePath, module, fileType, folderChunk);
                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);

                // Save the file
                string uniqueFileName = $"{docId}{fileExt}";
                string fullPath = Path.Combine(savePath, uniqueFileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                // Insert metadata into database
                var insertQuery = @"INSERT INTO EDT_ATTACHMENTS (ATCH_ID, ATCH_DOCID, ATCH_FNAME, ATCH_EXT, ATCH_CREATEDBY, ATCH_VERSION, ATCH_FLAG, ATCH_SIZE, ATCH_FROM, ATCH_Basename, ATCH_CREATEDON, 
                ATCH_Status, ATCH_CompID, Atch_Vstatus, ATCH_REPORTTYPE, ATCH_DRLID) VALUES (@AtchId, @DocId, @FileName, @FileExt, @CreatedBy, 1, 0, @Size, 0, 0, GETDATE(), 'X', @CompId, 'A', 0, 0);";

                await connection.ExecuteAsync(insertQuery, new
                {
                    AtchId = attachId,
                    DocId = docId,
                    FileName = safeFileName,
                    FileExt = fileExt.Trim('.'),
                    CreatedBy = dto.ATCH_CREATEDBY,
                    Size = fileSize,
                    CompId = dto.ATCH_COMPID
                });

                return (attachId, fullPath.Replace("\\", "/"));
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to upload the attachment document.", ex);
            }
        }

        public async Task RemoveAttachmentDocAsync(int compId, int attachId, int docId, int userId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"UPDATE EDT_ATTACHMENTS SET ATCH_Status = 'D', ATCH_MODIFIEDBY = @UserId WHERE ATCH_CompID = @CompId AND ATCH_DOCID = @DocId AND ATCH_ID = @AttachId";

                await connection.ExecuteAsync(query, new { CompId = compId, DocId = docId, AttachId = attachId, UserId = userId });
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to remove the selected document.", ex);
            }
        }

        public async Task UpdateAttachmentDocDescriptionAsync(int compId, int attachId, int docId, int userId, string description)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"UPDATE EDT_ATTACHMENTS SET ATCH_Desc = @Desc, ATCH_MODIFIEDBY = @UserId WHERE ATCH_CompID = @CompId AND ATCH_DOCID = @DocId AND ATCH_ID = @AttachId";

                await connection.ExecuteAsync(query, new { CompId = compId, DocId = docId, AttachId = attachId, Desc = description, UserId = userId });
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update the attachment description.", ex);
            }
        }

        public async Task<(bool, string)> GetAttachmentDocDetailsByIdAsync(int compId, int attachId, int docId, string module)
        {
            try
            {
                var result = new AttachmentDetailsDTO();
                var query = @"SELECT A.ATCH_ID, A.ATCH_DOCID, A.ATCH_FNAME, A.ATCH_EXT, A.ATCH_DESC, A.ATCH_CREATEDBY, U.Usr_FullName AS ATCH_CREATEDBYNAME, A.ATCH_CREATEDON, A.ATCH_SIZE FROM edt_attachments A 
                LEFT JOIN Sad_Userdetails U ON A.ATCH_CREATEDBY = U.Usr_ID AND A.ATCH_COMPID = U.Usr_CompId WHERE A.ATCH_CompID = @CompId AND A.ATCH_ID = @AttachId AND A.ATCH_DOCID = @DocId";

                using var connection = new SqlConnection(_connectionString);
                result = (await connection.QueryFirstAsync<AttachmentDetailsDTO>(query, new { CompId = compId, AttachId = attachId, DocId = docId }));

                if (result == null)
                    return (false, "Attachment metadata not found.");

                if (string.IsNullOrWhiteSpace(result.ATCH_EXT))
                    return (false, "File extension is missing in attachment record.");

                string fileExt = string.IsNullOrWhiteSpace(result.ATCH_EXT) ? ".unk" : (result.ATCH_EXT.StartsWith(".") ? result.ATCH_EXT.ToLower() : "." + result.ATCH_EXT.ToLower());

                // Determine file type
                string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tif", ".tiff", ".svg", ".psd", ".ai", ".eps", ".ico", ".webp", ".raw", ".heic", ".heif", ".exr", ".dng", ".jp2", ".j2k", ".cr2", ".nef", ".orf", ".arw", ".raf", ".rw2", ".mp4", ".avi", ".mov", ".wmv", ".mkv", ".flv", ".webm", ".m4v", ".mpg", ".mpeg", ".3gp", ".ts", ".m2ts", ".vob", ".mts", ".divx", ".ogv" };
                string[] documentExtensions = { ".pdf", ".doc", ".docx", ".txt", ".xls", ".xlsx", ".ppt", ".ppsx", ".pptx", ".odt", ".ods", ".odp", ".rtf", ".csv", ".pptm", ".xlsm", ".docm", ".xml", ".json", ".yaml", ".key", ".numbers", ".pages", ".tar", ".zip", ".rar" };
                string fileType = imageExtensions.Contains(fileExt) ? "Images" : documentExtensions.Contains(fileExt) ? "Documents" : "Others";

                // Build source file path
                string basePath = GetTRACeConfigValue("ImgPath"); // Or Directory.GetCurrentDirectory()
                string folderChunk = (docId / 301).ToString();
                string savedFilePath = Path.Combine(basePath, module, fileType, folderChunk, $"{docId}{fileExt}");

                if (!File.Exists(savedFilePath))
                    return (false, "Attachment file not found on server.");

                // Read file bytes
                byte[] fileBytes = await File.ReadAllBytesAsync(savedFilePath);

                // Build timestamped file name
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string sanitizedFileName = result.ATCH_FNAME.Replace("&", "and").Replace(" ", "_");
                string fileName = $"{sanitizedFileName}_{timestamp}{fileExt}";

                // Prepare temp path
                string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Tempfolder", compId.ToString());
                Directory.CreateDirectory(tempFolder);

                string tempFilePath = Path.Combine(tempFolder, fileName);

                // Overwrite if already exists
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);

                // Write to temp location
                await File.WriteAllBytesAsync(tempFilePath, fileBytes);

                // Generate public URL
                var request = _httpContextAccessor.HttpContext.Request;
                string baseUrl = $"{request.Scheme}://{request.Host}";
                string downloadUrl = $"{baseUrl}/Tempfolder/{compId}/{fileName}";

                return (true, downloadUrl);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting the attachment details", ex);
            }
        }

        public async Task<AuditDropDownListDataDTO> LoadUsersByCustomerIdDDLAsync(int custId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
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

        public async Task<bool> SendEmailAndSaveEngagementPlanExportDataAsync(EngagementPlanReportExportDetailsDTO dto)
        {
            try
            {
                var reportdto = await GetEngagementPlanReportDetailsByIdAsync(dto.CompId, dto.LOEId);
                if (reportdto == null)
                    throw new ApplicationException("Engagement Plan data not found for the specified ID.");

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                using var transaction = connection.BeginTransaction();

                try
                {
                    var docCountQuery = @"SELECT COUNT(*) + 1 FROM Doc_Reviewremarks_History WHERE DRH_Custid = @CustID AND DRH_Loeid = @LOEId AND DRH_Yearid = @YearId";
                    int iDocCount = await connection.ExecuteScalarAsync<int>(docCountQuery, new
                    {
                        CustID = dto.CustomerId,
                        LOEId = dto.LOEId,
                        YearId = dto.YearId
                    }, transaction);

                    var fileBytes = await GeneratePdfAsync(reportdto);
                    var stream = new MemoryStream(fileBytes);
                    var formFile = new FormFile(stream, 0, stream.Length, "File", $"LOE_Report_{iDocCount}.pdf")
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "application/pdf"
                    };

                    var dtoFile = new FileAttachmentDTO
                    {
                        ATCH_ID = dto.AttachmentId,
                        ATCH_CREATEDBY = dto.UserId,
                        ATCH_COMPID = dto.CompId,
                        File = formFile
                    };

                    var (attachmentId, attachmentPath) = await UploadAndSaveAttachmentAsync(dtoFile, "StandardAudit");

                    int attachId = await connection.ExecuteScalarAsync<int>("SELECT ISNULL(LOE_AttachID, 0) FROM LOE_Template WHERE LOET_LOEID = @LOE_Id", new { LOE_Id = dto.LOEId }, transaction);

                    if (attachId == 0)
                    {
                        await connection.ExecuteAsync(@"UPDATE LOE_Template SET LOE_AttachID = @LOE_AttachID WHERE LOET_LOEID = @LOE_Id;", new { LOE_AttachID = attachmentId, LOE_Id = dto.LOEId }, transaction);
                    }

                    int requestId = await connection.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(DR_ID), 0) + 1 FROM Doc_Reviewremarks WHERE DR_CompId = @CompId", new { CompId = dto.CompId }, transaction);

                    var insertRequestQuery = @"INSERT INTO Doc_Reviewremarks (DR_ID, DR_Custid, DR_DocLoeId_Branchid, DR_DocYearid, DR_DocStatus, DR_DocType, DR_Date, DR_CreatedBy, DR_CreatedOn, DR_CompId, DR_emailSentTo, DR_IPAddress, DR_Observation, DR_DocDelflag) 
                        VALUES (@DR_ID, @DR_Custid, @DR_DocLoeId_Branchid, @DR_DocYearid, 'W', 'C', GETDATE(), @DR_CreatedBy, GETDATE(), @DR_CompId, @DR_emailSentTo, @DR_IPAddress, @DR_Observation, 'LOE');";

                    List<int> customerUserIds = dto.CustomerUserIds;
                    string emailSentTo = string.Join(",", customerUserIds);
                    await connection.ExecuteAsync(insertRequestQuery, new
                    {
                        DR_ID = requestId,
                        DR_Custid = dto.CustomerId,
                        DR_DocLoeId_Branchid = dto.LOEId,
                        DR_DocYearid = dto.YearId,
                        DR_CreatedBy = dto.UserId,
                        DR_emailSentTo = emailSentTo,
                        DR_CompId = dto.CompId,
                        DR_IPAddress = dto.IPAddress,
                        DR_Observation = dto.Comments
                    }, transaction);

                    int newRemarkId = await connection.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(DRH_ID), 0) + 1 FROM Doc_Reviewremarks_History", transaction: transaction);
                    var insertRemarksQuery = @"INSERT INTO Doc_Reviewremarks_History (DRH_ID, DRH_MASid, DRH_Custid, DRH_Loeid, DRH_RemarksType, DRH_Remarks, DRH_RemarksBy, DRH_Status, DRH_Date, DRH_IPAddress, DRH_CompID, DRH_Yearid, DRH_attchmentid) 
                         VALUES (@DRH_ID, @DRH_MASid, @DRH_Custid, @DRH_Loeid, 'C', @DRH_Remarks, @DRH_RemarksBy, 'W', GETDATE(), @DRH_IPAddress, @DRH_CompID, @DRH_Yearid, @DRH_attchmentid);";

                    await connection.ExecuteAsync(insertRemarksQuery, new
                    {
                        DRH_ID = newRemarkId,
                        DRH_MASid = requestId,
                        DRH_Custid = dto.CustomerId,
                        DRH_Loeid = dto.LOEId,
                        DRH_Remarks = dto.Comments,
                        DRH_RemarksBy = dto.UserId,
                        DRH_IPAddress = dto.IPAddress,
                        DRH_CompID = dto.CompId,
                        DRH_Yearid = dto.YearId,
                        DRH_attchmentid = attachmentId
                    }, transaction);

                    transaction.Commit();

                    return await SendMailAsync(dto, attachmentPath);
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while saving or updating the engagement plan export data.", ex);
            }
        }

        public async Task<(byte[] fileBytes, string contentType, string fileName)> GenerateAndDownloadReportAsync(int compId, int epPKid, string format)
        {
            try
            {
                var dto = await GetEngagementPlanReportDetailsByIdAsync(compId, epPKid);

                if (dto == null)
                    throw new ApplicationException("Engagement Plan data not found for the specified ID.");

                byte[] fileBytes;
                string contentType;
                string fileName = dto.EngagementPlanNo ?? "LOE_Report";

                if (format.ToLower() == "pdf")
                {
                    fileBytes = await GeneratePdfAsync(dto);
                    contentType = "application/pdf";
                    fileName += ".pdf";
                }
                else
                {
                    fileBytes = await GenerateWordAsync(dto);
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    fileName += ".docx";
                }

                return (fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating the report.", ex);
            }
        }

        public async Task<string> GenerateReportAndGetURLPathAsync(int compId, int epPKid, string format)
        {
            try
            {
                var dto = await GetEngagementPlanReportDetailsByIdAsync(compId, epPKid);

                if (dto == null)
                    throw new ApplicationException("Engagement Plan data not found for the specified ID.");

                byte[] fileBytes;
                string extension;
                string contentType;
                string rawName = dto.EngagementPlanNo ?? "LOE_Report";
                string fileName = rawName.Replace("/", "_").Replace("\\", "_");

                if (format.ToLower() == "pdf")
                {
                    fileBytes = await GeneratePdfAsync(dto);
                    contentType = "application/pdf";
                    extension = ".pdf";
                }
                else
                {
                    fileBytes = await GenerateWordAsync(dto);
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    extension = ".docx";
                }
                fileName += extension;

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

        private async Task<byte[]> GeneratePdfAsync(EngagementPlanReportDetailsDTO dto)
        {
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
                            column.Item().Text($"Ref.No.: {dto.EngagementPlanNo}").FontSize(12).Bold();
                            column.Item().Text($"Date: {dto.CurrentDate:dd MMM yyyy}").FontSize(10);
                            column.Item().PaddingBottom(10).Text(dto.Customer ?? "N/A").FontSize(10);
                            column.Item().PaddingBottom(10).Text($"Dear: {dto.Customer ?? "Client"}").FontSize(10);
                            column.Item().PaddingBottom(10).Text($"{dto.Subject ?? "Sub: Engagement Letter"}").FontSize(10);

                            if (dto.EngagementTemplateDetails?.Any() == true)
                            {
                                int count = 1;
                                foreach (var item in dto.EngagementTemplateDetails)
                                {
                                    column.Item().Text($"{count}. {item.LTD_Heading}").FontSize(11).Bold();
                                    if (!string.IsNullOrWhiteSpace(item.LTD_Decription))
                                        column.Item().Text(item.LTD_Decription).FontSize(10);

                                    column.Item().PaddingBottom(5);
                                    count++;
                                }
                            }

                            if (dto.EngagementAdditionalFees?.Any() == true)
                            {
                                column.Item().PaddingTop(10).Text("Additional Fees").FontSize(12).Bold().Underline();
                                column.Item().PaddingBottom(10).Text($"Details of Engagement Estimate for the Letter of Engagement in {dto.AuditType} to {dto.Customer}").FontSize(10);

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
                                        header.Cell().Element(CellStyle).AlignRight().Text($"Charges In {dto.CurrencyType}").FontSize(10).Bold();
                                    });

                                    int slNo = 1;
                                    decimal totalCharges = 0;
                                    foreach (var fee in dto.EngagementAdditionalFees)
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
                            column.Item().Text(dto.CompanyName ?? "[Company Name]").FontSize(10).Bold();
                            column.Item().Text("[Firm Name]").FontSize(10);
                            column.Item().PaddingBottom(30);
                            column.Item().PaddingTop(10).PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Black);
                            column.Item().PaddingBottom(20).Text("We agree to the terms of the engagement described in this letter.").FontSize(10);
                            column.Item().Text(dto.Customer ?? "[Client Name]").FontSize(10).Bold();
                            column.Item().Text("[Client Name]").FontSize(10);
                            column.Item().PaddingBottom(20);
                            column.Item().Text("[Signature]").FontSize(10);
                            column.Item().PaddingBottom(20);
                            column.Item().Text("[Date]").FontSize(10);
                        });
                    });
                });

                using var ms = new MemoryStream();
                document.GeneratePdf(ms);
                return ms.ToArray();
            });
        }

        private async Task<byte[]> GenerateWordAsync(EngagementPlanReportDetailsDTO dto)
        {
            return await Task.Run(() =>
            {
                using var ms = new MemoryStream();
                using var doc = WordprocessingDocument.Create(ms, DocumentFormat.OpenXml.WordprocessingDocumentType.Document);
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());
                var body = mainPart.Document.Body;

                void AddParagraph(string text, bool bold = false, bool italic = false)
                {
                    var run = new Run(new Text(text ?? ""));
                    var props = new RunProperties();
                    if (bold) props.Append(new Bold());
                    if (italic) props.Append(new Italic());
                    if (props.HasChildren) run.PrependChild(props);

                    var para = new Paragraph(run);
                    body.AppendChild(para);
                }

                void AddEmptyLine() => body.AppendChild(new Paragraph(new Run(new Text(""))));

                AddParagraph("Letter of Engagement", bold: true);
                AddParagraph($"Ref.No.: {dto.EngagementPlanNo}", bold: true);
                AddParagraph($"Date: {dto.CurrentDate:dd MMM yyyy}");
                AddParagraph(dto.Customer);
                AddEmptyLine();

                AddParagraph($"Dear: {dto.Customer}");
                AddParagraph($"{dto.Subject}");
                AddEmptyLine();

                if (dto.EngagementTemplateDetails?.Any() == true)
                {
                    int count = 1;
                    foreach (var item in dto.EngagementTemplateDetails)
                    {
                        AddParagraph($"{count}. {item.LTD_Heading}", bold: true);
                        if (!string.IsNullOrWhiteSpace(item.LTD_Decription))
                            AddParagraph(item.LTD_Decription);
                        AddEmptyLine();
                        count++;
                    }
                }

                AddParagraph($"Details of Engagement Estimate for the Letter of Engagement in {dto.AuditType} to {dto.Customer}");
                AddEmptyLine();

                if (dto.EngagementAdditionalFees?.Any() == true)
                {
                    AddParagraph("Additional Fees", bold: true, italic: true);
                    decimal total = 0;
                    int slNo = 1;
                    foreach (var fee in dto.EngagementAdditionalFees)
                    {
                        var charge = decimal.TryParse(fee.LAF_Charges, out var parsed) ? parsed : 0;
                        total += charge;
                        AddParagraph($"{slNo}. {fee.LAF_OtherExpensesName}: {charge:N2}");
                        slNo++;
                    }
                    AddParagraph($"Total: {total:N2}", bold: true);
                    AddEmptyLine();
                }

                AddParagraph("Very truly yours,");
                AddParagraph(dto.CompanyName, bold: true);
                AddParagraph("[Firm Name]");
                AddEmptyLine();

                AddParagraph("We agree to the terms of the engagement described in this letter.");
                AddParagraph(dto.Customer, bold: true);
                AddParagraph("[Client Name]");
                AddEmptyLine();
                AddParagraph("[Signature]");
                AddEmptyLine();
                AddParagraph("[Date]");

                mainPart.Document.Save();
                return ms.ToArray();
            });
        }

        private async Task<bool> SendMailAsync(EngagementPlanReportExportDetailsDTO dto, string attachmentPath)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                List<int> customerUserIds = dto.CustomerUserIds;
                var emailList = (await connection.QueryAsync<string>(@"SELECT usr_Email FROM Sad_UserDetails WHERE USR_ID IN @UserIds", new { UserIds = customerUserIds })).Where(email => !string.IsNullOrWhiteSpace(email)).Distinct().ToList();
                var reportDetails = await GetEngagementPlanReportDetailsByIdAsync(dto.CompId, dto.LOEId);

                string subject = $"Intimation mail To Review The LOE for Client - {reportDetails.Customer}";

                string htmlBody = $@"
                    <!DOCTYPE html><html><head>
                        <style>table, th, td {{ border: 1px solid black; border-collapse: collapse; }}</style>
                        </head><body>
                        <p style='font-size:15px;font-family:Calibri,sans-serif;text-align:left;'>Dear Sir/Ma'am</p>
                        <p style='font-size:15px;font-family:Calibri,sans-serif;'>Greetings from TRACe PA.&nbsp;&nbsp;</p>
                        <table style='width:100%;border:1px solid black;'>
                            <tr>
                                <td style='padding:10px;text-align:left;'>
                                    <strong>Client Name:</strong> {reportDetails.Customer}<br/>                                  
                                </td>
                            </tr>
                            <tr>
                                <td style='padding:10px;text-align:left;'>
                                    <strong>LOE No.:</strong> {reportDetails.EngagementPlanNo}
                                </td>
                            </tr>
                            <tr>
                                <td style='padding:10px;'>
                                    <strong>Comments:</strong><br/>
                                    {System.Net.WebUtility.HtmlEncode(dto.Comments)}                                
                                </td>
                            </tr>
                        </table>
                        <p style='font-size:15px;font-family:Calibri,sans-serif;text-align:left;'>
                            <strong>Click Here: </strong>
                            <a href='https://tracepacust-user.multimedia.interactivedns.com'>
                                TRACePA
                            </a>
                        </p>
                        <p>Please login to TRACe PA website using the above link and credentials shared with you.</p>
                        <p>Home page of the application will show you the list of documents to review.</p>
                        <br/>
                        <p>Thanks,<br/>TRACe PA Team</p>
                    </body></html>";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("TRACe PA Team", "trace@mmcspl.com"));
                foreach (var bcc in emailList.Distinct())
                {
                    message.Bcc.Add(MailboxAddress.Parse(bcc));
                }

                message.Subject = subject;
                var builder = new BodyBuilder { HtmlBody = htmlBody };
                if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                {
                    builder.Attachments.Add(attachmentPath);
                }
                message.Body = builder.ToMessageBody();

                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var smtpUser = _configuration["EmailSettings:SmtpUser"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                var fromEmail = _configuration["EmailSettings:FromEmail"];

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(fromEmail, smtpPassword);//(smtpUser, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while sending the email.", ex);
            }
        }

        public async Task<LOEStatusSummary> GetLOEProgressAsync(int compId, int yearId, int custId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var parameters = new { CompId = compId, YearID = yearId, CustId = custId };

                var result = await connection.QueryFirstOrDefaultAsync<LOEStatusSummary>(@"SELECT COUNT(*) AS TotalLOEs, SUM(CASE WHEN LOE_STATUS = 'A' THEN 1 ELSE 0 END) AS ApprovedLOEs,
                    SUM(CASE WHEN LOE_STATUS != 'A' THEN 1 ELSE 0 END) AS PendingLOEs FROM SAD_CUST_LOE WHERE LOE_CompID = @CompId And LOE_YearId = @YearId And (@CustId <= 0 OR LOE_CustomerId = @CustId)", parameters);
                return result ?? new LOEStatusSummary();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting LOE progress data.", ex);
            }
        }

        public async Task<AuditStatusSummary> GetAuditProgressAsync(int compId, int yearId, int custId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var parameters = new { CompId = compId, YearID = yearId, CustId = custId };

                var result = await connection.QueryFirstOrDefaultAsync<AuditStatusSummary>(@"SELECT
                    COUNT(*) AS TotalAudits,
                    SUM(CASE WHEN SA_Status = 1 THEN 1 ELSE 0 END) AS Scheduled,
                    SUM(CASE WHEN SA_Status = 2 THEN 1 ELSE 0 END) AS CommunicationWithClient,
                    SUM(CASE WHEN SA_Status = 3 THEN 1 ELSE 0 END) AS TBR,
                    SUM(CASE WHEN SA_Status = 4 THEN 1 ELSE 0 END) AS ConductAudit,
                    SUM(CASE WHEN SA_Status = 5 THEN 1 ELSE 0 END) AS Report,
                    SUM(CASE WHEN SA_Status = 10 THEN 1 ELSE 0 END) AS Completed,
                    SUM(CASE WHEN SA_Status = 0 THEN 1 ELSE 0 END) AS AuditStarted,
                    SUM(CASE WHEN SA_Status <> 10 THEN 1 ELSE 0 END) AS InProgress
                    FROM StandardAudit_Schedule WHERE SA_CompID = @CompId AND SA_YearID = @YearId AND (@CustId <= 0 OR SA_CustID = @CustId)", parameters);

                return result ?? new AuditStatusSummary();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting Audit progress data.", ex);
            }
        }

        public async Task<PassedDueDatesSummary> GetAuditPassedDueDatesAsync(int compId, int yearId, int custId)
        {
            try
            {
                //using var connection = new SqlConnection(_connectionString);
                //await connection.OpenAsync();

                //var parameters = new { CompId = compId, YearID = yearId, CustId = custId };

                //var result = await connection.QueryFirstOrDefaultAsync<PassedDueDatesSummary>(@"", parameters);
                //return result ?? new PassedDueDatesSummary();

                await Task.CompletedTask;
                return new PassedDueDatesSummary
                {
                    OverdueAudits = 0,
                    LastDue = DateTime.MinValue,
                    HighRisk = false
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting Audit Passed due dates data.", ex);
            }
        }
    }
}
