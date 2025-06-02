using Dapper;
using DocumentFormat.OpenXml.Office.CoverPageProps;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Data.SqlClient;
using OfficeOpenXml.Table.PivotTable;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using TracePca.Data;
using TracePca.Dto.Audit;
using TracePca.Interface.Audit;
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
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly EngagementPlanInterface _engagementPlanInterface;

        public AuditCompletionService(Trdmyus1Context dbcontext, IConfiguration configuration, EngagementPlanInterface engagementPlanInterface)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _engagementPlanInterface = engagementPlanInterface;
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

        public async Task<IEnumerable<ReportTypeDetailsDTO>> GetReportTypeDetails(int compId)
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

                string relativeFolder = Path.Combine("Uploads", "Audit").ToString();
                string absoluteFolder = Path.Combine(Directory.GetCurrentDirectory(), relativeFolder);
                string fullFilePath = Path.Combine(absoluteFolder, "Audit_Completion.pdf");
                await File.WriteAllBytesAsync(fullFilePath, fileBytes);

                return (fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating the report.", ex);
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
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var query = @"SELECT DENSE_RANK() OVER (ORDER BY SAC_CheckPointID) AS SlNo, ACM_Heading AS Heading, ACM_Checkpoint AS CheckPoints, ISNULL(SAC_Remarks, '') AS Comments,
                    CASE WHEN SAC_Annexure = 1 THEN 'Yes' WHEN SAC_Mandatory = 0 THEN 'No' ELSE '' END AS Annexures FROM StandardAudit_ScheduleCheckPointList
                    JOIN AuditType_Checklist_Master ON ACM_ID = SAC_CheckPointID
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
                throw new Exception("Error getting Conduct Audit Report details.", ex);
            }
        }

        public async Task<List<ConductAuditObservationDTO>> GetConductAuditObservationsAsync(int compId, int auditId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                string query = @"SELECT SSO_SAC_CheckPointID, ACM_Checkpoint, SSO_Observations FROM StandardAudit_ScheduleObservations
                    LEFT JOIN AuditType_Checklist_Master ON ACM_ID = SSO_SAC_CheckPointID
                    WHERE SSO_SA_ID = @AuditID AND SSO_CompID = @CompID
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
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                string query = @"SELECT SSW_ID AS PKID, SSW_WorkpaperNo AS WorkpaperNo, SSW_WorkpaperRef AS WorkpaperRef, SSW_Observation AS Deviations, SSW_Conclusion AS Conclusion, 
                    SSW_NotesSteps As Notes, SSW_ReviewerComments AS ReviewerComments, SSW_CriticalAuditMatter As CriticalAuditMatter, SSW_AttachID AS AttachID, b.usr_FullName AS CreatedBy,
                    CONVERT(VARCHAR(10), SSW_CrOn, 103) AS CreatedOn, c.usr_FullName AS ReviewedBy, ISNULL(CONVERT(VARCHAR(10), SSW_ReviewedOn, 103), '') AS ReviewedOn,
                    CASE WHEN a.SSW_TypeOfTest = 1 THEN 'Inquiry' WHEN a.SSW_TypeOfTest = 2 THEN 'Observation' WHEN a.SSW_TypeOfTest = 3 THEN 'Examination'
                        WHEN a.SSW_TypeOfTest = 4 THEN 'Inspection' WHEN a.SSW_TypeOfTest = 5 THEN 'Substantive Testing' ELSE '' END AS TypeOfTest,
                    CASE WHEN a.SSW_Status = 1 THEN 'Open' WHEN a.SSW_Status = 2 THEN 'WIP' WHEN a.SSW_Status = 3 THEN 'Closed' ELSE '' END AS Status            
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
                throw new Exception("Error loading Conduct Audit Work Papers", ex);
            }
        }

        private async Task<byte[]> GeneratePdfAsync(int compId, int auditId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
            List<ConductAuditObservationDTO> dtoCAO = await GetConductAuditObservationsAsync(compId, auditId);

            var address = await GetCustomerDetailsByColumnAsync(compId, result.CustID, "CUST_ADDRESS");
            var city = await GetCustomerDetailsByColumnAsync(compId, result.CustID, "CUST_CITY");
            var state = await GetCustomerDetailsByColumnAsync(compId, result.CustID, "CUST_STATE");
            var pin = await GetCustomerDetailsByColumnAsync(compId, result.CustID, "CUST_PIN");
            var email = await GetCustomerDetailsByColumnAsync(compId, result.CustID, "CUST_EMAIL");
            var telephone = await GetCustomerDetailsByColumnAsync(compId, result.CustID, "CUST_TELPHONE");


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
                            column.Item().AlignCenter().PaddingBottom(10).Text("Letter of Engagement").FontSize(16).Bold();
                            column.Item().Text($"Ref.No.: {dtoEP.EngagementPlanNo}").FontSize(12).Bold();
                            column.Item().Text($"Date: {dtoEP.CurrentDate:dd MMM yyyy}").FontSize(10);
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
                            column.Item().AlignCenter().PaddingBottom(10).Text("Profile / Information about the Auditee").FontSize(16).Bold();
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

                    foreach (var reportType in reportTypeList)
                    {
                        var dtoCAE = allDtoCAEs.ContainsKey(reportType.ID) ? allDtoCAEs[reportType.ID] : new List<CommunicationWithClientTemplateReportDetailsDTO>();
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

                            if (dtoCA.Any() == true)
                            {
                                column.Item().AlignCenter().PaddingBottom(10).Text("Conduct Audit Details").FontSize(14).Bold();
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
    }
}
