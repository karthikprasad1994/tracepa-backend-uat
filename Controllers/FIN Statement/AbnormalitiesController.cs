using DocumentFormat.OpenXml.Wordprocessing;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.AbnormalitiesDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class AbnormalitiesController : ControllerBase
    {
        private AbnormalitiesInterface _AbnormalitiesInterface;
        private AbnormalitiesInterface _AbnormalitiesService;
        private readonly IWebHostEnvironment _env;

        public AbnormalitiesController(AbnormalitiesInterface AbnormalitiesInterface, IWebHostEnvironment env)
        {
            _AbnormalitiesInterface = AbnormalitiesInterface;
            _AbnormalitiesService = AbnormalitiesInterface;
            _env = env;
        }

        //GetAbnormalTransactions
        [HttpGet("Abnormal")]
        public async Task<IActionResult> GetAbnormalTransactions([FromQuery] int iCustId, 
            [FromQuery] int iBranchId, [FromQuery] int iYearID, [FromQuery] int iAbnormalType, [FromQuery] string sAmount,

            [FromQuery] string? searchTerm, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var result = await _AbnormalitiesService.GetAbnormalTransactionsAsync(iCustId, iBranchId, 
                    iYearID, iAbnormalType, sAmount, searchTerm, pageNumber, pageSize);

                if (result == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No abnormal transactions found.",
                        Data = new List<object>()
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Abnormal transactions retrieved successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching abnormal transactions.",
                    Error = ex.Message
                });
            }
        }

        //UpdateAEStatus
        [HttpPost("UpdateAEStatus")]
        public async Task<IActionResult> UpdateSeqReferenceAsync([FromBody] List<UpdateJournalEntrySeqRef1Dto> dtoList)
        {
            if (dtoList == null || !dtoList.Any())
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "No data provided for update."
                });
            }

            try
            {
                var updatedCount = await _AbnormalitiesService.UpdateJournalEntrySeqRefAsync(dtoList);

                if (updatedCount == 0)
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Journal Entry records found to update."
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = $"{updatedCount} Journal Entry records updated successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while updating Journal Entry SeqReference numbers.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("DownloadAbnormalTransactionsExcel")]
        public async Task<IActionResult> DownloadAbnormalTransactionsExcel(
    [FromQuery] int iCustId,
    [FromQuery] int iBranchId,
    [FromQuery] int iYearID,
    [FromQuery] int iAbnormalType,
    [FromQuery] decimal sAmount,
    [FromQuery] string searchTerm = "")
        {
            try
            {
                var excelData = await _AbnormalitiesService.DownloadAbnormalTransactionsExcelAsync(
                    iCustId,
                    iBranchId,
                    iYearID,
                    iAbnormalType,
                    sAmount,
                    searchTerm);

                var fileName = $"AbnormalTransactions_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelData,
                           "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                           fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating Excel file: {ex.Message}");
            }
        }


    }
}

