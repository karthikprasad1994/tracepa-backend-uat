using Microsoft.AspNetCore.Mvc;
using TracePca.Dto;
using TracePca.Interface.FixedAssetsInterface;
using static TracePca.Dto.FixedAssets.AssetCreationDto;
using static TracePca.Service.FixedAssetsService.AssetCreationService;

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


        //DownloadExcel
        [HttpGet("DownloadableAssetMasterExcelFile")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public IActionResult DownloadAssetMasterExcelTemplate()

        {
            var result = _AssetCreationService.GetAssetMasterExcelTemplate();

            if (result.FileBytes == null)
                return NotFound("File not found.");

            return File(result.FileBytes, result.ContentType, result.FileName);
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
                        data = new List<object>()
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
                var data = await _AssetCreationService.LoadAssetRegisterAsync(compId, assetTypeId, yearId, custId);

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

        //UnitsofMeasurement
        [HttpGet("LoadUnits/{compId}")]
        public async Task<IActionResult> LoadUnits(int compId)
        {
            try
            {
                var result = await _AssetCreationService.LoadUnitsOfMeasureAsync(compId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "No units found.",
                        data = new List<object>()
                    });
                }

                return Ok(new
                {
                    status = 200,
                    message = "Units loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while loading units.",
                    error = ex.Message
                });
            }
        }

        //LoadSuplierName
        [HttpGet("LoadSuplierName")]
        public async Task<IActionResult> GetExistingSupplier(int compId, int supplierId)
        {
            try
            {
                var result = await _AssetCreationService.LoadExistingSupplierAsync(compId, supplierId);

                if (result == null || !result.Any())
                    return NotFound(new
                    {
                        status = 404,
                        message = "Supplier not found.",
                        data = new List<object>()
                    });

                return Ok(new
                {
                    status = 200,
                    message = "Supplier data loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while loading supplier.",
                    error = ex.Message
                });
            }
        }

        //EditSuplierName
        [HttpGet("EditSupplierName")]
        public async Task<IActionResult> GetSupplierDetails(int compId, int supplierId)
        {
            try
            {
                var result = await _AssetCreationService.EditSupplierDetailsAsync(compId, supplierId);

                if (result == null || !result.Any())
                    return NotFound(new
                    {
                        status = 404,
                        message = "Supplier details not found.",
                        data = new List<object>()
                    });

                return Ok(new
                {
                    status = 200,
                    message = "Supplier details loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while loading supplier details.",
                    error = ex.Message
                });
            }
        }

        //SaveSuplierDetails
        [HttpPost("SaveSupplierDetails")]
        public async Task<IActionResult> SaveSupplierDetails([FromBody] SaveSupplierDto model)
        {
            try
            {
                var (iUpdateOrSave, iOper) = await _AssetCreationService.SaveSupplierDetailsAsync(model);

                // iUpdateOrSave: 1 = Saved, 2 = Updated (Based on your VB logic)

                if (iUpdateOrSave == 0)
                {
                    return BadRequest(new
                    {
                        status = 400,
                        message = "Failed to save or update supplier.",
                        data = new { iUpdateOrSave, iOper }
                    });
                }

                string msg = iUpdateOrSave == 1 ? "Supplier saved successfully." : "Supplier updated successfully.";

                return Ok(new
                {
                    status = 200,
                    message = msg,
                    data = new
                    {
                        iUpdateOrSave,
                        iOper
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while saving supplier details.",
                    error = ex.Message
                });
            }
        }

        //SaveAsset
            [HttpPost("SaveFixedAsset")]
            public async Task<IActionResult> SaveFixedAsset([FromBody] FixedAssetRequest request)
            {
                if (request == null || request.Asset == null || request.Audit == null)
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "Invalid request data."
                    });
                }

                try
                {
                    int savedId = await _AssetCreationService.SaveFixedAssetAsync(request.Asset, request.Audit);

                    return Ok(new
                    {
                        status = 200,
                        message = "Fixed Asset saved successfully.",
                        data = new { MasterID = savedId }
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        status = false,
                        message = "An error occurred while saving Fixed Asset.",
                        error = ex.Message
                    });
                }
            }

        //Generete

        [HttpGet("generate")]
        public async Task<IActionResult> GenerateAssetCode(
            [FromQuery] int compId,
            [FromQuery] int custId,
            [FromQuery] int locationId,
            [FromQuery] int divisionId,
            [FromQuery] int departmentId,
            [FromQuery] int bayId,
            [FromQuery] string assetCode)
        {
            if (string.IsNullOrWhiteSpace(assetCode))
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    success = false,
                    message = "AssetCode is required",
                    data = (string)null
                });
            }

            try
            {
                var result = await _AssetCreationService.GenerateAssetCodeAsync(
                    compId,
                    custId,
                    locationId,
                    divisionId,
                    departmentId,
                    bayId,
                    assetCode
                );

                return Ok(new
                {
                    statusCode = 200,
                    success = true,
                    message = "Asset code generated successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    success = false,
                    message = ex.Message,
                    data = (string)null
                });
            }
        }

        //UploadAssetCreationExcel
        [HttpPost("UploadAssetCreation")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFixedAssetExcel(
            [FromForm] UploadAssetCreationRequestDto dto
        )
        {
            if (dto.File == null || dto.File.Length == 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Excel file is required"
                });
            }

            try
            {
                var result = await _AssetCreationService.UploadAssetExcelAsync(
                    dto.CompId,
                    dto.CustId,
                    dto.YearId,
                    dto.UserId,
                    dto.File
                );

                return Ok(new
                {
                    status = 200,
                    success = true,
                    message = "Fixed Asset Excel uploaded successfully",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new
                {
                    status = 404,
                    success = false,
                    message = "Supplier details not found.",
                    data = new List<object>()
                });
            }
            catch (AssetUploadException ex)
            {
                return BadRequest(new
                {
                    status = 400,
                    success = false,
                    message = "Excel validation failed",
                    errors = ex.Errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    status = 500,
                    success = false,
                    message = "An error occurred while loading supplier.",
                    error = ex.Message
                });
            }
        }
    }

    }



















