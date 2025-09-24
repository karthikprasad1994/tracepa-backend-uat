using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.DigitalFiling;
using TracePca.Interface.DigitalFiling;
using TracePca.Interface.DigitalFilling;

namespace TracePca.Controllers.DigitalFiling
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubCabinetsController : ControllerBase
    {
        private readonly SubCabinetsInterface _cabinetsInterface;

        public SubCabinetsController(SubCabinetsInterface cabinetsInterface)
        {
            _cabinetsInterface = cabinetsInterface;
        }

        [HttpGet("LoadAllDDLData")]
        public async Task<IActionResult> LoadAllDDLData()
        {
            try
            {
                var dropdownData = await _cabinetsInterface.LoadAllDDLDataAsync();
                return Ok(new
                {
                    statusCode = 200,
                    message = "All dropdown data fetched successfully.",
                    data = dropdownData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load all dropdown data.", error = ex.Message });
            }
        }

        [HttpGet("LoadCabinetsByUserIdDDL")]
        public async Task<IActionResult> LoadCabinetsByUserIdDDL(int compId, int userId)
        {
            try
            {
                var dropdownData = await _cabinetsInterface.LoadCabinetsByUserIdDDLAsync(compId, userId);
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

        [HttpGet("LoadPermissionUsersByDeptIdDDL")]
        public async Task<IActionResult> LoadPermissionUsersByDeptIdDDL(int deptID)
        {
            try
            {
                var dropdownData = await _cabinetsInterface.LoadPermissionUsersByDeptIdDDLAsync(deptID);
                return Ok(new
                {
                    statusCode = 200,
                    message = "Department user permission dropdown data fetched successfully.",
                    data = dropdownData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load department user permission dropdown data.", error = ex.Message });
            }
        }



		[HttpGet("GetAllSubCabinetsDetailsByCabinetAndUserId")]
		public async Task<IActionResult> GetAllSubCabinetsDetailsByCabinetAndUserId(int userId, int cabinetId, string statusCode)
		{
			var dropdownData = await _cabinetsInterface.GetAllSubCabinetsDetailsByCabinetAndUserIdAsync(userId, cabinetId, statusCode);

			if (dropdownData != null && dropdownData.Any())
			{
				return Ok(new
				{
					statusCode = 200,
					message = "Cabinet loaded successfully.",
					data = dropdownData
				});
			}
			else
			{
				return NotFound(new
				{
					statusCode = 404,
					message = "No data found for the given criteria."
				});
			}
		}

		[HttpGet("GetAllSubCabinetsByCabinetAndUserId")]
        public async Task<IActionResult> GetAllSubCabinetsByCabinetAndUserId(int userId, int cabinetId, string statusCode)
        {
            try
            {
                var cabinets = await _cabinetsInterface.GetAllSubCabinetsByCabinetAndUserIdAsync(userId, cabinetId, statusCode);
                if (cabinets == null || !cabinets.Any())
                    return Ok(new { statusCode = 200, message = "No Sub cabinets found.", data = new List< SubCabinetDTO>() });

                return Ok(new { statusCode = 200, message = "Sub cabinets fetched successfully.", data = cabinets });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Error fetching sub cabinets.", error = ex.Message });
            }
        }

        [HttpPost("UpdateSubCabinetStatus")]
        public async Task<IActionResult> UpdateSubCabinetStatus([FromBody] UpdateSubCabinetStatusRequestDTO request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request payload.");

                var result = await _cabinetsInterface.UpdateSubCabinetStatusAsync(request);

                return Ok(new { statusCode = 200, message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Error updating sub cabinet status.", error = ex.Message });
            }
        }

        [HttpGet("CheckExtraPermissionsToCabinet")]
        public async Task<IActionResult> CheckExtraPermissionsToCabinet(int userId, int cabinetId)
        {
            try
            {
                var result = await _cabinetsInterface.CheckExtraPermissionsToCabinetAsync(userId, cabinetId, "CBP_Create");
                if (result > 0)
                {
                    return Ok(new { statusCode = 200, message = "Cabinet permission is Assigned.", Data = result });
                }
                else
                {
                    return StatusCode(500, new { statusCode = 500, message = "Cabinet permission is not Assigned." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Error checking cabinet permission.", error = ex.Message });
            }
        }

        [HttpGet("GetSubCabinetPermissionByLevelId")]
        public async Task<IActionResult> GetSubCabinetPermissionByLevelId(int compId, int deptID, int userId, int subCabinetId)
        {
            try
            {
                var permissions = await _cabinetsInterface.GetSubCabinetPermissionByLevelIdAsync(compId, deptID, userId, subCabinetId);
                if (permissions == null)
                    return Ok(new { statusCode = 200, message = "No permissions found for this sub cabinet.", data = new SubCabinetPermissionDTO() });

                return Ok(new { statusCode = 200, message = "Sub cabinet permissions fetched successfully.", data = permissions });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "An error occurred while fetching sub cabinet permissions.", error = ex.Message });
            }
        }

        [HttpGet("SaveOrUpdateSubCabinet")]
        public async Task<IActionResult> SaveOrUpdateSubCabinet(SubCabinetDTO dto)
        {
            try
            {
                bool isUpdate = dto.CBN_ID > 0;
                var result = await _cabinetsInterface.SaveOrUpdateSubCabinetAsync(dto);
                if (result > 0)
                {
                    if (isUpdate)
                        return Ok(new { statusCode = 200, message = "Sub cabinet data updated successfully.", Data = result });
                    else
                        return Ok(new { statusCode = 200, message = "Sub cabinet data inserted successfully.", Data = result });
                }
                else
                {
                    return StatusCode(500, new { statusCode = 500, message = "No Sub cabinet data was saved." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "An error occurred while saving or updating Sub cabinet data.", error = ex.Message });
            }
        }
    }
}
