using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.FlaggedTransactionDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlaggedTransactionController : ControllerBase
    {
        private FlaggedTransactionInterface _FlaggedTransactionInterface;
        private FlaggedTransactionInterface _FlaggedTransactionService;
        private readonly IWebHostEnvironment _env;

        public FlaggedTransactionController(FlaggedTransactionInterface FlaggedTransactionInterface, IWebHostEnvironment env)
        {
            _FlaggedTransactionInterface = FlaggedTransactionInterface;
            _FlaggedTransactionService = FlaggedTransactionInterface;
            _env = env;
        }


        //GetDiferenceAmountStatus
        [HttpGet("GetDiferenceAmountStatus")]
        public async Task<IActionResult> GetDiferenceAmountStatus([FromQuery] int CompId, [FromQuery] int CustId, [FromQuery] int BranchId, [FromQuery] int YearId)
        {
            try
            {
                var result = await _FlaggedTransactionService.GetDiferenceAmountStatusAsync(CompId, CustId, BranchId, YearId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No difference amount status found.",
                        data = Array.Empty<object>()
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Difference amount status loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching difference amount status.",
                    error = ex.Message
                });
            }
        }

        //GetAbnormalEntriesAEStatus
        [HttpGet("GetAbnormalEntriesAEStatus")]
        public async Task<IActionResult> GetAbnormalEntriesSeqReferenceNum([FromQuery] int CompId, [FromQuery] int CustId, [FromQuery] int BranchId, [FromQuery] int YearId)
        {
            try
            {
                var result = await _FlaggedTransactionService.GetAbnormalEntriesSeqReferenceNumAsync(CompId, CustId, BranchId, YearId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No abnormal entries found.",
                        data = Array.Empty<object>()
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Abnormal entries loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching abnormal entries.",
                    error = ex.Message
                });
            }
        }

        //GetSelectedPartiesSeqReferenceNum
        [HttpGet("GetSelectedPartiesSeqReferenceNum")]
        public async Task<IActionResult> GetSelectedPartiesSeqReferenceNum([FromQuery] int CompId, [FromQuery] int CustId, [FromQuery] int BranchId, [FromQuery] int YearId)
        {
            try
            {
                var result = await _FlaggedTransactionService.GetSelectedPartiesSeqReferenceNumAsync(CompId, CustId, BranchId, YearId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No selected parties found.",
                        data = Array.Empty<object>()
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Selected parties loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching selected parties.",
                    error = ex.Message
                });
            }
        }

        //GetSystemSamplingStatus
        [HttpGet("GetSystemSamplingStatus")]
        public async Task<IActionResult> GetSystemSamplingStatus([FromQuery] int CompId, [FromQuery] int CustId, [FromQuery] int BranchId, [FromQuery] int YearId)
        {
            try
            {
                var result = await _FlaggedTransactionService.GetSystemSamplingStatusAsync(CompId, CustId, BranchId, YearId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No System Sampling found.",
                        data = Array.Empty<object>()
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "System Sampling loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching System Sampling.",
                    error = ex.Message
                });
            }
        }

        //GetStatifiedSampingStatus
        [HttpGet("GetStatifiedSampingStatus")]
        public async Task<IActionResult> GetStatifiedSampingStatus([FromQuery] int CompId, [FromQuery] int CustId, [FromQuery] int BranchId, [FromQuery] int YearId)
        {
            try
            {
                var result = await _FlaggedTransactionService.GetStatifiedSampingStatusAsync(CompId, CustId, BranchId, YearId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Statified Samping found.",
                        data = Array.Empty<object>()
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Statified Samping loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching Statified Samping.",
                    error = ex.Message
                });
            }
        }
    }
}
