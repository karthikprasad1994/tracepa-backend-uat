using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TracePca.Data;
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

        //GetContentManagement
        [HttpGet("GetContentManagement")]
        public async Task<IActionResult> GetContentManagement([FromQuery] int CompId, [FromQuery] string cmm_Category)
        {
            try
            {
                var result = await _LedgerMaterialityService.GetContentManagementAsync(CompId, cmm_Category);

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

