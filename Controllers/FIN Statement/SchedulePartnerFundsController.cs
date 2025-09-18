using Microsoft.AspNetCore.Mvc;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.SchedulePartnerFundsDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulePartnerFundsController : ControllerBase
    {
        private SchedulePartnerFundsInterface _SchedulePartnerFundsService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public SchedulePartnerFundsController(SchedulePartnerFundsInterface SchedulePartnerFundsInterface, Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _SchedulePartnerFundsService = SchedulePartnerFundsInterface;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _dbcontext = dbcontext;
        }

        //GetAllPartnershipFirms
        [HttpGet("LoadAllPartnershipFirms")]
        public async Task<IActionResult> LoadAllPartnershipFirms([FromQuery] PartnershipFirmRequestDto request)
        {
            try
            {
                var result = await _SchedulePartnerFundsService.LoadAllPartnershipFirmsAsync(request);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "Partnership firms not found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Partnership firms loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while loading partnership firms.",
                    error = ex.Message
                });
            }
        }

        //GetPartnernName
        [HttpGet("GetCustomerPartners")]
        public async Task<IActionResult> GetCustomerPartners(int custId, int compId)
        {
            try
            {
                var result = await _SchedulePartnerFundsService.LoadCustPartnerAsync(custId, compId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No partners found for this customer."
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Partners fetched successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching partners.",
                    Error = ex.Message
                });
            }
        }

        //SavePartnershipFirms
        [HttpPost("SavePartnershipFirm")]
        public async Task<IActionResult> SavePartnershipFirm([FromBody] SavePartnershipFirmDto objPF)
        {
            try
            {
                if (objPF == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Invalid input data."
                    });
                }

                var result = await _SchedulePartnerFundsService.SavePartnershipFirmsAsync(objPF);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Partnership Firm saved successfully.",
                    UpdateOrSave = result[0],
                    Oper = result[1]
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving Partnership Firm.",
                    Error = ex.Message
                });
            }
        }

        //UpdatePartnershipFirms
        [HttpPut("UpdatePartnershipFirm")]
        public async Task<IActionResult> UpdatePartnershipFirm([FromBody] UpdatePartnershipFirmDto dto)
        {
            try

            {
                if (dto == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Invalid input data."
                    });
                }
                var result = await _SchedulePartnerFundsService.SaveOrUpdatePartnershipFirmAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.APF_ID > 0 ? "Successfully Updated Partnership Firm." : "Successfully Saved Partnership Firm.",
                    Data = new { UpdateOrSave = result[0], Oper = result[1] }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while updating Partnership Firm.",
                    Error = ex.Message
                });
            }
        }

        //GetSelectedPartnershipFirms
        [HttpGet("GetSelectedPartnershipFirms")]
        public async Task<IActionResult> LoadSelectedPartnershipFirm(int partnershipFirmId, int compId, int yearId)
        {
            try
            {
                var result = await _SchedulePartnerFundsService.LoadSelectedPartnershipFirmAsync(partnershipFirmId, compId, yearId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No partners found for this partnership firm."
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Partners fetched successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching partners.",
                    Error = ex.Message
                });
            }
        }


        //UpdateAndCalculate
        [HttpPut("UpdateAndCalculate")]
        public async Task<IActionResult> UpdateAndCalculate([FromBody] PartnershipFirmCalculationDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Request body cannot be null."
                    });
                }

                var result = await _SchedulePartnerFundsService.UpdateAndCalculateAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Partnership firm updated and total calculated successfully.",
                    Total = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while updating and calculating.",
                    Error = ex.Message
                });
            }
        }

        //GetCustomerPartnerDetails
        [HttpGet("GetCustomerPartnerDetails")]
        public async Task<IActionResult> GetCustomerPartnerDetails(int compId, int custId, int custPartnerPkId = 0)
        {
            try
            {
                var result = await _SchedulePartnerFundsService.GetCustomerPartnerDetailsAsync(compId, custId, custPartnerPkId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No partner details found."
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Partner details fetched successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching partner details.",
                    Error = ex.Message
                });
            }
        }

        //SaveCustomerStatutoryPartner
        [HttpPost("SaveOrUpdateStatutoryPartner")]
        public async Task<IActionResult> SaveOrUpdateStatutoryPartner([FromBody] SaveCustomerStatutoryPartnerDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Invalid input data."
                    });
                }

                var result = await _SchedulePartnerFundsService.SaveOrUpdateCustomerStatutoryPartnerAsync(dto);

                string actionMessage = dto.SSP_Id == 0
                    ? "Statutory Partner saved successfully."
                    : "Statutory Partner updated successfully.";

                return Ok(new
                {
                    StatusCode = 200,
                    Message = actionMessage,
                    UpdateOrSave = result[0],
                    Oper = result[1]
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving or updating Statutory Partner.",
                    Error = ex.Message
                });
            }
        }
    }
}
