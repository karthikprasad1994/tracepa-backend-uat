using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.Audit;
using TracePca.Interface.FIN_Statement;

using TracePca.Service.FIN_statement;

using static TracePca.Dto.FIN_Statement.ScheduleMappingDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleMappingController : ControllerBase
    {
        private ScheduleMappingInterface _ScheduleMappingInterface;
        private ScheduleMappingInterface _ScheduleMappingService;

        public ScheduleMappingController(ScheduleMappingInterface ScheduleMappingInterface)
        {
            _ScheduleMappingInterface = ScheduleMappingInterface;
            _ScheduleMappingService = ScheduleMappingInterface;
        }

        //GetScheduleHeading
        [HttpGet("GetScheduleHeading")]
        public async Task<IActionResult> GetScheduleHeadings(string DBName, int CompId, int CustId, int ScheduleTypeId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetScheduleHeadingAsync(DBName, CompId, CustId, ScheduleTypeId);

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
                var result = await _ScheduleMappingService.GetScheduleSubHeadingAsync(DBName, CompId, CustId, ScheduleTypeId);

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
                        id = item.ASSH_ID,
                        name = item.ASSH_Name
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
                var result = await _ScheduleMappingService.GetScheduleItemAsync(DBName, CompId, CustId, ScheduleTypeId);

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
                var result = await _ScheduleMappingService.GetScheduleSubItemAsync(DBName, CompId, CustId, ScheduleTypeId);

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

        //GetTotalAmount
        [HttpGet("GetTotalAmount")]
        public async Task<IActionResult> GetCustCOAMasterDetails(string DBName,
        int CompId, int CustId, int YearId, int BranchId, int DurationId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetCustCOAMasterDetailsAsync(DBName, CompId, CustId, YearId, BranchId, DurationId);

                if (result != null && result.Any())
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Data fetched successfully",
                        data = result
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No data found",
                        data = Enumerable.Empty<CustCOASummaryDto>()
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching data",
                    error = ex.Message
                });
            }
        }

        //GetTrialBalance(Grid)
        [HttpGet("GetTrailBalance(Grid)")]
        public async Task<IActionResult> GetCustCOADetails(string DBName,
    int CompId, int CustId, int YearId, int ScheduleTypeId, int Unmapped, int BranchId, int DurationId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetCustCOADetailsAsync(DBName, CompId, CustId, YearId, ScheduleTypeId, Unmapped, BranchId, DurationId);

                if (result != null && result.Any())
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Data fetched successfully",
                        data = result
                    });
                }

                return NotFound(new
                {
                    statusCode = 404,
                    message = "No data found",
                    data = Enumerable.Empty<CustCOADetailsDto>()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching data",
                    error = ex.Message
                });
            }
        }

        //FreezeForPreviousDuration
        [HttpPost("FreezePreviousDurationTrialBalance")]
        public async Task<IActionResult> FreezePreviousYearTrialBalance([FromQuery] string DBName, [FromBody] FreezePreviousDurationRequestDto input)
        {
            try
            {
                var detailIds = await _ScheduleMappingService.FreezePreviousYearTrialBalanceAsync(DBName, input);

                return Ok(new
                {
                    status = 200,
                    message = "Previous year trial balance frozen successfully.",
                    data = new
                    {
                        DetailIds = detailIds
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while freezing previous year trial balance.",
                    error = ex.Message
                });
            }
        }

        //FreezrForNextDuration
        [HttpPost("FreezeNextDuratiionrialBalance")]
        public async Task<IActionResult> FreezeNextDurationTrialBalance(string DBName, [FromBody] FreezeNextDurationRequestDto input)
        {
            try
            {
                var detailIds = await _ScheduleMappingService.FreezeNextDurationrialBalanceAsync(DBName, input);

                return Ok(new
                {
                    status = 200,
                    message = "Previous year trial balance frozen successfully.",
                    data = new
                    {
                        DetailIds = detailIds
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while freezing previous year trial balance.",
                    error = ex.Message
                });
            }
        }

        //DownloadUploadableExcelAndTemplate
        [HttpGet("DownloadableExcelFile")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public IActionResult DownloadExcelTemplate()
        {
            var result = _ScheduleMappingService.GetExcelTemplate();

            if (result.FileBytes == null)
                return NotFound("File not found.");

            return File(result.FileBytes, result.ContentType, result.FileName);
        }

        //SaveTrailBalanceDetails
        [HttpPost("SaveTrailBalanceDetails")]
        public async Task<IActionResult> SaveTrailBalanceDetails([FromQuery] string DBName, [FromQuery] int CompId, [FromBody] List<TrailBalanceDetailsDto> HeaderDtos)
        {
            try
            {
                var resultIds = await _ScheduleMappingService.SaveTrailBalanceDetailsAsync(DBName, CompId, HeaderDtos);

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

        //UpdateTrailBalance
        [HttpPost("UpdateTrailBalance")]
        public async Task<IActionResult> UpdateTrailBalance([FromQuery] string DBName, [FromBody] List<UpdateTrailBalanceDto> dtos)
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

                var resultIds = await _ScheduleMappingService.UpdateTrailBalanceAsync(DBName, dtos);

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

        //LoadSubHeadingByHeadingDto
        [HttpGet("subheadings")]
        public async Task<IActionResult> GetSubHeadingsByHeadingIdAsync([FromQuery]string DBName, [FromQuery] int headingId, [FromQuery] int orgType)
        {
            try
            {
                var result = await _ScheduleMappingService.GetSubHeadingsByHeadingIdAsync(DBName, headingId, orgType);

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

        //LoadItemBySubHeadingDto
        [HttpGet("items")]
        public async Task<IActionResult> GetItemsBySubHeadingIdAsync([FromQuery] string DBName, [FromQuery] int subHeadingId, [FromQuery] int orgType)
        {
            try
            {
                var result = await _ScheduleMappingService.GetItemsBySubHeadingIdAsync(DBName, subHeadingId, orgType);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No items found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Items fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving items.",
                    error = ex.Message
                });
            }
        }

        //LoadSubItemByItemDto
        [HttpGet("subitems")]
        public async Task<IActionResult> GetSubItemsByItemIdAsync([FromQuery] string DBName, [FromQuery] int itemId, [FromQuery] int orgType)
        {
            try
            {
                var result = await _ScheduleMappingService.GetSubItemsByItemIdAsync(DBName, itemId, orgType);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No subitems found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Subitems fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving subitems.",
                    error = ex.Message
                });
            }
        }

        //GetPreviousLoadId
        [HttpPost("GetPreviousLoadId")]
        public async Task<IActionResult> GetPreviousLoadIdAsync([FromQuery] string DBName, [FromBody] HierarchyRequestDto request)
        {
            if (request.SubItemId is null && request.ItemId is null && request.SubHeadingId is null)
                return BadRequest("At least one of SubItemId, ItemId, or SubHeadingId must be provided.");

            var (headingId, subHeadingId, itemId) = await _ScheduleMappingService.GetPreviousLoadIdAsync(DBName,
                request.SubItemId, request.ItemId, request.SubHeadingId);

            var response = new HierarchyResponseDto
            {
                HeadingId = headingId,
                SubHeadingId = subHeadingId,
                ItemId = itemId
            };
            return Ok(response);
        }
    }
}




