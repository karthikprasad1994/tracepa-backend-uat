using Microsoft.AspNetCore.Mvc;
using TracePca.Dto;
using TracePca.Interface.FixedAssetsInterface;
using static TracePca.Dto.FixedAssets.AssetCreationDto;

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
        //[HttpPost("SaveFixedAsset")]
        //public async Task<IActionResult> SaveFixedAssetWithAudit([FromBody] SaveFixedAssetRequest request)
        //{
        //    try
        //    {
        //        if (request == null)
        //            return BadRequest(new
        //            {
        //                status = 400,
        //                message = "Invalid request body."
        //            });

        //        // STEP 1: Save Fixed Asset
        //        var (iUpdateOrSave, iOper) = await _AssetCreationService.SaveFixedAssetAsync(request.FixedAsset);

        //        // STEP 2: Save Audit Log
        //        await _AssetCreationService.SaveGRACeFormOperationsAsync(request.Audit);

        //        // SUCCESS RESPONSE
        //        return Ok(new
        //        {
        //            status = 200,
        //            message = "Fixed Asset saved successfully and Audit Log saved successfully",
        //            fixedAssetResult = new
        //            {
        //                iUpdateOrSave = iUpdateOrSave,
        //                iOper = iOper,
        //                data = request.FixedAsset
        //            },
        //            auditResult = new
        //            {
        //                message = "Audit Log saved",
        //                data = request.Audit
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            status = 500,
        //            message = "Error saving Fixed Asset or Audit Log",
        //            details = ex.Message
        //        });
        //    }
        //}


        //------------------------------

        //[HttpPost("saveFixedasset")]
        //public async Task<IActionResult> SaveFixedAsset([FromBody] SaveFixedAssetRequestDto request)
        //{
        //    // -----------------------------
        //    // 1️⃣ Validate incoming request
        //    // -----------------------------
        //    if (request == null)
        //    {
        //        return BadRequest(new
        //        {
        //            status = 400,
        //            message = "Invalid request format",
        //            errorCode = 101
        //        });
        //    }

        //    if (request.Asset == null)
        //    {
        //        return BadRequest(new
        //        {
        //            status = 400,
        //            message = "Asset object is missing",
        //            errorCode = 102
        //        });
        //    }

        //    if (request.Audit == null)
        //    {
        //        return BadRequest(new
        //        {
        //            status = 400,
        //            message = "Audit object is missing",
        //            errorCode = 103
        //        });
        //    }

        //    try
        //    {
        //        // -------------------------
        //        // 2️⃣ Call service function
        //        // -------------------------
        //        var result = await _AssetCreationService
        //            .SaveFixedAssetWithAuditAsync(request.Asset, request.Audit);

        //        int updateOrSave = result[0];
        //        int oper = result[1];

        //        // -----------------------------
        //        // 3️⃣ Check SP operation result
        //        // -----------------------------
        //        if (oper != 1)
        //        {
        //            return StatusCode(500, new
        //            {
        //                status = 500,
        //                message = "Failed to save fixed asset",
        //                errorCode = 236,
        //                dbUpdateOrSave = updateOrSave,
        //                dbOper = oper
        //            });
        //        }

        //        // -----------------------------
        //        // 4️⃣ Success response
        //        // -----------------------------
        //        return Ok(new
        //        {
        //            status = 200,
        //            message = "Fixed asset saved successfully",
        //            data = new
        //            {
        //                savedId = updateOrSave,
        //                operation = oper
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // -----------------------------
        //        // 5️⃣ Exception handling
        //        // -----------------------------
        //        return StatusCode(500, new
        //        {
        //            status = 500,
        //            message = "An error occurred while saving asset",
        //            error = ex.Message,
        //            errorCode = 500
        //        });
        //    }
        //}

        //------


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
        }

    }

//ExcelUpload

















