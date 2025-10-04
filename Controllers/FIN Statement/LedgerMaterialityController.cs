using Microsoft.AspNetCore.Mvc;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
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
        public async Task<IActionResult> GetMaterialityDescription([FromQuery] int CompId, [FromQuery] string cmm_Category, [FromQuery] int YearId, [FromQuery] int CustId)
        {
            try
            {
                var result = await _LedgerMaterialityService.GetMaterialityDescriptionAsync(CompId, cmm_Category, YearId, CustId);

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
        [HttpPost("SaveOrUpdate")]
        public async Task<IActionResult> SaveOrUpdateLedgerMateriality([FromBody] IEnumerable<LedgerMaterialityMasterDto> dtos)
        {
            try
            {
                // ✅ Validation
                if (dtos == null || !dtos.Any())
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "No data provided to save or update.",
                        Data = (object)null
                    });
                }

                // ✅ Call service
                var results = await _LedgerMaterialityService.SaveOrUpdateLedgerMaterialityAsync(dtos);

                // ✅ Build response for each item
                var responseData = dtos.Zip(results, (dto, result) => new
                {
                    lm_ID = dto.lm_ID,
                    Operation = result[0] == 2 ? "Updated" : "Saved",
                    Message = result[0] == 2
                        ? $"Ledger Materiality (ID: {dto.lm_ID}) updated successfully."
                        : "Ledger Materiality saved successfully."
                }).ToList();

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Ledger Materiality records processed successfully.",
                    Data = responseData
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

        //GenerateIDButtonForContentMaterialityMaster
        [HttpPost("SaveContentMaterialityMaster")]
        public async Task<IActionResult> CreateMTContent([FromBody] CreateMTContentRequestDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Description))
            {
                return BadRequest(new CreateMTContentResponseDto
                {
                    StatusCode = 400,
                    Message = "Invalid request. CompId and Description are required."
                });
            }

            try
            {
                // Call service to generate MT code and insert record
                string newCode = await _LedgerMaterialityService.GenerateAndInsertContentForMTAsync(dto.CompId, dto.Description);

                return Ok(new CreateMTContentResponseDto
                {
                    StatusCode = 200,
                    Message = "MT content created successfully.",
                    NewCode = newCode
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new CreateMTContentResponseDto
                {
                    StatusCode = 500,
                    Message = $"An error occurred while creating MT content: {ex.Message}",
                    NewCode = string.Empty
                });
            }
        }
    }
}

