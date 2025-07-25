using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.JournalEntryDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class JournalEntryController : ControllerBase
    {
        private JournalEntryInterface _JournalEntryInterface;
        private JournalEntryInterface _JournalEntryService;

        public JournalEntryController(JournalEntryInterface JournalEntryInterface)
        {
            _JournalEntryInterface = JournalEntryInterface;
            _JournalEntryService = JournalEntryInterface;
        }
        
        //GetJournalEntryInformation
        [HttpGet("GetJournalEntryInformation")]
        public async Task<IActionResult> GetJournalEntryInformation(
        [FromQuery] string DBName,
        [FromQuery] int CompId,
        [FromQuery] int UserId,
        [FromQuery] string Status,
        [FromQuery] int CustId,
        [FromQuery] int YearId,
        [FromQuery] int BranchId,
        [FromQuery] string DateFormat,
        [FromQuery] int DurationId)
        {
            try
            {
                var result = await _JournalEntryService.GetJournalEntryInformationAsync( DBName,
                    CompId, UserId, Status, CustId, YearId, BranchId, DateFormat, DurationId);

                return Ok(new
                {
                    status = 200,
                    message = result.Any() ? "Journal entries retrieved successfully." : "No journal entries found.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while retrieving journal entries.",
                    error = ex.Message
                });
            }
        }
    }
}
