using System.Drawing.Printing;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StackExchange.Redis;
using System.Security.Cryptography;
using System.Xml.XPath;
using TracePca.Data;
using TracePca.Dto.Audit;
using TracePca.Interface.Audit;
<<<<<<< HEAD
using iTextSharp.text;
using iTextSharp.text.pdf;
using Xceed.Words.NET;
=======
using TracePca.Models;
>>>>>>> 3ae2e2ae96df442ac7507a63a0448e28e2c717f3

namespace TracePca.Service.Audit
{
    public class EngagementPlanService : EngagementPlanInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public EngagementPlanService(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }

        public async Task<AuditDropDownListDataDTO> LoadAllDDLDataAsync(int compId)
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

                var auditTypeList = connection.QueryAsync<DropDownListData>(@"SELECT cmm_ID AS ID, cmm_Desc AS Name FROM Content_Management_Master
                    WHERE CMM_Category = 'AT' AND CMM_Delflag = 'A' AND CMM_CompID = @CompId ORDER BY cmm_Desc ASC", parameters);

                var reportTypeList = connection.QueryAsync<DropDownListData>(@"SELECT RTM_Id AS ID, RTM_ReportTypeName As Name FROM SAD_ReportTypeMaster
                    WHERE RTM_TemplateId = 1 And RTM_DelFlag = 'A' AND RTM_CompID = @CompId ORDER BY RTM_ReportTypeName", parameters);

                var feeTypeList = connection.QueryAsync<DropDownListData>(@"SELECT cmm_ID AS ID, cmm_Desc AS Name FROM Content_Management_Master
                    WHERE cmm_Category = 'OE' AND CMM_Delflag = 'A' AND CMM_CompID = @CompId", parameters);

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
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                IEnumerable<LOEDropDownListData> loeList;
                if (custId > 0)
                {
                    loeList = await connection.QueryAsync<LOEDropDownListData>(@"SELECT LOE_ID AS ID, LOE_Name AS Name, LOE_ServiceTypeId As AuditTypeId FROM SAD_CUST_LOE WHERE LOE_YearId = @YearId AND LOE_CompID = @CompId AND LOE_CustomerId = @CustId",
                        new { CompId = compId, YearId = yearId, CustId = custId });
                }
                else
                {
                    loeList = await connection.QueryAsync<LOEDropDownListData>(@"SELECT LOE_ID AS ID, LOE_Name AS Name, LOE_ServiceTypeId  As AuditTypeId FROM SAD_CUST_LOE WHERE LOE_YearId = @YearId AND LOE_CompID = @CompId",
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
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
                    FROM LOE_Template_Details WHERE LTD_FormName = 'LOE' And LTD_LOE_ID = @LOEId And LTD_CompID = @CompId;",
                    new { CompId = compId, LOEId = epPKid });
                dto.EngagementTemplateDetails = templateDetails.ToList();

                var additionalFees = await connection.QueryAsync<EngagementPlanAdditionalFeesDTO>(
                    @"SELECT LAF_ID, LAF_LOEID, LAF_OtherExpensesID, LAF_Charges, LAF_OtherExpensesName, LAF_Delflag, LAF_STATUS, LAF_CrBy, LAF_CrOn, LAF_IPAddress, LAF_CompID
                    FROM LOE_AdditionalFees WHERE LAF_LOEID = @LOEId And LAF_CompID = @CompId;",
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
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var result = await connection.QueryFirstOrDefaultAsync<string>(
                    @"SELECT CONCAT('EP_', @YearId, '_', @CustomerId) AS LOE_Name FROM DUAL",
                    new { YearId = yearId, CustomerId = customerId });
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating LOE name", ex);
            }
        }

        public async Task<int> SaveOrUpdateEngagementPlanDataAsync(EngagementPlanDetailsDTO dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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

                dto.CompanyName = await connection.ExecuteScalarAsync<string>(@"SELECT STUFF((SELECT DISTINCT '; ' + CAST(Company_Name AS VARCHAR(MAX)) FROM Trace_CompanyDetails WHERE Company_CompID = @CompId FOR XML PATH('')), 1, 2, '')",
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

                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                result = (await connection.QueryAsync<AttachmentDetailsDTO>(query, new { CompId = compId, AttachId = attachId })).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting the attachment details", ex);
            }
        }

        public async Task<int> UploadAndSaveAttachmentAsync(FileAttachmentDTO dto)
        {
            try
            {
                if (dto.File == null || dto.File.Length == 0)
                    throw new ArgumentException("Invalid file.");

                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                string basePath = await connection.ExecuteScalarAsync<string>(@"SELECT sad_Config_Value FROM sad_config_settings WHERE sad_Config_Key = 'ImgPath' AND sad_compid = @CompId", new { CompId = dto.ATCH_COMPID });
                basePath ??= Directory.GetCurrentDirectory();

                int attachId = dto.ATCH_ID == 0 ? await connection.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(ATCH_ID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_COMPID = @CompId", new { CompId = dto.ATCH_COMPID }) : dto.ATCH_ID;
                int docId = await connection.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(ATCH_DOCID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_COMPID = @CompId", new { CompId = dto.ATCH_COMPID });

                string originalName = Path.GetFileNameWithoutExtension(dto.File.FileName) ?? "unknown";
                string safeFileName = originalName.Replace("&", "And");
                safeFileName = safeFileName.Length > 95 ? safeFileName.Substring(0, 95) : safeFileName;
                string fileExtension = (Path.GetExtension(dto.File.FileName) ?? ".unk").ToLower().TrimStart('.');
                long fileSize = dto.File.Length;

                string[] imageExtensions = { "jpg", "jpeg", "png", "gif", "bmp", "tif", "tiff", "svg", "psd", "ai", "eps", "ico", "webp", "raw", "heic", "heif", "exr", "dng", "jp2", "j2k", "cr2", "nef", "orf", "arw", "raf", "rw2", "mp4", "avi", "mov", "wmv", "mkv", "flv", "webm", "m4v", "mpg", "mpeg", "3gp", "ts", "m2ts", "vob", "mts", "divx", "ogv" };
                string[] documentExtensions = { "pdf", "doc", "docx", "txt", "xls", "xlsx", "ppt", "ppsx", "pptx", "odt", "ods", "odp", "rtf", "csv", "pptm", "xlsm", "docm", "xml", "json", "yaml", "key", "numbers", "pages", "tar", "zip", "rar" };

                string fileType = imageExtensions.Contains(fileExtension) ? "Images" : documentExtensions.Contains(fileExtension) ? "Documents" : "Others";

                string accessCodeModulePath = Path.Combine(basePath, "Audit");
                Directory.CreateDirectory(accessCodeModulePath);

                string finalDirectory = Path.Combine(accessCodeModulePath, fileType, (docId / 301).ToString());
                Directory.CreateDirectory(finalDirectory);

                string finalFilePath = Path.Combine(finalDirectory, $"{docId}.{fileExtension}");

                if (File.Exists(finalFilePath))
                    File.Delete(finalFilePath);

                await using var stream = new FileStream(finalFilePath, FileMode.Create);
                await dto.File.CopyToAsync(stream);

                var insertQuery = @"
                INSERT INTO EDT_ATTACHMENTS (ATCH_ID, ATCH_DOCID, ATCH_FNAME, ATCH_EXT, ATCH_CREATEDBY, ATCH_VERSION, ATCH_FLAG, ATCH_SIZE, ATCH_FROM, ATCH_Basename, ATCH_CREATEDON, 
                ATCH_Status, ATCH_CompID, Atch_Vstatus, ATCH_REPORTTYPE, ATCH_DRLID)
                VALUES (@AtchId, @DocId, @FileName, @FileExt, @CreatedBy, 1, 0, @Size, 0, 0, GETDATE(), 'X', @CompId, 'A', 0, 0);";

                await connection.ExecuteAsync(insertQuery, new
                {
                    AtchId = attachId,
                    DocId = docId,
                    FileName = safeFileName,
                    FileExt = fileExtension,
                    CreatedBy = dto.ATCH_CREATEDBY,
                    Size = fileSize,
                    CompId = dto.ATCH_COMPID
                });

                return attachId;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload the attachment document.", ex);
            }
        }

        public async Task RemoveAttachmentDocAsync(int compId, int attachId, int docId, int userId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var query = @"UPDATE EDT_ATTACHMENTS SET ATCH_Desc = @Desc, ATCH_MODIFIEDBY = @UserId WHERE ATCH_CompID = @CompId AND ATCH_DOCID = @DocId AND ATCH_ID = @AttachId";

                await connection.ExecuteAsync(query, new { CompId = compId, DocId = docId, AttachId = attachId, Desc = description, UserId = userId });
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update the attachment description.", ex);
            }
        }

        public async Task<AttachmentDownloadInfoDTO> GetAttachmentDocDetailsByIdAsync(int compId, int attachId, int docId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var result = await connection.QueryFirstOrDefaultAsync<(string FileExt, string FileName, string BasePath)>(
                    @"SELECT A.ATCH_EXT, A.ATCH_FNAME, (SELECT sad_Config_Value FROM sad_config_settings WHERE sad_Config_Key = 'ImgPath' AND sad_compid = A.ATCH_CompID) AS BasePath FROM EDT_ATTACHMENTS A 
                    WHERE A.ATCH_CompID = @CompId AND A.ATCH_ID = @AttachId AND A.ATCH_DOCID = @DocId", new { CompId = compId, AttachId = attachId, DocId = docId });

                if (string.IsNullOrWhiteSpace(result.FileExt) || string.IsNullOrWhiteSpace(result.FileName))
                    return null;

                string fileExtension = result.FileExt.Trim().ToLower();
                string basePath = result.BasePath ?? Directory.GetCurrentDirectory();
                string originalFileName = $"{result.FileName}.{fileExtension}";
                string[] imageExtensions = { "jpg", "jpeg", "png", "gif", "bmp", "tif", "tiff", "svg", "psd", "ai", "eps", "ico", "webp", "raw", "heic", "heif", "exr", "dng", "jp2", "j2k", "cr2", "nef", "orf", "arw", "raf", "rw2", "mp4", "avi", "mov", "wmv", "mkv", "flv", "webm", "m4v", "mpg", "mpeg", "3gp", "ts", "m2ts", "vob", "mts", "divx", "ogv" };
                string[] documentExtensions = { "pdf", "doc", "docx", "txt", "xls", "xlsx", "ppt", "ppsx", "pptx", "odt", "ods", "odp", "rtf", "csv", "pptm", "xlsm", "docm", "xml", "json", "yaml", "key", "numbers", "pages", "tar", "zip", "rar" };
                string fileType = imageExtensions.Contains(fileExtension) ? "Images" : documentExtensions.Contains(fileExtension) ? "Documents" : "Others";
                string sourceFolder = Path.Combine(basePath, "Audit", fileType, (docId / 301).ToString());
                string sourceFilePath = Path.Combine(sourceFolder, $"{docId}.{fileExtension}");

                if (!System.IO.File.Exists(sourceFilePath))
                    return null;

                string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "Tempfolder");
                Directory.CreateDirectory(tempFolder);

                string tempFilePath = Path.Combine(tempFolder, originalFileName);
                File.Copy(sourceFilePath, tempFilePath, true);

                return new AttachmentDownloadInfoDTO
                {
                    TempFilePath = tempFilePath,
                    OriginalFileName = originalFileName
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting the attachment for download.", ex);
            }
        }

        public async Task<byte[]> GeneratePdfAsync(EngagementPlanDetailsDTO data)
        {
            return await Task.Run(() =>
            {
                using var ms = new MemoryStream();
                var document = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(document, ms);
                document.Open();

                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var subHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                // Header
                document.Add(new Paragraph("ENGAGEMENT LETTER", headerFont)
                {
                    Alignment = Element.ALIGN_CENTER
                });
                document.Add(new Paragraph($"Reference: {data.LOE_Name}", bodyFont));
                document.Add(new Paragraph($"Service Type: {data.LOE_NatureOfService}", bodyFont));
                document.Add(Chunk.NEWLINE);

                // Scope of Work
                if (!string.IsNullOrEmpty(data.LOET_ScopeOfWork))
                {
                    document.Add(new Paragraph("Scope of Work", subHeaderFont));
                    document.Add(new Paragraph(data.LOET_ScopeOfWork, bodyFont));
                    document.Add(Chunk.NEWLINE);
                }

                // Template Sections
                foreach (var section in data.EngagementTemplateDetails)
                {
                    if (!string.IsNullOrEmpty(section.LTD_Heading))
                        document.Add(new Paragraph(section.LTD_Heading, subHeaderFont));

                    if (!string.IsNullOrEmpty(section.LTD_Decription))
                        document.Add(new Paragraph(section.LTD_Decription, bodyFont));

                    document.Add(Chunk.NEWLINE);
                }

                // Fees
                document.Add(new Paragraph("ENGAGEMENT FEES", subHeaderFont));
                document.Add(new Paragraph($"Total Fees: ₹{data.LOE_Total}", bodyFont));

                foreach (var fee in data.EngagementAdditionalFees)
                {
                    document.Add(new Paragraph($"Additional Fee - {fee.LAF_OtherExpensesName}: ₹{fee.LAF_Charges}", bodyFont));
                }

                // Footer
                document.Add(Chunk.NEWLINE);
                document.Add(new Paragraph("Sincerely,", bodyFont));
                document.Add(new Paragraph("M.S. Madhava Rao", bodyFont));
                document.Add(new Paragraph("Chartered Accountant", bodyFont));

                document.Close();
                return ms.ToArray();
            });
        }

        public async Task<byte[]> GenerateWordAsync(EngagementPlanDetailsDTO data)
        {
            return await Task.Run(() =>
            {
                using var ms = new MemoryStream();
                using var doc = DocX.Create(ms);

                doc.InsertParagraph("ENGAGEMENT LETTER")
                    .FontSize(16)
                    .Bold()
                    .Alignment = Alignment.center;

                doc.InsertParagraph($"Reference: {data.LOE_Name}").FontSize(12);
                doc.InsertParagraph($"Service Type: {data.LOE_NatureOfService}").FontSize(12);

                if (!string.IsNullOrEmpty(data.LOET_ScopeOfWork))
                {
                    doc.InsertParagraph("Scope of Work")
                        .Bold()
                        .FontSize(12)
                        .SpacingBefore(10);
                    doc.InsertParagraph(data.LOET_ScopeOfWork);
                }

                foreach (var section in data.EngagementTemplateDetails)
                {
                    if (!string.IsNullOrEmpty(section.LTD_Heading))
                        doc.InsertParagraph(section.LTD_Heading)
                            .Bold()
                            .UnderlineStyle(UnderlineStyle.singleLine);

                    if (!string.IsNullOrEmpty(section.LTD_Decription))
                        doc.InsertParagraph(section.LTD_Decription);
                }

                doc.InsertParagraph("ENGAGEMENT FEES")
                    .Bold()
                    .FontSize(12)
                    .SpacingBefore(10);
                doc.InsertParagraph($"Total Fees: ₹{data.LOE_Total}");

                foreach (var fee in data.EngagementAdditionalFees)
                {
                    doc.InsertParagraph($"Additional Fee - {fee.LAF_OtherExpensesName}: ₹{fee.LAF_Charges}");
                }

                doc.InsertParagraph("Sincerely,");
                doc.InsertParagraph("M.S. Madhava Rao").Bold();
                doc.InsertParagraph("Chartered Accountant");

                doc.Save();
                return ms.ToArray();
            });
        }
    }
}

