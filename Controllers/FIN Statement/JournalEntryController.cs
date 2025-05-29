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

        //GetCustomersName
        [HttpGet("GetCustomersName")]
        public async Task<IActionResult> GetCustomerName([FromQuery] int icompId)
        {

            try
            {
                var result = await _JournalEntryService.GetCustomerNameAsync(icompId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No company types found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Company types loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching company types.",
                    error = ex.Message
                });
            }
        }

        //GetFinancialYear
        [HttpGet("GetFinancialYear")]
        public async Task<IActionResult> GetFinancialYear([FromQuery] int icompId)
        {
            try
            {
                var result = await _JournalEntryService.GetFinancialYearAsync(icompId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No company types found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Company types loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching company types.",
                    error = ex.Message
                });
            }
        }

        //GetDuration
        [HttpGet("GetDuration")]
        public async Task<IActionResult> GetDuration([FromQuery] int compId, [FromQuery] int custId)
        {
            try
            {
                var data = await _JournalEntryService.GetDurationAsync(compId, custId);

                if (data == null || !data.Any())
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "No duration found for the given customer and company.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    status = 200,
                    message = "Duration loaded successfully.",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "Internal server error.",
                    error = ex.Message
                });
            }
        }

        //GetBranchName
        [HttpGet("GetBranchName")]
        public async Task<IActionResult> GetBranchName([FromQuery] int icompId, [FromQuery] int icustId)
        {
            try
            {
                var result = await _JournalEntryService.GetBranchNameAsync(icompId, icustId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No company types found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Company types loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching company types.",
                    error = ex.Message
                });
            }
        }

        //GetJournalEntryInformation
        [HttpGet("GetJournalEntryInformation")]
        public async Task<IActionResult> GetJournalEntryInformation(
        [FromQuery] int compId,
        [FromQuery] int userId,
        [FromQuery] string status,
        [FromQuery] int custId,
        [FromQuery] int yearId,
        [FromQuery] int branchId,
        [FromQuery] string dateFormat,
        [FromQuery] int durationId)
        {
            try
            {
                var result = await _JournalEntryService.GetJournalEntryInformationAsync(
                    compId, userId, status, custId, yearId, branchId, dateFormat, durationId);

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
    }
}
