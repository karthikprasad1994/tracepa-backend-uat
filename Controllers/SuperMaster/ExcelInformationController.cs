using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.Audit;
using TracePca.Dto.SuperMaster;
using TracePca.Interface.SuperMaster;
using TracePca.Service.SuperMaster;
using static TracePca.Dto.SuperMaster.ExcelInformationDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.SuperMaster
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelInformationController : ControllerBase
    {
        //private ExcelInformationInterface _ExcelInformationInterface;
        private readonly ExcelInformationInterfaces _ExcelInformationService;

        public ExcelInformationController(ExcelInformationInterfaces ExcelInformationInterfaces)
        {
            _ExcelInformationService = ExcelInformationInterfaces;
            //_ExcelInformationService = ExcelInformationInterface;
        }

        //UploadEmployeeMasters
        [HttpPost("UploadEmployeeMasters")]
        public async Task<IActionResult> UploadEmployeeMaster([FromQuery] int compId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "No file uploaded."
                });

            try
            {
                var results = await _ExcelInformationService.UploadEmployeeDetailsAsync(compId, file);

                // If no errors and employees inserted successfully
                return Ok(new
                {
                    statusCode = 200,
                    message = "Employee master processed successfully"
                });
            }
            catch (Exception ex)
            {
                List<string> errors;

                if (ex.Message.Contains("||"))
                    errors = ex.Message.Split("||").ToList();
                else
                    errors = new List<string> { ex.Message };

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    statusCode = 500,
                    message = "Error processing employee master",
                    error = errors
                });
            }
        }


        //SaveEmployeeMaster
        [HttpPost("SaveEmployeeMaster")]
        public async Task<IActionResult> SaveMultipleEmployees(int compId, [FromBody] List<SuperMasterSaveEmployeeMasterDto> employees)
        {
            if (employees == null || employees.Count == 0)
            {
                return BadRequest(new { message = "No employee data provided." });
            }

            try
            {
                var results = await _ExcelInformationService.SuperMasterSaveEmployeeDetailsAsync(compId, employees);

                return Ok(new
                {
                    message = "Employees processed successfully.",
                    results // This will contain a List<int[]> from service
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while saving employees.",
                    error = ex.Message
                });
            }
        }

     //   //UploadClientDetails
     //   [HttpPost("UploadClientDetails")]
     //   public async Task<IActionResult> UploadClientDetails(
     //[FromForm] int compId,
     //[FromForm] IFormFile excelFile,
     //[FromForm] string sheetName)
     //   {
     //       if (excelFile == null || excelFile.Length == 0)
     //       {
     //           return BadRequest(new { message = "No Excel file uploaded." });
     //       }

     //       try
     //       {
     //           var results = await _ExcelInformationService.UploadClientDetailsAsync(compId, excelFile, sheetName);

     //           return Ok(new
     //           {
     //               message = "Client details uploaded successfully.",
     //               results
     //           });
     //       }
     //       catch (Exception ex)
     //       {
     //           return StatusCode(500, new
     //           {
     //               message = "An error occurred while uploading client details.",
     //               error = ex.Message
     //           });
     //       }
     //   }


        //SaveClientDetails
        [HttpPost("SaveClientDetails")]
        public async Task<IActionResult> SaveCustomerDetails([FromQuery] int compId, [FromBody] List<SuperMasterSaveCustomerDto> customers)
        {
            if (customers == null || customers.Count == 0)
                return BadRequest("No customer data provided.");

            try
            {
                var result = await _ExcelInformationService.SuperMasterSaveCustomerDetailsAsync(compId, customers);
                return Ok(new
                {
                    Message = "Customer(s) saved successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while saving customer(s)",
                    Error = ex.Message
                });
            }
        }

        //UploadClientUser
        [HttpPost("UploadClientUser")]
        public async Task<IActionResult> UploadClientUser([FromQuery] int compId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "No file uploaded."
                });

            try
            {
                var results = await _ExcelInformationService.UploadClientUserAsync(compId, file);

                // If processed successfully
                return Ok(new
                {
                    statusCode = 200,
                    message = "Client user processed successfully"
                });
            }
            catch (Exception ex)
            {
                List<string> errors;

                if (ex.Message.Contains("||"))
                    errors = ex.Message.Split("||").ToList();
                else
                    errors = new List<string> { ex.Message };

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    statusCode = 500,
                    message = "Error processing client user",
                    error = errors
                });
            }
        }


        //SaveClientUser
        [HttpPost("SaveClientUser")]
        public async Task<IActionResult> SuperMasterSaveClientUser(int compId, [FromBody] List<SaveClientUserDto> clientUser)

        {
            if (clientUser == null || clientUser.Count == 0)
            {
                return BadRequest(new { message = "No client user data provided." });
            }

            try
            {
                var results = await _ExcelInformationService.SuperMasterSaveClientUserAsync(compId, clientUser);

                return Ok(new
                {
                    message = "Client User processed successfully.",
                    results // This will contain a List<int[]> from service
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while saving client user.",
                    error = ex.Message
                });
            }
        }
    }
}
