using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.ScheduleMastersDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleMastersController : ControllerBase
    {
        private ScheduleMastersInterface _ScheduleMastersService;

        public ScheduleMastersController(ScheduleMastersInterface ScheduleMastersInterface)
        {
            
            _ScheduleMastersService = ScheduleMastersInterface;
        }

        //GetCustomersName
        [HttpGet("GetCustomersName")]
        public async Task<IActionResult> GetCustomerName([FromQuery] string DBName, [FromQuery] int CompId)
        {

            try
            {
                var result = await _ScheduleMastersService.GetCustomerNameAsync(DBName, CompId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Customer name found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Customer name loaded successfully.",
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
        [HttpGet("GetCustomerDurationId")]
        public async Task<IActionResult> GetCustomerDurationId([FromQuery] string DBName, [FromQuery] int CompId, [FromQuery] int CustId)
        {
            try
            {
                var durationId = await _ScheduleMastersService.GetCustomerDurationIdAsync(DBName, CompId, CustId);

                if (durationId == null)
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "Duration ID not found for the provided Company ID and Customer ID.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Duration ID retrieved successfully.",
                    data = new { Cust_DurtnId = durationId }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving the duration ID.",
                    error = ex.Message
                });
            }
        }

        //GetFinancialYear
        [HttpGet("GetFinancialYear")]
        public async Task<IActionResult> GetFinancialYear([FromQuery] string DBName, [FromQuery] int CompId)
        {
            try
            {
                var result = await _ScheduleMastersService.GetFinancialYearAsync(DBName, CompId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Financial year found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Financial year loaded successfully.",
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
        public async Task<IActionResult> GetBranchName([FromQuery] string DBName, [FromQuery] int CompId, [FromQuery] int CustId)
        {
            try
            {
                var result = await _ScheduleMastersService.GetBranchNameAsync(DBName, CompId, CustId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Branch name found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Branch name loaded successfully.",
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

        //GetScheduleHeading
        [HttpGet("GetScheduleHeading")]
        public async Task<IActionResult> GetScheduleHeadings(string DBName, int CompId, int CustId, int ScheduleTypeId)
        {
            try
            {
                var result = await _ScheduleMastersService.GetScheduleHeadingAsync(DBName, CompId, CustId, ScheduleTypeId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Heading types found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Heading types loaded successfully.",
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

        //GetScheduleSub-Heading
        [HttpGet("GetScheduleSubHeading")]
        public async Task<IActionResult> GetScheduleSubHeading(string DBName, int CompId, int CustId, int ScheduleTypeId)
        {
            try
            {
                var result = await _ScheduleMastersService.GetScheduleItemAsync(DBName, CompId, CustId, ScheduleTypeId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Sub-Heading items found.",
                        data = new List<object>()
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Sub-Heading retrieved successfully.",
                    data = result.Select(item => new
                    {
                        id = item.ASI_ID,
                        name = item.ASI_Name
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving schedule items.",
                    error = ex.Message
                });
            }
        }

        //GetScheduleItem
        [HttpGet("GetScheduleItem")]
        public async Task<IActionResult> GetScheduleItem(string DBName, int CompId, int CustId, int ScheduleTypeId)
        {
            try
            {
                var result = await _ScheduleMastersService.GetScheduleItemAsync(DBName, CompId, CustId, ScheduleTypeId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No schedule items found.",
                        data = new List<object>()
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Schedule items retrieved successfully.",
                    data = result.Select(item => new
                    {
                        id = item.ASI_ID,
                        name = item.ASI_Name
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving schedule items.",
                    error = ex.Message
                });
            }
        }

        //GetScheduleSub-Item
        [HttpGet("GetScheduleSubItem")]
        public async Task<IActionResult> GetScheduleSubItem(string DBName, int CompId, int CustId, int ScheduleTypeId)
        {
            try
            {
                var result = await _ScheduleMastersService.GetScheduleSubItemAsync(DBName, CompId, CustId, ScheduleTypeId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No schedule sub items found.",
                        data = new List<object>()
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Schedule sub items retrieved successfully.",
                    data = result.Select(item => new
                    {
                        id = item.ASSI_ID,
                        name = item.ASSI_Name
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving schedule items.",
                    error = ex.Message
                });
            }
        }

        //GetCustomerOrgType
        [HttpGet("GetCustomerOrgType")]
        public async Task<IActionResult> GetCustomerOrgType([FromQuery] string DBName, [FromQuery] int CustId, [FromQuery] int CompId)
        {
            try
            {
                var result = await _ScheduleMastersService.GetCustomerOrgTypeAsync(DBName, CustId, CompId);

                if (string.IsNullOrWhiteSpace(result))
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "Customer organization type not found.",
                        data = (string)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Customer organization type retrieved successfully.",
                    data = new
                    {
                        orgType = result
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving the organization type.",
                    error = ex.Message
                });
            }
        }
    }
}
