using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TracePca.Data;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.LedgerMaterialityDto;


namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class LedgerMaterialityController : ControllerBase
    {
        private LedgerMaterialityInterface _LedgerMaterialityService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public LedgerMaterialityController(LedgerMaterialityInterface LedgerMaterialityInterface, Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _LedgerMaterialityService = LedgerMaterialityInterface;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _dbcontext = dbcontext;
        }

        //GetMaterialityDescription
        [HttpGet("GetMaterialityDescription")]
        public async Task<IActionResult> GetMaterialityDescription([FromQuery] int CompId, [FromQuery] string cmm_Category)
        {
            try
            {
                var result = await _LedgerMaterialityService.GetMaterialityDescriptionAsync(CompId, cmm_Category);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "No content found for the given parameters.",
                        data = new List<ContentManagementDto>()
                    });
                }

                return Ok(new
                {
                    status = 200,
                    message = "Content retrieved successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while retrieving content.",
                    error = ex.Message
                });
            }
        }

        //SaveOrUpdateLedgerMaterialityMaster
        [HttpPost("SaveOrUpdateLedgerMaterialityMaster")]
        public async Task<IActionResult> SaveOrUpdateLedgerMateriality([FromBody] LedgerMaterialityMasterDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Invalid input data."
                    });
                }

                var result = await _LedgerMaterialityService.SaveOrUpdateLedgerMaterialityAsync(dto);

                string actionMessage = dto.lm_ID == 0
                    ? "Ledger Materiality saved successfully."
                    : "Ledger Materiality updated successfully.";

                return Ok(new
                {
                    StatusCode = 200,
                    Message = actionMessage,
                    UpdateOrSave = result[0],
                    Oper = result[1]
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving or updating Ledger Materiality.",
                    Error = ex.Message
                });
            }
        }

        //GetLedgerMaterialityMaster
        [HttpGet("GetLedgerMaterialityMaster")]
        public async Task<IActionResult> GetLedgerMateriality([FromQuery] int compId, [FromQuery] int lm_ID)
        {
            try
            {
                var result = await _LedgerMaterialityService.GetLedgerMaterialityAsync(compId, lm_ID);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No Ledger Materiality records found."
                    });
                }
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Ledger Materiality records retrieved successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching Ledger Materiality.",
                    Error = ex.Message
                });
            }
        }
    }
}

