using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.JournalEntryDto;
//using static TracePca.Dto.FIN_Statement.LedgerMaterialityDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class JournalEntryController : ControllerBase
    {
        private JournalEntryInterface _JournalEntryInterface;
        private JournalEntryInterface _JournalEntryService;
        public JournalEntryController(JournalEntryInterface JournalEntryInterface)
        {
            _JournalEntryInterface = JournalEntryInterface;
            _JournalEntryService = JournalEntryInterface;
        }

        //GetJournalEntryInformation
        [HttpGet("GetJournalEntryInformation")]
        public async Task<IActionResult> GetJournalEntryInformation(
        [FromQuery] int CompId,
        [FromQuery] int UserId,
        [FromQuery] string Status,
        [FromQuery] int CustId,
        [FromQuery] int YearId,
        [FromQuery] int BranchId,
        [FromQuery] string DateFormat,
        [FromQuery] int DurationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string searchText = "", // ✅ Added search parameter
        [FromQuery] string sortBy = "Id", // ✅ Added sort field parameter
        [FromQuery] string sortOrder = "ASC") // ✅ Added sort order parameter
        {
            try
            {
                // Validate pagination parameters
                if (page < 1)
                {
                    return BadRequest(new
                    {
                        status = 400,
                        message = "Page number must be greater than 0.",
                        data = (object)null
                    });
                }

                if (pageSize < 1 || pageSize > 100)
                {
                    return BadRequest(new
                    {
                        status = 400,
                        message = "Page size must be between 1 and 100.",
                        data = (object)null
                    });
                }

                // Validate sort parameters
                var validSortColumns = new[] { "Id", "TransactionNo", "BillDate", "BillType", "Status", "Debit", "Credit" };
                if (!validSortColumns.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest(new
                    {
                        status = 400,
                        message = $"Invalid sort column. Valid columns are: {string.Join(", ", validSortColumns)}",
                        data = (object)null
                    });
                }

                sortOrder = sortOrder.ToUpper() == "DESC" ? "DESC" : "ASC";

                var result = await _JournalEntryService.GetJournalEntryInformationAsync(
                    CompId, UserId, Status, CustId, YearId, BranchId,
                    DateFormat, DurationId, page, pageSize, searchText, sortBy, sortOrder);

                return Ok(new
                {
                    status = 200,
                    message = result.Data.Any() ? "Journal entries retrieved successfully." : "No journal entries found.",
                    data = new
                    {
                        entries = result.Data,
                        pagination = new
                        {
                            currentPage = result.CurrentPage,
                            pageSize = result.PageSize,
                            totalCount = result.TotalCount,
                            totalPages = result.TotalPages,
                            hasPreviousPage = result.HasPreviousPage,
                            hasNextPage = result.HasNextPage
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                // Log the exception here if you have logging configured
                // _logger.LogError(ex, "Error retrieving journal entries");

                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while retrieving journal entries.",
                    error = ex.Message
                    // In production, you might want to hide the full exception message:
                    // error = "Internal server error. Please contact administrator."
                });
            }
        }

        //GetExistingJournalVouchers
        [HttpGet("GetExistingVoucherNos")]
        public async Task<IActionResult> GetExistingVoucherNos([FromQuery] int compId, [FromQuery] int yearId, [FromQuery] int partyId, [FromQuery] int branchId)
        {
            try
            {
                var result = await _JournalEntryService.LoadExistingVoucherNosAsync(compId, yearId, partyId, branchId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No existing voucher numbers found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Existing voucher numbers loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching existing voucher numbers.",
                    error = ex.Message
                });
            }
        }

        //GetJEType
        [HttpGet("GetGeneralMasters")]
        public async Task<IActionResult> LoadGeneralMasters([FromQuery] int compId, [FromQuery] string type)
        {
            try
            {
                var result = await _JournalEntryService.LoadGeneralMastersAsync(compId, type);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No records found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "General masters loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while loading general masters.",
                    error = ex.Message
                });
            }
        }

        //GetHeadOfAccounts
        [HttpGet("LoadDeschead")]
        public async Task<IActionResult> LoadDeschead([FromQuery] int compId, [FromQuery] int custId, [FromQuery] int yearId, [FromQuery] int branchId, [FromQuery] int durationId)
        {
            try
            {
                var result = await _JournalEntryService.LoadDescheadAsync(compId, custId, yearId, branchId, durationId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "Description head not found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Description head loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while loading description head.",
                    error = ex.Message
                });
            }
        }

        //GetGeneralLedger
        [HttpGet("LoadSubGLDetails")]
        public async Task<IActionResult> LoadSubGLDetails([FromQuery] int compId, [FromQuery] int custId)
        {
            try
            {
                var result = await _JournalEntryService.LoadSubGLDetailsAsync(compId, custId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "Sub GL details not found.",
                        data = (object)null
                    });
                }
                return Ok(new
                {
                    statusCode = 200,
                    message = "Sub GL details loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while loading Sub GL details.",
                    error = ex.Message
                });
            }
        }

        //SaveOrUpdateTransactionDetails
        [HttpPost("SaveTransactionDetails")]
        public async Task<IActionResult> SaveJournalEntryWithTransactions([FromBody] List<SaveJournalEntryWithTransactionsDto> dtos)
        {
            if (dtos == null)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Invalid input: DTO is null",
                    Data = (object)null
                });
            }
            try
            {
                var isUpdate = dtos[0].Acc_JE_ID > 0;
                var result = await _JournalEntryService.SaveJournalEntryWithTransactionsAsync(dtos);

                string successMessage = isUpdate
                    ? "Schedule Heading successfully updated."
                    : "Schedule Heading successfully created.";

                return Ok(new
                {
                    Status = 200,
                    Message = successMessage,
                    Data = new
                    {
                        UpdateOrSave = result[0],
                        Oper = result[1],
                        IsUpdate = isUpdate
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        //SaveGeneralLedger
        [HttpPost("SaveGeneralLedger")]
        public async Task<IActionResult> SaveGeneralLedger([FromQuery] int CompId, [FromBody] List<GeneralLedgerDto> dtos)
        {
            try
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "No data provided for upload."
                    });
                }

                var result = await _JournalEntryService.SaveGeneralLedgerAsync(CompId, dtos);

                // Check the result type
                if (result is object dynamicResult)
                {
                    var status = dynamicResult.GetType().GetProperty("Status")?.GetValue(dynamicResult)?.ToString();
                    var message = dynamicResult.GetType().GetProperty("Message")?.GetValue(dynamicResult)?.ToString();

                    if (status == "PartialSuccess")
                    {
                        return Ok(new
                        {
                            statusCode = 200,
                            message = message,
                            data = dynamicResult
                        });
                    }
                    else if (status == "Failed")
                    {
                        return BadRequest(new
                        {
                            statusCode = 400,
                            message = message,
                            data = dynamicResult
                        });
                    }
                    else if (status == "Error")
                    {
                        return StatusCode(500, new
                        {
                            statusCode = 500,
                            message = message,
                            error = dynamicResult.GetType().GetProperty("Error")?.GetValue(dynamicResult),
                            data = dynamicResult
                        });
                    }
                }

                // Fallback for legacy return type (int[])
                if (result is int[] intArray && intArray.Length > 0)
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "General Ledger uploaded successfully.",
                        data = new { updateOrSave = intArray[0], operationId = intArray[1] }
                    });
                }

                return BadRequest(new
                {
                    statusCode = 400,
                    message = "Failed to upload General Ledger. Invalid response from service."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while uploading General Ledger.",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        //ActivateJE
        [HttpPost("ActivateJournalEntries")]
        public async Task<IActionResult> ActivateJournalEntries([FromBody] ActivateRequestDto dto)
        {
            try
            {
                var rowsAffected = await _JournalEntryService.ActivateJournalEntriesAsync(dto);

                if (rowsAffected <= 0)
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No matching entries found to activate."
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Journal Entries activated successfully.",
                    data = new { count = rowsAffected, ids = dto.DescriptionIds }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while activating Journal Entries.",
                    error = ex.Message
                });
            }
        }

        //DeActivatedJE
        //[HttpPost("DeActivatedJE")]
        //public async Task<IActionResult> ApproveJournalEntries([FromBody] ApproveRequestDto dto)
        //{
        //    try
        //    {
        //        var rowsAffected = await _JournalEntryService.ApproveJournalEntriesAsync(dto);

        //        if (rowsAffected <= 0)
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "No records found to approve."
        //            });
        //        }

        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Successfully Approved.",
        //            data = new { count = rowsAffected, ids = dto.DescriptionIds }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while approving Journal Entries.",
        //            error = ex.Message
        //        });
        //    }
        //}

        [HttpGet("{jeId}/{compId}")]
        public async Task<IActionResult> GetJERecord(int jeId, int compId)
        {
            var record = await _JournalEntryService.GetJERecordAsync(jeId, compId);
            if (record == null)
                return NotFound(new { message = "Record not found" });

            return Ok(record);
        }
        [HttpGet("GetTransactionDetails")]
        public async Task<IActionResult> GetTransactionDetails(
    [FromQuery] int companyId,
    [FromQuery] int yearId,
    [FromQuery] int custId,
    [FromQuery] int jeId,
    [FromQuery] int branchId = 0,
    [FromQuery] int durationId = 0)
        {
            var result = await _JournalEntryService.LoadTransactionDetailsAsync(companyId, yearId, custId, jeId, branchId, durationId);
            return Ok(result);
        }
        [HttpPost("saveJEType")]
        public async Task<IActionResult> SaveOrUpdate([FromBody] AdminMasterDto dto)
        {
            var result = await _JournalEntryService.SaveOrUpdateAsync(dto);
            return Ok(new
            {
                UpdateOrSave = result.UpdateOrSave,
                Oper = result.Oper,
                FinalId = result.FinalId,
                FinalCode = result.FinalCode
            });
        }

        //GetJETypeDropDown
        [HttpGet("GetJETypeDropDown")]
        public async Task<IActionResult> GetJETypeList(int CompId)
        {
            try
            {
                var result = await _JournalEntryService.GetJETypeListAsync(CompId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Journal Entry types found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Journal Entry types loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching Journal Entry types.",
                    error = ex.Message
                });
            }
        }

        //GetJETypeDropDownDetails
        [HttpPost("get-je-dropdown-details")]
        public async Task<IActionResult> GetJETypeDropDownDetails([FromBody] JETypeDropdownRequestDto request)
        {
            try
            {
                // Validate page size (prevent too large requests)
                request.PageSize = Math.Min(request.PageSize, 100); // Max 100 records per page

                var result = await _JournalEntryService.GetJETypeDropDownDetailsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        //SaveJEType
        [HttpPost("SaveOrUpdateContentForJE")]
        public async Task<IActionResult> SaveOrUpdateJEContent(
    [FromBody] CreateJEContentRequestDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "Invalid request payload."
                });
            }

            try
            {
                string result = await _JournalEntryService
                    .SaveOrUpdateContentForJEAsync(
                        dto.cmm_ID,
                        dto.CMM_CompID,
                        dto.cmm_Desc,
                        dto.cms_Remarks,
                        dto.cmm_Category
                    );

                // ✅ EXISTS HANDLING
                if (result == "ALREADY_EXISTS")
                {
                    return Conflict(new   // HTTP 409
                    {
                        statusCode = 409,
                        message = "JE Content already exists."
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = dto.cmm_ID.HasValue && dto.cmm_ID.Value > 0
                        ? "JE Content updated successfully."
                        : "JE Content created successfully.",
                    newCode = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while saving or updating JE content.",
                    error = ex.Message
                });
            }
        }
        [HttpGet("get-Contra-id")]
        public async Task<IActionResult> GetContentMasterId(
            [FromQuery] string desc,
            [FromQuery] int compId)
        {
            var cmmId = await _JournalEntryService.GetContentMasterIdAsync(desc, compId);

            if (cmmId == null)
                return NotFound(new
                {
                    success = false,
                    message = "Record not found",
                    data = (int?)null   // ✅ correct C# way
                });

            return Ok(new
            {
                success = true,
                data = new { cmm_ID = cmmId }
            });
        }

        //[HttpPost("upload-excel")]
        //[RequestSizeLimit(100_000_000)] // 100MB limit
        //public async Task<IActionResult> UploadJournalEntriesExcel([FromForm] JournalEntryUploadDto uploadDto)
        //{
        //    try
        //    {
        //        if (uploadDto.File == null || uploadDto.File.Length == 0)
        //        {
        //            return BadRequest(new { Success = false, Message = "Please select a file to upload" });
        //        }

        //        // Validate file type
        //        var allowedExtensions = new[] { ".xlsx", ".xls" };
        //        var extension = Path.GetExtension(uploadDto.File.FileName).ToLowerInvariant();
        //        if (!allowedExtensions.Contains(extension))
        //        {
        //            return BadRequest(new { Success = false, Message = "Only Excel files are allowed" });
        //        }

        //        // Get user info from context
        //        var accessCodeId = int.Parse(Request.Headers["AccessCodeID"].FirstOrDefault() ?? "1");
        //        var userId = int.Parse(Request.Headers["UserID"].FirstOrDefault() ?? "0");
        //        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

        //        uploadDto.AccessCodeId = accessCodeId;
        //        uploadDto.UserId = userId;

        //        // Process the file
        //        var result = await _JournalEntryService.ProcessJournalEntriesAsync(uploadDto);

        //        if (result.Success)
        //        {
        //            return Ok(new
        //            {
        //                Success = true,
        //                Message = result.Message,
        //                Data = new
        //                {
        //                    result.TotalRecords,
        //                    result.ProcessedRecords,
        //                    result.FailedRecords,
        //                    result.ProcessingTime,
        //                    Errors = result.Errors.Take(10) // Return only first 10 errors
        //                }
        //            });
        //        }
        //        else
        //        {
        //            return BadRequest(new
        //            {
        //                Success = false,
        //                Message = result.Message,
        //                Errors = result.Errors.Take(20) // Return more errors for debugging
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            Success = false,
        //            Message = "Internal server error",
        //            Error = ex.Message
        //        });
        //    }
        //}
    
    }
}
