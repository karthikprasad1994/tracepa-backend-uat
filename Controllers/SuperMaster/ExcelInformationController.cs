using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.SuperMaster;
using TracePca.Interface.SuperMaster;
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

        //ValidateClientDetails
        [HttpPost("SuperMasterValidateCustomerExcel")]
        public async Task<IActionResult> SuperMasterValidateClientDetailsExcel([FromForm] SuperMasterValidateClientDetailsResult file)
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
                var result = await _ExcelInformationService.SuperMasterValidateClientDetailsExcelAsync(file);

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
        [HttpPost("SuperMasterSaveEmployee")]
        public async Task<IActionResult> SuperMasterSaveEmployee([FromQuery] int CompId, [FromBody] SuperMasterSaveEmployeeMasterDto objEmp)
        {
            if (objEmp == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "No employee data provided."
                });
            }

            try
            {
                var result = await _ExcelInformationService.SuperMasterSaveEmployeeDetailsAsync(CompId, objEmp);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = result[0] == 1 ? "Employee saved successfully." : "Employee updated successfully.",
                    Data = new
                    {
                        Status = result[0],
                        EmployeeId = result[1]
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving employee data.",
                    Error = ex.Message
                });
            }
        }

        //SaveClientDetails
        [HttpPost("SaveClientDetails")]
        public async Task<IActionResult> SuperMasterSaveCustomer([FromQuery] int CompId, [FromBody] SuperMasterSaveClientDetailsDto objCust)
        {
            if (objCust == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "No customer data provided."
                });
            }
            try
            {
                var result = await _ExcelInformationService.SuperMasterSaveCustomerDetailsAsync(CompId, objCust);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = result[0] == 1 ? "Customer saved successfully." : "Customer updated successfully.",
                    Data = new
                    {
                        Status = result[0],
                        CustomerId = result[1]
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving customer data.",
                    Error = ex.Message
                });
            }
        }

        //SaveClientUser
        [HttpPost("SaveClientUser")]
        public async Task<IActionResult> SaveClientUser([FromQuery] int CompId, [FromBody] SuperMasterSaveClientUserDto objCust)
        {
            if (objCust == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "No customer data provided."
                });
            }

            try
            {
                var result = await _ExcelInformationService.SaveClientUserAsync(CompId, objCust);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = result[0] == 1 ? "Customer detail saved successfully." : "Customer detail updated successfully.",
                    Data = new
                    {
                        Status = result[0],
                        CustomerDetailId = result[1]
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving customer detail.",
                    Error = ex.Message
                });
            }
        }



    }
}
