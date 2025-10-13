using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.CashFlowDto;
using TracePca.Service.FIN_statement;
using Microsoft.Data.SqlClient;
using TracePca.Dto.FIN_Statement;

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

        ////SaveCashFlow(Category 1)
        //[HttpPost("SaveCashFlow(Category 1)")]
        //public async Task<IActionResult> SaveCashFlowCategory1([FromBody] CashFlowCategory1 model)
        //{
        //    try
        //    {
        //        if (model == null)
        //        {
        //            return BadRequest(new
        //            {
        //                StatusCode = 400,
        //                Message = "Invalid input. Please provide valid cash flow details.",
        //                Data = new { }
        //            });
        //        }

        //        var (updateOrSave, oper) = await _CashFlowService.SaveCashFlowCategory1Async(model.ACF_Compid, model);

        //        return Ok(new
        //        {
        //            StatusCode = 200,
        //            Message = "Cash flow saved successfully.",
        //            Data = new
        //            {
        //                UpdateOrSave = updateOrSave,
        //                Operation = oper
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            StatusCode = 500,
        //            Message = "An unexpected error occurred while saving cash flow details.",
        //            Error = ex.Message
        //        });
        //    }
        //}

        ////SaveCashFlow(Category 2)
        //[HttpPost("SaveCashFlow(Category 2)")]
        //public async Task<IActionResult> SaveCashFlowCategory2([FromBody] CashFlowCategory2 dto)
        //{
        //    try
        //    {
        //        if (dto == null)
        //        {
        //            return BadRequest(new
        //            {
        //                StatusCode = 400,
        //                Message = "Invalid input. Please provide valid cash flow data.",
        //                Data = new { }
        //            });
        //        }

        //        var id = await _CashFlowService.SaveCashFlowCategory2Async(dto);

        //        string message = dto.ACF_pkid == 0
        //        ? "Cash flow record inserted successfully."
        //        : "Cash flow record updated successfully.";

        //        return Ok(new
        //        {
        //            StatusCode = 200,
        //            Message = message,
        //            Data = new { ACF_pkid = id }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            StatusCode = 500,
        //            Message = "An error occurred while saving cash flow data.",
        //            Error = ex.Message
        //        });
        //    }
        //}

        ////SaveCashFlow(Category 3)
        //[HttpPost("SaveCashFlow(Category 3)")]
        //public async Task<IActionResult> SaveCashFlowCategory3([FromBody] CashFlowCategory3 model)
        //{
        //    try
        //    {
        //        if (model == null)
        //        {
        //            return BadRequest(new
        //            {
        //                StatusCode = 400,
        //                Message = "Invalid input. Please provide valid cash flow details.",
        //                Data = new { }
        //            });
        //        }

        //        var (updateOrSave, oper) = await _CashFlowService.SaveCashFlowCategory3Async(model.ACF_Compid, model);

        //        return Ok(new
        //        {
        //            StatusCode = 200,
        //            Message = "Cash flow saved successfully.",
        //            Data = new
        //            {
        //                UpdateOrSave = updateOrSave,
        //                Operation = oper
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            StatusCode = 500,
        //            Message = "An unexpected error occurred while saving cash flow details.",
        //            Error = ex.Message
        //        });
        //    }
        //}

        ////SaveCashFlow(Category 4)
        //[HttpPost("SaveCashFlow(Category 4)")]
        //public async Task<IActionResult> SaveCashFlowCategory4([FromBody] CashFlowCategory4 model)
        //{
        //    try
        //    {
        //        if (model == null)
        //        {
        //            return BadRequest(new
        //            {
        //                StatusCode = 400,
        //                Message = "Invalid input. Please provide valid cash flow details.",
        //                Data = new { }
        //            });
        //        }

        //        var (updateOrSave, oper) = await _CashFlowService.SaveCashFlowCategory4Async(model.ACF_Compid, model);

        //        return Ok(new
        //        {
        //            StatusCode = 200,
        //            Message = "Cash flow saved successfully.",
        //            Data = new
        //            {
        //                UpdateOrSave = updateOrSave,
        //                Operation = oper
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            StatusCode = 500,
        //            Message = "An unexpected error occurred while saving cash flow details.",
        //            Error = ex.Message
        //        });
        //    }
        //}

        ////SaveCashFlow(Category 5)
        //[HttpPost("SaveCashFlow(Category 5)")]
        //public async Task<IActionResult> SaveCashFlowCategory5([FromBody] CashFlowCategory5 dto)
        //{
        //    try
        //    {
        //        if (dto == null)
        //        {
        //            return BadRequest(new
        //            {
        //                StatusCode = 400,
        //                Message = "Invalid input. Please provide valid cash flow data.",
        //                Data = new { }
        //            });
        //        }

        //        var id = await _CashFlowService.SaveCashFlowCategory5Async(dto);

        //        string message = dto.ACF_pkid == 0
        //        ? "Cash flow record inserted successfully."
        //        : "Cash flow record updated successfully.";

        //        return Ok(new
        //        {
        //            StatusCode = 200,
        //            Message = message,
        //            Data = new { ACF_pkid = id }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            StatusCode = 500,
        //            Message = "An error occurred while saving cash flow data.",
        //            Error = ex.Message
        //        });
        //    }
        //}

        //SaveCashFlow(Category 1)
        [HttpPost("SaveCashFlow(Category 1)")]
        public async Task<IActionResult> SaveCashFlowCategory1([FromBody] List<CashFlowCategory1> dtos)
        {
            try
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Invalid input. Please provide valid cash flow details.",
                        Data = new { }
                    });
                }

                // Call the service to save/update/delete all items
                var processedIds = await _CashFlowService.SaveCashFlowCategory1Async(dtos);

                // Generate message for each row dynamically
                var messages = dtos.Select(dto =>
                {
                    if (dto.IsDeleted)
                        return $"Cash flow record '{dto.ACF_Description}' deleted successfully.";
                    else if (dto.ACF_pkid == 0)
                        return $"Cash flow record '{dto.ACF_Description}' inserted successfully.";
                    else
                        return $"Cash flow record '{dto.ACF_Description}' updated successfully.";
                }).ToList();

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Cash flow records processed successfully.",
                    Details = messages,
                    Data = new
                    {
                        ProcessedCount = processedIds.Count,
                        ACF_pkids = processedIds
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = ex.Message,
                    Data = new { }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An unexpected error occurred while processing cash flow records.",
                    Error = ex.Message
                });
            }
        }
    }
}
