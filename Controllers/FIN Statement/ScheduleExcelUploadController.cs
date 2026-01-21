using Dapper;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.ScheduleExcelUploadDto;
using static TracePca.Service.FIN_statement.ScheduleExcelUploadService;
using static TracePca.Service.FIN_statement.ScheduleMappingService;

using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using Microsoft.Identity.Client;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleExcelUploadController : ControllerBase
    {
        private readonly ScheduleExcelUploadInterface _ScheduleExcelUploadService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private ScheduleMappingInterface _ScheduleMappingService;

        public ScheduleExcelUploadController(ScheduleExcelUploadInterface ScheduleExcelUploadInterface, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ScheduleMappingInterface scheduleMappingService)
        {
            _ScheduleExcelUploadService = ScheduleExcelUploadInterface;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _ScheduleMappingService = scheduleMappingService;
        }

        //DownloadUploadableExcelAndTemplate
        [HttpGet("DownloadableExcelFile")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public IActionResult DownloadExcelTemplate()
        {
            var result = _ScheduleExcelUploadService.GetExcelTemplate();

            if (result.FileBytes == null)
                return NotFound("File not found.");

            return File(result.FileBytes, result.ContentType, result.FileName);
        }


        //SaveScheduleTemplate(P and L)
        [HttpPost("SaveScheduleTemplate(P and L)")]
        public async Task<IActionResult> SaveSchedulePandL([FromQuery] int CompId, [FromBody] List<ScheduleTemplatePandLDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "No schedule data provided."
                });
            }

            try
            {
                var resultIds = await _ScheduleExcelUploadService.SaveSchedulePandLAsync(CompId, dtos);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Schedule data saved successfully.",
                    Data = resultIds
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving schedule data.",
                    Error = ex.Message
                });
            }
        }

        //SaveScheduleTemplate(BalanceSheet)
        [HttpPost("SaveScheduleTemplate(BalnceSheet)")]
        public async Task<IActionResult> SaveScheduleBalanceSheet([FromQuery] int CompId, [FromBody] List<ScheduleTemplateBalanceSheetDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "No schedule data provided."
                });
            }

            try
            {
                var resultIds = await _ScheduleExcelUploadService.SaveScheduleBalanceSheetAsync(CompId, dtos);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Schedule data saved successfully.",
                    Data = resultIds
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving schedule data.",
                    Error = ex.Message
                });
            }
        }

        //SaveScheduleTemplate
        [HttpPost("SaveScheduleTemplate")]
        public async Task<IActionResult> SaveScheduleTemplate([FromQuery] int CompId, [FromBody] List<ScheduleTemplateDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "No schedule data provided."
                });
            }

            try
            {
                var resultIds = await _ScheduleExcelUploadService.SaveScheduleTemplateAsync(CompId, dtos);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Schedule data saved successfully.",
                    Data = resultIds
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving schedule data.",
                    Error = ex.Message
                });
            }
        }

        //SaveOpeningBalance
        [HttpPost("SaveOpeningBalance")]
        public async Task<IActionResult> SaveOpeningBalance([FromQuery] int CompId, [FromBody] List<OpeningBalanceDto> dtos)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "No trail balance data received.",
                        data = (object)null
                    });
                }

                var resultIds = await _ScheduleExcelUploadService.SaveOpeningBalanceAsync(CompId, dtos);

                return Ok(new
                {
                    statusCode = 200,
                    message = "Trail balance data uploaded successfully.",
                    data = resultIds
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while uploading trail balance data.",
                    error = ex.Message
                });
            }
        }

        //SaveTrailBalance
        [HttpPost("SaveTrailBalance")]
        public async Task<IActionResult> SaveTrailBalanceDetails([FromQuery] int CompId, [FromBody] List<TrailBalanceDto> HeaderDtos)
        {
            try
            {
                var resultIds = await _ScheduleExcelUploadService.SaveTrailBalanceDetailsAsync(CompId, HeaderDtos);
                return Ok(new
                {
                    status = 200,
                    message = "Trail Balance uploaded successfully.",
                    data = new
                    {
                        ResultIds = resultIds
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while uploading trial balance.",
                    error = ex.Message
                });
            }
        }

        //SaveClientTrailBalance
        [HttpPost("SaveClientTrailBalance")]
        public async Task<IActionResult> UploadClientTrailBalance([FromQuery] int CompId, [FromBody] List<ClientTrailBalance> items)
        {
            if (items == null || !items.Any())
                return BadRequest("No data provided.");
            try
            {
                var result = await _ScheduleExcelUploadService.ClientTrailBalanceAsync(CompId, items);
                return Ok(new
                {
                    Success = true,
                    Message = "Client Trail Balance uploaded successfully.",
                    SavedIds = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while uploading data.",
                    Details = ex.Message
                });
            }
        }

        [HttpPost("uploadJE")]
        public async Task<IActionResult> UploadTrailBalance(
       int compId,
       IFormFile excelFile,
       string sheetName)
        {
            if (excelFile == null || excelFile.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                var resultIds = await _ScheduleExcelUploadService.SaveCompleteTrailBalanceAsync(
                    compId,
                    null, // models will be filled from Excel inside service
                    excelFile,
                    sheetName
                );

                return Ok(new { Success = true, SavedIds = resultIds });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Success = false, Message = ex.Message });
            }
        }

        //UploadCustomerTrialBalance
        [HttpPost("UploadCustomerTrialBalanceExcel")]
        public async Task<IActionResult> UploadCustomerTrialBalanceExcel(
     IFormFile file,
     [FromQuery] int customerId,
     [FromQuery] int yearId,
     [FromQuery] int branchId,
     [FromQuery] int quarterId,
     [FromQuery] int companyId,
     [FromQuery] int userId)
        {
            // -------- FILE VALIDATION --------
            if (file == null || file.Length == 0)
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "No file uploaded."
                });
            }

            try
            {
                var result = await _ScheduleExcelUploadService.UploadCustomerTrialBalanceExcelAsync(
                    file,
                    customerId,
                    yearId,
                    branchId,
                    quarterId,
                    companyId,
                    userId
                );

                if (result != "Successfully Upload")
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = result
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Trial Balance Excel uploaded successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    statusCode = 404,
                    message = "Error processing Trial Balance Excel",
                    data = new List<string> { ex.Message }
                });
            }
        }

        public class JournalEntryUploadRequest
        {
            public int CustomerId { get; set; }
            public int FinancialYearId { get; set; }
            public int BranchId { get; set; }
            public int DurationId { get; set; }
            public List<JournalEntryRow> Rows { get; set; }
        }

        public class JournalEntryRow
        {
            public string JE_Type { get; set; }
            public string Bill_No { get; set; }
            public string Transaction_Date { get; set; }
            public string Account { get; set; }
            public decimal? Debit { get; set; }
            public decimal? Credit { get; set; }
            public string Narration { get; set; }
            public string Comments { get; set; }
        }

        public class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
            public string Error { get; set; }
        }








        [HttpPost("upload-je-transactions")]
        public async Task<ActionResult<ApiResponse<string>>> UploadJournalEntries(
       [FromBody] JournalEntryUploadRequest request)
        {
            try
            {
                string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

                if (string.IsNullOrEmpty(dbName))
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "CustomerCode is missing in session. Please log in again."
                    });

                var connectionString = _configuration.GetConnectionString(dbName);

                // Headers
                var accessCodeId = int.Parse(Request.Headers["AccessCodeID"].FirstOrDefault() ?? "1");
                var userId = int.Parse(Request.Headers["UserID"].FirstOrDefault() ?? "0");
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

                // Basic validation
                if (request.CustomerId <= 0)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Select Client" });

                if (request.FinancialYearId <= 0)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Select FinancialYear" });

                if (request.BranchId <= 0)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Select Branch" });

                if (request.DurationId == 0)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Select Duration" });

                if (request.Rows == null || request.Rows.Count == 0)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "No data to upload" });

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                /* ----------------------------------------------------
                 * 1️⃣ FAST HEADER / MASTER VALIDATION
                 * ---------------------------------------------------- */
                var validationResult = await ValidateUploadData(
                    request, connection, null, accessCodeId);

                if (!validationResult.Success)
                    return BadRequest(validationResult);

                /* ----------------------------------------------------
                 * 2️⃣ ROW-LEVEL PRE VALIDATION (NEW)
                 * ---------------------------------------------------- */
                var preValidationResult = await PreValidateRows(
                    connection, null, request, accessCodeId, userId, ipAddress);

                if (!preValidationResult.Success)
                    return BadRequest(preValidationResult);

                /* ----------------------------------------------------
                 * 3️⃣ TRANSACTIONAL INSERT
                 * ---------------------------------------------------- */
                using var transaction = connection.BeginTransaction();
                try
                {
                    var processResult = await ProcessJournalEntriesOptimized(
                        connection, transaction, request, accessCodeId, userId, ipAddress);

                    if (!processResult.Success)
                    {
                        transaction.Rollback();
                        return BadRequest(processResult);
                    }

                    transaction.Commit();

                    return Ok(new ApiResponse<string>
                    {
                        Success = true,
                        Message = $"Successfully uploaded {request.Rows.Count} records"
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(500, new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Upload failed",
                        Error = ex.Message
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Internal server error",
                    Error = ex.Message
                });
            }
        }


        private async Task<ApiResponse<string>> ProcessJournalEntriesOptimized(
            SqlConnection connection,
            SqlTransaction transaction,
            JournalEntryUploadRequest request,
            int accessCodeId,
            int userId,
            string ipAddress)
        {
            // Step 1: Filter and prepare data
            var validRows = request.Rows
                .Where(r => !string.IsNullOrWhiteSpace(r.Account))
                .ToList();

            if (!validRows.Any())
            {
                return new ApiResponse<string> { Success = true, Message = "No valid rows to process" };
            }

            // Step 2: Bulk fetch existing accounts
            var distinctAccounts = validRows
                .Select(r => r.Account.Trim())
                .Distinct()
                .ToList();

            var existingAccounts = await BulkFetchAccounts(
                connection, transaction, distinctAccounts, accessCodeId,
                request.CustomerId, request.FinancialYearId, request.BranchId, request.DurationId);

            // Step 3: Create missing accounts
            var missingAccounts = distinctAccounts
                .Where(acc => !existingAccounts.ContainsKey(acc))
                .ToList();

            if (missingAccounts.Any())
            {
                await CreateMissingAccounts(
          connection: connection,
          transaction: transaction,
          missingAccounts: missingAccounts,
          accessCodeId: accessCodeId,
          request: request,
          validRows: validRows, // ✅ Pass the validRows parameter
          userId: userId,
          ipAddress: ipAddress
      );

                // Re-fetch newly created accounts
                var newAccounts = await BulkFetchAccounts(
                    connection, transaction, missingAccounts, accessCodeId,
                    request.CustomerId, request.FinancialYearId, request.BranchId, request.DurationId);

                foreach (var kvp in newAccounts)
                {
                    existingAccounts[kvp.Key] = kvp.Value;
                }
            }

            // Step 4: Process transactions using SqlBulkCopy
            var success = await ProcessTransactionsWithBulkCopy(
                connection, transaction, validRows, existingAccounts,
                request, accessCodeId, userId, ipAddress);

            return new ApiResponse<string> { Success = success };
        }

        private async Task<Dictionary<string, int>> BulkFetchAccounts(
            SqlConnection connection,
            SqlTransaction transaction,
            List<string> accounts,
            int compId,
            int customerId,
            int yearId,
            int branchId,
            int durationId)
        {
            var result = new Dictionary<string, int>();

            // Process in batches to avoid parameter limits
            const int batchSize = 1000;
            var batches = accounts
                .Select((acc, index) => new { acc, index })
                .GroupBy(x => x.index / batchSize)
                .Select(g => g.Select(x => x.acc).ToList())
                .ToList();

            foreach (var batch in batches)
            {
                // Build parameterized query for this batch
                var parameters = new List<SqlParameter>();
                var paramNames = new List<string>();

                for (int i = 0; i < batch.Count; i++)
                {
                    var paramName = $"@Acc{i}";
                    paramNames.Add(paramName);
                    parameters.Add(new SqlParameter(paramName, batch[i]));
                }

                var sql = $@"
            SELECT ATBU_Description, ATBU_ID 
            FROM Acc_TrailBalance_Upload 
            WHERE ATBU_Description IN ({string.Join(",", paramNames)})
              AND ATBU_CustId = @CustId 
              AND ATBU_Branchid = @BranchId 
              AND ATBU_CompId = @CompId 
              AND ATBU_YEARId = @YearId
              AND ATBU_QuarterId = @QuarterId";

                using var command = new SqlCommand(sql, connection, transaction);

                // Add batch parameters
                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }

                command.Parameters.AddWithValue("@CustId", customerId);
                command.Parameters.AddWithValue("@BranchId", branchId);
                command.Parameters.AddWithValue("@CompId", compId);
                command.Parameters.AddWithValue("@YearId", yearId);
                command.Parameters.AddWithValue("@QuarterId", durationId);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result[reader.GetString(0)] = reader.GetInt32(1);
                }
            }

            return result;
        }
        private async Task<int> CheckVoucherType(SqlConnection connection, SqlTransaction transaction, int compId, string type, string description)
        {
            var sql = "SELECT cmm_ID FROM Content_Management_Master WHERE CMM_CompID = @CompID AND cmm_Category = @Category AND cmm_Delflag = 'A' AND cmm_Desc = @Description ORDER BY cmm_Desc ASC";

            using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@CompID", compId);
            command.Parameters.AddWithValue("@Category", type);
            command.Parameters.AddWithValue("@Description", description);

            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }
        private async Task<ApiResponse<string>> PreValidateRows(SqlConnection connection, SqlTransaction transaction, JournalEntryUploadRequest request, int accessCodeId, int userId, string ipAddress)
        {
            var iErrorLine = 1;

            // ✅ OPTIMIZATION: Pre-load voucher types in batch
            var distinctVoucherTypes = request.Rows
                .Where(row => !string.IsNullOrEmpty(row.JE_Type))
                .Select(row => row.JE_Type)
                .Distinct()
                .ToList();

            var voucherTypes = new Dictionary<string, int>();
            foreach (var voucherType in distinctVoucherTypes)
            {
                var voucherId = await CheckVoucherType(connection, transaction, accessCodeId, "JE", voucherType);
                voucherTypes[voucherType] = voucherId;
            }

            // Validate each row
            foreach (var row in request.Rows)
            {
                if (string.IsNullOrEmpty(row.JE_Type))
                {
                    if (!string.IsNullOrEmpty(row.Account))
                    {
                        return new ApiResponse<string> { Success = false, Message = $"JE Type cannot be blank - Line No: {iErrorLine}" };
                    }
                }
                else if (!voucherTypes.ContainsKey(row.JE_Type) || voucherTypes[row.JE_Type] == 0)
                {
                    return new ApiResponse<string> { Success = false, Message = $"Create JE Type {row.JE_Type} in the General Master" };
                }

                if (string.IsNullOrEmpty(row.Account))
                {
                    if (row.Debit.HasValue || row.Credit.HasValue)
                    {
                        return new ApiResponse<string> { Success = false, Message = $"Account cannot be blank - Line No: {iErrorLine}" };
                    }
                }

                // Validate date format
                if (!string.IsNullOrEmpty(row.Transaction_Date) && !IsValidDate(row.Transaction_Date))
                {
                    return new ApiResponse<string> { Success = false, Message = $"Invalid Date Format (Enter Transaction Date in dd/MM/yyyy format) - Line No: {iErrorLine}" };
                }

                iErrorLine++;
            }

            return new ApiResponse<string> { Success = true };
        }
        private async Task<int> GetIdFromNameAsync(SqlConnection conn, SqlTransaction tran, string table, string column, string name, int orgType)
        {
            string nameCol = column.Replace("_ID", "_Name");
            string orgCol = column.Replace("_ID", "_Orgtype");

            var query = $"SELECT ISNULL({column}, 0) FROM {table} WHERE {nameCol} = @name AND {orgCol} = @orgType";
            using var cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@name", name.Trim());
            cmd.Parameters.AddWithValue("@orgType", orgType);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0);
        }

        private async Task<DataTable> GetScheduleIDs(SqlConnection conn, SqlTransaction tran, int Id, int orgType, int LevelId)
        {
            string query = string.Empty;
            int idValue = 0;
            if (LevelId == 4)
            {

                query = @"SELECT AST_HeadingID, a.ASH_ID, a.ASH_Name, ISNULL(AST_Schedule_type,0) AS AST_Schedule_type,
                      ISNULL(b.ASSH_ID,0) AS ASSH_ID, ISNULL(b.ASSH_Name,'') AS ASSH_Name, 
                      ISNULL(c.ASI_ID,0) AS ASI_ID, ISNULL(c.ASI_Name,'') AS ASI_Name,
                      ISNULL(d.ASSI_ID,0) AS ASSi_ID, ISNULL(d.ASSI_Name,'') AS ASSI_Name 
               FROM ACC_ScheduleTemplates 
               LEFT JOIN ACC_ScheduleSubItems d ON d.ASSI_ID = AST_SubItemID
               LEFT JOIN ACC_ScheduleItems c ON c.ASI_ID = AST_ItemID
               LEFT JOIN ACC_ScheduleSubHeading b ON b.ASSH_ID = AST_SubHeadingID
               LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = AST_HeadingID
               WHERE AST_SubItemID = @ID AND AST_Companytype = @OrgType";
            }
            else if (LevelId == 3)
            {
                query = @"SELECT AST_HeadingID, a.ASH_ID, a.ASH_Name, ISNULL(AST_Schedule_type,0) AS AST_Schedule_type,
                      ISNULL(b.ASSH_ID,0) AS ASSH_ID, ISNULL(b.ASSH_Name,'') AS ASSH_Name, 
                      ISNULL(c.ASI_ID,0) AS ASI_ID, ISNULL(c.ASI_Name,'') AS ASI_Name,
                      0 AS ASSi_ID, '' AS ASSI_Name 
               FROM ACC_ScheduleTemplates 
               LEFT JOIN ACC_ScheduleSubItems d ON d.ASSI_ID = AST_SubItemID
               LEFT JOIN ACC_ScheduleItems c ON c.ASI_ID = AST_ItemID
               LEFT JOIN ACC_ScheduleSubHeading b ON b.ASSH_ID = AST_SubHeadingID
               LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = AST_HeadingID
               WHERE AST_ItemID = @ID AND AST_Companytype = @OrgType AND AST_SubItemID = 0";
            }
            else if (LevelId == 2)
            {
                query = @"SELECT AST_HeadingID, a.ASH_ID, a.ASH_Name, ISNULL(AST_Schedule_type,0) AS AST_Schedule_type,
                      ISNULL(b.ASSH_ID,0) AS ASSH_ID, ISNULL(b.ASSH_Name,'') AS ASSH_Name,
                      0 AS ASI_ID, '' AS ASI_Name,
                      0 AS ASSi_ID, '' AS ASSI_Name 
               FROM ACC_ScheduleTemplates 
               LEFT JOIN ACC_ScheduleSubItems d ON d.ASSI_ID = AST_SubItemID
               LEFT JOIN ACC_ScheduleItems c ON c.ASI_ID = AST_ItemID
               LEFT JOIN ACC_ScheduleSubHeading b ON b.ASSH_ID = AST_SubHeadingID
               LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = AST_HeadingID
               WHERE AST_SubHeadingID = @ID AND AST_Companytype = @OrgType";
            }
            else if (LevelId == 1)
            {
                query = @"SELECT AST_HeadingID, a.ASH_ID, a.ASH_Name, ISNULL(AST_Schedule_type,0) AS AST_Schedule_type,
                      0 AS ASSH_ID, '' AS ASSH_Name,
                      0 AS ASI_ID, '' AS ASI_Name,
                      0 AS ASSi_ID, '' AS ASSI_Name 
               FROM ACC_ScheduleTemplates 
               LEFT JOIN ACC_ScheduleSubItems d ON d.ASSI_ID = AST_SubItemID
               LEFT JOIN ACC_ScheduleItems c ON c.ASI_ID = AST_ItemID
               LEFT JOIN ACC_ScheduleSubHeading b ON b.ASSH_ID = AST_SubHeadingID
               LEFT JOIN ACC_ScheduleHeading a ON a.ASH_ID = AST_HeadingID
               WHERE AST_HeadingID = @ID AND AST_Companytype = @OrgType";
            }
            else
            {
                return new DataTable(); // return empty if nothing is matched
            }

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                // Determine which ID to pass     
                cmd.Parameters.AddWithValue("@ID", Id);
                cmd.Parameters.AddWithValue("@OrgType", orgType);

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable result = new DataTable();
                    await Task.Run(() => adapter.Fill(result));
                    return result;
                }
            }
        }
        private async Task<int> GetScheduleTypeFromTemplateAsync(SqlConnection conn, SqlTransaction tran, int subItemId, int itemId, int subHeadingId, int headingId, int orgType)
        {
            string query = "";

            if (subItemId > 0)
                query = "SELECT ISNULL(AST_Schedule_type, 0) FROM ACC_ScheduleTemplates WHERE AST_SubItemID = @id AND AST_Companytype = @orgType";
            else if (itemId > 0)
                query = "SELECT ISNULL(AST_Schedule_type, 0) FROM ACC_ScheduleTemplates WHERE AST_ItemID = @id AND AST_SubItemID = 0 AND AST_Companytype = @orgType";
            else if (subHeadingId > 0)
                query = "SELECT ISNULL(AST_Schedule_type, 0) FROM ACC_ScheduleTemplates WHERE AST_SubHeadingID = @id AND AST_Companytype = @orgType";
            else if (headingId > 0)
                query = "SELECT ISNULL(AST_Schedule_type, 0) FROM ACC_ScheduleTemplates WHERE AST_HeadingID = @id AND AST_Companytype = @orgType";
            else
                return 0;

            using var cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@id", subItemId > 0 ? subItemId : itemId > 0 ? itemId : subHeadingId > 0 ? subHeadingId : headingId);
            cmd.Parameters.AddWithValue("@orgType", orgType);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0);
        }
        private async Task<int> ReserveAtbuIds(
    SqlConnection connection,
    SqlTransaction transaction)
        {
            var cmd = new SqlCommand(@"
        DECLARE @StartId INT;

        SELECT @StartId = ISNULL(MAX(ATBU_ID), 0) + 1
        FROM Acc_TrailBalance_Upload WITH (TABLOCKX);

        SELECT @StartId;
    ", connection, transaction);

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        private async Task<int> ReserveAtbudIds(
            SqlConnection connection,
            SqlTransaction transaction)
        {
            var cmd = new SqlCommand(@"
        DECLARE @StartId INT;

        SELECT @StartId = ISNULL(MAX(ATBUD_ID), 0) + 1
        FROM Acc_TrailBalance_Upload_Details WITH (TABLOCKX);

        SELECT @StartId;
    ", connection, transaction);

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        private async Task CreateMissingAccounts(
         SqlConnection connection,
         SqlTransaction transaction,
         List<string> missingAccounts,
         int accessCodeId,
         JournalEntryUploadRequest request,
         List<JournalEntryRow> validRows,
         int userId,
         string ipAddress)
        {
            // 1️⃣ Reserve ID blocks (SAFE)
            int startAtbuId = await ReserveAtbuIds(connection, transaction);
            int startAtbudId = await ReserveAtbudIds(connection, transaction);

            int startCount = await GetAccountCount(
                connection, transaction,
                accessCodeId,
                request.CustomerId,
                request.FinancialYearId,
                request.BranchId,
                request.DurationId);

            // 2️⃣ Schedule detection
            bool hasPurchase = validRows.Any(r =>
                r.JE_Type?.Equals("Purchase", StringComparison.OrdinalIgnoreCase) == true);

            bool hasSales = validRows.Any(r =>
                r.JE_Type?.Equals("Sales", StringComparison.OrdinalIgnoreCase) == true);

            int pHeadingId = 0, pSubHeadingId = 0, pItemId = 0, pScheduleType = 0;
            int sHeadingId = 0, sSubHeadingId = 0, sItemId = 0, sScheduleType = 0;

            if (hasPurchase)
            {
                pItemId = await GetIdFromNameAsync(
                    connection, transaction,
                    "ACC_ScheduleItems", "ASI_ID",
                    "Others - Trade Payables",
                    request.CustomerId);

                var dt = await GetScheduleIDs(connection, transaction, pItemId, request.CustomerId, 3);
                if (dt.Rows.Count > 0)
                {
                    pHeadingId = Convert.ToInt32(dt.Rows[0]["ASH_ID"]);
                    pSubHeadingId = Convert.ToInt32(dt.Rows[0]["ASSH_ID"]);
                    pItemId = Convert.ToInt32(dt.Rows[0]["ASI_ID"]);
                }

                pScheduleType = await GetScheduleTypeFromTemplateAsync(
                    connection, transaction,
                    0, pItemId, pSubHeadingId, pHeadingId, request.CustomerId);
            }

            if (hasSales)
            {
                sItemId = await GetIdFromNameAsync(
                    connection, transaction,
                    "ACC_ScheduleItems", "ASI_ID",
                    "Any others - Trade receivables",
                    request.CustomerId);

                var dt = await GetScheduleIDs(connection, transaction, sItemId, request.CustomerId, 3);
                if (dt.Rows.Count > 0)
                {
                    sHeadingId = Convert.ToInt32(dt.Rows[0]["ASH_ID"]);
                    sSubHeadingId = Convert.ToInt32(dt.Rows[0]["ASSH_ID"]);
                    sItemId = Convert.ToInt32(dt.Rows[0]["ASI_ID"]);
                }

                sScheduleType = await GetScheduleTypeFromTemplateAsync(
                    connection, transaction,
                    0, sItemId, sSubHeadingId, sHeadingId, request.CustomerId);
            }

            // 3️⃣ DataTables
            var masterTable = new DataTable();
            masterTable.Columns.Add("ATBU_ID", typeof(int));
            masterTable.Columns.Add("ATBU_CODE", typeof(string));
            masterTable.Columns.Add("ATBU_Description", typeof(string));
            masterTable.Columns.Add("ATBU_CustId", typeof(int));
            masterTable.Columns.Add("ATBU_Opening_Debit_Amount", typeof(decimal));
            masterTable.Columns.Add("ATBU_Opening_Credit_Amount", typeof(decimal));
            masterTable.Columns.Add("ATBU_TR_Debit_Amount", typeof(decimal));
            masterTable.Columns.Add("ATBU_TR_Credit_Amount", typeof(decimal));
            masterTable.Columns.Add("ATBU_Closing_Debit_Amount", typeof(decimal));
            masterTable.Columns.Add("ATBU_Closing_Credit_Amount", typeof(decimal));
            masterTable.Columns.Add("ATBU_DELFLG", typeof(string));
            masterTable.Columns.Add("ATBU_CRBY", typeof(int));
            masterTable.Columns.Add("ATBU_STATUS", typeof(string));
            masterTable.Columns.Add("ATBU_UPDATEDBY", typeof(int));
            masterTable.Columns.Add("ATBU_IPAddress", typeof(string));
            masterTable.Columns.Add("ATBU_CompId", typeof(int));
            masterTable.Columns.Add("ATBU_YEARId", typeof(int));
            masterTable.Columns.Add("ATBU_Branchid", typeof(int));
            masterTable.Columns.Add("ATBU_QuarterId", typeof(int));

            var detailTable = new DataTable();

            detailTable.Columns.Add("ATBUD_ID", typeof(int));
            detailTable.Columns.Add("ATBUD_Masid", typeof(int));
            detailTable.Columns.Add("ATBUD_CODE", typeof(string));
            detailTable.Columns.Add("ATBUD_Description", typeof(string));
            detailTable.Columns.Add("ATBUD_CustId", typeof(int));
            detailTable.Columns.Add("ATBUD_SChedule_Type", typeof(int));
            detailTable.Columns.Add("ATBUD_Company_Type", typeof(int));
            detailTable.Columns.Add("ATBUD_Headingid", typeof(int));
            detailTable.Columns.Add("ATBUD_Subheading", typeof(int));
            detailTable.Columns.Add("ATBUD_itemid", typeof(int));
            detailTable.Columns.Add("ATBUD_SubItemId", typeof(int));
            detailTable.Columns.Add("ATBUD_DELFLG", typeof(string));
            detailTable.Columns.Add("ATBUD_CRBY", typeof(int));
            detailTable.Columns.Add("ATBUD_STATUS", typeof(string));
            detailTable.Columns.Add("ATBUD_Progress", typeof(string));
            detailTable.Columns.Add("ATBUD_UPDATEDBY", typeof(int));
            detailTable.Columns.Add("ATBUD_IPAddress", typeof(string));
            detailTable.Columns.Add("ATBUD_CompId", typeof(int));
            detailTable.Columns.Add("ATBUD_YEARId", typeof(int));
            detailTable.Columns.Add("Atbud_Branchnameid", typeof(int));
            detailTable.Columns.Add("ATBUd_QuarterId", typeof(int));


            // 4️⃣ Populate rows (SAFE INCREMENT)
            for (int i = 0; i < missingAccounts.Count; i++)
            {
                int newAtbuId = startAtbuId + i;
                int newAtbudId = startAtbudId + i;

                string code = $"SCh00{startCount + i + 1}";
                string account = missingAccounts[i];

                bool isPurchase = validRows.Any(r =>
                    r.Account == account &&
                    r.JE_Type?.Equals("Purchase", StringComparison.OrdinalIgnoreCase) == true);

                bool isSales = validRows.Any(r =>
                    r.Account == account &&
                    r.JE_Type?.Equals("Sales", StringComparison.OrdinalIgnoreCase) == true);

                int headingId = isPurchase ? pHeadingId : isSales ? sHeadingId : 0;
                int subHeadingId = isPurchase ? pSubHeadingId : isSales ? sSubHeadingId : 0;
                int itemId = isPurchase ? pItemId : isSales ? sItemId : 0;
                int scheduleType = isPurchase ? pScheduleType : isSales ? sScheduleType : 0;

                masterTable.Rows.Add(
                    newAtbuId, code, account, request.CustomerId,
                    0m, 0m, 0m, 0m, 0m, 0m,
                    "A", userId, "C", userId, ipAddress,
                    accessCodeId, request.FinancialYearId,
                    request.BranchId, request.DurationId);

                detailTable.Rows.Add(
    newAtbudId,          // ATBUD_ID
    newAtbuId,           // ATBUD_Masid
    code,                // ATBUD_CODE
    account,             // ATBUD_Description
    request.CustomerId,  // ATBUD_CustId
    scheduleType,        // ATBUD_SChedule_Type
    request.CustomerId,  // ATBUD_Company_Type
    headingId,           // ATBUD_Headingid
    subHeadingId,        // ATBUD_Subheading
    itemId,              // ATBUD_itemid
    0,                   // ATBUD_SubItemId
    "A",                 // ATBUD_DELFLG
    userId,              // ATBUD_CRBY
    "C",                 // ATBUD_STATUS
    "Uploaded",          // ATBUD_Progress
    userId,              // ATBUD_UPDATEDBY
    ipAddress,           // ATBUD_IPAddress
    accessCodeId,        // ATBUD_CompId
    request.FinancialYearId, // ATBUD_YEARId
    request.BranchId,    // Atbud_Branchnameid
    request.DurationId   // ATBUd_QuarterId
);

            }

            // 5️⃣ Bulk copy
            using (var bulkMaster = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
            {
                bulkMaster.DestinationTableName = "Acc_TrailBalance_Upload";
                foreach (DataColumn col in masterTable.Columns)
                    bulkMaster.ColumnMappings.Add(col.ColumnName, col.ColumnName);

                await bulkMaster.WriteToServerAsync(masterTable);
            }

            using (var bulkDetail = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
            {
                bulkDetail.DestinationTableName = "Acc_TrailBalance_Upload_Details";
                foreach (DataColumn col in detailTable.Columns)
                    bulkDetail.ColumnMappings.Add(col.ColumnName, col.ColumnName);

                await bulkDetail.WriteToServerAsync(detailTable);
            }
        }


        private async Task<int> GetMaxDetailAccountId(
    SqlConnection connection,
    SqlTransaction transaction,
    int compId,
    int custId,
    int yearId,
    int branchId,
    int quarterId)
        {
            var cmd = new SqlCommand(@"
        SELECT ISNULL(MAX(ATBUD_ID), 0)
        FROM Acc_TrailBalance_Upload_Details
        WHERE ATBUD_CompId = @CompId ",
                connection, transaction);

            cmd.Parameters.AddWithValue("@CompId", compId);

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }



        private async Task<int> GetMaxAccountId(
            SqlConnection connection,
            SqlTransaction transaction,
            int compId,
            int customerId,
            int yearId,
            int branchId,
            int durationId)
        {
            var sql = "SELECT ISNULL(MAX(ATBU_ID), 0) FROM Acc_TrailBalance_Upload WHERE ATBU_CustId = @CustId AND ATBU_CompId = @CompId";

            using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@CustId", customerId);
            command.Parameters.AddWithValue("@CompId", compId);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        private async Task<int> GetAccountCount(
            SqlConnection connection,
            SqlTransaction transaction,
            int compId,
            int customerId,
            int yearId,
            int branchId,
            int durationId)
        {
            var sql = "SELECT COUNT(*) FROM Acc_TrailBalance_Upload WHERE ATBU_CustId = @CustId AND ATBU_CompId = @CompId AND ATBU_YEARId = @YearId AND ATBU_BranchId = @BranchId AND ATBU_QuarterId = @QuarterId";

            using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@CustId", customerId);
            command.Parameters.AddWithValue("@CompId", compId);
            command.Parameters.AddWithValue("@YearId", yearId);
            command.Parameters.AddWithValue("@BranchId", branchId);
            command.Parameters.AddWithValue("@QuarterId", durationId);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        private async Task<bool> ProcessTransactionsWithBulkCopy(
     SqlConnection connection,
     SqlTransaction transaction,
     List<JournalEntryRow> validRows,
     Dictionary<string, int> accountIds,
     JournalEntryUploadRequest request,
     int accessCodeId,
     int userId,
     string ipAddress)
        {
            try
            {
                DateTime sqlMin = new DateTime(1753, 1, 1);
                DateTime SafeDate(DateTime d) => d < sqlMin ? sqlMin : d;

                int maxJeId = await GetMaxJournalEntryId(
                    connection, transaction,
                    accessCodeId,
                    request.FinancialYearId,
                    request.BranchId,
                    request.DurationId);

                int maxAjtbId = await GetMaxJournalEntryDetailId(
                    connection, transaction,
                    accessCodeId,
                    request.FinancialYearId,
                    request.BranchId,
                    request.DurationId);

                // ================= BILL TYPE CACHE =================
                var billTypeCache = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                // Load all bill types upfront
                var billTypes = await GetAllBillTypes(connection, transaction, accessCodeId);
                foreach (var bt in billTypes)
                {
                    billTypeCache[bt.Name.Trim()] = bt.Id;
                }

                // ================= BUILD VOUCHERS =================
                var voucherGroups = new List<VoucherGroup>();
                VoucherGroup currentVoucher = null;

                foreach (var row in validRows)
                {
                    DateTime? rowDate = null;
                    if (!string.IsNullOrWhiteSpace(row.Transaction_Date))
                        rowDate = ParseTransactionDate(row.Transaction_Date);

                    if (IsNewVoucherStart(row))
                    {
                        if (currentVoucher?.Rows.Count > 0)
                            voucherGroups.Add(currentVoucher);

                        int billTypeId = 0;
                        if (!string.IsNullOrWhiteSpace(row.JE_Type) &&
                            billTypeCache.TryGetValue(row.JE_Type, out int cachedId))
                        {
                            billTypeId = cachedId;
                        }
                        else if (!string.IsNullOrWhiteSpace(row.JE_Type))
                        {
                            // Only query database if not in cache (should rarely happen)
                            billTypeId = (int)await GetBillTypeId(connection, transaction, accessCodeId, row.JE_Type);
                            billTypeCache[row.JE_Type] = billTypeId;
                        }

                        currentVoucher = new VoucherGroup
                        {
                            VoucherDate = rowDate,
                            BillTypeId = billTypeId,
                            BillNo = row.Bill_No?.Trim(),
                            Rows = new List<VoucherRow>()
                        };
                    }

                    currentVoucher.Rows.Add(new VoucherRow
                    {
                        OriginalRow = row,
                        EffectiveDate = rowDate,
                        HasOriginalDate = rowDate.HasValue
                    });
                }

                if (currentVoucher?.Rows.Count > 0)
                    voucherGroups.Add(currentVoucher);

                // ================= MASTER TABLE =================
                var vouchersWithDates = voucherGroups.Where(v => v.VoucherDate.HasValue).ToList();
                var transactionNumbers = GenerateTransactionNumbers(vouchersWithDates.Count, request);

                var masterData = new DataTable();
                masterData.BeginLoadData();

                masterData.Columns.Add("Acc_JE_ID", typeof(int));
                masterData.Columns.Add("Acc_JE_TransactionNo", typeof(string));
                masterData.Columns.Add("Acc_JE_Party", typeof(int));
                masterData.Columns.Add("acc_JE_BranchId", typeof(int));
                masterData.Columns.Add("Acc_JE_Location", typeof(int));
                masterData.Columns.Add("Acc_JE_BillType", typeof(int));
                masterData.Columns.Add("Acc_JE_BillNo", typeof(string));
                masterData.Columns.Add("Acc_JE_BillDate", typeof(DateTime));
                masterData.Columns.Add("Acc_JE_BillAmount", typeof(decimal));
                masterData.Columns.Add("Acc_JE_CreatedBy", typeof(int));
                masterData.Columns.Add("Acc_JE_YearID", typeof(int));
                masterData.Columns.Add("Acc_JE_CompID", typeof(int));
                masterData.Columns.Add("Acc_JE_Status", typeof(string));
                masterData.Columns.Add("Acc_JE_Operation", typeof(string));
                masterData.Columns.Add("Acc_JE_IPAddress", typeof(string));
                masterData.Columns.Add("acc_JE_QuarterId", typeof(int));

                var voucherMasterMap = new Dictionary<VoucherGroup, (string TransNo, int JeId, DateTime VoucherDate)>();
                var jeDetUpdates = new List<(int CompId, int YearId, int AccountId, int CustId, int BranchId,
                    decimal Debit, decimal Credit, int DurationId)>();

                for (int i = 0; i < vouchersWithDates.Count; i++)
                {
                    var v = vouchersWithDates[i];
                    int jeId = maxJeId + i + 1;
                    string transNo = transactionNumbers[i];

                    voucherMasterMap[v] = (transNo, jeId, v.VoucherDate.Value);

                    decimal total = 0;
                    foreach (var r in v.Rows)
                        total += (r.OriginalRow.Debit ?? 0) + (r.OriginalRow.Credit ?? 0);

                    masterData.Rows.Add(
                        jeId,
                        transNo,
                        request.CustomerId,
                        request.BranchId,
                        3,
                        v.BillTypeId,
                        v.BillNo ?? "",
                        SafeDate(v.VoucherDate.Value),
                        total,
                        userId,
                        request.FinancialYearId,
                        accessCodeId,
                        "A",
                        "C",
                        ipAddress,
                        request.DurationId
                    );
                }

                masterData.EndLoadData();

                using (var bulkMaster = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkMaster.DestinationTableName = "Acc_JE_Master";
                    bulkMaster.BatchSize = 5000;
                    bulkMaster.EnableStreaming = true;
                    bulkMaster.BulkCopyTimeout = 0;

                    foreach (DataColumn c in masterData.Columns)
                        bulkMaster.ColumnMappings.Add(c.ColumnName, c.ColumnName);

                    await bulkMaster.WriteToServerAsync(masterData);
                }

                // ================= DETAIL TABLE =================
                var detailData = new DataTable();
                detailData.BeginLoadData();

                detailData.Columns.Add("AJTB_ID", typeof(int));
                detailData.Columns.Add("AJTB_CustId", typeof(int));
                detailData.Columns.Add("AJTB_BranchId", typeof(int));
                detailData.Columns.Add("AJTB_Deschead", typeof(int));
                detailData.Columns.Add("AJTB_Desc", typeof(int));
                detailData.Columns.Add("AJTB_Debit", typeof(decimal));
                detailData.Columns.Add("AJTB_Credit", typeof(decimal));
                detailData.Columns.Add("AJTB_CreatedBy", typeof(int));
                detailData.Columns.Add("AJTB_CreatedOn", typeof(DateTime));
                detailData.Columns.Add("AJTB_Status", typeof(string));
                detailData.Columns.Add("AJTB_IPAddress", typeof(string));
                detailData.Columns.Add("AJTB_CompID", typeof(int));
                detailData.Columns.Add("AJTB_YearID", typeof(int));
                detailData.Columns.Add("AJTB_Operation", typeof(string));
                detailData.Columns.Add("AJTB_TranscNo", typeof(string));
                detailData.Columns.Add("Ajtb_Masid", typeof(int));
                detailData.Columns.Add("AJTB_QuarterId", typeof(int));
                detailData.Columns.Add("AJTB_BillType", typeof(int));
                detailData.Columns.Add("AJTB_DescName", typeof(string));

                int counter = 0;
                (string TransNo, int JeId, DateTime VoucherDate) lastMasterInfo = default;

                foreach (var voucher in voucherGroups)
                {
                    if (voucherMasterMap.TryGetValue(voucher, out var info))
                        lastMasterInfo = info;

                    foreach (var row in voucher.Rows)
                    {
                        if (string.IsNullOrWhiteSpace(row.OriginalRow.Account))
                            continue;

                        if (!accountIds.TryGetValue(row.OriginalRow.Account.Trim(), out int accId))
                            continue;

                        counter++;

                        DateTime finalDate =
                            row.EffectiveDate
                            ?? (DateTime?)lastMasterInfo.VoucherDate
                            ?? DateTime.Today;

                        decimal debit = row.OriginalRow.Debit ?? 0;
                        decimal credit = row.OriginalRow.Credit ?? 0;

                        detailData.Rows.Add(
                            maxAjtbId + counter,
                            request.CustomerId,
                            request.BranchId,
                            accId,
                            accId,
                            debit,
                            credit,
                            userId,
                            SafeDate(finalDate),
                            "A",
                            ipAddress,
                            accessCodeId,
                            request.FinancialYearId,
                            "C",
                            lastMasterInfo.TransNo,
                            lastMasterInfo.JeId,
                            request.DurationId,
                            voucher.BillTypeId,
                            row.OriginalRow.Account.Trim()
                        );

                        // Collect updates for batch processing instead of calling immediately
                        jeDetUpdates.Add((
                            accessCodeId,
                            request.FinancialYearId,
                            accId,
                            request.CustomerId,
                            request.BranchId,
                            debit,
                            credit,
                            request.DurationId
                        ));
                    }
                }

                detailData.EndLoadData();

                using (var bulkDetail = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkDetail.DestinationTableName = "Acc_JETransactions_Details";
                    bulkDetail.BatchSize = 5000;
                    bulkDetail.EnableStreaming = true;
                    bulkDetail.BulkCopyTimeout = 0;

                    foreach (DataColumn c in detailData.Columns)
                        bulkDetail.ColumnMappings.Add(c.ColumnName, c.ColumnName);

                    await bulkDetail.WriteToServerAsync(detailData);
                }

                // ================= BATCH PROCESS JE DET UPDATES =================
                await BatchUpdateJeDet(connection, transaction, jeDetUpdates);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private async Task BatchUpdateJeDet(
          SqlConnection connection,
          SqlTransaction transaction,
          List<(int CompId, int YearId, int AccountId, int CustId, int BranchId,
              decimal Debit, decimal Credit, int DurationId)> updates)
        {
            if (updates.Count == 0)
                return;

            // Group by account using a dictionary instead of anonymous types
            var updateDict = new Dictionary<(int, int, int, int, int), (decimal, decimal)>();

            foreach (var update in updates)
            {
                var key = (update.AccountId, update.CustId, update.CompId, update.DurationId, update.BranchId);

                if (!updateDict.TryGetValue(key, out var current))
                {
                    current = (0, 0);
                }

                updateDict[key] = (current.Item1 + update.Debit, current.Item2 + update.Credit);
            }

            // Convert to a list of tuples for processing
            var aggregatedList = new List<(int AccountId, int CustId, int CompId, int DurationId, int BranchId,
                decimal TotalDebit, decimal TotalCredit)>();

            foreach (var kvp in updateDict)
            {
                aggregatedList.Add((
                    kvp.Key.Item1,  // AccountId
                    kvp.Key.Item2,  // CustId
                    kvp.Key.Item3,  // CompId
                    kvp.Key.Item4,  // DurationId
                    kvp.Key.Item5,  // BranchId
                    kvp.Value.Item1, // TotalDebit
                    kvp.Value.Item2  // TotalCredit
                ));
            }

            // Process in batches to avoid parameter limit
            // Each update uses 7 parameters, limit is 2100, so max 2100/7 = 300 per batch
            const int batchSize = 200; // Using 200 for safety margin

            for (int i = 0; i < aggregatedList.Count; i += batchSize)
            {
                var batch = aggregatedList.Skip(i).Take(batchSize).ToList();
                await ExecuteBatchUpdateTuples(connection, transaction, batch);
            }
        }

        private async Task ExecuteBatchUpdateTuples(
            SqlConnection connection,
            SqlTransaction transaction,
            List<(int AccountId, int CustId, int CompId, int DurationId, int BranchId,
                decimal TotalDebit, decimal TotalCredit)> batch)
        {
            if (batch.Count == 0)
                return;

            // Build dynamic SQL for the batch
            var sqlBuilder = new StringBuilder();
            var parameters = new List<SqlParameter>();
            int paramIndex = 0;

            foreach (var update in batch)
            {
                // Create unique parameter names
                string idParam = $"@id{paramIndex}";
                string custIdParam = $"@cust{paramIndex}";
                string compIdParam = $"@comp{paramIndex}";
                string quarterIdParam = $"@quarter{paramIndex}";
                string branchIdParam = $"@branch{paramIndex}";
                string debitParam = $"@debit{paramIndex}";
                string creditParam = $"@credit{paramIndex}";

                // Add parameters
                parameters.Add(new SqlParameter(idParam, update.AccountId));
                parameters.Add(new SqlParameter(custIdParam, update.CustId));
                parameters.Add(new SqlParameter(compIdParam, update.CompId));
                parameters.Add(new SqlParameter(quarterIdParam, update.DurationId));
                parameters.Add(new SqlParameter(branchIdParam, update.BranchId));
                parameters.Add(new SqlParameter(debitParam, update.TotalDebit));
                parameters.Add(new SqlParameter(creditParam, update.TotalCredit));

                // Build branch condition
                string branchCondition = update.BranchId != 0
                    ? $" AND ATBU_Branchid = {branchIdParam}"
                    : "";

                // Add UPDATE statement
                sqlBuilder.AppendLine($@"
            UPDATE Acc_TrailBalance_Upload 
            SET ATBU_Closing_TotalDebit_Amount = ISNULL(ATBU_Closing_TotalDebit_Amount, 0) + {debitParam},
                ATBU_Closing_TotalCredit_Amount = ISNULL(ATBU_Closing_TotalCredit_Amount, 0) + {creditParam}
            WHERE ATBU_ID = {idParam} 
              AND ATBU_CustId = {custIdParam} 
              AND ATBU_CompID = {compIdParam} 
              AND ATBU_QuarterId = {quarterIdParam}
              {branchCondition};");

                paramIndex++;
            }

            // Execute the batch
            if (sqlBuilder.Length > 0)
            {
                using var cmd = new SqlCommand(sqlBuilder.ToString(), connection, transaction);
                cmd.Parameters.AddRange(parameters.ToArray());
                await cmd.ExecuteNonQueryAsync();
            }
        }
        // Add this helper method to get all bill types at once
        private async Task<List<(int Id, string Name)>> GetAllBillTypes(
            SqlConnection connection,
            SqlTransaction transaction,
            int accessCodeId)
        {
            var billTypes = new List<(int, string)>();
            string sql = "SELECT cmm_ID, cmm_Desc FROM Content_Management_Master WHERE CMM_CompID = @CompId";

            using var cmd = new SqlCommand(sql, connection, transaction);
            cmd.Parameters.AddWithValue("@CompId", accessCodeId);
 
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                billTypes.Add((
                    reader.GetInt32(0),
                    reader.GetString(1)
                ));
            }

            return billTypes;
        }

        // Keep the original UpdateJeDet method unchanged (for backward compatibility)
        private async Task UpdateJeDet(SqlConnection connection, SqlTransaction transaction, int compId, int yearId, int id, int custId, int transId, double transAmt, int branchId, double transDbAmt, double transCrAmt, int durationId)
        {
            // Keep the original implementation unchanged
            var sql = @"SELECT * FROM Acc_TrailBalance_Upload 
        WHERE ATBU_ID = @ID AND ATBU_CustId = @CustId 
        AND ATBU_CompID = @CompId AND ATBU_QuarterId = @QuarterId";

            if (branchId != 0)
                sql += " AND ATBU_Branchid = @BranchId";

            using var selectCommand = new SqlCommand(sql, connection, transaction);
            selectCommand.Parameters.AddWithValue("@ID", id);
            selectCommand.Parameters.AddWithValue("@CustId", custId);
            selectCommand.Parameters.AddWithValue("@CompId", compId);
            selectCommand.Parameters.AddWithValue("@QuarterId", durationId);

            if (branchId != 0)
                selectCommand.Parameters.AddWithValue("@BranchId", branchId);

            using var reader = await selectCommand.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                double currentDebit = reader["ATBU_Closing_TotalDebit_Amount"] != DBNull.Value ? Convert.ToDouble(reader["ATBU_Closing_TotalDebit_Amount"]) : 0;
                double currentCredit = reader["ATBU_Closing_TotalCredit_Amount"] != DBNull.Value ? Convert.ToDouble(reader["ATBU_Closing_TotalCredit_Amount"]) : 0;

                await reader.CloseAsync();

                string updateSql;
                double amount;

                if (transId == 0)
                {
                    if (currentDebit != 0)
                    {
                        transDbAmt = currentDebit + transDbAmt;
                        amount = transDbAmt;
                        updateSql = transDbAmt >= 0
                            ? "UPDATE Acc_TrailBalance_Upload SET ATBU_Closing_TotalDebit_Amount = @Amount WHERE ATBU_ID = @ID AND ATBU_CustId = @CustId AND ATBU_CompID = @CompId"
                            : "UPDATE Acc_TrailBalance_Upload SET ATBU_Closing_TotalCredit_Amount = @Amount WHERE ATBU_ID = @ID AND ATBU_CustId = @CustId AND ATBU_CompID = @CompId";
                    }
                    else if (currentCredit != 0)
                    {
                        transDbAmt = currentCredit - transDbAmt;
                        amount = transDbAmt;
                        if (transDbAmt >= 0)
                        {
                            updateSql = "UPDATE Acc_TrailBalance_Upload SET ATBU_Closing_TotalCredit_Amount = @Amount WHERE ATBU_ID = @ID AND ATBU_CustId = @CustId AND ATBU_CompID = @CompId";
                        }
                        else
                        {
                            amount = Math.Abs(transDbAmt);
                            updateSql = "UPDATE Acc_TrailBalance_Upload SET ATBU_Closing_TotalDebit_Amount = @Amount, ATBU_Closing_TotalCredit_Amount = 0.00 WHERE ATBU_ID = @ID AND ATBU_CustId = @CustId AND ATBU_CompID = @CompId";
                        }
                    }
                    else
                    {
                        transDbAmt = currentDebit + transDbAmt;
                        amount = transDbAmt;
                        updateSql = transDbAmt >= 0
                            ? "UPDATE Acc_TrailBalance_Upload SET ATBU_Closing_TotalDebit_Amount = @Amount WHERE ATBU_ID = @ID AND ATBU_CustId = @CustId AND ATBU_CompID = @CompId"
                            : "UPDATE Acc_TrailBalance_Upload SET ATBU_Closing_TotalCredit_Amount = @Amount WHERE ATBU_ID = @ID AND ATBU_CustId = @CustId AND ATBU_CompID = @CompId";
                    }
                }
                else
                {
                    if (currentCredit != 0)
                    {
                        transCrAmt = currentCredit + transCrAmt;
                        amount = transCrAmt;
                        updateSql = transCrAmt >= 0
                            ? "UPDATE Acc_TrailBalance_Upload SET ATBU_Closing_TotalCredit_Amount = @Amount WHERE ATBU_ID = @ID AND ATBU_CustId = @CustId AND ATBU_CompID = @CompId"
                            : "UPDATE Acc_TrailBalance_Upload SET ATBU_Closing_TotalDebit_Amount = @Amount WHERE ATBU_ID = @ID AND ATBU_CustId = @CustId AND ATBU_CompID = @CompId";
                    }
                    else if (currentDebit != 0)
                    {
                        transCrAmt = currentDebit - transCrAmt;
                        amount = transCrAmt;
                        if (transCrAmt >= 0)
                        {
                            updateSql = "UPDATE Acc_TrailBalance_Upload SET ATBU_Closing_TotalDebit_Amount = @Amount WHERE ATBU_ID = @ID AND ATBU_CustId = @CustId AND ATBU_CompID = @CompId";
                        }
                        else
                        {
                            amount = Math.Abs(transCrAmt);
                            updateSql = "UPDATE Acc_TrailBalance_Upload SET ATBU_Closing_TotalCredit_Amount = @Amount, ATBU_Closing_TotalDebit_Amount = 0.00 WHERE ATBU_ID = @ID AND ATBU_CustId = @CustId AND ATBU_CompID = @CompId";
                        }
                    }
                    else
                    {
                        transCrAmt = currentCredit + transCrAmt;
                        amount = transCrAmt;
                        updateSql = transCrAmt >= 0
                            ? "UPDATE Acc_TrailBalance_Upload SET ATBU_Closing_TotalCredit_Amount = @Amount WHERE ATBU_ID = @ID AND ATBU_CustId = @CustId AND ATBU_CompID = @CompId"
                            : "UPDATE Acc_TrailBalance_Upload SET ATBU_Closing_TotalDebit_Amount = @Amount WHERE ATBU_ID = @ID AND ATBU_CustId = @CustId AND ATBU_CompID = @CompId";
                    }
                }

                using var updateCommand = new SqlCommand(updateSql, connection, transaction);
                updateCommand.Parameters.AddWithValue("@Amount", amount);
                updateCommand.Parameters.AddWithValue("@ID", id);
                updateCommand.Parameters.AddWithValue("@CustId", custId);
                updateCommand.Parameters.AddWithValue("@CompId", compId);

                await updateCommand.ExecuteNonQueryAsync();
            }
            else
            {
                await reader.CloseAsync();
            }
        }


        // Helper method to get BillTypeId from Content_Management_Master
        private async Task<int?> GetBillTypeId(SqlConnection connection, SqlTransaction transaction, int compId, string jeType)
        {
            if (string.IsNullOrWhiteSpace(jeType))
                return null;

            string query = @"
        SELECT cmm_ID
        FROM Content_Management_Master
        WHERE CMM_CompID = @CompID
          AND cmm_Category = 'JE'
          AND cmm_Delflag = 'A'
          AND cmm_Desc = @JEType";

            using var cmd = new SqlCommand(query, connection, transaction);
            cmd.Parameters.AddWithValue("@CompID", compId);
            cmd.Parameters.AddWithValue("@JEType", jeType);

            var result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : (int?)null;
        }


        // ================= HELPER METHODS AND CLASSES =================

        private bool IsNewVoucherStart(JournalEntryRow row)
        {
            // A new voucher starts when:

            // 1. Row has both Vch Type and Vch No
            if (!string.IsNullOrWhiteSpace(row.JE_Type) &&
                !string.IsNullOrWhiteSpace(row.Bill_No))
                return true;

            // 2. Row has a date AND (debit or credit amount)
            if (!string.IsNullOrWhiteSpace(row.Transaction_Date) &&
                (row.Debit.HasValue || row.Credit.HasValue))
                return true;

            // 3. Row has account name AND (debit or credit amount)
            if (!string.IsNullOrWhiteSpace(row.Account) &&
                (row.Debit.HasValue || row.Credit.HasValue))
                return true;

            return false;
        }

 
        public class VoucherGroup
        {
            public DateTime? VoucherDate { get; set; }
            public int BillTypeId { get; set; }
            public string BillNo { get; set; }
            public List<VoucherRow> Rows { get; set; }
        }

        public class VoucherRow
        {
            public JournalEntryRow OriginalRow { get; set; }
            public DateTime? EffectiveDate { get; set; }
            public bool HasOriginalDate { get; set; }
        }


        // Updated GetTransactionDateKey method
        private string GetTransactionDateKey(JournalEntryRow row)
        {
            if (string.IsNullOrWhiteSpace(row.Transaction_Date))
                return "NO_DATE";

            var date = ParseTransactionDate(row.Transaction_Date);
            return date.ToString("yyyyMMdd");
        }

        // Method to parse transaction date (handle different formats)
        private DateTime ParseTransactionDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return DateTime.Today;

            // Try parsing common formats
            if (DateTime.TryParseExact(dateString, "dd-MMM-yy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                return result;

            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                return result;

            if (DateTime.TryParse(dateString, out result))
                return result;

            return DateTime.Today;
        }

        // Helper class
        //private class VoucherGroup
        //{
        //    public string VoucherKey { get; set; }
        //    public List<JournalEntryRow> Rows { get; set; }
        //    public DateTime? VoucherDate { get; set; }
        //}


        private async Task<int> GetMaxJournalEntryId(
    SqlConnection con, SqlTransaction tx,
    int compId, int yearId, int branchId, int quarterId)
        {
            var cmd = new SqlCommand(@"
        SELECT ISNULL(MAX(Acc_JE_ID),0)
        FROM Acc_JE_Master
        WHERE Acc_JE_CompID=@CompId ",
                con, tx);

            cmd.Parameters.AddWithValue("@CompId", compId);

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        private async Task<int> GetMaxJournalEntryDetailId(
            SqlConnection con, SqlTransaction tx,
            int compId, int yearId, int branchId, int quarterId)
        {
            var cmd = new SqlCommand(@"
        SELECT ISNULL(MAX(AJTB_ID),0)
        FROM Acc_JETransactions_Details
        WHERE AJTB_CompID=@CompId ",
                con, tx);

            cmd.Parameters.AddWithValue("@CompId", compId);

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }



        private List<string> GenerateTransactionNumbers(int count, JournalEntryUploadRequest request)
        {
            var result = new List<string>();

            // Get last transaction number from database or start from 1
            int startNumber = 1; // You should fetch this from DB
            for (int i = 0; i < count; i++)
            {
                result.Add($"TR{startNumber + i:D3}");
            }

            return result;
        }

        //private string GetTransactionDateKey(JournalEntryRow row)
        //{
        //    if (string.IsNullOrWhiteSpace(row.Transaction_Date))
        //        return "NO_DATE";

        //    var date = ParseTransactionDate(row.Transaction_Date);
        //    return date.ToString("yyyyMMdd");
        //}

        private async Task<ApiResponse<string>> ValidateUploadData(
            JournalEntryUploadRequest request,
            SqlConnection connection,
            SqlTransaction transaction,
            int accessCodeId)
        {
            // Basic validation
            var errorLines = new List<int>();

            for (int i = 0; i < request.Rows.Count; i++)
            {
                var row = request.Rows[i];

                // Check if account is provided when amount exists
                if (string.IsNullOrWhiteSpace(row.Account) && (row.Debit.HasValue || row.Credit.HasValue))
                {
                    errorLines.Add(i + 1);
                    if (errorLines.Count >= 10) break;
                }

                // Validate date format
                if (!string.IsNullOrWhiteSpace(row.Transaction_Date) && !IsValidDate(row.Transaction_Date))
                {
                    errorLines.Add(i + 1);
                    if (errorLines.Count >= 10) break;
                }
            }

            if (errorLines.Any())
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Validation failed at lines: {string.Join(", ", errorLines)}"
                };
            }

            return new ApiResponse<string> { Success = true };
        }

        // ✅ OPTIMIZED: Bulk voucher type checking
        private async Task<Dictionary<string, int>> BulkCheckVoucherTypes(
            SqlConnection connection,
            SqlTransaction transaction,
            int compId,
            string type,
            List<string> descriptions)
        {
            if (!descriptions.Any()) return new Dictionary<string, int>();

            var result = new Dictionary<string, int>();

            using var command = new SqlCommand(@"
        SELECT cmm_Desc, cmm_ID 
        FROM Content_Management_Master 
        WHERE CMM_CompID = @CompID 
          AND cmm_Category = @Category 
          AND cmm_Delflag = 'A'
          AND cmm_Desc IN (" + string.Join(",", descriptions.Select((_, i) => $"@Desc{i}")) + ")",
                connection, transaction);

            command.Parameters.AddWithValue("@CompID", compId);
            command.Parameters.AddWithValue("@Category", type);

            for (int i = 0; i < descriptions.Count; i++)
            {
                command.Parameters.AddWithValue($"@Desc{i}", descriptions[i]);
            }

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result[reader.GetString(0)] = reader.GetInt32(1);
            }

            return result;
        }

        // ✅ OPTIMIZED: Bulk account creation using SQL Bulk Copy
        private async Task BulkCreateAccounts(
            SqlConnection connection,
            SqlTransaction transaction,
            JournalEntryUploadRequest request,
            List<string> accounts,
            int accessCodeId,
            int userId,
            string ipAddress)
        {
            if (!accounts.Any()) return;

            // Get current max count
            var maxCount = await GetDescMaxId(
                connection, transaction, accessCodeId, request.CustomerId,
                request.FinancialYearId, request.BranchId, request.DurationId);

            // Prepare data for bulk insert
            var uploadData = new DataTable();
            uploadData.Columns.Add("ATBU_ID", typeof(int));
            uploadData.Columns.Add("ATBU_CODE", typeof(string));
            uploadData.Columns.Add("ATBU_Description", typeof(string));
            uploadData.Columns.Add("ATBU_CustId", typeof(int));
            uploadData.Columns.Add("ATBU_Opening_Debit_Amount", typeof(decimal));
            uploadData.Columns.Add("ATBU_Opening_Credit_Amount", typeof(decimal));
            uploadData.Columns.Add("ATBU_TR_Debit_Amount", typeof(decimal));
            uploadData.Columns.Add("ATBU_TR_Credit_Amount", typeof(decimal));
            uploadData.Columns.Add("ATBU_Closing_Debit_Amount", typeof(decimal));
            uploadData.Columns.Add("ATBU_Closing_Credit_Amount", typeof(decimal));
            uploadData.Columns.Add("ATBU_DELFLG", typeof(string));
            uploadData.Columns.Add("ATBU_CRBY", typeof(int));
            uploadData.Columns.Add("ATBU_STATUS", typeof(string));
            uploadData.Columns.Add("ATBU_UPDATEDBY", typeof(int));
            uploadData.Columns.Add("ATBU_IPAddress", typeof(string));
            uploadData.Columns.Add("ATBU_CompId", typeof(int));
            uploadData.Columns.Add("ATBU_YEARId", typeof(int));
            uploadData.Columns.Add("ATBU_Branchid", typeof(int));
            uploadData.Columns.Add("ATBU_QuarterId", typeof(int));

            foreach (var account in accounts)
            {
                maxCount++;
                uploadData.Rows.Add(
                    0, // ATBU_ID (auto-generated)
                    "SCh00" + maxCount,
                    account,
                    request.CustomerId,
                    0, 0, 0, 0, 0, 0, // Amounts
                    "A", // DELFLG
                    userId,
                    "C", // STATUS
                    userId,
                    ipAddress,
                    accessCodeId,
                    request.FinancialYearId,
                    request.BranchId,
                    request.DurationId
                );
            }

            // Use SqlBulkCopy for maximum performance
            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction);
            bulkCopy.DestinationTableName = "Acc_TrailBalance_Upload";

            // Map columns
            for (int i = 0; i < uploadData.Columns.Count; i++)
            {
                bulkCopy.ColumnMappings.Add(uploadData.Columns[i].ColumnName, uploadData.Columns[i].ColumnName);
            }

            await bulkCopy.WriteToServerAsync(uploadData);
        }

        // ✅ OPTIMIZED: Bulk transaction insertion
        private async Task BulkInsertTransactions(
            SqlConnection connection,
            SqlTransaction transaction,
            List<BulkTransactionData> transactions)
        {
            if (!transactions.Any()) return;

            // Prepare DataTable for SqlBulkCopy
            var masterData = new DataTable();
            masterData.Columns.Add("Acc_JE_ID", typeof(int));
            masterData.Columns.Add("Acc_JE_TransactionNo", typeof(string));
            masterData.Columns.Add("Acc_JE_Party", typeof(int));
            masterData.Columns.Add("Acc_JE_Location", typeof(int));
            masterData.Columns.Add("Acc_JE_BillType", typeof(int));
            masterData.Columns.Add("Acc_JE_BillNo", typeof(string));
            masterData.Columns.Add("Acc_JE_BillDate", typeof(DateTime));
            masterData.Columns.Add("Acc_JE_BillAmount", typeof(decimal));
            masterData.Columns.Add("Acc_JE_CreatedBy", typeof(int));
            masterData.Columns.Add("Acc_JE_YearID", typeof(int));
            masterData.Columns.Add("Acc_JE_CompID", typeof(int));
            masterData.Columns.Add("Acc_JE_Status", typeof(string));
            masterData.Columns.Add("Acc_JE_Operation", typeof(string));
            masterData.Columns.Add("Acc_JE_IPAddress", typeof(string));
            masterData.Columns.Add("acc_JE_BranchId", typeof(int));
            masterData.Columns.Add("Acc_JE_Quarterly", typeof(int));
            masterData.Columns.Add("Acc_JE_Comments", typeof(string));

            // Group by transaction number to avoid duplicates
            var groupedTransactions = transactions
                .GroupBy(t => t.TransactionNo)
                .Select(g => g.First())
                .ToList();

            foreach (var trans in groupedTransactions)
            {
                masterData.Rows.Add(
                    0, // Acc_JE_ID (auto)
                    trans.TransactionNo,
                    trans.CustomerId,
                    3, // Location
                    trans.JE_Type,
                    trans.BillNo,
                    trans.TransactionDate,
                    Math.Max(trans.Debit, trans.Credit),
                    trans.UserId,
                    trans.FinancialYearId,
                    trans.AccessCodeId,
                    "A", // Status
                    "C", // Operation
                    trans.IpAddress,
                    trans.BranchId,
                    trans.DurationId,
                    trans.Comments
                );
            }

            // Bulk insert master records
            using var bulkCopyMaster = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction);
            bulkCopyMaster.DestinationTableName = "Acc_JE_Master";
            await bulkCopyMaster.WriteToServerAsync(masterData);

            // Now insert details
            var detailData = new DataTable();
            detailData.Columns.Add("AJTB_ID", typeof(int));
            detailData.Columns.Add("AJTB_TranscNo", typeof(string));
            detailData.Columns.Add("AJTB_CustId", typeof(int));
            detailData.Columns.Add("AJTB_Desc", typeof(int));
            detailData.Columns.Add("AJTB_DescName", typeof(string));
            detailData.Columns.Add("AJTB_Debit", typeof(decimal));
            detailData.Columns.Add("AJTB_Credit", typeof(decimal));
            detailData.Columns.Add("AJTB_CreatedBy", typeof(int));
            detailData.Columns.Add("AJTB_CreatedOn", typeof(DateTime));
            detailData.Columns.Add("AJTB_Status", typeof(string));
            detailData.Columns.Add("AJTB_IPAddress", typeof(string));
            detailData.Columns.Add("AJTB_CompID", typeof(int));
            detailData.Columns.Add("AJTB_YearID", typeof(int));
            detailData.Columns.Add("AJTB_BillType", typeof(int));
            detailData.Columns.Add("AJTB_BranchId", typeof(int));
            detailData.Columns.Add("AJTB_QuarterId", typeof(int));

            foreach (var trans in transactions)
            {
                detailData.Rows.Add(
                    0, // AJTB_ID (auto)
                    trans.TransactionNo,
                    trans.CustomerId,
                    trans.AccountId,
                    trans.AccountName,
                    trans.Debit,
                    trans.Credit,
                    trans.UserId,
                    trans.TransactionDate,
                    "A", // Status
                    trans.IpAddress,
                    trans.AccessCodeId,
                    trans.FinancialYearId,
                    trans.JE_Type,
                    trans.BranchId,
                    trans.DurationId
                );
            }

            // Bulk insert detail records
            using var bulkCopyDetail = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction);
            bulkCopyDetail.DestinationTableName = "Acc_JETransactions_Details";
            await bulkCopyDetail.WriteToServerAsync(detailData);
        }

        // ✅ OPTIMIZED: Bulk update account balances
        private async Task BulkUpdateAccountBalances(
            SqlConnection connection,
            SqlTransaction transaction,
            List<BulkTransactionData> transactions)
        {
            // Group by account ID and calculate totals
            var accountTotals = transactions
                .GroupBy(t => t.AccountId)
                .Select(g => new
                {
                    AccountId = g.Key,
                    TotalDebit = g.Sum(x => x.Debit),
                    TotalCredit = g.Sum(x => x.Credit)
                })
                .ToList();

            // Use single UPDATE with CASE statement for all accounts
            var updateSql = new StringBuilder();
            updateSql.AppendLine("UPDATE Acc_TrailBalance_Upload SET");
            updateSql.AppendLine("ATBU_Closing_TotalDebit_Amount = CASE ATBU_ID");

            foreach (var total in accountTotals)
            {
                updateSql.AppendLine($"WHEN {total.AccountId} THEN ISNULL(ATBU_Closing_TotalDebit_Amount, 0) + {total.TotalDebit}");
            }

            updateSql.AppendLine("ELSE ATBU_Closing_TotalDebit_Amount END,");
            updateSql.AppendLine("ATBU_Closing_TotalCredit_Amount = CASE ATBU_ID");

            foreach (var total in accountTotals)
            {
                updateSql.AppendLine($"WHEN {total.AccountId} THEN ISNULL(ATBU_Closing_TotalCredit_Amount, 0) + {total.TotalCredit}");
            }

            updateSql.AppendLine("ELSE ATBU_Closing_TotalCredit_Amount END");
            updateSql.AppendLine($"WHERE ATBU_ID IN ({string.Join(",", accountTotals.Select(a => a.AccountId))})");

            using var command = new SqlCommand(updateSql.ToString(), connection, transaction);
            await command.ExecuteNonQueryAsync();
        }

        // ✅ OPTIMIZED: Fast validation
        private async Task<ApiResponse<string>> PreValidateRowsFast(
            SqlConnection connection,
            SqlTransaction transaction,
            JournalEntryUploadRequest request,
            int accessCodeId)
        {
            // Check for required fields
            for (int i = 0; i < request.Rows.Count; i++)
            {
                var row = request.Rows[i];
                if (string.IsNullOrEmpty(row.Account) && (row.Debit.HasValue || row.Credit.HasValue))
                {
                    return new ApiResponse<string> { Success = false, Message = $"Account cannot be blank - Line No: {i + 1}" };
                }
            }

            // Date format validation
            var invalidDates = request.Rows
                .Where(r => !string.IsNullOrEmpty(r.Transaction_Date) && !IsValidDate(r.Transaction_Date))
                .Select((r, i) => i + 1)
                .ToList();

            if (invalidDates.Any())
            {
                return new ApiResponse<string> { Success = false, Message = $"Invalid Date Format at lines: {string.Join(", ", invalidDates.Take(10))}" };
            }

            return new ApiResponse<string> { Success = true };
        }
        private bool IsValidDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return false;

            // Common date formats
            string[] dateFormats = {
        "MM/dd/yyyy", "dd/MM/yyyy", "yyyy-MM-dd",
        "dd-MM-yyyy", "MM-dd-yyyy", "dd.MM.yyyy",
        "yyyy/MM/dd", "MMM dd, yyyy", "dd-MMM-yyyy",
        "yyyyMMdd", "M/d/yyyy", "d/M/yyyy",
        "dd-MM-yy", "MM/dd/yy", "d-MMM-yy"
    };

            return DateTime.TryParseExact(dateString, dateFormats,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

    //    private DateTime ParseTransactionDate(string dateString)
    //    {
    //        if (string.IsNullOrEmpty(dateString))
    //            return DateTime.MinValue;

    //        // First try exact formats
    //        string[] dateFormats = {
    //    "MM/dd/yyyy", "dd/MM/yyyy", "yyyy-MM-dd",
    //    "dd-MM-yyyy", "MM-dd-yyyy", "dd.MM.yyyy",
    //    "yyyy/MM/dd", "MMM dd, yyyy", "dd-MMM-yyyy",
    //    "yyyyMMdd", "M/d/yyyy", "d/M/yyyy",
    //    "dd-MM-yy", "MM/dd/yy", "d-MMM-yy",
    //    "MMMM dd, yyyy", "dd MMMM yyyy"
    //};

    //        if (DateTime.TryParseExact(dateString, dateFormats,
    //            CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
    //        {
    //            return result;
    //        }

    //        // Fallback to regular parse
    //        if (DateTime.TryParse(dateString, out var fallbackResult))
    //        {
    //            return fallbackResult;
    //        }

    //        return DateTime.MinValue;
    //    }

    //    private DateTime ParseTransactionDate(string dateString, DateTime fallbackDate)
    //    {
    //        var parsedDate = ParseTransactionDate(dateString);
    //        return parsedDate != DateTime.MinValue ? parsedDate : fallbackDate;
    //    }
        // Helper classes for bulk operations
        private class BulkTransactionData
        {
            public string TransactionNo { get; set; }
            public int CustomerId { get; set; }
            public int AccountId { get; set; }
            public string AccountName { get; set; }
            public int JE_Type { get; set; }
            public DateTime TransactionDate { get; set; }
            public decimal Debit { get; set; }
            public decimal Credit { get; set; }
            public int BranchId { get; set; }
            public int DurationId { get; set; }
            public int FinancialYearId { get; set; }
            public int AccessCodeId { get; set; }
            public int UserId { get; set; }
            public string IpAddress { get; set; }
            public string Narration { get; set; }
            public string Comments { get; set; }
            public string BillNo { get; set; }
        }

        // Helper method to generate sequential transaction numbers
        private string GenerateTransactionNo(string lastTransactionNo)
        {
            if (string.IsNullOrEmpty(lastTransactionNo) || !lastTransactionNo.StartsWith("TR"))
            {
                return "TR001";
            }

            var numberPart = lastTransactionNo.Substring(2);
            if (int.TryParse(numberPart, out int number))
            {
                return $"TR{(number + 1):D3}";
            }

            return "TR001";
        }

        // Get last transaction number
        private async Task<string> GetLastTransactionNo(
            SqlConnection connection,
            SqlTransaction transaction,
            int compId,
            int yearId,
            int party,
            int durationId,
            int branchId)
        {
            var sql = @"
        SELECT TOP 1 Acc_JE_TransactionNo 
        FROM Acc_JE_Master
        WHERE Acc_JE_YearID = @YearId
          AND Acc_JE_Party = @Party
          AND Acc_JE_QuarterId = @QuarterId
          AND acc_JE_BranchId = @BranchId
        ORDER BY Acc_JE_ID DESC";

            using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@YearId", yearId);
            command.Parameters.AddWithValue("@Party", party);
            command.Parameters.AddWithValue("@QuarterId", durationId);
            command.Parameters.AddWithValue("@BranchId", branchId);

            var result = await command.ExecuteScalarAsync();
            return result?.ToString() ?? "TR000";
        }

        // Optimized GetDescMaxId
        private async Task<int> GetDescMaxId(
            SqlConnection connection,
            SqlTransaction transaction,
            int compId,
            int customerId,
            int yearId,
            int branchId,
            int durationId)
        {
            var sql = "SELECT COUNT(*) FROM Acc_TrailBalance_Upload WHERE ATBU_CustId = @CustId AND ATBU_CompId = @CompId AND ATBU_YEARId = @YearId AND ATBU_BranchId = @BranchId AND ATBU_QuarterId = @QuarterId";

            using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@CustId", customerId);
            command.Parameters.AddWithValue("@CompId", compId);
            command.Parameters.AddWithValue("@YearId", yearId);
            command.Parameters.AddWithValue("@BranchId", branchId);
            command.Parameters.AddWithValue("@QuarterId", durationId);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
    }
}