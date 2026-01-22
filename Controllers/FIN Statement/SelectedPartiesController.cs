using Microsoft.AspNetCore.Mvc;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.SelectedPartiesDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class SelectedPartiesController : ControllerBase
    {
        private SelectedPartiesInterface _SelectedPartiesService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public SelectedPartiesController(SelectedPartiesInterface SelectedPartiesInterface, Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _SelectedPartiesService = SelectedPartiesInterface;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _dbcontext = dbcontext;
        }

        //GetSelectedParties
        [HttpGet("GetTrailBalance")]
        public async Task<IActionResult> GetTrailBalance([FromQuery] int custId, [FromQuery] int financialYearId, [FromQuery] int branchId, [FromQuery] int id, [FromQuery] int pkid)
        {
            try
            {
                var result = await _SelectedPartiesService.GetTrailBalanceAsync(custId, financialYearId, branchId,id, pkid);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No Trail Balance records found."
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Trail Balance records retrieved successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching Trail Balance records.",
                    Error = ex.Message
                });
            }
        }

        //UpdateSelectedPartiesStatus
        [HttpPost("UpdateTrailBalanceStatusBulk")]
        public async Task<IActionResult> UpdateTrailBalanceStatusBulk([FromBody] List<UpdateTrailBalanceStatusDto> dtoList)
        {
            if (dtoList == null || dtoList.Count == 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "No records provided for update."
                });
            }

            try
            {
                var updatedCount = await _SelectedPartiesService.UpdateTrailBalanceStatusAsync(dtoList);

                if (updatedCount == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No Trail Balance records found to update."
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = $"{updatedCount} Trail Balance record(s) updated successfully.",
                    UpdatedCount = updatedCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while updating Trail Balance statuses.",
                    Error = ex.Message
                });
            }
        }

        //GetJETransactionDetails
        [HttpGet("GetJournalEntryWithTrailBalance")]
        public async Task<IActionResult> GetJournalEntryWithTrailBalance([FromQuery] int custId, [FromQuery] int yearId, [FromQuery] int branchId)
        {
            try
            {
                var result = await _SelectedPartiesService.GetJournalEntryWithTrailBalanceAsync(custId, yearId, branchId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No Journal Entry records linked to Trail Balance found."
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Journal Entry records with Trail Balance retrieved successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching Journal Entry with Trail Balance.",
                    Error = ex.Message
                });
            }
        }

        //UpdateJESeqReferenceNum
        [HttpPost("UpdateSeqReference")]
        public async Task<IActionResult> UpdateSeqReferenceAsync([FromBody] List<UpdateJournalEntrySeqRefDto> dtoList)
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
                var updatedCount = await _SelectedPartiesService.UpdateJournalEntrySeqRefAsync(dtoList);

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
    }
}
