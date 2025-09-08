using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TracePca.Dto.AssetRegister;
using TracePca.Dto.Audit;
using TracePca.Dto.DigitalFilling;
using TracePca.Interface.Audit;
using TracePca.Interface.DigitalFilling;
using TracePca.Interface.FixedAssetsInterface;

namespace TracePca.Controllers.DigitalFilling
{
    [Route("api/[controller]")]
    [ApiController]
    public class CabinetController : ControllerBase
    {
        
        private CabinetInterface _CabinetInterface;
        public CabinetController(CabinetInterface CabinetInterface)
        {
            _CabinetInterface = CabinetInterface;

        }
       
        [HttpGet("LoadCabinet")]
        public async Task<IActionResult> LoadCabinet(int deptId, int userId, int compID)
        {
            var dropdownData = await _CabinetInterface.LoadCabinetAsync(deptId, userId, compID);

			if (dropdownData != null && dropdownData.Any())  // Check if the collection exists and has items
			{
				return Ok(new
				{
					statusCode = 200,
					message = "Cabinet loaded successfully.",
					data = dropdownData  // Return the actual data
				});
			}
			else
			{
				return NotFound(new
				{
					statusCode = 404,
					message = "No data found for the given criteria."
				});
			}
		}


        [HttpPost("CreateCabinet")]
        public async Task<IActionResult> CreateCabinet(string CabinetName, int deptId, int userId, int compID, [FromBody] CabinetDto dto)
        {
            var result = await _CabinetInterface.CreateCabinetAsync(CabinetName,deptId, userId, compID, dto);

            if (result > 0)
            {
                return Ok(new { statusCode = 200, message = "Cabinet created successfully.", Data = result });
            }
            else
            {
                return StatusCode(500, new { statusCode = 500, message = "No Cabient data is saved." });
            }
        }


        [HttpPut("UpdateCabinet")]
        public async Task<IActionResult> UpdateCabinet(string CabinetName, int iCabinetId, int userId,  int compID, [FromBody] CabinetDto dto)
        {
            var result = await _CabinetInterface.UpdateCabinetAsync(CabinetName, iCabinetId, userId, compID, dto);

            if (result == 0)
            {
                return Ok(new { statusCode = 200, message = "Cabinet updated successfully.", Data = result });
            }
            else
            {
                return StatusCode(500, new { statusCode = 500, message = "No Cabient data is updated." });
            }
        }


        [HttpPost("IndexDocuments")]
        public async Task<IActionResult> IndexDocuments([FromForm] IndexDocumentDto dto)
        {
            try
            {
                var result = await _CabinetInterface.IndexDocuments(dto);

                if (result.StartsWith("Error"))
                {
                    return StatusCode(500, result); // Internal Server Error
                }
                else
                {
                    return Ok(result); // Success
                }
                
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


		[HttpGet("LoadDescriptor")]
		public async Task<IActionResult> LoadDescriptor(int iDescId, int compID)
		{
			var dropdownData = await _CabinetInterface.LoadDescriptorAsync(iDescId, compID);

			if (dropdownData != null && dropdownData.Any())  // Check if the collection exists and has items
			{
				return Ok(new
				{
					statusCode = 200,
					message = "Descriptor loaded successfully.",
					data = dropdownData  // Return the actual data
				});
			}
			else
			{
				return NotFound(new
				{
					statusCode = 404,
					message = "No data found for the given criteria."
				});
			}
		}


		[HttpPost("CreateDescriptor")]
		public async Task<IActionResult> CreateDescriptor(string DESC_NAME, string DESC_NOTE, string DESC_DATATYPE, string DESC_SIZE, [FromBody] DescriptorDto dto)
		{
			var result = await _CabinetInterface.CreateDescriptorAsync(DESC_NAME, DESC_NOTE, DESC_DATATYPE, DESC_SIZE, dto);

			if (result > 0)
			{
				return Ok(new { statusCode = 200, message = "Descriptor created successfully.", Data = result });
			}
			else
			{
				return StatusCode(500, new { statusCode = 500, message = "No Cabient data is saved." });
			}
		}

		[HttpPut("UpdateDescriptor")]
		public async Task<IActionResult> UpdateDescriptor([FromBody] DescriptorDto dto)
		{
			var result = await _CabinetInterface.UpdateDescriptorAsync(dto);

			if (result > 0)
			{
				return Ok(new { statusCode = 200, message = "Descriptor updated successfully.", Data = result });
			}
			else
			{
				return StatusCode(500, new { statusCode = 500, message = "No Cabient data is updated." });
			}
		}


		[HttpPost("LoadDocumentType")]
		public async Task<IActionResult> LoadDocumentType(int iDocTypeID, int iDepartmentID,   [FromBody] DocumentTypeDto dto)
		{
			var result = await _CabinetInterface.LoadDocumentTypeAsync(iDocTypeID, iDepartmentID, dto);

			if (result != null && result.Any())  // Check if the collection exists and has items
			{
				return Ok(new
				{
					statusCode = 200,
					message = "Document Type loaded successfully.",
					data = result  // Return the actual data
				});
			}
			else
			{
				return NotFound(new
				{
					statusCode = 404,
					message = "No data found for the given criteria."
				});
			}

		}


		[HttpPost("CreateDocumentType")]
		public async Task<IActionResult> CreateDocumentType(string DocumentName, string DocumentNote, string DepartmentId,  [FromBody] DocumentTypeDto dto)
		{
			var result = await _CabinetInterface.CreateDescriptorAsync(DocumentName, DocumentNote, DepartmentId, dto);

			if (result > 0)
			{
				return Ok(new { statusCode = 200, message = "Document Type created successfully.", Data = result });
			}
			else
			{
				return StatusCode(500, new { statusCode = 500, message = "No Document Type data is saved." });
			}
		}


		[HttpPut("UpdateDocumentType")]
		public async Task<IActionResult> UpdateDocumentType(int iDocTypeID, string DocumentName, string DocumentNote, [FromBody] DocumentTypeDto dto)
		{
			var result = await _CabinetInterface.UpdateDocumentTypeAsync(iDocTypeID, DocumentName, DocumentNote, dto);

			if (result > 0)
			{
				return Ok(new { statusCode = 200, message = "Document Type updated successfully.", Data = result });
			}
			else
			{
				return StatusCode(500, new { statusCode = 500, message = "No data is updated." });
			}
		}

		[HttpGet("SearchDocuments")]
		public async Task<IActionResult> SearchDocuments(string sValue)
		{
			var dropdownData = await _CabinetInterface.SearchDocumentsAsync(sValue);

			if (dropdownData != null && dropdownData.Any())  // Check if the collection exists and has items
			{
				return Ok(new
				{
					statusCode = 200,
					message = "Search loaded successfully.",
					data = dropdownData  // Return the actual data
				});
			}
			else
			{
				return NotFound(new
				{
					statusCode = 404,
					message = "No data found for the given criteria."
				});
			}
		}


		[HttpGet("LoadRententionData")]
		public async Task<IActionResult> LoadRententionData(int compID)
		{
			var dropdownData = await _CabinetInterface.LoadRententionDataAsync(compID);

			if (dropdownData != null && dropdownData.Any())  // Check if the collection exists and has items
			{
				return Ok(new
				{
					statusCode = 200,
					message = "Cabinet loaded successfully.",
					data = dropdownData  // Return the actual data
				});
			}
			else
			{
				return NotFound(new
				{
					statusCode = 404,
					message = "No data found for the given criteria."
				});
			}
		}
	}
}
