using System.Data;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using Azure.Core;
using Dapper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text.pdf;
using MailKit.Net.Smtp;
using MailKit.Security;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Utils;
using OfficeOpenXml.Table.PivotTable;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Tls;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TracePca.Data;
using TracePca.Dto;
using TracePca.Dto.Audit;
using TracePca.Interface.Audit;
using TracePca.Models;
using TracePca.Models.UserModels;
using Xceed.Document.NET;
using Xceed.Words.NET;
//using static Org.BouncyCastle.Math.EC.ECCurve;
using Body = DocumentFormat.OpenXml.Wordprocessing.Body;
using Bold = DocumentFormat.OpenXml.Wordprocessing.Bold;
using Document = QuestPDF.Fluent.Document;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using QuestColors = QuestPDF.Helpers.Colors;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
// Aliases to avoid conflicts
using WordDoc = DocumentFormat.OpenXml.Wordprocessing.Document;
using WordprocessingDocument = DocumentFormat.OpenXml.Packaging.WordprocessingDocument;
using WorkpaperDto = TracePca.Dto.Audit.WorkpaperDto;




using TracePca.Models;
using DocumentFormat.OpenXml.Office2010.Word;
using Microsoft.Playwright;



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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        private readonly DbConnectionProvider _dbConnectionProvider;



        public Communication(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, DbConnectionProvider dbConnectionProvider)
        {

            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
            _dbConnectionProvider = dbConnectionProvider;

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
      string connectionStringName, int companyId, int auditTypeId, int customerId, int yearId)
        {
            const string query = @"
    SELECT ISNULL(FORMAT(LOET_ApprovedOn, 'dd-MM-yyyy'), '') AS LOET_ApprovedOn
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
        public async Task<ScheduleMergedDto> GetScheduleMergedDetailsAsync(int customerId, int auditId)
        {
            const string query = @"
SELECT 
    FORMAT(SA.SA_StartDate, 'dd/MM/yyyy') + ' to ' + FORMAT(SA.SA_ExpCompDate, 'dd/MM/yyyy') AS AuditPeriod,
    SA.SA_CustID,
    SA.SA_AuditTypeID,
    SA.SA_YearID,
    CASE 
        WHEN ISNULL(CM.CUST_FY, '0') = 1 THEN 'Jan 1st to Dec 31st'
        WHEN ISNULL(CM.CUST_FY, '0') = 2 THEN 'Feb 1st to Jan 31st'
        WHEN ISNULL(CM.CUST_FY, '0') = 3 THEN 'Mar 1st to Feb 28th'
        WHEN ISNULL(CM.CUST_FY, '0') = 4 THEN 'Apr 1st to May 31st'
        WHEN ISNULL(CM.CUST_FY, '0') = 5 THEN 'May 1st to Apr 30th'
        WHEN ISNULL(CM.CUST_FY, '0') = 6 THEN 'Jun 1st to May 31st'
        WHEN ISNULL(CM.CUST_FY, '0') = 7 THEN 'Jul 1st to Jun 30th'
        WHEN ISNULL(CM.CUST_FY, '0') = 8 THEN 'Aug 1st to Jul 31st'
        WHEN ISNULL(CM.CUST_FY, '0') = 9 THEN 'Sep 1st to Aug 31st'
        WHEN ISNULL(CM.CUST_FY, '0') = 10 THEN 'Oct 1st to Sep 30th'
        WHEN ISNULL(CM.CUST_FY, '0') = 11 THEN 'Nov 1st to Oct 31st'
        WHEN ISNULL(CM.CUST_FY, '0') = 12 THEN 'Dec 1st to Jan 31st'
        ELSE '-' 
    END AS CustomerFY
FROM StandardAudit_Schedule SA
JOIN SAD_CUSTOMER_MASTER CM ON SA.SA_CustID = CM.Cust_ID
WHERE SA.SA_ID = @AuditId AND SA.SA_CustID = @CustomerId;
";

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            //  var connectionString = _configuration.GetConnectionString("DefaultConnection");

            //  using var connection = new SqlConnection(connectionString);
            //   await connection.OpenAsync();

            var partialResult = await connection.QueryFirstOrDefaultAsync<dynamic>(query, new
            {
                AuditId = auditId,
                CustomerId = customerId
            });

            if (partialResult == null) return null;

            // LOE Approved Date query (only date part)
            const string loeQuery = @"
SELECT TOP 1 FORMAT(LOET_ApprovedOn, 'dd/MM/yyyy')
FROM LOE_Template
WHERE LOET_CustomerId = @CustomerId
  AND LOET_FunctionId = @AuditTypeId
  AND LOET_LOEID IN (
      SELECT LOE_Id 
      FROM SAD_CUST_LOE 
      WHERE LOE_YearId = @YearId 
        AND LOE_CustomerId = @CustomerId
);";

            var loeApprovedOn = await connection.ExecuteScalarAsync<string>(loeQuery, new
            {
                CustomerId = (int)partialResult.SA_CustID,
                AuditTypeId = (int)partialResult.SA_AuditTypeID,
                YearId = (int)partialResult.SA_YearID
            });

            return new ScheduleMergedDto
            {
                AuditPeriod = partialResult.AuditPeriod,
                CustomerFY = partialResult.CustomerFY,
                LOET_ApprovedOn = loeApprovedOn
            };
        }





        //public async Task<ScheduleMergedDto> GetScheduleMergedDetailsAsync(int companyId, int auditId)
        //{
        //    const string query = @"
        //SELECT 
        //    SA.SA_StartDate AS StartDate,
        //    SA.SA_ExpCompDate AS EndDate,

        //    -- Customer Financial Year Description
        //    CASE 
        //        WHEN ISNULL(CM.CUST_FY, '0') = 1 THEN 'Jan 1st to Dec 31st'
        //        WHEN ISNULL(CM.CUST_FY, '0') = 2 THEN 'Feb 1st to Jan 31st'
        //        WHEN ISNULL(CM.CUST_FY, '0') = 3 THEN 'Mar 1st to Feb 28th'
        //        WHEN ISNULL(CM.CUST_FY, '0') = 4 THEN 'Apr 1st to May 31st'
        //        WHEN ISNULL(CM.CUST_FY, '0') = 5 THEN 'May 1st to Apr 30th'
        //        WHEN ISNULL(CM.CUST_FY, '0') = 6 THEN 'Jun 1st to May 31st'
        //        WHEN ISNULL(CM.CUST_FY, '0') = 7 THEN 'Jul 1st to Jun 30th'
        //        WHEN ISNULL(CM.CUST_FY, '0') = 8 THEN 'Aug 1st to Jul 31st'
        //        WHEN ISNULL(CM.CUST_FY, '0') = 9 THEN 'Sep 1st to Aug 31st'
        //        WHEN ISNULL(CM.CUST_FY, '0') = 10 THEN 'Oct 1st to Sep 30th'
        //        WHEN ISNULL(CM.CUST_FY, '0') = 11 THEN 'Nov 1st to Oct 31st'
        //        WHEN ISNULL(CM.CUST_FY, '0') = 12 THEN 'Dec 1st to Jan 31st'
        //        ELSE '-' 
        //    END AS CustomerFY,

        //    -- LOE Signed Date
        //    (
        //        SELECT TOP 1 LOET_ApprovedOn
        //        FROM LOE_Template
        //        WHERE LOET_CustomerId = SA.SA_CustID
        //          AND LOET_FunctionId = SA.SA_AuditTypeID
        //          AND LOET_CompID = @CompanyId
        //          AND LOET_LOEID IN (
        //              SELECT LOE_Id 
        //              FROM SAD_CUST_LOE 
        //              WHERE LOE_YearId = SA.SA_YearID 
        //                AND LOE_CustomerId = SA.SA_CustID
        //          )
        //    ) AS LOET_ApprovedOn

        //FROM StandardAudit_Schedule SA
        //JOIN SAD_CUSTOMER_MASTER CM ON SA.SA_CustID = CM.Cust_ID
        //WHERE SA.SA_ID = @AuditId AND SA.SA_CompID = @CompanyId AND CM.CUST_CompID = @CompanyId;
        //";

        //    var connectionString = _configuration.GetConnectionString("DefaultConnection");

        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    var result = await connection.QueryFirstOrDefaultAsync<ScheduleMergedDto>(query, new
        //    {
        //        AuditId = auditId,
        //        CompanyId = companyId
        //    });

        //    return result;
        //}




        public async Task<IEnumerable<Dto.Audit.CustomerDto>> GetCustomerLoeAsync(int companyId)
        {

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
          

          //  using var connection = _dbConnectionProvider.GetConnection();

            string query = @"SELECT LOE_ID as CustomerID, LOE_Name as CustomerName
                     FROM SAD_CUST_LOE
                     WHERE LOE_CustomerId = @CustomerId";

            return await connection.QueryAsync<Dto.Audit.CustomerDto>(query, new { CompanyId = companyId });
        }

        public async Task<IEnumerable<ReportData>> GetReportTypesAsync(int companyId)
        {
            const string query = @"
        SELECT RTM_Id AS ReportId , RTM_ReportTypeName AS  ReportName
        FROM SAD_ReportTypeMaster
        WHERE RTM_TemplateId = 1
          AND RTM_DelFlag = 'A'
          AND RTM_CompID = @CompId
        ORDER BY RTM_ReportTypeName";

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);

            //using var connection = new SqlConnection(_configuration.GetConnectionString(connectionKey));




            var parameters = new { CompId = companyId };
            return await connection.QueryAsync<ReportData>(query, parameters);
        }



        public async Task<IEnumerable<AuditTypeDto>> GetAuditTypesAsync(int companyId)
        {
            //using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);




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
            // using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);


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
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            const string query = @"
        SELECT Cust_Id, Cust_Name 
        FROM SAD_CUSTOMER_MASTER 
        WHERE CUST_DelFlg = 'A' AND Cust_CompId = @CompanyId 
        ORDER BY Cust_Name";

            return await connection.QueryAsync<Dto.Audit.CustomerDto>(query, new { CompanyId = companyId });
        }


        public async Task<IEnumerable<AuditScheduleDto>> LoadScheduledAuditNosAsync(
         int companyId, int financialYearId, int customerId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

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



        //   public async Task<IEnumerable<AuditScheduleDto>> LoadScheduledAuditNosAsync(
        //string connectionStringName, int companyId, int financialYearId, int customerId)
        //   {
        //       var connectionString = _configuration.GetConnectionString(connectionStringName);
        //       using var connection = new SqlConnection(connectionString);
        //       await connection.OpenAsync();

        //       var sql = @"
        //   SELECT SA_ID, SA_AuditNo + ' - ' + CMM_Desc AS SA_AuditNo
        //   FROM StandardAudit_Schedule
        //   LEFT JOIN Content_Management_Master ON CMM_ID = SA_AuditTypeID
        //   WHERE SA_CompID = @CompanyId";

        //       if (financialYearId > 0)
        //           sql += " AND SA_YearID = @FinancialYearId";

        //       if (customerId > 0)
        //           sql += " AND SA_CustID = @CustomerId";

        //       sql += " ORDER BY SA_ID DESC";

        //       return await connection.QueryAsync<AuditScheduleDto>(sql, new)
        //       {
        //           CompanyId = companyId,
        //           FinancialYearId = financialYearId,                                                                                         
        //           CustomerId = customerId
        //       });
        //   }


        public async Task<IEnumerable<ReportTypeDto>> LoadAllReportTypeDetailsDRLAsync(
           int companyId, int templateId, string auditNo)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

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

            // sql += " ORDER BY RTM_Id";
            sql += " ORDER BY TRY_CAST(RTM_Id AS INT) ASC";

            var parameters = new
            {
                CompanyId = companyId,
                AudRptType = audRptType,
                TemplateId = templateId
            };

            return await connection.QueryAsync<ReportTypeDto>(sql, parameters);
        }

        public async Task<IEnumerable<DropDownListDto>> LoadDRLClientSideAsync(int compId, string type, string auditNo)
        {
            //var connectionString = _configuration.GetConnectionString("DefaultConnection");


            // using var connection = new SqlConnection(connectionString);
            //await connection.OpenAsync();
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Step 1: Determine the Audit Report Type (iaudrpttype)
            const string auditTypeQuery = @"
        SELECT 
            CASE 
                WHEN CHARINDEX('Q', @AuditNo) > 0 THEN 1 
                WHEN CHARINDEX('KY', @AuditNo) > 0 THEN 2 
                ELSE 3 
            END";

            var iaudrpttype = await connection.ExecuteScalarAsync<int>(auditTypeQuery, new { AuditNo = auditNo });

            // Step 2: Fetch dropdown items from Content_Management_Master
            const string contentQuery = @"
        SELECT 
            CMM_ID AS PKID,
            CMM_Desc AS Name 
        FROM Content_Management_Master 
        WHERE 
            CMM_Category = @Type 
            AND CMM_CompID = @CompId 
            AND CMM_AudrptType IN (3, @AuditRptType)
            AND CMM_DelFlag = 'A'
        ORDER BY CMM_Desc ASC";

            var parameters = new
            {
                Type = type,
                CompId = compId,
                AuditRptType = iaudrpttype
            };

            return await connection.QueryAsync<DropDownListDto>(contentQuery, parameters);
        }

        public async Task<IEnumerable<CustomerUserEmailDto>> GetCustAllUserEmailsAsync(
     int companyId, int customerId)
        {

            //  var connectionString = _configuration.GetConnectionString(connectionStringName);
            //    using var connection = new SqlConnection(connectionString);
            //    await connection.OpenAsync();
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

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

        public async Task<IEnumerable<DrlDescListDto>> LoadAllDRLDescriptionsAsync(int companyId)
        {
            var sQuery = @"
    SELECT 
        CMM_ID AS DrlId,
        ISNULL(Cms_Remarks, '') AS Cms_Remarks
    FROM Content_Management_Master
    WHERE CMM_CompID = @CompanyId";
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);

            // var connectionString = _configuration.GetConnectionString(connectionStringName);
            //using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryAsync<DrlDescListDto>(sQuery, new { CompanyId = companyId });
            return result;
        }
        public async Task<List<(int Id, string Action)>> SaveOrUpdateLOETemplateDetailsAsync(
     List<LoETemplateDetailInputDto> dtos)
        {
            var result = new List<(int Id, string Action)>();
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);

            // using var connection = new SqlConnection(_configuration.GetConnectionString(connectionKey));
            await connection.OpenAsync();

            foreach (var dto in dtos)
            {
                if (dto.Id > 0)
                {
                    const string updateSql = @"
            UPDATE LOE_Template_Details 
            SET 
                LTD_Heading = @Heading,
                LTD_Decription = @Description,
                LTD_UpdatedBy = @UserId,
                LTD_UpdatedOn = GETDATE()
            WHERE LTD_ID = @Id";

                    await connection.ExecuteAsync(updateSql, new
                    {
                        dto.Id,
                        dto.Heading,
                        dto.Description,
                        dto.UserId
                    });

                    result.Add((dto.Id, "Updated"));
                    continue;
                }

                const string selectSql = @"
        SELECT TOP 1 LTD_ID 
        FROM LOE_Template_Details 
        WHERE 
            LTD_LOE_ID = @LoeTemplateId 
            AND LTD_ReportTypeID = @ReportTypeId 
            AND LTD_HeadingID = @HeadingId 
            AND LTD_FormName = @FormName 
            AND LTD_CompID = @CompanyId";

                var existingId = await connection.QueryFirstOrDefaultAsync<int?>(selectSql, new
                {
                    dto.LoeTemplateId,
                    dto.ReportTypeId,
                    dto.HeadingId,
                    dto.FormName,
                    dto.CompanyId
                });

                if (existingId.HasValue && existingId.Value > 0)
                {
                    const string updateSql = @"
            UPDATE LOE_Template_Details 
            SET 
                LTD_Heading = @Heading,
                LTD_Decription = @Description,
                LTD_UpdatedBy = @UserId,
                LTD_UpdatedOn = GETDATE()
            WHERE LTD_ID = @Id";

                    await connection.ExecuteAsync(updateSql, new
                    {
                        Id = existingId.Value,
                        dto.Heading,
                        dto.Description,
                        dto.UserId
                    });

                    result.Add((existingId.Value, "Updated"));
                    continue;
                }

                const string maxIdSql = "SELECT ISNULL(MAX(LTD_ID), 0) + 1 FROM LOE_Template_Details";
                var newId = await connection.ExecuteScalarAsync<int>(maxIdSql);

                const string insertSql = @"
        INSERT INTO LOE_Template_Details (
            LTD_ID, LTD_LOE_ID, LTD_ReportTypeID, LTD_HeadingID,
            LTD_Heading, LTD_Decription, LTD_FormName,
            LTD_CrBy, LTD_CrOn, LTD_IPAddress, LTD_CompID)
        VALUES (
            @Id, @LoeTemplateId, @ReportTypeId, @HeadingId,
            @Heading, @Description, @FormName,
            @UserId, GETDATE(), @IpAddress, @CompanyId)";

                await connection.ExecuteAsync(insertSql, new
                {
                    Id = newId,
                    dto.LoeTemplateId,
                    dto.ReportTypeId,
                    dto.HeadingId,
                    dto.Heading,
                    dto.Description,
                    dto.FormName,
                    dto.UserId,
                    dto.IpAddress,
                    dto.CompanyId
                });

                result.Add((newId, "Inserted"));
            }

            return result;
        }


        //public async Task<(int Id, string Action)> SaveOrUpdateLOETemplateDetailsAsync(string connectionKey, LoETemplateDetailInputDto dto)
        //{
        //    using var connection = new SqlConnection(_configuration.GetConnectionString(connectionKey));
        //    await connection.OpenAsync();

        //    // ✅ If Id is passed, treat this as an update directly
        //    if (dto.Id > 0)
        //    {
        //        const string updateSql = @"
        //    UPDATE LOE_Template_Details 
        //    SET 
        //        LTD_Heading = @Heading,
        //        LTD_Decription = @Description,
        //        LTD_UpdatedBy = @UserId,
        //        LTD_UpdatedOn = GETDATE()
        //    WHERE LTD_ID = @Id";

        //        await connection.ExecuteAsync(updateSql, new
        //        {
        //            dto.Id,
        //            dto.Heading,
        //            dto.Description,
        //            dto.UserId
        //        });

        //        return (dto.Id, "Updated");
        //    }

        //    // 🔍 Otherwise check if a matching record exists
        //    const string selectSql = @"
        //SELECT TOP 1 LTD_ID 
        //FROM LOE_Template_Details 
        //WHERE 
        //    LTD_LOE_ID = @LoeTemplateId 
        //    AND LTD_ReportTypeID = @ReportTypeId 
        //    AND LTD_HeadingID = @HeadingId 
        //    AND LTD_FormName = @FormName 
        //    AND LTD_CompID = @CompanyId";

        //    var existingId = await connection.QueryFirstOrDefaultAsync<int?>(selectSql, new
        //    {
        //        dto.LoeTemplateId,
        //        dto.ReportTypeId,
        //        dto.HeadingId,
        //        dto.FormName,
        //        dto.CompanyId
        //    });

        //    if (existingId.HasValue && existingId.Value > 0)
        //    {
        //        const string updateSql = @"
        //    UPDATE LOE_Template_Details 
        //    SET 
        //        LTD_Heading = @Heading,
        //        LTD_Decription = @Description,
        //        LTD_UpdatedBy = @UserId,
        //        LTD_UpdatedOn = GETDATE()
        //    WHERE LTD_ID = @Id";

        //        await connection.ExecuteAsync(updateSql, new
        //        {
        //            Id = existingId.Value,
        //            dto.Heading,
        //            dto.Description,
        //            dto.UserId
        //        });

        //        return (existingId.Value, "Updated");
        //    }

        //    // ➕ Insert new
        //    const string maxIdSql = "SELECT ISNULL(MAX(LTD_ID), 0) + 1 FROM LOE_Template_Details";
        //    var newId = await connection.ExecuteScalarAsync<int>(maxIdSql);

        //    const string insertSql = @"
        //INSERT INTO LOE_Template_Details (
        //    LTD_ID, LTD_LOE_ID, LTD_ReportTypeID, LTD_HeadingID,
        //    LTD_Heading, LTD_Decription, LTD_FormName,
        //    LTD_CrBy, LTD_CrOn, LTD_IPAddress, LTD_CompID)
        //VALUES (
        //    @Id, @LoeTemplateId, @ReportTypeId, @HeadingId,
        //    @Heading, @Description, @FormName,
        //    @UserId, GETDATE(), @IpAddress, @CompanyId)";

        //    await connection.ExecuteAsync(insertSql, new
        //    {
        //        Id = newId,
        //        dto.LoeTemplateId,
        //        dto.ReportTypeId,
        //        dto.HeadingId,
        //        dto.Heading,
        //        dto.Description,
        //        dto.FormName,
        //        dto.UserId,
        //        dto.IpAddress,
        //        dto.CompanyId
        //    });

        //    return (newId, "Inserted");
        //}




        //    public async Task<(int Id, string Action)> SaveOrUpdateLOETemplateDetailsAsync(string connectionKey, LoETemplateDetailInputDto dto)
        //    {
        //        using var connection = new SqlConnection(_configuration.GetConnectionString(connectionKey));
        //        await connection.OpenAsync();

        //        const string selectSql = @"
        //SELECT TOP 1 LTD_ID 
        //FROM LOE_Template_Details 
        //WHERE 
        //    LTD_LOE_ID = @LoeTemplateId 
        //    AND LTD_ReportTypeID = @ReportTypeId 
        //    AND LTD_HeadingID = @HeadingId 
        //    AND LTD_FormName = @FormName 
        //    AND LTD_CompID = @CompanyId";

        //        var existingId = await connection.QueryFirstOrDefaultAsync<int?>(selectSql, new
        //        {
        //            dto.LoeTemplateId,
        //            dto.ReportTypeId,
        //            dto.HeadingId,
        //            dto.FormName,
        //            dto.CompanyId
        //        });

        //        if (existingId.HasValue && existingId.Value > 0)
        //        {
        //            const string updateSql = @"
        //    UPDATE LOE_Template_Details 
        //    SET 
        //        LTD_Heading = @Heading,
        //        LTD_Decription = @Description,
        //        LTD_UpdatedBy = @UserId,
        //        LTD_UpdatedOn = GETDATE()
        //    WHERE LTD_ID = @Id";

        //            await connection.ExecuteAsync(updateSql, new
        //            {
        //                Id = existingId.Value,
        //                dto.Heading,
        //                dto.Description,
        //                dto.UserId
        //            });

        //            return (existingId.Value, "Updated");
        //        }
        //        else
        //        {
        //            const string maxIdSql = "SELECT ISNULL(MAX(LTD_ID), 0) + 1 FROM LOE_Template_Details";
        //            var newId = await connection.ExecuteScalarAsync<int>(maxIdSql);

        //            const string insertSql = @"
        //    INSERT INTO LOE_Template_Details (
        //        LTD_ID, LTD_LOE_ID, LTD_ReportTypeID, LTD_HeadingID,
        //        LTD_Heading, LTD_Decription, LTD_FormName,
        //        LTD_CrBy, LTD_CrOn, LTD_IPAddress, LTD_CompID)
        //    VALUES (
        //        @Id, @LoeTemplateId, @ReportTypeId, @HeadingId,
        //        @Heading, @Description, @FormName,
        //        @UserId, GETDATE(), @IpAddress, @CompanyId)";

        //            await connection.ExecuteAsync(insertSql, new
        //            {
        //                Id = newId,
        //                dto.LoeTemplateId,
        //                dto.ReportTypeId,
        //                dto.HeadingId,
        //                dto.Heading,
        //                dto.Description,
        //                dto.FormName,
        //                dto.UserId,
        //                dto.IpAddress,
        //                dto.CompanyId
        //            });

        //            return (newId, "Inserted");
        //        }
        //    }



        public async Task<DrlDescReqDto> LoadDRLDescriptionAsync(int companyId, int drlId)
        {
            var sQuery = @"
        SELECT ISNULL(Cms_Remarks, '') AS Cms_Remarks 
        FROM Content_Management_Master 
        WHERE CMM_CompID = @CompanyId AND CMM_ID = @DrlId";
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);

            //var connectionString = _configuration.GetConnectionString(connectionStringName);
            
            await connection.OpenAsync();

            var result = await connection.QueryFirstOrDefaultAsync<DrlDescReqDto>(sQuery, new
            {
                CompanyId = companyId,
                DrlId = drlId
            });

            return result ?? new DrlDescReqDto { Cms_Remarks = string.Empty };
        }

        public async Task<string> GetUserFullNameAsync(int companyId, int userId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            
            
            await connection.OpenAsync();

            var query = @"
       SELECT usr_FullName
       FROM Sad_UserDetails
       WHERE usr_Id = @UserId";

            var fullName = await connection.ExecuteScalarAsync<string>(query, new
            {
                UserId = userId,

            });

            return fullName ?? string.Empty;
        }

        public async Task<int> GetRequestedIdAsync(int exportType, string customerCode)
        {
            var query = @"
        SELECT ISNULL(cmm_ID, 0)
        FROM Content_Management_Master
        WHERE cmm_Category = 'DRL'
          AND cmm_Desc = @Desc";

            var cmmDesc = exportType == 1
                ? "Beginning of the Audit"
                : "Nearing completion of the Audit";

           var connectionString = _configuration.GetConnectionString(customerCode);
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

        public async Task<string> GetDRLDescriptionByIdAsync(int companyId, int drlId)
        {
            const string query = @"
    SELECT ISNULL(Cms_Remarks, '') 
    FROM Content_Management_Master 
    WHERE CMM_CompID = @CompanyId AND CMM_ID = @DrlId";


            var connectionString = _configuration.GetConnectionString("DefaultConnection");
             using var connection = new SqlConnection(connectionString);
         //   using var connection = _dbConnectionProvider.GetConnection();
            await connection.OpenAsync();

            var result = await connection.ExecuteScalarAsync<string>(query, new
            {
                CompanyId = companyId,
                DrlId = drlId
            });

            return result ?? string.Empty;
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
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

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
            //var connectionString = _configuration.GetConnectionString("DefaultConnection");
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

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
                    page.PageColor(QuestPDF.Helpers.Colors.White);
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




        public async Task<(byte[] fileBytes, string contentType, string fileName)> GenerateDRLReportWithoutSavingAsync(DRLRequestDto request, string format)
        {
            // ✅ Generate a safe file name
            var generatedFileName = $"DRL_{request.CustomerId}_{DateTime.Now:yyyyMMddHHmmss}.{format.ToLower()}";

            // ✅ Use system temp directory (cloud-safe)
            var outputFolder = Path.Combine(Path.GetTempPath(), "GeneratedReports");
            Directory.CreateDirectory(outputFolder); // creates the folder if it doesn't exist

            var tempFilePath = Path.Combine(outputFolder, generatedFileName);

            // ✅ Fetch required customer data
            var customerData = await GetCustomerDetailsWithTemplatesAsync(request.CompanyId, request.CustomerId, request.ReportType);
            if (customerData == null)
                throw new Exception("Customer data not found.");

            // ✅ Generate report
            if (format.ToLower() == "pdf")
                GeneratePdf(customerData, tempFilePath);
            else
                GenerateWord(customerData, tempFilePath);

            // ✅ Ensure the file was created
            if (!File.Exists(tempFilePath))
                throw new FileNotFoundException($"Expected file not found at {tempFilePath}");

            try
            {
                // ✅ Read file as bytes to return
                var fileBytes = await File.ReadAllBytesAsync(tempFilePath);

                // ✅ Set correct content type
                var contentType = format.ToLower() == "pdf"
                    ? "application/pdf"
                    : "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                return (fileBytes, contentType, generatedFileName);
            }
            finally
            {
                // ✅ Clean up the temporary file
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }




        private async Task<byte[]> GeneratePdfAsync(DRLRequestDto request)
        {
            return await Task.Run(() =>
            {
                QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = true;

                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(595, 842); // A4
                        page.Margin(40);

                        page.Content().Column(column =>
                        {
                            // Header
                            column.Item().AlignCenter().PaddingBottom(10).Text("General Report")
                                .FontSize(18).Bold();

                            column.Item().PaddingBottom(4)
                                .Text($"{request.ReferenceNumber?.Replace("\r", "").Replace("\n", "")} - {request.Title?.Replace("\r", "").Replace("\n", "")}")
                                .FontSize(12).Bold();

                            column.Item().PaddingBottom(2).Text($"Date: {DateTime.Today:dd MMM yyyy}").FontSize(10);
                            column.Item().PaddingBottom(2).Text((request.CompanyName ?? "").Replace("\r", "").Replace("\n", "")).FontSize(10);
                            column.Item().PaddingBottom(10).Text((request.Address ?? "").Replace("\r", "").Replace("\n", "")).FontSize(10);

                            // Report Title
                            column.Item().PaddingBottom(10).Text((request.Title ?? "").Replace("\r", "").Replace("\n", ""))
                                .FontSize(14).Bold().Underline();

                            // Template Items
                            foreach (var item in request.TemplateItems ?? new List<DRLTemplateItem>())
                            {
                                // Heading
                                if (!string.IsNullOrWhiteSpace(item.Heading))
                                {
                                    var headingText = "• " + item.Heading.Trim().Replace("\r", "").Replace("\n", "");
                                    column.Item().PaddingBottom(2).Text(headingText).FontSize(11).Bold();
                                }

                                // Description
                                if (!string.IsNullOrWhiteSpace(item.Description))
                                {
                                    var lines = item.Description
                                        .Replace("\r\n", "\n")
                                        .Replace("\r", "\n")
                                        .Split('\n', StringSplitOptions.RemoveEmptyEntries);

                                    foreach (var line in lines)
                                    {
                                        var trimmed = line.Trim();
                                        if (!string.IsNullOrWhiteSpace(trimmed))
                                        {
                                            var safeText = trimmed.Replace("\r", "").Replace("\n", "");
                                            column.Item().PaddingBottom(1).Text(safeText).FontSize(10);
                                        }
                                    }
                                }

                                column.Item().PaddingBottom(5);
                            }

                            // Footer
                            column.Item().PaddingTop(20).Text("Very truly yours,").FontSize(10);
                            column.Item().PaddingTop(2).Text("M.S. Madhava Rao").FontSize(10).Bold();
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
                    var run = new Run(new DocumentFormat.OpenXml.Wordprocessing.Text(text));
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

        //public async Task SendDRLReportEmailAsync(string emailTo, string attachmentFilePath)
        //{
        //    try
        //    {
        //        using (var message = new MailMessage())
        //        {
        //            message.To.Add(emailTo);
        //            message.Subject = "DRL Report Document";
        //            message.Body = @"
        //        <html>
        //        <body>
        //            <p>Dear User,</p>
        //            <p>Please find the attached DRL report document.</p>
        //            <p>Regards,<br/>Your Company</p>
        //        </body>
        //        </html>";
        //            message.IsBodyHtml = true;

        //            if (!string.IsNullOrEmpty(attachmentFilePath) && File.Exists(attachmentFilePath))
        //            {
        //                var attachment = new Attachment(attachmentFilePath);
        //                message.Attachments.Add(attachment);
        //            }

        //            using (var smtp = new SmtpClient())
        //            {
        //                smtp.Host = "your-smtp-host";         // e.g., smtp.office365.com
        //                smtp.Port = 587;                      // or 25/465
        //                smtp.EnableSsl = true;
        //                smtp.Credentials = new NetworkCredential("your-email@example.com", "your-password");

        //                await smtp.SendMailAsync(message);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Optional: Log the email error here


        //        Console.WriteLine($"Error sending DRL report email: {ex.Message}");
        //        throw;
        //    }
        //}

        public async Task<int> SaveDRLLogWithAttachmentAsync(DRLLogDto dto, string filePath, string fileType)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int drlId = dto.Id;

                if (dto.Id == 0)
                {
                    // Manually get next ADRL_ID (Absent Number)
                    var getNextIdSql = @"
                SELECT ISNULL(MAX(ADRL_ID), 0) + 1 
                FROM Audit_DRLLog WITH (TABLOCKX, HOLDLOCK);";

                    drlId = await connection.ExecuteScalarAsync<int>(getNextIdSql, transaction: transaction);
                    var existingAttachId = await connection.ExecuteScalarAsync<int>(
              @"SELECT ATCH_ID FROM EDT_ATTACHMENTS WHERE ATCH_drlid = @RequestedListId order by ATCH_ID desc",
              new { RequestedListId = dto.RequestedListId }, transaction);
                    int attachIdToMap;
                    if (existingAttachId == 0)
                    {
                        attachIdToMap = await connection.ExecuteScalarAsync<int>(
                            @"SELECT ISNULL(MAX(ATCH_ID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_CompID = @CompanyId",
                            new { CompanyId = dto.CompanyId }, transaction);
                    }
                    else
                    {
                        attachIdToMap = existingAttachId;
                    }

                    var emailIdsCsv = dto.EmailIds != null ? string.Join(",", dto.EmailIds) : null;
                    var insert = @"
                INSERT INTO Audit_DRLLog (
                    ADRL_ID,
                    ADRL_YearID, ADRL_AuditNo, ADRL_CustID,
                    ADRL_RequestedListID, ADRL_RequestedTypeID,
                    ADRL_RequestedOn, ADRL_TimlinetoResOn,
                    ADRL_EmailID, ADRL_Comments,
                    ADRL_CrBy, ADRL_UpdatedBy,
                    ADRL_IPAddress, ADRL_CompID, ADRL_ReportType, ADRL_ATTACHID
                ) VALUES (
                    @Id,
                    @YearId, @AuditNo, @CustomerId,
                    @RequestedListId, @RequestedTypeId,
                    @RequestedOn, @TimelineToRespond,
                    @EmailIds, @Comments,
                    @CreatedBy, @UpdatedBy,
                    @IPAddress, @CompanyId, @ReportType, @AttachId
                );";

                    await connection.ExecuteAsync(insert, new
                    {
                        Id = drlId,
                        dto.YearId,
                        dto.AuditNo,
                        dto.CustomerId,
                        dto.RequestedListId,
                        dto.RequestedTypeId,
                        dto.RequestedOn,
                        dto.TimelineToRespond,
                        EmailIds = emailIdsCsv,
                        dto.Comments,
                        CreatedBy = dto.CreatedBy,
                        UpdatedBy = dto.UpdatedBy,
                        IPAddress = dto.IPAddress,
                        CompanyId = dto.CompanyId,
                        ReportType = dto.ReportType,
                        AttachId = attachIdToMap
                    }, transaction);

                    dto.Id = drlId; // Update DTO with generated ID
                }
                else
                {
                    var update = @"
                UPDATE Audit_DRLLog SET
                    ADRL_RequestedOn = @RequestedOn,
                    ADRL_TimlinetoResOn = @TimelineToRespond,
                    ADRL_RequestedListID = @RequestedListId,
                    ADRL_EmailID = @EmailIds,
                    ADRL_Comments = @Comments,
                    ADRL_UpdatedBy = @UpdatedBy,
                    ADRL_ReportType = @ReportType
                WHERE ADRL_ID = @Id";
                    var emailIdsCsv = dto.EmailIds != null ? string.Join(",", dto.EmailIds) : null;


                    await connection.ExecuteAsync(update, new
                    {
                        dto.RequestedOn,
                        dto.TimelineToRespond,
                        dto.RequestedListId,
                        EmailIds = emailIdsCsv,
                        dto.Comments,
                        UpdatedBy = dto.UpdatedBy,
                        ReportType = dto.ReportType,
                        Id = dto.Id
                    }, transaction);

                    drlId = dto.Id;
                }
                //int drlId = dto.Id;

                //// 1. Insert or Update Audit_DRLLog
                //if (dto.Id == 0)
                //{
                //    var insert = @"
                //INSERT INTO Audit_DRLLog (
                //    ADRL_YearID, ADRL_AuditNo, ADRL_CustID,
                //    ADRL_RequestedListID, ADRL_RequestedTypeID,
                //    ADRL_RequestedOn, ADRL_TimlinetoResOn,
                //    ADRL_EmailID, ADRL_Comments,
                //    ADRL_CrBy, ADRL_UpdatedBy,
                //    ADRL_IPAddress, ADRL_CompID, ADRL_ReportType
                //) VALUES (
                //    @YearId, @AuditNo, @CustomerId,
                //    @RequestedListId, @RequestedTypeId,
                //    @RequestedOn, @TimelineToRespond,
                //    @EmailIds, @Comments,
                //    @CreatedBy, @UpdatedBy,
                //    @IPAddress, @CompanyId, @ReportType
                //);
                //SELECT CAST(SCOPE_IDENTITY() AS INT);";

                //    drlId = await connection.ExecuteScalarAsync<int>(insert, new
                //    {
                //        dto.YearId,
                //        dto.AuditNo,
                //        dto.CustomerId,
                //        dto.RequestedListId,
                //        dto.RequestedTypeId,
                //        dto.RequestedOn,
                //        dto.TimelineToRespond,
                //        dto.EmailIds,
                //        dto.Comments,
                //        CreatedBy = dto.CreatedBy,
                //        UpdatedBy = dto.UpdatedBy,
                //        IPAddress = dto.IPAddress,
                //        CompanyId = dto.CompanyId,
                //        ReportType = dto.ReportType
                //    }, transaction);
                //}
                //else
                //{
                //    var update = @"
                //UPDATE Audit_DRLLog SET
                //    ADRL_RequestedOn = @RequestedOn,
                //    ADRL_TimlinetoResOn = @TimelineToRespond,
                //    ADRL_RequestedListID = @RequestedListId,

                //    ADRL_EmailID = @EmailIds,
                //    ADRL_Comments = @Comments,
                //    ADRL_UpdatedBy = @UpdatedBy,
                //    ADRL_ReportType = @ReportType
                //WHERE ADRL_ID = @Id";

                //    await connection.ExecuteAsync(update, new
                //    {
                //        dto.RequestedOn,
                //        dto.TimelineToRespond,
                //        dto.EmailIds,
                //        dto.Comments,
                //        UpdatedBy = dto.UpdatedBy,
                //        ReportType = dto.ReportType,
                //        dto.Id
                //    }, transaction);
                //}

                // 2. Fetch customer & template data
                var customerData = await GetCustomerDetailsWithTemplatesAsync(dto.CompanyId, dto.CustomerId, dto.ReportType);

                if (customerData == null)
                    throw new Exception("Customer data not found.");

                // 3. Generate Word/PDF
                var outputFolder = Path.Combine("DRLReports", dto.CompanyId.ToString(), dto.CustomerId.ToString());
                Directory.CreateDirectory(outputFolder);

                var fileName = Path.GetFileName(filePath);
                var generatedFilePath = filePath; // Use the temp path passed in

                if (fileType.ToLower() == "pdf")
                    GeneratePdf(customerData, generatedFilePath);
                else
                    GenerateWord(customerData, generatedFilePath);
                // 4. Save metadata in EDT_ATTACHMENTS
                //var attachId = await connection.ExecuteScalarAsync<int>(
                //    @"SELECT ISNULL(MAX(ATCH_ID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_CompID = @CompanyId",
                //    new { CompanyId = dto.CompanyId }, transaction);

                var docId = await connection.ExecuteScalarAsync<int>(
                    @"SELECT ISNULL(MAX(ATCH_DOCID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_CompID = @CompanyId",
                    new { CompanyId = dto.CompanyId }, transaction);


                var preAttachId = await connection.ExecuteScalarAsync<int>(
                  @"SELECT ATCH_ID FROM EDT_ATTACHMENTS WHERE ATCH_drlid = @RequestedListId order by ATCH_ID desc",
                  new { RequestedListId = dto.RequestedListId }, transaction);
                var AttachId = 0;

                if (preAttachId == 0)
                {
                    AttachId = await connection.ExecuteScalarAsync<int>(
                     @"SELECT ISNULL(MAX(ATCH_ID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_CompID = @CompanyId",
                     new { CompanyId = dto.CompanyId }, transaction);
                }
                else
                {
                    AttachId = preAttachId;
                }




                var extension = Path.GetExtension(fileName)?.TrimStart('.') ?? "unk";
                var fileSize = new FileInfo(generatedFilePath).Length;

                var insertAttach = @"
            INSERT INTO EDT_ATTACHMENTS (
                ATCH_ID, ATCH_DOCID, ATCH_FNAME, ATCH_EXT,
                ATCH_CREATEDBY, ATCH_MODIFIEDBY, ATCH_VERSION,
                ATCH_FLAG, ATCH_SIZE, ATCH_FROM, ATCH_Basename,
                ATCH_CREATEDON, ATCH_Status, ATCH_CompID,
                Atch_Vstatus, ATCH_ReportType, ATCH_AuditID, 
               ATCH_drlid

            )
            VALUES (
                @AttachId, @DocId, @FileName, @Extension,
                @CreatedBy, @CreatedBy, 1,
                @Flag, @Size, 0, 0,
                GETDATE(), 'X', @CompanyId,
                'C', @ReportType, @AuditNo, @RequestedListId
            );";

                await connection.ExecuteAsync(insertAttach, new
                {
                    AttachId = AttachId,
                    DocId = docId,
                    FileName = fileName.Length > 95 ? fileName.Substring(0, 95) : fileName.Replace("&", " and"),
                    Extension = extension,
                    CreatedBy = dto.CreatedBy,
                    Flag = 1,
                    Size = fileSize,
                    CompanyId = dto.CompanyId,
                    ReportType = dto.ReportType,
                    //  Status = dto.Status,
                    AuditNo = dto.AuditNo,
                    RequestedListId = dto.RequestedListId

                }, transaction);

                // 5. Generate next Remark ID
                var remarkId = await connection.ExecuteScalarAsync<int>(
                    @"SELECT ISNULL(MAX(SAR_ID), 0) + 1 
      FROM StandardAudit_Audit_DRLLog_RemarksHistory",
                    new { dto.CustomerId, dto.AuditNo }, transaction);

                // 6. Insert into Remarks History
                await connection.ExecuteAsync(
                    @"INSERT INTO StandardAudit_Audit_DRLLog_RemarksHistory (
        SAR_ID, SAR_SAC_ID, SAR_SA_ID, SAR_DRLId,
        SAR_Remarks, SAR_Date, SAR_RemarksType, sar_Yearid,
        SAR_AttchId, SAR_AtthachDocId, SAR_TimlinetoResOn, SAR_ReportType, SAR_CompID, SAR_MASid
    ) VALUES (
        @RemarkId, @CustomerId, @AuditNo, @RequestedListId,
        @Remark, @RequestedOn, @Type, @YearId,
        @AttachId, @DocId, @TimelineToRespond, @ReportType, @CompanyId, @SarMasId
    )",
                    new
                    {
                        RemarkId = remarkId,
                        CustomerId = dto.CustomerId,
                        AuditNo = dto.AuditNo,
                        DRLId = drlId,
                        Remark = dto.Comments, // or dto.Remark if that's available
                        RequestedOn = dto.RequestedOn,
                        Type = dto.RequestedTypeId, // or another appropriate value
                        AttachId = AttachId,
                        DocId = docId,
                        TimelineToRespond = dto.TimelineToRespond,
                        ReportType = dto.ReportType,
                        RequestedListId = dto.RequestedListId,
                        YearId = dto.YearId,
                        CompanyId = dto.CompanyId,
                        SarMasId = drlId

                    }, transaction);


                transaction.Commit();
                await SendBeginningOfAuditEmailAsync(dto);

                return drlId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //        private async Task<string> GetReportTypeNameAsync(SqlConnection connection, SqlTransaction transaction, int reportTypeId)
        //        {
        //            const string query = @"
        //SELECT TOP 1 RT_ReportTypeName 
        //FROM Report_Type_Master 
        //WHERE RT_ReportTypeID = @ReportTypeId";

        //            var result = await connection.ExecuteScalarAsync<string>(query, new { ReportTypeId = reportTypeId }, transaction);
        //            return result ?? "N/A";
        //        }



        //    public async Task<int> SaveDRLLogWithAttachmentAsync(DRLLogDto dto, string filePath, string fileType)
        //    {
        //        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        //        using var connection = new SqlConnection(connectionString);
        //        await connection.OpenAsync();
        //        using var transaction = connection.BeginTransaction();

        //        try
        //        {
        //            int drlId = dto.Id;

        //            if (dto.Id == 0)
        //            {
        //                // Manually get next ADRL_ID (Absent Number)
        //                var getNextIdSql = @"
        //            SELECT ISNULL(MAX(ADRL_ID), 0) + 1 
        //            FROM Audit_DRLLog WITH (TABLOCKX, HOLDLOCK);";

        //                drlId = await connection.ExecuteScalarAsync<int>(getNextIdSql, transaction: transaction);
        //                var existingAttachId = await connection.ExecuteScalarAsync<int>(
        //          @"SELECT ATCH_ID FROM EDT_ATTACHMENTS WHERE ATCH_drlid = @RequestedListId order by ATCH_ID desc",
        //          new { RequestedListId = dto.RequestedListId }, transaction);
        //                int attachIdToMap;
        //                if (existingAttachId == 0)
        //                {
        //                    attachIdToMap = await connection.ExecuteScalarAsync<int>(
        //                        @"SELECT ISNULL(MAX(ATCH_ID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_CompID = @CompanyId",
        //                        new { CompanyId = dto.CompanyId }, transaction);
        //                }
        //                else
        //                {
        //                    attachIdToMap = existingAttachId;
        //                }

        //                var emailIdsCsv = dto.EmailIds != null ? string.Join(",", dto.EmailIds) : null;
        //                var insert = @"
        //            INSERT INTO Audit_DRLLog (
        //                ADRL_ID,
        //                ADRL_YearID, ADRL_AuditNo, ADRL_CustID,
        //                ADRL_RequestedListID, ADRL_RequestedTypeID,
        //                ADRL_RequestedOn, ADRL_TimlinetoResOn,
        //                ADRL_EmailID, ADRL_Comments,
        //                ADRL_CrBy, ADRL_UpdatedBy,
        //                ADRL_IPAddress, ADRL_CompID, ADRL_ReportType, ADRL_ATTACHID
        //            ) VALUES (
        //                @Id,
        //                @YearId, @AuditNo, @CustomerId,
        //                @RequestedListId, @RequestedTypeId,
        //                @RequestedOn, @TimelineToRespond,
        //                @EmailIds, @Comments,
        //                @CreatedBy, @UpdatedBy,
        //                @IPAddress, @CompanyId, @ReportType, @AttachId
        //            );";

        //                await connection.ExecuteAsync(insert, new
        //                {
        //                    Id = drlId,
        //                    dto.YearId,
        //                    dto.AuditNo,
        //                    dto.CustomerId,
        //                    dto.RequestedListId,
        //                    dto.RequestedTypeId,
        //                    dto.RequestedOn,
        //                    dto.TimelineToRespond,
        //                    EmailIds = emailIdsCsv,
        //                    dto.Comments,
        //                    CreatedBy = dto.CreatedBy,
        //                    UpdatedBy = dto.UpdatedBy,
        //                    IPAddress = dto.IPAddress,
        //                    CompanyId = dto.CompanyId,
        //                    ReportType = dto.ReportType,
        //                    AttachId = attachIdToMap
        //                }, transaction);

        //                dto.Id = drlId; // Update DTO with generated ID
        //            }
        //            else
        //            {
        //                var update = @"
        //            UPDATE Audit_DRLLog SET
        //                ADRL_RequestedOn = @RequestedOn,
        //                ADRL_TimlinetoResOn = @TimelineToRespond,
        //                ADRL_RequestedListID = @RequestedListId,
        //                ADRL_EmailID = @EmailIds,
        //                ADRL_Comments = @Comments,
        //                ADRL_UpdatedBy = @UpdatedBy,
        //                ADRL_ReportType = @ReportType
        //            WHERE ADRL_ID = @Id";
        //                var emailIdsCsv = dto.EmailIds != null ? string.Join(",", dto.EmailIds) : null;


        //                await connection.ExecuteAsync(update, new
        //                {
        //                    dto.RequestedOn,
        //                    dto.TimelineToRespond,
        //                    dto.RequestedListId,
        //                    EmailIds = emailIdsCsv,
        //                    dto.Comments,
        //                    UpdatedBy = dto.UpdatedBy,
        //                    ReportType = dto.ReportType,
        //                    Id = dto.Id
        //                }, transaction);

        //                drlId = dto.Id;
        //            }
        //            //int drlId = dto.Id;

        //            //// 1. Insert or Update Audit_DRLLog
        //            //if (dto.Id == 0)
        //            //{
        //            //    var insert = @"
        //            //INSERT INTO Audit_DRLLog (
        //            //    ADRL_YearID, ADRL_AuditNo, ADRL_CustID,
        //            //    ADRL_RequestedListID, ADRL_RequestedTypeID,
        //            //    ADRL_RequestedOn, ADRL_TimlinetoResOn,
        //            //    ADRL_EmailID, ADRL_Comments,
        //            //    ADRL_CrBy, ADRL_UpdatedBy,
        //            //    ADRL_IPAddress, ADRL_CompID, ADRL_ReportType
        //            //) VALUES (
        //            //    @YearId, @AuditNo, @CustomerId,
        //            //    @RequestedListId, @RequestedTypeId,
        //            //    @RequestedOn, @TimelineToRespond,
        //            //    @EmailIds, @Comments,
        //            //    @CreatedBy, @UpdatedBy,
        //            //    @IPAddress, @CompanyId, @ReportType
        //            //);
        //            //SELECT CAST(SCOPE_IDENTITY() AS INT);";

        //            //    drlId = await connection.ExecuteScalarAsync<int>(insert, new
        //            //    {
        //            //        dto.YearId,
        //            //        dto.AuditNo,
        //            //        dto.CustomerId,
        //            //        dto.RequestedListId,
        //            //        dto.RequestedTypeId,
        //            //        dto.RequestedOn,
        //            //        dto.TimelineToRespond,
        //            //        dto.EmailIds,
        //            //        dto.Comments,
        //            //        CreatedBy = dto.CreatedBy,
        //            //        UpdatedBy = dto.UpdatedBy,
        //            //        IPAddress = dto.IPAddress,
        //            //        CompanyId = dto.CompanyId,
        //            //        ReportType = dto.ReportType
        //            //    }, transaction);
        //            //}
        //            //else
        //            //{
        //            //    var update = @"
        //            //UPDATE Audit_DRLLog SET
        //            //    ADRL_RequestedOn = @RequestedOn,
        //            //    ADRL_TimlinetoResOn = @TimelineToRespond,
        //            //    ADRL_RequestedListID = @RequestedListId,

        //            //    ADRL_EmailID = @EmailIds,
        //            //    ADRL_Comments = @Comments,
        //            //    ADRL_UpdatedBy = @UpdatedBy,
        //            //    ADRL_ReportType = @ReportType
        //            //WHERE ADRL_ID = @Id";

        //            //    await connection.ExecuteAsync(update, new
        //            //    {
        //            //        dto.RequestedOn,
        //            //        dto.TimelineToRespond,
        //            //        dto.EmailIds,
        //            //        dto.Comments,
        //            //        UpdatedBy = dto.UpdatedBy,
        //            //        ReportType = dto.ReportType,
        //            //        dto.Id
        //            //    }, transaction);
        //            //}

        //            // 2. Fetch customer & template data
        //            var customerData = await GetCustomerDetailsWithTemplatesAsync(dto.CompanyId, dto.CustomerId, dto.ReportType);

        //            if (customerData == null)
        //                throw new Exception("Customer data not found.");

        //            // 3. Generate Word/PDF
        //            var outputFolder = Path.Combine("DRLReports", dto.CompanyId.ToString(), dto.CustomerId.ToString());
        //            Directory.CreateDirectory(outputFolder);

        //            var fileName = Path.GetFileName(filePath);
        //            var generatedFilePath = filePath; // Use the temp path passed in

        //            if (fileType.ToLower() == "pdf")
        //                GeneratePdf(customerData, generatedFilePath);
        //            else
        //                GenerateWord(customerData, generatedFilePath);
        //            // 4. Save metadata in EDT_ATTACHMENTS
        //            //var attachId = await connection.ExecuteScalarAsync<int>(
        //            //    @"SELECT ISNULL(MAX(ATCH_ID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_CompID = @CompanyId",
        //            //    new { CompanyId = dto.CompanyId }, transaction);

        //            var docId = await connection.ExecuteScalarAsync<int>(
        //                @"SELECT ISNULL(MAX(ATCH_DOCID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_CompID = @CompanyId",
        //                new { CompanyId = dto.CompanyId }, transaction);


        //            var preAttachId = await connection.ExecuteScalarAsync<int>(
        //              @"SELECT ATCH_ID FROM EDT_ATTACHMENTS WHERE ATCH_drlid = @RequestedListId order by ATCH_ID desc",
        //              new { RequestedListId = dto.RequestedListId }, transaction);
        //            var AttachId = 0;

        //            if (preAttachId == 0)
        //            {
        //                AttachId = await connection.ExecuteScalarAsync<int>(
        //                 @"SELECT ISNULL(MAX(ATCH_ID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_CompID = @CompanyId",
        //                 new { CompanyId = dto.CompanyId }, transaction);
        //            }
        //            else
        //            {
        //                AttachId = preAttachId;
        //            }




        //            var extension = Path.GetExtension(fileName)?.TrimStart('.') ?? "unk";
        //            var fileSize = new FileInfo(generatedFilePath).Length;

        //            var insertAttach = @"
        //        INSERT INTO EDT_ATTACHMENTS (
        //            ATCH_ID, ATCH_DOCID, ATCH_FNAME, ATCH_EXT,
        //            ATCH_CREATEDBY, ATCH_MODIFIEDBY, ATCH_VERSION,
        //            ATCH_FLAG, ATCH_SIZE, ATCH_FROM, ATCH_Basename,
        //            ATCH_CREATEDON, ATCH_Status, ATCH_CompID,
        //            Atch_Vstatus, ATCH_ReportType, ATCH_AuditID, 
        //           ATCH_drlid

        //        )
        //        VALUES (
        //            @AttachId, @DocId, @FileName, @Extension,
        //            @CreatedBy, @CreatedBy, 1,
        //            @Flag, @Size, 0, 0,
        //            GETDATE(), 'X', @CompanyId,
        //            'C', @ReportType, @AuditNo, @RequestedListId
        //        );";

        //            await connection.ExecuteAsync(insertAttach, new
        //            {
        //                AttachId = AttachId,
        //                DocId = docId,
        //                FileName = fileName.Length > 95 ? fileName.Substring(0, 95) : fileName.Replace("&", " and"),
        //                Extension = extension,
        //                CreatedBy = dto.CreatedBy,
        //                Flag = 1,
        //                Size = fileSize,
        //                CompanyId = dto.CompanyId,
        //                ReportType = dto.ReportType,
        //              //  Status = dto.Status,
        //                AuditNo = dto.AuditNo,
        //                RequestedListId = dto.RequestedListId

        //            }, transaction);

        //            // 5. Generate next Remark ID
        //            var remarkId = await connection.ExecuteScalarAsync<int>(
        //                @"SELECT ISNULL(MAX(SAR_ID), 0) + 1 
        //  FROM StandardAudit_Audit_DRLLog_RemarksHistory",
        //                new { dto.CustomerId, dto.AuditNo }, transaction);

        //            // 6. Insert into Remarks History
        //            await connection.ExecuteAsync(
        //                @"INSERT INTO StandardAudit_Audit_DRLLog_RemarksHistory (
        //    SAR_ID, SAR_SAC_ID, SAR_SA_ID, SAR_DRLId,
        //    SAR_Remarks, SAR_Date, SAR_RemarksType, sar_Yearid,
        //    SAR_AttchId, SAR_AtthachDocId, SAR_TimlinetoResOn, SAR_ReportType, SAR_CompID, SAR_MASid
        //) VALUES (
        //    @RemarkId, @CustomerId, @AuditNo, @RequestedListId,
        //    @Remark, @RequestedOn, @Type, @YearId,
        //    @AttachId, @DocId, @TimelineToRespond, @ReportType, @CompanyId, @SarMasId
        //)",
        //                new
        //                {
        //                    RemarkId = remarkId,
        //                    CustomerId = dto.CustomerId,
        //                    AuditNo = dto.AuditNo,
        //                    DRLId = drlId,
        //                    Remark = dto.Comments, // or dto.Remark if that's available
        //                    RequestedOn = dto.RequestedOn,
        //                    Type = dto.RequestedTypeId, // or another appropriate value
        //                    AttachId = AttachId,
        //                    DocId = docId,
        //                    TimelineToRespond = dto.TimelineToRespond,
        //                    ReportType = dto.ReportType,
        //                    RequestedListId = dto.RequestedListId,
        //                    YearId = dto.YearId,
        //                    CompanyId = dto.CompanyId,
        //                    SarMasId = drlId

        //                }, transaction);


        //            transaction.Commit();
        //            await SendBeginningOfAuditEmailAsync(dto);

        //            return drlId;
        //        }
        //        catch
        //        {
        //            transaction.Rollback();
        //            throw;
        //        }
        //    }

        //    private async Task<string> GetReportTypeNameAsync(SqlConnection connection, SqlTransaction transaction, int reportTypeId)
        //    {
        //        const string query = @"
        //SELECT TOP 1 RT_ReportTypeName 
        //FROM Report_Type_Master 
        //WHERE RT_ReportTypeID = @ReportTypeId";

        //        var result = await connection.ExecuteScalarAsync<string>(query, new { ReportTypeId = reportTypeId }, transaction);
        //        return result ?? "N/A";
        //    }


        private async Task SendBeginningOfAuditEmailAsync(DRLLogDto dto)
        {
            if (dto.EmailIds == null || !dto.EmailIds.Any())
                return;

            // ✅ Get real Audit No & Name from DB
            var (auditNo, auditName) = await GetAuditInfoByIdAsync(dto.AuditNo);
            //  var reportTypeName = await GetReportTypeNameAsync(connection, transaction, dto.ReportType);

            var smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("harsha.s2700@gmail.com", "edvemvlmgfkcasrp"),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress("harsha.s2700@gmail.com"),
                Subject = $"Intimation mail for Beginning of the Audit - {auditNo}",
                IsBodyHtml = true
            };

            // First email as To, others as CC
            mail.To.Add(dto.EmailIds[0]);
            for (int i = 1; i < dto.EmailIds.Count; i++)
            {
                mail.CC.Add(dto.EmailIds[i]);
            }

            string body = $@"
<p><strong>Intimation mail</strong></p>
<p>Document Requested</p>
<p>Greetings from TRACe PA.</p>
<p>This mail is an intimation for sharing the documents requested by the Auditor's office.</p>
<p><strong>Beginning of the Audit</strong></p>

<p><strong>Audit No.:</strong> {auditNo} - {auditName}</p>
<p><strong>Report Type:</strong> </p>
<p><strong>Document Requested List:</strong> Beginning of the Audit</p>
<p><strong>Comments:</strong> {dto.Comments}</p>

<br />
<p>Please login to TRACe PA website using the link and credentials shared with you.</p>
<p><a href='https://tracepacust-user.multimedia.interactivedns.com/'>TRACe PA Portal</a></p>
<br />
<p>Thanks,</p>
<p>TRACe PA Team</p>";

            mail.Body = body;

            await smtpClient.SendMailAsync(mail);
        }




        public async Task<int> GetDuringSelfAttachIdAsync(int companyId, int yearId, int customerId, int auditId, int drlId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT ISNULL(a.SAR_AttchId, 0)
        FROM EDT_Attachments ea
        LEFT JOIN StandardAudit_Audit_DRLLog_RemarksHistory a
            ON a.SAR_AtthachDocId = ea.ATCH_DOCID
        WHERE 
            a.SAR_SA_ID = @AuditId AND 
            a.SAR_SAC_ID = @CustomerId AND 
            a.SAR_CompID = @CompanyId AND 
            a.SAR_DRLId = @DrlId AND 
            ea.Atch_Vstatus = 'DS'";

            var result = await connection.ExecuteScalarAsync<int>(query, new
            {
                AuditId = auditId,
                CustomerId = customerId,
                CompanyId = companyId,
                DrlId = drlId
            });

            return result;
        }




        public async Task<IEnumerable<DRLAttachmentInfoDto>> GetDRLAttachmentInfoAsync(int compId, int customerId, int drlId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            //var connectionString = _configuration.GetConnectionString("DefaultConnection");

            //using var connection = new SqlConnection(connectionString);
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



        public async Task<List<AttachmentDto>> LoadAttachmentsAsync(string connectionStringName, int companyId, int attachId, int ReportType)
        {
            var connectionString = _configuration.GetConnectionString(connectionStringName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
SELECT 
    ATCH_ID,
    Atch_DocID,
    ATCH_DRLID,
    ATCH_FNAME,
 CASE 
        WHEN ATCH_ReportType > 0 THEN  RTM_ReportTypeName 
    END AS ReportName,
    ATCH_EXT,
    ATCH_Desc,
    ATCH_CreatedBy,
    FORMAT(ATCH_CREATEDON, 'dd-MMM-yyyy') AS ATCH_CREATEDON,
    ATCH_SIZE,
    ATCH_ReportType,
    CASE 
        WHEN Atch_Vstatus = 'AS' THEN 'Not Shared'
        WHEN Atch_Vstatus = 'A' THEN 'Shared'
        WHEN Atch_Vstatus = 'C' THEN 'Received'
        WHEN Atch_Vstatus = 'DS' THEN 'Received'
    END AS Atch_Vstatus
FROM edt_attachments
LEFT JOIN SAD_ReportTypeMaster ON RTM_Id = ATCH_ReportType
WHERE ATCH_CompID = @CompanyId 
  AND ATCH_ID = @AttachId 
  AND ATCH_Status <> 'D' 
";

            if (ReportType > 0)
            {
                query += " AND ATCH_ReportType = @ReportType";
            }

            query += @"
  AND Atch_Vstatus IN ('A', 'AS', 'C', 'DS')
ORDER BY Atch_DocID desc";

            var rawData = (await connection.QueryAsync(query, new
            {
                CompanyId = companyId,
                AttachId = attachId,
                ReportType = ReportType,

            })).ToList();

            var result = new List<AttachmentDto>();
            int index = 1;

            foreach (var row in rawData)
            {
                result.Add(new AttachmentDto
                {

                    SrNo = index++,
                    PkId = Convert.ToInt32(row.ATCH_ID),
                    AtchID = Convert.ToInt32(row.Atch_DocID),
                    DrlId = Convert.ToInt32(row.ATCH_DRLID),
                    FName = $"{row.ATCH_FNAME}.{row.ATCH_EXT}",
                    ReportName = row.ReportName?.ToString() ?? "",
                    FDescription = row.ATCH_Desc?.ToString() ?? "",
                    CreatedById = Convert.ToInt32(row.ATCH_CreatedBy),
                    CreatedBy = await GetUserFullNameAsync( companyId, Convert.ToInt32(row.ATCH_CreatedBy)),
                    CreatedOn = row.ATCH_CREATEDON?.ToString() ?? "",
                    FileSize = $"{(Convert.ToDouble(row.ATCH_SIZE) / 1024):0.00} KB",
                    Extention = row.ATCH_EXT?.ToString(),
                    Type = row.ATCH_ReportType?.ToString(),
                    Status = row.Atch_Vstatus?.ToString()
                });
            }

            return result;
        }

        private async Task<int> GetOrCreateAttachmentIdAsync(int drlId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var existingId = await connection.ExecuteScalarAsync<int?>(
                @"SELECT TOP 1 ATCH_ID FROM Edt_Attachments WHERE ATCH_DRLID = @DrlId",
                new { DrlId = drlId });

            if (existingId.HasValue)
                return existingId.Value;

            var newId = await connection.ExecuteScalarAsync<int>(
                @"SELECT ISNULL(MAX(ATCH_ID), 0) + 1 FROM Edt_Attachments");

            return newId;
        }


        private async Task<int?> GetExistingAttachmentIdByDrlIdAsync(int drlId, int auditId)
        {
            const string sql = "SELECT TOP 1 ATCH_ID FROM Edt_Attachments WHERE ATCH_drlid = @DrlId AND ATCH_AuditID = @AuditId ORDER BY ATCH_ID";

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            return await connection.ExecuteScalarAsync<int?>(sql, new { DrlId = drlId, AuditId = auditId });
        }

        private async Task<string> SaveAuditDocumentAsync(AddFileDto dto, int attachId, IFormFile file, int requestedId, int reportid)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file.");

            var safeFileName = Path.GetFileName(file.FileName);
            var fileExt = Path.GetExtension(safeFileName)?.TrimStart('.');
            var fileBaseName = Path.GetFileNameWithoutExtension(safeFileName)
                                    .Replace("&", " and");
            fileBaseName = fileBaseName.Substring(0, Math.Min(fileBaseName.Length, 95));

            // Step 1: Resolve base upload path
            var basePath = EnsureDirectoryExists(GetConfigValue("ImgPath"), dto.UserId.ToString(), "Upload");

            // Step 2: Save the uploaded file to disk
            var fullFilePath = Path.Combine(basePath, $"{fileBaseName}.{fileExt}");
            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Step 3: Generate document ID
            var documentId = await GenerateNextDocIdAsync(dto.CustomerId, dto.AuditId);

            // Step 4: Determine attachment ID
            int newAttachId = attachId;
            if (newAttachId == 0)
            {
                var existingAttachId = await GetExistingAttachmentIdByDrlIdAsync(requestedId, dto.AuditId);
                newAttachId = existingAttachId ?? await GenerateNextAttachmentIdAsync(dto.AuditId);
            }

            // Step 5: Insert metadata into Edt_Attachments
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                const string insertQuery = @"
INSERT INTO Edt_Attachments (
    ATCH_ID, ATCH_DOCID, ATCH_FNAME, ATCH_EXT, ATCH_Desc, ATCH_SIZE,
    ATCH_AuditID, ATCH_AUDScheduleID, ATCH_CREATEDBY, ATCH_CREATEDON,
    ATCH_COMPID, ATCH_ReportType, ATCH_drlid, Atch_Vstatus, ATCH_Status
)
VALUES (
    @AtchId, @DocId, @FileName, @FileExt, @Description, @Size,
    @AuditId, @ScheduleId, @UserId, GETDATE(),
    @CompId, @ReportType, @DrlId, @Status, 'X'
);";

                await connection.ExecuteAsync(insertQuery, new
                {
                    AtchId = newAttachId,
                    DocId = documentId,
                    FileName = fileBaseName,
                    FileExt = fileExt,
                    Description = dto.Remark,
                    Size = file.Length,
                    AuditId = dto.AuditId,
                    ScheduleId = dto.AuditScheduleId,
                    UserId = dto.UserId,
                    CompId = dto.CompId,
                    ReportType = reportid,
                    DrlId = requestedId,
                    Status = dto.Status
                });

                // Encrypt the file and move it to the final directory
                string finalDirectory = GetOrCreateTargetDirectory(GetConfigValue("ImgPath"), "SamplingCU", documentId / 301, fullFilePath);
                string finalFilePath = Path.Combine(finalDirectory, $"{documentId}.{fileExt}");

                if (File.Exists(finalFilePath))
                    File.Delete(finalFilePath);

                EncryptFile(fullFilePath, finalFilePath);

                if (File.Exists(fullFilePath))
                    File.Delete(fullFilePath);

                return finalFilePath;
            }
        }


        //        private async Task<string> SaveAuditDocumentAsync(AddFileDto dto, int attachId, IFormFile file, int requestedId, int reportid)
        //        {
        //            if (file == null || file.Length == 0)
        //                throw new ArgumentException("Invalid file.");

        //            var safeFileName = Path.GetFileName(file.FileName);
        //            var fileExt = Path.GetExtension(safeFileName)?.TrimStart('.');
        //             var relativeFolder = Path.Combine("Uploads", "Documents");
        //             var absoluteFolder = Path.Combine(Directory.GetCurrentDirectory(), relativeFolder);
        //            string basePath;
        //            string userLoginName;


        //            if (!Directory.Exists(absoluteFolder))
        //                Directory.CreateDirectory(absoluteFolder);

        //            var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}";
        //            var fullFilePath = Path.Combine(absoluteFolder, uniqueFileName);

        //            using (var stream = new FileStream(fullFilePath, FileMode.Create))
        //            {
        //                await file.CopyToAsync(stream);
        //            }

        //            var docId = await GenerateNextDocIdAsync(dto.CustomerId, dto.AuditId);

        //            int newAttachId = attachId;
        //           // int newAttachId = await GetOrCreateAttachmentIdAsync(requestedId);

        //            // If attachId is zero or not passed, try to get existing ATCH_ID by DRL ID
        //            if (newAttachId == 0)
        //            {
        //                var existingAttachId = await GetExistingAttachmentIdByDrlIdAsync(requestedId, dto.AuditId);
        //                if (existingAttachId.HasValue)
        //                {
        //                    newAttachId = existingAttachId.Value;
        //                }
        //                else
        //                {
        //                    newAttachId = await GenerateNextAttachmentIdAsync(dto.AuditId);
        //                }
        //            }

        //            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //            {
        //                await connection.OpenAsync();

        //                const string insertQuery = @"
        //INSERT INTO Edt_Attachments (
        //    ATCH_ID, ATCH_DOCID, ATCH_FNAME, ATCH_EXT, ATCH_Desc, ATCH_SIZE,
        //    ATCH_AuditID, ATCH_AUDScheduleID, ATCH_CREATEDBY, ATCH_CREATEDON,
        //    ATCH_COMPID, ATCH_ReportType, ATCH_drlid, Atch_Vstatus, ATCH_Status
        //)
        //VALUES (
        //    @AtchId, @DocId, @FileName, @FileExt, @Description, @Size,
        //    @AuditId, @ScheduleId, @UserId, GETDATE(),
        //    @CompId, @ReportType, @DrlId, @Status, 'X'
        //);";

        //                await connection.ExecuteAsync(insertQuery, new
        //                {
        //                    AtchId = newAttachId,
        //                    DocId = docId,
        //                    FileName = safeFileName,
        //                    FileExt = fileExt,
        //                    Description = dto.Remark,
        //                    Size = file.Length,
        //                    AuditId = dto.AuditId,
        //                    ScheduleId = dto.AuditScheduleId,
        //                    UserId = dto.UserId,
        //                    CompId = dto.CompId,
        //                    ReportType = reportid,
        //                    DrlId = requestedId,
        //                    Status = dto.Status
        //                });

        //                return fullFilePath;
        //            }
        //        }






        //        private async Task<string> SaveAuditDocumentAsync(AddFileDto dto, int attachId, IFormFile file, int requestedId, int reportid)
        //        {
        //            if (file == null || file.Length == 0)
        //                throw new ArgumentException("Invalid file.");

        //            var safeFileName = Path.GetFileName(file.FileName);
        //            var fileExt = Path.GetExtension(safeFileName)?.TrimStart('.');
        //            var relativeFolder = Path.Combine("Uploads", "Documents");
        //            var absoluteFolder = Path.Combine(Directory.GetCurrentDirectory(), relativeFolder);

        //            if (!Directory.Exists(absoluteFolder))
        //                Directory.CreateDirectory(absoluteFolder);

        //            var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}";
        //            var fullFilePath = Path.Combine(absoluteFolder, uniqueFileName);

        //            using (var stream = new FileStream(fullFilePath, FileMode.Create))
        //            {
        //                await file.CopyToAsync(stream);
        //            }

        //            var docId = await GenerateNextDocIdAsync(dto.CustomerId, dto.AuditId);

        //            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //            {
        //                await connection.OpenAsync();

        //                var upsertQuery = @"
        //IF EXISTS (SELECT 1 FROM Edt_Attachments WHERE ATCH_ID = @AtchId)
        //BEGIN
        //    UPDATE Edt_Attachments SET
        //        ATCH_DOCID = @DocId,
        //        ATCH_FNAME = @FileName,
        //        ATCH_EXT = @FileExt,
        //        ATCH_Desc = @Description,
        //        ATCH_SIZE = @Size,
        //        ATCH_AuditID = @AuditId,
        //        ATCH_AUDScheduleID = @ScheduleId,
        //        ATCH_CREATEDBY = @UserId,
        //        ATCH_CREATEDON = GETDATE(),
        //        ATCH_COMPID = @CompId,
        //        ATCH_ReportType = @ReportType,
        //        ATCH_drlid = @DrlId,
        //        Atch_Vstatus = @Status,
        //        ATCH_Status = 'X'
        //    WHERE ATCH_ID = @AtchId;
        //END
        //ELSE
        //BEGIN
        //    INSERT INTO Edt_Attachments (
        //        ATCH_ID, ATCH_DOCID, ATCH_FNAME, ATCH_EXT, ATCH_Desc, ATCH_SIZE,
        //        ATCH_AuditID, ATCH_AUDScheduleID, ATCH_CREATEDBY, ATCH_CREATEDON,
        //        ATCH_COMPID, ATCH_ReportType, ATCH_drlid, Atch_Vstatus, ATCH_Status
        //    )
        //    VALUES (
        //        @AtchId, @DocId, @FileName, @FileExt, @Description, @Size,
        //        @AuditId, @ScheduleId, @UserId, GETDATE(),
        //        @CompId, @ReportType, @DrlId, @Status, 'X'
        //    );
        //END";

        //                await connection.ExecuteAsync(upsertQuery, new
        //                {
        //                    AtchId = attachId,
        //                    DocId = docId,
        //                    FileName = safeFileName,
        //                    FileExt = fileExt,
        //                    Description = dto.Remark,
        //                    Size = file.Length,
        //                    AuditId = dto.AuditId,
        //                    ScheduleId = dto.AuditScheduleId,
        //                    UserId = dto.UserId,
        //                    CompId = dto.CompId,
        //                    ReportType = reportid,
        //                    DrlId = requestedId,
        //                    Status = dto.Status
        //                });

        //                return fullFilePath;
        //            }
        //        }


        private async Task<int> GenerateNextDocIdAsync(int customerId, int auditId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                return await connection.ExecuteScalarAsync<int>(
                    @"SELECT ISNULL(MAX( ATCH_DOCID), 0) + 1 FROM Edt_Attachments",
                    new { customerId, auditId });



            }
        }


        private async Task<int> InsertIntoAuditDrlLogAsync(AddFileDto dto, int requestedId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Get next ADRL_ID with lock to prevent race condition
                const string getNextIdSql = @"
            SELECT ISNULL(MAX(ADRL_ID), 0) + 1 
            FROM Audit_DRLLog WITH (TABLOCKX, HOLDLOCK);";

                int nextAdrlId = await connection.ExecuteScalarAsync<int>(getNextIdSql, transaction: transaction);

                // Check if record exists with UPDLOCK to avoid race conditions
                const string checkSql = @"
            SELECT TOP 1 ADRL_ID 
            FROM Audit_DRLLog WITH (UPDLOCK, ROWLOCK)
            WHERE ADRL_CustID = @CustomerId 
              AND ADRL_AuditNo = @AuditId 
              AND ADRL_RequestedListID = @RequestedId";

                var existingId = await connection.ExecuteScalarAsync<int?>(checkSql, new
                {
                    CustomerId = dto.CustomerId,
                    AuditId = dto.AuditId,
                    RequestedId = requestedId
                }, transaction);

                int resultId;

                if (existingId.HasValue && existingId.Value > 0)
                {
                    // Update existing record
                    const string updateSql = @"
                UPDATE Audit_DRLLog
                SET 
                    ADRL_YearID = @YearId,
                    
                    ADRL_CrBy = @UserId,
                    ADRL_IPAddress = @IpAddress,
                    ADRL_CompID = @CompId,
                    ADRL_ReportType = @ReportType,
                    ADRL_AttachID = @AttachId, 
                    
                    ADRL_UpdatedOn = GETDATE()
                WHERE ADRL_ID = @AdrlId;";
                    //var emailIdsCsv = dto.EmailId != null ? string.Join(",", dto.EmailId) : null;

                    await connection.ExecuteAsync(updateSql, new
                    {
                        AdrlId = existingId.Value,
                        YearId = dto.YearId,
                        // EmailIds = emailIdsCsv,
                        Remarks = dto.Remark,
                        UserId = dto.UserId,
                        IpAddress = dto.IpAddress,
                        CompId = dto.CompId,
                        // AttchDocId = dto.DocId,
                        ReportType = dto.ReportType,
                        AttachId = dto.AtchId
                    }, transaction);

                    resultId = existingId.Value;
                }
                else
                {
                    // Insert new record with manually generated ADRL_ID
                    const string insertSql = @"
                INSERT INTO Audit_DRLLog (
                    ADRL_ID,
                    ADRL_YearID, ADRL_AuditNo, ADRL_CustID, 
                    ADRL_RequestedListID, ADRL_Comments,
                    ADRL_CrBy, ADRL_IPAddress, ADRL_CompID, ADRL_Status,
                    ADRL_ReportType, ADRL_RequestedOn, ADRL_UpdatedOn, ADRL_AttachID
                ) 
                VALUES (
                    @AdrlId,
                    @YearId, @AuditId, @CustomerId,
                    @RequestedId, @Remarks,
                    @UserId, @IpAddress, @CompId, @Status,
                    @ReportType, GETDATE(), GETDATE(), @AttachId
                );";
                    // var emailIdsCsv = dto.EmailId != null ? string.Join(",", dto.EmailId) : null;
                    await connection.ExecuteAsync(insertSql, new
                    {
                        AdrlId = nextAdrlId,
                        YearId = dto.YearId,
                        AuditId = dto.AuditId,
                        CustomerId = dto.CustomerId,
                        RequestedId = requestedId,
                        //  EmailIds = emailIdsCsv,
                        Remarks = dto.Remark,
                        UserId = dto.UserId,
                        IpAddress = dto.IpAddress,
                        CompId = dto.CompId,
                        Status = dto.Status,
                        ReportType = dto.ReportType,
                        AttachId = dto.AtchId
                    }, transaction);

                    resultId = nextAdrlId;
                }

                await transaction.CommitAsync();
                dto.AdrlId = resultId;
                return resultId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        private async Task<string> GetAuditNoByIdAsync(int auditId)
        {
            const string query = @"
        SELECT TOP 1 SA_AuditNo
        FROM StandardAudit_Audit
        WHERE SA_ID = @AuditId";

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            return await connection.ExecuteScalarAsync<string>(query, new { AuditId = auditId }) ?? "N/A";
        }


        public async Task<IEnumerable<DRLDetailsDto>> LoadPostAndPreAuditAsync(
       int customerId, int auditId, int reportType)
        {
            var query = @"
WITH RecentRecords AS (
    SELECT 
        c.RTM_ReportTypeName,
        CONCAT(ATCH_FNAME, '-', ATCH_EXT) AS Fname, 
        SAR_ID,
        sar_Yearid,
        SAR_SA_ID,
        SAR_SAC_ID,
        SAR_Date,
        COALESCE(SAR_TimlinetoResOn, 'N/A') AS SAR_TimlinetoResOn,
        COALESCE(SAR_Remarks, 'N/A') AS SAR_Remarks,
        SAR_AttchId,
        SAR_ReportType,
        d.usr_FullName,
        COALESCE(
            MAX(CASE WHEN SAR_RemarksType = 'RC' THEN d.usr_FullName END) 
            OVER (PARTITION BY c.RTM_ReportTypeName), 'N/A') AS CustomerRemarksBY,
        COALESCE(
            CAST(MAX(CASE WHEN SAR_RemarksType = 'RC' THEN SAR_Date END) 
            OVER (PARTITION BY c.RTM_ReportTypeName) AS NVARCHAR), 'N/A') AS RecentRC_RemarkDate,
        COALESCE(
            MAX(CASE WHEN SAR_RemarksType = 'RC' THEN SAR_Remarks END) 
            OVER (PARTITION BY c.RTM_ReportTypeName), 'N/A') AS SARCustRemarks,
        ROW_NUMBER() OVER (PARTITION BY c.RTM_ReportTypeName ORDER BY SAR_Date DESC) AS RowNum
    FROM 
        StandardAudit_Audit_DRLLog_RemarksHistory
    LEFT JOIN Audit_DRLLog ON ADRL_AttchDocId = SAR_MASid
    LEFT JOIN Edt_Attachments ON ATCH_DOCID = SAR_AtthachDocId 
    LEFT JOIN Content_Management_Master ON CMM_ID = ADRL_RequestedListID
    LEFT JOIN SAD_ReportTypeMaster c ON c.RTM_Id = SAR_ReportType
    LEFT JOIN Sad_UserDetails d ON d.usr_Id = SAR_RemarksBy
    WHERE SAR_SA_ID = @AuditId 
      AND SAR_SAC_ID = @CustomerId"
            + (reportType != 0 ? " AND SAR_ReportType = @ReportType" : "") +
        @"
)
SELECT
    RTM_ReportTypeName AS ReportTypeText,
    Fname,
    SAR_ID AS DRLID,
    sar_Yearid,
    SAR_SA_ID,
    SAR_SAC_ID,
    FORMAT(SAR_Date, 'yyyy-MM-dd') AS RequestedOn,
    SAR_TimlinetoResOn AS TimlinetoResOn,
    SAR_Remarks AS Comments,
    SAR_AttchId AS AttachID,
    SAR_ReportType AS ReportType,
    usr_FullName AS RemarksBy,
    CustomerRemarksBY AS CustomerBy,
    RecentRC_RemarkDate AS RespondDate,
    SARCustRemarks AS CustomerRemarks
FROM RecentRecords
WHERE RowNum = 1 AND COALESCE(SAR_Remarks, '') <> ''
ORDER BY SAR_Date DESC;";

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
           // await connection.OpenAsync();

            var results = await connection.QueryAsync<DRLDetailsDto>(query, new
            {
                CustomerId = customerId,
                AuditId = auditId,
                ReportType = reportType
            });

            return results;
        }

        public async Task<IEnumerable<dynamic>> GetReportHistoryComments(ReportHistoryCommentsDto dto)
        {
            var sql = @"
WITH RecentRecords AS (
    SELECT 
        c.RTM_ReportTypeName,
        CONCAT(ATCH_FNAME, '-', ATCH_EXT) AS Fname, 
        SAR_ID,
        sar_Yearid,
        SAR_SA_ID,
        SAR_SAC_ID,
        SAR_Date,
        COALESCE(SAR_TimlinetoResOn, 'N/A') AS SAR_TimlinetoResOn,
        COALESCE(SAR_Remarks, 'N/A') AS SAR_Remarks,
        SAR_AttchId,
        SAR_ReportType,
        d.usr_FullName,
        COALESCE(MAX(CASE WHEN SAR_RemarksType = 'RC' THEN d.usr_FullName END) 
                 OVER (PARTITION BY c.RTM_ReportTypeName), 'N/A') AS CustomerRemarksBY,
        COALESCE(CAST(MAX(CASE WHEN SAR_RemarksType = 'RC' THEN SAR_Date END) 
                 OVER (PARTITION BY c.RTM_ReportTypeName) AS NVARCHAR(50)), 'N/A') AS RecentRC_RemarkDate,
        COALESCE(MAX(CASE WHEN SAR_RemarksType = 'RC' THEN SAR_Remarks END) 
                 OVER (PARTITION BY c.RTM_ReportTypeName), 'N/A') AS SARCustRemarks
    FROM StandardAudit_Audit_DRLLog_RemarksHistory
    LEFT JOIN Audit_DRLLog ON ADRL_AttchDocId = SAR_MASid
    LEFT JOIN Edt_Attachments ON ATCH_DOCID = SAR_AtthachDocId 
    LEFT JOIN Content_Management_Master ON CMM_ID = ADRL_RequestedListID AND CMM_CompID = 1
    LEFT JOIN SAD_ReportTypeMaster c ON c.RTM_Id = SAR_ReportType
    LEFT JOIN Sad_UserDetails d ON d.usr_Id = SAR_RemarksBy
    WHERE 
        SAR_SA_ID = @AuditId AND 
        SAR_SAC_ID = @CustId AND 
        SAR_DRLId = @RequestId" +
            (dto.ReportType != 0 ? " AND SAR_ReportType = @ReportType" : "") + @"
)
SELECT
    RTM_ReportTypeName AS ReportTypeText,
    Fname,
    SAR_ID AS DRLID,
    SAR_Date AS RequestedOn,
    SAR_TimlinetoResOn AS TimlinetoResOn,
    SAR_Remarks AS Comments,
    SAR_AttchId AS AttachID,
    SAR_ReportType AS ReportType,
    usr_FullName AS RemarksBy,
    CustomerRemarksBY AS CustomerBy,
    SARCustRemarks AS CustomerRemarks,
    RecentRC_RemarkDate AS RespondDate
FROM 
    RecentRecords
WHERE 
    COALESCE(SAR_Remarks, '') <> ''
ORDER BY 
    SAR_Date DESC;
";
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryAsync(sql, new
            {
                dto.CustId,
                dto.AuditId,
                dto.RequestId,
                dto.ReportType
            });

            return result;
        }




        public async Task<string> UploadAndSaveAttachmentAsync(AddFileDto dto)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())

                {
                    try
                    {
                        string filePath = null;
                        int attachId = dto.AtchId;

                        // STEP 1: Handle file upload if provided
                        if (dto.File != null && dto.File.Length > 0)
                        {
                            // Check if attachment already exists for this DRL
                            if (attachId <= 0)
                            {
                                // Try to reuse existing ATCH_ID for this DRLID
                                var existingAttachId = await GetExistingAttachmentIdByDrlIdAsync(dto.DrlId, dto.AuditId);
                                if (existingAttachId.HasValue)
                                {
                                    attachId = existingAttachId.Value;
                                }
                                else
                                {
                                    // Generate new attachId only if none exists
                                    attachId = await GenerateNextAttachmentIdAsync(dto.AuditId);
                                }
                                dto.AtchId = attachId;
                            }



                            // Save file and insert into Edt_Attachments
                            filePath = await SaveAuditDocumentAsync(dto, attachId, dto.File, dto.DrlId, dto.ReportType);

                            // Update references in DRL tables
                            int pkId = await GetDrlPkIdAsync(dto.CustomerId, dto.AuditId, attachId);
                            int docId = await GetLatestDocIdAsync(dto.CustomerId, dto.AuditId);
                            await UpdateDrlAttachIdAndDocIdAsync(dto.CustomerId, dto.AuditId, attachId, docId, pkId);

                            // You can trigger email sending here if needed
                            // await SendDuringAuditEmailAsync(dto);
                        }

                        // STEP 2: Always log to DRL and Remarks tables
                        int requestedId = dto.DrlId;
                        int adrlId = await InsertIntoAuditDrlLogAsync(dto, requestedId);
                        dto.AdrlId = adrlId;

                        int docIdForRemarks = await GetLatestDocIdAsync(dto.CustomerId, dto.AuditId);
                        int remarkId = await GenerateNextRemarkIdAsync(dto.CustomerId, dto.AuditId);
                        await InsertIntoAuditDocRemarksLogAsync(dto, requestedId, remarkId, attachId, docIdForRemarks, adrlId);

                        await transaction.CommitAsync();

                        // ✅ Return attachId along with the message
                        return $"File uploaded Successfully. AttachId: {attachId}";
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return $"Error: {ex.Message}";
                    }
                }
            }
        }



        //public async Task<string> UploadAndSaveAttachmentAsync(AddFileDto dto)
        //{
        //    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        await connection.OpenAsync();
        //        using (var transaction = await connection.BeginTransactionAsync())
        //        {
        //            try
        //            {
        //                string filePath = null;
        //                int attachId = dto.AtchId;

        //                // STEP 1: Handle file upload if provided
        //                if (dto.File != null && dto.File.Length > 0)
        //                {
        //                    // Check if attachment already exists for this DRL
        //                    if (attachId <= 0)
        //                    {
        //                        // Try to reuse existing ATCH_ID for this DRLID
        //                        var existingAttachId = await GetExistingAttachmentIdByDrlIdAsync(dto.DrlId, dto.AuditId);
        //                        if (existingAttachId.HasValue)
        //                        {
        //                            attachId = existingAttachId.Value;
        //                        }
        //                        else
        //                        {
        //                            // Generate new attachId only if none exists
        //                            attachId = await GenerateNextAttachmentIdAsync(dto.AuditId);
        //                        }
        //                        dto.AtchId = attachId;
        //                    }
        //                    //int newAttachId = await GetOrCreateAttachmentIdAsync(dto.DrlId);
        //                    // Save file and insert into Edt_Attachments
        //                    filePath = await SaveAuditDocumentAsync(dto, attachId, dto.File, dto.DrlId, dto.ReportType);

        //                    // Update references in DRL tables
        //                    int pkId = await GetDrlPkIdAsync(dto.CustomerId, dto.AuditId, attachId);
        //                    int docId = await GetLatestDocIdAsync(dto.CustomerId, dto.AuditId);
        //                    await UpdateDrlAttachIdAndDocIdAsync(dto.CustomerId, dto.AuditId, attachId, docId, pkId);

        //                    // Send email (non-transactional)
        //                    // await SendEmailWithAttachmentAsync(dto.EmailId, filePath);
        //                    //await SendDuringAuditEmailAsync(dto);
        //                }

        //                // STEP 2: Always log to DRL and Remarks tables
        //                int requestedId = dto.DrlId;
        //                int adrlId = await InsertIntoAuditDrlLogAsync(dto, requestedId);
        //                dto.AdrlId = adrlId;

        //                int docIdForRemarks = await GetLatestDocIdAsync(dto.CustomerId, dto.AuditId);
        //                int remarkId = await GenerateNextRemarkIdAsync(dto.CustomerId, dto.AuditId);
        //                await InsertIntoAuditDocRemarksLogAsync(dto, requestedId, remarkId, attachId, docIdForRemarks, adrlId);
        //                //await SendDuringAuditEmailAsync(dto);
        //                await transaction.CommitAsync();

        //                return "Attachment uploaded, details saved successfully.";
        //            }
        //            catch (Exception ex)
        //            {
        //                await transaction.RollbackAsync();
        //                return $"Error: {ex.Message}";
        //            }
        //        }
        //    }
        //}

        //        private async Task SendDuringAuditEmailAsync(AddFileDto dto)
        //        {
        //            if (dto.EmailId == null || !dto.EmailId.Any())
        //                return;

        //            // ✅ Fetch Audit Info
        //            var (auditNo, auditName) = await GetAuditInfoByIdAsync(dto.AuditId);

        //            var smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com")
        //            {
        //                Port = 587,
        //                Credentials = new NetworkCredential("harsha.s2700@gmail.com", "edvemvlmgfkcasrp"),
        //                EnableSsl = true
        //            };

        //            var mail = new MailMessage
        //            {
        //                From = new MailAddress("harsha.s2700@gmail.com"),
        //                Subject = $"Intimation mail for sharing the Documents requested by the Auditor - {auditNo}",
        //                IsBodyHtml = true
        //            };

        //            // ✅ Add To and CC
        //            mail.To.Add(dto.EmailId[0]);
        //            for (int i = 1; i < dto.EmailId.Count; i++)
        //            {
        //                mail.CC.Add(dto.EmailId[i]);
        //            }

        //            var requestedOn = dto.RequestedOn?.ToString("MMM/dd/yy") ?? "";

        //            // ✅ Updated Body with AuditNo and AuditName
        //            string body = $@"
        //<p><strong>Intimation mail</strong></p>
        //<p>Document Requested</p>
        //<p>Greetings from TRACe PA.</p>
        //<p>This mail is an intimation for sharing the documents requested by the Auditor's office.</p>

        //<p><strong>Audit No.:</strong> {auditNo} - {auditName} and Date : {requestedOn}</p>
        //<p><strong>Document Requested List:</strong> Journal Entries</p>";

        //            if (!string.IsNullOrWhiteSpace(dto.Remark))
        //            {
        //                body += $@"
        //<p><strong>Specific request for client:</strong></p>
        //<p>{dto.Remark}</p>";
        //            }

        //            body += @"
        //<br />
        //<p>Please login to TRACe PA website using the link and credentials shared with you.</p>
        //<p><a href='https://tracepacust-user.multimedia.interactivedns.com/'>TRACe PA Portal</a></p>
        //<p>Home page of the application will show you the list of documents requested by the auditor. Upload all the requested documents using links provided.</p>
        //<br />
        //<p>Thanks,</p>
        //<p>TRACe PA Team</p>";

        //            mail.Body = body;

        //            await smtpClient.SendMailAsync(mail);
        //        }


        //private async Task UpdateLatestEdtAttachmentStatusAsync(int customerId, int auditId)
        //{
        //    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        await connection.OpenAsync();

        //        const string sql = @"
        //    UPDATE TOP (1) Edt_Attachments
        //    SET Atch_Vstatus = 'C'
        //    WHERE CUSTOMERID = @CustomerId AND AUDITID = @AuditId
        //    ORDER BY ATTACHID DESC;
        //";

        //        await connection.ExecuteAsync(sql, new { CustomerId = customerId, AuditId = auditId });
        //    }
        //}







        //private async Task<int> GenerateNextAttachmentIdAsync(int customerId, int auditId, int YearId)
        //{
        //    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        await connection.OpenAsync();
        //        return await connection.ExecuteScalarAsync<int>(
        //            @"SELECT ISNULL(MAX(ATCH_ID), 0) + 1 
        //      FROM Edt_Attachments
        //      LEFT JOIN StandardAudit_Audit_DRLLog_RemarksHistory a 
        //      ON a.SAR_AttchId = Atch_Id
        //      WHERE SAR_SAC_ID = @CustomerId 
        //      AND SAR_SA_ID = @AuditId 
        //      AND SAR_Yearid = @YearId",
        //            new { customerId, auditId, YearId });
        //    }
        //}

        private async Task<int> GenerateNextAttachmentIdAsync(int auditId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            //using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                // Use transaction with locking to prevent race conditions
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        var nextId = await connection.ExecuteScalarAsync<int>(
                            @"SELECT ISNULL(MAX(ATCH_ID), 0) + 1 
                      FROM Edt_Attachments",
                            new { AuditId = auditId },
                            transaction: transaction);

                        await transaction.CommitAsync();
                        return nextId;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        private async Task<int> GetRequestedIdByDocumentNameAsync(string documentName)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            //using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
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
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            // using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
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
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            //using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                return await connection.ExecuteScalarAsync<int>(
                    @"SELECT ISNULL(MAX(SAR_ID), 0) + 1 
              FROM StandardAudit_Audit_DRLLog_RemarksHistory 
              WHERE SAR_SAC_ID = @CustomerId AND SAR_SA_ID = @AuditId",
                    new { customerId, auditId });
            }
        }


        private async Task InsertIntoAuditDocRemarksLogAsync(AddFileDto dto, int requestedId, int remarkId, int attachId, int docId, int adrlId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            //  using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(
                    @"INSERT INTO StandardAudit_Audit_DRLLog_RemarksHistory

(
                SAR_ID, SAR_SAC_ID, SAR_SA_ID, SAR_DRLId, sar_Yearid,

                SAR_Remarks, SAR_Date, SAR_RemarksType, SAR_AttchId, SAR_AtthachDocId, SAR_TimlinetoResOn, SAR_ReportType, SAR_MASid
            ) VALUES (
                @RemarkId, @CustomerId, @AuditId, @RequestedId,  @YearId,
                @Remark, @RequestedOn, @Type, @AtchId, @DocId, @RespondTime, @ReportType, @AdrlId
            )",
                    new
                    {
                        RemarkId = remarkId,
                        CustomerId = dto.CustomerId,
                        AuditId = dto.AuditId,
                        RequestedId = requestedId,
                        Remark = dto.Remark,
                        Type = dto.Type,
                        AtchId = attachId,
                        DocId = docId,
                        RequestedOn = dto.RequestedOn,
                        RespondTime = dto.RespondTime,
                        YearId = dto.YearId,

                        ReportType = dto.ReportType,
                        AdrlId = adrlId

                    }
                );
            }
        }





        private async Task<int> GetDrlPkIdAsync(int customerId, int auditId, int attachId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            // using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
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
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            //  using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(
                    @"UPDATE StandardAudit_Audit_DRLLog_RemarksHistory 
              SET SAR_AttchId = @AttachId, SAR_AtthachDocId = @DocId
              WHERE SAR_SAC_ID = @CustomerId AND SAR_SA_ID = @AuditId AND SAR_ID = @PkId",
                    new { AttachId = attachId, DocId = docId, CustomerId = customerId, AuditId = auditId, PkId = pkId });
            }
        }

        public async Task<(bool IsSuccess, string Message)> UpdateDrlStatusAsync(UpdateDrlStatusDto dto)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

 if (string.IsNullOrEmpty(dbName))
     throw new Exception("CustomerCode is missing in session. Please log in again.");

 // ✅ Step 2: Get the connection string
 var connectionString = _configuration.GetConnectionString(dbName);

    using var connection = new SqlConnection(connectionString);
            //using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var emailCsv = dto.EmailId != null ? string.Join(",", dto.EmailId) : null;

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Status", dto.Status);
                parameters.Add("@Remarks", dto.Remarks);
                parameters.Add("@Timeline", dto.Timeline);
                parameters.Add("@CustomerId", dto.CustomerId);
                parameters.Add("@YearId", dto.YearId);
                parameters.Add("@CompId", dto.CompId);
                parameters.Add("@Reporttype", dto.Reporttype);
                parameters.Add("@EmailId", emailCsv); // ✅ Using correct variable and property

                // Step 1: Update Edt_Attachments
                string updateAttachmentSql = @"
UPDATE Edt_Attachments 
SET Atch_Vstatus = @Status
WHERE ATCH_ReportType IN @Reporttype
  AND ATCH_Status <> 'D'";

                await connection.ExecuteAsync(updateAttachmentSql, parameters, transaction);

                // Step 2: Update Remarks History
                string updateRemarksSql = @"
UPDATE StandardAudit_Audit_DRLLog_RemarksHistory
SET SAR_Remarks = @Remarks,
    SAR_TimlinetoResOn = @Timeline,
    SAR_EmailIds = @EmailId
WHERE SAR_ReportType IN @Reporttype
  AND SAR_SAC_ID = @CustomerId
  AND SAR_Yearid = @YearId
  AND SAR_CompID = @CompId";

                await connection.ExecuteAsync(updateRemarksSql, parameters, transaction);

                // Step 3: Update Audit_DRLLog
                string updateDrlLogSql = @"
UPDATE Audit_DRLLog
SET ADRL_RequestedOn = @Timeline,
    ADRL_EmailID = @EmailId,
    ADRL_Comments = @Remarks,
    ADRL_Status = @Status
WHERE ADRL_ReportType IN @Reporttype
  AND ADRL_YearID = @YearId
  AND ADRL_CompID = @CompId
  AND ADRL_CustID = @CustomerId";

                await connection.ExecuteAsync(updateDrlLogSql, parameters, transaction);

                await transaction.CommitAsync();

                var auditInfo = await GetAuditInfoByIdAsync(dto.CustomerId);
                await SendAuditLifecycleEmailAsync(dto.EmailId ?? new List<string>(), auditInfo.AuditNo, auditInfo.AuditName, dto.Remarks);
                return (true, "DRL status updated successfully.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return (false, "Failed to update DRL status.");
            }
        }




        private async Task<(string AuditNo, string AuditName)> GetAuditInfoByIdAsync(int auditId)
        {
            const string query = @"
SELECT TOP 1 SA_AuditNo,  SA_ScopeOfAudit 
FROM StandardAudit_Schedule 
WHERE SA_ID = @AuditId";
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            //using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var result = await connection.QueryFirstOrDefaultAsync(query, new { AuditId = auditId });

            if (result != null)
            {
                return (result.SA_AuditNo ?? "N/A", result.SA_ScopeOfAudit ?? "N/A");
            }

            return ("N/A", "N/A");


        }

        private async Task SendAuditLifecycleEmailAsync(List<string> toEmails, string auditNo, string auditName, string remarks)
        {
            var subject = $"Intimation mail for Nearing completion of the Audit - {auditNo}";

            var body = $@"
<p><strong>Intimation mail</strong></p>

<p><strong>Document Requested</strong></p>

<p>Greetings from TRACe PA.</p>

<p>This mail is an intimation for sharing the documents requested by the Auditor's office.</p>

<p><strong>Audit No.</strong>: {auditNo} - {auditName}</p>

<p><strong>Document Requested List</strong> :</p>

<p><strong>Comments</strong> :</p>

<p>{remarks}</p>

<p>Please login to TRACe PA website using the link and credentials shared with you.</p>

<p><a href='https://tracepacust-user.multimedia.interactivedns.com/'>Click Here</a></p>

<p>Home page of the application will show you the list of documents requested by the auditor. Upload all the requested documents using links provided.</p>

<br/>
<p>Thanks,</p>
<p>TRACe PA Team</p>
";

            using var message = new MailMessage();
            message.From = new MailAddress("harsha.s2700@gmail.com");

            foreach (var email in toEmails)
            {
                if (!string.IsNullOrWhiteSpace(email))
                    message.To.Add(new MailAddress(email.Trim()));
            }

            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using var smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("harsha.s2700@gmail.com", "edvemvlmgfkcasrp"), // Use app password!
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(message);
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

            using var client = new MailKit.Net.Smtp.SmtpClient();
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

        //        private async Task<string> SaveAuditDocumentAsync(AddFileDto dto, int attachId, IFormFile file, int requestedId, int reportId)
        //        {
        //            if (file == null || file.Length == 0)
        //                throw new ArgumentException("Invalid file.");

        //            var safeFileName = Path.GetFileName(file.FileName);
        //            var fileExt = Path.GetExtension(safeFileName)?.TrimStart('.');
        //            var relativeFolder = Path.Combine("Uploads", "Documents");
        //            var absoluteFolder = Path.Combine(Directory.GetCurrentDirectory(), relativeFolder);

        //            if (!Directory.Exists(absoluteFolder))
        //                Directory.CreateDirectory(absoluteFolder);

        //            var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}";
        //            var fullFilePath = Path.Combine(absoluteFolder, uniqueFileName);

        //            using (var stream = new FileStream(fullFilePath, FileMode.Create))
        //            {
        //                await file.CopyToAsync(stream);
        //            }

        //            var docId = await GenerateNextDocIdAsync(dto.CustomerId, dto.AuditId);

        //            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //            {
        //                await connection.OpenAsync();

        //                var insertQuery = @"
        //INSERT INTO Edt_Attachments (
        //    ATCH_ID, ATCH_DOCID, ATCH_FNAME, ATCH_EXT, ATCH_Desc, ATCH_SIZE,
        //    ATCH_AuditID, ATCH_AUDScheduleID, ATCH_CREATEDBY, ATCH_CREATEDON,
        //    ATCH_COMPID, ATCH_ReportType, ATCH_drlid, Atch_Vstatus, ATCH_Status
        //)
        //VALUES (
        //    @AtchId, @DocId, @FileName, @FileExt, @Description, @Size,
        //    @AuditId, @ScheduleId, @UserId, GETDATE(),
        //    @CompId, @ReportType, @DrlId, 'A', 'X'
        //);";

        //                await connection.ExecuteAsync(insertQuery, new
        //                {
        //                    AtchId = attachId,
        //                    DocId = docId,
        //                    FileName = safeFileName,
        //                    FileExt = fileExt,
        //                    Description = dto.Remark,
        //                    Size = file.Length,
        //                    AuditId = dto.AuditId,
        //                    ScheduleId = dto.AuditScheduleId,
        //                    UserId = dto.UserId,
        //                    CompId = dto.CompId,
        //                    ReportId = reportId,
        //                    DrlId = requestedId
        //                });

        //                return fullFilePath;
        //            }
        //        }




        //        public async Task<int> InsertIntoAuditDrlLogAsync(AddFileDto dto, int requestedId, int reportTypeId)
        //        {
        //            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //            {
        //                await connection.OpenAsync();

        //                var sql = @"
        //        INSERT INTO Audit_DRLLog (
        //            ADRL_AuditId,
        //            ADRL_CustomerId,
        //            ADRL_RequestedId,
        //            ADRL_Date,
        //            ADRL_Status,
        //            ADRL_ReportType
        //        )
        //        VALUES (
        //            @AuditId,
        //            @CustomerId,
        //            @RequestedId,
        //            GETDATE(),
        //            'Uploaded',
        //            @ReportTypeId
        //        );
        //        SELECT SCOPE_IDENTITY();";

        //                var parameters = new
        //                {
        //                    AuditId = dto.AuditId,
        //                    CustomerId = dto.CustomerId,
        //                    RequestedId = requestedId,
        //                    ReportTypeId = reportTypeId
        //                };

        //                var insertedId = await connection.ExecuteScalarAsync<int>(sql, parameters);
        //                return insertedId;
        //            }
        //        }


        //        private async Task InsertIntoBeginingAuditDocRemarksLogAsync(AddFileDto dto, int requestedId, int remarkId, int attachId, int docId)
        //        {
        //            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //            {
        //                await connection.OpenAsync();
        //                await connection.ExecuteAsync(
        //                @"INSERT INTO StandardAudit_Audit_DRLLog_RemarksHistory (
        //            SAR_ID,
        //            SAR_SAC_ID,
        //            SAR_SA_ID,
        //            SAR_DRLId,
        //            SAR_Remarks,
        //            SAR_Date,
        //            SAR_RemarksType,
        //            SAR_AttchId,
        //            SAR_AttachDocId,   -- ✅ fixed typo here
        //            SAR_TimlinetoResOn,
        //            SAR_ReportType
        //        )
        //        VALUES (
        //            @RemarkId,
        //            @CustomerId,
        //            @AuditId,
        //            @RequestedId,
        //            @Remark,
        //            @RequestedOn,
        //            @Type,
        //            @AtchId,
        //            @DocId,
        //            @RespondTime,
        //            @ReportTypeId
        //        )",
        //                new
        //                {
        //                    RemarkId = remarkId,
        //                    CustomerId = dto.CustomerId,
        //                    AuditId = dto.AuditId,
        //                    RequestedId = requestedId,
        //                    Remark = dto.Remark,
        //                    RequestedOn = dto.RequestedOn,
        //                    Type = dto.Type,
        //                    AtchId = attachId,
        //                    DocId = docId,
        //                    RespondTime = dto.RespondTime,
        //                    ReportTypeId = dto.ReportId
        //                });
        //            }
        //        }





        //        public async Task<string> BeginAuditUploadWithReportTypeAsync(AddFileDto dto)
        //        {
        //            try
        //            {
        //                int attachId = dto.AtchId > 0
        //                    ? dto.AtchId
        //                    : await GenerateNextAttachmentIdAsync(dto.AuditId);

        //                int docId = await GetLatestDocIdAsync(dto.CustomerId, dto.AuditId);
        //                int remarkId = await GenerateNextRemarkIdAsync(dto.CustomerId, dto.AuditId);

        //                // Use dto.DrlId directly, assuming it’s already passed from the front-end or fetched earlier
        //                await InsertIntoBeginingAuditDocRemarksLogAsync(dto, dto.DrlId, remarkId, attachId, docId);

        //                var filePath = await SaveAuditDocumentAsync(dto, attachId, dto.File, dto.DrlId, dto.ReportId);

        //                int pkId = await GetDrlPkIdAsync(dto.CustomerId, dto.AuditId, attachId);
        //                await UpdateDrlAttachIdAndDocIdAsync(dto.CustomerId, dto.AuditId, attachId, docId, pkId);

        //                await SendEmailWithAttachmentAsync(dto.EmailId, filePath);

        //                return "Attachment uploaded, details saved, and email sent successfully.";
        //            }
        //            catch (Exception ex)
        //            {
        //                return $"Error: {ex.Message}";
        //            }
        //        }



        public async Task<List<LOEHeadingDto>> LoadLOEHeadingAsync(string sFormName, int compId, int reportTypeId, int loeTemplateId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var primaryQuery = @"
SELECT 
    LTD_ID AS PKID,  
    LTD_HeadingID AS LOEHeadingID, 
    LTD_Heading AS LOEHeading, 
    LTD_Decription AS LOEDesc
FROM LOE_Template_Details 
WHERE 
    LTD_LOE_ID = @LoeTemplateId 
    AND LTD_ReportTypeID = @ReportTypeId 
    AND LTD_FormName = @FormName 
    AND LTD_CompID = @CompId 
ORDER BY LTD_ID";

            var loeDetails = (await connection.QueryAsync<LOEHeadingDto>(primaryQuery, new
            {
                LoeTemplateId = loeTemplateId,
                ReportTypeId = reportTypeId,
                FormName = sFormName,
                CompId = compId
            })).ToList();

            if (loeDetails.Any()) return loeDetails;

            var contentIdQuery = @"SELECT TEM_ContentId FROM SAD_Finalisation_Report_Template WHERE TEM_FunctionId = @ReportTypeId";
            var contentId = await connection.ExecuteScalarAsync<string>(contentIdQuery, new { ReportTypeId = reportTypeId });

            List<LOEHeadingDto> fallbackHeadings;

            if (!string.IsNullOrWhiteSpace(contentId))
            {
                var fallbackQueryWithContent = $@"
SELECT 
    RCM_Id AS LOEHeadingID, 
    RCM_Heading AS LOEHeading, 
    RCM_Description AS LOEDesc
FROM SAD_ReportContentMaster 
WHERE 
    RCM_Id IN ({contentId}) 
    AND RCM_ReportId = @ReportTypeId 
ORDER BY RCM_Id";

                fallbackHeadings = (await connection.QueryAsync<LOEHeadingDto>(fallbackQueryWithContent, new { ReportTypeId = reportTypeId })).ToList();
            }
            else
            {
                var fallbackQuery = @"
SELECT 
    RCM_Id AS LOEHeadingID, 
    RCM_Heading AS LOEHeading, 
    RCM_Description AS LOEDesc
FROM SAD_ReportContentMaster 
WHERE 
    RCM_ReportId = @ReportTypeId 
ORDER BY RCM_Id";

                fallbackHeadings = (await connection.QueryAsync<LOEHeadingDto>(fallbackQuery, new { ReportTypeId = reportTypeId })).ToList();
            }

            return fallbackHeadings;
        }


        public byte[] GeneratePdfByFormName(
       string formName,
       string title,
       List<LOEHeadingDto> headings,
       int reportTypeId,
       int loeTemplateId,
       int customerId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            // using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var ms = new MemoryStream();

            // Fetch report, audit, and customer names
            var reportName = connection.QueryFirstOrDefault<string>(
                "SELECT RTM_ReportTypeName FROM SAD_ReportTypeMaster WHERE RTM_Id = @ReportTypeId",
                new { ReportTypeId = reportTypeId });

            var auditName = connection.QueryFirstOrDefault<string>(
                "SELECT SA_AuditNo FROM StandardAudit_Schedule WHERE SA_ID = @AuditId",
                new { AuditId = loeTemplateId });

            var customerName = connection.QueryFirstOrDefault<string>(
                "SELECT CUST_Name FROM SAD_Customer_Master WHERE CUST_ID = @Id",
                new { Id = customerId });

            // Create the document
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Helvetica"));

                    // Header
                    page.Header().Element(header =>
                    {
                        header.Column(column =>
                        {
                            column.Item().AlignCenter().Text(text =>
                            {
                                text.Span(reportName ?? "Report")
                                    .FontSize(16)
                                    .Bold()
                                    .FontColor(QuestPDF.Helpers.Colors.Black);
                            });

                            // Add some spacing between header and content
                            column.Item().PaddingBottom(10);
                        });
                    });

                    // Content
                    page.Content().Element(content =>
                    {
                        content.Column(col =>
                        {
                            // Client and Audit information (shown once at the top)
                            col.Item().Background(QuestPDF.Helpers.Colors.Grey.Lighten4).Padding(10).Column(infoCol =>
                            {
                                infoCol.Item().Text(text =>
                                {
                                    text.Span("Client: ").SemiBold();
                                    text.Span(customerName ?? "N/A");
                                });

                                infoCol.Item().PaddingTop(5).Text(text =>
                                {
                                    text.Span("Audit No: ").SemiBold();
                                    text.Span(auditName ?? "N/A");
                                });

                                // Add current date after Audit No
                                infoCol.Item().PaddingTop(5).Text(text =>
                                {
                                    text.Span("Date: ").SemiBold();
                                    text.Span(DateTime.Now.ToString("dd-MMM-yy"));
                                });
                            });

                            col.Item().PaddingVertical(10);

                            // Headings section
                            foreach (var heading in headings)
                            {
                                // Heading section
                                col.Item().Background(QuestPDF.Helpers.Colors.Grey.Lighten4).Padding(10).Column(headingCol =>
                                {
                                    headingCol.Item().Text(text =>
                                    {
                                        text.Span((heading.LOEHeading ?? "N/A").Replace("\r", "").Replace("\n", " "));
                                    });
                                });

                                // Description section
                                col.Item().PaddingVertical(10).Column(descCol =>
                                {
                                    if (!string.IsNullOrWhiteSpace(heading.LOEDesc))
                                    {
                                        var lines = heading.LOEDesc
                                            .Replace("\r", "")
                                            .Split('\n', StringSplitOptions.RemoveEmptyEntries);

                                        foreach (var line in lines)
                                        {
                                            descCol.Item().PaddingBottom(5).Text(line.Trim())
                                                .FontSize(11)
                                                .FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                                        }
                                    }
                                    else
                                    {
                                        descCol.Item().Text("No description available.")
                                            .Italic()
                                            .FontSize(11)
                                            .FontColor(QuestPDF.Helpers.Colors.Grey.Lighten1);
                                    }
                                });

                                // Divider between headings (only if not last item)
                                if (headings.IndexOf(heading) != headings.Count - 1)
                                {
                                    col.Item()
                                       .PaddingVertical(5)
                                       .LineHorizontal(1)
                                       .LineColor(QuestPDF.Helpers.Colors.Grey.Lighten3);
                                }
                            }
                        });
                    });

                    // Footer
                    page.Footer().Element(footer =>
                    {
                        footer.AlignCenter().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(9).FontColor(QuestPDF.Helpers.Colors.Grey.Medium));
                            text.Span("Generated on ").SemiBold();
                            text.Span(DateTime.Now.ToString("dd-MMM-yyyy HH:mm"));
                            text.Span(" | Page ");
                            text.CurrentPageNumber();
                        });
                    });
                });
            });

            document.GeneratePdf(ms);
            return ms.ToArray();
        }
        //public async Task<List<LOEHeadingDto>> LoadLOEHeadingAsync(string sFormName, int compId, int reportTypeId, int loeTemplateId)
        //{
        //    using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    await connection.OpenAsync();

        //    string query = @"
        //SELECT 
        //    LTD_ID AS PKID,  
        //    LTD_HeadingID AS LOEHeadingID, 
        //    LTD_Heading AS LOEHeading, 
        //    LTD_Decription AS LOEDesc
        //FROM LOE_Template_Details 
        //WHERE 
        //    LTD_LOE_ID = @LoeTemplateId 
        //    AND LTD_ReportTypeID = @ReportTypeId 
        //    AND LTD_FormName = @FormName 
        //    AND LTD_CompID = @CompId 
        //ORDER BY LTD_ID";

        //    var loeDetails = (await connection.QueryAsync<LOEHeadingDto>(query, new
        //    {
        //        LoeTemplateId = loeTemplateId,
        //        ReportTypeId = reportTypeId,
        //        FormName = sFormName,
        //        CompId = compId
        //    })).ToList();

        //    return loeDetails;
        //}



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
      SAC_ID AS SAC_ID, 
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
                int attachId = await GenerateNextAttachmentIdAsync(dto.AuditId);

                // 2. Get Requested ID (based on document name)
                int requestedId = await GetRequestedIdByDocumentNameAsync(dto.DocumentName);

                // 3. Insert into Audit_DRLLog FIRST and get the primary key
                int drlLogId = await InsertIntoAuditDrlLogAsync(dto, requestedId);

                // 4. Save the attachment and get the path (insert into Edt_Attachments)
                var filePath = await SaveAuditDocumentAsync(dto, attachId, dto.File, drlLogId, dto.ReportType);

                // 5. Get latest DocId for this customer and audit
                int docId = await GetLatestDocIdAsync(dto.CustomerId, dto.AuditId);

                // 6. Generate next Remark ID
                int remarkId = await GenerateNextRemarkIdAsync(dto.CustomerId, dto.AuditId);

                // 7. Insert into Audit_DocRemarksLog
                await InsertIntoAuditDocRemarksLogAsync(dto, requestedId, remarkId, attachId, docId, dto.AdrlId);

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


        //        private async Task<int> InsertAuditDrlLogAsync(InsertFileInfoDto dto, int requestedId)
        //        {
        //            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //            {
        //                await connection.OpenAsync();

        //                var drlLogId = await connection.ExecuteScalarAsync<int>(@"
        //INSERT INTO Audit_DRLLog (
        //    ADRL_YearID, ADRL_AuditNo, ADRL_CustID, 
        //    ADRL_RequestedListID, ADRL_RequestedTypeID, ADRL_RequestedOn, ADRL_TimlinetoResOn,
        //    ADRL_EmailID, ADRL_Comments,
        //    ADRL_CrBy, ADRL_UpdatedBy, ADRL_IPAddress, ADRL_CompID, ADRL_ReportType
        //) VALUES (
        //    @YearId, @AuditId, @CustomerId,
        //    @RequestedId, @RequestedTypeId, @RequestedOn, @TimelineToRespondOn,
        //    @EmailIds, @Remarks,
        //    @UserId, @UpdatedBy, @IpAddress, @CompId, @ReportType
        //);
        //SELECT SCOPE_IDENTITY();",
        //                    new
        //                    {
        //                        YearId = dto.YearId,
        //                        AuditId = dto.AuditId,
        //                        CustomerId = dto.CustomerId,
        //                        RequestedId = requestedId,
        //                        RequestedTypeId = dto.RequestedTypeId,
        //                        RequestedOn = DateTime.Now, // or dto.RequestedOn if needed
        //                        TimelineToRespondOn = dto.TimlineToRespondOn,
        //                        EmailIds = dto.EmailId,
        //                        Remarks = dto.Remark,
        //                        UserId = dto.UserId,
        //                        UpdatedBy = dto.UpdatedBy,
        //                        IpAddress = dto.IpAddress,
        //                        CompId = dto.CompId,
        //                        ReportType = dto.ReportType
        //                    });

        //                return Convert.ToInt32(drlLogId);
        //            }
        //        }

        //private async Task InsertIntoRemarksLogAsync(InsertFileInfoDto dto, int drlLogId, int attachId)
        //{
        //    using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    await connection.OpenAsync();

        //    await connection.ExecuteAsync(@"
        //INSERT INTO StandardAudit_Audit_DRLLog_RemarksHistory (
        //    SAR_ID, SAR_SA_ID, SAR_SAC_ID, SAR_RemarksType,
        //    SAR_Remarks, SAR_RemarksBy, SAR_Date, SAR_IPAddress,
        //    SAR_CompID, SAR_EmailIds, SAR_TimlinetoResOn,
        //    sar_Yearid, SAR_DBFlag, SAR_AttchId, SAR_DRLId
        //) VALUES (
        //    (SELECT ISNULL(MAX(SAR_ID), 0) + 1 FROM StandardAudit_Audit_DRLLog_RemarksHistory),
        //    @AuditId, @CustomerId, 'C',
        //    @Remark, @UserId, GETDATE(), @IpAddress,
        //    @CompId, @EmailId, @TimlineToRespondOn,
        //    @YearId, 'W', @AttachId, @DrlLogId
        //)", new
        //    {
        //        dto.AuditId,
        //        dto.CustomerId,
        //        dto.Remark,
        //        dto.UserId,
        //        dto.IpAddress,
        //        dto.CompId,
        //        dto.EmailId,
        //        dto.TimlineToRespondOn,
        //        dto.YearId,
        //        AttachId = attachId,
        //        DrlLogId = drlLogId
        //    });
        //}

        //private async Task InsertIntoFormOperationLogAsync(InsertFileInfoDto dto, string module, string form, string eventName, int masterId, string masterName, int subMasterId, string subMasterName)
        //{
        //    using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    await connection.OpenAsync();

        //    await connection.ExecuteAsync(@"
        //INSERT INTO Audit_Log_Form_Operations (
        //    ALFO_PKID, ALFO_UserID, ALFO_Module, ALFO_Form, ALFO_Event,
        //    ALFO_MasterID, ALFO_MasterName, ALFO_SubMasterID, ALFO_SubMasterName,
        //    ALFO_IPAddress, ALFO_CompID
        //) VALUES (
        //    (SELECT ISNULL(MAX(ALFO_PKID), 0) + 1 FROM Audit_Log_Form_Operations),
        //    @UserId, @Module, @Form, @Event,
        //    @MasterId, @MasterName, @SubMasterId, @SubMasterName,
        //    @IpAddress, @CompId
        //)",
        //        new
        //        {
        //            UserId = dto.UserId,
        //            Module = module,
        //            Form = form,
        //            Event = eventName,
        //            MasterId = masterId,
        //            MasterName = masterName,
        //            SubMasterId = subMasterId,
        //            SubMasterName = subMasterName,
        //            IpAddress = dto.IpAddress,
        //            CompId = dto.CompId
        //        });
        //}


        //public async Task<int> SaveAuditAllAsync(InsertFileInfoDto dto, int requestedId,
        //string module, string form, string eventName,
        //int masterId, string masterName, int subMasterId, string subMasterName,
        //int attachId)
        //{
        //    using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    await connection.OpenAsync();

        //    using var transaction = connection.BeginTransaction();

        //    try
        //    {
        //        // Call InsertAuditDrlLogAsync
        //       // var drlLogId = await InsertAuditDrlLogAsync(connection, transaction, dto, requestedId);

        //        // Call InsertIntoRemarksLogAsync
        //        await InsertIntoRemarksLogAsync(connection, transaction, dto, drlLogId, attachId);

        //        // Call InsertIntoFormOperationLogAsync
        //        await InsertIntoFormOperationLogAsync(connection, transaction, dto, module, form, eventName,
        //            masterId, masterName, subMasterId, subMasterName);

        //        await transaction.CommitAsync();

        //        //return drlLogId;
        //    }
        //    catch
        //    {
        //        await transaction.RollbackAsync();
        //        throw;
        //    }
        //}

        //        private async Task<int> InsertAuditDrlLogAsync(SqlConnection connection, SqlTransaction transaction, InsertFileInfoDto dto, int requestedId)
        //        {
        //            var drlLogId = await connection.ExecuteScalarAsync<int>(@"
        //INSERT INTO Audit_DRLLog (
        //    ADRL_YearID, ADRL_AuditNo, ADRL_CustID, 
        //    ADRL_RequestedListID, ADRL_RequestedTypeID, ADRL_RequestedOn, ADRL_TimlinetoResOn,
        //    ADRL_EmailID, ADRL_Comments,
        //    ADRL_CrBy, ADRL_UpdatedBy, ADRL_IPAddress, ADRL_CompID, ADRL_ReportType
        //) VALUES (
        //    @YearId, @AuditId, @CustomerId,
        //    @RequestedId, @RequestedTypeId, @RequestedOn, @TimelineToRespondOn,
        //    @EmailIds, @Remarks,
        //    @UserId, @UpdatedBy, @IpAddress, @CompId, @ReportType
        //);
        //SELECT CAST(SCOPE_IDENTITY() as int);",
        //                new
        //                {
        //                    YearId = dto.YearId,
        //                    AuditId = dto.AuditId,
        //                    CustomerId = dto.CustomerId,
        //                    RequestedId = requestedId,
        //                    RequestedTypeId = dto.RequestedTypeId,
        //                    RequestedOn = DateTime.Now,
        //                    TimelineToRespondOn = dto.TimlineToRespondOn,
        //                    EmailIds = dto.EmailId,
        //                    Remarks = dto.Remark,
        //                    UserId = dto.UserId,
        //                    UpdatedBy = dto.UpdatedBy,
        //                    IpAddress = dto.IpAddress,
        //                    CompId = dto.CompId,
        //                    ReportType = dto.ReportType
        //                }, transaction);

        //            return drlLogId;
        //        }

        //        private async Task InsertIntoRemarksLogAsync(SqlConnection connection, SqlTransaction transaction, InsertFileInfoDto dto, int drlLogId, int attachId)
        //        {
        //            await connection.ExecuteAsync(@"
        //INSERT INTO StandardAudit_Audit_DRLLog_RemarksHistory (
        //    SAR_ID, SAR_SA_ID, SAR_SAC_ID, SAR_RemarksType,
        //    SAR_Remarks, SAR_RemarksBy, SAR_Date, SAR_IPAddress,
        //    SAR_CompID, SAR_EmailIds, SAR_TimlinetoResOn,
        //    sar_Yearid, SAR_DBFlag, SAR_AttchId, SAR_DRLId
        //) VALUES (
        //    (SELECT ISNULL(MAX(SAR_ID), 0) + 1 FROM StandardAudit_Audit_DRLLog_RemarksHistory),
        //    @AuditId, @CustomerId, 'C',
        //    @Remark, @UserId, GETDATE(), @IpAddress,
        //    @CompId, @EmailId, @TimlineToRespondOn,
        //    @YearId, 'W', @AttachId, @DrlLogId
        //)",
        //                new
        //                {
        //                    dto.AuditId,
        //                    dto.CustomerId,
        //                    dto.Remark,
        //                    dto.UserId,
        //                    dto.IpAddress,
        //                    dto.CompId,
        //                    dto.EmailId,
        //                    dto.TimlineToRespondOn,
        //                    dto.YearId,
        //                    AttachId = attachId,
        //                    DrlLogId = drlLogId
        //                }, transaction);
        //        }

        //        private async Task InsertIntoFormOperationLogAsync(SqlConnection connection, SqlTransaction transaction, InsertFileInfoDto dto,
        //     string module, string form, string eventName,
        //     int masterId, string masterName, int subMasterId, string subMasterName)
        //        {
        //            await connection.ExecuteAsync(@"
        //INSERT INTO Audit_Log_Form_Operations (
        //    ALFO_PKID, ALFO_UserID, ALFO_Module, ALFO_Form, ALFO_Event,
        //    ALFO_MasterID, ALFO_MasterName, ALFO_SubMasterID, ALFO_SubMasterName,
        //    ALFO_IPAddress, ALFO_CompID
        //) VALUES (
        //    (SELECT ISNULL(MAX(ALFO_PKID), 0) + 1 FROM Audit_Log_Form_Operations),
        //    @UserId, @Module, @Form, @Event,
        //    @MasterId, @MasterName, @SubMasterId, @SubMasterName,
        //    @IpAddress, @CompId
        //)",
        //                new
        //                {
        //                    UserId = dto.UserId,
        //                    Module = module,
        //                    Form = form,
        //                    Event = eventName,
        //                    MasterId = masterId,
        //                    MasterName = masterName,
        //                    SubMasterId = subMasterId,
        //                    SubMasterName = subMasterName,
        //                    IpAddress = dto.IpAddress,
        //                    CompId = dto.CompId
        //                }, transaction);
        //        

        //private async Task InsertIntoRemarksLogAsync(InsertFileInfoDto dto, int drlLogId, int attachId)
        //    {
        //        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //        await connection.OpenAsync();

        //        await connection.ExecuteAsync(@"
        //    INSERT INTO StandardAudit_Audit_DRLLog_RemarksHistory (
        //        SAR_ID, SAR_SA_ID, SAR_SAC_ID, SAR_RemarksType,
        //        SAR_Remarks, SAR_RemarksBy, SAR_Date, SAR_IPAddress,
        //        SAR_CompID, SAR_EmailIds, SAR_TimlinetoResOn,
        //        sar_Yearid, SAR_DBFlag, SAR_AttchId, SAR_DRLId
        //    ) VALUES (
        //        (SELECT ISNULL(MAX(SAR_ID), 0) + 1 FROM StandardAudit_Audit_DRLLog_RemarksHistory),
        //        @AuditId, @CustomerId, 'C',
        //        @Remark, @UserId, GETDATE(), @IpAddress,
        //        @CompId, @EmailId, @TimlineToRespondOn,
        //        @YearId, 'W', @AttachId, @DrlLogId
        //    )", new
        //        {
        //            dto.AuditId,
        //            dto.CustomerId,
        //            dto.Remark,
        //            dto.UserId,
        //            dto.IpAddress,
        //            dto.CompId,
        //            dto.EmailId,
        //            dto.TimlineToRespondOn,
        //            dto.YearId,
        //            AttachId = attachId,
        //            DrlLogId = drlLogId
        //        });
        //    }

        //    private async Task InsertIntoFormOperationLogAsync(InsertFileInfoDto dto, string module, string form, string eventName, int masterId, string masterName, int subMasterId, string subMasterName)
        //    {
        //        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //        await connection.OpenAsync();

        //        await connection.ExecuteAsync(@"
        //    INSERT INTO Audit_Log_Form_Operations (
        //        ALFO_PKID, ALFO_UserID, ALFO_Module, ALFO_Form, ALFO_Event,
        //        ALFO_MasterID, ALFO_MasterName, ALFO_SubMasterID, ALFO_SubMasterName,
        //        ALFO_IPAddress, ALFO_CompID
        //    ) VALUES (
        //        (SELECT ISNULL(MAX(ALFO_PKID), 0) + 1 FROM Audit_Log_Form_Operations),
        //        @UserId, @Module, @Form, @Event,
        //        @MasterId, @MasterName, @SubMasterId, @SubMasterName,
        //        @IpAddress, @CompId
        //    )",
        //            new
        //            {
        //                UserId = dto.UserId,
        //                Module = module,
        //                Form = form,
        //                Event = eventName,
        //                MasterId = masterId,
        //                MasterName = masterName,
        //                SubMasterId = subMasterId,
        //                SubMasterName = subMasterName,
        //                IpAddress = dto.IpAddress,
        //                CompId = dto.CompId
        //            });
        //    }

        //public async Task<int> SaveOrUpdateAuditDrlLogAsync(InsertAuditRemarksDto dto)
        //{
        //    using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    var parameters = new DynamicParameters();

        //    parameters.Add("@ADRL_ID", dto.DrlId); // Pass actual ID for update

        //    parameters.Add("@ADRL_YearID", dto.YearId);
        //    parameters.Add("@ADRL_AuditNo", dto.AuditId);
        //    parameters.Add("@ADRL_FunID", dto.FunctionId);
        //    parameters.Add("@ADRL_CustID", dto.CustomerId);
        //    parameters.Add("@ADRL_RequestedListID", dto.RequestedListId);
        //    parameters.Add("@ADRL_RequestedTypeID", dto.RequestedTypeId);
        //    parameters.Add("@ADRL_RequestedOn", dto.RequestedOn);
        //    parameters.Add("@ADRL_TimlinetoResOn", dto.TimelineToRespondOn);
        //    parameters.Add("@ADRL_EmailID", dto.EmailId);
        //    parameters.Add("@ADRL_Comments", dto.Remark);
        //    parameters.Add("@ADRL_CrBy", dto.UserId);
        //    parameters.Add("@ADRL_UpdatedBy", dto.UpdatedBy);
        //    parameters.Add("@ADRL_IPAddress", dto.IpAddress);
        //    parameters.Add("@ADRL_CompID", dto.CompId);
        //    parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
        //    parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //    await connection.ExecuteAsync("spAudit_DRLLog", parameters, commandType: CommandType.StoredProcedure);

        //    return parameters.Get<int>("@iOper");
        //}

        //public async Task SaveRemarksHistoryAsync(InsertAuditRemarksDto dto)
        //{
        //    var sql = @"
        //INSERT INTO StandardAudit_Audit_DRLLog_RemarksHistory
        //(SAR_ID, SAR_SA_ID, SAR_SAC_ID, SAR_CheckPointIDs, SAR_RemarksType, SAR_Remarks, 
        // SAR_RemarksBy, SAR_Date, SAR_IPAddress, SAR_CompID, SAR_EmailIds, SAR_TimlinetoResOn, 
        // sar_Yearid, SAR_DBFlag, SAR_MASid, SAR_AttchId, SAR_DRLId)
        //VALUES (
        //    (SELECT ISNULL(MAX(SAR_ID) + 1, 1) FROM StandardAudit_Audit_DRLLog_RemarksHistory),
        //    @AuditId, @CustomerId, @CheckPointIds, 'C', @Remark,
        //    @UserId, GETDATE(), @IpAddress, @CompId, @EmailId, @TimelineToRespondOn,
        //    @YearId, 'W', @MasId, @AttachId, @DrlId);";

        //    using var conn1 = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    using var conn2 = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        //    await conn1.ExecuteAsync(sql, dto);
        //    await conn2.ExecuteAsync(sql, dto);
        //}

        //public async Task<int> SaveAuditDataAsync(InsertAuditRemarksDto dto)
        //{
        //    // Save or update Audit_DRLLog and receive the actual DRL ID
        //    int drlId = await SaveOrUpdateAuditDrlLogAsync(dto);
        //    return drlId;
        //}

        //public async Task<int> SaveOrUpdateAuditDrlLogAsync(InsertAuditRemarksDto dto)
        //{
        //    using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    var parameters = new DynamicParameters();

        //    // Pass ID (0 for insert, or actual ID for update)
        //    parameters.Add("@ADRL_ID", dto.DrlId);
        //    parameters.Add("@ADRL_YearID", dto.YearId);
        //    parameters.Add("@ADRL_AuditNo", dto.AuditId);
        //    parameters.Add("@ADRL_FunID", dto.FunctionId);
        //    parameters.Add("@ADRL_CustID", dto.CustomerId);
        //    parameters.Add("@ADRL_RequestedListID", dto.RequestedListId);
        //    parameters.Add("@ADRL_RequestedTypeID", dto.RequestedTypeId);
        //    parameters.Add("@ADRL_RequestedOn", dto.RequestedOn);
        //    parameters.Add("@ADRL_TimlinetoResOn", dto.TimelineToRespondOn);
        //    parameters.Add("@ADRL_EmailID", dto.EmailId);
        //    parameters.Add("@ADRL_Comments", dto.Remark);
        //    parameters.Add("@ADRL_CrBy", dto.UserId);
        //    parameters.Add("@ADRL_UpdatedBy", dto.UpdatedBy);
        //    parameters.Add("@ADRL_IPAddress", dto.IpAddress);
        //    parameters.Add("@ADRL_CompID", dto.CompId);

        //    // OUTPUT parameters
        //    parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output); // 3 = insert, 2 = update
        //    parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);         // returns DRL_ID

        //    await connection.ExecuteAsync("spAudit_DRLLog", parameters, commandType: CommandType.StoredProcedure);

        //    int operationType = parameters.Get<int>("@iUpdateOrSave");
        //    int returnedDrlId = parameters.Get<int>("@iOper");

        //    // Insert into RemarksHistory only for new inserts
        //    if (operationType == 3)
        //    {
        //        dto.DrlId = returnedDrlId; // assign the inserted DRL ID
        //        await SaveRemarksHistoryAsync(dto);
        //    }

        //    return returnedDrlId;
        //}



        public async Task<(int DrlId, bool IsInsert)> SaveOrUpdateAuditDrlLogAsync(InsertAuditRemarksDto dto)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            // using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var emailCsv = dto.EmailId != null ? string.Join(",", dto.EmailId) : null;

            // First check if record exists with matching keys
            var existingId = await connection.QueryFirstOrDefaultAsync<int?>(@"
        SELECT ADRL_ID FROM Audit_DRLLog 
        WHERE ADRL_AuditNo = @AuditNo 
          AND ADRL_FunID = @FunID 
          AND ADRL_CustID = @CustID
          AND ADRL_RequestedListID = @RequestedListID
          AND ADRL_RequestedTypeID = @RequestedTypeID
          AND ADRL_CompID = @CompID
          AND ADRL_YearID = @YearID",
                new
                {
                    AuditNo = dto.AuditId,
                    FunID = dto.FunctionId,
                    CustID = dto.CustomerId,
                    RequestedListID = dto.RequestedListId,
                    RequestedTypeID = dto.RequestedTypeId,
                    CompID = dto.CompId,
                    YearID = dto.YearId
                });

            if (existingId.HasValue)
            {
                // Update existing record
                await connection.ExecuteAsync(@"
            UPDATE Audit_DRLLog SET 
                ADRL_RequestedOn = @RequestedOn,
                ADRL_EmailID = @EmailId,
                ADRL_Comments = @Remark,
                ADRL_UpdatedBy = @UpdatedBy,
                ADRL_UpdatedOn = GETDATE(),
                ADRL_IPAddress = @IpAddress,
                ADRL_Status = 'Updated',
                ADRL_TimlinetoResOn = '@TimelineToRespondOn'
               
                
            WHERE ADRL_ID = @Id",
                    new
                    {
                        RequestedOn = dto.RequestedOn,
                        EmailId = emailCsv,
                        Remark = dto.Remark,
                        UpdatedBy = dto.UpdatedBy,
                        IpAddress = dto.IpAddress,
                        TimelineToRespondOn = dto.TimelineToRespondOn,
                        Id = existingId.Value

                    });

                return (existingId.Value, false);
            }
            else
            {
                // Insert new record with max+1 ADRL_ID
                var maxId = await connection.QueryFirstOrDefaultAsync<int?>("SELECT MAX(ADRL_ID) FROM Audit_DRLLog") ?? 0;
                int newId = maxId + 1;

                await connection.ExecuteAsync(@"
            INSERT INTO Audit_DRLLog (
                ADRL_ID, ADRL_YearID, ADRL_AuditNo, ADRL_FunID, ADRL_CustID, 
                ADRL_RequestedListID, ADRL_RequestedTypeID, ADRL_RequestedOn, ADRL_EmailID,
                ADRL_Comments, ADRL_CrBy, ADRL_CrOn, ADRL_IPAddress, ADRL_CompID, ADRL_Status, ADRL_TimlinetoResOn
            ) VALUES (
                @Id, @YearID, @AuditNo, @FunID, @CustID,
                @RequestedListID, @RequestedTypeID, @RequestedOn, @EmailId,
                @Remark, @UserId, GETDATE(), @IpAddress, @CompID, 'Saved', GETDATE()
            )",
                    new
                    {
                        Id = newId,
                        YearID = dto.YearId,
                        AuditNo = dto.AuditId,
                        FunID = dto.FunctionId,
                        CustID = dto.CustomerId,
                        RequestedListID = dto.RequestedListId,
                        RequestedTypeID = dto.RequestedTypeId,
                        RequestedOn = dto.RequestedOn,
                        TimelineToRespondOn = dto.TimelineToRespondOn,
                        EmailId = emailCsv,
                        Remark = dto.Remark,
                        UserId = dto.UserId,
                        IpAddress = dto.IpAddress,
                        CompID = dto.CompId
                    });

                await SaveRemarksHistoryAsync(dto, masId: newId);

                return (newId, true);
            }
        }



        public async Task SaveRemarksHistoryAsync(InsertAuditRemarksDto dto, int masId)
        {
            const string sql = @"
INSERT INTO StandardAudit_Audit_DRLLog_RemarksHistory
(SAR_ID, SAR_SA_ID, SAR_SAC_ID, SAR_CheckPointIDs, SAR_RemarksType, SAR_Remarks, 
 SAR_RemarksBy, SAR_Date, SAR_IPAddress, SAR_CompID, SAR_EmailIds, SAR_TimlinetoResOn, 
 sar_Yearid, SAR_DBFlag, SAR_MASid, SAR_AttchId, SAR_DRLId)
VALUES (
    (SELECT ISNULL(MAX(SAR_ID) + 1, 1) FROM StandardAudit_Audit_DRLLog_RemarksHistory),
    @AuditId, @CustomerId, @CheckPointIds, 'C', @Remark,
    @UserId, GETDATE(), @IpAddress, @CompId, @EmailId, GETDATE(),
    @YearId, 'W', @MasId, @AttachId, @DrlId);";

            //using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            var emailCsv = dto.EmailId != null ? string.Join(",", dto.EmailId) : null;

            await connection.ExecuteAsync(sql, new
            {
                dto.AuditId,
                dto.CustomerId,
                dto.CheckPointIds,
                dto.Remark,
                dto.UserId,
                dto.IpAddress,
                dto.CompId,
                EmailId = emailCsv,
                dto.TimelineToRespondOn,
                dto.YearId,
                MasId = masId,    // Explicit mapping for SAR_MASid
                dto.AttachId,
                DrlId = dto.RequestedListId         // ADRL_ID from DRL log
            });
        }

        public async Task<(int DrlId, bool IsInsert)> SaveAuditDataAsync(InsertAuditRemarksDto dto)
        {
            var (drlId, isInsert) = await SaveOrUpdateAuditDrlLogAsync(dto);
            dto.DrlId = drlId;
            await SaveRemarksHistoryAsync(dto, masId: drlId);
            await SendDuringAuditEmailAsync(dto);
            return (drlId, isInsert);
        }

        private async Task SendDuringAuditEmailAsync(InsertAuditRemarksDto dto)
        {
            if (dto.EmailId == null || !dto.EmailId.Any())
                return;

            // ✅ Fetch Audit Info
            var (auditNo, auditName) = await GetAuditInfoByIdAsync(dto.AuditId);

            var smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("harsha.s2700@gmail.com", "edvemvlmgfkcasrp"),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress("harsha.s2700@gmail.com"),
                Subject = $"Intimation mail for sharing the Documents requested by the Auditor - {auditNo}",
                IsBodyHtml = true
            };

            // ✅ Add To and CC
            mail.To.Add(dto.EmailId[0]);
            for (int i = 1; i < dto.EmailId.Count; i++)
            {
                mail.CC.Add(dto.EmailId[i]);
            }

            var requestedOn = dto.RequestedOn;

            // ✅ Updated Body with AuditNo and AuditName
            string body = $@"
        <p><strong>Intimation mail</strong></p>
        <p>Document Requested</p>
        <p>Greetings from TRACe PA.</p>
        <p>This mail is an intimation for sharing the documents requested by the Auditor's office.</p>

        <p><strong>Audit No.:</strong> {auditNo} - {auditName} and Date : {requestedOn}</p>
        <p><strong>Document Requested List:</strong> Journal Entries</p>";

            if (!string.IsNullOrWhiteSpace(dto.Remark))
            {
                body += $@"
        <p><strong>Specific request for client:</strong></p>
        <p>{dto.Remark}</p>";
            }

            body += @"
        <br />
        <p>Please login to TRACe PA website using the link and credentials shared with you.</p>
        <p><a href='https://tracepacust-user.multimedia.interactivedns.com/'>TRACe PA Portal</a></p>
        <p>Home page of the application will show you the list of documents requested by the auditor. Upload all the requested documents using links provided.</p>
        <br />
        <p>Thanks,</p>
        <p>TRACe PA Team</p>";

            mail.Body = body;

            await smtpClient.SendMailAsync(mail);
        }





        public async Task<List<DRLDetailDto>> LoadDRLdgAsync(int compId, int auditNo)
        {
            //using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
           

            int iBA = await GetDRLBeginningoftheAuditIDAsync(compId);
            int iCA = await GetDRLNearingCompletionoftheAuditIDAsync(compId);

            var sql = @"
SELECT 
    CMM_ID,  CASE 
        WHEN ADRL_ReportType > 0 THEN CMM_Desc + ' - ' + RTM_ReportTypeName 
        ELSE CMM_Desc 
    END AS CMM_Desc, DRL_DRLID, DRL_Name, ADRL_ID AS DrlpkId, ADRL_YearID, ADRL_AuditNo, ADRL_FunID, 
    ACM_Checkpoint, ADRL_CustID, ADRL_RequestedListID, ADRL_RequestedTypeID, ADRL_RequestedOn, 
    ADRL_TimlinetoResOn, ADRL_EmailID, ADRL_Comments, ADRL_Status, ADRL_AttachID, ADRL_CompID, 
    ADRL_ReceivedComments, ADRL_LogStatus, ADRL_ReceivedOn, ADRL_ReportType
FROM Audit_DRLLog
LEFT JOIN AuditType_Checklist_Master ON ACM_ID = ADRL_FunID
LEFT JOIN Content_Management_Master 
    ON CMM_ID = ADRL_RequestedListID AND CMM_CompID = @CompId
LEFT JOIN SAD_ReportTypeMaster 
    ON ADRL_ReportType = RTM_Id

LEFT JOIN Audit_Doc_Request_List ON DRL_DRLID = ADRL_RequestedTypeID AND DRL_CompID = @CompId
WHERE ADRL_CompID = @CompId
  AND ADRL_AuditNo = @AuditNo";

            var parameters = new



            {
                CompId = compId,
                AuditNo = auditNo,
                iBA,
                iCA
            };

            var data = (await connection.QueryAsync<dynamic>(sql, parameters)).ToList();

            var results = new List<DRLDetailDto>();

            foreach (var row in data)
            {
                var dto = new DRLDetailDto
                {

                    DrlpkId = row.DrlpkId ?? 0,  // ✅ Correctly mapping the PK
                    DRLID = row.DRL_DRLID ?? 0,
                    CheckPointID = row.ADRL_FunID ?? 0,
                    CheckPoint = row.ACM_Checkpoint ?? "Others",
                    DocumentRequestedListID = row.CMM_ID ?? 0,
                    DocumentRequestedList = row.CMM_Desc ?? "Others",
                    DocumentRequestedTypeID = row.DRL_DRLID ?? 0,
                    DocumentRequestedType = row.DRL_Name ?? "Others",
                    RequestedOn = FormatDate(row.ADRL_RequestedOn),
                    TimlinetoResOn = FormatDate(row.ADRL_TimlinetoResOn),
                    EmailID = row.ADRL_EmailID?.Replace(".com", ".com "),
                    Comments = row.ADRL_Comments,
                    ReceivedComments = ReplaceSafeSQL(row.ADRL_ReceivedComments),
                    ReceivedOn = FormatDate(row.ADRL_ReceivedOn),
                    AttachID = row.ADRL_AttachID ?? 0,
                    ReportType = row.ADRL_ReportType ?? 0,
                    Status = row.ADRL_LogStatus switch
                    {
                        1 => "Outstanding",



                        2 => "Acceptable",
                        3 => "Partially",
                        4 => "No",
                        _ => null
                    }
                };

                results.Add(dto);
            }

            return results;
        }

        private string FormatDate(object dateObj)
        {
            if (dateObj == null || dateObj == DBNull.Value)
                return string.Empty;

            if (DateTime.TryParse(dateObj.ToString(), out DateTime date))
                return date.ToString("yyyy-MM-dd"); // or any desired format

            return string.Empty;
        }

        private string ReplaceSafeSQL(string input)
        {
            return string.IsNullOrEmpty(input) ? "" : input.Replace("'", "''");
        }

        public async Task<int> GetDRLBeginningoftheAuditIDAsync(int compId)
        {
            // using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            const string query = @"
SELECT CMM_ID 
FROM Content_Management_Master 
WHERE CMM_CompID = @CompId 
  AND CMM_Desc LIKE '%Beginning of the Audit%'";

            var id = await connection.ExecuteScalarAsync<int?>(query, new { CompId = compId });
            return id ?? 0;
        }

        public async Task<int> GetDRLNearingCompletionoftheAuditIDAsync(int compId)
        {
            //using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            const string query = @"
SELECT CMM_ID 
FROM Content_Management_Master 
WHERE CMM_CompID = @CompId 
  AND CMM_Desc LIKE '%Nearing Completion of the Audit%'";

            var id = await connection.ExecuteScalarAsync<int?>(query, new { CompId = compId });
            return id ?? 0;
        }


        public async Task<int> GetRequestedIdByExportTypeAsync(int exportType)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string description = exportType switch
            {
                1 => "Beginning of the Audit",
                2 => "During of the Audit",
                3 => "Nearing Completion of the Audit",
                _ => null
            };

            if (string.IsNullOrEmpty(description))
                return 0;

            var query = @"
        SELECT TOP 1
            TRY_CAST(cmm_ID AS INT) AS ParsedId
        FROM Content_Management_Master cmm
        LEFT JOIN Audit_DRLLog a ON TRY_CAST(a.ADRL_RequestedListID AS INT) = TRY_CAST(cmm.cmm_ID AS INT)
        WHERE 
            cmm_Category = 'DRL' AND 
            cmm_Desc = @Description AND
            ISNUMERIC(cmm_ID) = 1 AND
            cmm_ID NOT LIKE '%,%'";

            var result = await connection.ExecuteScalarAsync<int?>(query, new { Description = description });

            return result ?? 0;
        }

        public async Task<int> GetMaxAttachmentIdAsync(int customerId, int auditId, int yearId, int exportType)
        {
            //using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            const string query = @"
        SELECT ISNULL(MAX(ATCH_ID), 0) 
        FROM Edt_Attachments ea
        LEFT JOIN StandardAudit_Audit_DRLLog_RemarksHistory a ON a.SAR_AttchId = ea.ATCH_ID
        WHERE a.SAR_SAC_ID = @CustomerId
          AND a.SAR_SA_ID = @AuditId
          AND a.sar_Yearid = @YearId";

            var maxId = await connection.ExecuteScalarAsync<int>(query, new
            {
                CustomerId = customerId,
                AuditId = auditId,
                YearId = yearId,
                ExportType = exportType
            });

            return maxId;
        }

        public async Task<int?> GetMaxAttachmentIdAsync(GetMaxAttachmentIdRequest request)
        {
            const string query = @"
        SELECT MAX(ATCH_ID)
        FROM Edt_Attachments
        LEFT JOIN StandardAudit_Audit_DRLLog_RemarksHistory a ON a.SAR_AttchId = ATCH_ID
        WHERE SAR_SAC_ID = @CustomerId
          AND SAR_SA_ID = @AuditId
         AND sar_Yearid = @YearId 

          
          AND ATCH_ReportType = @ReportTypeId";

            //using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            return await connection.ExecuteScalarAsync<int?>(query, request);
        }


        public async Task<IEnumerable<CustomerUserDto>> GetAllCustomerUsersAsync(int customerId)
        {
            // using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);

            string query = @"
        SELECT usr_FullName AS UsrFullName,  usr_Email AS Email,
               usr_Id AS UsrId
        FROM Sad_UserDetails
        WHERE usr_Type = 'C'
          AND usr_CompanyId = @CustomerId";

            return await connection.QueryAsync<CustomerUserDto>(query, new { CustomerId = customerId });
        }

        public async Task<List<int>> GetAttachIdsAsync(string connectionStringName, int companyId)
        {
            var connectionString = _configuration.GetConnectionString(connectionStringName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT DISTINCT ATCH_ID
        FROM edt_attachments
        WHERE 
            ATCH_CompID = @CompanyId
            AND ATCH_Status <> 'D'
            AND Atch_Vstatus IN ('A', 'AS', 'C', 'DS')
        ORDER BY ATCH_ID";

            var attachIds = (await connection.QueryAsync<int>(query, new { CompanyId = companyId })).ToList();
            return attachIds;
        }

        public async Task<List<AttachmentDto>> LoadAllAttachmentsAsync(
            string connectionStringName,
            int companyId,
            int attachId)
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
    ATCH_CREATEDON,
    ATCH_SIZE,
    ATCH_ReportType,
    CASE 
        WHEN Atch_Vstatus = 'AS' THEN 'Not Shared'
        WHEN Atch_Vstatus = 'A' THEN 'Shared'
        WHEN Atch_Vstatus = 'C' THEN 'Received'
        WHEN Atch_Vstatus = 'DS' THEN 'Received'
    END AS Atch_Vstatus
FROM edt_attachments
WHERE 
    ATCH_CompID = @CompanyId 
    AND ATCH_ID = @AttachId 
    AND ATCH_Status <> 'D' 
    AND Atch_Vstatus IN ('A', 'AS', 'C', 'DS')";

            var rawData = (await connection.QueryAsync(query, new
            {
                CompanyId = companyId,
                AttachId = attachId
            })).ToList();

            // Optional optimization: pre-fetch all distinct user names by CreatedById
            var createdByIds = rawData.Select(r => (int)r.ATCH_CreatedBy).Distinct().ToList();

            var userNames = new Dictionary<int, string>();
            foreach (var userId in createdByIds)
            {
                var fullName = await GetUserFullNameFromUserIDAsync(companyId, userId, connection);
                userNames[userId] = fullName;
            }

            var result = new List<AttachmentDto>();
            int index = 1;

            foreach (var row in rawData)
            {
                int createdById = Convert.ToInt32(row.ATCH_CreatedBy);
                result.Add(new AttachmentDto
                {
                    SrNo = index++,
                    AtchID = Convert.ToInt32(row.Atch_DocID),
                    DrlId = Convert.ToInt32(row.ATCH_DRLID),
                    FName = $"{row.ATCH_FNAME}.{row.ATCH_EXT}",
                    FDescription = row.ATCH_Desc?.ToString() ?? "",
                    CreatedById = createdById,
                    CreatedBy = userNames.ContainsKey(createdById) ? userNames[createdById] : "Unknown User",
                    CreatedOn = row.ATCH_CREATEDON != null ? Convert.ToDateTime(row.ATCH_CREATEDON).ToString("dd-MMM-yyyy") : "",
                    FileSize = $"{(Convert.ToDouble(row.ATCH_SIZE) / 1024):0.00} KB",
                    Extention = row.ATCH_EXT?.ToString(),
                    Type = row.ATCH_ReportType?.ToString(),
                    Status = row.Atch_Vstatus?.ToString()
                });
            }

            return result;
        }

        private async Task<string> GetUserFullNameFromUserIDAsync(int companyId, int userId, IDbConnection connection)
        {
            var query = "SELECT FullName FROM Users WHERE CompanyId = @CompanyId AND UserId = @UserId";
            return await connection.QueryFirstOrDefaultAsync<string>(query, new { CompanyId = companyId, UserId = userId })
                   ?? "Unknown User";
        }

        public async Task<IEnumerable<StandardAuditCheckpointDto>> LoadSelectedStandardAuditCheckPointDetailsAsync(
    string connStr, int compId, int auditId, int empId, bool isPartner, int headingId, string heading)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(connStr));
            await connection.OpenAsync();

            string checkpointIds = string.Empty;

            if (!isPartner)
            {
                var innerQuery = @"
            SELECT ISNULL(STUFF((
                SELECT ',' + CAST(SACD_CheckpointId AS VARCHAR(MAX)),
                
                FROM StandardAudit_Checklist_Details
                WHERE SACD_EmpId = @EmpId AND SACD_AuditId = @AuditId
                FOR XML PATH('')), 1, 1, ''), '')";

                checkpointIds = await connection.ExecuteScalarAsync<string>(innerQuery, new { EmpId = empId, AuditId = auditId });
            }

            var sql = $@"
        SELECT
            @AuditId AS AuditId,
            DENSE_RANK() OVER (ORDER BY SAC_CheckPointID) AS SrNo,
            SAC_ID AS ConductAuditCheckPointPKId,
            SAC_CheckPointID AS CheckPointID,
            ACM_Heading AS Heading,
            ACM_Checkpoint AS [CheckPoint], -- fix: alias wrapped
            ISNULL(ACM_Assertions, '') AS Assertions,
            SAC_Remarks AS Remarks,
            CASE WHEN SAC_Mandatory = 1 THEN 'Yes' ELSE 'No' END AS Mandatory,
            SAC_TestResult AS TestResult,
            SAC_ReviewerRemarks AS ReviewerRemarks,
            COALESCE(SAC_AttachID, 0) AS AttachmentID,
            CASE WHEN SAC_Annexure = 1 THEN 'TRUE' ELSE 'FALSE' END AS Annexure,
            a.USr_FullName AS ConductedBy,
            SAC_LastUpdatedOn, -- removed FORMAT
            ISNULL(SSW_ID, 0) AS WorkpaperId,
            ISNULL(SSW_WorkpaperNo, '') AS WorkpaperNo,
            ISNULL(SSW_WorkpaperRef, '') AS WorkpaperRef,
            ISNULL(b.USr_FullName, '') AS CreatedBy,
            SSW_CrOn AS CreatedOn, -- removed FORMAT
            (SELECT COUNT(*) FROM Audit_DRLLog WHERE ADRL_AuditNo = @AuditId AND ADRL_FunID = SAC_CheckPointID) AS DRLCount
        FROM StandardAudit_ScheduleCheckPointList
        JOIN AuditType_Checklist_Master ON ACM_ID = SAC_CheckPointID
        LEFT JOIN sad_userdetails a ON a.Usr_ID = SAC_ConductedBy
        LEFT JOIN StandardAudit_ScheduleConduct_WorkPaper ON SSW_SA_ID = @AuditId AND SSW_ID = SAC_WorkpaperID
        LEFT JOIN sad_userdetails b ON b.Usr_ID = SSW_CrBy
        WHERE SAC_SA_ID = @AuditId AND SAC_CompID = @CompId";

            if (headingId > 0 && !string.IsNullOrEmpty(heading))
            {
                sql += @"
            AND ACM_ID IN (
                SELECT ACM_ID FROM AuditType_Checklist_Master
                WHERE ACM_Heading = @Heading AND ACM_CompId = @CompId AND ACM_DELFLG = 'A')";
            }

            if (!isPartner && !string.IsNullOrWhiteSpace(checkpointIds))
            {
                sql += $" AND SAC_CheckPointID IN ({checkpointIds})";
            }

            sql += " ORDER BY SAC_CheckPointID";

            var parameters = new
            {
                AuditId = auditId,
                CompId = compId,
                Heading = heading
            };

            return await connection.QueryAsync<StandardAuditCheckpointDto>(sql, parameters);
        }

        public async Task<ConductAuditWorkpaperDto?> LoadSelectedConductAuditWorkPapersDetailsAsync(
    string connStrName, int compId, int auditId, int workpaperId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(connStrName));
            await connection.OpenAsync();

            string sql = @"
        SELECT 
            SSW_ID,
            SSW_WorkpaperNo,
            SSW_WorkpaperRef,
            SSW_Observation,
            SSW_Conclusion,
            SSW_TypeOfTest,
            SSW_WPCheckListID,
            SSW_DRLID,
            SSW_Status,
            SSW_ExceededMateriality,
            SSW_AuditorHoursSpent,
            SSW_NotesSteps,
            SSW_CriticalAuditMatter,
            SSW_AttachID
        FROM StandardAudit_ScheduleConduct_WorkPaper
        WHERE SSW_SA_ID = @AuditId AND SSW_ID = @WorkpaperId AND SSW_CompID = @CompId";

            var parameters = new { AuditId = auditId, WorkpaperId = workpaperId, CompId = compId };

            return await connection.QueryFirstOrDefaultAsync<ConductAuditWorkpaperDto>(sql, parameters);
        }


        public async Task<IEnumerable<DrlRemarksHistoryDto>> LoadSelectedDRLCheckPointRemarksHistoryDetailsAsync(
    string connStrName, int compId, int auditId, int reportType, int customerId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(connStrName));
            await connection.OpenAsync();

            var sql = @"
        SELECT
            SAR_ID AS SarId,
            b.Mas_Description AS Role,
            Usr_FullName AS RemarksBy,
            SAR_Remarks AS Remarks,
            ISNULL(FORMAT(SAR_Date, 'dd-MM-yyyy hh:mm:ss tt'), '') AS Date,
            SAR_TimlinetoResOn AS Timeline,
            CASE 
                WHEN SAR_RemarksType = 'C' THEN 'Auditor' 
                WHEN SAR_RemarksType = 'RC' THEN 'Customer' 
            END AS Comments
        FROM StandardAudit_Audit_DRLLog_RemarksHistory
        LEFT JOIN sad_userdetails ON Usr_ID = SAR_RemarksBy

 LEFT JOIN SAD_GrpOrLvl_General_Master b ON b.Mas_ID = Usr_Role
        WHERE SAR_SA_ID = @AuditId

          AND SAR_CompID = @CompId
          AND SAR_SAC_ID = @CustomerId";

            if (reportType != 0)
            {
                sql += " AND SAR_ReportType = @ReportType";
            }

            sql += " ORDER BY SAR_Date DESC";

            var result = await connection.QueryAsync<DrlRemarksHistoryDto>(sql, new
            {
                CompId = compId,
                AuditId = auditId,
                CustomerId = customerId,
                ReportType = reportType
            });

            return result;
        }


        public async Task<int?> GetSACIdAsync(CheckPointIdentifierDto dto)
        {
            const string sql = @"
        SELECT SAC_ID 
        FROM StandardAudit_ScheduleCheckPointList
        WHERE 
            SAC_SA_ID = @AuditId AND 
            SAC_CompID = @CompanyId AND 
            SAC_CheckPointID = @CheckPointId";

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            return await connection.ExecuteScalarAsync<int?>(sql, dto);
        }


        public async Task<bool> UpdateAttachmentDescriptionOnlyAsync(UpdateAttachmentDescriptionDto dto)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            //using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Update only the Edt_Attachments table
                const string updateAttachmentSql = @"
            UPDATE Edt_Attachments
            SET ATCH_Desc = @Description
            WHERE ATCH_DOCID = @DocId";

                var rowsAffected = await connection.ExecuteAsync(updateAttachmentSql, new
                {
                    Description = dto.Description,
                    DocId = dto.DocId
                }, transaction);

                await transaction.CommitAsync();
                return rowsAffected > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public string GetConfigValue(string key)
        {
            // using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))



            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            {
                var query = "SELECT SAD_Config_Value FROM [dbo].[Sad_Config_Settings] WHERE SAD_Config_Key = @Key";
                return connection.QueryFirstOrDefault<string>(query, new { Key = key });
            }
        }

        public async Task<List<int>> SaveAttachmentsAsync(LocalAttachmentDto request)
        {
            var savedAttachmentIds = new List<int>();
            var attachmentId = 0;
            var documentId = 0;


            // ✅ Skip if there are no files to process
            if (request.Files == null || !request.Files.Any())
            {
                return savedAttachmentIds; // Return empty list
            }

            request.AccessCodeDirectory = GetConfigValue("ImgPath");

            foreach (var file in request.Files)
            {
                var tempFolderPath = EnsureDirectoryExists(request.AccessCodeDirectory, request.UserId.ToString(), "Upload");
                //if (file.FileName == "empty.txt")
                //{
                //    file.FileName = "";
                //}


                var originalFileName = Path.GetFileName(file.FileName);
                var tempFilePath = Path.Combine(tempFolderPath, originalFileName);

                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileExtension = Path.GetExtension(originalFileName).TrimStart('.').ToLower();
                var fileBaseName = Path.GetFileNameWithoutExtension(originalFileName).Replace("&", " and");
                fileBaseName = fileBaseName.Substring(0, Math.Min(fileBaseName.Length, 95));

                if (file.FileName == "empty.txt")
                {
                    fileExtension = "";
                    fileBaseName = "";
                }
                var fileSize = new FileInfo(tempFilePath).Length;

                attachmentId = request.AttachmentId == 0 ? GetNextId("ATCH_ID", request.CompanyId) : request.AttachmentId;
                documentId = GetNextId("ATCH_DOCID", request.CompanyId);

                if (documentId == 0 && DocumentIdExists(request.CompanyId, attachmentId))
                {
                    attachmentId = GetNextId("ATCH_ID", request.CompanyId);
                    documentId = GetNextId("ATCH_DOCID", request.CompanyId);
                }

                if (IsFileStoredInDatabase(request.CompanyId))
                {
                    byte[] fileData = await File.ReadAllBytesAsync(tempFilePath);
                    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

                    if (string.IsNullOrEmpty(dbName))
                        throw new Exception("CustomerCode is missing in session. Please log in again.");

                    // ✅ Step 2: Get the connection string
                    var connectionString = _configuration.GetConnectionString(dbName);

                    using var connection = new SqlConnection(connectionString);
                    // using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                    string sql = @"INSERT INTO EDT_ATTACHMENTS 
                          (ATCH_ID, ATCH_DOCID, ATCH_FNAME, ATCH_EXT, ATCH_CREATEDBY, ATCH_MODIFIEDBY, 
                           ATCH_VERSION, ATCH_FLAG, ATCH_OLE, ATCH_SIZE, ATCH_FROM, ATCH_Basename, ATCH_CREATEDON, 
                           ATCH_Status, ATCH_CompID)
                          VALUES (@ATCH_ID, @ATCH_DOCID, @ATCH_FNAME, @ATCH_EXT, @CREATEDBY, @MODIFIEDBY, 1, 0, @ATCH_OLE, 
                          @SIZE, 0, 0, GETDATE(), 'X', @COMPID)";

                    await connection.ExecuteAsync(sql, new
                    {
                        ATCH_ID = attachmentId,
                        ATCH_DOCID = documentId,
                        ATCH_FNAME = fileBaseName,
                        ATCH_EXT = fileExtension,
                        CREATEDBY = request.UserId,
                        MODIFIEDBY = request.UserId,
                        ATCH_OLE = fileData,
                        SIZE = fileSize,
                        COMPID = request.CompanyId
                    });
                }
                else
                {
                    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

                    if (string.IsNullOrEmpty(dbName))
                        throw new Exception("CustomerCode is missing in session. Please log in again.");

                    // ✅ Step 2: Get the connection string
                    var connectionString = _configuration.GetConnectionString(dbName);

                    using var connection = new SqlConnection(connectionString);
                    //using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                    string sql = @"INSERT INTO EDT_ATTACHMENTS 
                          (ATCH_ID, ATCH_DOCID, ATCH_FNAME, ATCH_EXT, ATCH_CREATEDBY, ATCH_MODIFIEDBY, 
                           ATCH_VERSION, ATCH_FLAG, ATCH_SIZE, ATCH_FROM, ATCH_Basename, ATCH_CREATEDON, 
                           ATCH_Status, ATCH_CompID, Atch_Vstatus)
                          VALUES (@ATCH_ID, @ATCH_DOCID, @ATCH_FNAME, @ATCH_EXT, @CREATEDBY, @MODIFIEDBY, 1, 0, 
                                  @SIZE, 0, 0, GETDATE(), 'X', @COMPID, 'A')";

                    await connection.ExecuteAsync(sql, new
                    {
                        ATCH_ID = attachmentId,
                        ATCH_DOCID = documentId,
                        ATCH_FNAME = fileBaseName,
                        ATCH_EXT = fileExtension,
                        CREATEDBY = request.UserId,
                        MODIFIEDBY = request.UserId,
                        SIZE = fileSize,
                        COMPID = request.CompanyId
                    });

                    string finalDirectory = GetOrCreateTargetDirectory(request.AccessCodeDirectory, request.ModuleName, documentId / 301, tempFilePath);
                    string finalFilePath = Path.Combine(finalDirectory, $"{documentId}.{fileExtension}");
                    if (File.Exists(finalFilePath)) File.Delete(finalFilePath);
                    EncryptFile(tempFilePath, finalFilePath);
                    if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
                }

                savedAttachmentIds.Add(attachmentId);
            }

            // ✅ You can still log something even if attachments were skipped
            await SaveDRLLogDetailsAsync(request, attachmentId, documentId);
            return savedAttachmentIds;
        }



        private async Task SaveDRLLogDetailsAsync(LocalAttachmentDto req, int attachmentId, int documentId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            //using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            req.ADRLId = await GetExistingADRLIdAsync(req) ?? 0;
            var AuditName = "";
            const string query = @"
SELECT TOP 1 SA_AuditNo,  SA_ScopeOfAudit 
FROM StandardAudit_Schedule 
WHERE SA_ID = @AuditId";



           // using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var result = await connection.QueryFirstOrDefaultAsync(query, new { AuditId = req.AuditNo });
            if (result != null)
            {
                AuditName = result.SA_AuditNo;

            }


            if (req.SendeMailFlag == 1)
            {
                NewSendAuditLifecycleEmailAsync(req.EmailIds, req.AuditNo, AuditName, req.Comments);
            }
            else if (req.SendeMailFlag == 2)
            {
                DuringSendDuringAuditEmailAsync(req.EmailIds, req.AuditNo, req.ReportType, req.RequestedOn, req.Comments);
            }
            else if (req.SendeMailFlag == 3)
            {
                NewSendAuditLifecycleEmailAsync(req.EmailIds, req.AuditNo, AuditName, req.Comments);
            }

            string Emails = string.Join(",", req.EmailIds);

            // ADRL_ID is input-only (DO NOT use InputOutput since SP doesn't support OUTPUT)
            parameters.Add("@ADRL_ID", req.ADRLId);
            parameters.Add("@ADRL_YearID", req.YearId);
            parameters.Add("@ADRL_AuditNo", req.AuditNo);
            parameters.Add("@ADRL_FunID", 0);
            parameters.Add("@ADRL_CustID", req.CustomerId);
            parameters.Add("@ADRL_RequestedListID", req.RequestedListId);
            parameters.Add("@ADRL_RequestedTypeID", 0);
            parameters.Add("@ADRL_RequestedOn", req.RequestedOn);
            parameters.Add("@ADRL_TimlinetoResOn", req.TimelineToRespondOn);
            parameters.Add("@ADRL_EmailID", Emails);
            parameters.Add("@ADRL_Comments", req.Comments);
            parameters.Add("@ADRL_CrBy", req.UserId);
            parameters.Add("@ADRL_UpdatedBy", req.UserId);
            parameters.Add("@ADRL_IPAddress", req.IPAddress);
            parameters.Add("@ADRL_CompID", req.CompanyId);

            parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("spAudit_DRLLog", parameters, commandType: CommandType.StoredProcedure);

            // ✅ Get ADRL_ID returned through @iOper
            var iOper = parameters.Get<int>("@iOper");

            // Call update using the returned ADRL_ID
            await UpdateReportTypeAsync(
                req.CompanyId,
                iOper, // <-- This is the correct ADRL_ID from SP
                req.ReportType,
                req.Comments ?? string.Empty,
                attachmentId,
                req.AuditNo,
                req.RequestedListId,
                documentId
            );


            var remarkInsertSql = @"
        DECLARE @NewId INT = (SELECT ISNULL(MAX(SAR_ID) + 1, 1) FROM StandardAudit_Audit_DRLLog_RemarksHistory);

        INSERT INTO StandardAudit_Audit_DRLLog_RemarksHistory (
            SAR_ID, SAR_SA_ID, SAR_SAC_ID, SAR_CheckPointIDs, SAR_RemarksType, SAR_Remarks, SAR_RemarksBy, SAR_Date,
            SAR_IPAddress, SAR_CompID, SAR_EmailIds, SAR_TimlinetoResOn, sar_Yearid, SAR_DBFlag,
            SAR_AtthachDocId, SAR_ReportType, SAR_MASid, SAR_AttchId, SAR_DRLId
        )
        VALUES (
            @NewId, @AuditID, @CustID, @CheckPointID, 'C', @Remarks, @UserID, GETDATE(),
            @IPAddress, @CompanyID, @EmailIds, @RespondTime, @YearID, 'A',
            @DocID, @TabType, @MasID, @AttachID, @DRLID
        );
    ";

            await connection.ExecuteAsync(remarkInsertSql, new
            {
                AuditID = req.AuditNo,
                CustID = req.CustomerId,
                CheckPointID = 0,
                Remarks = req.Comments ?? "",
                UserID = req.UserId,
                IPAddress = req.IPAddress,
                CompanyID = req.CompanyId,
                EmailIds = Emails,
                RespondTime = req.TimelineToRespondOn ?? "",
                YearID = req.YearId,
                DocID = documentId,
                TabType = req.ReportType,
                MasID = iOper, // You can replace with actual MasID if needed
                AttachID = attachmentId,
                DRLID = req.RequestedListId
            });
        }
        private async Task<int?> GetExistingADRLIdAsync(LocalAttachmentDto req)
        {
            const string sql = @"
        SELECT TOP 1 ADRL_ID 
        FROM Audit_DRLLog
        WHERE ADRL_AuditNo = @AuditNo
          AND ADRL_FunID = 0
          AND ADRL_CustID = @CustomerId
          AND ADRL_RequestedListID = @RequestedListId
          AND ADRL_RequestedTypeID = 0
          AND ADRL_CompID = @CompanyId
          AND ADRL_YearID = @YearId";

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);

            //using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return await connection.QueryFirstOrDefaultAsync<int?>(sql, new
            {
                req.AuditNo,
                req.CustomerId,
                req.RequestedListId,
                req.CompanyId,
                req.YearId
            });
        }


        public async Task UpdateReportTypeAsync(int companyId, int DRLpkId, int reportType, string comments, int attachmentId, int auditId, int drl_RequestId, int DocId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            //using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // 1. Update Audit_DRLLog
                string sql1 = @"
            UPDATE Audit_DRLLog
            SET ADRL_ReportType = @ReportType,
                ADRL_Comments = @Comments,
                ADRL_AttachID = @ADRL_AttachID
            WHERE ADRL_ID = @PkId AND ADRL_CompID = @CompanyId";

                var parameters1 = new
                {
                    ReportType = reportType,
                    Comments = comments,
                    PkId = DRLpkId,
                    ADRL_AttachID = attachmentId,
                    CompanyId = companyId
                };

                await connection.ExecuteAsync(sql1, parameters1, transaction);

                // 2. Update EDT_Attachments
                string sql2 = @"
            UPDATE EDT_Attachments
            SET ATCH_AuditID = @AuditId,
                ATCH_ReportType = @ReportType,
                ATCH_DRLID = @DRLID
            WHERE ATCH_DOCID = @AttachmentId";

                var parameters2 = new
                {
                    AuditId = auditId,
                    ReportType = reportType,
                    DRLID = drl_RequestId,
                    AttachmentId = DocId
                };

                await connection.ExecuteAsync(sql2, parameters2, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }



        private string EnsureDirectoryExists(string rootPath, string user, string subFolder)
        {
            var path = Path.Combine(rootPath, "Tempfolder", user, subFolder);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        private string GetOrCreateTargetDirectory(string basePath, string module, int folderNumber, string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            string[] imageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tif", ".tiff", ".svg"];
            string[] documentExtensions = [".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".txt", ".csv"];

            string fileType = imageExtensions.Contains(ext) ? "Images"
                             : documentExtensions.Contains(ext) ? "Documents"
                             : "Others";

            var modulePath = Path.Combine(basePath, module, fileType, folderNumber.ToString());
            if (!Directory.Exists(modulePath))
                Directory.CreateDirectory(modulePath);

            return modulePath;
        }

        private int GetNextId(string column, int companyId)
        {
            // using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            string sql = $"SELECT ISNULL(MAX({column}), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_CompID = @CompID";
            return connection.ExecuteScalar<int>(sql, new { CompID = companyId });
        }

        private bool DocumentIdExists(int companyId, int attachId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            // using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string sql = "SELECT 1 FROM EDT_ATTACHMENTS WHERE ATCH_CompID = @CompID AND ATCH_ID = @AttachID";
            return connection.ExecuteScalar<int?>(sql, new { CompID = companyId, AttachID = attachId }) == 1;
        }

        private bool IsFileStoredInDatabase(int companyId)
        {
            //using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            string sql = "SELECT Sad_Config_Value FROM Sad_Config_Settings WHERE Sad_Config_Key = 'FilesInDB' AND Sad_CompID = @CompID";
            var result = connection.ExecuteScalar<string>(sql, new { CompID = companyId });
            return result?.ToUpper() == "TRUE";
        }

        private void EncryptFile(string sourcePath, string targetPath)
        {
            File.Copy(sourcePath, targetPath); // Placeholder
        }
        private async Task NewSendAuditLifecycleEmailAsync(List<string> toEmail, int auditNo, string auditName, string remarks)
        {
            var subject = $"Intimation mail for Nearing completion of the Audit - {auditNo}";

            var body = $@"
<p><strong>Intimation mail</strong></p>

<p><strong>Document Requested</strong></p>

<p>Greetings from TRACe PA.</p>

<p>This mail is an intimation for sharing the documents requested by the Auditor's office.</p>

<p><strong>Audit No.</strong>: {auditNo} - {auditName}</p>

<p><strong>Comments</strong>:</p>
<p>{remarks}</p>

<p>Please login to TRACe PA website using the link and credentials shared with you.</p>

<p><a href='https://tracepacust-user.multimedia.interactivedns.com/'>Click Here</a></p>

<p>Home page of the application will show you the list of documents requested by the auditor. Upload all the requested documents using links provided.</p>

<br/>
<p>Thanks,</p>
<p>TRACe PA Team</p>
";

            using var message = new MailMessage();
            message.From = new MailAddress("harsha.s2700@gmail.com");

            foreach (var email in toEmail)
            {
                if (!string.IsNullOrWhiteSpace(email))
                    message.To.Add(new MailAddress(email));
            }

            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using var smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("harsha.s2700@gmail.com", "edvemvlmgfkcasrp"),
                EnableSsl = true // ✅ IMPORTANT
            };

            await smtpClient.SendMailAsync(message);
        }

        private async Task DuringSendDuringAuditEmailAsync(List<string> EmailIds, int AuditNo, int ReportType, string RequestedOn, string Comments)
        {
            if (EmailIds == null || !EmailIds.Any())
                return;

            var smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("harsha.s2700@gmail.com", "edvemvlmgfkcasrp"),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress("harsha.s2700@gmail.com"),
                Subject = $"Intimation mail for sharing the Documents requested by the Auditor - {AuditNo}",
                IsBodyHtml = true
            };

            mail.To.Add(EmailIds[0]);

            for (int i = 1; i < EmailIds.Count; i++)
            {
                mail.CC.Add(EmailIds[i]);
            }

            //var requestedOn = dto.RequestedOn?.ToString("MMM/dd/yy") ?? "";

            string body = $@"
<p><strong>Intimation mail</strong></p>
<p>Document Requested</p>
<p>Greetings from TRACe PA.</p>
<p>This mail is an intimation for sharing the documents requested by the Auditor's office.</p>

<p><strong>Audit No.:</strong> {AuditNo} - {ReportType} and Date : {RequestedOn}</p>
<p><strong>Document Requested List:</strong> Journal Entries</p>";

            if (!string.IsNullOrWhiteSpace(Comments))
            {
                body += $@"
<p><strong>Specific request for client:</strong></p>
<p>{Comments}</p>";
            }

            body += @"
<br />
<p>Please login to TRACe PA website using the link and credentials shared with you.</p>
<p><a href='https://tracepacust-user.multimedia.interactivedns.com/'>TRACe PA Portal</a></p>
<p>Home page of the application will show you the list of documents requested by the auditor. Upload all the requested documents using links provided.</p>
<br />
<p>Thanks,</p>
<p>TRACe PA Team</p>";

            mail.Body = body;

            await smtpClient.SendMailAsync(mail);
        }
        public async Task<string> GetHttpsDocumentPathModulewiseAsync(GetDocumentPathRequestDto dto)
        {
            if (dto.AttachDocId == 0) return string.Empty;

            string fileDownloadUrl = string.Empty;
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);

            //using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            string sql = @"
        SELECT ATCH_DocId, ATCH_FNAME, atch_ext, atch_ole 
        FROM EDT_ATTACHMENTS 
        WHERE ATCH_CompID = @CompanyId AND ATCH_ID = @AttachId AND ATCH_DOCID = @AttachDocId";

            using var reader = await connection.ExecuteReaderAsync(sql, new
            {
                dto.CompanyId,
                dto.AttachId,
                dto.AttachDocId
            });

            if (!reader.HasRows) return string.Empty;

            while (await reader.ReadAsync())
            {
                string fileName = reader["ATCH_FNAME"].ToString();
                string ext = reader["atch_ext"].ToString();
                int docId = Convert.ToInt32(reader["ATCH_DocId"]);

                string accessCodeDir = GetConfigValue("ImgPath");
                string downloadDir = Path.Combine(accessCodeDir, "Tempfolder", dto.UserId, "Download");
                Directory.CreateDirectory(downloadDir);

                string downloadFilePath = Path.Combine(downloadDir, $"{fileName}.{ext}");
                if (File.Exists(downloadFilePath))
                    File.Delete(downloadFilePath);

                // Get FilesInDB setting
                string filesInDb = connection.ExecuteScalar<string>(
                    @"SELECT Sad_Config_Value FROM Sad_Config_Settings 
              WHERE Sad_Config_Key = 'FilesInDB' AND Sad_CompID = @CompanyId",
                    new { dto.CompanyId })?.ToUpper();

                if (filesInDb == "TRUE")
                {
                    byte[] buffer = (byte[])reader["atch_ole"];
                    await File.WriteAllBytesAsync(downloadFilePath, buffer);
                }
                else
                {
                    string folder = (docId / 301).ToString();
                    string encryptedDir = CheckOrCreateFileIsImageOrDocumentDirectory(accessCodeDir, dto.Module, folder, downloadFilePath);
                    string encryptedFilePath = Path.Combine(encryptedDir, $"{docId}.{ext}");

                    if (File.Exists(encryptedFilePath))
                    {
                        try
                        {
                            Decrypt(encryptedFilePath, downloadFilePath);
                        }
                        catch
                        {
                            File.Copy(encryptedFilePath, downloadFilePath, true);
                        }
                    }
                }

                if (File.Exists(downloadFilePath))
                {
                    string displayPath = connection.ExecuteScalar<string>(
                        @"SELECT Sad_Config_Value FROM Sad_Config_Settings 
                  WHERE Sad_Config_Key = 'DisplayPath' AND Sad_CompID = @CompanyId",
                        new { dto.CompanyId });

                    fileDownloadUrl = $"{displayPath.TrimEnd('/')}/Tempfolder/{dto.UserId}/Download/{fileName}.{ext}";
                }
            }

            return fileDownloadUrl;
        }


        public string CheckOrCreateFileIsImageOrDocumentDirectory(string accessCodeDirectory, string module, string folder, string filePath)
        {
            if (string.IsNullOrEmpty(module)) return string.Empty;

            string fileExtension = Path.GetExtension(filePath)?.ToLower();
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tif", ".tiff", ".svg", ".psd", ".ai", ".eps", ".ico", ".webp", ".raw", ".heic", ".heif", ".exr", ".dng", ".jp2", ".j2k", ".cr2", ".nef", ".orf", ".arw", ".raf", ".rw2", ".mp4", ".avi", ".mov", ".wmv", ".mkv", ".flv", ".webm", ".m4v", ".mpg", ".mpeg", ".3gp", ".ts", ".m2ts", ".vob", ".mts", ".divx", ".ogv" };
            string[] documentExtensions = { ".pdf", ".doc", ".docx", ".txt", ".xls", ".xlsx", ".ppt", ".ppsx", ".pptx", ".odt", ".ods", ".odp", ".rtf", ".csv", ".pptm", ".xlsm", ".docm", ".xml", ".json", ".yaml", ".key", ".numbers", ".pages", ".tar", ".zip", ".rar" };

            string fileType = imageExtensions.Contains(fileExtension) ? "Images" :
                              documentExtensions.Contains(fileExtension) ? "Documents" : "Others";

            string modulePath = Path.Combine(accessCodeDirectory, module);
            if (!Directory.Exists(modulePath)) Directory.CreateDirectory(modulePath);

            string finalPath = Path.Combine(modulePath, fileType, folder);
            if (!Directory.Exists(finalPath)) Directory.CreateDirectory(finalPath);

            return finalPath;
        }

        public void Decrypt(string inputFilePath, string outputFilePath)
        {
            string encryptionKey = "MAKV2SPBNI99212";

            using (Aes aes = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] {
                0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D,
                0x65, 0x64, 0x76, 0x65, 0x64, 0x65,
                0x76
            });

                aes.Key = pdb.GetBytes(32);
                aes.IV = pdb.GetBytes(16);

                using FileStream fsInput = new FileStream(inputFilePath, FileMode.Open);
                using CryptoStream cs = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read);
                using FileStream fsOutput = new FileStream(outputFilePath, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                {
                    fsOutput.WriteByte((byte)data);
                }
            }
        }
    }
}

