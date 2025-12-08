using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FixedAssetsInterface;
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

        //Location
        [HttpGet("GetLocation")]
        public async Task<IActionResult> GetLocations(int compId, int custId)
        {
            try
            {
                var result = await _AssetSetupService.GetLocationAsync(compId, custId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No locations found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Locations fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching locations.",
                    error = ex.Message
                });
            }
        }

        //LoadDivision
        [HttpGet("LoadDivision")]
        public async Task<IActionResult> LoadDivisions(int compId, int parentId, int custId)
        {
            try
            {
                var result = await _AssetSetupService.LoadDivisionAsync(compId, parentId, custId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No divisions found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Divisions fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching divisions.",
                    error = ex.Message
                });
            }
        }

        //LoadDepartment
        [HttpGet("LoadDepartment")]
        public async Task<IActionResult> LoadDepartment(int compId, int parentId, int custId)
        {
            try
            {
                var result = await _AssetSetupService.LoadDepartmentAsync(compId, parentId, custId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Department found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Department fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching Department.",
                    error = ex.Message
                });
            }
        }

        //LoadBay
        [HttpGet("LoadBay")]
        public async Task<IActionResult> LoadBay(int compId, int parentId, int custId)
        {
            try
            {
                var result = await _AssetSetupService.LoadBayAsync(compId, parentId, custId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Bay found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Bay fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching Bay.",
                    error = ex.Message
                });
            }
        }

        // LoadHeading
        [HttpGet("LoadHeading")]
        public async Task<IActionResult> LoadHeading(int compId, int custId)
        {
            try
            {
                var result = await _AssetSetupService.LoadHeadingAsync(compId, custId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Heading found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Heading fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching Heading.",
                    error = ex.Message
                });
            }
        }

        //LoadSubHeading
        [HttpGet("LoadSubHeading")]
        public async Task<IActionResult> LoadSubHeading(int compId, int parentId, int custId)
        {
            try
            {
                var result = await _AssetSetupService.LoadSubHeadingAsync(compId, parentId, custId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No SubHeading found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "SubHeading fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching SubHeading.",
                    error = ex.Message
                });
            }
        }

        //AssetClassUnderSubHeading
        [HttpGet("LoadItem")]
        public async Task<IActionResult> LoadItem(int compId, int ParentId, int custId)
        {
            try
            {
                var result = await _AssetSetupService.LoadItemsAsync(compId, ParentId, custId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Item found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Item fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching Item.",
                    error = ex.Message
                });
            }
        }

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

        //EditLocation
        [HttpGet("editLocation")]
        public async Task<IActionResult> LoadLocationForEdit(int compId, int locationId)
        {
            try
            {
                // Validate input
                if (compId <= 0 || locationId <= 0)
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "Invalid compId or locationId."
                    });
                }

                // Call service
                var result = await _AssetSetupService.LoadLocationForEditAsync(compId, locationId);

                // If no result found
                if (result == null)
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "Location not found."
                    });
                }

                // Success
                return Ok(new
                {
                    statusCode = 200,
                    message = "Location loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                // Unexpected error
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while loading location.",
                    error = ex.Message
                });
            }
        }

        //EditDivision
        [HttpGet("editDivision")]
        public async Task<IActionResult> LoadDivisionForEdit(int compId, int DivisionId)
        {
            try
            {
                // Validate input
                if (compId <= 0 || DivisionId <= 0)
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "Invalid compId or DivisionId."
                    });
                }

                // Call service
                var result = await _AssetSetupService.LoadDivisionForEditAsync(compId, DivisionId);

                // If no result found
                if (result == null)
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "Division not found."
                    });
                }

                // Success
                return Ok(new
                {
                    statusCode = 200,
                    message = "Division loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                // Unexpected error
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while loading Division.",
                    error = ex.Message
                });
            }
        }

        //EditDepartment
        [HttpGet("editDepartment")]
        public async Task<IActionResult> LoadDepartmentForEdit(int compId, int DepartmentId)
        {
            try
            {
                // Validate input
                if (compId <= 0 || DepartmentId <= 0)
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "Invalid compId or DepartmentId."
                    });
                }

                // Call service
                var result = await _AssetSetupService.LoadDepartmentForEditAsync(compId, DepartmentId);

                // If no result found
                if (result == null)
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "Department not found."
                    });
                }

                // Success
                return Ok(new
                {
                    statusCode = 200,
                    message = "Department loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                // Unexpected error
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while loading Department.",
                    error = ex.Message
                });
            }
        }

    }
}
