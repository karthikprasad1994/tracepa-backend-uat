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
        //[HttpGet("GetMandatoryCashflow")]
        //public async Task<IActionResult> GetMandatoryCashflow(int yearId = 0, int customerId = 0, int branchId = 0)
        //{
        //    try
        //    {
        //        var (hasCashflow, particulars) = await _CashFlowService.GetMandatoryCashflowInMemoryAsync(yearId, customerId, branchId);

        //        if (!hasCashflow)
        //        {
        //            return Ok(new
        //            {
        //                StatusCode = 200,
        //                Message = "No cashflow data found for the specified customer/year.",
        //                Data = new
        //                {
        //                    HasCashflow = false,
        //                    Particulars = Array.Empty<object>()
        //                }
        //            });
        //        }

        //        // Map/return DTOs directly
        //        var responseList = particulars.Select(p => new
        //        {
        //            p.Description,
        //            p.IsHeading,
        //            Amount = p.Amount,
        //            p.SourceHint,
        //            p.Note
        //        }).ToList();

        //        return Ok(new
        //        {
        //            StatusCode = 200,
        //            Message = "Mandatory cashflow particulars fetched successfully.",
        //            Data = new
        //            {
        //                HasCashflow = true,
        //                Particulars = responseList
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Keep consistent with your existing error response shape
        //        return StatusCode(500, new
        //        {
        //            StatusCode = 500,
        //            Message = "An error occurred while fetching mandatory cashflow particulars.",
        //            Error = ex.Message
        //        });
        //    }
        //}
        //[HttpGet("GetCashFlowCategory1")]
        //public async Task<IActionResult> GetCashFlowCategory1(int customerId, int yearId, int branchId)
        //{
        //    try
        //    {
        //        var result = await _CashFlowService.LoadCashFlowCategory1Async(customerId, yearId, branchId);

        //        if (result == null)
        //        {
        //            return NotFound(new
        //            {
        //                StatusCode = 404,
        //                Message = $"No cash flow records found for Customer '{customerId}', Year '{yearId}', Branch '{branchId}'.",
        //                Data = (object)null
        //            });
        //        }

        //        return Ok(new
        //        {
        //            StatusCode = 200,
        //            Message = "Cash flow (Category 1) fetched successfully.",
        //            Data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            StatusCode = 500,
        //            Message = "An error occurred while fetching Category 1 cash flow records.",
        //            Error = ex.Message
        //        });
        //    }
        //}
        //[HttpGet("GetCashFlowCategory1")]
        //public async Task<IActionResult> GetCashFlowCategory1(
        //    [FromQuery] int customerId,
        //    [FromQuery] int yearId,
        //    [FromQuery] int branchId,
        //    [FromBody] List<UserAdjustmentInput>? userAdjustments = null)
        //{
        //    try
        //    {
        //        var result = await _CashFlowService.LoadCashFlowCategory1Async(customerId, yearId, branchId, userAdjustments);

        //        if (result == null || result.Particular == null || !result.Particular.Any())
        //        {
        //            return NotFound(new
        //            {
        //                StatusCode = 404,
        //                Message = $"No cash flow records found for Customer '{customerId}', Year '{yearId}', Branch '{branchId}'.",
        //                Data = (object)null
        //            });
        //        }
        //        return Ok(new
        //        {
        //            StatusCode = 200,
        //            Message = "Cash flow (Category 1) fetched successfully.",
        //            Data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            StatusCode = 500,
        //            Message = "An error occurred while fetching Category 1 cash flow records.",
        //            Error = ex.Message
        //        });
        //    }
        //}
    }
}
