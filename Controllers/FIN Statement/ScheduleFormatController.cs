using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleFormatDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleFormatController : ControllerBase
    {
        private ScheduleFormatInterface _ScheduleFormatInterface;
        private ScheduleFormatInterface _ScheduleFormatService;

        public ScheduleFormatController(ScheduleFormatInterface ScheduleFormatInterface)
        {
            _ScheduleFormatInterface = ScheduleFormatInterface;
            _ScheduleFormatService = ScheduleFormatInterface;
        }

        //GetSecheduleFormat-ClientName
        [HttpGet("GetScheduleFormatClientName")]
        public async Task<IActionResult> GetAllCustomersAsync([FromQuery] int iCompId)
        {
            try
            {
                var result = await _ScheduleFormatService.GetScheduleFormatClientAsync(iCompId); // Pass actual value or remove sAC if unused

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No customers found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Customer list fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving customer data.",
                    error = ex.Message
                });
            }
        }

        //GetScheduleFormat-Heading
        [HttpGet("GetScheduleFormatHeading")]
        public async Task<IActionResult> GetScheduleFormatHeadingAsync([FromQuery] int iCompId, [FromQuery] int iScheduleId, [FromQuery] int iCustId, [FromQuery] int iAccHead)
        {
            try
            {
                var result = await _ScheduleFormatService.GetScheduleFormatHeadingAsync(iCompId, iScheduleId, iCustId, iAccHead);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No schedule format headings found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Schedule format headings fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving schedule format headings.",
                    error = ex.Message
                });
            }
        }

        //GetScheduleFormat-SubHeading
        [HttpGet("GetScheduleFormatSubHeading")]
        public async Task<IActionResult> GetScheduleFormatSubHeadingAsync([FromQuery] int iCompId, [FromQuery] int iScheduleId, [FromQuery] int iCustId, [FromQuery] int iHeadingId)
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

                var result = await _ScheduleFormatService.GetScheduleFormatSubHeadingAsync(
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

        //GetScheduleFormat-ItemUnderSubHeading
        [HttpGet("GetScheduleFormatItemsUnderSubHeading")]
        public async Task<IActionResult> GetScheduleFormatItemsAsync(
        [FromQuery] int iCompId, [FromQuery] int iScheduleId, [FromQuery] int iCustId, [FromQuery] int iHeadingId, [FromQuery] int iSubHeadId)
        {
            try
            {
                // Call service method
                var result = await _ScheduleFormatService.GetScheduleFormatItemsAsync(
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

        //GetScheduleFormat-SubitemsUnderItems
        [HttpGet("GetScheduleFormatSubItems")]
        public async Task<IActionResult> GetScheduleFormatSubItemsAsync(
       [FromQuery] int iCompId, [FromQuery] int iScheduleId, [FromQuery] int iCustId, [FromQuery] int iHeadingId, [FromQuery] int iSubHeadId, [FromQuery] int iItemId)
        {
            try
            {
                var result = await _ScheduleFormatService.GetScheduleFormatSubItemsAsync(
                    iCompId, iScheduleId, iCustId, iHeadingId, iSubHeadId, iItemId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No sub-items found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Sub-items list fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving sub-items.",
                    error = ex.Message
                });
            }
        }

        //GetScheduleTemplate
        [HttpGet("GetScheduleTemplate")]
        public async Task<IActionResult> GetScheduleTemplateAsync(
    [FromQuery] int iCompId,
    [FromQuery] int iScheduleId,
    [FromQuery] int iCustId,
    [FromQuery] int iAccHead)
        {
            try
            {
                var result = await _ScheduleFormatService.GetScheduleTemplateAsync(iCompId, iScheduleId, iCustId, iAccHead);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No schedule template data found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Schedule template data fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving the schedule template.",
                    error = ex.Message
                });
            }
        }

        //
        [HttpDelete("DeleteSchedulesTemplate")]
        public async Task<IActionResult> DeleteScheduleTemplateAsync(int iCompId, int scheduleType, int custId, int selectedValue, int mainId)
        {
            try
            {
                bool result = await _ScheduleFormatService.DeleteScheduleTemplateAsync(iCompId, scheduleType, custId, selectedValue, mainId);

                if (result)
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        Message = "Schedules deleted successfully.",
                        Data = (object)null
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No records found to delete.",
                        Data = (object)null
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while deleting schedules.",
                    Data = ex.Message
                });
            }
        }

        //SaveOrUpdateScheduleFormatHeading
        [HttpPost("SaveOrUpdateScheduleFormatHeading")]
        public async Task<IActionResult> SaveScheduleHeadingAndTemplateAsync([FromQuery] int iCompId, [FromBody] SaveScheduleFormatHeadingDto dto)
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
                bool isUpdate = dto.ASH_ID > 0;

                var result = await _ScheduleFormatService.SaveScheduleHeadingAndTemplateAsync(iCompId, dto);

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

        //SaveScheduleFormatSub-HeadingAndTemplate
        [HttpPost("SaveOrUpdateScheduleFormatSubHeading")]
        public async Task<IActionResult> SaveScheduleSubHeadingAndTemplate([FromQuery] int iCompId, [FromBody] SaveScheduleFormatSub_HeaddingDto dto)
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
                bool isUpdate = dto.ASSH_ID > 0;

                var result = await _ScheduleFormatService.SaveScheduleSubHeadingAndTemplateAsync(iCompId, dto);

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

        //SaveScheduleFormatItemsAndTemplate
        [HttpPost("SaveOrUpdateScheduleFormatItems")]
        public async Task<IActionResult> SaveScheduleItemAndTemplateAsync([FromQuery] int iCompId, [FromBody] SaveScheduleFormatItemDto dto)
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
                bool isUpdate = dto.ASI_ID > 0;

                var result = await _ScheduleFormatService.SaveScheduleItemAndTemplateAsync(iCompId, dto);

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

        //SaveScheduleFormatSub-ItemAndHeading
        [HttpPost("SaveOrUpdateScheduleFormatSub-Items")]
        public async Task<IActionResult> SaveScheduleSubItemAndTemplateAsync([FromQuery] int iCompId, [FromBody] SaveScheduleFormatSub_ItemDto dto)
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
                bool isUpdate = dto.ASSI_ID > 0;

                var result = await _ScheduleFormatService.SaveScheduleSubItemAndTemplateAsync(iCompId, dto);

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
    }
}
