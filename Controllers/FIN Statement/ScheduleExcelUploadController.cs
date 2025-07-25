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

        //SaveScheduleTemplate(P and L)
        [HttpPost("SaveScheduleTemplate(P and L)")]
        public async Task<IActionResult> SaveSchedulePandL([FromQuery] string DBName, [FromQuery] int CompId, [FromBody] List<ScheduleTemplatePandLDto> dtos)
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
                var resultIds = await _ScheduleExcelUploadService.SaveSchedulePandLAsync(DBName, CompId, dtos);

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
        public async Task<IActionResult> SaveScheduleBalanceSheet([FromQuery] string DBName, [FromQuery] int CompId, [FromBody] List<ScheduleTemplateBalanceSheetDto> dtos)
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
                var resultIds = await _ScheduleExcelUploadService.SaveScheduleBalanceSheetAsync(DBName, CompId, dtos);

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

        //SaveScheduleTemplate
        [HttpPost("SaveScheduleTemplate")]
        public async Task<IActionResult> SaveScheduleTemplate([FromQuery] string DBName, [FromQuery] int CompId, [FromBody] List<ScheduleTemplateDto> dtos)
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
                var resultIds = await _ScheduleExcelUploadService.SaveScheduleTemplateAsync(DBName, CompId, dtos);

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
        public async Task<IActionResult> SaveOpeningBalance([FromQuery] string DBName, [FromQuery] int CompId, [FromBody] List<OpeningBalanceDto> dtos)
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

                var resultIds = await _ScheduleExcelUploadService.SaveOpeningBalanceAsync(DBName, CompId, dtos);

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
        public async Task<IActionResult> SaveTrailBalanceDetails([FromQuery] string DBName, [FromQuery] int CompId, [FromBody] List<TrailBalanceDto> HeaderDtos)
        {
            try
            {
                var resultIds = await _ScheduleExcelUploadService.SaveTrailBalanceDetailsAsync(DBName, CompId, HeaderDtos);
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

        //SaveClientTrailBalance
        [HttpPost("SaveClientTrailBalance")]
        public async Task<IActionResult> UploadClientTrailBalance([FromQuery] string DBName, [FromQuery] int CompId, [FromBody] List<ClientTrailBalance> items)
        {
            if (items == null || !items.Any())
                return BadRequest("No data provided.");
            try
            {
                var result = await _ScheduleExcelUploadService.ClientTrailBalanceAsync(DBName, CompId, items);
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
        public async Task<IActionResult> SaveCompleteTrailBalance([FromQuery] string DBName, [FromQuery] int CompId, [FromBody] List<TrailBalanceCompositeModel> models)
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
                var result = await _ScheduleExcelUploadService.SaveCompleteTrailBalanceAsync(DBName, CompId, models);
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
