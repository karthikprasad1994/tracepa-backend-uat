using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TracePca.Dto.FixedAssets;
using TracePca.Interface.FixedAssetsInterface;
using TracePca.Service.AssetService;
using TracePca.Service.FixedAssetsService;
using static TracePca.Dto.FixedAssets.AssetSetupDto;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FixedAssets
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetSetupController : ControllerBase
    {
        private AssetSetupInterface AssetSetupInterface;
        private AssetSetupInterface _AssetSetupService;

        public AssetSetupController(AssetSetupInterface AssetSetupInterface)
        {
            AssetSetupInterface = AssetSetupInterface;
            _AssetSetupService = AssetSetupInterface;
        }

        ////Location
        //[HttpGet("GetLocation")]
        //public async Task<IActionResult> GetLocations(int compId, int custId)
        //{
        //    try
        //    {
        //        var result = await _AssetSetupService.GetLocationAsync(compId, custId);

        //        if (result == null || !result.Any())
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "No locations found.",
        //                data = new List<object>()
        //            });
        //        }

        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Locations fetched successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while fetching locations.",
        //            error = ex.Message
        //        });
        //    }
        //}

        ////LoadDivision
        //[HttpGet("LoadDivision")]
        //public async Task<IActionResult> LoadDivisions(int compId, int parentId, int custId)
        //{
        //    try
        //    {
        //        var result = await _AssetSetupService.LoadDivisionAsync(compId, parentId, custId);

        //        if (result == null || !result.Any())
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "No divisions found.",
        //                data = new List<object>()
        //            });
        //        }

        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Divisions fetched successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while fetching divisions.",
        //            error = ex.Message
        //        });
        //    }
        //}

        ////LoadDepartment
        //[HttpGet("LoadDepartment")]
        //public async Task<IActionResult> LoadDepartment(int compId, int parentId, int custId)
        //{
        //    try
        //    {
        //        var result = await _AssetSetupService.LoadDepartmentAsync(compId, parentId, custId);

        //        if (result == null || !result.Any())
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "No Department found.",
        //                data = new List<object>()
        //            });
        //        }

        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Department fetched successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while fetching Department.",
        //            error = ex.Message
        //        });
        //    }
        //}

        ////LoadBay
        //[HttpGet("LoadBay")]
        //public async Task<IActionResult> LoadBay(int compId, int parentId, int custId)
        //{
        //    try
        //    {
        //        var result = await _AssetSetupService.LoadBayAsync(compId, parentId, custId);

        //        if (result == null || !result.Any())
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "No Bay found.",
        //                data = new List<object>()
        //            });
        //        }

        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Bay fetched successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while fetching Bay.",
        //            error = ex.Message
        //        });
        //    }
        //}

        //// LoadHeading
        //[HttpGet("LoadHeading")]
        //public async Task<IActionResult> LoadHeading(int compId, int custId)
        //{
        //    try
        //    {
        //        var result = await _AssetSetupService.LoadHeadingAsync(compId, custId);

        //        if (result == null || !result.Any())
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "No Heading found.",
        //                data = new List<object>()
        //            });
        //        }

        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Heading fetched successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while fetching Heading.",
        //            error = ex.Message
        //        });
        //    }
        //}

        ////LoadSubHeading
        //[HttpGet("LoadSubHeading")]
        //public async Task<IActionResult> LoadSubHeading(int compId, int parentId, int custId)
        //{
        //    try
        //    {
        //        var result = await _AssetSetupService.LoadSubHeadingAsync(compId, parentId, custId);

        //        if (result == null || !result.Any())
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "No SubHeading found.",
        //                data = new List<object>()
        //            });
        //        }

        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "SubHeading fetched successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while fetching SubHeading.",
        //            error = ex.Message
        //        });
        //    }
        //}

        ////AssetClassUnderSubHeading
        //[HttpGet("LoadItem")]
        //public async Task<IActionResult> LoadItem(int compId, int ParentId, int custId)
        //{
        //    try
        //    {
        //        var result = await _AssetSetupService.LoadItemsAsync(compId, ParentId, custId);

        //        if (result == null || !result.Any())
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "No Item found.",
        //                data = new List<object>()
        //            });
        //        }

        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Item fetched successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while fetching Item.",
        //            error = ex.Message
        //        });
        //    }
        //}

        //SaveAsset
        [HttpPost("SaveAsset")]
        public async Task<IActionResult> SaveAsset([FromBody] AssetMasterDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Invalid input: DTO is null",
                    Data = (object)null
                });
            }

            try
            {
                bool isUpdate = dto.AM_ID > 0;

                var result = await _AssetSetupService.SaveAssetAsync(dto);

                string successMessage = isUpdate
                    ? "Asset successfully updated."
                    : "Asset successfully created.";

                return Ok(new
                {
                    Status = 200,
                    Message = successMessage,
                    Data = new
                    {
                        UpdateOrSave = result[0],
                        Oper = result[1],
                        IsUpdate = isUpdate
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        ////EditLocation
        //[HttpGet("editLocation")]
        //public async Task<IActionResult> LoadLocationForEdit(int compId, int locationId)
        //{
        //    try
        //    {
        //        // Validate input
        //        if (compId <= 0 || locationId <= 0)
        //        {
        //            return BadRequest(new
        //            {
        //                statusCode = 400,
        //                message = "Invalid compId or locationId."
        //            });
        //        }

        //        // Call service
        //        var result = await _AssetSetupService.LoadLocationForEditAsync(compId, locationId);

        //        // If no result found
        //        if (result == null)
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "Location not found.",
        //                data = new List<object>()
        //            });
        //        }

        //        // Success
        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Location loaded successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Unexpected error
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while loading location.",
        //            error = ex.Message
        //        });
        //    }
        //}

        ////EditDivision
        //[HttpGet("editDivision")]
        //public async Task<IActionResult> LoadDivisionForEdit(int compId, int DivisionId)
        //{
        //    try
        //    {
        //        // Validate input
        //        if (compId <= 0 || DivisionId <= 0)
        //        {
        //            return BadRequest(new
        //            {
        //                statusCode = 400,
        //                message = "Invalid compId or DivisionId."
        //            });
        //        }

        //        // Call service
        //        var result = await _AssetSetupService.LoadDivisionForEditAsync(compId, DivisionId);

        //        // If no result found
        //        if (result == null)
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "Division not found.",
        //                data = new List<object>()
        //            });
        //        }

        //        // Success
        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Division loaded successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Unexpected error
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while loading Division.",
        //            error = ex.Message
        //        });
        //    }
        //}

        ////EditDepartment
        //[HttpGet("editDepartment")]
        //public async Task<IActionResult> LoadDepartmentForEdit(int compId, int DepartmentId)
        //{
        //    try
        //    {
        //        // Validate input
        //        if (compId <= 0 || DepartmentId <= 0)
        //        {
        //            return BadRequest(new
        //            {
        //                statusCode = 400,
        //                message = "Invalid compId or DepartmentId."
        //            });
        //        }

        //        // Call service
        //        var result = await _AssetSetupService.LoadDepartmentForEditAsync(compId, DepartmentId);

        //        // If no result found
        //        if (result == null)
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "Department not found.",
        //                data = new List<object>()
        //            });
        //        }

        //        // Success
        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Department loaded successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Unexpected error
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while loading Department.",
        //            error = ex.Message
        //        });
        //    }
        //}

        ////EditBay
        //[HttpGet("editBay")]
        //public async Task<IActionResult> LoadBayForEdit(int compId, int bayId)
        //{
        //    try
        //    {
        //        // 🛑 Validate inputs
        //        if (compId <= 0 || bayId <= 0)
        //        {
        //            return BadRequest(new
        //            {
        //                statusCode = 400,
        //                message = "Invalid compId or bayId."
        //            });
        //        }

        //        // 🔽 Call service
        //        var result = await _AssetSetupService.LoadBayForEditAsync(compId, bayId);

        //        // 🛑 Not found
        //        if (result == null || !result.Any())
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "Bay not found.",
        //                data = new List<object>()
        //            });
        //        }

        //        // ✅ Success
        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Bay loaded successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // ❌ Internal Error
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while loading bay.",
        //            error = ex.Message
        //        });
        //    }
        //}

        ////EditHeading
        //[HttpGet("editHeading")]
        //public async Task<IActionResult> HeadingForEdit(int compId, int HeadingId)
        //{
        //    try
        //    {
        //        // Validate input
        //        if (compId <= 0 || HeadingId <= 0)
        //        {
        //            return BadRequest(new
        //            {
        //                statusCode = 400,
        //                message = "Invalid compId or HeadingId."
        //            });
        //        }

        //        // Call service
        //        var result = await _AssetSetupService.HeadingForEditAsync(compId, HeadingId);

        //        // If no result found
        //        if (result == null)
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "Heading not found.",
        //                data = new List<object>()
        //            });
        //        }

        //        // Success
        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Heading loaded successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Unexpected error
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while loading Heading.",
        //            error = ex.Message
        //        });
        //    }
        //}

        ////EditSubHeading
        //[HttpGet("editSubHeading")]
        //public async Task<IActionResult> SubHeadingForEdit(int compId, int SubHeadingId)
        //{
        //    try
        //    {
        //        // Validate input
        //        if (compId <= 0 || SubHeadingId <= 0)
        //        {
        //            return BadRequest(new
        //            {
        //                statusCode = 400,
        //                message = "Invalid compId or SubHeadingId."
        //            });
        //        }

        //        // Call service
        //        var result = await _AssetSetupService.SubHeadingForEditAsync(compId, SubHeadingId);

        //        // If no result found
        //        if (result == null)
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "SubHeading not found.",
        //                data = new List<object>()
        //            });
        //        }

        //        // Success
        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "SubHeading loaded successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Unexpected error
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while loading SubHeading.",
        //            error = ex.Message
        //        });
        //    }
        //}

        //////EditAssetClassUnderSubHeading(LoadItem)
        //[HttpGet("editItem")]
        //public async Task<IActionResult> ItemSubHeadingForEdit(int compId, int ItemId)
        //{
        //    try
        //    {
        //        // Validate input
        //        if (compId <= 0 || ItemId <= 0)
        //        {
        //            return BadRequest(new
        //            {
        //                statusCode = 400,
        //                message = "Invalid compId or ItemId."
        //            });
        //        }

        //        // Call service
        //        var result = await _AssetSetupService.ItemForEditAsync(compId, ItemId);

        //        // If no result found
        //        if (result == null)
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "Item not found.",
        //                data = new List<object>()
        //            });
        //        }

        //        // Success
        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Item loaded successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Unexpected error
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while loading Item.",
        //            error = ex.Message
        //        });
        //    }
        //}

        //SaveLocation
        [HttpPost("saveLocation")]
        public async Task<IActionResult> SaveLocationSetup([FromBody] LocationSetupDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "Invalid request body.",
                    data = (object)null
                });
            }

            try
            {
                var result = await _AssetSetupService.SaveLocationSetupAsync(dto);

                return Ok(new
                {
                    statusCode = 200,
                    message = result[0] == 1 ? "Location saved successfully." : "Location updated successfully.",
                    data = new
                    {
                        UpdateOrSave = result[0],
                        Oper = result[1]
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = ex.Message,
                    data = (object)null
                });
            }
        }


        //SaveDivision
        [HttpPost("saveDivision")]
        public async Task<IActionResult> SaveDivision([FromBody] SaveDivisionDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Invalid input: DTO is null",
                    Data = (object)null
                });
            }

            try
            {
                bool isUpdate = dto.LS_ID > 0;

                var result = await _AssetSetupService.SaveDivisionAsync(dto);

                string successMessage = isUpdate
                    ? "Division successfully updated."
                    : "Division successfully saved.";

                return Ok(new
                {
                    Status = 200,
                    Message = successMessage,
                    Data = new
                    {
                        UpdateOrSave = result[0],
                        Oper = result[1],
                        IsUpdate = isUpdate
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        //SaveDepartment
        [HttpPost("SaveDepartment")]
        public async Task<IActionResult> SaveDepartment([FromBody] SaveDepartmentDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Invalid input: DTO is null",
                    Data = (object)null
                });
            }

            try
            {
                bool isUpdate = dto.LS_ID > 0;

                var result = await _AssetSetupService.SaveDepartmentAsync(dto);

                string successMessage = isUpdate
                    ? "Department successfully updated."
                    : "Department successfully saved.";

                return Ok(new
                {
                    Status = 200,
                    Message = successMessage,
                    Data = new
                    {
                        UpdateOrSave = result[0],
                        Oper = result[1],
                        IsUpdate = isUpdate
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        //SaveBay
        [HttpPost("SaveBay")]
        public async Task<IActionResult> SaveBay([FromBody] SaveBayDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Invalid input: DTO is null",
                    Data = (object)null
                });
            }

            try
            {
                bool isUpdate = dto.LS_ID > 0;

                var result = await _AssetSetupService.SaveBayAsync(dto);

                string successMessage = isUpdate
                    ? "Bay successfully updated."
                    : "Bay successfully created.";

                return Ok(new
                {
                    Status = 200,
                    Message = successMessage,
                    Data = new
                    {
                        UpdateOrSave = result[0],
                        Oper = result[1],
                        IsUpdate = isUpdate
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        //SaveHeading

        [HttpPost("SaveHeading")]
        public async Task<IActionResult> SaveHeading([FromBody] SaveHeadingDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new
                    {
                        status = 400,
                        message = "Invalid request body."
                    });

                var result = await _AssetSetupService.SaveHeadingAsync(dto);

                return Ok(new
                {
                    status = 200,
                    message = "Heading saved successfully.",
                    updateOrSave = result[0],
                    operation = result[1]
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while processing the request.",
                    error = ex.Message
                });
            }
        }

        //SaveSubHeading
        [HttpPost("SaveSubHeading")]
        public async Task<IActionResult> SaveSubHeading([FromBody] SaveSubHeadingDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new
                    {
                        status = 400,
                        message = "Request body cannot be empty"
                    });

                var result = await _AssetSetupService.SaveSubHeadingAsync(dto);

                return Ok(new
                {
                    status = 200,
                    message = "Saved successfully",
                    updateOrSave = result[0],
                    oper = result[1]
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while processing your request.",
                    error = ex.Message
                });
            }
        }

        //SaveAssetClassUnderSubHeading
        [HttpPost("saveAssetClass")]
        public async Task<IActionResult> SaveAsset([FromBody] SaveAssetClassDto asset)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    status = 400,
                    message = "Request body cannot be empty"
                });

            try
            {
                var result = await _AssetSetupService.SaveAssetClassAsync(asset);

                var iUpdateOrSave = result[0];   // Save or Update
                var iOper = result[1];           // Operation Result

                return Ok(new
                {
                    status = 200,
                    message = "Asset saved successfully",
                    updateOrSave = iUpdateOrSave,
                    oper = iOper
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred",
                    error = ex.Message
                });
            }
        }

        //UpdateLoaction
        [HttpPost("UpdateLocationSetup")]
        public async Task<IActionResult> UpdateLocationSetup([FromBody] UpadteLocationSetupDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "No location setup data received.",
                        data = (object)null
                    });
                }

                var result = await _AssetSetupService.UpdateLocationSetupAsync(dto);

                return Ok(new
                {
                    statusCode = 200,
                    message = "Location setup data saved successfully.",
                    data = new
                    {
                        updateOrSave = result[0],
                        operation = result[1]
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while saving location setup data.",
                    error = ex.Message
                });
            }
        }
    }
}


