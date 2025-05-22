using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.DigitalFiling;
using TracePca.Interface.DigitalFiling;
using TracePca.Service.DigitalFiling;

namespace TracePca.Controllers.DigitalFiling
{
    [ApiController]
    [Route("api/[controller]")]
    public class CabinetsController : ControllerBase
    {
        private readonly CabinetsInterface _cabinetsInterface;

        public CabinetsController(CabinetsInterface cabinetsInterface)
        {
            _cabinetsInterface = cabinetsInterface;
        }

        [HttpGet("LoadDepartmentDDL")]
        public async Task<IActionResult> LoadDepartmentDDL()
        {
            try
            {
                var dropdownData = await _cabinetsInterface.LoadDepartmentDDLAsync();
                return Ok(new
                {
                    statusCode = 200,
                    message = "Department dropdown data fetched successfully.",
                    data = dropdownData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load department dropdown data.", error = ex.Message });
            }
        }

        [HttpGet("LoadCabinetUserPermissionDDL")]
        public async Task<IActionResult> LoadCabinetUserPermissionDDL(int deptID)
        {
            try
            {
                var dropdownData = await _cabinetsInterface.LoadCabinetUserPermissionDDLAsync(deptID);
                return Ok(new
                {
                    statusCode = 200,
                    message = "Cabinet user permission dropdown data fetched successfully.",
                    data = dropdownData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load cabinet user permission dropdown data.", error = ex.Message });
            }
        }

        [HttpGet("LoadAllUserCabinetDLL")]
        public async Task<IActionResult> LoadAllUserCabinetDLL(int compId, int userId)
        {
            try
            {
                var dropdownData = await _cabinetsInterface.LoadAllUserCabinetDLLAsync(compId, userId);
                return Ok(new
                {
                    statusCode = 200,
                    message = "Cabinet dropdown data fetched successfully.",
                    data = dropdownData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load cabinet dropdown data.", error = ex.Message });
            }
        }

        [HttpGet("GetAllCabinets")]
        public async Task<IActionResult> GetAllSubCabinets(string status, int cabId, int userId)
        {
            try
            {
                var cabinets = await _cabinetsInterface.GetAllSubCabAsync(status, cabId, userId);

                if (cabinets == null || !cabinets.Any())
                    return Ok(new { statusCode = 200, message = "No cabinets found.", data = new List<CabinetDto>() });

                return Ok(new { statusCode = 200, message = "Cabinets fetched successfully.", data = cabinets });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Error fetching sub cabinets.", error = ex.Message });
            }
        }

        [HttpPost("UpdateCabinetStatus")]
        public async Task<IActionResult> UpdateCabinetStatus([FromBody] UpdateCabinetStatusRequestDTO request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request payload.");

                var result = await _cabinetsInterface.UpdateCabinetStatusAsync(request);

                return Ok(new { statusCode = 200, message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Error updating sub cabinet status.", error = ex.Message });
            }
        }
    }
}
