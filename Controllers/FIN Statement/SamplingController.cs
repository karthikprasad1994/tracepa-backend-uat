using DocumentFormat.OpenXml.Office.Y2022.FeaturePropertyBag;
using Microsoft.AspNetCore.Mvc;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.SamplingDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class SamplingController : ControllerBase
    {
        private SamplingInterface _SamplingService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public SamplingController(SamplingInterface SamplingInterface, Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _SamplingService = SamplingInterface;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _dbcontext = dbcontext;
        }

        //GetSystemSampling
        [HttpGet("GetSystemSampling")]
        public async Task<IActionResult> GetSystemSampling(int compId, int custId, int branchId, int yearId, int nthPosition, int fromRow, int toRow, int sampleSize)
        {
            try
            {
                var result = await _SamplingService.GetSystemSamplingAsync(compId, custId, branchId, yearId, nthPosition, fromRow, toRow, sampleSize);

                if (result == null)
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

        //GetStatifiedSamping
        [HttpGet("GetStratifiedSampling")]
        public async Task<IActionResult> GetStratifiedSampling(
        int compId,
        int custId,
        int branchId,
        int yearId,
        decimal percentage)
        {
            try
            {
                var result = await _SamplingService.GetStratifiedSamplingAsync(compId, custId, branchId, yearId, percentage);

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
                    Message = "An error occurred while fetching stratified sampling data.",
                    Error = ex.Message
                });
            }
        }

        //UpdateSystemSamplingStatus
        [HttpPost("UpdateSystemSamplingStatus")]
        public async Task<IActionResult> UpdateSystemSamplingStatus([FromBody] List<UpdateSystemSamplingStatusDto> dtoList)
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
                var updatedCount = await _SamplingService.UpdateSystemSamplingStatusAsync(dtoList);

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
                    message = $"{updatedCount} Random sampling Status updated successfully."
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

        //UpdateStatifiedSampingStatus
        [HttpPost("UpdateStatifiedSampingStatus")]
        public async Task<IActionResult> UpdateSystemSamplingStatus([FromBody] List<UpdateStatifiedSampingStatusDto> dtoList)
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
                var updatedCount = await _SamplingService.UpdateStatifiedSampingStatusAsync(dtoList);

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
                    message = $"{updatedCount} Random sampling Status updated successfully."
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
