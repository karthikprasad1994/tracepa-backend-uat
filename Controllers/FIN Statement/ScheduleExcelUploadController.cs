using Dapper;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.ScheduleExcelUploadDto;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleExcelUploadController : ControllerBase
    {
        private ScheduleExcelUploadInterface _ScheduleExcelUploadService;

        public ScheduleExcelUploadController(ScheduleExcelUploadInterface ExcelUploadInterface)
        {
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
        public async Task<IActionResult> GetCustomerName([FromQuery] int CompId)
        {

            try
            {
                var result = await _ScheduleExcelUploadService.GetCustomerNameAsync(CompId);

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

        //GetFinancialYear
        [HttpGet("GetFinancialYear")]
        public async Task<IActionResult> GetFinancialYear([FromQuery] int CompId)
        {
            try
            {
                var result = await _ScheduleExcelUploadService.GetFinancialYearAsync(CompId);

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
        [HttpGet("GetCustomerDurationId")]
        public async Task<IActionResult> GetCustomerDurationId([FromQuery] int CompId, [FromQuery] int CustId)
        {
            try
            {
                var durationId = await _ScheduleExcelUploadService.GetCustomerDurationIdAsync(CompId, CustId);

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

        //GetBranchName
        [HttpGet("GetBranchName")]
        public async Task<IActionResult> GetBranchName([FromQuery] int CompId, [FromQuery] int CustId)
        {
            try
            {
                var result = await _ScheduleExcelUploadService.GetBranchNameAsync(CompId, CustId);

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

        //SaveScheduleTemplate(P and L)
        [HttpPost("SaveScheduleTemplate(P and L)")]
        public async Task<IActionResult> SaveSchedulePandL([FromBody] List<ScheduleTemplatePandLDto> dtos)
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
                var resultIds = await _ScheduleExcelUploadService.SaveSchedulePandLAsync(dtos);

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

        //SaveScheduleTemplate(BalanceSheet)
        [HttpPost("SaveScheduleTemplate(BalnceSheet)")]
        public async Task<IActionResult> SaveScheduleBalanceSheet([FromBody] List<ScheduleTemplateBalanceSheetDto> dtos)
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
                var resultIds = await _ScheduleExcelUploadService.SaveScheduleBalanceSheetAsync(dtos);

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

        //SaveOpeningBalance
        [HttpPost("SaveOpeningBalance")]
        public async Task<IActionResult> SaveOpeningBalance([FromBody] List<OpeningBalanceDto> dtos)
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

                var resultIds = await _ScheduleExcelUploadService.SaveOpeningBalanceAsync(dtos);

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

        //SaveTrailBalance
        [HttpPost("SaveTrailBalance")]
        public async Task<IActionResult> SaveTrailBalance([FromBody] List<TrailBalanceDto> dtos)
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

                var resultIds = await _ScheduleExcelUploadService.SaveTrailBalanceAsync(dtos);

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

        //SaveClientTrailBalance
        [HttpPost("SaveClientTrailBalance")]
        public async Task<IActionResult> UploadClientTrailBalance([FromBody] List<ClientTrailBalance> items)
        {
            if (items == null || !items.Any())
                return BadRequest("No data provided.");

            try
            {
                var result = await _ScheduleExcelUploadService.ClientTrailBalanceAsync(items);
                return Ok(new
                {
                    Success = true,
                    Message = "Client Trail Balance uploaded successfully.",
                    SavedIds = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while uploading data.",
                    Details = ex.Message
                });
            }
        }

        //SaveJournalEntry
        [HttpPost("SaveJournalEntry")]
        public async Task<IActionResult> SaveCompleteTrailBalance([FromBody] List<TrailBalanceCompositeModel> models)
        {
            try
            {
                if (models == null || !models.Any())
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "Invalid input data.",
                        data = (object)null
                    });
                }

                var result = await _ScheduleExcelUploadService.SaveCompleteTrailBalanceAsync(models);

                return Ok(new
                {
                    statusCode = 200,
                    message = "Trail balance data saved successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while saving trail balance data.",
                    error = ex.Message
                });
            }
        }
    }
}
