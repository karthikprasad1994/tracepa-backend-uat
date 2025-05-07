using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TracePca.Data;
using TracePca.Dto;
using TracePca.Dto.Audit;
using TracePca.Interface.Audit;

namespace TracePca.Service.Communication_with_client
{
    public class Communication: AuditInterface
    {
        private readonly IConfiguration _configuration;



        public Communication(IConfiguration configuration)
        {

            _configuration = configuration;
        }

        public async Task<IEnumerable<Dto.Audit.CustomerDto>> GetCustomerLoeAsync(int companyId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"SELECT LOE_ID as CustomerID, LOE_Name as CustomerName
                     FROM SAD_CUST_LOE
                     WHERE LOE_CustomerId = @CustomerId";

            return await connection.QueryAsync<Dto.Audit.CustomerDto>(query, new { CompanyId = companyId });
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
        SELECT RTM_Id, RTM_ReportTypeName, @AudRptType AS AuditNo
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

        public  async Task<string> GetUserFullNameAsync(string connectionStringName, int companyId, int userId)
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


        public async Task<List<AttachmentDto>> LoadAttachmentsAsync(string connectionStringName, int companyId, int attachId, string dateFormat)
        {
            var connectionString = _configuration.GetConnectionString(connectionStringName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT 
            Atch_DocID,
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
            AND ATCH_Status <> 'D' 
            AND Atch_Vstatus IN ('A', 'AS', 'C')
        ORDER BY ATCH_CREATEDON";

            var rawData = (await connection.QueryAsync(query, new
            {
                CompanyId = companyId,
                AttachId = attachId,
                DateFormat = dateFormat
            })).ToList();

            var result = new List<AttachmentDto>();
            int index = 1;

            foreach (var row in rawData)
            {
                result.Add(new AttachmentDto
                {
                    SrNo = index++,
                    AtchID = row.Atch_DocID,
                    FName = $"{row.ATCH_FNAME}.{row.ATCH_EXT}",
                    FDescription = row.ATCH_Desc ?? "",
                    CreatedById = row.ATCH_CreatedBy,
                    CreatedBy = await GetUserFullNameAsync(connectionStringName, companyId, row.ATCH_CreatedBy), // Assumed existing helper
                    CreatedOn = row.ATCH_CREATEDON,
                    FileSize = $"{(Convert.ToDouble(row.ATCH_SIZE) / 1024):0.00} KB",
                    Extention = row.ATCH_EXT,
                    Type = row.ATCH_ReportType,
                    Status = row.Atch_Vstatus
                });
            }

            return result;
        }

        private async Task SaveAuditDocumentAsync(AddFileDto dto, int attachId, IFormFile file)
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

                // ✅ Get max existing ATCH_ID
                var maxIdQuery = "SELECT ISNULL(MAX(ATCH_ID), 0) + 1 FROM Edt_Attachments";
                var nextAttachId = await connection.ExecuteScalarAsync<int>(maxIdQuery);

                var insertQuery = @"
INSERT INTO Edt_Attachments (
    ATCH_ID, ATCH_DOCID, ATCH_FNAME, ATCH_EXT, ATCH_Desc, ATCH_SIZE,
    ATCH_AuditID, ATCH_AUDScheduleID, ATCH_CREATEDBY, ATCH_CREATEDON,
    ATCH_COMPID, ATCH_ReportType, ATCH_drlid
)
VALUES (
    @AtchId, @DocId, @FileName, @FileExt, @Description, @Size,
    @AuditId, @ScheduleId, @UserId, GETDATE(),
    @CompId, @ReportType, @DrlId
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
                    DrlId = dto.DrlId
                });
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

                // 3. Save into Audit_Document
                await SaveAuditDocumentAsync(dto, attachId, dto.File);

                // 4. Get latest DocId for this customer and audit
                int docId = await GetLatestDocIdAsync(dto.CustomerId, dto.AuditId);

                // 5. Generate next Remark ID
                int remarkId = await GenerateNextRemarkIdAsync(dto.CustomerId, dto.AuditId);

                // 6. Insert into Audit_DocRemarksLog
                await InsertIntoAuditDocRemarksLogAsync(dto, requestedId, remarkId, attachId);

                // 7. Get DRL_PKID for updating
                int pkId = await GetDrlPkIdAsync(dto.CustomerId, dto.AuditId, attachId);

                // 8. Update DRL_AttachID and DRL_DocID in Audit_DocRemarksLog
                await UpdateDrlAttachIdAndDocIdAsync(dto.CustomerId, dto.AuditId, attachId, docId, pkId);

                return "Attachment uploaded and details saved successfully.";
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


        private async Task InsertIntoAuditDocRemarksLogAsync(AddFileDto dto, int requestedId, int remarkId, int attachId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(
                    @"INSERT INTO StandardAudit_Audit_DRLLog_RemarksHistory (
                SAR_ID, SAR_SAC_ID, SAR_SA_ID, SAR_DocRequestedListID,
                SAR_Remarks, SAR_RemarksDate, SAR_UploadedBy, SAR_RemarkType, SAR_AttachmentID
            ) VALUES (
                @RemarkId, @CustomerId, @AuditId, @RequestedId,
                @Remark, GETDATE(), @UserId, @Type, @AttachId
            )",
                    new
                    {
                        RemarkId = remarkId,
                        CustomerId = dto.CustomerId,
                        AuditId = dto.AuditId,
                        RequestedId = requestedId,
                        Remark = dto.Remark,
                        UserId = dto.UserId,
                        //Type = dto.Type,
                        AttachId = attachId
                    });
            }
        }


        private async Task<int> GetDrlPkIdAsync(int customerId, int auditId, int attachId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                return await connection.ExecuteScalarAsync<int>(
                    @"SELECT SAR_PKID 
              FROM StandardAudit_Audit_DRLLog_RemarksHistory 
              WHERE SAR_SAC_ID = @CustomerId AND SAR_SA_ID = @AuditId AND SAR_AttachmentID = @AttachId",
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
              SET SAR_AttachmentID = @AttachId, SAR_DocID = @DocId
              WHERE SAR_SAC_ID = @CustomerId AND SAR_SA_ID = @AuditId AND SAR_PKID = @PkId",
                    new { AttachId = attachId, DocId = docId, CustomerId = customerId, AuditId = auditId, PkId = pkId });
            }
        }





    }

}
    
