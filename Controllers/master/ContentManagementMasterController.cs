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
    }
}
