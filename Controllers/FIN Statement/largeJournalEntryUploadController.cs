using JournalEntryUploadAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{

    [ApiController]
    [Route("api/[controller]")]
    public class largeJournalEntryUploadController : ControllerBase
    {
        private readonly IBulkJournalEntryService _bulkJournalEntryService;
        private readonly ILogger<JournalEntryController> _logger;

        public largeJournalEntryUploadController(IBulkJournalEntryService bulkJournalEntryService,
                                    ILogger<JournalEntryController> logger)
        {
            _bulkJournalEntryService = bulkJournalEntryService;
            _logger = logger;
        }

        [HttpPost("bulk-upload")]
        [RequestSizeLimit(100 * 1024 * 1024)] // 100MB
        [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)]
        public async Task<IActionResult> BulkUpload([FromForm] JournalEntryUploadDto request)
        {
            try
            {
                _logger.LogInformation($"Bulk upload request: Customer={request.CustomerId}, File={request.File?.FileName}");

                if (request.File == null || request.File.Length == 0)
                    return BadRequest(new { Error = "File is required" });

                var result = await _bulkJournalEntryService.BulkUploadJournalEntriesAsync(request.File, request);

                if (result.Success)
                {
                    return Ok(new
                    {
                        result.Success,
                        result.Message,
                        result.TotalRecords,
                        result.ProcessedRecords,
                        result.FailedRecords,
                        ProcessingTime = $"{result.ProcessingTimeInSeconds:F2} seconds",
                        RecordsPerSecond = $"{result.RecordsPerSecond:F0}",
                        result.TransactionId,
                        result.StartTime,
                        result.EndTime
                    });
                }

                return BadRequest(new
                {
                    result.Success,
                    result.Message,
                    result.TotalRecords,
                    result.ProcessedRecords,
                    result.FailedRecords,
                    Errors = result.Errors.Take(10).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk upload failed");
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromForm] JournalEntryUploadDto request)
        {
            try
            {
                var result = await _bulkJournalEntryService.ValidateJournalEntriesAsync(request.File, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Validation failed");
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(
            [FromQuery] int customerId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var history = await _bulkJournalEntryService.GetUploadHistoryAsync(customerId, startDate, endDate, pageNumber, pageSize);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get upload history");
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}
