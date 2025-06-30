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

        [HttpPost("SaveOrUpdateAuditFrequencyData")]
        public async Task<IActionResult> SaveOrUpdateAuditFrequencyData([FromBody] ContentManagementMasterDTO dto)
        {
            try
            {
                var (isSaved, message) = await _contentManagementMasterInterface.SaveOrUpdateAuditFrequencyDataAsync(dto);
                if (isSaved > 0)
                {
                    return Ok(new { statusCode = 200, success = true, message = message });
                }
                else
                {
                    return Ok(new { statusCode = 200, success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while saving/updating master data: {ex.Message}" });
            }
        }

        [HttpPost("SaveOrUpdateEngagementFeesData")]
        public async Task<IActionResult> SaveOrUpdateEngagementFeesData([FromBody] ContentManagementMasterDTO dto)
        {
            try
            {
                var (isSaved, message) = await _contentManagementMasterInterface.SaveOrUpdateEngagementFeesDataAsync(dto);
                if (isSaved > 0)
                {
                    return Ok(new { statusCode = 200, success = true, message = message });
                }
                else
                {
                    return Ok(new { statusCode = 200, success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while saving/updating master data: {ex.Message}" });
            }
        }

        [HttpPost("SaveOrUpdateTypeofReportData")]
        public async Task<IActionResult> SaveOrUpdateTypeofReportData([FromBody] ContentManagementMasterDTO dto)
        {
            try
            {
                var (isSaved, message) = await _contentManagementMasterInterface.SaveOrUpdateTypeofReportDataAsync(dto);
                if (isSaved > 0)
                {
                    return Ok(new { statusCode = 200, success = true, message = message });
                }
                else
                {
                    return Ok(new { statusCode = 200, success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while saving/updating master data: {ex.Message}" });
            }
        }

        [HttpPost("SaveOrUpdateTypeofTestData")]
        public async Task<IActionResult> SaveOrUpdateTypeofTestData([FromBody] ContentManagementMasterDTO dto)
        {
            try
            {
                var (isSaved, message) = await _contentManagementMasterInterface.SaveOrUpdateTypeofTestDataAsync(dto);
                if (isSaved > 0)
                {
                    return Ok(new { statusCode = 200, success = true, message = message });
                }
                else
                {
                    return Ok(new { statusCode = 200, success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while saving/updating master data: {ex.Message}" });
            }
        }

        [HttpPost("SaveOrUpdateManagementRepresentationsData")]
        public async Task<IActionResult> SaveOrUpdateManagementRepresentationsData([FromBody] ContentManagementMasterDTO dto)
        {
            try
            {
                var (isSaved, message) = await _contentManagementMasterInterface.SaveOrUpdateManagementRepresentationsDataAsync(dto);
                if (isSaved > 0)
                {
                    return Ok(new { statusCode = 200, success = true, message = message });
                }
                else
                {
                    return Ok(new { statusCode = 200, success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while saving/updating master data: {ex.Message}" });
            }
        }

        [HttpPost("SaveOrUpdateAuditTaskOrAssignmentTaskData")]
        public async Task<IActionResult> SaveOrUpdateAuditTaskOrAssignmentTaskData([FromBody] ContentManagementMasterDTO dto)
        {
            try
            {
                var (isSaved, message) = await _contentManagementMasterInterface.SaveOrUpdateAuditTaskOrAssignmentTaskDataAsync(dto);
                if (isSaved > 0)
                {
                    return Ok(new { statusCode = 200, success = true, message = message });
                }
                else
                {
                    return Ok(new { statusCode = 200, success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while saving/updating master data: {ex.Message}" });
            }
        }

        [HttpPost("SaveOrUpdateWorkpaperChecklistMasterData")]
        public async Task<IActionResult> SaveOrUpdateWorkpaperChecklistMasterData([FromBody] ContentManagementMasterDTO dto)
        {
            try
            {
                var (isSaved, message) = await _contentManagementMasterInterface.SaveOrUpdateWorkpaperChecklistMasterDataAsync(dto);
                if (isSaved > 0)
                {
                    return Ok(new { statusCode = 200, success = true, message = message });
                }
                else
                {
                    return Ok(new { statusCode = 200, success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while saving/updating master data: {ex.Message}" });
            }
        }

        [HttpPost("SaveOrUpdateAuditCompletionCheckPointsData")]
        public async Task<IActionResult> SaveOrUpdateAuditCompletionCheckPointsData([FromBody] ContentManagementMasterDTO dto)
        {
            try
            {
                var (isSaved, message) = await _contentManagementMasterInterface.SaveOrUpdateAuditCompletionCheckPointsDataAsync(dto);
                if (isSaved > 0)
                {
                    return Ok(new { statusCode = 200, success = true, message = message });
                }
                else
                {
                    return Ok(new { statusCode = 200, success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred while saving/updating master data: {ex.Message}" });
            }
        }
    }
}
