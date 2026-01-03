using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TracePca.Interface.FixedAssetsInterface;
using static TracePca.Dto.FixedAssets.AssetTransactionAdditionDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FixedAssets
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetTransactionAdditionController : ControllerBase
    {

        private AssetTransactionAdditionInterface _AssetTransactionAdditionInterface;
        private AssetTransactionAdditionInterface _AssetTransactionAdditionService;

        public AssetTransactionAdditionController(AssetTransactionAdditionInterface AssetTransactionAdditionInterface)
        {
            _AssetTransactionAdditionInterface = AssetTransactionAdditionInterface;
            _AssetTransactionAdditionService = AssetTransactionAdditionInterface;
        }

        //LoadCustomer
        //[HttpGet("GetCustomerNames")]
        //public async Task<IActionResult> GetCustomerNames(int CompId)
        //{
        //    try
        //    {
        //        var result = await _AssetTransactionAdditionService.LoadCustomerAsync(CompId);

        //        if (result == null || !result.Any())
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "No customers found.",
        //                data = new List<object>()
        //            });
        //        }

        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Customers loaded successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while fetching customers.",
        //            error = ex.Message
        //        });
        //    }
        //}

        ////LoadStatus
        //[HttpGet("LoadStatus")]
        //public async Task<IActionResult> LoadStatus(int CompId, string Name)
        //{
        //    try
        //    {
        //        var result = await _AssetTransactionAdditionService.LoadStatusAsync(CompId, Name);

        //        if (result == null || !result.Any())
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "No status found.",
        //                data = new List<object>()
        //            });
        //        }

        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "status loaded successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while fetching status.",
        //            error = ex.Message
        //        });
        //    }
        //}

        ////FinancialYear
        //[HttpGet("GetYears")]
        //public async Task<IActionResult> GetYears(int compId)
        //{
        //    try
        //    {
        //        var result = await _AssetTransactionAdditionService.GetYearsAsync(compId);

        //        if (result == null || !result.Any())
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "No years found.",
        //                data = new List<object>()
        //            });
        //        }

        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Years fetched successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while fetching years.",
        //            error = ex.Message
        //        });
        //    }
        //}

        //AddDetails

        //[HttpPost("add-details")]
        //public async Task<IActionResult> AddAssetDetails([FromBody] AddAssetDetailsRequest request)
        //{
        //    try
        //    {
        //        // Model validation
        //        if (request == null)
        //            return BadRequest("Invalid request payload.");

        //        // Call service
        //        var result = await _AssetTransactionAdditionService.AddAssetDetailsAsync(request);

        //        // Return success
        //        return Ok(new
        //        {
        //            Status = 200,
        //            Message = "Asset details added successfully.",
        //            Data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Return validation error → 400
        //        if (ex.Message.Contains("Select"))
        //        {
        //            return BadRequest(new
        //            {
        //                Status = 400,
        //                Error = ex.Message
        //            });
        //        }

        //        // Return internal server error → 500
        //        return StatusCode(500, new
        //        {
        //            Status = 500,
        //            Error = ex.Message
        //        });
        //    }
        //}

        //LoadAsset
            [HttpGet("LoadfixedAssetTypes")]
            public async Task<IActionResult> LoadFxdAssetType(int compId, int custId)
            {
                try
                {
                    var result = await _AssetTransactionAdditionService.LoadFxdAssetTypeAsync(compId, custId);

                    if (result == null || !result.Any())
                    {
                        return NotFound(new
                        {
                            statusCode = 404,
                            message = "No asset types found.",
                            data = new List<object>()
                        });
                    }

                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Fixed asset types loaded successfully.",
                        data = result
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        statusCode = 500,
                        message = "An error occurred while loading asset types.",
                        error = ex.Message
                    });
                }
            }
        

        //SaveTransactionAddition

        [HttpPost("SaveTrancastionAdditionAsset")]
            public async Task<IActionResult> SaveAsset([FromBody] SaveAssetRequest request)
            {
                if (request == null || request.Asset == null || request.Audit == null)
                    return BadRequest("Invalid request payload.");

                try
                {
                    int result = await _AssetTransactionAdditionService.SaveTransactionAssetAndAuditAsync(request.Asset, request.Audit);

                    if (result > 0)
                        return Ok(new
                        {
                            Status = true,
                            Message = "Asset saved successfully.",
                            SavedAssetId = result
                        });

                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        Status = false,
                        Message = "Failed to save asset."
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        Status = false,
                        Message = ex.Message
                    });
                }
            }

        //LoadVoucherNo(transactionno)
        [HttpGet("LoadVoucherNo(transactionno)")]
            public async Task<IActionResult> ExistingTransactionNo(
                int compId,
                int yearId,
                int custId)
            {
                try
                {
                    var result = await _AssetTransactionAdditionService.ExistingTransactionNoAsync(
                        compId, yearId, custId);

                    if (result == null || !result.Any())
                    {
                        return NotFound(new
                        {
                            statusCode = 404,
                            message = "No asset transaction numbers found.",
                            data = new List<object>()
                        });
                    }

                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Asset transaction numbers fetched successfully.",
                        data = result
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        statusCode = 500,
                        message = "An error occurred while fetching asset transaction numbers.",
                        error = ex.Message
                    });
                }
            }

        //ExcelUpload
        [HttpGet("DownloadableAssetAdditionExcelFile")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public IActionResult DownloadAssetAdditionExcelTemplate()

        {
            var result = _AssetTransactionAdditionService.GetAssetAdditionExcelTemplate();

            if (result.FileBytes == null)
                return NotFound("File not found.");

            return File(result.FileBytes, result.ContentType, result.FileName);
        }


        //SaveDetails
            [HttpPost("SaveDetails")]
            public async Task<IActionResult> SaveFixedAsset([FromBody] SaveFixedAssetRequestDto request)
            {
                if (request == null || request.Header == null || request.Audit == null)
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "Invalid request payload"
                    });
                }

                try
                {
                    var result = await _AssetTransactionAdditionService.SaveFixedAssetAsync(
                        request.Header,
                        request.Details,
                        request.Audit
                    );

                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Fixed asset saved successfully",
                        updateOrSave = result.UpdateOrSave,
                        operId = result.Oper
                    });
                }
                catch (SqlException ex)
                {
                    return StatusCode(500, new
                    {
                        statusCode = 500,
                        message = "Database error occurred",
                        error = ex.Message
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        statusCode = 500,
                        message = "An unexpected error occurred",
                        error = ex.Message
                    });
                }
            }
                            


    }

}