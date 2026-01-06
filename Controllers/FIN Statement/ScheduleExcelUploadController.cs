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
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "CustomerCode is missing in session. Please log in again."
                });

            var connectionString = _configuration.GetConnectionString(dbName);

            SqlConnection connection = null;
            SqlTransaction transaction = null;

            try
            {
                connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Headers
                var accessCodeId = int.Parse(Request.Headers["AccessCodeID"].FirstOrDefault() ?? "1");
                var userId = int.Parse(Request.Headers["UserID"].FirstOrDefault() ?? "0");
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

                // Validation
                if (request.CustomerId <= 0)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Select Client" });

                if (request.FinancialYearId <= 0)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Select FinancialYear" });

                if (request.BranchId <= 0)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Select Branch" });

                if (request.DurationId == 0)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Select Duration" });

                if (request.Rows == null || request.Rows.Count == 0)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Please select the Load Button" });

                // ✅ PRE-VALIDATION (NO TRANSACTION)
                var validationResult = await PreValidateRows(
                    connection, null, request, accessCodeId, userId, ipAddress);

                if (!validationResult.Success)
                    return BadRequest(validationResult);

                // ✅ START TRANSACTION ONLY FOR DB WRITE
                transaction = connection.BeginTransaction();

                //await BulkCreateMissingAccounts(
                //    connection, transaction, request, accessCodeId, userId, ipAddress);

                var processResult = await BulkCreateAndProcessTransactions(connection, transaction, request, accessCodeId, userId, ipAddress);

                if (!processResult.Success)
                {
                    transaction.Rollback();
                    return BadRequest(processResult);
                }

                transaction.Commit(); // ✅ RELEASE LOCKS IMMEDIATELY

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Successfully Uploaded"
                });
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();

                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Internal server error",
                    Error = ex.Message
                });
            }
            finally
            {
                // ✅ FORCE KILL CONNECTION
                if (transaction != null)
                    transaction.Dispose();

                if (connection != null)
                {
                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                }
            }
        }
        private async Task<ApiResponse<string>> BulkCreateAndProcessTransactions(
 SqlConnection connection,
 SqlTransaction transaction,
 JournalEntryUploadRequest request,
 int accessCodeId,
 int userId,
 string ipAddress)
        {
            const int batchSize = 1000;
            var validRows = request.Rows.Where(r => !string.IsNullOrEmpty(r.Account)).ToList();
            var batches = validRows.Batch(batchSize);

            var transactionNo = "0";
            var iErrorLine = 1;
            int MasId = 0;
            DateTime currentTransactionDate = DateTime.MinValue;

            // ===============================
            // STEP 1: PRE-CACHE ALL DATA
            // ===============================
            var accountsToCreate = new List<string>();
            var existingAccountsCache = new Dictionary<string, int>();
            var voucherTypesCache = new Dictionary<string, int>();

            // Cache existing accounts and identify missing ones
            foreach (var row in validRows)
            {
                if (string.IsNullOrWhiteSpace(row.Account)) continue;

                var account = row.Account;
                if (existingAccountsCache.ContainsKey(account)) continue;

                var exists = await CheckAccountData(
                    connection, transaction, accessCodeId,
                    request.CustomerId, account,
                    request.FinancialYearId,
                    request.BranchId,
                    request.DurationId);

                existingAccountsCache[account] = exists;

                if (exists == 0)
                    accountsToCreate.Add(account);

                // Cache voucher types
                if (!string.IsNullOrWhiteSpace(row.JE_Type) && !voucherTypesCache.ContainsKey(row.JE_Type))
                {
                    var voucherId = await CheckVoucherType(connection, transaction, accessCodeId, "JE", row.JE_Type);
                    voucherTypesCache[row.JE_Type] = voucherId;
                }
            }

            // ===============================
            // STEP 2: CREATE MISSING ACCOUNTS
            // ===============================
            if (accountsToCreate.Any())
            {
                int maxCount = await GetDescMaxId(
                    connection, transaction,
                    accessCodeId, request.CustomerId, "",
                    request.FinancialYearId,
                    request.BranchId,
                    request.DurationId);

                // Determine if we need schedule mappings
                bool hasPurchase = validRows.Any(r =>
                    r.JE_Type?.Equals("Purchase", StringComparison.OrdinalIgnoreCase) == true);
                bool hasSales = validRows.Any(r =>
                    r.JE_Type?.Equals("Sales", StringComparison.OrdinalIgnoreCase) == true);

                // Pre-calculate schedule mappings
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

                // Create missing accounts
                foreach (var account in accountsToCreate)
                {
                    maxCount++;

                    var uploadId = await SaveTrailBalanceExcelUpload(
                        connection, transaction,
                        new TrailBalanceUpload
                        {
                            ATBU_ID = 0,
                            ATBU_CODE = "SCh00" + maxCount,
                            ATBU_Description = account,
                            ATBU_CustId = request.CustomerId,
                            ATBU_Opening_Debit_Amount = 0,
                            ATBU_Opening_Credit_Amount = 0,
                            ATBU_TR_Debit_Amount = 0,
                            ATBU_TR_Credit_Amount = 0,
                            ATBU_Closing_Debit_Amount = 0,
                            ATBU_Closing_Credit_Amount = 0,
                            ATBU_DELFLG = "A",
                            ATBU_CRBY = userId,
                            ATBU_STATUS = "C",
                            ATBU_UPDATEDBY = userId,
                            ATBU_IPAddress = ipAddress,
                            ATBU_CompId = accessCodeId,
                            ATBU_YEARId = request.FinancialYearId,
                            ATBU_Branchname = request.BranchId,
                            ATBU_QuaterId = request.DurationId
                        });

                    bool isPurchaseAccount = validRows.Any(r =>
                        r.Account == account &&
                        r.JE_Type?.Equals("Purchase", StringComparison.OrdinalIgnoreCase) == true);

                    bool isSalesAccount = validRows.Any(r =>
                        r.Account == account &&
                        r.JE_Type?.Equals("Sales", StringComparison.OrdinalIgnoreCase) == true);

                    bool hasTransactionDate = validRows.Any(r =>
                        r.Account == account &&
                        !string.IsNullOrWhiteSpace(r.Transaction_Date));

                    int headingId = 0;
                    int subHeadingId = 0;
                    int itemId = 0;
                    int scheduleType = 0;

                    if (hasTransactionDate)
                    {
                        if (isPurchaseAccount)
                        {
                            headingId = pHeadingId;
                            subHeadingId = pSubHeadingId;
                            itemId = pItemId;
                            scheduleType = pScheduleType;
                        }
                        else if (isSalesAccount)
                        {
                            headingId = sHeadingId;
                            subHeadingId = sSubHeadingId;
                            itemId = sItemId;
                            scheduleType = sScheduleType;
                        }
                    }

                    await SaveTrailBalanceExcelUploadDetails(
                        connection, transaction,
                        new TrailBalanceUpload
                        {
                            ATBUD_ID = 0,
                            ATBUD_Masid = uploadId,
                            ATBUD_CODE = "SCh00" + maxCount,
                            ATBUD_Description = account,
                            ATBUD_CustId = request.CustomerId,
                            ATBUD_SChedule_Type = scheduleType,
                            ATBUD_Branchname = request.BranchId,
                            ATBUD_iQuarterly = request.DurationId,
                            ATBUD_Company_Type = request.CustomerId,
                            ATBUD_Headingid = headingId,
                            ATBUD_Subheading = subHeadingId,
                            ATBUD_itemid = itemId,
                            ATBUD_Subitemid = 0,
                            ATBUD_DELFLG = "A",
                            ATBUD_CRBY = userId,
                            ATBUD_STATUS = "C",
                            ATBUD_Progress = "Uploaded",
                            ATBUD_UPDATEDBY = userId,
                            ATBUD_IPAddress = ipAddress,
                            ATBUD_CompId = accessCodeId,
                            ATBUD_YEARId = request.FinancialYearId
                        });

                    // Get the actual account ID after creation
                    var newAccountId = await CheckAccountData(
                        connection, transaction, accessCodeId,
                        request.CustomerId, account,
                        request.FinancialYearId,
                        request.BranchId,
                        request.DurationId);

                    existingAccountsCache[account] = newAccountId;
                }
            }

            // ===============================
            // STEP 3: PROCESS TRANSACTIONS
            // ===============================
            foreach (var batch in batches)
            {
                foreach (var row in batch)
                {
                    if (string.IsNullOrEmpty(row.Account)) continue;

                    // Parse transaction date
                    DateTime transactionDate = DateTime.MinValue;

                    if (!string.IsNullOrEmpty(row.Transaction_Date))
                    {
                        transactionDate = ParseTransactionDate(row.Transaction_Date);

                        if (transactionDate == DateTime.MinValue)
                        {
                            return new ApiResponse<string> { Success = false, Message = $"Invalid Date Format - Line No: {iErrorLine}" };
                        }

                        currentTransactionDate = transactionDate;
                    }
                   

                    // Get account ID from cache
                    var accountId = existingAccountsCache[row.Account];
                    if (accountId == 0)
                    {
                        return new ApiResponse<string> { Success = false, Message = $"Account not found: {row.Account} - Line No: {iErrorLine}" };
                    }

                    // Get voucher type from cache
                    var billType = !string.IsNullOrWhiteSpace(row.JE_Type) && voucherTypesCache.ContainsKey(row.JE_Type)
                        ? voucherTypesCache[row.JE_Type]
                        : 0;

                    // Create Journal Entry Master for new transaction date
                    if (transactionDate != DateTime.MinValue && transactionDate != new DateTime(1900, 1, 1))
                    {
                        transactionNo = await GenerateTransactionNo(
                            connection, transaction, accessCodeId,
                            request.FinancialYearId, request.CustomerId,
                            request.DurationId, request.BranchId);

                        MasId = await SaveJournalEntryMaster(connection, transaction, new JournalEntry
                        {
                            Acc_JE_ID = 0,
                            AJTB_TranscNo = transactionNo,
                            Acc_JE_Location = 3,
                            Acc_JE_Party = request.CustomerId,
                            Acc_JE_BillType = billType,
                            Acc_JE_BillNo = row.Bill_No ?? "",
                            Acc_JE_BillDate = transactionDate,
                            Acc_JE_BillAmount = row.Debit ?? row.Credit ?? 0,
                            Acc_JE_YearID = request.FinancialYearId,
                            Acc_JE_Status = "A",
                            Acc_JE_CreatedBy = userId,
                            Acc_JE_Operation = "C",
                            Acc_JE_IPAddress = ipAddress,
                            Acc_JE_AdvanceNaration = row.Narration ?? "",
                            Acc_JE_CompID = accessCodeId,
                            Acc_JE_AdvanceAmount = 0.00m,
                            Acc_JE_BalanceAmount = 0.00m,
                            Acc_JE_NetAmount = 0.00m,
                            Acc_JE_PaymentNarration = "",
                            Acc_JE_ChequeNo = "",
                            Acc_JE_ChequeDate = new DateTime(1900, 1, 1),
                            Acc_JE_IFSCCode = "",
                            Acc_JE_BankName = "",
                            Acc_JE_BranchName = "",
                            Acc_JE_BillCreatedDate = new DateTime(1900, 1, 1),
                            acc_JE_BranchId = request.BranchId,
                            Acc_JE_Comments = row.Comments ?? "",
                            Acc_JE_Quarterly = request.DurationId
                        });
                    }
                    if (!string.IsNullOrEmpty(row.Transaction_Date))
                    {
                        transactionDate = ParseTransactionDate(row.Transaction_Date);

                        if (transactionDate == DateTime.MinValue)
                        {
                            return new ApiResponse<string> { Success = false, Message = $"Invalid Date Format - Line No: {iErrorLine}" };
                        }

                        currentTransactionDate = transactionDate;
                    }
                    else
                    {
                        transactionDate = currentTransactionDate;

                        if (transactionDate == DateTime.MinValue)
                        {
                            return new ApiResponse<string> { Success = false, Message = $"No valid date found to carry forward - Line No: {iErrorLine}" };
                        }
                    }
                    // Process Debit
                    if (row.Debit.HasValue && row.Debit > 0)
                    {
                        await SaveTransactionDetails(connection, transaction, new JournalEntry
                        {
                            AJTB_ID = 0,
                            AJTB_TranscNo = transactionNo,
                            AJTB_CustId = request.CustomerId,
                            AJTB_MAsID = MasId,
                            AJTB_Deschead = accountId,
                            AJTB_Desc = accountId,
                            AJTB_DescName = row.Account,
                            AJTB_Debit = (decimal)row.Debit.Value,
                            AJTB_Credit = 0,
                            AJTB_CreatedBy = userId,
                            AJTB_UpdatedBy = userId,
                            AJTB_Status = "A",
                            AJTB_YearID = request.FinancialYearId,
                            AJTB_CompID = accessCodeId,
                            AJTB_IPAddress = ipAddress,
                            AJTB_BillType = billType,
                            AJTB_BranchId = request.BranchId,
                            AJTB_QuarterId = request.DurationId,
                            AJTB_CreatedOn = transactionDate
                        });

                        await UpdateJeDet(
                            connection, transaction, accessCodeId,
                            request.FinancialYearId, accountId,
                            request.CustomerId, 0, (double)row.Debit.Value,
                            request.BranchId, (double)row.Debit.Value, 0,
                            request.DurationId);
                    }

                    // Process Credit
                    if (row.Credit.HasValue && row.Credit > 0)
                    {
                        await SaveTransactionDetails(connection, transaction, new JournalEntry
                        {
                            AJTB_ID = 0,
                            AJTB_TranscNo = transactionNo,
                            AJTB_CustId = request.CustomerId,
                            AJTB_MAsID = MasId,
                            AJTB_Deschead = accountId,
                            AJTB_Desc = accountId,
                            AJTB_DescName = row.Account,
                            AJTB_Debit = 0,
                            AJTB_Credit = (decimal)row.Credit.Value,
                            AJTB_CreatedBy = userId,
                            AJTB_UpdatedBy = userId,
                            AJTB_Status = "A",
                            AJTB_YearID = request.FinancialYearId,
                            AJTB_CompID = accessCodeId,
                            AJTB_IPAddress = ipAddress,
                            AJTB_BillType = billType,
                            AJTB_BranchId = request.BranchId,
                            AJTB_QuarterId = request.DurationId,
                            AJTB_CreatedOn = transactionDate
                        });

                        await UpdateJeDet(
                            connection, transaction, accessCodeId,
                            request.FinancialYearId, accountId,
                            request.CustomerId, 1, (double)row.Credit.Value,
                            request.BranchId, 0, (double)row.Credit.Value,
                            request.DurationId);
                    }

                    iErrorLine++;
                }
            }

            return new ApiResponse<string> { Success = true };
        }


        #region Optimized Helper Methods

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
        private async Task BulkCreateMissingAccounts(
          SqlConnection connection,
          SqlTransaction transaction,
          JournalEntryUploadRequest request,
          int accessCodeId,
          int userId,
          string ipAddress)
        {
            /* ===============================
               STEP 1: FIND MISSING ACCOUNTS
            =============================== */
            var accountsToCreate = new List<string>();
            var existingAccountsCache = new Dictionary<string, int>();

            foreach (var row in request.Rows.Where(r => !string.IsNullOrWhiteSpace(r.Account)))
            {
                if (existingAccountsCache.ContainsKey(row.Account))
                    continue;

                var exists = await CheckAccountData(
                    connection, transaction, accessCodeId,
                    request.CustomerId, row.Account,
                    request.FinancialYearId,
                    request.BranchId,
                    request.DurationId);

                existingAccountsCache[row.Account] = exists;

                if (exists == 0)
                    accountsToCreate.Add(row.Account);
            }

            if (!accountsToCreate.Any())
                return;

            int maxCount = await GetDescMaxId(
                connection, transaction,
                accessCodeId, request.CustomerId, "",
                request.FinancialYearId,
                request.BranchId,
                request.DurationId);

            /* ===============================
               STEP 2: CHECK JE TYPES EXISTENCE
            =============================== */
            bool hasPurchase = request.Rows.Any(r =>
                r.JE_Type?.Equals("Purchase", StringComparison.OrdinalIgnoreCase) == true);

            bool hasSales = request.Rows.Any(r =>
                r.JE_Type?.Equals("Sales", StringComparison.OrdinalIgnoreCase) == true);

            /* ===============================
               STEP 3: PREPARE PURCHASE MAPPING
            =============================== */
            int pHeadingId = 0, pSubHeadingId = 0, pItemId = 0, pScheduleType = 0;

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

            /* ===============================
               STEP 4: PREPARE SALES MAPPING
            =============================== */
            int sHeadingId = 0, sSubHeadingId = 0, sItemId = 0, sScheduleType = 0;

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

            /* ===============================
               STEP 5: SAVE ACCOUNTS
               → PASS 0 IF NO TRANSACTION DATE
            =============================== */
            foreach (var account in accountsToCreate)
            {
                maxCount++;

                var uploadId = await SaveTrailBalanceExcelUpload(
                    connection, transaction,
                    new TrailBalanceUpload
                    {
                        ATBU_ID = 0,
                        ATBU_CODE = "SCh00" + maxCount,
                        ATBU_Description = account,
                        ATBU_CustId = request.CustomerId,
                        ATBU_Opening_Debit_Amount = 0,
                        ATBU_Opening_Credit_Amount = 0,
                        ATBU_TR_Debit_Amount = 0,
                        ATBU_TR_Credit_Amount = 0,
                        ATBU_Closing_Debit_Amount = 0,
                        ATBU_Closing_Credit_Amount = 0,
                        ATBU_DELFLG = "A",
                        ATBU_CRBY = userId,
                        ATBU_STATUS = "C",
                        ATBU_UPDATEDBY = userId,
                        ATBU_IPAddress = ipAddress,
                        ATBU_CompId = accessCodeId,
                        ATBU_YEARId = request.FinancialYearId,
                        ATBU_Branchname = request.BranchId,
                        ATBU_QuaterId = request.DurationId
                    });

                bool isPurchaseAccount = request.Rows.Any(r =>
                    r.Account == account &&
                    r.JE_Type?.Equals("Purchase", StringComparison.OrdinalIgnoreCase) == true);

                bool isSalesAccount = request.Rows.Any(r =>
                    r.Account == account &&
                    r.JE_Type?.Equals("Sales", StringComparison.OrdinalIgnoreCase) == true);

                // 🔑 NEW CHECK → Transaction Date exists for this account
                bool hasTransactionDate = request.Rows.Any(r =>
                    r.Account == account &&
                    !string.IsNullOrWhiteSpace(r.Transaction_Date));

                // DEFAULT TO 0
                int headingId = 0;
                int subHeadingId = 0;
                int itemId = 0;
                int scheduleType = 0;

                if (hasTransactionDate)
                {
                    if (isPurchaseAccount)
                    {
                        headingId = pHeadingId;
                        subHeadingId = pSubHeadingId;
                        itemId = pItemId;
                        scheduleType = pScheduleType;
                    }
                    else if (isSalesAccount)
                    {
                        headingId = sHeadingId;
                        subHeadingId = sSubHeadingId;
                        itemId = sItemId;
                        scheduleType = sScheduleType;
                    }
                }

                await SaveTrailBalanceExcelUploadDetails(
                    connection, transaction,
                    new TrailBalanceUpload
                    {
                        ATBUD_ID = 0,
                        ATBUD_Masid = uploadId,
                        ATBUD_CODE = "SCh00" + maxCount,
                        ATBUD_Description = account,
                        ATBUD_CustId = request.CustomerId,
                        ATBUD_SChedule_Type = scheduleType,
                        ATBUD_Branchname = request.BranchId,
                        ATBUD_iQuarterly = request.DurationId,
                        ATBUD_Company_Type = request.CustomerId,
                        ATBUD_Headingid = headingId,      // ✅ 0 if no date
                        ATBUD_Subheading = subHeadingId,  // ✅ 0 if no date
                        ATBUD_itemid = itemId,            // ✅ 0 if no date
                        ATBUD_Subitemid = 0,
                        ATBUD_DELFLG = "A",
                        ATBUD_CRBY = userId,
                        ATBUD_STATUS = "C",
                        ATBUD_Progress = "Uploaded",
                        ATBUD_UPDATEDBY = userId,
                        ATBUD_IPAddress = ipAddress,
                        ATBUD_CompId = accessCodeId,
                        ATBUD_YEARId = request.FinancialYearId
                    });
            }
        }



        private async Task<ApiResponse<string>> ProcessTransactionsInBatches(SqlConnection connection, SqlTransaction transaction, JournalEntryUploadRequest request, int accessCodeId, int userId, string ipAddress)
        {
            const int batchSize = 1000; // Process 1000 records at a time
            var validRows = request.Rows.Where(r => !string.IsNullOrEmpty(r.Account)).ToList();
            var batches = validRows.Batch(batchSize);

            var transactionNo = "0";
            var iErrorLine = 1;
            int MasId = 0;

            // ✅ ADDED: Variable to track the last valid transaction date
            DateTime currentTransactionDate = DateTime.MinValue;

            foreach (var batch in batches)
            {
                // ✅ OPTIMIZATION: Pre-load account IDs for this batch
                var accountIds = new Dictionary<string, int>();
                var accountsInBatch = batch.Where(r => !string.IsNullOrEmpty(r.Account)).Select(r => r.Account).Distinct();

                foreach (var account in accountsInBatch)
                {
                    var accountId = await CheckAccountData(connection, transaction, accessCodeId, request.CustomerId, account, request.FinancialYearId, request.BranchId, request.DurationId);
                    accountIds[account] = accountId;
                }

                // ✅ OPTIMIZATION: Pre-load voucher types for this batch
                var voucherTypes = new Dictionary<string, int>();
                var voucherTypesInBatch = batch.Where(r => !string.IsNullOrEmpty(r.JE_Type)).Select(r => r.JE_Type).Distinct();

                foreach (var voucherType in voucherTypesInBatch)
                {
                    var voucherId = await CheckVoucherType(connection, transaction, accessCodeId, "JE", voucherType);
                    voucherTypes[voucherType] = voucherId;
                }


                // Process each row in the batch
                foreach (var row in batch)
                {
                    if (string.IsNullOrEmpty(row.Account)) continue;

                    // ✅ MODIFIED: Parse date only if it exists, otherwise use the last valid date
                    DateTime transactionDate = DateTime.MinValue;

                    if (!string.IsNullOrEmpty(row.Transaction_Date))
                    {
                        transactionDate = ParseTransactionDate(row.Transaction_Date);

                        if (transactionDate == DateTime.MinValue)
                        {
                            return new ApiResponse<string> { Success = false, Message = $"Invalid Date Format - Line No: {iErrorLine}" };
                        }

                        // ✅ Update currentTransactionDate with the new valid date
                        currentTransactionDate = transactionDate;
                    }

                    // Generate transaction number if needed
                    var billType = voucherTypes.ContainsKey(row.JE_Type) ? voucherTypes[row.JE_Type] : 0;
                    var accountId = accountIds[row.Account];
                    int iJEID = 0;


                    // Save Journal Entry Master if we have a valid date
                    // ✅ MODIFIED: Only check for default date, not 1900-01-01
                    if (transactionDate != DateTime.MinValue && transactionDate != new DateTime(1900, 1, 1))
                    {
                        transactionNo = await GenerateTransactionNo(connection, transaction, accessCodeId, request.FinancialYearId, request.CustomerId, request.DurationId, request.BranchId);

                        iJEID = await SaveJournalEntryMaster(connection, transaction, new JournalEntry
                        {
                            Acc_JE_ID = 0,
                            AJTB_TranscNo = transactionNo,
                            Acc_JE_Location = 3,
                            Acc_JE_Party = request.CustomerId,
                            Acc_JE_BillType = billType,
                            Acc_JE_BillNo = row.Bill_No ?? "",
                            Acc_JE_BillDate = transactionDate,
                            Acc_JE_BillAmount = row.Debit ?? row.Credit ?? 0,
                            Acc_JE_YearID = request.FinancialYearId,
                            Acc_JE_Status = "A",
                            Acc_JE_CreatedBy = userId,
                            Acc_JE_Operation = "C",
                            Acc_JE_IPAddress = ipAddress,
                            Acc_JE_AdvanceNaration = row.Narration ?? "",
                            Acc_JE_CompID = accessCodeId,
                            Acc_JE_AdvanceAmount = 0.00m,
                            Acc_JE_BalanceAmount = 0.00m,
                            Acc_JE_NetAmount = 0.00m,
                            Acc_JE_PaymentNarration = "",
                            Acc_JE_ChequeNo = "",
                            Acc_JE_ChequeDate = new DateTime(1900, 1, 1),
                            Acc_JE_IFSCCode = "",
                            Acc_JE_BankName = "",
                            Acc_JE_BranchName = "",
                            Acc_JE_BillCreatedDate = new DateTime(1900, 1, 1),
                            acc_JE_BranchId = request.BranchId,
                            Acc_JE_Comments = row.Comments ?? "",
                            Acc_JE_Quarterly = request.DurationId
                        });
                        MasId = iJEID;
                    }
                    if (!string.IsNullOrEmpty(row.Transaction_Date))
                    {
                        transactionDate = ParseTransactionDate(row.Transaction_Date);

                        if (transactionDate == DateTime.MinValue)
                        {
                            return new ApiResponse<string> { Success = false, Message = $"Invalid Date Format - Line No: {iErrorLine}" };
                        }

                        // ✅ Update currentTransactionDate with the new valid date
                        currentTransactionDate = transactionDate;
                    }
                    else
                    {
                        // ✅ If no date provided, use the last valid transaction date
                        transactionDate = currentTransactionDate;

                        // Check if we have a valid date to carry forward
                        if (transactionDate == DateTime.MinValue)
                        {
                            return new ApiResponse<string> { Success = false, Message = $"No valid date found to carry forward - Line No: {iErrorLine}" };
                        }
                    }
                    // Process Debit
                    if (row.Debit.HasValue && row.Debit > 0)
                    {
                        await SaveTransactionDetails(connection, transaction, new JournalEntry
                        {
                            AJTB_ID = 0,
                            AJTB_TranscNo = transactionNo,
                            AJTB_CustId = request.CustomerId,
                            AJTB_MAsID = MasId,
                            AJTB_Deschead = accountId,
                            AJTB_Desc = accountId,
                            AJTB_DescName = row.Account,
                            AJTB_Debit = (decimal)row.Debit.Value,
                            AJTB_Credit = 0,
                            AJTB_CreatedBy = userId,
                            AJTB_UpdatedBy = userId,
                            AJTB_Status = "A",
                            AJTB_YearID = request.FinancialYearId,
                            AJTB_CompID = accessCodeId,
                            AJTB_IPAddress = ipAddress,
                            AJTB_BillType = billType,
                            AJTB_BranchId = request.BranchId,
                            AJTB_QuarterId = request.DurationId,
                            AJTB_CreatedOn = transactionDate
                        });

                        await UpdateJeDet(connection, transaction, accessCodeId, request.FinancialYearId, accountId, request.CustomerId, 0, (double)row.Debit.Value, request.BranchId, (double)row.Debit.Value, 0, request.DurationId);
                    }

                    // Process Credit
                    if (row.Credit.HasValue && row.Credit > 0)
                    {
                        await SaveTransactionDetails(connection, transaction, new JournalEntry
                        {
                            AJTB_ID = 0,
                            AJTB_TranscNo = transactionNo,
                            AJTB_CustId = request.CustomerId,
                            AJTB_MAsID = MasId,
                            AJTB_Deschead = accountId,
                            AJTB_Desc = accountId,
                            AJTB_DescName = row.Account,
                            AJTB_Debit = 0,
                            AJTB_Credit = (decimal)row.Credit.Value,
                            AJTB_CreatedBy = userId,
                            AJTB_UpdatedBy = userId,
                            AJTB_Status = "A",
                            AJTB_YearID = request.FinancialYearId,
                            AJTB_CompID = accessCodeId,
                            AJTB_IPAddress = ipAddress,
                            AJTB_BillType = billType,
                            AJTB_BranchId = request.BranchId,
                            AJTB_QuarterId = request.DurationId,
                            AJTB_CreatedOn = transactionDate
                        });

                        await UpdateJeDet(connection, transaction, accessCodeId, request.FinancialYearId, accountId, request.CustomerId, 1, (double)row.Credit.Value, request.BranchId, 0, (double)row.Credit.Value, request.DurationId);
                    }

                    iErrorLine++;
                }
            }

            return new ApiResponse<string> { Success = true };
        }

        private bool IsValidDate(string dateString)
        {
            string[] dateFormats = {
            "MM/dd/yyyy", "dd/MM/yyyy", "yyyy-MM-dd",
            "dd-MM-yyyy", "MM-dd-yyyy", "dd.MM.yyyy",
            "MMMM dd, yyyy", "dd MMMM yyyy", "yyyy/MM/dd",
            "MMM dd, yyyy", "dd-MMM-yyyy", "yyyyMMdd"
        };

            return DateTime.TryParseExact(dateString, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        private DateTime ParseTransactionDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return new DateTime(1900, 1, 1);

            string[] dateFormats = {
            "MM/dd/yyyy", "dd/MM/yyyy", "yyyy-MM-dd",
            "dd-MM-yyyy", "MM-dd-yyyy", "dd.MM.yyyy",
            "MMMM dd, yyyy", "dd MMMM yyyy", "yyyy/MM/dd",
            "MMM dd, yyyy", "dd-MMM-yyyy", "yyyyMMdd"
        };

            if (DateTime.TryParseExact(dateString, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                return result;

            return DateTime.MinValue;
        }

        #endregion

        #region Database Helper Methods

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

        private async Task<int> CheckAccountData(SqlConnection connection, SqlTransaction transaction, int compId, int customerId, string description, int yearId, int branchId, int durationId)
        {
            var sql = "SELECT ISNULL(ATBU_ID, 0) AS ATBU_ID FROM Acc_TrailBalance_Upload WHERE ATBU_Description = @Description AND ATBU_CustId = @CustId AND ATBU_Branchid = @BranchId AND ATBU_CompId = @CompId AND ATBU_YEARId = @YearId";

            if (durationId != 0)
            {
                sql += " AND ATBU_QuarterId = @QuarterId";
            }

            using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@Description", description);
            command.Parameters.AddWithValue("@CustId", customerId);
            command.Parameters.AddWithValue("@BranchId", branchId);
            command.Parameters.AddWithValue("@CompId", compId);
            command.Parameters.AddWithValue("@YearId", yearId);

            if (durationId != 0)
            {
                command.Parameters.AddWithValue("@QuarterId", durationId);
            }

            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        private async Task<int> GetDescMaxId(SqlConnection connection, SqlTransaction transaction, int compId, int customerId, string description, int yearId, int branchId, int durationId)
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

        private async Task<int> SaveTrailBalanceExcelUpload(SqlConnection connection, SqlTransaction transaction, TrailBalanceUpload upload)
        {
            using var command = new SqlCommand("spAcc_TrailBalance_Upload", connection, transaction);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@ATBU_ID", upload.ATBU_ID);
            command.Parameters.AddWithValue("@ATBU_CODE", upload.ATBU_CODE);
            command.Parameters.AddWithValue("@ATBU_Description", upload.ATBU_Description);
            command.Parameters.AddWithValue("@ATBU_CustId", upload.ATBU_CustId);
            command.Parameters.AddWithValue("@ATBU_Opening_Debit_Amount", upload.ATBU_Opening_Debit_Amount);
            command.Parameters.AddWithValue("@ATBU_Opening_Credit_Amount", upload.ATBU_Opening_Credit_Amount);
            command.Parameters.AddWithValue("@ATBU_TR_Debit_Amount", upload.ATBU_TR_Debit_Amount);
            command.Parameters.AddWithValue("@ATBU_TR_Credit_Amount", upload.ATBU_TR_Credit_Amount);
            command.Parameters.AddWithValue("@ATBU_Closing_Debit_Amount", upload.ATBU_Closing_Debit_Amount);
            command.Parameters.AddWithValue("@ATBU_Closing_Credit_Amount", upload.ATBU_Closing_Credit_Amount);
            command.Parameters.AddWithValue("@ATBU_DELFLG", upload.ATBU_DELFLG);
            command.Parameters.AddWithValue("@ATBU_CRBY", upload.ATBU_CRBY);
            command.Parameters.AddWithValue("@ATBU_STATUS", upload.ATBU_STATUS);
            command.Parameters.AddWithValue("@ATBU_UPDATEDBY", upload.ATBU_UPDATEDBY);
            command.Parameters.AddWithValue("@ATBU_IPAddress", upload.ATBU_IPAddress);
            command.Parameters.AddWithValue("@ATBU_CompId", upload.ATBU_CompId);
            command.Parameters.AddWithValue("@ATBU_YEARId", upload.ATBU_YEARId);
            command.Parameters.AddWithValue("@ATBU_Branchid", upload.ATBU_Branchname);
            command.Parameters.AddWithValue("@ATBU_QuarterId", upload.ATBU_QuaterId);

            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
            command.Parameters.Add(updateOrSaveParam);
            command.Parameters.Add(operParam);

            await command.ExecuteNonQueryAsync();

            return (int)(operParam.Value ?? 0);
        }

        private async Task<int> SaveTrailBalanceExcelUploadDetails(SqlConnection connection, SqlTransaction transaction, TrailBalanceUpload upload)
        {
            using var command = new SqlCommand("spAcc_TrailBalance_Upload_Details", connection, transaction);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@ATBUD_ID", upload.ATBUD_ID);
            command.Parameters.AddWithValue("@ATBUD_Masid", upload.ATBUD_Masid);
            command.Parameters.AddWithValue("@ATBUD_CODE", upload.ATBUD_CODE);
            command.Parameters.AddWithValue("@ATBUD_Description", upload.ATBUD_Description);
            command.Parameters.AddWithValue("@ATBUD_CustId", upload.ATBUD_CustId);
            command.Parameters.AddWithValue("@ATBUD_SChedule_Type", upload.ATBUD_SChedule_Type);
            command.Parameters.AddWithValue("@ATBUD_Branchid", upload.ATBUD_Branchname);
            command.Parameters.AddWithValue("@ATBUD_QuarterId", upload.ATBUD_iQuarterly);
            command.Parameters.AddWithValue("@ATBUD_Company_Type", upload.ATBUD_Company_Type);
            command.Parameters.AddWithValue("@ATBUD_Headingid", upload.ATBUD_Headingid);
            command.Parameters.AddWithValue("@ATBUD_Subheading", upload.ATBUD_Subheading);
            command.Parameters.AddWithValue("@ATBUD_itemid", upload.ATBUD_itemid);
            command.Parameters.AddWithValue("@ATBUD_Subitemid", upload.ATBUD_Subitemid);
            command.Parameters.AddWithValue("@ATBUD_DELFLG", upload.ATBUD_DELFLG);
            command.Parameters.AddWithValue("@ATBUD_CRBY", upload.ATBUD_CRBY);
            command.Parameters.AddWithValue("@ATBUD_UPDATEDBY", upload.ATBUD_UPDATEDBY);
            command.Parameters.AddWithValue("@ATBUD_STATUS", upload.ATBUD_STATUS);
            command.Parameters.AddWithValue("@ATBUD_Progress", upload.ATBUD_Progress);
            command.Parameters.AddWithValue("@ATBUD_IPAddress", upload.ATBUD_IPAddress);
            command.Parameters.AddWithValue("@ATBUD_CompId", upload.ATBUD_CompId);
            command.Parameters.AddWithValue("@ATBUD_YEARId", upload.ATBUD_YEARId);

            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
            command.Parameters.Add(updateOrSaveParam);
            command.Parameters.Add(operParam);

            await command.ExecuteNonQueryAsync();

            return (int)(operParam.Value ?? 0);
        }

        private async Task<int> SaveJournalEntryMaster(SqlConnection connection, SqlTransaction transaction, JournalEntry journalEntry)
        {
            using var command = new SqlCommand("spAcc_JE_Master", connection, transaction);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Acc_JE_ID", journalEntry.Acc_JE_ID);
            command.Parameters.AddWithValue("@Acc_JE_TransactionNo", journalEntry.AJTB_TranscNo);
            command.Parameters.AddWithValue("@Acc_JE_Party", journalEntry.Acc_JE_Party);
            command.Parameters.AddWithValue("@Acc_JE_Location", journalEntry.Acc_JE_Location);
            command.Parameters.AddWithValue("@Acc_JE_BillType", journalEntry.Acc_JE_BillType);
            command.Parameters.AddWithValue("@Acc_JE_BillNo", journalEntry.Acc_JE_BillNo);
            command.Parameters.AddWithValue("@Acc_JE_BillDate", journalEntry.Acc_JE_BillDate);
            command.Parameters.AddWithValue("@Acc_JE_BillAmount", journalEntry.Acc_JE_BillAmount);
            command.Parameters.AddWithValue("@Acc_JE_AdvanceAmount", journalEntry.Acc_JE_AdvanceAmount);
            command.Parameters.AddWithValue("@Acc_JE_AdvanceNaration", journalEntry.Acc_JE_AdvanceNaration);
            command.Parameters.AddWithValue("@Acc_JE_BalanceAmount", journalEntry.Acc_JE_BalanceAmount);
            command.Parameters.AddWithValue("@Acc_JE_NetAmount", journalEntry.Acc_JE_NetAmount);
            command.Parameters.AddWithValue("@Acc_JE_PaymentNarration", journalEntry.Acc_JE_PaymentNarration);
            command.Parameters.AddWithValue("@Acc_JE_ChequeNo", journalEntry.Acc_JE_ChequeNo);
            command.Parameters.AddWithValue("@Acc_JE_ChequeDate", journalEntry.Acc_JE_ChequeDate);
            command.Parameters.AddWithValue("@Acc_JE_IFSCCode", journalEntry.Acc_JE_IFSCCode);
            command.Parameters.AddWithValue("@Acc_JE_BankName", journalEntry.Acc_JE_BankName);
            command.Parameters.AddWithValue("@Acc_JE_BranchName", journalEntry.Acc_JE_BranchName);
            command.Parameters.AddWithValue("@Acc_JE_CreatedBy", journalEntry.Acc_JE_CreatedBy);
            command.Parameters.AddWithValue("@Acc_JE_YearID", journalEntry.Acc_JE_YearID);
            command.Parameters.AddWithValue("@Acc_JE_CompID", journalEntry.Acc_JE_CompID);
            command.Parameters.AddWithValue("@Acc_JE_Status", journalEntry.Acc_JE_Status);
            command.Parameters.AddWithValue("@Acc_JE_Operation", journalEntry.Acc_JE_Operation);
            command.Parameters.AddWithValue("@Acc_JE_IPAddress", journalEntry.Acc_JE_IPAddress);
            command.Parameters.AddWithValue("@Acc_JE_BillCreatedDate", journalEntry.Acc_JE_BillCreatedDate);
            command.Parameters.AddWithValue("@acc_JE_BranchId", journalEntry.acc_JE_BranchId);
            command.Parameters.AddWithValue("@Acc_JE_QuarterId", journalEntry.Acc_JE_Quarterly);
            command.Parameters.AddWithValue("@Acc_JE_Comnments", journalEntry.Acc_JE_Comments);

            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
            command.Parameters.Add(updateOrSaveParam);
            command.Parameters.Add(operParam);

            await command.ExecuteNonQueryAsync();

            return (int)(operParam.Value ?? 0);
        }

        private async Task<int> SaveTransactionDetails(SqlConnection connection, SqlTransaction transaction, JournalEntry journalEntry)
        {
            using var command = new SqlCommand("spAcc_JETransactions_Details", connection, transaction);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@AJTB_ID", journalEntry.AJTB_ID);
            command.Parameters.AddWithValue("@AJTB_MasID", journalEntry.AJTB_MAsID);
            command.Parameters.AddWithValue("@AJTB_TranscNo", journalEntry.AJTB_TranscNo);
            command.Parameters.AddWithValue("@AJTB_CustId", journalEntry.AJTB_CustId);
            command.Parameters.AddWithValue("@AJTB_ScheduleTypeid", journalEntry.AJTB_ScheduleTypeid);
            command.Parameters.AddWithValue("@AJTB_Deschead", journalEntry.AJTB_Deschead);
            command.Parameters.AddWithValue("@AJTB_Desc", journalEntry.AJTB_Desc);
            command.Parameters.AddWithValue("@AJTB_Debit", journalEntry.AJTB_Debit);
            command.Parameters.AddWithValue("@AJTB_Credit", journalEntry.AJTB_Credit);
            command.Parameters.AddWithValue("@AJTB_CreatedBy", journalEntry.AJTB_CreatedBy);
            command.Parameters.AddWithValue("@AJTB_CreatedOn", journalEntry.AJTB_CreatedOn);
            command.Parameters.AddWithValue("@AJTB_UpdatedBy", journalEntry.AJTB_UpdatedBy);
            command.Parameters.AddWithValue("@AJTB_Status", journalEntry.AJTB_Status);
            command.Parameters.AddWithValue("@AJTB_IPAddress", journalEntry.AJTB_IPAddress);
            command.Parameters.AddWithValue("@AJTB_CompID", journalEntry.AJTB_CompID);
            command.Parameters.AddWithValue("@AJTB_YearID", journalEntry.AJTB_YearID);
            command.Parameters.AddWithValue("@AJTB_BillType", journalEntry.AJTB_BillType);
            command.Parameters.AddWithValue("@AJTB_DescName", journalEntry.AJTB_DescName);
            command.Parameters.AddWithValue("@AJTB_BranchId", journalEntry.AJTB_BranchId);
            command.Parameters.AddWithValue("@AJTB_QuarterId", journalEntry.AJTB_QuarterId);

            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
            command.Parameters.Add(updateOrSaveParam);
            command.Parameters.Add(operParam);

            await command.ExecuteNonQueryAsync();

            return (int)(operParam.Value ?? 0);
        }

        private async Task UpdateJeDet(SqlConnection connection, SqlTransaction transaction, int compId, int yearId, int id, int custId, int transId, double transAmt, int branchId, double transDbAmt, double transCrAmt, int durationId)
        {
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
                // ✅ Read first, store in variables
                double currentDebit = reader["ATBU_Closing_TotalDebit_Amount"] != DBNull.Value ? Convert.ToDouble(reader["ATBU_Closing_TotalDebit_Amount"]) : 0;
                double currentCredit = reader["ATBU_Closing_TotalCredit_Amount"] != DBNull.Value ? Convert.ToDouble(reader["ATBU_Closing_TotalCredit_Amount"]) : 0;

                await reader.CloseAsync(); // ✅ Now it's safe to close

                string updateSql;
                double amount;

                if (transId == 0) // Debit
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
                else // Credit
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

        private async Task<string> GenerateTransactionNo(
     SqlConnection connection,
     SqlTransaction transaction,
     int compId,
     int yearId,
     int party,
     int durationId,
     int branchId)
        {
            var sql = @"
        SELECT ISNULL(COUNT(*), 0) + 1
        FROM Acc_JE_Master
        WHERE Acc_JE_YearID = @YearId
          AND Acc_JE_Party = @Party
          AND Acc_JE_QuarterId = @QuarterId
          AND acc_JE_BranchId = @BranchId";

            using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@YearId", yearId);
            command.Parameters.AddWithValue("@Party", party);
            command.Parameters.AddWithValue("@QuarterId", durationId);
            command.Parameters.AddWithValue("@BranchId", branchId);

            int nextNo = Convert.ToInt32(await command.ExecuteScalarAsync());

            // ✅ Pad with leading zeros → TR001, TR002, TR010
            return $"TR{nextNo:D3}";
        }


        #endregion
    }

    #region Supporting Classes

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string Error { get; set; } = string.Empty;
    }

    public class JournalEntryUploadRequest
    {
        public int CustomerId { get; set; }
        public int FinancialYearId { get; set; }
        public int BranchId { get; set; }
        public int DurationId { get; set; }
        public List<JournalEntryRow> Rows { get; set; } = new List<JournalEntryRow>();
    }

    public class JournalEntryRow
    {
        public string? JE_Type { get; set; }
        public string? Account { get; set; }
        public string? Transaction_Date { get; set; }
        public string? Bill_No { get; set; }
        public string? Narration { get; set; }
        public string? Comments { get; set; }
        public double? Debit { get; set; }
        public double? Credit { get; set; }
    }



    public static class LinqExtensions
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            var batch = new List<T>();
            foreach (var item in source)
            {
                batch.Add(item);
                if (batch.Count >= batchSize)
                {
                    yield return batch;
                    batch = new List<T>();
                }
            }
            if (batch.Any())
                yield return batch;
        }
    }

    // Supporting classes for the stored procedures
    public class TrailBalanceUpload
    {
        public int ATBU_ID { get; set; }
        public string ATBU_CODE { get; set; }
        public string ATBU_Description { get; set; }
        public int ATBU_CustId { get; set; }
        public double ATBU_Opening_Debit_Amount { get; set; }
        public double ATBU_Opening_Credit_Amount { get; set; }
        public double ATBU_TR_Debit_Amount { get; set; }
        public double ATBU_TR_Credit_Amount { get; set; }
        public double ATBU_Closing_Debit_Amount { get; set; }
        public double ATBU_Closing_Credit_Amount { get; set; }
        public string ATBU_DELFLG { get; set; }
        public int ATBU_CRBY { get; set; }
        public string ATBU_STATUS { get; set; }
        public int ATBU_UPDATEDBY { get; set; }
        public string ATBU_IPAddress { get; set; }
        public int ATBU_CompId { get; set; }
        public int ATBU_YEARId { get; set; }
        public int ATBU_Branchname { get; set; }
        public int ATBU_QuaterId { get; set; }

        // Details properties
        public int ATBUD_ID { get; set; }
        public int ATBUD_Masid { get; set; }
        public string ATBUD_CODE { get; set; }
        public string ATBUD_Description { get; set; }
        public int ATBUD_CustId { get; set; }
        public int ATBUD_SChedule_Type { get; set; }
        public int ATBUD_Branchname { get; set; }
        public int ATBUD_iQuarterly { get; set; }
        public int ATBUD_Company_Type { get; set; }
        public int ATBUD_Headingid { get; set; }
        public int ATBUD_Subheading { get; set; }
        public int ATBUD_itemid { get; set; }
        public int ATBUD_Subitemid { get; set; }
        public string ATBUD_DELFLG { get; set; }
        public int ATBUD_CRBY { get; set; }
        public int ATBUD_UPDATEDBY { get; set; }
        public string ATBUD_STATUS { get; set; }
        public string ATBUD_Progress { get; set; }
        public string ATBUD_IPAddress { get; set; }
        public int ATBUD_CompId { get; set; }
        public int ATBUD_YEARId { get; set; }
    }

    public class JournalEntry
    {
        public int Acc_JE_ID { get; set; }
        public string AJTB_TranscNo { get; set; }
        public int Acc_JE_Party { get; set; }
        public int Acc_JE_Location { get; set; }
        public int Acc_JE_BillType { get; set; }
        public string Acc_JE_BillNo { get; set; }
        public DateTime Acc_JE_BillDate { get; set; }
        public decimal Acc_JE_BillAmount { get; set; }
        public decimal Acc_JE_AdvanceAmount { get; set; }
        public string Acc_JE_AdvanceNaration { get; set; }
        public decimal Acc_JE_BalanceAmount { get; set; }
        public decimal Acc_JE_NetAmount { get; set; }
        public string Acc_JE_PaymentNarration { get; set; }
        public string Acc_JE_ChequeNo { get; set; }
        public DateTime Acc_JE_ChequeDate { get; set; }
        public string Acc_JE_IFSCCode { get; set; }
        public string Acc_JE_BankName { get; set; }
        public string Acc_JE_BranchName { get; set; }
        public int Acc_JE_CreatedBy { get; set; }
        public int Acc_JE_YearID { get; set; }
        public int Acc_JE_CompID { get; set; }
        public string Acc_JE_Status { get; set; }
        public string Acc_JE_Operation { get; set; }
        public string Acc_JE_IPAddress { get; set; }
        public DateTime Acc_JE_BillCreatedDate { get; set; }
        public int acc_JE_BranchId { get; set; }
        public string Acc_JE_Comments { get; set; }
        public int Acc_JE_Quarterly { get; set; }

        // Transaction details properties
        public int AJTB_ID { get; set; }
        public int AJTB_MAsID { get; set; }
        public int AJTB_CustId { get; set; }
        public int AJTB_Deschead { get; set; }
        public int AJTB_Desc { get; set; }
        public string AJTB_DescName { get; set; }
        public decimal AJTB_Debit { get; set; }
        public decimal AJTB_Credit { get; set; }
        public int AJTB_CreatedBy { get; set; }
        public int AJTB_UpdatedBy { get; set; }
        public string AJTB_Status { get; set; }
        public int AJTB_YearID { get; set; }
        public int AJTB_CompID { get; set; }
        public int AJTB_BillType { get; set; }
        public string AJTB_IPAddress { get; set; }
        public int AJTB_BranchId { get; set; }
        public int AJTB_QuarterId { get; set; }
        public int AJTB_ScheduleTypeid { get; set; }
        public DateTime AJTB_CreatedOn { get; set; }
    }
}



#endregion