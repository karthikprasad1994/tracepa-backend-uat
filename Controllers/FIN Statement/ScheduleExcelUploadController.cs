using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.ScheduleExcelUploadDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleExcelUploadController : ControllerBase
    {
       // private ScheduleExcelUploadInterface _ScheduleExcelUploadInterface;
        private ScheduleExcelUploadInterface _ScheduleExcelUploadService;

        public ScheduleExcelUploadController(ScheduleExcelUploadInterface ExcelUploadInterface)
        {
          //  _ScheduleExcelUploadInterface = ExcelUploadInterface;
            _ScheduleExcelUploadService = ExcelUploadInterface;
        }
        //DownloadUploadableExcelAndTemplate
        [HttpGet("DownloadableExcelFile")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public IActionResult DownloadExcelTemplate()
        {
            var result = _ScheduleExcelUploadService.GetExcelTemplate();

            if (result.FileBytes == null)
                return NotFound("File not found.");

            return File(result.FileBytes, result.ContentType, result.FileName);
        }

        //GetCustomersName
        [HttpGet("GetCustomersName")]
        public async Task<IActionResult> GetCustomerName([FromQuery] int icompId)
        {

            try
            {
                var result = await _ScheduleExcelUploadService.GetCustomerNameAsync(icompId);

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
                    message = "ustomer name loaded successfully.",
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
                var result = await _ScheduleExcelUploadService.GetFinancialYearAsync(icompId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Financial types found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Financial types loaded successfully.",
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
        [HttpGet("duration-id")]
        public async Task<IActionResult> GetCustomerDurationId([FromQuery] int compId, [FromQuery] int custId)
        {
            var durationId = await _ScheduleExcelUploadService.GetCustomerDurationIdAsync(compId, custId);
            if (durationId == null)
            {
                return NotFound("Duration ID not found.");
            }

            return Ok(new { Cust_DurtnId = durationId });
        }

        //GetBranchName
        [HttpGet("GetBranchName")]
        public async Task<IActionResult> GetBranchName([FromQuery] int icompId, [FromQuery] int icustId)
        {
            try
            {
                var result = await _ScheduleExcelUploadService.GetBranchNameAsync(icompId, icustId);

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

        //SaveAllInformation
        [HttpPost("SaveAllInformation")]
        public async Task<IActionResult> SaveAllInformation([FromBody] UploadExcelRequestDto request)
        {
            if (request == null || request.ExcelRows == null || request.ExcelRows.Count == 0)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Invalid request: request or Excel data is null or empty.",
                    Data = (object)null
                });
            }
            try
            {
                int[] result = await _ScheduleExcelUploadService.SaveAllInformationAsync(request);

                if (result != null && result.Length > 0 && result[0] > 0)
                {
                    return Ok(new
                    {
                        Status = 200,
                        Message = "Upload successful.",
                        Data = new
                        {
                            RecordsProcessed = result
                        }
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        Status = 500,
                        Message = "Upload failed. No records were processed.",
                        Data = (object)null
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Message = "An error occurred while uploading Excel data.",
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }
    }
}
