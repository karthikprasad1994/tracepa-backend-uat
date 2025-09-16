using Microsoft.AspNetCore.Mvc;
using TracePca.Dto;
using TracePca.Dto.Audit;
using TracePca.Interface;
using TracePca.Interface.Audit;
using TracePca.Interface.Master;

namespace TracePca.Controllers.master
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentManagementMasterController : ControllerBase
    {
        private readonly ContentManagementMasterInterface _contentManagementMasterInterface;

        public ContentManagementMasterController(ContentManagementMasterInterface contentManagementMasterInterface)
        {
            _contentManagementMasterInterface = contentManagementMasterInterface;
        }

        [HttpGet("GetAllMasterDDL")]
        public async Task<IActionResult> GetAllMasterDDL([FromQuery] int compId)
        {
            var (success, message, data) = await _contentManagementMasterInterface.LoadAllMasterDDLDataAsync(compId);

            if (!success)
                return StatusCode(500, new { success, message });

            return Ok(new { statusCode = 200, success, message, data });
        }

        [HttpGet("GetMasterDataByStatus")]
        public async Task<IActionResult> GetMasterDataByStatus([FromQuery] string type, [FromQuery] string status, [FromQuery] int compId)
        {
            var (success, message, data) = await _contentManagementMasterInterface.GetMasterDataByStatusAsync(type, status, compId);

            if (!success)
                return StatusCode(500, new { success, message });

            return Ok(new { statusCode = 200, success, message, data });
        }

        [HttpGet("GetMasterDataById/{id}")]
        public async Task<IActionResult> GetMasterDataById(int id, [FromQuery] int compId)
        {
            var (success, message, data) = await _contentManagementMasterInterface.GetMasterDataByIdAsync(id, compId);

            if (!success)
                return NotFound(new { statusCode = 404, success, message });

            return Ok(new { statusCode = 200, success, message, data });
        }


        [HttpPost("SaveOrUpdateMasterDataAndGetRecords")]
        public async Task<IActionResult> SaveOrUpdateMasterDataAndGetRecords([FromBody] ContentManagementMasterDTO dto)
        {
            try
            {
                var (id, message, masterList) = await _contentManagementMasterInterface.SaveOrUpdateMasterDataAndGetRecordsAsync(dto);

                return Ok(new { statusCode = 200, success = id > 0, message = message, data = masterList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while saving/updating master data: {ex.Message}" });
            }
        }

        [HttpPost("UpdateRecordsStatus")]
        public async Task<IActionResult> UpdateRecordsStatus([FromBody] UpdateStatusRequest request)
        {
            var (success, message) = await _contentManagementMasterInterface.UpdateRecordsStatusAsync(request.Ids, request.Action, request.CompId, request.UpdatedBy, request.IpAddress);

            if (!success)
                return StatusCode(500, new { success, message });

            return Ok(new { statusCode = 200, success, message });
        }
    }
}
