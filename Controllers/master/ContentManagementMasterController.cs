using Microsoft.AspNetCore.Mvc;
using TracePca.Dto;
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

        [HttpGet("GetActMasterData")]
        public async Task<IActionResult> GetActMasterData([FromQuery] int compId)
        {
            var (success, message, data) = await _contentManagementMasterInterface.GetActMasterDataAsync(compId);

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

        [HttpPost("UpdateMasterRecordsStatus")]
        public async Task<IActionResult> UpdateMasterRecordsStatus([FromBody] UpdateStatusRequest request)
        {
            var (success, message) = await _contentManagementMasterInterface.UpdateMasterRecordsStatusAsync(request.Ids, request.Action, request.CompId, request.UserId, request.IpAddress);

            if (!success)
                return StatusCode(500, new { success, message });

            return Ok(new { statusCode = 200, success, message });
        }

        [HttpGet("GetAuditTypeChecklistHeadingData")]
        public async Task<IActionResult> GetAuditTypeChecklistHeadingData([FromQuery] int compId)
        {
            var (success, message, data) = await _contentManagementMasterInterface.GetAuditTypeChecklistHeadingDataAsync(compId);

            if (!success)
                return StatusCode(500, new { success, message });

            return Ok(new { statusCode = 200, success, message, data });
        }

        [HttpGet("GetAuditTypeChecklistByStatus")]
        public async Task<IActionResult> GetAuditTypeChecklistByStatus([FromQuery] int typeId, [FromQuery] string status, [FromQuery] int compId)
        {
            var (success, message, data) = await _contentManagementMasterInterface.GetAuditTypeChecklistByStatusAsync(typeId, status, compId);

            if (!success)
                return StatusCode(500, new { success, message });

            return Ok(new { statusCode = 200, success, message, data });
        }

        [HttpGet("GetAuditTypeChecklistById/{id}")]
        public async Task<IActionResult> GetAuditTypeChecklistById(int id, [FromQuery] int compId)
        {
            var (success, message, data) = await _contentManagementMasterInterface.GetAuditTypeChecklistByIdAsync(id, compId);

            if (!success)
                return NotFound(new { statusCode = 404, success, message });

            return Ok(new { statusCode = 200, success, message, data });
        }


        [HttpPost("SaveOrUpdateAuditTypeChecklistAndGetRecords")]
        public async Task<IActionResult> SaveOrUpdateAuditTypeChecklistAndGetRecords([FromBody] AuditTypeChecklistMasterDTO dto)
        {
            try
            {
                var (id, message, masterList) = await _contentManagementMasterInterface.SaveOrUpdateAuditTypeChecklistAndGetRecordsAsync(dto);

                return Ok(new { statusCode = 200, success = id > 0, message = message, data = masterList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while saving/updating master data: {ex.Message}" });
            }
        }

        [HttpPost("UpdateAuditTypeChecklistStatus")]
        public async Task<IActionResult> UpdateAuditTypeChecklistStatus([FromBody] UpdateStatusRequest request)
        {
            var (success, message) = await _contentManagementMasterInterface.UpdateAuditTypeChecklistStatusAsync(request.Ids, request.Action, request.CompId, request.UserId, request.IpAddress);

            if (!success)
                return StatusCode(500, new { success, message });

            return Ok(new { statusCode = 200, success, message });
        }

        [HttpGet("GetAssignmentTaskChecklistHeadingData")]
        public async Task<IActionResult> GetAssignmentTaskChecklistHeadingData([FromQuery] int compId)
        {
            var (success, message, data) = await _contentManagementMasterInterface.GetAssignmentTaskChecklistHeadingDataAsync(compId);

            if (!success)
                return StatusCode(500, new { success, message });

            return Ok(new { statusCode = 200, success, message, data });
        }

        [HttpGet("GetAssignmentTaskChecklistByStatus")]
        public async Task<IActionResult> GetAssignmentTaskChecklistByStatus([FromQuery] int taskId, [FromQuery] string status, [FromQuery] int compId)
        {
            var (success, message, data) = await _contentManagementMasterInterface.GetAssignmentTaskChecklistByStatusAsync(taskId, status, compId);

            if (!success)
                return StatusCode(500, new { success, message });

            return Ok(new { statusCode = 200, success, message, data });
        }

        [HttpGet("GetAssignmentTaskChecklistById/{id}")]
        public async Task<IActionResult> GetAssignmentTaskChecklistById(int id, [FromQuery] int compId)
        {
            var (success, message, data) = await _contentManagementMasterInterface.GetAssignmentTaskChecklistByIdAsync(id, compId);

            if (!success)
                return NotFound(new { statusCode = 404, success, message });

            return Ok(new { statusCode = 200, success, message, data });
        }

        [HttpPost("SaveOrUpdateAssignmentTaskChecklistAndGetRecords")]
        public async Task<IActionResult> SaveOrUpdateAssignmentTaskChecklistAndGetRecords([FromBody] AssignmentTaskChecklistMasterDTO dto)
        {
            try
            {
                var (id, message, masterList) = await _contentManagementMasterInterface.SaveOrUpdateAssignmentTaskChecklistAndGetRecordsAsync(dto);

                return Ok(new { statusCode = 200, success = id > 0, message = message, data = masterList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while saving/updating master data: {ex.Message}" });
            }
        }

        [HttpPost("UpdateAssignmentTaskChecklistStatus")]
        public async Task<IActionResult> UpdateAssignmentTaskChecklistStatus([FromBody] UpdateStatusRequest request)
        {
            var (success, message) = await _contentManagementMasterInterface.UpdateAssignmentTaskChecklistStatusAsync(request.Ids, request.Action, request.CompId, request.UserId, request.IpAddress);

            if (!success)
                return StatusCode(500, new { success, message });

            return Ok(new { statusCode = 200, success, message });
        }

        [HttpGet("GetAuditSubPointsByStatus")]
        public async Task<IActionResult> GetAuditSubPointsByStatus([FromQuery] int checkPointId, [FromQuery] string status, [FromQuery] int compId)
        {
            var (success, message, data) = await _contentManagementMasterInterface.GetAuditSubPointsByStatusAsync(checkPointId, status, compId);

            if (!success)
                return StatusCode(500, new { success, message });

            return Ok(new { statusCode = 200, success, message, data });
        }

        [HttpGet("GetAuditSubPointById/{id}")]
        public async Task<IActionResult> GetAuditSubPointById(int id, [FromQuery] int compId)
        {
            var (success, message, data) = await _contentManagementMasterInterface.GetAuditSubPointByIdAsync(id, compId);

            if (!success)
                return NotFound(new { statusCode = 404, success, message });

            return Ok(new { statusCode = 200, success, message, data });
        }

        [HttpPost("SaveOrUpdateAuditSubPointAndGetRecords")]
        public async Task<IActionResult> SaveOrUpdateAuditSubPointAndGetRecords([FromBody] AuditCompletionSubPointMasterDTO dto)
        {
            try
            {
                var (id, message, masterList) = await _contentManagementMasterInterface.SaveOrUpdateAuditSubPointAndGetRecordsAsync(dto);

                return Ok(new { statusCode = 200, success = id > 0, message = message, data = masterList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while saving/updating master data: {ex.Message}" });
            }
        }

        [HttpPost("UpdateAuditSubPointStatus")]
        public async Task<IActionResult> UpdateAuditSubPointStatus([FromBody] UpdateStatusRequest request)
        {
            var (success, message) = await _contentManagementMasterInterface.UpdateAuditSubPointStatusAsync(request.Ids, request.Action, request.CompId, request.UserId, request.IpAddress);

            if (!success)
                return StatusCode(500, new { success, message });

            return Ok(new { statusCode = 200, success, message });
        }

        [HttpGet("GetTRACeModuleByStatus")]
        public async Task<IActionResult> GetTRACeModuleByStatus([FromQuery] int projectId, [FromQuery] string status, [FromQuery] int compId)
        {
            var (success, message, data) = await _contentManagementMasterInterface.GetTRACeModuleByStatusAsync(projectId, status, compId);

            if (!success)
                return StatusCode(500, new { success, message });

            return Ok(new { statusCode = 200, success, message, data });
        }

        [HttpGet("GetTRACeModuleById/{id}")]
        public async Task<IActionResult> GetTRACeModuleById(int id, [FromQuery] int compId)
        {
            var (success, message, data) = await _contentManagementMasterInterface.GetTRACeModuleByIdAsync(id, compId);

            if (!success)
                return NotFound(new { statusCode = 404, success, message });

            return Ok(new { statusCode = 200, success, message, data });
        }

        [HttpPost("SaveOrUpdateTRACeModuleAndGetRecords")]
        public async Task<IActionResult> SaveOrUpdateTRACeModuleAndGetRecords([FromBody] TRACeModuleMasterDTO dto)
        {
            try
            {
                var (id, message, masterList) = await _contentManagementMasterInterface.SaveOrUpdateTRACeModuleAndGetRecordsAsync(dto);

                return Ok(new { statusCode = 200, success = id > 0, message = message, data = masterList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while saving/updating master data: {ex.Message}" });
            }
        }

        [HttpPost("UpdateTRACeModuleStatus")]
        public async Task<IActionResult> UpdateTRACeModuleStatus([FromBody] UpdateStatusRequest request)
        {
            var (success, message) = await _contentManagementMasterInterface.UpdateTRACeModuleStatusAsync(request.Ids, request.Action, request.CompId, request.UserId, request.IpAddress);

            if (!success)
                return StatusCode(500, new { success, message });

            return Ok(new { statusCode = 200, success, message });
        }
    }
}
