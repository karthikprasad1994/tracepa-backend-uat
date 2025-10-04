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
        public async Task<IActionResult> GetTrailBalance([FromQuery] int custId, [FromQuery] int financialYearId, [FromQuery] int branchId)
        {
            try
            {
                var result = await _SelectedPartiesService.GetTrailBalanceAsync(custId, financialYearId, branchId);

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
        [HttpPut("UpdateTrailBalanceStatus")]
        public async Task<IActionResult> UpdateTrailBalanceStatus([FromBody] UpdateTrailBalanceStatusDto dto)
        {
            if (dto == null || dto.Id <= 0 || dto.CustId <= 0 || dto.FinancialYearId <= 0 || dto.BranchId <= 0 || string.IsNullOrWhiteSpace(dto.Status))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Invalid input parameters."
                });
            }

            try
            {
                var updatedId = await _SelectedPartiesService.UpdateTrailBalanceStatusAsync(dto);

                if (updatedId == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No Trail Balance record found to update."
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Trail Balance status updated successfully.",
                    Id = updatedId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while updating Trail Balance status.",
                    Error = ex.Message
                });
            }
        }
    }
}
