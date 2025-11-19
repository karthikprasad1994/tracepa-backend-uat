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
        [HttpPost("SaveCashFlow(Category1)")]
        public async Task<IActionResult> SaveCashFlowCategory1([FromBody] List<CashFlowCategory1> dtos)
        {
            try
            {
                // ✅ Basic validation
                if (dtos == null || dtos.Count == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Please provide valid cash flow data.",
                        Data = (object)null
                    });
                }

                var processedIds = await _CashFlowService.SaveCashFlowCategory1Async(dtos);

                string message = dtos.Any(d => d.ACF_pkid == 0)
                    ? "Cash flow records saved successfully."
                    : "Cash flow records updated successfully.";

                return Ok(new
                {
                    StatusCode = 200,
                    Message = message,
                    Data = new
                    {
                        ProcessedCount = processedIds.Count,
                        ACF_pkids = processedIds
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving cash flow records.",
                    Error = ex.Message
                });
            }
        }

        //SaveCashFlow(Category3)
        [HttpPost("SaveCashFlow(Category3)")]
        public async Task<IActionResult> SaveCashFlowCategory3([FromBody] List<CashFlowCategory3> dtos)
        {
            try
            {
                // ✅ Basic validation
                if (dtos == null || dtos.Count == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Please provide valid cash flow data.",
                        Data = (object)null
                    });
                }

                var processedIds = await _CashFlowService.SaveCashFlowCategory3Async(dtos);

                string message = dtos.Any(d => d.ACF_pkid == 0)
                    ? "Cash flow records saved successfully."
                    : "Cash flow records updated successfully.";

                return Ok(new
                {
                    StatusCode = 200,
                    Message = message,
                    Data = new
                    {
                        ProcessedCount = processedIds.Count,
                        ACF_pkids = processedIds
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving cash flow records.",
                    Error = ex.Message
                });
            }
        }

        //SaveCashFlow(Category4)
        [HttpPost("SaveCashFlow(Category4)")]
        public async Task<IActionResult> SaveCashFlowCategory4([FromBody] List<CashFlowCategory4> dtos)
        {
            try
            {
                // ✅ Basic validation
                if (dtos == null || dtos.Count == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Please provide valid cash flow data.",
                        Data = (object)null
                    });
                }

                var processedIds = await _CashFlowService.SaveCashFlowCategory4Async(dtos);

                string message = dtos.Any(d => d.ACF_pkid == 0)
                    ? "Cash flow records saved successfully."
                    : "Cash flow records updated successfully.";

                return Ok(new
                {
                    StatusCode = 200,
                    Message = message,
                    Data = new
                    {
                        ProcessedCount = processedIds.Count,
                        ACF_pkids = processedIds
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving cash flow records.",
                    Error = ex.Message
                });
            }
        }

        //SaveCashFlow(Category2)
        [HttpPost("SaveCashFlow(Category2)")]
        public async Task<IActionResult> SaveCashFlowCategory2([FromBody] List<CashFlowCategory2> dtos)
        {
            try
            {
                // ✅ Basic validation
                if (dtos == null || dtos.Count == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Please provide valid cash flow data.",
                        Data = (object)null
                    });
                }

                var processedIds = await _CashFlowService.SaveCashFlowCategory2Async(dtos);

                string message = dtos.Any(d => d.ACF_pkid == 0)
                    ? "Cash flow records saved successfully."
                    : "Cash flow records updated successfully.";

                return Ok(new
                {
                    StatusCode = 200,
                    Message = message,
                    Data = new
                    {
                        ProcessedCount = processedIds.Count,
                        ACF_pkids = processedIds
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving cash flow records.",
                    Error = ex.Message
                });
            }
        }

        //SaveCashFlow(Category5)
        [HttpPost("SaveCashFlow(Category5)")]
        public async Task<IActionResult> SaveCashFlowCategory5([FromBody] List<CashFlowCategory5> dtos)
        {
            try
            {
                // ✅ Basic validation
                if (dtos == null || dtos.Count == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Please provide valid cash flow data.",
                        Data = (object)null
                    });
                }

                var processedIds = await _CashFlowService.SaveCashFlowCategory5Async(dtos);

                string message = dtos.Any(d => d.ACF_pkid == 0)
                    ? "Cash flow records saved successfully."
                    : "Cash flow records updated successfully.";

                return Ok(new
                {
                    StatusCode = 200,
                    Message = message,
                    Data = new
                    {
                        ProcessedCount = processedIds.Count,
                        ACF_pkids = processedIds
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving cash flow records.",
                    Error = ex.Message
                });
            }
        }
    }
}
