using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.ScheduleReportDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleReportController : ControllerBase
    {
        private ScheduleReportInterface _ScheduleReportInterface;
        private ScheduleReportInterface _ScheduleReportService;

        public ScheduleReportController(ScheduleReportInterface ScheduleReportInterface)
        {
            _ScheduleReportInterface = ScheduleReportInterface;
            _ScheduleReportService = ScheduleReportInterface;
        }

        //GetCustomersName
        [HttpGet("GetCustomersName")]
        public async Task<IActionResult> GetCustomerName([FromQuery] int icompId)
        {

            try
            {
                var result = await _ScheduleReportService.GetCustomerNameAsync(icompId);

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
                var result = await _ScheduleReportService.GetFinancialYearAsync(icompId);

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

        //GetBranchName
        [HttpGet("GetBranchName")]
        public async Task<IActionResult> GetBranchName([FromQuery] int icompId, [FromQuery] int icustId)
        {
            try
            {
                var result = await _ScheduleReportService.GetBranchNameAsync(icompId, icustId);

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

        //GetCompanyName
        [HttpGet("GetCompanyName")]
        public async Task<IActionResult> GetCopanyName([FromQuery] int iCompId)
        {
            try
            {
                var result = await _ScheduleReportService.GetCompanyNameAsync(iCompId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No company details found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Company details loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching company details.",
                    error = ex.Message
                });
            }
        }

        //GetPartner
        [HttpGet("LoadCustomerPartners")]
        public async Task<ActionResult<IEnumerable<PartnersDto>>> LoadCustomerPartners(int compId, int detailsId)
        {
            try
            {
                var result = await _ScheduleReportService.LoadCustomerPartnersAsync(compId, detailsId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // optionally log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //GetSubHeading
        [HttpGet("GetSubHeading")]
        public async Task<IActionResult> GetSubHeading([FromQuery] int iCompId, [FromQuery] int iScheduleId, [FromQuery] int iCustId, [FromQuery] int iHeadingId)
        {
            try
            {
                if (iHeadingId <= 0)
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "HeadingId is required and must be greater than zero.",
                        data = (object)null
                    });
                }

                var result = await _ScheduleReportService.GetSubHeadingAsync(
                    iCompId, iScheduleId, iCustId, iHeadingId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No subheadings found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Subheadings fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving subheadings.",
                    error = ex.Message
                });
            }
        }

        //Getitem
        [HttpGet("GetItem")]
        public async Task<IActionResult> GetItem(
        [FromQuery] int iCompId, [FromQuery] int iScheduleId, [FromQuery] int iCustId, [FromQuery] int iHeadingId, [FromQuery] int iSubHeadId)
        {
            try
            {
                // Call service method
                var result = await _ScheduleReportService.GetItemAsync(
                    iCompId, iScheduleId, iCustId, iHeadingId, iSubHeadId);

                // Check for no data
                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No schedule items found.",
                        data = (object)null
                    });
                }

                // Return success
                return Ok(new
                {
                    statusCode = 200,
                    message = "Schedule items fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving schedule items.",
                    error = ex.Message
                });
            }
        }

        //GetDateFormat
        [HttpGet("GetDateFormatSelection")]
        public async Task<IActionResult> GetDateFormatSelection([FromQuery] int companyId, [FromQuery] string configKey)
        {
            if (string.IsNullOrEmpty(configKey) || companyId <= 0)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Invalid parameters: companyId and configKey are required.",
                    Data = (object)null
                });
            }

            try
            {
                var result = await _ScheduleReportService.GetDateFormatSelectionAsync(companyId, configKey);

                return Ok(new
                {
                    Status = 200,
                    Message = "Date format selection retrieved successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Message = "An error occurred while retrieving date format selection.",
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        //LoadButton
        [HttpGet("Load")]
        public async Task<IActionResult> LoadReport(
    [FromQuery] int reportType,
    [FromQuery] int scheduleTypeId,
    [FromQuery] int accountId,
    [FromQuery] int customerId,
    [FromQuery] int yearId)
        {
            if (reportType <= 0 || scheduleTypeId <= 0 || accountId <= 0 || customerId <= 0 || yearId <= 0)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Invalid parameters: all values are required and must be greater than zero.",
                    Data = (object)null
                });
            }

            try
            {
                var result = await _ScheduleReportService.GenerateReportAsync(
                    reportType, scheduleTypeId, accountId, customerId, yearId);

                return Ok(new
                {
                    Status = 200,
                    Message = "Report loaded successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Message = "An error occurred while loading the report.",
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }
    }
}
