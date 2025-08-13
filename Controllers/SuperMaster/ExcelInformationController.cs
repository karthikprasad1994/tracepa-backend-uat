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

        //ValidateEmployeeMasters
        [HttpPost("ValidateEmployeeMasters")]
        public async Task<IActionResult> ValidateEmployees([FromQuery] int CompId, [FromBody] List<SuperMasterValidateEmployeeDto> employees)
        {
            //if (file == null || file.Length == 0)
            //{
            //    return BadRequest(new
            //    {
            //        StatusCode = 400,
            //        Message = "No Excel file provided."
            //    });
            //}
            try
            {
                var result = await _ExcelInformationService.ValidateExcelDataAsync(CompId, employees);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Excel file processed successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while validating the Excel file.",
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

        //ValidateClientDetails
        [HttpPost("ValidateClientDetails")]
        public async Task<IActionResult> ValidateClients([FromQuery] int CompId, [FromBody] List<SuperMasterValidateClientDetailsDto> employees)
        {
            try
            {
                var result = await _ExcelInformationService.ValidateClientDetailsAsync(CompId, employees);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Client data processed successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while validating the client data.",
                    Error = ex.Message
                });
            }
        }

        //SaveClientDetails
        [HttpPost("SaveClientDetails")]
        public async Task<IActionResult> SaveCustomerDetails([FromQuery] int compId, [FromBody] List<SuperMasterSaveCustomerDto> customers)
        {
            if (customers == null || !customers.Any())
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
        public async Task<IActionResult> SuperMasterSaveClientUser(int compId, [FromBody] List<SaveClientUserDto> employees)

        {
            if (employees == null || employees.Count == 0)
            {
                return BadRequest(new { message = "No employee data provided." });
            }

            try
            {
                var results = await _ExcelInformationService.SuperMasterSaveClientUserAsync(compId, employees);

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
