using Microsoft.AspNetCore.Mvc;
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

    }
}

