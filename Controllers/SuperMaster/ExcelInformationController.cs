using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.Audit;
using TracePca.Dto.SuperMaster;
using TracePca.Interface.SuperMaster;
using TracePca.Service.FIN_statement;
using TracePca.Service.SuperMaster;
using static TracePca.Dto.SuperMaster.ExcelInformationDto;
using static TracePca.Service.SuperMaster.ExcelInformationService;

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

                return Ok(new
                {
                    statusCode = 200,
                    message = "Employee master processed successfully"
                });
            }
            catch (EmployeeUploadException ex) // <-- catch your structured exception
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    statusCode = 404,
                    message = ex.Message,
                    data = ex.Errors // <-- structured dictionary
                });
            }
            catch (Exception ex) // fallback for other unexpected errors
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    statusCode = 404,
                    message = "Error processing employee master",
                    data = new List<string> { ex.Message }
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

        //UploadClientDetails
        [HttpPost("UploadClientDetails")]
        public async Task<IActionResult> UploadClientDetails([FromQuery] int compId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "No file uploaded."
                });

            try
            {
                await _ExcelInformationService.UploadClientDetailsAsync(compId, file);

                return Ok(new
                {
                    statusCode = 200,
                    message = "Client details processed successfully"
                });
            }
            catch (ClientDetailsUploadException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    statusCode = 404,
                    message = "Error processing client details",
                    data = ex.Errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    statusCode = 404,
                    message = "Error processing client details",
                    data = new List<string> { ex.Message }
                });
            }
        }

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
            catch (ClientUserUploadException ex) // <-- catch your structured exception
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    statusCode = 404,
                    message = "Error processing client user",  // custom message for client users
                    data = ex.Errors // <-- structured dictionary (Missing column, Missing values, Duplication)
                });
            }
            catch (Exception ex) // fallback for other unexpected errors
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    statusCode = 404,
                    message = "Error processing client user",
                    data = new List<string> { ex.Message }
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

        //DownloadEmployeeMaster
        [HttpGet("DownloadableEmployeeMasterExcelFile")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public IActionResult DownloadEmployeeMasterExcelTemplate()

        {
            var result = _ExcelInformationService.GetEmployeeMasterExcelTemplate();

            if (result.FileBytes == null)
                return NotFound("File not found.");

            return File(result.FileBytes, result.ContentType, result.FileName);
        }

        //DownloadClientDetails
        [HttpGet("DownloadableClientDetailsExcelFile")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public IActionResult DownloadClientDetailsExcelTemplate()

        {
            var result = _ExcelInformationService.GetClientDetailsExcelTemplate();

            if (result.FileBytes == null)
                return NotFound("File not found.");

            return File(result.FileBytes, result.ContentType, result.FileName);
        }

        //DownloadClientuser
        [HttpGet("DownloadableClientUserExcelFile")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public IActionResult DownloadClientUserExcelTemplate()

        {
            var result = _ExcelInformationService.GetClientUserExcelTemplate();

            if (result.FileBytes == null)
                return NotFound("File not found.");

            return File(result.FileBytes, result.ContentType, result.FileName);
        }

        //DownloadFiles
    }
}
