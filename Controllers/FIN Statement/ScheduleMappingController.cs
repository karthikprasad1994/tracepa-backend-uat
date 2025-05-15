using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.Audit;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleCustDto;

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

        [HttpGet("GetCustomer")]
        public async Task<IActionResult> GetCustomers([FromQuery] int compId)
        {
            var result = await _ScheduleMappingInterface.LoadCustomers(compId);

            var response = new
            {
                Status = 200,
                msg = result != null && result.Any() ? "Fetched Successfully" : "No data found",
                data = result
            };

            return Ok(response);
        }

        [HttpGet("GetFinancialYear")]
        public async Task<IActionResult> LoadFinancialYear([FromQuery] int compId)
        {
            var result = await _ScheduleMappingInterface.LoadFinancialYear(compId);

            var response = new
            {
                Status = 200,
                msg = result != null && result.Any() ? "Fetched Successfully" : "No data found",
                data = result
            };

            return Ok(response);
        }


        [HttpGet("GetCustBranch")]
        public async Task<IActionResult> LoadcustBranches([FromQuery] int compId,int CustId)
        {
            var result = await _ScheduleMappingInterface.LoadBranches(compId,CustId);

            var response = new
            {
                Status = 200,
                msg = result != null && result.Any() ? "Fetched Successfully" : "No data found",
                data = result
            };

            return Ok(response);
        }

        [HttpGet("GetCustDuration")]
        public async Task<IActionResult> LoadcustDuration([FromQuery] int compId, int CustId)
        {
            var result = await _ScheduleMappingInterface.LoadDuration(compId, CustId);

            var response = new
            {
                Status = 200,
                msg = result != null && result.Any() ? "Fetched Successfully" : "No data found",
                data = result
            };

            return Ok(response);
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

        //SaveOrUpdateScheduleFormatHeading
        [HttpPost("SaveOrUpdateScheduleFormatHeading")]
        public async Task<IActionResult> SaveScheduleHeadingAndTemplateAsync([FromQuery] int iCompId, [FromBody] SaveScheduleFormatHeadingDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Invalid input: DTO is null",
                    Data = (object)null
                });
            }
            try
            {
                bool isUpdate = dto.ASH_ID > 0;

                var result = await _ScheduleMappingService.SaveScheduleHeadingAndTemplateAsync(iCompId, dto);

                string successMessage = isUpdate
                    ? "Schedule Heading successfully updated."
                    : "Schedule Heading successfully created.";

                return Ok(new
                {
                    Status = 200,
                    Message = successMessage,
                    Data = new
                    {
                        UpdateOrSave = result[0],
                        Oper = result[1],
                        IsUpdate = isUpdate
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }
    }
}

