using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TracePca.Interface.FixedAssetsInterface;
using static TracePca.Dto.FixedAssets.DepreciationComputationDto;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FixedAssets
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepreciationComputationController : ControllerBase
    {
        private DepreciationComputationInterface _DepreciationComputationInterface;
        private DepreciationComputationInterface _DepreciationComputationService;

        public DepreciationComputationController(DepreciationComputationInterface DepreciationComputationInterface)
        {
            _DepreciationComputationInterface = DepreciationComputationInterface;
            _DepreciationComputationService = DepreciationComputationInterface;
        }


        //DepreciationBasis


        //MethodofDepreciation


        //SaveDepreciation

        [HttpPost("SaveDepreciationComputation")]
        public async Task<IActionResult> SaveDepreciation(
            [FromBody] SaveDepreciationRequestDto request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            try
            {
                var result = await _DepreciationComputationService.SaveDepreciationAsync(
                    request.DepBasis,
                    request.YearId,
                    request.CompId,
                    request.CustId,
                    request.Method,
                    request.UserId,
                    request.IpAddress,
                    request.NormalList,
                    request.ItActList,
                    request.Audit
                );

                return Ok(new
                {
                    statusCode = 200,
                    message = "Depreciation saved successfully",
                    success = result
                });
            }
            catch (Exception ex)
            {
                // ⚠️ Keep detailed error for debugging (log in real app)
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }

        //DownloadExcel
        [HttpGet("DownloadableAssetDepreciationExcelFile")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public IActionResult DownloadAssetDepreciationExcelTemplate()

        {
            var result = _DepreciationComputationService.GetAssetDepreciationExcelTemplate();

            if (result.FileBytes == null)
                return NotFound("File not found.");

            return File(result.FileBytes, result.ContentType, result.FileName);
        }


        //Go
        //itcorrect
        //[HttpGet("GO(ITAct)")]
        //public async Task<ActionResult> GetITActDepreciation(
        //    int compId, int yearId, int custId, DateTime endDate)
        //{
        //    try
        //    {
        //        var result = await _DepreciationComputationService.LoadDepreciationITActAsync(compId, yearId, custId, endDate);

        //        if (result == null || !result.Any())
        //            return NotFound(new { Status = 404, Message = "No IT Act depreciation records found." });

        //        return Ok(new
        //        {
        //            Status = 200,
        //            Message = "Success",
        //            Data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            Status = 500,
        //            Message = "Internal server error.",
        //            Details = ex.Message
        //        });
        //    }
        //}

        ////--------------------newit correct

        //[HttpPost("calculation")]
        //public async Task<IActionResult> CalculateDepreciation([FromBody] DepreciationRequesttDto request)
        //{
        //    try
        //    {
        //        if (request == null)
        //            return BadRequest("Invalid request");

        //        var result = await _DepreciationComputationService.CalculateDepreciationAsync(request);

        //        if ((result.ITActData?.Count ?? 0) == 0 &&
        //            (result.CompanyActData?.Count ?? 0) == 0)
        //        {
        //            return NotFound(new
        //            {
        //                StatusCode = 404,
        //                Message = "No depreciation data found"
        //            });
        //        }

        //        return Ok(new
        //        {
        //            StatusCode = 200,
        //            Message = "Depreciation calculated successfully",
        //            Data = result
        //        });
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(new { StatusCode = 400, Message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { StatusCode = 500, Message = ex.Message });
        //    }
        //}


        ////------------ wdv

        //[HttpGet("GetCompanyActWDV")]
        //public async Task<IActionResult> GetCompanyActWDV(
        //    [FromQuery] CompanyActWDVRequestDto request)
        //{
        //    if (request == null)
        //        return BadRequest(new
        //        {
        //            statusCode = 400,
        //            message = "Request parameters are required"
        //        });

        //    try
        //    {
        //        var data = await _DepreciationComputationService.CalculateCompanyActWDVAsync(
        //            request.CompId,
        //            request.YearId,
        //            request.CustId,
        //            request.NoOfDays,
        //            request.TotalDays,
        //            request.Duration,
        //            request.StartDate,
        //            request.EndDate,
        //            request.Method
        //        );

        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Company Act WDV depreciation calculated successfully",
        //            success = true,
        //            data = data
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // ⚠️ log ex in real app
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "Internal server error",
        //            error = ex.Message
        //        });
        //    }
        //}

        ////--------sln
        //[HttpGet("company-act")]
        //public async Task<IActionResult> GetCompanyActDepreciation(
        //            [FromQuery] int compId,
        //            [FromQuery] int yearId,
        //            [FromQuery] int custId,
        //            [FromQuery] int noOfDays,
        //            [FromQuery] int totalDays,
        //            [FromQuery] int duration,
        //            [FromQuery] DateTime startDate,
        //            [FromQuery] DateTime endDate,
        //            [FromQuery] int method)
        //{
        //    try
        //    {
        //        var data = await _DepreciationComputationService.CalculateCompanyActDepreciationAsync(
        //            compId,
        //            yearId,
        //            custId,
        //            noOfDays,
        //            totalDays,
        //            duration,
        //            startDate,
        //            endDate,
        //            method
        //        );

        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Company Act depreciation calculated successfully",
        //            success = true,
        //            data = data
        //        });
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(new
        //        {
        //            statusCode = 400,
        //            message = ex.Message,
        //            success = false
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // log ex in real app
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "Internal server error",
        //            success = false,
        //            error = ex.Message
        //        });
        //    }
        //}

        //--------------finalCalculateeDepreciationAsync
        [HttpGet("calculate")]
        public async Task<IActionResult> CalculateDepreciation(
     [FromQuery] DepreciatiionRequestDto request)
        {
            try
            {
                var result = await _DepreciationComputationService.CalculateDepreciationAsync(
                    request.DepBasis,
                    request.CompId,
                    request.YearId,
                    request.CustId,
                    request.NoOfDays,
                    request.TotalDays,
                    request.Duration,
                    request.StartDate,
                    request.EndDate,
                    request.Method
                );

                // ✅ FIX: check result, not DepBasis
                if (result == null)
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        success = false,
                        message = "No depreciation data found"
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    success = true,
                    message = "Depreciation calculated successfully",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    success = false,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }

        // ITcalculation
        [HttpGet("CalculateITAct")]
        public async Task<IActionResult> GetITActDepreciation(
            [FromQuery] string NameSpace,
            [FromQuery] int compId,
            [FromQuery] int yearId,
            [FromQuery] int custId,
            [FromQuery] DateTime endDate)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(NameSpace))
                    return BadRequest("SNameSpace is required.");
                if (compId <= 0)
                    return BadRequest("CompId must be greater than zero.");
                if (yearId <= 0)
                    return BadRequest("YearId must be greater than zero.");
                if (custId <= 0)
                    return BadRequest("CustId must be greater than zero.");

                var request = new ITDepreciationRequestDto
                {
                    NameSpace = NameSpace,
                    CompId = compId,
                    YearId = yearId,
                    CustId = custId,
                    EndDate = endDate
                };

                var result = await _DepreciationComputationService.CalculateITActDepreciationAsync(request);

                if (result == null || !result.Any())
                    return NotFound("No depreciation data found for the given criteria.");

                return Ok(new
                {
                    StatusCode = 200,
                    Success = true,
                    Message = "IT Act Depreciation calculated successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                // Log exception (optional)
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "An error occurred while calculating IT Act Depreciation",
                    Error = ex.Message
                });
            }
        }

        //CompanyCalculation
        //[HttpGet("calculationwdv")]
        //public async Task<IActionResult> GetWDVDepreciation(
        //    [FromQuery] int companyId,
        //    [FromQuery] int customerId,
        //    [FromQuery] int financialYearId,
        //    [FromQuery] DateTime startDate,
        //    [FromQuery] DateTime endDate,
        //    [FromQuery]string customerCode)
        //{
        //    // ---------------------------
        //    // 400 – Validation
        //    // ---------------------------
        //    if (companyId <= 0 || customerId <= 0 || financialYearId <= 0)
        //        return BadRequest("Company, Customer and FinancialYear are required.");

        //    if (startDate > endDate)
        //        return BadRequest("StartDate cannot be greater than EndDate.");

        //    try
        //    {
        //        var request = new DepreciationRequest
        //        {
        //            CompanyId = companyId,
        //            CustomerId = customerId,
        //            FinancialYearId = financialYearId,
        //            StartDate = startDate,
        //            EndDate = endDate,
        //            CustomerCode = customerCode   // from query/header
        //        };

        //        var result = await _DepreciationComputationService.CalculateWDVAsync(request);

        //        // 204 – No Content
        //        if (result == null || result.Count == 0)
        //            return NoContent();

        //        // 200 – Success
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        // 500 – Server Error
        //        return StatusCode(500, new
        //        {
        //            message = "Error while calculating depreciation",
        //            error = ex.Message
        //        });
        //    }
        //}


    }

}












