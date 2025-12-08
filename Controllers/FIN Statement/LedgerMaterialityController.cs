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

        //SaveOrUpdateContentMateriality
        [HttpPost("SaveOrUpdateContentMateriality")]
        public async Task<IActionResult> SaveOrUpdateMTContent([FromBody] CreateMTContentRequestDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "Invalid request payload."
                });
            }

            try
            {
                string newCode = await _LedgerMaterialityService.SaveOrUpdateContentForMTAsync(dto.cmm_ID, dto.CMM_CompID, dto.cmm_Desc, dto.cms_Remarks, dto.cmm_Category);

                return Ok(new
                {
                    statusCode = 200,
                    message = dto.cmm_ID.HasValue && dto.cmm_ID.Value > 0
                        ? "MT Content updated successfully."
                        : "MT Content created successfully.",
                    newCode = newCode
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while saving or updating MT content.",
                    error = ex.Message
                });
            }
        }

        //GetMaterialityId
        [HttpGet("GetMatrialityId")]
        public async Task<IActionResult> GetMatrialityId([FromQuery] int CompId, [FromQuery] int Id)
        {
            try
            {
                var result = await _LedgerMaterialityService.GetMaterialityIdAsync(CompId, Id);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "No content found for the given parameters.",
                        data = new List<GetMaterialityIdDto>()
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

        //DeleteMaterialityById
        [HttpDelete("DeleteMaterialityById")]
        public async Task<IActionResult> DeleteMaterialityById([FromQuery] int Id)
        {
            try
            {
                int rowsDeleted = await _LedgerMaterialityService.DeleteMaterialityByIdAsync(Id);

                if (rowsDeleted == 0)
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "No record found with the given ID or it may have been already deleted."
                    });
                }

                return Ok(new
                {
                    status = 200,
                    message = "Record deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while deleting the record.",
                    error = ex.Message
                });
            }
        }

        //LoadDescription
        [HttpGet("LoadDescription")]
        public async Task<IActionResult> LoadDescription([FromQuery] int compId, [FromQuery] string category)
        {
            try
            {
                var result = await _LedgerMaterialityService.LoadDescriptionAsync(compId, category);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "No descriptions found for the given company and category.",
                        data = new List<LoadDescriptionDto>()
                    });
                }

                return Ok(new
                {
                    status = 200,
                    message = "Descriptions loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while loading descriptions.",
                    error = ex.Message
                });
            }
        }

        //GetMaterialityBasis
        [HttpGet("GetMaterialityBasis")]
        public async Task<IActionResult> GetMaterialityBasis(int compId, int custId, int branchId, int yearId, int typeId)
        {
            try
            {
                var result = await _LedgerMaterialityService.GetMaterialityBasisAsync(compId, custId, branchId, yearId, typeId);

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

    }
}

