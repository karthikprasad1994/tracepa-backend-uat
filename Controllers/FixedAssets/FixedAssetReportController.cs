using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TracePca.Interface.FixedAssetsInterface;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FixedAssets
{
    [Route("api/[controller]")]
    [ApiController]
    public class FixedAssetReportController : ControllerBase
    {
        private FixedAssetReportInterface _FixedAssetReportInterface;
        private FixedAssetReportInterface _FixedAssetReportService;
        private readonly IDbConnection _connection;

        public FixedAssetReportController(FixedAssetReportInterface FixedAssetReportInterface)
        {
            _FixedAssetReportInterface = FixedAssetReportInterface;
            _FixedAssetReportService = FixedAssetReportInterface;
        }

        //Report(Ok)
        //[HttpPost("GetReport")]
        //public async Task<IActionResult> GetReport(
        //    [FromBody] FixedAssetReportRequestDto request)
        //{
        //    if (request == null)
        //        return BadRequest("Request body is required");

        //    try
        //    {
        //        var data = await _FixedAssetReportService.GetFixedAssetReportAsync(request);

        //        if (data == null || data.Count == 0)
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "No records found.",
        //                data = new List<object>()
        //            });


        //        return Ok(new
        //        {
        //            Status = 200,
        //            Message = "Report generated successfully",
        //            Data = data
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            Status = 500,
        //            Message = ex.Message
        //        });
        //    }
        //}

        //-----------


        [HttpGet("GetReport")]
        public async Task<IActionResult> GetReport(
            [FromQuery] int ReportType,
            [FromQuery] int CompId,
            [FromQuery] int CustId,
            [FromQuery] int YearId,
            [FromQuery] int locationIds,
            [FromQuery] int MethodType, 
            [FromQuery] string financialYear
        )
        {
            try
            {
                var data = await _FixedAssetReportService.GetFixedAssetReportAsync(
                    ReportType,
                    CompId,
                    CustId,
                    YearId,
                    MethodType,
                    locationIds,
                    financialYear
                );

                return Ok(new
                {
                    Status = 200,
                    Message = "Report generated successfully",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Message = "Internal server error",
                    Error = ex.Message
                });
            }
        }

        //Report(Go)
        //[HttpGet("dynamic-detailed")]
        //public async Task<IActionResult> GetDynamicDetailedReport(
        //    [FromQuery] string nameSpace,
        //    [FromQuery] int compId,
        //    [FromQuery] int yearId,
        //    [FromQuery] int custId,
        //    [FromQuery] string locationIds,
        //    [FromQuery] string divisionIds,
        //    [FromQuery] string departmentIds,
        //    [FromQuery] string bayIds,
        //    [FromQuery] int assetClassId,
        //    [FromQuery] int transType,
        //    [FromQuery] int inAmount,
        //    [FromQuery] int roundOff)
        //{
        //    var dt = await _FixedAssetReportService.LoadDynCompanyDetailedActAsync(
        //        nameSpace,
        //        compId,
        //        yearId,
        //        custId,
        //        locationIds,
        //        divisionIds,
        //        departmentIds,
        //        bayIds,
        //        assetClassId,
        //        transType,
        //        inAmount,
        //        roundOff
        //    );

        //    var result = new List<object>();

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        result.Add(new
        //        {
        //            AssetClass = row["AssetClass"]?.ToString(),
        //            Asset = row["Asset"]?.ToString(),
        //            AssetCode = row["AssetCode"]?.ToString(),

        //            Costasat = row["Costasat"]?.ToString(),
        //            Additions = row["Additions"]?.ToString(),
        //            Deletions = row["Deletions"]?.ToString(),
        //            TotalAmount = row["TotalAmount"]?.ToString(),

        //            DepUptoPY = row["DepUptoPY"]?.ToString(),
        //            DepOnOpengBal = row["DepOnOpengBal"]?.ToString(),
        //            DepOnAdditions = row["DepOnAdditions"]?.ToString(),
        //            DepOnDeletions = row["DepOnDeletions"]?.ToString(),

        //            TotalDepFY = row["TotalDepFY"]?.ToString(),
        //            TotalDepasOn = row["TotalDepasOn"]?.ToString(),

        //            WDVasOn = row["WDVasOn"]?.ToString(),
        //            WDVasOnPY = row["WDVasOnPY"]?.ToString()
        //        });
        //    }

        //    return Ok(result);
        //}



        //[HttpGet("LoadDynComnyDetailedAct")]
        //public async Task<IActionResult> LoadDynComnyDetailedAct(
        //    [FromQuery] string nameSpace,
        //    [FromQuery] int acId,
        //    [FromQuery] int yearId,
        //    [FromQuery] int custId,
        //    [FromQuery] string locationIds,
        //    [FromQuery] string divisionIds,
        //    [FromQuery] string departmentIds,
        //    [FromQuery] string bayIds,
        //    [FromQuery] int assetClassId,
        //    [FromQuery] int transType,
        //    [FromQuery] int inAmount = 0,
        //    [FromQuery] int roundOff = 2)
        //{
        //    try
        //    {
        //        var result = await _FixedAssetReportService.LoadDynComnyDetailedActAsync(
        //            nameSpace, acId, yearId, custId, locationIds, divisionIds, departmentIds, bayIds, assetClassId, transType, inAmount, roundOff);

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        // log the exception here if needed
        //        return StatusCode(500, new { message = ex.Message });
        //    }
        //}


       

            [HttpGet("company-detailed-act")]
            public async Task<IActionResult> GetCompanyDetailedAct(
                [FromQuery] string nameSpace,
                [FromQuery] int acId,
                [FromQuery] int yearId,
                [FromQuery] int custId,
                [FromQuery] string locationIds = "",
                [FromQuery] string divisionIds = "",
                [FromQuery] string departmentIds = "",
                [FromQuery] string bayIds = "",
                [FromQuery] int assetClassId = 0,
                [FromQuery] int transType = 0,
                [FromQuery] int inAmount = 0,
                [FromQuery] int roundOff = 2)
            {
                try
                {
                    var data = await _FixedAssetReportService.LoadDynComnyDetailedActAsync(
                        nameSpace,
                        acId,
                        yearId,
                        custId,
                        locationIds,
                        divisionIds,
                        departmentIds,
                        bayIds,
                        assetClassId,
                        transType,
                        inAmount,
                        roundOff
                    );

                    // 200 - Success with data
                    if (data != null && data.Any())
                    {
                        return StatusCode(StatusCodes.Status200OK, new
                        {
                            success = true,
                            count = data.Count(),
                            data
                        });
                    }

                    // 204 - No Content
                    return StatusCode(StatusCodes.Status204NoContent, new
                    {
                        success = true,
                        message = "No records found"
                    });
                }
                catch (SqlException sqlEx)
                {
                    // 500 - Database error
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        success = false,
                        message = "Database error occurred",
                        error = sqlEx.Message
                    });
                }
                catch (Exception ex)
                {
                    // 500 - General error
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        success = false,
                        message = "An unexpected error occurred",
                        error = ex.Message
                    });
                }
            }

        //---


            //[HttpGet("GetReport")]
            //public async Task<IActionResult> GetReport(
            //    int CompId,
            //    int CustId,
            //    int YearId,
            //    string LocationIds,
            //    string DivisionIds,
            //    string DepartmentIds,
            //    string BayIds,
            //    int AssetClass,
            //    int TransType,
            //    int AmountType,
            //    int RoundOff)
            //{
            //    try
            //    {
            //        var data = await _FixedAssetReportService.LoadDynComDetailedReportAsync(
            //            "FA",
            //            CompId,
            //            YearId,
            //            CustId,
            //            LocationIds,
            //            DivisionIds,
            //            DepartmentIds,
            //            BayIds,
            //            AssetClass,
            //            TransType,
            //            AmountType,
            //            RoundOff);

            //        return Ok(new
            //        {
            //            Status = 200,
            //            Message = "Report generated successfully",
            //            Data = data
            //        });
            //    }
            //    catch (Exception ex)
            //    {
            //        return StatusCode(500, new
            //        {
            //            Status = 500,
            //            Message = ex.Message,
            //            StackTrace = ex.StackTrace
            //        });
            //    }
            //}
       

    }

}

