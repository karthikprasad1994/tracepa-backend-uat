using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FixedAssetsInterface;
using static TracePca.Dto.FixedAssets.AssetTransactionDeletionnDto;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FixedAssets
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetTransactionDeletionController : ControllerBase
    {
        private AssetTransactionDeletionInterface _AssetTransactionDeletionInterface;
        private AssetTransactionDeletionInterface _AssetTransactionDeletionService;

        public AssetTransactionDeletionController(AssetTransactionDeletionInterface AssetTransactionDeletionInterface)
        {
            _AssetTransactionDeletionInterface = AssetTransactionDeletionInterface;
            _AssetTransactionDeletionService = AssetTransactionDeletionInterface;
        }

        //Deletee
            [HttpPost("DeleteTransactionDeletionn")]
            public async Task<IActionResult> SaveFixedAssetDeletionn(
                [FromBody] AssetDeletionnRequest request)
            {
                if (request == null)
                    return BadRequest("Request body is required.");

                try
                {
                    var result = await _AssetTransactionDeletionService.SaveFixedAssetDeletionnAsync(
                        request.AssetDeletion,
                        request.Audit);

                    return Ok(new
                    {
                        StatusCode = 200,
                        Message = result.UpdateOrSave == 2
                                    ? "Saved successfully"
                                    : "Updated successfully",
                        UpdateOrSave = result.UpdateOrSave,
                        OperationId = result.Oper
                    });
                }
                catch (ArgumentNullException ex)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = ex.Message
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Message = "Internal server error",
                        Error = ex.Message
                    });
                }
            }
       
    }
}
