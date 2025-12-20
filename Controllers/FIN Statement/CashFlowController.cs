using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.CashFlowDto;
using TracePca.Service.FIN_statement;
using Microsoft.Data.SqlClient;
using TracePca.Dto.FIN_Statement;
using DocumentFormat.OpenXml.Bibliography;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashFlowController : ControllerBase
    {
        private CashFlowInterface _CashFlowInterface;
        private CashFlowInterface _CashFlowService;
        private readonly IWebHostEnvironment _env;

        public CashFlowController(CashFlowInterface CashFlowInterface, IWebHostEnvironment env)
        {
            _CashFlowInterface = CashFlowInterface;
            _CashFlowService = CashFlowInterface;
            _env = env;
        }

        //DeleteCashFlowCategoryWise
        [HttpDelete("DeleteCashFlowCategoryWise")]
        public async Task<IActionResult> DeleteCashflowCategory1([FromBody] DeleteCashflowCategoryWiseDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "Invalid request data.",
                    data = (object)null
                });
            }
            try
            {
                await _CashFlowService.DeleteCashflowCategory1Async(dto.CompId, dto.PkId, dto.CustId, dto.Category);

                return Ok(new
                {
                    statusCode = 200,
                    message = "Cashflow row deleted successfully.",
                    data = (object)null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching Acc_CashFlow records.",
                    Error = ex.Message
                });
            }
        }

        //GetCashFlowID(SearchButton)
        [HttpGet("GetCashFlowID(SearchButton)")]
        public async Task<IActionResult> GetCashFlowParticularsId(int compId, string description, int custId, int branchId)
        {
            try
            {
                var result = await _CashFlowService.GetCashFlowParticularsIdAsync(compId, description, custId, branchId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = $"No records found.",
                        Data = (object)null
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = $"Records fetched successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching records.",
                    Error = ex.Message
                });
            }
        }

        //GetCashFlowForAllCategory
        [HttpGet("GetCashFlowForAllCategory")]
        public async Task<IActionResult> GetCashFlowForAllCategory(int compId, int custId, int yearId, int branchId, int category)
        {
            try
            {
                var result = await _CashFlowService.GetCashFlowForAllCategoryAsync(compId, custId, yearId, branchId, category);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = $"No records found in Acc_CashFlow for category '{category}' and companyId '{compId}'.",
                        Data = (object)null
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = $"Records fetched successfully for category '{category}'.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching Acc_CashFlow records.",
                    Error = ex.Message
                });
            }
        }

        //SaveCashFlow(Category 1)
        [HttpPost("GetCashFlowCategory1")]
        public async Task<IActionResult> GetCashFlowCategory1(
              [FromQuery] int compId,
              [FromQuery] int custId,
              [FromQuery] int yearId,
              [FromQuery] int branchId,
              [FromBody] List<UserAdjustmentInput>? userAdjustments)
        {
            try
            {
                var result = await _CashFlowService.LoadCashFlowCategory1Async(custId, yearId, branchId, userAdjustments);
                if (custId == 0 || yearId == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Invalid request. Please provide valid custId and yearId.",
                        Data = (object)null
                    });
                }
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Cash flow (Category 1) fetched successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching Category 1 cash flow records.",
                    Error = ex.Message
                });
            }
        }
    }
}
