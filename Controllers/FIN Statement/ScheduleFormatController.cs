using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.ScheduleFormatDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleFormatController : ControllerBase
    {
        private ScheduleFormatInterface _ScheduleFormatInterface;
        private ScheduleFormatInterface _ScheduleFormatService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ScheduleFormatController(ScheduleFormatInterface ScheduleFormatInterface)
        {
            _ScheduleFormatInterface = ScheduleFormatInterface;
            _ScheduleFormatService = ScheduleFormatInterface;
        }

        //GetScheduleHeading
        [HttpGet("GetScheduleHeading")]
        public async Task<IActionResult> GetScheduleFormatHeadingAsync([FromQuery] int CompId, [FromQuery] int ScheduleId, [FromQuery] int CustId, [FromQuery] int AccHead)
        {
            try
            {
                var result = await _ScheduleFormatService.GetScheduleFormatHeadingAsync(CompId, ScheduleId, CustId, AccHead);

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

        //GetScheduleSubHeading
        [HttpGet("GetScheduleSubHeading")]
        public async Task<IActionResult> GetScheduleFormatSubHeadingAsync([FromQuery] int CompId, [FromQuery] int ScheduleId, [FromQuery] int CustId, [FromQuery] int HeadingId)
        {
            try
            {
                if (HeadingId <= 0)
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "HeadingId is required and must be greater than zero.",
                        data = (object)null
                    });
                }

                var result = await _ScheduleFormatService.GetScheduleFormatSubHeadingAsync(CompId, ScheduleId, CustId, HeadingId);

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

        //GetScheduleItem
        [HttpGet("GetScheduleItem")]
        public async Task<IActionResult> GetScheduleFormatItemsAsync([FromQuery] int CompId, [FromQuery] int ScheduleId, [FromQuery] int CustId, [FromQuery] int HeadingId, [FromQuery] int SubHeadId)
        {
            try
            {
                // Call service method
                var result = await _ScheduleFormatService.GetScheduleFormatItemsAsync(CompId, ScheduleId, CustId, HeadingId, SubHeadId);

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

        //GetScheduleSubitems
        [HttpGet("GetScheduleSubItem")]
        public async Task<IActionResult> GetScheduleFormatSubItemsAsync([FromQuery] int CompId, [FromQuery] int ScheduleId, [FromQuery] int CustId, [FromQuery] int HeadingId, [FromQuery] int SubHeadId, [FromQuery] int ItemId)
        {
            try
            {
                var result = await _ScheduleFormatService.GetScheduleFormatSubItemsAsync(CompId, ScheduleId, CustId, HeadingId, SubHeadId, ItemId);

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
        public async Task<IActionResult> GetScheduleTemplateAsync([FromQuery] int CompId, [FromQuery] int ScheduleId, [FromQuery] int CustId, [FromQuery] int AccHead)
        {
            try
            {
                var result = await _ScheduleFormatService.GetScheduleTemplateAsync(CompId, ScheduleId, CustId, AccHead);

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

        //DeleteScheduleTemplate(Grid)
        [HttpDelete("DeleteSchedulesTemplate")]
        public async Task<IActionResult> DeleteScheduleTemplateAsync(int CompId, int ScheduleType, int CustId, int SelectedValue, int MainId)
        {
            try
            {
                bool result = await _ScheduleFormatService.DeleteScheduleTemplateAsync(CompId, ScheduleType, CustId, SelectedValue, MainId);

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

        //SaveOrUpdateScheduleHeading
        [HttpPost("SaveOrUpdateScheduleHeading")]
        public async Task<IActionResult> SaveScheduleHeadingAndTemplateAsync([FromQuery] int CompId, [FromBody] SaveScheduleHeadingDto dto)
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
                var result = await _ScheduleFormatService.SaveScheduleHeadingAndTemplateAsync(CompId, dto);

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

        //SaveScheduleSubHeading
        [HttpPost("SaveOrUpdateScheduleSubHeading")]
        public async Task<IActionResult> SaveScheduleSubHeadingAndTemplate([FromQuery] int CompId, [FromBody] SaveScheduleSubHeadingDto dto)
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

                var result = await _ScheduleFormatService.SaveScheduleSubHeadingAndTemplateAsync(CompId, dto);

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

        //SaveScheduleItem
        [HttpPost("SaveOrUpdateScheduleItems")]
        public async Task<IActionResult> SaveScheduleItemAndTemplateAsync([FromQuery] int CompId, [FromBody] SaveScheduleItemDto dto)
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

                var result = await _ScheduleFormatService.SaveScheduleItemAndTemplateAsync(CompId, dto);

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

        //SaveScheduleSubItem
        [HttpPost("SaveOrUpdateScheduleSubItem")]
        public async Task<IActionResult> SaveScheduleSubItemAndTemplateAsync([FromQuery] int CompId, [FromBody] SaveScheduleSubItemDto dto)
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

                var result = await _ScheduleFormatService.SaveScheduleSubItemAndTemplateAsync(CompId, dto);

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

        //SaveOrUpdateScheduleHeadingAlias
        [HttpPost("SaveOrUpdateScheduleHeadingAlias")]
        public async Task<IActionResult> SaveOrUpdateScheduleHeadingAlias([FromQuery] int CompId, [FromBody] ScheduleHeadingAliasDto dto)
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
                bool isUpdate = dto.AGA_ID > 0;

                var result = await _ScheduleFormatService.SaveScheduleHeadingAliasAsync(CompId, dto);

                string successMessage = isUpdate
                    ? "Schedule Heading Alias successfully updated."
                    : "Schedule Heading Alias successfully created.";

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

        //GetScheduleTemplateCount
        [HttpGet("ScheduleCount")]
        public async Task<IActionResult> GetScheduleTemplateCount([FromQuery] int CustId, [FromQuery] int CompId)
        {
            try
            {
                var result = await _ScheduleFormatService.GetScheduleFormatItemsAsync(CustId, CompId);

                return Ok(new
                {
                    statusCode = 200,
                    message = "Schedule template count retrieved successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving the template count.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("GetSystemGenExcelFile")]
        public IActionResult GetExcelFilePathOnly(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest("File name is required.");

            // Optional: validate file exists if needed (on the server side)
            //string physicalPath = Path.Combine(
            //    "C:\\inetpub\\vhosts\\multimedia.interactivedns.com\\tracelites.multimedia.interactivedns.com\\public\\SampleExcels",
            //    fileName
            //);

            string physicalPath = Path.Combine(
    @"C:\Users\MMCS\Desktop\TracePa FrontEnd Updated - Copy\tracepa_nextjs\public\SampleExcels",
    fileName
);


            if (!System.IO.File.Exists(physicalPath))
                return NotFound($"File not found: {fileName}");

            // ✅ Return public URL instead of local path
            string publicUrl = $"https://tracelites.multimedia.interactivedns.com/SampleExcels/{fileName}";

            return Ok(publicUrl);
        }

        //SaveScheduleTemplate
        [HttpPost("SaveScheduleTemplate")]
        public async Task<IActionResult> SaveScheduleTemplate([FromQuery] int CompId, [FromBody] List<ScheduleTemplate> dtos)
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
                var resultIds = await _ScheduleFormatService.SaveScheduleTemplateAsync(CompId, dtos);

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

        //GetGridViewAlias
        [HttpGet("GetGridView1")]
        public async Task<IActionResult> LoadGridView1gridAsync([FromQuery] int CompId, [FromQuery] int CustId, [FromQuery] string lblText, [FromQuery] int SelectedVal)
        {
            try
            {
                var result = await _ScheduleFormatService.LoadGridView1gridAsync(CompId, CustId, lblText, SelectedVal);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No heading records found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Heading records fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving heading records.",
                    error = ex.Message
                });
            }
        }
        [HttpGet("get-customer-details")]
        public async Task<IActionResult> GetCustomerDetails(int custId, int compId)
        {
            var customer = await _ScheduleFormatService.GetCustomerDetailsAsync(custId, compId);

            if (customer == null)
                return NotFound("Customer not found.");

            return Ok(customer);
        }
    }
}
