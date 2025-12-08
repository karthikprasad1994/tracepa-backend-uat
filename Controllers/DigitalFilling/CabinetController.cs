using Dapper;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.Style.XmlAccess;
using OpenAI.ObjectModels.ResponseModels;
using System.ServiceModel.Channels;
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

		//      [HttpGet("LoadCabinet")]
		//      public async Task<IActionResult> LoadCabinet(int deptId, int userId, int compID)
		//      {
		//          var dropdownData = await _CabinetInterface.LoadCabinetAsync(deptId, userId, compID);

		//	if (dropdownData != null && dropdownData.Any())  // Check if the collection exists and has items
		//	{
		//		return Ok(new
		//		{
		//			statusCode = 200,
		//			message = "Cabinet loaded successfully.",
		//			data = dropdownData  // Return the actual data
		//		});
		//	}
		//	else
		//	{
		//		return NotFound(new
		//		{
		//			statusCode = 404,
		//			message = "No data found for the given criteria."
		//		});
		//	}
		//}


		[HttpGet("LoadCabinet")]
		public async Task<IActionResult> LoadCabinet(int compID)
		{
			try
			{
                var dropdownData = await _CabinetInterface.LoadCabinetAsync(compID);

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
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An unexpected error occurred while loading cabinet data.",
                    details = ex.Message   
                });
            } 
		}

		[HttpGet("LoadDescriptor")]
		public async Task<IActionResult> LoadDescriptor(int iDescId, int compID)
		{
			try
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
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An unexpected error occurred while loading cabinet data.",
                    details = ex.Message
                });
            }
		}

		[HttpPost("LoadDocumentType")]
		public async Task<IActionResult> LoadDocumentType(int iDocTypeID, int iDepartmentID, [FromBody] DocumentTypeDto dto)
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


		[HttpGet("LoadAllDocumentType")]
		public async Task<IActionResult> LoadAllDocumentType(int iCompID)
		{
			var dropdownData = await _CabinetInterface.LoadAllDocumentTypeAsync(iCompID);

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


		[HttpGet("LoadArchiveDetails")]
		public async Task<IActionResult> LoadArchiveDetails(int compID)
		{
			var dropdownData = await _CabinetInterface.LoadArchiveDetailsAsync(compID);

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


		[HttpGet("ArchivedDocumentFileDetails")]
		public async Task<IActionResult> ArchivedDocumentFileDetails(string sAttachID)
		{
			var dropdownData = await _CabinetInterface.ArchivedDocumentFileDetailsAsync(sAttachID);

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


		[HttpGet("LoadAllDepartment")]
		public async Task<IActionResult> LoadAllDepartment(int compID)
		{
			var dropdownData = await _CabinetInterface.LoadAllDepartmentAsync(compID);

			if (dropdownData != null && dropdownData.Any())  // Check if the collection exists and has items
			{
				return Ok(new
				{
					statusCode = 200,
					message = "Department loaded successfully.",
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
        public async Task<IActionResult> CreateCabinet(string CabinetName, int deptId, int userId, int compID)
        {
            var result = await _CabinetInterface.CreateCabinetAsync(CabinetName,deptId, userId, compID);

            if (result > 0)
            {
                return Ok(new { statusCode = 200, message = "Cabinet created successfully.", CabinetID = result });
            }
            else
            {
                return StatusCode(500, new { statusCode = 500, message = "No Cabient data is saved." });
            }
        }


		[HttpPost("CreateSubCabinet")]
		public async Task<IActionResult> CreateSubCabinet(string SubCabinetName, int iCabinetID,  int compID)
		{
			var result = await _CabinetInterface.CreateSubCabinetAsync(SubCabinetName, iCabinetID,  compID);
			if (result == "Subcabinet created Successfully.")
			{
				return Ok(new
				{
					statusCode = 200,
					message = result
				});
			}
			else
			{
				return StatusCode(400, new { statusCode = 400, message = result });
			}
		}



		[HttpPost("CreateFolder")]
		public async Task<IActionResult> CreateFolder(string FolderName, int iCabinetID, int iSubCabinetID, int compID)
		{
			var result = await _CabinetInterface.CreateFolderAsync(FolderName, iCabinetID, iSubCabinetID, compID);
			if (result == "Folder created Successfully.")
			{
				return Ok(new
				{
					statusCode = 200,
					message = result
				});
			}
			else
			{
				return StatusCode(404, new { statusCode = 404, message = result });
			}
		}


		[HttpPost("UpdateCabinet")]
        public async Task<IActionResult> UpdateCabinet(string CabinetName, int iCabinetId, int iUserID, int compID)
        {
            var result = await _CabinetInterface.UpdateCabinetAsync(CabinetName, iCabinetId, iUserID, compID);

            if (result > 0)
            {
                return Ok(new { statusCode = 200, message = "Cabinet updated successfully.", Data = result });
            }
            else
            {
                return StatusCode(500, new { statusCode = 500, message = "No Cabient data is updated." });
            }
        }




		[HttpPost("UpdateSubCabinet")]
		public async Task<IActionResult> UpdateSubCabinet(string SubCabinetName, int iSubCabinetId, int iUserID, int compID)
		{
			var result = await _CabinetInterface.UpdateSubCabinetAsync(SubCabinetName, iSubCabinetId, iUserID, compID);

			if (result > 0)
			{
				return Ok(new { statusCode = 200, message = "Sub Cabinet updated successfully.", Data = result });
			}
			else
			{
				return StatusCode(500, new { statusCode = 500, message = "No Sub Cabient data is updated." });
			}
		}


		[HttpPost("UpdateFolder")]
		public async Task<IActionResult> UpdateFolder(string FolderName, int iFolderID, int iUserID, int compID)
		{
			var result = await _CabinetInterface.UpdateFolderAsync(FolderName, iFolderID, iUserID, compID);

			if (result > 0)
			{
				return Ok(new { statusCode = 200, message = "Folder updated successfully.", Data = result });
			}
			else
			{
				return StatusCode(500, new { statusCode = 500, message = "No Folder data is updated." });
			}
		}



		[HttpPost("UpdateArchiveDetails")]
		public async Task<IActionResult> UpdateArchiveDetails(string retentionDate, int retentionPeriod, int archiveId, int compId)
		{
			try
			{
				var result = await _CabinetInterface.UpdateArchiveDetailsAsync(retentionDate, retentionPeriod, archiveId, compId);

				if (result == "Invalid Archive Id.")
					return NotFound(new { message = result });

				if (result == "Updated Successfully.")
					return Ok(new { message = result });

				return BadRequest(new { message = "Unexpected result." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
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
					//return StatusCode(500, result); // Internal Server Error
					return NotFound(new
					{
						statusCode = 500,
						message = result
					});
				}
                else
                {
                    //return Ok(result); // Success

					if(result == "Indexed Successfully.")
					{
						return Ok(new
						{
							statusCode = 200,
							message = "Successfully Indexed."
							//result
						});
					}
					else
					{
						return NotFound(new
						{
							statusCode = 400,
							message = result
							//result
						});
					}
				}
                
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                return StatusCode(500, $"An error occurred: {ex.Message}");
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

		[HttpPost("UpdateDescriptor")]
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
		 

		[HttpPost("CreateDocumentType")]
		public async Task<IActionResult> CreateDocumentType(string DocumentName, string DepartmentId, int userID, int CompID)
		{
			var result = await _CabinetInterface.CreateDocumentTypeAsync(DocumentName, DepartmentId, userID, CompID);

			if (result > 0)
			{
				return Ok(new { statusCode = 200, message = "Document Type created successfully.", Data = result });
			}
			else
			{
				return StatusCode(500, new { statusCode = 500, message = "No Document Type data is saved." });
			}
		}


		[HttpPost("UpdateDocumentType")]
		public async Task<IActionResult> UpdateDocumentType(int iDocTypeID, string DocumentName, int userID, int CompID)
		{
			var result = await _CabinetInterface.UpdateDocumentTypeAsync(iDocTypeID, DocumentName, userID, CompID);

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

		 
		[HttpPost("CreateDepartment")]
		public async Task<IActionResult> CreateDepartment(string Code, string DepartmentName, string userId, int compID)
		{
			var result = await _CabinetInterface.CreateDepartmentAsync(Code, DepartmentName, userId, compID);

			if (result == "Department Created Successfully.")
			{
				return Ok(new
				{
					statusCode = 200,
					message = result
				});
			}
			else
			{
				return StatusCode(500, new { statusCode = 400, message = result });
			}
 
		}


		[HttpGet("DownloadArchieveDocuments")]
		public async Task<IActionResult> DownloadArchieveDocuments(string sAttachID)
		{
			var zipPath = await _CabinetInterface.DownloadArchieveDocumentsAsync(sAttachID);

			if (System.IO.File.Exists(zipPath.ToString()))
			{
				var fileBytes = await System.IO.File.ReadAllBytesAsync(zipPath.ToString());
				var fileName = Path.GetFileName(zipPath.ToString());

				return File(fileBytes, "application/zip", fileName);
			}

			return NotFound(new
			{
				statusCode = 404,
				message = "No documents found to download."
			});
		}



		[HttpPost("DeleteArchiveDocuments")]
		public async Task<IActionResult> DeleteArchiveDocuments(int archiveId, string AttachID, int compId)
		{
			try
			{
				var result = await _CabinetInterface.DeleteArchiveDocumentsAsync(archiveId, AttachID, compId);

				if (result == "Invalid Archive Id.")
					return NotFound(new { message = result });

				if (result == "Deleted Successfully.")
					return Ok(new { message = result });

				return BadRequest(new { message = "Unexpected result." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
			}
		}


		[HttpPost("UpdateDepartment")]
		public async Task<IActionResult> UpdateDepartment(string Code, string DepartmentName, int iDepartmentID, int iUserID, int compID)
		{
			var result = await _CabinetInterface.UpdateDepartmentAsync(Code, DepartmentName, iDepartmentID, iUserID, compID);

			if (result > 0)
			{
				return Ok(new { statusCode = 200, message = "Department updated successfully.", Data = result });
			}
			else
			{
				return StatusCode(500, new { statusCode = 500, message = "No Department data is updated." });
			}
		}

        [HttpGet("getfilebyid")]
        public async Task<IActionResult> GetFileById([FromQuery] string userEmail, [FromQuery] int DocId)
        {
            if (DocId <= 0)
                return BadRequest(new { Status = "Error", Message = "Document Not Exist!" });

            try
            {
                var file = await _CabinetInterface.GetFileByIdAsync(DocId, userEmail);
                return Ok(new
                {
                    Status = "Success",
                    Message = "File retrieved successfully.",
                    Data = file
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }
    }
}
