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
        public async Task<IActionResult> GetScheduleHeadings(int CompId, int CustId, int ScheduleTypeId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetScheduleHeadingAsync(CompId, CustId, ScheduleTypeId);

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
        public async Task<IActionResult> GetScheduleSubHeading(int CompId, int CustId, int ScheduleTypeId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetScheduleSubHeadingAsync(CompId, CustId, ScheduleTypeId);

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
        public async Task<IActionResult> GetScheduleItem(int CompId, int CustId, int ScheduleTypeId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetScheduleItemAsync(CompId, CustId, ScheduleTypeId);

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
        public async Task<IActionResult> GetScheduleSubItem(int CompId, int CustId, int ScheduleTypeId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetScheduleSubItemAsync(CompId, CustId, ScheduleTypeId);

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
        public async Task<IActionResult> GetCustCOAMasterDetails(int CompId, int CustId, int YearId, int BranchId, int DurationId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetCustCOAMasterDetailsAsync(CompId, CustId, YearId, BranchId, DurationId);

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
        public async Task<IActionResult> GetCustCOADetails(int CompId, int CustId, int YearId, int ScheduleTypeId, int Unmapped, int BranchId, int DurationId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetCustCOADetailsAsync(CompId, CustId, YearId, ScheduleTypeId, Unmapped, BranchId, DurationId);

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
        public async Task<IActionResult> FreezePreviousDurationTrialBalance([FromBody] List<FreezePreviousYearTrialBalanceDto> inputList)
        {
            try
            {
                var detailIds = await _ScheduleMappingService.FreezePreviousDurationTrialBalanceAsync(inputList);

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
        [HttpPost("FreezeNextDurationTrialBalance")]
        public async Task<IActionResult> FreezeNextDurationTrialBalance([FromBody] List<FreezeNextYearTrialBalanceDto> inputList)
        {
            try
            {
                if (inputList == null || !inputList.Any())
                {
                    return BadRequest(new
                    {
                        status = 400,
                        message = "No data provided to freeze trial balance."
                    });
                }

                var detailIds = await _ScheduleMappingService.FreezeNextDurationTrialBalanceAsync(inputList);

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

        //CheckTrailBalanceRecordExists 
        [HttpGet("CheckTrailBalanceRecordExists")]
        public async Task<IActionResult> CheckTrailBalanceRecordExists(int CompId, int CustId, int YearId, int BranchId, int QuarterId)
        {
            try
            {
                bool exists = await _ScheduleMappingService.CheckTrailBalanceRecordExistsAsync(CompId, CustId, YearId, BranchId, QuarterId);

                return Ok(new
                {
                    statusCode = 200,
                    message = exists ? "Trail Balance record already exists." : "No duplicate found. Safe to proceed.",
                    data = new { exists }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while checking the trail balance record.",
                    error = ex.Message
                });
            }
        }


        //SaveTrailBalanceDetails
        [HttpPost("SaveTrailBalanceDetails")]
        public async Task<IActionResult> SaveTrailBalanceDetails([FromQuery] int CompId, [FromBody] List<TrailBalanceDetailsDto> HeaderDtos)
        {
            try
            {
                var resultIds = await _ScheduleMappingService.SaveTrailBalanceDetailsAsync(CompId, HeaderDtos);

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
        public async Task<IActionResult> UpdateTrailBalance([FromBody] List<UpdateTrailBalanceDto> dtos)
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

                var resultIds = await _ScheduleMappingService.UpdateTrailBalanceAsync(dtos);

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
        public async Task<IActionResult> GetSubHeadingsByHeadingIdAsync([FromQuery] int headingId, [FromQuery] int orgType)
        {
            try
            {
                var result = await _ScheduleMappingService.GetSubHeadingsByHeadingIdAsync(headingId, orgType);

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
        public async Task<IActionResult> GetItemsBySubHeadingIdAsync([FromQuery] int subHeadingId, [FromQuery] int orgType)
        {
            try
            {
                var result = await _ScheduleMappingService.GetItemsBySubHeadingIdAsync(subHeadingId, orgType);

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
        public async Task<IActionResult> GetSubItemsByItemIdAsync([FromQuery] int itemId, [FromQuery] int orgType)
        {
            try
            {
                var result = await _ScheduleMappingService.GetSubItemsByItemIdAsync(itemId, orgType);

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
        public async Task<IActionResult> GetPreviousLoadIdAsync([FromBody] HierarchyRequestDto request)
        {
            if (request.SubItemId is null && request.ItemId is null && request.SubHeadingId is null)
                return BadRequest("At least one of SubItemId, ItemId, or SubHeadingId must be provided.");

            var (headingId, subHeadingId, itemId) = await _ScheduleMappingService.GetPreviousLoadIdAsync(request.SubItemId, request.ItemId, request.SubHeadingId);

            var response = new HierarchyResponseDto
            {
                HeadingId = headingId,
                SubHeadingId = subHeadingId,
                ItemId = itemId
            };
            return Ok(response);
        }

        //UpdateNetIncome
        [HttpPost("UpdateNetIncome")]
        public async Task<IActionResult> UpdateNetIncome([FromBody] UpdateNetIncomeRequestDto request)
        {
            try
            {
                var result = await _ScheduleMappingService.UpdateNetIncomeAsync(
                    request.CompId,
                    request.CustId,
                    request.UserId,
                    request.YearId,
                    request.BranchId,
                    request.DurationId
                );

                return Ok(new
                {
                    statusCode = 200,
                    message = result ? "Net Income inserted successfully." : "Net Income already exists.",
                    success = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while processing Net Income.",
                    error = ex.Message
                });
            }
        }

        //SaveMappingTransactionDetails
        [HttpPost("SaveMappingTransactionDetails")]
        public async Task<IActionResult> SaveMappingTransactionDetails([FromBody] SaveMappingTransactionDetailsDto dto)
        {
            if (dto == null)
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
                var isUpdate = dto.Acc_JE_ID > 0;  // ✅ FIXED — no [0]
                var result = await _ScheduleMappingService.SaveMappingTransactionDetailsAsync(dto);

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

        //GetCustomerTrailBalance
        [HttpGet("GetCustomerTB")]
        public async Task<IActionResult> GetCustomerTB([FromQuery] int compId,[FromQuery] int yearId,[FromQuery] int custId,[FromQuery] int orgType)
        {
            try
            {
                var result = await _ScheduleMappingService.GetCustomerTBAsync(
                    compId,
                    yearId,
                    custId,
                    orgType
                );

                return Ok(new
                {
                    statusCode = 200,
                    message = "Customer trial balance loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    statusCode = 500,
                    message = ex.Message
                });
            }
        }

        //GetVODTotalGrid
        [HttpGet("GetVODTotalGrid")]
        public async Task<IActionResult> GetCustCOAMasterDetailsCustomer(
      [FromQuery] int compId,[FromQuery] int custId,[FromQuery] int yearId,[FromQuery] int scheduleTypeId,[FromQuery] int unmapped,[FromQuery] int branchId)
        {
            try
            {
                var result = await _ScheduleMappingService
                    .GetCustCOAMasterDetailsCustomerAsync(
                        compId,custId,yearId,scheduleTypeId,unmapped,branchId
                    );

                return Ok(new
                {
                    statusCode = 200,
                    message = "Customer COA details loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    statusCode = 500,
                    message = ex.Message
                });
            }
        }
    }
}




