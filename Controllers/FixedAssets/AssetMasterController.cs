using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.DigitalFilling;

//using TracePca.Dto;
using TracePca.Interface;
using TracePca.Interface.FixedAssetsInterface;
using TracePca.Models.UserModels;
using TracePca.Service;
using TracePca.Service.FixedAssetsService;
using static TracePca.Dto.FixedAssets.AssetMasterdto;
//using static TracePca.Service.FixedAssetsService.AssetMasterService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetMasterController : ControllerBase
    {
        private AssetMasterInterface _AssetMasterInterface;
        private AssetMasterInterface _AssetMasterService;

        public AssetMasterController(AssetMasterInterface AssetMasterInterface)
        {
            _AssetMasterInterface = AssetMasterInterface;
            _AssetMasterService = AssetMasterInterface;
        }

        //LoadCustomer
        [HttpGet("GetCustomerNames")]
        public async Task<IActionResult> GetCustomerNames(int CompId)
        {
            try
            {
                var result = await _AssetMasterService.LoadCustomerAsync(CompId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No customers found.",
                        data = new List<object>()
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Customers loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching customers.",
                    error = ex.Message
                });
            }
        }

        //LoadStatus
        [HttpGet("LoadStatus")]
        public async Task<IActionResult> LoadStatus(int CompId, string Name)
        {
            try
            {
                var result = await _AssetMasterService.LoadStatusAsync(CompId, Name);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No status found.",
                        data = new List<object>()
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "status loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching status.",
                    error = ex.Message
                });
            }
        }

        //FinancialYear
        [HttpGet("GetYears")]
        public async Task<IActionResult> GetYears(int compId)
        {
            try
            {
                var result = await _AssetMasterService.GetYearsAsync(compId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No years found.",
                        data = new List<object>()
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Years fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching years.",
                    error = ex.Message
                });
            }
        }

        //Location
        [HttpGet("GetLocation")]
        public async Task<IActionResult> GetLocations(int compId, int custId)
        {
            try
            {
                var result = await _AssetMasterService.GetLocationAsync(compId, custId);

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
                var result = await _AssetMasterService.LoadDivisionAsync(compId, parentId, custId);

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
                var result = await _AssetMasterService.LoadDepartmentAsync(compId, parentId, custId);

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
                var result = await _AssetMasterService.LoadBayAsync(compId, parentId, custId);

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
                var result = await _AssetMasterService.LoadHeadingAsync(compId, custId);

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
                var result = await _AssetMasterService.LoadSubHeadingAsync(compId, parentId, custId);

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
                var result = await _AssetMasterService.LoadItemsAsync(compId, ParentId, custId);

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

    }
}


