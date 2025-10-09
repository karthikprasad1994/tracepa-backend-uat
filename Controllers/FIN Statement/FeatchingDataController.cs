using Microsoft.AspNetCore.Mvc;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.FeatchingDataDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatchingDataController : ControllerBase
    {
        private FeatchingDataInterface _FeatchingDataService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public FeatchingDataController(FeatchingDataInterface FeatchingDataInterface, Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _FeatchingDataService = FeatchingDataInterface;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _dbcontext = dbcontext;
        }

        //Trdm
        [HttpGet("exportTrdm")]
        public async Task<ActionResult<DatabaseExportResultTrdmDto>> ExportTrdmFullDatabaseAsync()
        {
            try
            {
                string filePath = await _FeatchingDataService.ExportTrdmFullDatabaseAsync();

                var result = new DatabaseExportResultTrdmDto
                {
                    IsSuccess = true,
                    Message = "Database exported successfully.",
                    FilePath = filePath,
                    ExportedAt = DateTime.UtcNow
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = new DatabaseExportResultTrdmDto
                {
                    IsSuccess = false,
                    Message = $"Database export failed: {ex.Message}",
                    ExportedAt = DateTime.UtcNow
                };

                return StatusCode(500, result);
            }
        }

        //Tr25_44
        [HttpGet("exportTr25_44")]
        public async Task<ActionResult<DatabaseExportResultT25_44Dto>> ExportTr25_44FullDatabaseAsync()
        {
            try
            {
                string filePath = await _FeatchingDataService.ExportTr25_44FullDatabaseAsync();

                var result = new DatabaseExportResultT25_44Dto
                {
                    IsSuccess = true,
                    Message = "TR25_044 database exported successfully.",
                    FilePath = filePath,
                    ExportedAt = DateTime.UtcNow
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = new DatabaseExportResultT25_44Dto
                {
                    IsSuccess = false,
                    Message = $"TR25_044 database export failed: {ex.Message}",
                    ExportedAt = DateTime.UtcNow
                };

                return StatusCode(500, result);
            }
        }
    }
}
