using System.Data;
using System.Text;
using Dapper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using MailKit.Net.Smtp;
using MailKit.Security;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Utils;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Tls;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TracePca.Data;
using TracePca.Dto;
using TracePca.Dto.Audit;
using TracePca.Interface.Audit;
using Xceed.Document.NET;
using Xceed.Words.NET;
using Body = DocumentFormat.OpenXml.Wordprocessing.Body;
using Bold = DocumentFormat.OpenXml.Wordprocessing.Bold;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
// Aliases to avoid conflicts
using WordDoc = DocumentFormat.OpenXml.Wordprocessing.Document;
using WordprocessingDocument = DocumentFormat.OpenXml.Packaging.WordprocessingDocument;
using WorkpaperDto = TracePca.Dto.Audit.WorkpaperDto;

//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;
//using TracePca.Data;
//using TracePca.Dto;
//using TracePca.Dto.Audit;
//using TracePca.Interface.Audit;
////using Xceed.Document.NET;
//using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
//using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
//using WorkpaperDto = TracePca.Dto.Audit.WorkpaperDto;

namespace TracePca.Service.Communication_with_client
{
    public class Communication : AuditInterface
    {
        private readonly IConfiguration _configuration;



        public Communication(IConfiguration configuration)
        {

            _configuration = configuration;
        }


        public async Task<string> GetDateFormatAsync(string connectionKey, int companyId, string configKey)
        {
            const string query = @"
        SELECT SAD_Config_Value 
        FROM SAD_Config_Settings 
        WHERE SAD_Config_Key = @ConfigKey AND SAD_CompID = @CompanyId";

            using var connection = new SqlConnection(_configuration.GetConnectionString(connectionKey));
            var configValue = await connection.ExecuteScalarAsync<string>(query, new
            {
                ConfigKey = configKey,
                CompanyId = companyId
            });

            return configValue switch
            {
                "1" => "dd-MMM-yy",
                "2" => "dd/MM/yyyy",
                "3" => "MM/dd/yyyy",
                "4" => "yyyy/MM/dd",
                "5" => "MMM-dd-yy",
                "6" => "MMM/dd/yy",
                "7" => "MM-dd-yyyy",
                "8" => "MMM/dd/yyyy",
                _ => string.Empty
            };
        }



        public async Task<string> GetLoeTemplateSignedOnAsync(
     string connectionStringName, int companyId, int auditTypeId, int customerId, int yearId, string dateFormat)
        {
            const string query = @"
        SELECT ISNULL(FORMAT(LOET_ApprovedOn, @DateFormat), '') AS LOET_ApprovedOn
        FROM LOE_Template
        WHERE LOET_CustomerId = @CustomerId
          AND LOET_FunctionId = @AuditTypeId
          AND LOET_CompID = @CompanyId
          AND LOET_LOEID IN (
              SELECT LOE_Id
              FROM SAD_CUST_LOE
              WHERE LOE_YearId = @YearId
                AND LOE_CustomerId = @CustomerId
          )";

            var parameters = new
            {
                DateFormat = dateFormat,
                CustomerId = customerId,
                AuditTypeId = auditTypeId,
                CompanyId = companyId,
                YearId = yearId
            };

            using var connection = new SqlConnection(_configuration.GetConnectionString(connectionStringName));
            var approvedOn = await connection.ExecuteScalarAsync<string>(query, parameters);
            return approvedOn ?? string.Empty;
        }

        public async Task<string> GetCustomerFinancialYearAsync(string connectionKey, int companyId, int customerId)
        {
            const string query = @"
        SELECT CASE 
            WHEN ISNULL(CUST_FY, '0') = 1 THEN 'Jan 1st to Dec 31st'
            WHEN ISNULL(CUST_FY, '0') = 2 THEN 'Feb 1st to Jan 31st'
            WHEN ISNULL(CUST_FY, '0') = 3 THEN 'Mar 1st to Feb 28th'
            WHEN ISNULL(CUST_FY, '0') = 4 THEN 'Apr 1st to May 31st'
            WHEN ISNULL(CUST_FY, '0') = 5 THEN 'May 1st to Apr 30th'
            WHEN ISNULL(CUST_FY, '0') = 6 THEN 'Jun 1st to May 31st'
            WHEN ISNULL(CUST_FY, '0') = 7 THEN 'Jul 1st to Jun 30th'
            WHEN ISNULL(CUST_FY, '0') = 8 THEN 'Aug 1st to Jul 31st'
            WHEN ISNULL(CUST_FY, '0') = 9 THEN 'Sep 1st to Aug 31st'
            WHEN ISNULL(CUST_FY, '0') = 10 THEN 'Oct 1st to Sep 30th'
            WHEN ISNULL(CUST_FY, '0') = 11 THEN 'Nov 1st to Oct 31st'
            WHEN ISNULL(CUST_FY, '0') = 12 THEN 'Dec 1st to Jan 31st'
            ELSE '-' 
        END AS CUST_FY_Text
        FROM SAD_CUSTOMER_MASTER
        WHERE CUST_ID = @CustomerId AND CUST_CompID = @CompanyId";

            var parameters = new
            {
                CustomerId = customerId,
                CompanyId = companyId
            };

            using var connection = new SqlConnection(_configuration.GetConnectionString(connectionKey));
            var financialYear = await connection.ExecuteScalarAsync<string>(query, parameters);

            return financialYear ?? "-";
        }



        public async Task<IEnumerable<Dto.Audit.CustomerDto>> GetCustomerLoeAsync(int companyId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"SELECT LOE_ID as CustomerID, LOE_Name as CustomerName
                     FROM SAD_CUST_LOE
                     WHERE LOE_CustomerId = @CustomerId";

            return await connection.QueryAsync<Dto.Audit.CustomerDto>(query, new { CompanyId = companyId });
        }

        public async Task<IEnumerable<ReportData>> GetReportTypesAsync(string connectionKey, int companyId)
        {
            const string query = @"
        SELECT RTM_Id AS ReportId , RTM_ReportTypeName AS  ReportName
        FROM SAD_ReportTypeMaster
        WHERE RTM_TemplateId = 1
          AND RTM_DelFlag = 'A'
          AND RTM_CompID = @CompId
        ORDER BY RTM_ReportTypeName";

            using var connection = new SqlConnection(_configuration.GetConnectionString(connectionKey));

            var parameters = new { CompId = companyId };
            return await connection.QueryAsync<ReportData>(query, parameters);
        }



        public async Task<IEnumerable<AuditTypeDto>> GetAuditTypesAsync(int companyId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"
        SELECT cmm_ID AS CmmId, 
               cmm_Desc AS CmmDesc
        FROM Content_Management_Master
        WHERE CMM_CompID = @CompanyId
          AND CMM_Category = 'AuditType'
          AND CMM_Delflag = 'A'
        ORDER BY cmm_Desc ASC";

            return await connection.QueryAsync<AuditTypeDto>(query, new { CompanyId = companyId });
        }

        public async Task<CustomerAuditDropdownDto> GetCustomerAuditDropdownAsync(int companyId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            // Run both queries
            var customerLoeQuery = @"SELECT LOE_ID  LOE_Name as CustomerName
                             FROM SAD_CUST_LOE
                             WHERE LOE_CompID = @CompanyID";

            var auditTypeQuery = @"SELECT cmm_ID AS AuditId, cmm_Desc AS AuditDesc
                           FROM Content_Management_Master
                           WHERE CMM_CompID = @CompanyId
                             AND CMM_Category = 'AuditType'
                             AND CMM_Delflag = 'A'
                           ORDER BY cmm_Desc ASC";

            var customerLoes = await connection.QueryAsync<Dto.Audit.CustomerDto>(customerLoeQuery, new { CompanyId = companyId });
            var auditTypes = await connection.QueryAsync<AuditTypeDto>(auditTypeQuery, new { CompanyId = companyId });

            return new CustomerAuditDropdownDto
            {
                CustomerLoes = customerLoes,
                AuditTypes = auditTypes
            };
        }


        public async Task<IEnumerable<Dto.Audit.CustomerDto>> LoadActiveCustomersAsync(int companyId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var query = @"
        SELECT Cust_Id, Cust_Name 
        FROM SAD_CUSTOMER_MASTER 
        WHERE CUST_DelFlg = 'A' AND Cust_CompId = @CompanyId 
        ORDER BY Cust_Name";

            return await connection.QueryAsync<Dto.Audit.CustomerDto>(query, new { CompanyId = companyId });
        }


        public async Task<IEnumerable<AuditScheduleDto>> LoadScheduledAuditNosAsync(
     string connectionStringName, int companyId, int financialYearId, int customerId)
        {
            var connectionString = _configuration.GetConnectionString(connectionStringName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var sql = @"
        SELECT SA_ID, SA_AuditNo + ' - ' + CMM_Desc AS SA_AuditNo
        FROM StandardAudit_Schedule
        LEFT JOIN Content_Management_Master ON CMM_ID = SA_AuditTypeID
        WHERE SA_CompID = @CompanyId";

            if (financialYearId > 0)
                sql += " AND SA_YearID = @FinancialYearId";

            if (customerId > 0)
                sql += " AND SA_CustID = @CustomerId";

            sql += " ORDER BY SA_ID DESC";

            return await connection.QueryAsync<AuditScheduleDto>(sql, new
            {
                CompanyId = companyId,
                FinancialYearId = financialYearId,
                CustomerId = customerId
            });
        }


        public async Task<IEnumerable<ReportTypeDto>> LoadAllReportTypeDetailsDRLAsync(
      string connectionStringName, int companyId, int templateId, string auditNo)
        {
            var connectionString = _configuration.GetConnectionString(connectionStringName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Step 1: Determine Audit Report Type
            var auditTypeQuery = @"
        SELECT CASE 
            WHEN CHARINDEX('Q', @AuditNo) > 0 THEN 1
            WHEN CHARINDEX('KY', @AuditNo) > 0 THEN 2
            ELSE 3 
        END";

            var audRptType = await connection.ExecuteScalarAsync<int>(auditTypeQuery, new { AuditNo = auditNo });

            // Step 2: Get Report Types
            var sql = @"
        SELECT RTM_Id, RTM_ReportTypeName
        FROM SAD_ReportTypeMaster
        WHERE RTM_CompID = @CompanyId
          AND RTM_DelFlag = 'A'
          AND RTM_AudrptType IN (3, @AudRptType)";

            if (templateId > 0)
                sql += " AND RTM_TemplateId = @TemplateId";

            sql += " ORDER BY RTM_Id";

            var parameters = new
            {
                CompanyId = companyId,
                AudRptType = audRptType,
                TemplateId = templateId
            };

            return await connection.QueryAsync<ReportTypeDto>(sql, parameters);
        }

        public async Task<IEnumerable<CustomerUserEmailDto>> GetCustAllUserEmailsAsync(
    string connectionStringName, int companyId, int customerId)
        {
            var connectionString = _configuration.GetConnectionString(connectionStringName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var sql = @"
        SELECT Usr_ID, Usr_Email
        FROM Sad_UserDetails
        WHERE Usr_Companyid = @CustomerId
          AND Usr_CompId = @CompanyId
          AND (Usr_DelFlag = 'A' OR Usr_DelFlag = 'B' OR Usr_DelFlag = 'L')
          AND Usr_Email IS NOT NULL
          AND Usr_Email <> ''
        ORDER BY Usr_Email";

            return await connection.QueryAsync<CustomerUserEmailDto>(sql, new
            {
                CompanyId = companyId,
                CustomerId = customerId
            });
        }


        public async Task<IEnumerable<YearDto>> GetAddYearTo2DigitFinancialYearAsync(
    string connectionStringName, int companyId, int incrementBy)
        {
            var connectionString = _configuration.GetConnectionString(connectionStringName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Step 1: Get the default YearID
            var getDefaultYearIdSql = @"
        SELECT YMS_YearID 
        FROM Year_Master 
        WHERE YMS_Default = 1 
          AND YMS_CompID = @CompanyId 
          AND YMS_DelFlag = 'A'";

            var defaultYearId = await connection.ExecuteScalarAsync<int>(getDefaultYearIdSql, new { CompanyId = companyId });

            // Step 2: Get Year List based on range
            var getYearsSql = @"
        SELECT YMS_ID, YMS_YearID 
        FROM Year_Master 
        WHERE YMS_YearID <= @MaxYearId
          AND YMS_YearID > 8 
          AND YMS_CompID = @CompanyId 
          AND YMS_DelFlag = 'A'
        ORDER BY YMS_YearID DESC";

            return await connection.QueryAsync<YearDto>(getYearsSql, new
            {
                CompanyId = companyId,
                MaxYearId = defaultYearId + incrementBy
            });
        }

        public async Task<int> GetDuringSelfAttachIdAsync(
    string connectionStringName, int companyId, int yearId, int customerId, int auditId, int drlId)
        {
            var connectionString = _configuration.GetConnectionString(connectionStringName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var sql = @"
        SELECT a.SAR_AttchId 
        FROM EDT_attachments 
        LEFT JOIN StandardAudit_Audit_DRLLog_RemarksHistory a 
            ON a.SAR_AtthachDocId = ATCH_DOCID 
        WHERE SAR_SA_ID = @AuditId 
          AND SAR_SAC_ID = @CustomerId 
          AND SAR_CompID = @CompanyId 
          AND a.SAR_DRLId = @DrlId ";

            var result = await connection.ExecuteScalarAsync<int?>(sql, new
            {
                AuditId = auditId,
                CustomerId = customerId,
                CompanyId = companyId,
                DrlId = drlId
            });

            return result ?? 0; // Return 0 if no value is found
        }

        public async Task<IEnumerable<DrlDescListDto>> LoadAllDRLDescriptionsAsync(string connectionStringName, int companyId)
        {
            var sQuery = @"
    SELECT 
        CMM_ID AS DrlId,
        ISNULL(Cms_Remarks, '') AS Cms_Remarks
    FROM Content_Management_Master
    WHERE CMM_CompID = @CompanyId";

            var connectionString = _configuration.GetConnectionString(connectionStringName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryAsync<DrlDescListDto>(sQuery, new { CompanyId = companyId });
            return result;
        }


        public async Task<DrlDescReqDto> LoadDRLDescriptionAsync(string connectionStringName, int companyId, int drlId)
        {
            var sQuery = @"
        SELECT ISNULL(Cms_Remarks, '') AS Cms_Remarks 
        FROM Content_Management_Master 
        WHERE CMM_CompID = @CompanyId AND CMM_ID = @DrlId";

            var connectionString = _configuration.GetConnectionString(connectionStringName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryFirstOrDefaultAsync<DrlDescReqDto>(sQuery, new
            {
                CompanyId = companyId,
                DrlId = drlId
            });

            return result ?? new DrlDescReqDto { Cms_Remarks = string.Empty };
        }

        public async Task<string> GetUserFullNameAsync(string connectionStringName, int companyId, int userId)
        {
            var connectionString = _configuration.GetConnectionString(connectionStringName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
            SELECT ISNULL(usr_FullName, '') 
            FROM Sad_UserDetails
            WHERE usr_Id = @UserId AND usr_CompanyId = @CompanyId";

            var fullName = await connection.ExecuteScalarAsync<string>(query, new
            {
                UserId = userId,
                CompanyId = companyId
            });

            return fullName ?? string.Empty;
        }

        public async Task<int> GetRequestedIdAsync(int exportType)
        {
            var query = @"
        SELECT ISNULL(cmm_ID, 0)
        FROM Content_Management_Master
        WHERE cmm_Category = 'DRL'
          AND cmm_Desc = @Desc";

            var cmmDesc = exportType == 1
                ? "Beginning of the Audit"
                : "Nearing completion of the Audit";

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryFirstOrDefaultAsync<int>(query, new { Desc = cmmDesc });
            return result;
        }

        public async Task<int> CheckAndGetDRLPKIDAsync(int yearId, int customerId, int auditNo, int requestedId)
        {
            var query = @"
        SELECT ISNULL(ADRL_ID, 0)
        FROM Audit_DRLLog
        WHERE ADRL_YearID = @YearId
          AND ADRL_CustID = @CustomerId
          AND ADRL_AuditNo = @AuditNo
          AND ADRL_RequestedListID = @RequestedId";

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryFirstOrDefaultAsync<int>(query, new
            {
                YearId = yearId,
                CustomerId = customerId,
                AuditNo = auditNo,
                RequestedId = requestedId
            });

            return result;
        }

        public async Task<int> SaveDRLLogAsync(DRLLogDto dto)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (dto.Id == 0)
            {
                var insert = @"
            INSERT INTO Audit_DRLLog (
                ADRL_YearID, ADRL_AuditNo, ADRL_CustID,
                ADRL_RequestedListID, ADRL_RequestedTypeID,
                ADRL_RequestedOn, ADRL_TimlinetoResOn,
                ADRL_EmailID, ADRL_Comments,
                ADRL_CrBy, ADRL_UpdatedBy,
                ADRL_IPAddress, ADRL_CompID, ADRL_ReportType
            ) VALUES (
                @YearId, @AuditNo, @CustomerId,
                @RequestedListId, @RequestedTypeId,
                @RequestedOn, @TimelineToRespond,
                @EmailIds, @Comments,
                @CreatedBy, @UpdatedBy,
                @IPAddress, @CompanyId, @ReportType
            );
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

                return await connection.ExecuteScalarAsync<int>(insert, new
                {
                    dto.YearId,
                    dto.AuditNo,
                    dto.CustomerId,
                    dto.RequestedListId,
                    dto.RequestedTypeId,
                    dto.RequestedOn,
                    dto.TimelineToRespond,
                    dto.EmailIds,
                    dto.Comments,
                    CreatedBy = dto.CreatedBy,
                    UpdatedBy = dto.UpdatedBy,
                    IPAddress = dto.IPAddress,
                    CompanyId = dto.CompanyId,
                    ReportType = dto.ReportType
                });
            }
            else
            {
                var update = @"
            UPDATE Audit_DRLLog SET
                ADRL_RequestedOn = @RequestedOn,
                ADRL_TimlinetoResOn = @TimelineToRespond,
                ADRL_EmailID = @EmailIds,
                ADRL_Comments = @Comments,
                ADRL_UpdatedBy = @UpdatedBy,
                ADRL_ReportType = @ReportType
            WHERE ADRL_ID = @Id";

                await connection.ExecuteAsync(update, new
                {
                    dto.RequestedOn,
                    dto.TimelineToRespond,
                    dto.EmailIds,
                    dto.Comments,
                    UpdatedBy = dto.UpdatedBy,
                    ReportType = dto.ReportType,
                    dto.Id
                });

                return dto.Id;
            }
        }
        public async Task<CustomerInvoiceDto> GetCustomerDetailsForInvoiceAsync(int companyId, int customerId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Step 1: Get base customer details
            var customerQuery = @"
            SELECT CUST_ID, CUST_NAME, CUST_ADDRESS, CUST_CITY, CUST_STATE, CUST_PIN, CUST_EMAIL, CUST_TELPHONE
            FROM SAD_CUSTOMER_MASTER
            WHERE CUST_ID = @CustomerId AND CUST_CompID = @CompanyId";

            var customer = await connection.QueryFirstOrDefaultAsync(customerQuery, new
            {
                CustomerId = customerId,
                CompanyId = companyId
            });


            if (customer == null)
                return null;

            // Step 2: Get GSTIN
            var gstinQuery = @"
            SELECT Cust_Value 
            FROM SAD_CUST_Accounting_Template 
            WHERE Cust_ID = @CustomerId AND Cust_Desc = 'GSTIN'";

            var gstin = await connection.ExecuteScalarAsync<string>(gstinQuery, new { CustomerId = customerId });

            // Step 3: Build and return the DTO
            return new CustomerInvoiceDto
            {
                CustName = customer.CUST_NAME,
                CustAddress = customer.CUST_ADDRESS,
                CustCityPin = $"{customer.CUST_CITY} {customer.CUST_PIN}",
                CustState = $"State: {customer.CUST_STATE}",
                CustEmail = customer.CUST_EMAIL,
                CustTelephone = customer.CUST_TELPHONE,
                CustGSTIN = $"GSTIN Number: {gstin ?? string.Empty}"
            };
        }


        //public async Task<GenerateDRLReportResultDto> GenerateDRLReportAsync(GenerateDRLReportRequestDto request)
        //{
        //    // Step 1: Get Requested ID
        //    int requestedId = await GetRequestedIdAsync(request.ExportType);

        //    // Step 2: Check for existing record
        //    int existingId = await CheckAndGetDRLPKIDAsync(request.YearId, request.CustomerId, request.AuditNo, requestedId);

        //    // Step 3: Insert or Update
        //    var logDto = new DRLLogDto
        //    {
        //        Id = existingId,
        //        YearId = request.YearId,
        //        AuditNo = request.AuditNo,
        //        CustomerId = request.CustomerId,
        //        RequestedListId = requestedId,
        //        RequestedTypeId = request.RequestedTypeId,
        //        RequestedOn = DateTime.Now,
        //        TimelineToRespond = request.TimelineToRespond,
        //        EmailIds = request.EmailIds,
        //        Comments = request.Comments,
        //        CreatedBy = request.CreatedBy,
        //        UpdatedBy = request.UpdatedBy,
        //        IPAddress = request.IPAddress,
        //        CompanyId = request.CompanyId,
        //        ReportType = request.ReportType
        //    };

        //    int drlLogId = await SaveDRLLogAsync(logDto);

        //    // Step 4: Fetch Customer + Template Data
        //    var customerData = await GetCustomerDetailsWithTemplatesAsync(request.CompanyId, request.CustomerId, request.ReportTypeId);

        //    if (customerData == null)
        //        throw new Exception("No customer/template data found.");

        //    // Step 5: Generate Document
        //    string fileName = $"DRL_Report_{drlLogId}_{DateTime.Now:yyyyMMddHHmmss}.{(request.Format == "pdf" ? "pdf" : "docx")}";
        //    string filePath = Path.Combine("wwwroot", "generated", fileName);

        //    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        //    if (request.Format == "pdf")
        //    {
        //        DocumentGenerator.GeneratePdf(customerData, filePath);
        //    }
        //    else
        //    {
        //        DocumentGenerator.GenerateWord(customerData, filePath);
        //    }

        //    // Step 6: Return result
        //    return new GenerateDRLReportResultDto
        //    {
        //        DrlLogId = drlLogId,
        //        FilePath = $"/generated/{fileName}",
        //        Message = "Report generated successfully."
        //    };
        //}


        //public async Task<CustomerDataDto> GetCustomerDetailsWithTemplatesAsync(int companyId, int customerId, int reportTypeId)
        //{
        //    var connectionString = _configuration.GetConnectionString("DefaultConnection");

        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    // Step 1: Get base customer details
        //    var customerQuery = @"
        //SELECT CUST_ID, CUST_NAME, CUST_ADDRESS, CUST_CITY, CUST_STATE, CUST_PIN, CUST_EMAIL, CUST_TELPHONE
        //FROM SAD_CUSTOMER_MASTER
        //WHERE CUST_ID = @CustomerId AND CUST_CompID = @CompanyId";

        //    var customer = await connection.QueryFirstOrDefaultAsync(customerQuery, new
        //    {
        //        CustomerId = customerId,
        //        CompanyId = companyId
        //    });

        //    if (customer == null)
        //        return null;

        //    // Step 2: Get GSTIN
        //    var gstinQuery = @"
        //SELECT Cust_Value 
        //FROM SAD_CUST_Accounting_Template 
        //WHERE Cust_ID = @CustomerId AND Cust_Desc = 'GSTIN'";

        //    var gstin = await connection.ExecuteScalarAsync<string>(gstinQuery, new { CustomerId = customerId });

        //    var customerDto = new CustomerInvoiceDto
        //    {
        //        CustName = customer.CUST_NAME,
        //        CustAddress = customer.CUST_ADDRESS,
        //        CustCityPin = $"{customer.CUST_CITY} {customer.CUST_PIN}",
        //        CustState = $"State: {customer.CUST_STATE}",
        //        CustEmail = customer.CUST_EMAIL,
        //        CustTelephone = customer.CUST_TELPHONE,
        //        CustGSTIN = $"GSTIN Number: {gstin ?? string.Empty}"
        //    };

        //    // Step 3: Load template sections
        //    var templateQuery = @"
        //SELECT LTD_Heading AS Heading, LTD_Decription AS Description
        //FROM LOE_Template_Details
        //WHERE LTD_ReportTypeID = @ReportTypeId AND LTD_CompID = @CompanyId AND LTD_LOE_ID = (
        //    SELECT TOP 1 LOET_LOEID FROM LOE_Template
        //    WHERE LOET_CustomerId = @CustomerId AND LOET_CompID = @CompanyId
        //    ORDER BY LOET_Id DESC
        //)
        //ORDER BY LTD_ID";

        //    var templateSections = (await connection.QueryAsync<DRLTemplateItem>(templateQuery, new
        //    {
        //        ReportTypeId = reportTypeId,
        //        CompanyId = companyId,
        //        CustomerId = customerId
        //    })).ToList();

        //    return new CustomerDataDto
        //    {
        //        Customer = customerDto,
        //        TemplateSections = templateSections
        //    };
        //}

        public async Task<(string WordFilePath, string PdfFilePath)> GenerateCustomerReportFilesAsync(int companyId, int customerId, int reportTypeId)
        {
            var customerData = await GetCustomerDetailsWithTemplatesAsync(companyId, customerId, reportTypeId);

            if (customerData == null)
                throw new Exception("Customer data not found.");

            string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedReports");
            Directory.CreateDirectory(outputDir);

            string baseFileName = $"CustomerReport_{customerId}_{DateTime.Now:yyyyMMddHHmmss}";
            string wordFilePath = Path.Combine(outputDir, baseFileName + ".docx");
            string pdfFilePath = Path.Combine(outputDir, baseFileName + ".pdf");

            GenerateWord(customerData, wordFilePath);
            GeneratePdf(customerData, pdfFilePath);

            return (wordFilePath, pdfFilePath);
        }

        // Existing method reused
        public async Task<CustomerDataDto> GetCustomerDetailsWithTemplatesAsync(int companyId, int customerId, int reportTypeId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var customerQuery = @"
            SELECT CUST_ID, CUST_NAME, CUST_ADDRESS, CUST_CITY, CUST_STATE, CUST_PIN, CUST_EMAIL, CUST_TELPHONE
            FROM SAD_CUSTOMER_MASTER
            WHERE CUST_ID = @CustomerId AND CUST_CompID = @CompanyId";

            var customer = await connection.QueryFirstOrDefaultAsync(customerQuery, new
            {
                CustomerId = customerId,
                CompanyId = companyId
            });

            if (customer == null)
                return null;

            var gstinQuery = @"
            SELECT Cust_Value 
            FROM SAD_CUST_Accounting_Template 
            WHERE Cust_ID = @CustomerId AND Cust_Desc = 'GSTIN'";

            var gstin = await connection.ExecuteScalarAsync<string>(gstinQuery, new { CustomerId = customerId });

            var customerDto = new CustomerInvoiceDto
            {
                CustName = customer.CUST_NAME,
                CustAddress = customer.CUST_ADDRESS,
                CustCityPin = $"{customer.CUST_CITY} {customer.CUST_PIN}",
                CustState = $"State: {customer.CUST_STATE}",
                CustEmail = customer.CUST_EMAIL,
                CustTelephone = customer.CUST_TELPHONE,
                CustGSTIN = $"GSTIN Number: {gstin ?? string.Empty}"
            };

            var templateQuery = @"
            SELECT LTD_Heading AS Heading, LTD_Decription AS Description
            FROM LOE_Template_Details
            WHERE LTD_ReportTypeID = @ReportTypeId AND LTD_CompID = @CompanyId AND LTD_LOE_ID = (
                SELECT TOP 1 LOET_LOEID FROM LOE_Template
                WHERE LOET_CustomerId = @CustomerId AND LOET_CompID = @CompanyId
                ORDER BY LOET_Id DESC
            )
            ORDER BY LTD_ID";

            var templateSections = (await connection.QueryAsync<DRLTemplateItem>(templateQuery, new
            {
                ReportTypeId = reportTypeId,
                CompanyId = companyId,
                CustomerId = customerId
            })).ToList();

            return new CustomerDataDto
            {
                Customer = customerDto,
                TemplateSections = templateSections
            };
        }

        // Word (Xceed.Words.NET)
        private void GenerateWord(CustomerDataDto data, string filePath)
        {
            using var doc = DocX.Create(filePath);

            doc.InsertParagraph("Customer Report")
                .FontSize(18)
                .Bold()
                .UnderlineStyle(UnderlineStyle.singleLine)
                .SpacingAfter(10);

            doc.InsertParagraph($"Name: {data.Customer.CustName}");
            doc.InsertParagraph($"Address: {data.Customer.CustAddress}");
            doc.InsertParagraph($"City/Pin: {data.Customer.CustCityPin}");
            doc.InsertParagraph($"{data.Customer.CustState}");
            doc.InsertParagraph($"Email: {data.Customer.CustEmail}");
            doc.InsertParagraph($"Phone: {data.Customer.CustTelephone}");
            doc.InsertParagraph($"{data.Customer.CustGSTIN}");

            doc.InsertParagraph("Template Sections")
                 .FontSize(16)
                .Bold()
                .SpacingBefore(15);

            foreach (var section in data.TemplateSections)
            {
                doc.InsertParagraph(section.Heading).Bold();
                doc.InsertParagraph(section.Description).SpacingAfter(10);
            }

            doc.Save();
        }

        // PDF (QuestPDF)
        private void GeneratePdf(CustomerDataDto data, string filePath)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;
            QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Content()
                        .Column(col =>
                        {
                            col.Item().PaddingBottom(10).Text("Customer Report").FontSize(18).Bold().Underline();
                            col.Item().Text($"Name: {data.Customer.CustName}");
                            col.Item().Text($"Address: {data.Customer.CustAddress}");
                            col.Item().Text($"City/Pin: {data.Customer.CustCityPin}");
                            col.Item().Text($"{data.Customer.CustState}");
                            col.Item().Text($"Email: {data.Customer.CustEmail}");
                            col.Item().Text($"Phone: {data.Customer.CustTelephone}");
                            col.Item().Text($"{data.Customer.CustGSTIN}");

                            col.Item().PaddingTop(10).Text("Template Sections").FontSize(16).Bold();

                            foreach (var section in data.TemplateSections)
                            {
                                col.Item().Text(section.Heading).Bold();
                                col.Item().PaddingBottom(8).Text(section.Description);
                            }
                        });
                });
            }).GeneratePdf(filePath);
        }




        public async Task<(byte[] fileBytes, string contentType, string fileName)> GenerateAndLogDRLReportAsync(DRLRequestDto request, string format)
        {
            var requestedId = await GetRequestedIdAsync(request.ExportType);
            var existingId = await CheckAndGetDRLPKIDAsync(request.FinancialYearId, request.CustomerId, request.AuditNo, requestedId);

            var drlLog = new DRLLogDto
            {
                Id = existingId,
                YearId = request.FinancialYearId,
                AuditNo = request.AuditNo,
                CustomerId = request.CustomerId,
                RequestedListId = requestedId,
                RequestedTypeId = request.ExportType,
                RequestedOn = DateTime.Parse(request.RequestedOn),
                TimelineToRespond = DateTime.Parse(request.TimelineToRespond),
                EmailIds = request.EmailIds,
                Comments = request.Comments,
                CreatedBy = request.CreatedBy,
                UpdatedBy = request.UpdatedBy,
                IPAddress = request.IPAddress,
                CompanyId = request.CompanyId,
                ReportType = request.ReportType
            };

            await SaveDRLLogAsync(drlLog);

            byte[] fileBytes;
            string contentType;
            string fileName = request.ReferenceNumber ?? "DRL_Report";

            if (format.ToLower() == "pdf")
            {
                fileBytes = await GeneratePdfAsync(request);
                contentType = "application/pdf";
                fileName += ".pdf";
            }
            else
            {
                fileBytes = await GenerateWordAsync(request);
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                fileName += ".docx";
            }

            return (fileBytes, contentType, fileName);
        }

        private async Task<byte[]> GeneratePdfAsync(DRLRequestDto request)
        {
            return await Task.Run(() =>
            {
                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(595, 842); // A4 size manually specified
                        page.Margin(40);

                        page.Content().Column(column =>
                        {
                            column.Item().AlignCenter().PaddingBottom(10)
                                  .Text("GeneralReport").FontSize(18).Bold();

                            column.Item().Text($"Ref.No.: {request.ReferenceNumber} - {request.Title}")
                                         .FontSize(12).Bold();

                            column.Item().Text($"Date: {DateTime.Today:dd MMM yyyy}").FontSize(10);
                            column.Item().Text(request.CompanyName).FontSize(10);
                            column.Item().Text(request.Address).FontSize(10);
                            column.Item().PaddingBottom(10);

                            column.Item().PaddingBottom(10)
                                  .Text(request.Title)
                                  .FontSize(14).Bold().Underline();

                            foreach (var item in request.TemplateItems)
                            {
                                column.Item().Text("• " + item.Heading).FontSize(11).Bold();
                                if (!string.IsNullOrWhiteSpace(item.Description))
                                    column.Item().Text(item.Description).FontSize(10);
                                column.Item().PaddingBottom(5);
                            }

                            column.Item().PaddingTop(20).Text("Very truly yours,").FontSize(10);
                            column.Item().Text("M.S. Madhava Rao").FontSize(10).Bold();
                            column.Item().Text("Chartered Accountant").FontSize(10);
                        });
                    });
                });

                using var ms = new MemoryStream();
                document.GeneratePdf(ms);
                return ms.ToArray();
            });
        }



        private async Task<byte[]> GenerateWordAsync(DRLRequestDto request)
        {
            return await Task.Run(() =>
            {
                using var ms = new MemoryStream();
                using var doc = WordprocessingDocument.Create(ms, DocumentFormat.OpenXml.WordprocessingDocumentType.Document);
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new WordDoc(new Body());
                var body = mainPart.Document.Body;

                void AddParagraph(string text, bool bold = false)
                {
                    var run = new Run(new Text(text));
                    if (bold)
                        run.RunProperties = new RunProperties(new Bold());

                    body.AppendChild(new Paragraph(run));
                }

                AddParagraph("GeneralReport", true);
                AddParagraph($"Ref.No.: {request.ReferenceNumber} - {request.Title}", true);
                AddParagraph($"Date: {DateTime.Today:dd MMM yyyy}");
                AddParagraph(request.CompanyName);
                AddParagraph(request.Address);
                AddParagraph(request.Title, true);

                foreach (var item in request.TemplateItems)
                {
                    AddParagraph("• " + item.Heading, true);
                    AddParagraph(item.Description ?? "");
                }

                AddParagraph("Very truly yours,");
                AddParagraph("M.S. Madhava Rao", true);
                AddParagraph("Chartered Accountant");

                doc.Save();
                return ms.ToArray();
            });
        }
        public async Task<int> SaveDRLLogWithAttachmentAsync(DRLLogDto dto, string filePath, string fileType)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int drlId = dto.Id;

                // 1. Insert or Update Audit_DRLLog
                if (dto.Id == 0)
                {
                    var insert = @"
                INSERT INTO Audit_DRLLog (
                    ADRL_YearID, ADRL_AuditNo, ADRL_CustID,
                    ADRL_RequestedListID, ADRL_RequestedTypeID,
                    ADRL_RequestedOn, ADRL_TimlinetoResOn,
                    ADRL_EmailID, ADRL_Comments,
                    ADRL_CrBy, ADRL_UpdatedBy,
                    ADRL_IPAddress, ADRL_CompID, ADRL_ReportType
                ) VALUES (
                    @YearId, @AuditNo, @CustomerId,
                    @RequestedListId, @RequestedTypeId,
                    @RequestedOn, @TimelineToRespond,
                    @EmailIds, @Comments,
                    @CreatedBy, @UpdatedBy,
                    @IPAddress, @CompanyId, @ReportType
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    drlId = await connection.ExecuteScalarAsync<int>(insert, new
                    {
                        dto.YearId,
                        dto.AuditNo,
                        dto.CustomerId,
                        dto.RequestedListId,
                        dto.RequestedTypeId,
                        dto.RequestedOn,
                        dto.TimelineToRespond,
                        dto.EmailIds,
                        dto.Comments,
                        CreatedBy = dto.CreatedBy,
                        UpdatedBy = dto.UpdatedBy,
                        IPAddress = dto.IPAddress,
                        CompanyId = dto.CompanyId,
                        ReportType = dto.ReportType
                    }, transaction);
                }
                else
                {
                    var update = @"
                UPDATE Audit_DRLLog SET
                    ADRL_RequestedOn = @RequestedOn,
                    ADRL_TimlinetoResOn = @TimelineToRespond,
                    ADRL_EmailID = @EmailIds,
                    ADRL_Comments = @Comments,
                    ADRL_UpdatedBy = @UpdatedBy,
                    ADRL_ReportType = @ReportType
                WHERE ADRL_ID = @Id";

                    await connection.ExecuteAsync(update, new
                    {
                        dto.RequestedOn,
                        dto.TimelineToRespond,
                        dto.EmailIds,
                        dto.Comments,
                        UpdatedBy = dto.UpdatedBy,
                        ReportType = dto.ReportType,
                        dto.Id
                    }, transaction);
                }

                // 2. Fetch customer & template data
                var customerData = await GetCustomerDetailsWithTemplatesAsync(dto.CompanyId, dto.CustomerId, dto.ReportType);

                if (customerData == null)
                    throw new Exception("Customer data not found.");

                // 3. Generate Word/PDF
                var outputFolder = Path.Combine("DRLReports", dto.CompanyId.ToString(), dto.CustomerId.ToString());
                Directory.CreateDirectory(outputFolder);

                var fileName = Path.GetFileName(filePath);
                var generatedFilePath = Path.Combine(outputFolder, fileName);

                if (fileType.ToLower() == "pdf")
                    GeneratePdf(customerData, generatedFilePath);
                else
                    GenerateWord(customerData, generatedFilePath);

                // 4. Save metadata in EDT_ATTACHMENTS
                var attachId = await connection.ExecuteScalarAsync<int>(
                    @"SELECT ISNULL(MAX(ATCH_ID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_CompID = @CompanyId",
                    new { CompanyId = dto.CompanyId }, transaction);

                var docId = await connection.ExecuteScalarAsync<int>(
                    @"SELECT ISNULL(MAX(ATCH_DOCID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_CompID = @CompanyId",
                    new { CompanyId = dto.CompanyId }, transaction);

                var extension = Path.GetExtension(fileName)?.TrimStart('.') ?? "unk";
                var fileSize = new FileInfo(generatedFilePath).Length;

                var insertAttach = @"
            INSERT INTO EDT_ATTACHMENTS (
                ATCH_ID, ATCH_DOCID, ATCH_FNAME, ATCH_EXT,
                ATCH_CREATEDBY, ATCH_MODIFIEDBY, ATCH_VERSION,
                ATCH_FLAG, ATCH_SIZE, ATCH_FROM, ATCH_Basename,
                ATCH_CREATEDON, ATCH_Status, ATCH_CompID,
                Atch_Vstatus, ATCH_ReportType
            )
            VALUES (
                @AttachId, @DocId, @FileName, @Extension,
                @CreatedBy, @CreatedBy, 1,
                @Flag, @Size, 0, 0,
                GETDATE(), 'X', @CompanyId,
                'A', @ReportType
            );";

                await connection.ExecuteAsync(insertAttach, new
                {
                    AttachId = attachId,
                    DocId = docId,
                    FileName = fileName.Length > 95 ? fileName.Substring(0, 95) : fileName.Replace("&", " and"),
                    Extension = extension,
                    CreatedBy = dto.CreatedBy,
                    Flag = 1,
                    Size = fileSize,
                    CompanyId = dto.CompanyId,
                    ReportType = dto.ReportType
                }, transaction);

                transaction.Commit();
                return drlId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }



        public async Task<IEnumerable<DRLAttachmentInfoDto>> GetDRLAttachmentInfoAsync(int compId, int customerId, int drlId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
    SELECT 
        a.ATCH_ID AS AttachID,
        FORMAT(a.ATCH_CREATEDON, 'dd-MMM-yyyy hh:mm tt') AS FormattedDate
    FROM 
        EDT_ATTACHMENTS a
    INNER JOIN 
        Audit_DRLLog d ON a.ATCH_CompID = d.ADRL_CompID 
                     
    WHERE 
        a.ATCH_CompID = @CompId
        AND d.ADRL_CustID = @CustomerId
        AND d.ADRL_ID = @DrlId
        AND a.ATCH_Status = 'X'
    ORDER BY 
        a.ATCH_CREATEDON DESC";

            var result = await connection.QueryAsync<DRLAttachmentInfoDto>(query, new
            {
                CompId = compId,
                CustomerId = customerId,
                DrlId = drlId
            });

            return result;
        }



        public async Task<List<AttachmentDto>> LoadAttachmentsAsync(string connectionStringName, int companyId, int attachId, int drlId, string dateFormat)
        {
            var connectionString = _configuration.GetConnectionString(connectionStringName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
SELECT 
    Atch_DocID,
    ATCH_DRLID,
    ATCH_FNAME,
    ATCH_EXT,
    ATCH_Desc,
    ATCH_CreatedBy,
    FORMAT(ATCH_CREATEDON, @DateFormat) AS ATCH_CREATEDON,
    ATCH_SIZE,
    ATCH_ReportType,
    CASE 
        WHEN Atch_Vstatus = 'AS' THEN 'Not Shared'
        WHEN Atch_Vstatus = 'A' THEN 'Shared'
        WHEN Atch_Vstatus = 'C' THEN 'Received'
    END AS Atch_Vstatus
FROM edt_attachments
WHERE ATCH_CompID = @CompanyId 
    AND ATCH_ID = @AttachId 
    AND ATCH_DRLID = @DrlId
    AND ATCH_Status <> 'D' 
    AND Atch_Vstatus IN ('A', 'AS', 'C')
ORDER BY ATCH_CREATEDON";

            var rawData = (await connection.QueryAsync(query, new
            {
                CompanyId = companyId,
                AttachId = attachId,
                DrlId = drlId,
                DateFormat = dateFormat
            })).ToList();

            var result = new List<AttachmentDto>();
            int index = 1;

            foreach (var row in rawData)
            {
                result.Add(new AttachmentDto
                {
                    SrNo = index++,
                    AtchID = Convert.ToInt32(row.Atch_DocID),
                    DrlId = Convert.ToInt32(row.ATCH_DRLID),
                    FName = $"{row.ATCH_FNAME}.{row.ATCH_EXT}",
                    FDescription = row.ATCH_Desc?.ToString() ?? "",
                    CreatedById = Convert.ToInt32(row.ATCH_CreatedBy),
                    CreatedBy = await GetUserFullNameAsync(connectionStringName, companyId, Convert.ToInt32(row.ATCH_CreatedBy)),
                    CreatedOn = row.ATCH_CREATEDON?.ToString() ?? "",
                    FileSize = $"{(Convert.ToDouble(row.ATCH_SIZE) / 1024):0.00} KB",
                    Extention = row.ATCH_EXT?.ToString(),
                    Type = row.ATCH_ReportType?.ToString(),
                    Status = row.Atch_Vstatus?.ToString()
                });
            }

            return result;
        }

        private async Task<string> SaveAuditDocumentAsync(AddFileDto dto, int attachId, IFormFile file, int drlId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file.");

            var safeFileName = Path.GetFileName(file.FileName);
            var fileExt = Path.GetExtension(safeFileName)?.TrimStart('.');
            var relativeFolder = Path.Combine("Uploads", "Documents");
            var absoluteFolder = Path.Combine(Directory.GetCurrentDirectory(), relativeFolder);

            if (!Directory.Exists(absoluteFolder))
                Directory.CreateDirectory(absoluteFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}";
            var fullFilePath = Path.Combine(absoluteFolder, uniqueFileName);

            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var docId = await GenerateNextDocIdAsync(dto.CustomerId, dto.AuditId);

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                var maxIdQuery = "SELECT ISNULL(MAX(ATCH_ID), 0) + 1 FROM Edt_Attachments";
                var nextAttachId = await connection.ExecuteScalarAsync<int>(maxIdQuery);

                var insertQuery = @"
INSERT INTO Edt_Attachments (
    ATCH_ID, ATCH_DOCID, ATCH_FNAME, ATCH_EXT, ATCH_Desc, ATCH_SIZE,
    ATCH_AuditID, ATCH_AUDScheduleID, ATCH_CREATEDBY, ATCH_CREATEDON,
    ATCH_COMPID, ATCH_ReportType, ATCH_drlid, Atch_Vstatus
)
VALUES (
    @AtchId, @DocId, @FileName, @FileExt, @Description, @Size,
    @AuditId, @ScheduleId, @UserId, GETDATE(),
    @CompId, @ReportType, @DrlId, 'A'
);";

                await connection.ExecuteAsync(insertQuery, new
                {
                    AtchId = nextAttachId,
                    DocId = docId,
                    FileName = safeFileName,
                    FileExt = fileExt,
                    Description = dto.Remark,
                    Size = file.Length,
                    AuditId = dto.AuditId,
                    ScheduleId = dto.AuditScheduleId,
                    UserId = dto.UserId,
                    CompId = dto.CompId,
                    ReportType = dto.ReportType,
                    DrlId = drlId
                });

                return fullFilePath;
            }
        }

        
        private async Task<int> InsertIntoAuditDrlLogAsync(AddFileDto dto, int requestedId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                var drlLogId = await connection.ExecuteScalarAsync<int>(@"
            INSERT INTO Audit_DRLLog (
                ADRL_YearID, ADRL_AuditNo, ADRL_CustID, 
                ADRL_RequestedListID, ADRL_EmailID, ADRL_Comments,
                ADRL_CrBy, ADRL_IPAddress, ADRL_CompID, ADRL_ReportType, ADRL_RequestedOn
            ) VALUES (
                @YearId, @AuditId, @CustomerId,
                @RequestedId, @EmailIds, @Remarks,
                @UserId, @IpAddress, @CompId, @ReportType, GETDATE()
            );
            SELECT SCOPE_IDENTITY();",
                    new
                    {
                        YearId = dto.YearId,
                        AuditId = dto.AuditId,
                        CustomerId = dto.CustomerId,
                        RequestedId = requestedId,
                        EmailIds = dto.EmailId,
                        Remarks = dto.Remark,
                        UserId = dto.UserId,
                        IpAddress = dto.IpAddress,
                        CompId = dto.CompId,
                        ReportType = dto.ReportType
                    });

                return Convert.ToInt32(drlLogId);
            }
        }


        private async Task<int> GenerateNextDocIdAsync(int customerId, int auditId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                return await connection.ExecuteScalarAsync<int>(
                    @"SELECT ISNULL(MAX( ATCH_DOCID), 0) + 1 FROM Edt_Attachments 
              WHERE ATCH_COMPID = @CustomerId AND ATCH_AuditID = @AuditId",
                    new { customerId, auditId });
            }
        }


       

        public async Task<string> UploadAndSaveAttachmentAsync(AddFileDto dto)
        {
            try
            {
                // 1. Generate Next Attachment ID
                int attachId = await GenerateNextAttachmentIdAsync(dto.CustomerId, dto.AuditId, dto.YearId);

                // 2. Get Requested ID (based on document name)
                int requestedId = await GetRequestedIdByDocumentNameAsync(dto.DocumentName);
                int drlLogId = await InsertIntoAuditDrlLogAsync(dto, requestedId);

                // 3. Save into Audit_Document
                //  await SaveAuditDocumentAsync(dto, attachId, dto.File);

                // 4. Get latest DocId for this customer and audit
                int docId = await GetLatestDocIdAsync(dto.CustomerId, dto.AuditId);

                // 5. Generate next Remark ID
                int remarkId = await GenerateNextRemarkIdAsync(dto.CustomerId, dto.AuditId);

                // 6. Insert into Audit_DocRemarksLog
                await InsertIntoAuditDocRemarksLogAsync(dto, requestedId, remarkId, attachId, docId);

                var filePath = await SaveAuditDocumentAsync(dto, attachId, dto.File, drlLogId);

                // 7. Get DRL_PKID for updating
                int pkId = await GetDrlPkIdAsync(dto.CustomerId, dto.AuditId, attachId);

                // 8. Update DRL_AttachID and DRL_DocID in Audit_DocRemarksLog
                await UpdateDrlAttachIdAndDocIdAsync(dto.CustomerId, dto.AuditId, attachId, docId, pkId);
                await SendEmailWithAttachmentAsync("varunhallur417@gmail.com", filePath);

                return "Attachment uploaded, details saved, and email sent successfully.";


            }
            catch (Exception ex)
            {
                // Log the exception for better error handling
                return $"Error: {ex.Message}";
            }
        }
        private async Task<int> GenerateNextAttachmentIdAsync(int customerId, int auditId, int YearId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                return await connection.ExecuteScalarAsync<int>(
                    @"SELECT ISNULL(MAX(ATCH_ID), 0) + 1 
              FROM Edt_Attachments
              LEFT JOIN StandardAudit_Audit_DRLLog_RemarksHistory a 
              ON a.SAR_AttchId = Atch_Id
              WHERE SAR_SAC_ID = @CustomerId 
              AND SAR_SA_ID = @AuditId 
              AND SAR_Yearid = @YearId",
                    new { customerId, auditId, YearId });
            }
        }

        private async Task<int> GetRequestedIdByDocumentNameAsync(string documentName)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                return await connection.ExecuteScalarAsync<int>(
                    @"SELECT ISNULL(cmm_ID, 0) 
              FROM Content_Management_Master 
              LEFT JOIN Audit_DRLLog a 
              ON a.ADRL_RequestedListID = cmm_ID 
              WHERE cmm_Desc = @DocumentName",
                    new { documentName });
            }
        }



        private async Task<int> GetLatestDocIdAsync(int customerId, int auditId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                return await connection.ExecuteScalarAsync<int>(
                    @"SELECT ISNULL(MAX(SAR_ID), 0) 
              FROM StandardAudit_Audit_DRLLog_RemarksHistory 
              WHERE SAR_SAC_ID = @CustomerId AND SAR_SA_ID = @AuditId",
                    new { customerId, auditId });
            }
        }


        private async Task<int> GenerateNextRemarkIdAsync(int customerId, int auditId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                return await connection.ExecuteScalarAsync<int>(
                    @"SELECT ISNULL(MAX(SAR_ID), 0) + 1 
              FROM StandardAudit_Audit_DRLLog_RemarksHistory 
              WHERE SAR_SAC_ID = @CustomerId AND SAR_SA_ID = @AuditId",
                    new { customerId, auditId });
            }
        }


        private async Task InsertIntoAuditDocRemarksLogAsync(AddFileDto dto, int requestedId, int remarkId, int attachId, int docId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(
                    @"INSERT INTO StandardAudit_Audit_DRLLog_RemarksHistory (
                SAR_ID, SAR_SAC_ID, SAR_SA_ID, SAR_DRLId,
                SAR_Remarks, SAR_Date, SAR_RemarksType, SAR_AttchId, SAR_AtthachDocId, SAR_TimelinetoResOn
            ) VALUES (
                @RemarkId, @CustomerId, @AuditId, @RequestedId,
                @Remark, GETDATE(), @Type, @AtchId,  @DocId,  GETDATE()
            )",
                    new
                    {
                        RemarkId = remarkId,
                        CustomerId = dto.CustomerId,
                        AuditId = dto.AuditId,
                        RequestedId = requestedId,
                        Remark = dto.Remark,
                        //UserId = dto.UserId,
                        Type = dto.Type,
                        AtchId = attachId,   // use the parameter
                        DocId = docId
                    });
            }
        }


        private async Task<int> GetDrlPkIdAsync(int customerId, int auditId, int attachId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                return await connection.ExecuteScalarAsync<int>(
                    @"SELECT SAR_ID 
              FROM StandardAudit_Audit_DRLLog_RemarksHistory 
              WHERE SAR_SAC_ID = @CustomerId AND SAR_SA_ID = @AuditId AND SAR_AtthachDocId = @AttachId",
                    new { customerId, auditId, attachId });
            }
        }


        private async Task UpdateDrlAttachIdAndDocIdAsync(int customerId, int auditId, int attachId, int docId, int pkId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(
                    @"UPDATE StandardAudit_Audit_DRLLog_RemarksHistory 
              SET SAR_AttchId = @AttachId, SAR_AtthachDocId = @DocId
              WHERE SAR_SAC_ID = @CustomerId AND SAR_SA_ID = @AuditId AND SAR_ID = @PkId",
                    new { AttachId = attachId, DocId = docId, CustomerId = customerId, AuditId = auditId, PkId = pkId });
            }
        }




        private async Task SendEmailWithAttachmentAsync(string toEmail, string attachmentPath)
        {
            var fileName = Path.GetFileName(attachmentPath);
            var uploadTime = DateTime.Now.ToString("f"); // e.g., Monday, May 8, 2025 3:20 PM

            var subject = "Document Uploaded Successfully";
            var body = $@"
        <p>Dear User,</p>
        <p>The document <strong>{fileName}</strong> has been uploaded successfully on <strong>{uploadTime}</strong>.</p>
        <p>Please find the document attached with this email.</p>
        <br/>
        <p>Regards,<br/>TracePCA Team</p>
    ";

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse("harsha.s2700@gmail.com"));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body
            };

            if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
            {
                builder.Attachments.Add(attachmentPath);
            }

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync("harsha.s2700@gmail.com", "vihooiylqkwuzqtg"); // App password
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email send failed: {ex.Message}");
            }


        }

        public async Task<List<LOEHeadingDto>> LoadLOEHeadingAsync(string sFormName, int compId, int reportTypeId, int loeTemplateId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            string query1 = @"
        SELECT LTD_HeadingID AS LOEHeadingID, LTD_Heading AS LOEHeading, LTD_Decription AS LOEDesc 
        FROM LOE_Template_Details 
        WHERE LTD_LOE_ID = @LoeTemplateId 
        AND LTD_ReportTypeID = @ReportTypeId 
        AND LTD_FormName = @FormName 
        AND LTD_CompID = @CompId 
        ORDER BY LTD_ID";

            var loeDetails = (await connection.QueryAsync<LOEHeadingDto>(query1, new
            {
                LoeTemplateId = loeTemplateId,
                ReportTypeId = reportTypeId,
                FormName = sFormName,
                CompId = compId
            })).ToList();

            if (loeDetails.Any())
                return loeDetails;

            string templateQuery = @"SELECT TEM_ContentId FROM SAD_Finalisation_Report_Template WHERE TEM_FunctionId = @ReportTypeId";
            var templateContentId = await connection.ExecuteScalarAsync<string>(templateQuery, new { ReportTypeId = reportTypeId });

            string contentQuery;

            if (!string.IsNullOrEmpty(templateContentId))
            {
                contentQuery = $@"
            SELECT RCM_Id AS LOEHeadingID, RCM_Heading AS LOEHeading, RCM_Description AS LOEDesc 
            FROM SAD_ReportContentMaster 
            WHERE RCM_Id IN ({templateContentId}) 
            AND RCM_ReportId = @ReportTypeId 
            ORDER BY RCM_Id";
            }
            else
            {
                contentQuery = @"
            SELECT RCM_Id AS LOEHeadingID, RCM_Heading AS LOEHeading, RCM_Description AS LOEDesc 
            FROM SAD_ReportContentMaster 
            WHERE RCM_ReportId = @ReportTypeId 
            ORDER BY RCM_Id";
            }

            var contentData = (await connection.QueryAsync<LOEHeadingDto>(contentQuery, new { ReportTypeId = reportTypeId })).ToList();
            return contentData;
        }



        public async Task<IEnumerable<WorkpaperNoDto>> GetAuditWorkpaperNosAsync(string connectionStringName, int companyId, int auditId)
        {
            var connectionString = _configuration.GetConnectionString(connectionStringName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT SSW_ID, SSW_WorkpaperNo, SSW_WorkpaperRef
        FROM StandardAudit_ScheduleConduct_WorkPaper
        WHERE SSW_SA_ID = @AuditId AND SSW_CompID = @CompanyId
        ORDER BY SSW_ID DESC";

            var result = await connection.QueryAsync<WorkpaperNoDto>(query, new
            {
                AuditId = auditId,
                CompanyId = companyId
            });

            return result;
        }



        public async Task<IEnumerable<ChecklistItemDto>> LoadWorkpaperChecklistsAsync(string connectionStringName, int companyId)
        {
            var connectionString = _configuration.GetConnectionString(connectionStringName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
            SELECT cmm_ID, cmm_Desc
            FROM Content_Management_Master
            WHERE cmm_Delflag = 'A'
              AND cmm_Category = 'WCM'
              AND cmm_CompID = @CompanyId
            ORDER BY cmm_Desc";

            var result = await connection.QueryAsync<ChecklistItemDto>(query, new { CompanyId = companyId });
            return result;
        }

        public async Task<IEnumerable<DRLAttachmentDto>> LoadOnlyDRLWithAttachmentsAsync(string connectionStringName, int companyId, string categoryType, string auditNo, int auditId)
        {
            var connectionString = _configuration.GetConnectionString(connectionStringName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Determine Audit Report Type
            var typeQuery = @"SELECT 
                            CASE 
                                WHEN CHARINDEX('Q', @AuditNo) > 0 THEN 1
                                WHEN CHARINDEX('KY', @AuditNo) > 0 THEN 2
                                ELSE 3 
                            END";

            var auditRptType = await connection.ExecuteScalarAsync<int>(typeQuery, new { AuditNo = auditNo });

            // Main Query
            var dataQuery = @"
            SELECT CMM_ID AS PKID, CMM_Desc AS Name 
            FROM Content_Management_Master 
            WHERE 
                CMM_Category = @CategoryType 
                AND CMM_CompID = @CompanyId 
                AND cmm_AudrptType IN (3, @AuditRptType) 
                AND CMM_ID IN (
                    SELECT ADRL_RequestedListID 
                    FROM Audit_DRLLog 
                    WHERE 
                        ADRL_AuditNo = @AuditId 
                        AND ADRL_AttachID > 0 
                        AND ADRL_CompID = @CompanyId
                ) 
                AND cmm_delflag = 'A' 
            ORDER BY CMM_Desc ASC";

            var result = await connection.QueryAsync<DRLAttachmentDto>(dataQuery, new
            {
                CategoryType = categoryType,
                CompanyId = companyId,
                AuditRptType = auditRptType,
                AuditId = auditId
            });

            return result;
        }

        public async Task<bool> CheckWorkpaperRefExists(int auditId, string workpaperRef, int? workpaperId)
        {
            var query = @"
    SELECT COUNT(1) 
    FROM StandardAudit_ScheduleConduct_WorkPaper 
    WHERE SSW_SA_ID = @AuditId
    AND SSW_WorkpaperRef = @WorkpaperRef
    AND (@WorkpaperId = 0 OR SSW_ID != @WorkpaperId)";

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            int count = await connection.ExecuteScalarAsync<int>(query, new
            {
                AuditId = auditId,
                WorkpaperRef = workpaperRef,
                WorkpaperId = workpaperId ?? 0 // Default to 0 if null
            });

            return count > 0;
        }

        public async Task<string> GenerateWorkpaperNo(int auditId)
        {
            string prefix = $"WP_{auditId}_";
            string sql = "SELECT COUNT(*) FROM StandardAudit_ScheduleConduct_WorkPaper WHERE SSW_SA_ID = @AuditId";

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            int count = await connection.ExecuteScalarAsync<int>(sql, new { auditId });
            return prefix + (count + 1).ToString("D3");
        }


        public async Task<int> GetNextWorkpaperIdAsync()
        {
            string sql = "SELECT ISNULL(MAX(SSW_ID), 0) FROM StandardAudit_ScheduleConduct_WorkPaper";

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            int maxId = await connection.ExecuteScalarAsync<int>(sql);
            return maxId + 1;
        }

        public async Task<int> SaveWorkpaperAsync(WorkpaperDto dto, string workpaperNo)
        {
            bool isUpdate = dto.WorkpaperId.HasValue && dto.WorkpaperId.Value > 0;
            string sql;

            if (isUpdate)
            {
                sql = @"
        UPDATE StandardAudit_ScheduleConduct_WorkPaper
        SET
            SSW_SA_ID = @AuditId,
            SSW_WorkpaperNo = @WorkpaperNo,
            SSW_WorkpaperRef = @WorkpaperRef,
            SSW_TypeOfTest = @TypeOfTestId,
            SSW_WPCheckListID = @CheckListId,
            SSW_DRLID = @DocumentRequestId,
            SSW_ExceededMateriality = @ExceededMaterialityId,
            SSW_AuditorHoursSpent = @AuditorHoursSpent,
            SSW_Observation = @Observation,
            SSW_NotesSteps = @NotesSteps,
            SSW_CriticalAuditMatter = @CriticalAuditMatter,
            SSW_Conclusion = @Conclusion,
            SSW_Status = @StatusId,
            SSW_AttachID = @AttachId,
            SSW_UpdatedBy = @CreatedBy,
            SSW_IPAddress = @IPAddress,
            SSW_CompID = @CompanyId
        WHERE SSW_ID = @WorkpaperId;
        SELECT @WorkpaperId;";
            }
            else
            {
                dto.WorkpaperId = await GetNextWorkpaperIdAsync();
                sql = @"
        INSERT INTO StandardAudit_ScheduleConduct_WorkPaper (
            SSW_ID, SSW_SA_ID, SSW_WorkpaperNo, SSW_WorkpaperRef,
            SSW_TypeOfTest, SSW_WPCheckListID, SSW_DRLID,
            SSW_ExceededMateriality, SSW_AuditorHoursSpent, SSW_Observation,
            SSW_NotesSteps, SSW_CriticalAuditMatter, SSW_Conclusion,
            SSW_Status, SSW_AttachID, SSW_CrBy, SSW_UpdatedBy,
            SSW_IPAddress, SSW_CompID
        ) VALUES (
            @WorkpaperId, @AuditId, @WorkpaperNo, @WorkpaperRef,
            @TypeOfTestId, @CheckListId, @DocumentRequestId,
            @ExceededMaterialityId, @AuditorHoursSpent, @Observation,
            @NotesSteps, @CriticalAuditMatter, @Conclusion,
            @StatusId, @AttachId, @CreatedBy, @CreatedBy,
            @IPAddress, @CompanyId
        );
        SELECT @WorkpaperId;";
            }

            var parameters = new
            {
                dto.WorkpaperId,
                dto.AuditId,
                WorkpaperNo = workpaperNo,
                dto.WorkpaperRef,
                dto.TypeOfTestId,
                dto.CheckListId,
                dto.DocumentRequestId,
                dto.ExceededMaterialityId,
                dto.AuditorHoursSpent,
                dto.Observation,
                dto.NotesSteps,
                dto.CriticalAuditMatter,
                dto.Conclusion,
                dto.StatusId,
                dto.AttachId,
                dto.CreatedBy,
                dto.IPAddress,
                dto.CompanyId
            };

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            return await connection.ExecuteScalarAsync<int>(sql, parameters);
        }



        public async Task<IEnumerable<WorkpaperViewDto>> LoadConductAuditWorkPapersAsync(int companyId, int auditId)
        {
            var query = @"
        SELECT 
            SSW_ID AS PKID,
            SSW_WorkpaperNo AS WorkpaperNo,
            SSW_WorkpaperRef AS WorkpaperRef,
            SSW_Observation AS Observation,
            SSW_Conclusion AS Conclusion,
            SSW_ReviewerComments AS ReviewerComments,
            CASE 
                WHEN a.SSW_TypeOfTest = 1 THEN 'Inquiry'
                WHEN a.SSW_TypeOfTest = 2 THEN 'Observation'
                WHEN a.SSW_TypeOfTest = 3 THEN 'Examination'
                WHEN a.SSW_TypeOfTest = 4 THEN 'Inspection'
                WHEN a.SSW_TypeOfTest = 5 THEN 'Substantive Testing'
            END AS TypeOfTest,
            CASE 
                WHEN a.SSW_Status = 1 THEN 'Open'
                WHEN a.SSW_Status = 2 THEN 'WIP'
                WHEN a.SSW_Status = 3 THEN 'Closed'
            END AS Status,
            SSW_AttachID AS AttachID,
            b.usr_FullName AS CreatedBy,
            CONVERT(VARCHAR(10), SSW_CrOn, 103) AS CreatedOn,
            c.usr_FullName AS ReviewedBy,
            ISNULL(CONVERT(VARCHAR(10), SSW_ReviewedOn, 103), '') AS ReviewedOn
        FROM StandardAudit_ScheduleConduct_WorkPaper a
        LEFT JOIN sad_userdetails b ON b.Usr_ID = a.SSW_CrBy
        LEFT JOIN sad_userdetails c ON c.Usr_ID = a.SSW_ReviewedBy
        WHERE SSW_SA_ID = @AuditId AND SSW_CompID = @CompanyId
        ORDER BY a.SSW_WorkpaperNo DESC";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<WorkpaperViewDto>(query, new { AuditId = auditId, CompanyId = companyId });
                return result;
            }
        }


        public async Task<IEnumerable<StandardAuditHeadingDto>> LoadAllStandardAuditHeadingsAsync(int companyId, int auditId)
        {
            var query = @"
        SELECT DISTINCT(ACM_Heading),
               DENSE_RANK() OVER (ORDER BY ACM_Heading DESC) AS ACM_ID
        FROM AuditType_Checklist_Master
        WHERE ACM_ID IN (
            SELECT SAC_CheckPointID 
            FROM StandardAudit_ScheduleCheckPointList 
            WHERE SAC_SA_ID = @AuditId AND SAC_CompID = @CompanyId
        ) 
        AND ACM_CompId = @CompanyId
        AND ACM_Heading <> '' 
        AND ACM_Heading <> 'NULL'";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<StandardAuditHeadingDto>(query, new { AuditId = auditId, CompanyId = companyId });
                return result;
            }
        }


        public async Task<IEnumerable<WorkpaperNoDto>> GetConductAuditWorkpaperNosAsync(int companyId, int auditId)
        {
            var query = @"
        SELECT SSW_ID, SSW_WorkpaperNo, SSW_WorkpaperRef
        FROM StandardAudit_ScheduleConduct_WorkPaper
        WHERE SSW_SA_ID = @AuditId AND SSW_CompID = @CompanyId
        ORDER BY SSW_ID DESC";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<WorkpaperNoDto>(query, new { AuditId = auditId, CompanyId = companyId });
                return result;
            }
        }

        public async Task AssignWorkpaperToCheckPointAsync(AssignWorkpaperDto dto)
        {
            var query = @"
    UPDATE StandardAudit_ScheduleCheckPointList
    SET 
        SAC_WorkpaperID = @WorkpaperId,
        SAC_ConductedBy = @UserId,
        SAC_LastUpdatedOn = GETDATE()
    WHERE 
        SAC_ID = @SACId AND 
        SAC_SA_ID = @AuditId AND 
        SAC_CompID = @CompanyId AND 
        SAC_CheckPointID = @CheckPointId"; // This is the full flow from the source

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(query, dto);
            }
        }


        public async Task<IEnumerable<AuditCheckPointDto>> LoadSelectedAuditCheckPointDetailsAsync(
      int companyId, int auditId, int empId, bool isPartner, int headingId, string heading)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            string checkpointIds = "";
            if (!isPartner)
            {
                var sqlCheckpointIds = @"
            SELECT ISNULL(STUFF((
                SELECT ',' + CAST(SACD_CheckpointId AS VARCHAR(MAX)) 
                FROM StandardAudit_Checklist_Details 
                WHERE SACD_EmpId = @EmpId AND SACD_AuditId = @AuditId 
                FOR XML PATH('')), 1, 1, ''), '')";

                checkpointIds = await connection.ExecuteScalarAsync<string>(sqlCheckpointIds, new { EmpId = empId, AuditId = auditId });
            }

            var sql = @"
        SELECT @AuditId AS AuditID,
            DENSE_RANK() OVER (ORDER BY SAC_CheckPointID) AS SrNo,
            SAC_ID AS ConductAuditCheckPointPKId,
            SAC_CheckPointID AS CheckPointID,
            ACM_Heading AS [Heading],
            ACM_Checkpoint AS [CheckPoint],
            ISNULL(ACM_Assertions, '') AS [Assertions],
            SAC_Remarks AS [Remarks],
            CASE WHEN SAC_Mandatory = 1 THEN 'Yes' ELSE 'No' END AS [Mandatory],
            SAC_TestResult AS [TestResult],
            SAC_ReviewerRemarks AS [ReviewerRemarks],
            COALESCE(SAC_AttachID, 0) AS [AttachmentID],
            CASE WHEN SAC_Annexure = 1 THEN 'TRUE' ELSE 'FALSE' END AS [Annexure],
            a.USr_FullName AS [ConductedBy],
            CONVERT(VARCHAR(10), SAC_LastUpdatedOn, 103) AS [LastUpdatedOn],
            ISNULL(SSW_ID, 0) AS [WorkpaperId],
            ISNULL(SSW_WorkpaperNo, '') AS [WorkpaperNo],
            ISNULL(SSW_WorkpaperRef, '') AS [WorkpaperRef],
            ISNULL(b.USr_FullName, '') AS [CreatedBy],
            CASE WHEN SSW_CrOn IS NULL THEN '' ELSE CONVERT(VARCHAR(10), SSW_CrOn, 103) END AS [CreatedOn],
            (SELECT COUNT(*) FROM Audit_DRLLog 
             WHERE ADRL_AuditNo = @AuditId AND ADRL_FunID = SAC_CheckPointID) AS [DRLCount]
        FROM StandardAudit_ScheduleCheckPointList
        JOIN AuditType_Checklist_Master ON ACM_ID = SAC_CheckPointID
        LEFT JOIN sad_userdetails a ON a.Usr_ID = SAC_ConductedBy
        LEFT JOIN StandardAudit_ScheduleConduct_WorkPaper 
            ON SSW_SA_ID = @AuditId AND SSW_ID = SAC_WorkpaperID
        LEFT JOIN sad_userdetails b ON b.Usr_ID = SSW_CrBy
        WHERE SAC_SA_ID = @AuditId AND SAC_CompID = @CompanyId";

            if (headingId > 0 && !string.IsNullOrWhiteSpace(heading))
            {
                sql += @"
            AND ACM_ID IN (
                SELECT ACM_ID 
                FROM AuditType_Checklist_Master 
                WHERE ACM_Heading = @Heading 
                  AND ACM_CompId = @CompanyId 
                  AND ACM_DELFLG = 'A')";
            }

            if (!isPartner && !string.IsNullOrWhiteSpace(checkpointIds))
            {
                sql += $" AND SAC_CheckPointID IN ({checkpointIds})";
            }

            sql += " ORDER BY SAC_CheckPointID";

            var results = await connection.QueryAsync<AuditCheckPointDto>(sql, new { AuditId = auditId, CompanyId = companyId, Heading = heading });
            return results;
        }

        public async Task UpdateScheduleCheckPointRemarksAnnexureAsync(UpdateScheduleCheckPointDto dto)
        {
            try
            {
                var updateSql = new StringBuilder();
                updateSql.Append("UPDATE StandardAudit_ScheduleCheckPointList SET ");
                updateSql.Append("SAC_ConductedBy = @UserId, ");
                updateSql.Append("SAC_LastUpdatedOn = GETDATE(), ");

                // Conditional for Remarks update
                if (dto.RemarksType == 1 || dto.RemarksType == 2)
                {
                    updateSql.Append("SAC_Remarks = @Remarks, ");
                }
                else if (dto.RemarksType == 3)
                {
                    updateSql.Append("SAC_ReviewerRemarks = @Remarks, ");
                }

                // Update Annexure
                updateSql.Append("SAC_Annexure = @Annexure, ");

                // Update AuditId as well
                updateSql.Append("SAC_SA_ID = @AuditId "); // Assuming SAC_SA_ID is AuditId

                // Adding the WHERE clause to match SAC_ID (primary key), AuditId, and CheckPointId
                updateSql.Append("WHERE SAC_ID = @SACId AND SAC_CheckPointID = @CheckPointId;");

                // Parameters for SQL query
                var parameters = new
                {
                    dto.UserId,
                    dto.Remarks,
                    dto.Annexure,
                    dto.AuditId, // Update the AuditId
                    dto.SACId,   // The primary key to identify the record
                    dto.CheckPointId // The CheckPointId to identify the record
                };

                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.ExecuteAsync(updateSql.ToString(), parameters);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                throw;
            }
        }




        public async Task<string> UploadAndSaveAttachmentsAsync(AddFileDto dto)
        {
            try
            {
                // 1. Generate Next Attachment ID
                int attachId = await GenerateNextAttachmentIdAsync(dto.CustomerId, dto.AuditId, dto.YearId);

                // 2. Get Requested ID (based on document name)
                int requestedId = await GetRequestedIdByDocumentNameAsync(dto.DocumentName);

                // 3. Insert into Audit_DRLLog FIRST and get the primary key
                int drlLogId = await InsertIntoAuditDrlLogAsync(dto, requestedId);

                // 4. Save the attachment and get the path (insert into Edt_Attachments)
                var filePath = await SaveAuditDocumentAsync(dto, attachId, dto.File, drlLogId);

                // 5. Get latest DocId for this customer and audit
                int docId = await GetLatestDocIdAsync(dto.CustomerId, dto.AuditId);

                // 6. Generate next Remark ID
                int remarkId = await GenerateNextRemarkIdAsync(dto.CustomerId, dto.AuditId);

                // 7. Insert into Audit_DocRemarksLog
                await InsertIntoAuditDocRemarksLogAsync(dto, requestedId, remarkId, attachId, docId);

                // 8. Update DRL_AttachID and DRL_DocID in Audit_DRLLog
                await UpdateDrlAttachIdAndDocIdAsync(dto.CustomerId, dto.AuditId, attachId, docId, drlLogId);

                // Optional: Send email
                // await SendEmailWithAttachmentAsync("varunhallur417@gmail.com", filePath);

                return "Attachment uploaded and details saved successfully.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }


    }
}


    


        



    


    
