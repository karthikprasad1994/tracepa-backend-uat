using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.JournalEntryDto;

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
        public async Task<IActionResult> GetJournalEntryInformation([FromQuery] int CompId,
        [FromQuery] int UserId,
        [FromQuery] string Status,
        [FromQuery] int CustId,
        [FromQuery] int YearId,
        [FromQuery] int BranchId,
        [FromQuery] string DateFormat,
        [FromQuery] int DurationId)
        {
            try
            {
                var result = await _JournalEntryService.GetJournalEntryInformationAsync(CompId, UserId, Status, CustId, YearId, BranchId, DateFormat, DurationId);

                return Ok(new
                {
                    status = 200,
                    message = result.Any() ? "Journal entries retrieved successfully." : "No journal entries found.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while retrieving journal entries.",
                    error = ex.Message
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
                var result = await _JournalEntryService.SaveGeneralLedgerAsync(CompId, dtos);

                if (result == null || result.Length == 0)
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "Failed to upload General Ledger."
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "General Ledger uploaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while uploading General Ledger.",
                    error = ex.Message
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
        [HttpGet("GetJETypeDropDownDetails")]
        public async Task<IActionResult> GetJETypeDropDownDetails([FromQuery] int compId, [FromQuery] int custId, [FromQuery] int yearId, [FromQuery] int BranchId, int jetype)
        {
            try
            {
                var result = await _JournalEntryService.GetJETypeDropDownDetailsAsync(compId, custId, yearId, BranchId, jetype);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No journal entry information found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Journal entry information loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching journal entry information.",
                    error = ex.Message
                });
            }
        }

    }
}
