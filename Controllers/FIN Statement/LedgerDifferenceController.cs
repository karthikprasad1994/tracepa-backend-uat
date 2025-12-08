using Microsoft.AspNetCore.Mvc;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.LedgerDifferenceDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class LedgerDifferenceController : ControllerBase
    {
        private LedgerDifferenceInterface _LedgerDifferenceService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public LedgerDifferenceController(LedgerDifferenceInterface LedgerDifferenceInterface, Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _LedgerDifferenceService = LedgerDifferenceInterface;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _dbcontext = dbcontext;
        }

        //GetDescriptionWiseDetails
        [HttpGet("GetDescriptionWiseDetails")]
        public async Task<IActionResult> GetDescriptionWiseDetails(int compId, int custId, int branchId, int yearId, int typeId)
        {
            try
            {
                var result = await _LedgerDifferenceService.GetDescriptionWiseDetailsAsync(compId, custId, branchId, yearId, typeId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No records found."
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Data fetched successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching description-wise details.",
                    Error = ex.Message
                });
            }
        }

        //UpdateDescriptionWiseDetailsStatus
        [HttpPost("UpdateDescriptionWiseDetailsStatus")]
        public async Task<IActionResult> UpdateTrailBalanceStatusBulk([FromBody] List<UpdateDescriptionWiseDetailsStatusDto> dtoList)
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
                var updatedCount = await _LedgerDifferenceService.UpdateTrailBalanceStatusAsync(dtoList);

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

        //GetDescriptionDetails
        [HttpGet("GetDescriptionDetails")]
        public async Task<IActionResult> GetAccountDetails(int compId, int custId, int branchId, int yearId, int typeId, int pkId)
        {
            try
            {
                var result = await _LedgerDifferenceService.GetAccountDetailsAsync(compId, custId, branchId, yearId, typeId, pkId);

                // If no data found return empty list rather than NotFound()
                if (result == null || !result.Any())
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        Message = "No records found.",
                        Data = new List<object>() // empty array
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Data fetched successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching description-by-accountwise details.",
                    Error = ex.Message
                });
            }
        }

    }
}
