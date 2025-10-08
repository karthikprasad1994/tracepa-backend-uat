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
using TracePca.Interface.Permission;

namespace TracePca.Controllers.Permission
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        
        private PermissionInterface _PermissionInterface;
        public PermissionController(PermissionInterface PermissionInterface)
        {
			_PermissionInterface = PermissionInterface;

        }
 
 

		[HttpGet("LoadPermissionDetails")]
		public async Task<IActionResult> LoadPermissionDetails(int CompID)
		{
			var dropdownData = await _PermissionInterface.LoadPermissionDetailsAsync(CompID);

			if (dropdownData != null && dropdownData.Any())  // Check if the collection exists and has items
			{
				return Ok(new
				{
					statusCode = 200,
					message = "Permission loaded successfully.",
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


		[HttpGet("LoadPermissionModule")]
		public async Task<IActionResult> LoadPermissionModule(int CompID)
		{
			var dropdownData = await _PermissionInterface.LoadPermissionModuleAsync(CompID);

			if (dropdownData != null && dropdownData.Any())  // Check if the collection exists and has items
			{
				return Ok(new
				{
					statusCode = 200,
					message = "Permission Module successfully.",
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



		[HttpGet("LoadPermissionRole")]
		public async Task<IActionResult> LoadPermissionRole(string sPermissionType, int CompID)
		{
			var dropdownData = await _PermissionInterface.LoadPermissionRoleAsync(sPermissionType, CompID);

			if (dropdownData != null && dropdownData.Any())  // Check if the collection exists and has items
			{
				return Ok(new
				{
					statusCode = 200,
					message = "Permission Role successfully.",
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
