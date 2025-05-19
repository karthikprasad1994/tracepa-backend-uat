using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.Audit;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleMappingDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleMappingController : ControllerBase
    {
        private ScheduleMappingInterface _ScheduleMappingInterface;
        private ScheduleMappingInterface _ScheduleMappingService;

        public ScheduleMappingController(ScheduleMappingInterface ScheduleMappingInterface)
        {
            _ScheduleMappingInterface = ScheduleMappingInterface;
            _ScheduleMappingService = ScheduleMappingInterface;       
        }

        //GetCustomersName
        [HttpGet("GetCustomersName")]
        public async Task<IActionResult> GetCustomerName([FromQuery] int icompId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetCustomerNameAsync(icompId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No company types found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Company types loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching company types.",
                    error = ex.Message
                });
            }
        }

        //GetFinancialYear
        [HttpGet("GetFinancialYear")]
        public async Task<IActionResult> GetFinancialYear([FromQuery] int icompId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetFinancialYearAsync(icompId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No company types found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Company types loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching company types.",
                    error = ex.Message
                });
            }
        }

        //GetDuration
        [HttpGet("GetDuration")]
        public async Task<IActionResult> GetDuration([FromQuery] int compId, [FromQuery] int custId)
        {
            try
            {
                var data = await _ScheduleMappingService.GetDurationAsync(compId, custId);

                if (data == null || !data.Any())
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "No duration found for the given customer and company.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    status = 200,
                    message = "Duration loaded successfully.",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "Internal server error.",
                    error = ex.Message
                });
            }
        }

        //GetBranchName
        [HttpGet("GetBranchName")]
        public async Task<IActionResult> GetBranchName([FromQuery] int icompId, [FromQuery] int icustId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetBranchNameAsync(icompId, icustId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No company types found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Company types loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching company types.",
                    error = ex.Message
                });
            }
        }

        [HttpPost("headings")]
        public async Task<IActionResult> GetScheduleHeadings([FromQuery] int compId, int CustId,int ScheduleTypeId)
        {
            var result = await _ScheduleMappingInterface.GetScheduleHeadingsAsync(compId, CustId,ScheduleTypeId);
            return Ok(result);
        }
        [HttpPost("Subheadings")]
        public async Task<IActionResult> GetSchedulesubHeadings([FromQuery] int compId, int CustId, int ScheduleTypeId)
        {
            var result = await _ScheduleMappingInterface.GetSchedulesubHeadingsAsync(compId, CustId, ScheduleTypeId);
            return Ok(result);
        }
        [HttpPost("Items")]
        public async Task<IActionResult> GetScheduleitem([FromQuery] int compId, int CustId, int ScheduleTypeId)
        {
            var result = await _ScheduleMappingInterface.GetScheduleItemAsync(compId, CustId, ScheduleTypeId);
            return Ok(result);
        }
        [HttpPost("SubItem")]
        public async Task<IActionResult> GetScheduleSubi([FromQuery] int compId, int CustId, int ScheduleTypeId)
        {
            var result = await _ScheduleMappingInterface.GetScheduleSubItemAsync(compId, CustId, ScheduleTypeId);
            return Ok(result);
        }


        [HttpPost("saveTrailbalance")]
            public async Task<IActionResult> SaveTrailBalance([FromBody] TrailBalanceUploadDto dto)
            {
                if (dto == null)
                    return BadRequest("Invalid data.");

                string dbName = ""; // derive from config or token context if needed
                int userId = dto.ATBU_CRBY;

                try
                {
                    var result = await _ScheduleMappingInterface.SaveTrailBalanceExcelUploadAsync(dbName, dto, userId);
                    return Ok(new { Success = true, Result = result });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Success = false, Message = ex.Message });
                }
            }

        [HttpPost("saveTrailBalanceDetails")]
        public async Task<IActionResult> SaveTrailBalance([FromQuery] string dbName, [FromBody] TrailBalanceUploadDetailDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid data.");

            try
            {
                int userId = dto.ATBUD_CRBY; // you may get this from token if needed
                int result = await _ScheduleMappingInterface.SaveTrailBalanceDetailAsync(dbName, dto, userId);
                return Ok(new { success = true, id = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/<ScheduleMappingController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ScheduleMappingController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ScheduleMappingController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ScheduleMappingController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ScheduleMappingController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

