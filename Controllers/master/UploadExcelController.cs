using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FIN_Statement;
using TracePca.Interface.Master;
using TracePca.Service.FIN_statement;
using TracePca.Service.Master;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.master
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadExcelController : ControllerBase
    {
        private UploadExcelInterface _UploadExcelInterface;
        private UploadExcelInterface _UploadExcelService;

        public UploadExcelController(UploadExcelInterface UploadExcelInterface)
        {
            _UploadExcelInterface = UploadExcelInterface;
            _UploadExcelService = UploadExcelInterface;
        }

        //ValidateClientDetails
        [HttpPost("ValidateCustomerExcel")]
        public async Task<IActionResult> ValidateCustomerExcelAsync([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "No file uploaded or file is empty.",
                        data = (object)null
                    });
                }

                var result = await _UploadExcelService.ValidateCustomerExcelAsync(file);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No records found in the uploaded Excel.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Excel validated successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while validating the Excel.",
                    error = ex.Message
                });
            }
        }
    }
}
