using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TracePca.Dto.AssetRegister;
using TracePca.Dto.Audit;
using TracePca.Dto.DigitalFilling;
using TracePca.Dto.Masters;
using TracePca.Interface.Audit;
using TracePca.Interface.DigitalFilling;
using TracePca.Interface.FixedAssetsInterface;
using TracePca.Interface.Master;

namespace TracePca.Controllers.master
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorLogController : ControllerBase
    {
        
        private ErrorLogInterface _ErrorLogInterface;
        public ErrorLogController(ErrorLogInterface ErrorLogInterface)
        {
			_ErrorLogInterface = ErrorLogInterface;
        }
       
        

        [HttpPost("CreateLogError")]
        public async Task<IActionResult> CreateLogError(string accessCode, string message, string className, string functionName, ErrorLogDto dto)
        {
            var result = await _ErrorLogInterface.LogErrorAsync(accessCode, message, className, functionName, dto);

            if (result != "")
            {
                return Ok(new { statusCode = 200, message = "Error Log created successfully.", Data = result });
            }
            else
            {
                return StatusCode(500, new { statusCode = 500, message = "No Error Log data is saved." });
            }
        }
		 
	}
}
