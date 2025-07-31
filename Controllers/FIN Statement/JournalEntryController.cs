using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.JournalEntryDto;
using static TracePca.Dto.FIN_Statement.ScheduleExcelUploadDto;

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
        [HttpGet("GetJEType")]
        public async Task<IActionResult> GetJEType([FromQuery] int compId, [FromQuery] string type)
        {
            try
            {
                var result = await _JournalEntryService.GetJETypeAsync(compId, type);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "JE Type name found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "JE Type  loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching JE Type.",
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

        //SaveTransactionDetails
        [HttpPost("SaveTransactionDetails")]
        public async Task<IActionResult> SaveJournalEntryWithTransactions([FromBody] List<SaveJournalEntryWithTransactionsDto> dtos)
        {
            try
            {
                var result = await _JournalEntryService.SaveJournalEntryWithTransactionsAsync(dtos);
                return Ok(new
                {
                    Message = "Journal Entry saved successfully.",
                    UpdateOrSave = result[0],
                    Oper = result[1]
                });
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return StatusCode(500, new
                {
                    Message = "An error occurred while saving the journal entry.",
                    Error = ex.Message
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
    }
}
