using Microsoft.AspNetCore.Mvc;
using TracePca.Dto;
using TracePca.Interface.FixedAssetsInterface;
using static TracePca.Dto.FixedAssets.AssetCreationDto;
using TracePca.Service.FixedAssetsService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860


namespace TracePca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetCreationController : ControllerBase
    {
        private AssetCreationInterface _AssetCreationInterface;
        private AssetCreationInterface _AssetCreationService;

        public AssetCreationController(AssetCreationInterface AssetCreationInterface)
        {
            _AssetCreationInterface = AssetCreationInterface;
            _AssetCreationService = AssetCreationInterface;
        }

        //LoadAssetClass
        [HttpGet("GetAssetClass")]
        public async Task<IActionResult> GetAssetClass(int compId, int custId)
        {
            try
            {
                var result = await _AssetCreationService.LoadAssetTypeAsync(compId, custId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No asset types found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Asset types fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching asset types.",
                    error = ex.Message
                });
            }
        }

        //New
        [HttpPost("New")]
        public async Task<IActionResult> AddNewAsset([FromBody] NewDto asset)
        {
            try
            {
                if (asset == null)
                    return BadRequest("Invalid asset data.");

                int newId = await _AssetCreationService.AddNewAssetAsync(asset);

                if (newId <= 0)
                    return StatusCode(500, "Failed to create asset.");

                return Ok(new
                {
                    Status = true,
                    Message = "Asset created successfully.",
                    AssetId = newId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = ex.Message
                });
            }
        }

        //Search
        [HttpGet("GetAssetRegister(search)")]
        public async Task<IActionResult> GetAssetRegister(int compId, int assetTypeId, int yearId, int custId)
        {
            try
            {
                // -------------------- VALIDATION --------------------
                if (compId <= 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "CompanyId is required.",
                        Data = (object)null
                    });
                }

                if (custId <= 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "CustomerId is required.",
                        Data = (object)null
                    });
                }

                if (yearId <= 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "YearId is required.",
                        Data = (object)null
                    });
                }

                // assetTypeId = 0 means ALL → no validation needed

                // -------------------- CALL SERVICE --------------------
                var data = await _AssetCreationService
                    .LoadAssetRegisterAsync(compId, assetTypeId, yearId, custId);

                // -------------------- SUCCESS RESPONSE --------------------
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Asset register data loaded successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                // -------------------- ERROR RESPONSE --------------------
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while loading asset register.",
                    Error = ex.Message
                });
            }
        }



    }
}
