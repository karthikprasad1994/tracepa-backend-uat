using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.DigitalFiling;
using TracePca.Interface.DigitalFiling;
using TracePca.Interface.DigitalFilling;

namespace TracePca.Controllers.DigitalFiling
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoldersController : ControllerBase
    {
        private readonly FoldersInterface _foldersInterface;

        public FoldersController(FoldersInterface foldersInterface)
        {
            _foldersInterface = foldersInterface;
        }

        [HttpGet("LoadAllDDLData")]
        public async Task<IActionResult> LoadAllDDLData()
        {
            try
            {
                var dropdownData = await _foldersInterface.LoadAllDDLDataAsync();
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
                var dropdownData = await _foldersInterface.LoadCabinetsByUserIdDDLAsync(compId, userId);
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

        [HttpGet("LoadSubCabinetsByCabinetIdDDL")]
        public async Task<IActionResult> LoadSubCabinetsByCabinetIdDDL(int compId, int userId)
        {
            try
            {
                var dropdownData = await _foldersInterface.LoadSubCabinetsByCabinetIdDDLAsync(compId, userId);
                return Ok(new
                {
                    statusCode = 200,
                    message = "SubCabinet dropdown data fetched successfully.",
                    data = dropdownData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Failed to load sub cabinet dropdown data.", error = ex.Message });
            }
        }

        [HttpGet("GetAllFoldersBySubCabinetId")]
        public async Task<IActionResult> GetAllFoldersBySubCabinetId(int subCabinetId, string statusCode)
        {
            try
            {
                var cabinets = await _foldersInterface.GetAllFoldersBySubCabinetIdAsync(subCabinetId, statusCode);
                if (cabinets == null || !cabinets.Any())
                    return Ok(new { statusCode = 200, message = "No Folder found.", data = new List<SubCabinetDTO>() });

                return Ok(new { statusCode = 200, message = "Folder fetched successfully.", data = cabinets });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Error fetching Folder.", error = ex.Message });
            }
        }

        [HttpPost("UpdateFolderStatus")]
        public async Task<IActionResult> UpdateFolderStatus([FromBody] UpdateFolderStatusRequestDTO request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request payload.");

                var result = await _foldersInterface.UpdateFolderStatusAsync(request);

                return Ok(new { statusCode = 200, message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Error updating folder status.", error = ex.Message });
            }
        }

        [HttpGet("SaveOrUpdateFolder")]
        public async Task<IActionResult> SaveOrUpdateFolder(FolderDTO dto)
        {
            try
            {
                bool isUpdate = dto.FOL_FolID > 0;
                var result = await _foldersInterface.SaveOrUpdateFolderAsync(dto);
                if (result > 0)
                {
                    if (isUpdate)
                        return Ok(new { statusCode = 200, message = "Folder data updated successfully.", Data = result });
                    else
                        return Ok(new { statusCode = 200, message = "Folder data inserted successfully.", Data = result });
                }
                else
                {
                    return StatusCode(500, new { statusCode = 500, message = "No Folder data was saved." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "An error occurred while saving or updating Folder data.", error = ex.Message });
            }
        }


		[HttpGet("GetAllFoldersDetailsBySubCabinetId")]
		public async Task<IActionResult> GetAllFoldersDetailsBySubCabinetId( int subcabinet, string statusCode)
		{
			var dropdownData = await _foldersInterface.GetAllFoldersDetailsBySubCabinetIdAsync(subcabinet, statusCode);

			if (dropdownData != null && dropdownData.Any())  // Check if the collection exists and has items
			{
				return Ok(new
				{
					statusCode = 200,
					message = "Cabinet loaded successfully.",
					data = dropdownData  // Return the actual data
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
         
	}
}
