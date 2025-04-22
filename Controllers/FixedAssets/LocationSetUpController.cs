using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using TracePca.Dto;
using TracePca.Interface;
using TracePca.Interface.AssetMaserInterface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.Fixed_Assets
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationSetUpController : ControllerBase
    {

        private readonly LocationSetUpInterface _LocationSetUpInterface;
        public LocationSetUpController(LocationSetUpInterface LocationSetUpInterface)
        {
            _LocationSetUpInterface = LocationSetUpInterface;

        }

        // GET: api/<LocationSetUpController>

        [HttpGet("LoadAllDropDowns")]
        public async Task<IActionResult> GetAllDropdowns(int companyId, int customerId, int parentId = 0)
        {
            // Fetch data for all dropdowns

            var divisions = await _LocationSetUpInterface.GetDivisionsAsync(companyId, customerId, 0);
            var departments = await _LocationSetUpInterface.GetDepartmentsAsync(companyId, customerId, 0);
            var locations = await _LocationSetUpInterface.GetLocationsAsync(companyId, customerId, 0);
            var headers = await _LocationSetUpInterface.GetHeadersAsync(companyId, customerId, 0);
            var subheaders = await _LocationSetUpInterface.GetSubHeadersAsync(companyId, customerId, 0);
            var bay = await _LocationSetUpInterface.GetBayiAsync(companyId, customerId, "0");
            var Itemdetails = await _LocationSetUpInterface.GetAssetsAsync(companyId, customerId, "0");

            // Create and return the response DTO with all dropdown data
          
                // Filter to ensure only the relevant field is included
                var filteredDivisions = divisions.Select(d => new DropDownDto
                { Id = d.Id, Name = d.Name, Code = d.Code, LevelCode = d.LevelCode, AmLevelCode = null }).ToList();

                var filteredDepartments = departments.Select(d => new DropDownDto
                { Id = d.Id, Name = d.Name, Code = d.Code, LevelCode = d.LevelCode, AmLevelCode = null }).ToList();

                var filteredBay = bay.Select(d => new DropDownDto
                { Id = d.Id, Name = d.Name, Code = d.Code, LevelCode = d.LevelCode, AmLevelCode = null }).ToList();

                var filteredLocations = locations.Select(d => new DropDownDto
                { Id = d.Id, Name = d.Name, Code = d.Code, LevelCode = d.LevelCode, AmLevelCode = null }).ToList();

                var filteredHeaders = headers.Select(d => new DropDownDto
                { Id = d.Id, Name = d.Name, Code = d.Code, LevelCode = null, AmLevelCode = d.AmLevelCode }).ToList();

                var filteredSubHeaders = subheaders.Select(d => new DropDownDto
                { Id = d.Id, Name = d.Name, Code = d.Code, LevelCode = null, AmLevelCode = d.AmLevelCode }).ToList();

                var filteredItems = Itemdetails.Select(d => new DropDownDto
                { Id = d.Id, Name = d.Name, Code = d.Code, LevelCode = null, AmLevelCode = d.AmLevelCode }).ToList();

                // Create and return the response DTO with filtered dropdown data
                var response = new DropResponse
                {
                    Divisions = filteredDivisions,
                    Departments = filteredDepartments,
                    Locations = filteredLocations,
                    Headers = filteredHeaders,
                    SubHeaders = filteredSubHeaders,
                    Bay = filteredBay,
                    Items = filteredItems
                };

                return Ok(response);
            }






        [HttpPost("InsertUpdateLocation")]
        public async Task<IActionResult> SaveLocation([FromBody] AddLocationDto locationDto)
        {
            if (string.IsNullOrWhiteSpace(locationDto.Name))
            {
                return BadRequest(new { message = "Location name is required." });
            }

            var result = await _LocationSetUpInterface.SaveLocationAsync(locationDto);

            if (result == -1) // Duplicate check
                return Conflict(new { message = "Entered Location Already Exists. Please enter a different location." });

            // Determine if it's an insertion or update
            string message = (locationDto.Id.HasValue && locationDto.Id.Value > 0)
                ? "Location updated successfully."
                : "Location registered successfully.";

            return Ok(new { message, locationId = result });
        }

        [HttpPost("InsertUpdateDivision")]
        public async Task<IActionResult> SaveDivision([FromBody] AddDivisionDto divisionDto)
        {
            if (string.IsNullOrWhiteSpace(divisionDto.Name))
            {
                return BadRequest(new { message = "Division name is required." });
            }

            var result = await _LocationSetUpInterface.SaveDivisionAsync(divisionDto);

            if (result == -1) // Duplicate check
            {
                return Conflict(new { message = "Entered Division Already Exists. Please enter a different Division." });
            }

            return Ok(new
            {
                message = divisionDto.Id.HasValue && divisionDto.Id.Value > 0
                    ? "Division Successfully Updated"
                    : "Division Successfully Registered",
                divisionId = result
            });
        }


        [HttpPost("InsertUpdateDepartment")]
        public async Task<IActionResult> SaveDepartment([FromBody] AddDepartmentDto departmentDto)
        {
            if (string.IsNullOrWhiteSpace(departmentDto.Name))
            {
                return BadRequest(new { message = "Department name is required." });
            }

            var result = await _LocationSetUpInterface.SaveDepartmentAsync(departmentDto);

            if (result == -1) // Duplicate check
            {
                return Conflict(new { message = "Entered Department Already Exists. Please enter a different department." });
            }

            // Ensure correct success message for insertion vs. update
            string message = departmentDto.Id.HasValue && departmentDto.Id.Value > 0
                ? "Department updated successfully."
                : "Department registered successfully.";

            return Ok(new { message = message, departmentId = result });
        }


        [HttpPost("InsertUpdateBay")]
        public async Task<IActionResult> SaveBay([FromBody] AddBayDto bayDto)
        {
            if (string.IsNullOrWhiteSpace(bayDto.Name))
            {
                return BadRequest(new { message = "Bay name is required." });
            }

            var result = await _LocationSetUpInterface.SaveBayAsync(bayDto);

            if (result == -1) // Duplicate check
            {
                return Conflict(new { message = "Entered Bay Already Exists. Please enter a different Bay." });
            }

            return Ok(new
            {
                message = bayDto.Id.HasValue && bayDto.Id.Value > 0
                    ? "Bay Successfully Updated"
                    : "Bay Successfully Registered",
                bayId = result
            });
        }


        [HttpPost("InsertUpdateHeading")]
        public async Task<IActionResult> SaveHeading([FromBody] AddHeadingDto headingDto)
        {
            if (string.IsNullOrWhiteSpace(headingDto.Name))
            {
                return BadRequest(new { message = "Heading name is required." });
            }

            var result = await _LocationSetUpInterface.SaveHeadingAsync(headingDto);

            if (result == -1)
            {
                return Conflict(new { message = "Entered Heading Already Exists. Please enter a different Heading." });
            }

            return Ok(new
            {
                message = headingDto.Id.HasValue ? "Heading Successfully Updated" : "Heading Successfully Saved",
                headingId = result
            });
        }


        [HttpPost("InsertUpdateSubHeading")]
        public async Task<IActionResult> SaveSubheadingAsync([FromBody] AddSubHeadingDto subheadingDto)
        {
            if (subheadingDto == null || string.IsNullOrWhiteSpace(subheadingDto.Name))
                return BadRequest("Invalid subheading details.");

            int result = await _LocationSetUpInterface.SaveSubHeadingAsync(subheadingDto);
            if (result == -1)
                return Conflict("Subheading already exists.");

            return Ok(new { Id = result, Message = subheadingDto.Id.HasValue ? "Subheading updated successfully." : "Subheading saved successfully." });
        }

        [HttpPost("InsertUpdateAsset")]
        public async Task<IActionResult> SaveAssetAsync([FromBody] AddAssetClassDto assetDto)
        {
            if (assetDto == null || string.IsNullOrWhiteSpace(assetDto.Name))
                return BadRequest("Invalid asset details.");

            int result = await _LocationSetUpInterface.SaveAssetAsync(assetDto);
            if (result == -1)
                return Conflict("Asset already exists.");

            return Ok(new { Id = result, Message = assetDto.Id.HasValue ? "Asset updated successfully." : "Asset saved successfully." });
        }


        [HttpGet("GetInsertedLocations")]
        public async Task<IActionResult> GetInsertedLocations(int companyId, int customerId, int topRecords = 10)
        {
            var locations = await _LocationSetUpInterface.GetInsertedLocationsAsync(companyId, customerId, topRecords);

            if (locations == null || !locations.Any())
                return NotFound(new { message = "No locations found." });

            return Ok(locations);
        }
        [HttpGet("GetItemDetails")]
        public async Task<IActionResult> GetItemDetails(
        [FromQuery] int amId,
        [FromQuery] int companyId,
        [FromQuery] int customerId)
        {
            var result = await _LocationSetUpInterface.GetItemDetailsByAmIdAsync(amId, companyId, customerId);

            if (result != null)
            {
                return Ok(new
                {
                    statusCode = 200,
                    message = "Item details fetched successfully",
                    data = result
                });
            }
            else
            {
                return NotFound(new
                {
                    statusCode = 404,
                    message = "Item not found"
                });
            }
        }






        // GET api/<LocationSetUpController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LocationSetUpController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<LocationSetUpController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LocationSetUpController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
