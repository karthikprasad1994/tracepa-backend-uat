using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.Master;
using TracePca.Interface.Master;

namespace TracePca.Controllers.Master
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyDetailsController : ControllerBase
    {
        private readonly CompanyDetailsInterface _companyDetailsService;

        public CompanyDetailsController(CompanyDetailsInterface companyDetailsService)
        {
            _companyDetailsService = companyDetailsService;
        }

        [HttpGet("GetCompanyDetailsList")]
        public async Task<IActionResult> GetCompanyDetailsList([FromQuery] int compId)
        {
            var (success, message, data) = await _companyDetailsService.GetCompanyDetailsListAsync(compId);
            if (!success)
                return StatusCode(500, new { success, message });

            return Ok(new { statusCode = 200, success, message, data });
        }

        [HttpGet("GetCompanyDetailsById")]
        public async Task<IActionResult> GetCompanyDetailsById([FromQuery] int id, [FromQuery] int compId)
        {
            var (success, message, data) = await _companyDetailsService.GetCompanyDetailsByIdAsync(id, compId);
            if (!success)
                return StatusCode(500, new { success, message });

            return Ok(new { statusCode = 200, success, message, data });
        }

        [HttpPost("SaveOrUpdateCompanyDetails")]
        public async Task<IActionResult> SaveOrUpdateCompanyDetails([FromBody] CompanyDetailsDto dto)
        {
            var (success, message, data) = await _companyDetailsService.SaveOrUpdateCompanyDetailsAsync(dto);

            return Ok(new { statusCode = 200, success, message, data });
        }
    }
}
