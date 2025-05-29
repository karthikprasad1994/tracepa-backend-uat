using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.ExcelUploadDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelUploadController : ControllerBase
    {
        private ExcelUploadInterface _ExcelUploadInterface;
        private ExcelUploadInterface _ExcelUploadService;

        public ExcelUploadController(ExcelUploadInterface ExcelUploadInterface)
        {
            _ExcelUploadInterface = ExcelUploadInterface;
            _ExcelUploadService = ExcelUploadInterface;
        }
        //DownloadUploadableExcelAndTemplate
        [HttpGet("DownloadableExcelFile")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public IActionResult DownloadExcelTemplate()
        {
            var result = _ExcelUploadService.GetExcelTemplate();

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
                var result = await _ExcelUploadService.GetCustomerNameAsync(icompId);

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
                var result = await _ExcelUploadService.GetFinancialYearAsync(icompId);

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
                var data = await _ExcelUploadService.GetDurationAsync(compId, custId);

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
                var result = await _ExcelUploadService.GetBranchNameAsync(icompId, icustId);

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
                int[] result = await _ExcelUploadService.SaveAllInformationAsync(request);

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
