using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.Audit;
using TracePca.Interface.Audit;
using TracePca.Interface.FixedAssetsInterface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.Audit
{
    [Route("api/[controller]")]
    [ApiController]
    public class EngagementController : ControllerBase
    {
        private EngagementPlanInterface _EngagementInterface;
        public EngagementController(EngagementPlanInterface EngagementInterface)
        {
            _EngagementInterface = EngagementInterface;

        }






        [HttpGet("LoadAllDropDowns")]
        public async Task<IActionResult> LoadAllDropDowns(int compId)
        {
            var dropdownData = await _EngagementInterface.LoadAllDropdownDataAsync(compId);

            return Ok(new
            {
                statusCode = 200,
                message = "Dropdowns fetched successfully",
                data = dropdownData
            });
        }
        [HttpPost("InsertLoe")]
        public async Task<IActionResult> SaveLoe([FromBody] AddEngagementDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Call the method and get success flag
            bool isSuccess = await _EngagementInterface.SaveAllLoeDataAsync(dto);

            if (isSuccess)
            {
                return Ok(new { Message = "Engagementplan Inserted Successfully" });
            }
            else
            {
                return StatusCode(500, new { Message = "Failed to insert data." });
            }
        }




    }
}

