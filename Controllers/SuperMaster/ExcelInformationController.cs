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
                return BadRequest("No file uploaded.");

            try
            {
                var results = await _ExcelInformationService.SaveEmployeeDetailsAsync(compId, file);
                return Ok(new
                {
                    Success = true,
                    Message = "Employee master processed successfully",
                    Details = results
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Success = false,
                    Message = "Error processing employee master",
                    Error = ex.Message
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

        //SaveClientUser
        [HttpPost("SaveClientUser")]
        public async Task<IActionResult> SuperMasterSaveClientUser(int compId, [FromBody] List<SaveClientUserDto> clientUser)

        {
            if (clientUser == null || clientUser.Count == 0)
            {
                return BadRequest(new { message = "No employee data provided." });
            }

            try
            {
                var results = await _ExcelInformationService.SuperMasterSaveClientUserAsync(compId, clientUser);

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
    }
}
