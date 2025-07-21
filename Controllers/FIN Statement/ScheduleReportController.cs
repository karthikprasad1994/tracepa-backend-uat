using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.ScheduleReportDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleReportController : ControllerBase
    {
        private ScheduleReportInterface _ScheduleReportInterface;
        private ScheduleReportInterface _ScheduleReportService;

        public ScheduleReportController(ScheduleReportInterface ScheduleReportInterface)
        {
            _ScheduleReportInterface = ScheduleReportInterface;
            _ScheduleReportService = ScheduleReportInterface;
        }

        //GetCompanyName
        [HttpGet("GetCompanyName")]
        public async Task<IActionResult> GetCopanyName([FromQuery] int CompId)
        {
            try
            {
                var result = await _ScheduleReportService.GetCompanyNameAsync(CompId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No company details found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Company details loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching company details.",
                    error = ex.Message
                });
            }
        }

        //GetPartner
        [HttpGet("LoadCustomerPartners")]
        public async Task<ActionResult<IEnumerable<PartnersDto>>> LoadCustomerPartners(int CompId, int DetailsId)
        {
            try
            {
                var result = await _ScheduleReportService.LoadCustomerPartnersAsync(CompId, DetailsId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // optionally log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //GetSubHeading
        [HttpGet("GetSubHeading")]
        public async Task<IActionResult> GetSubHeading([FromQuery] int CompId, [FromQuery] int ScheduleId, [FromQuery] int CustId, [FromQuery] int HeadingId)
        {
            try
            {
                if (HeadingId <= 0)
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "HeadingId is required and must be greater than zero.",
                        data = (object)null
                    });
                }

                var result = await _ScheduleReportService.GetSubHeadingAsync(
                    CompId, ScheduleId, CustId, HeadingId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No subheadings found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Subheadings fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving subheadings.",
                    error = ex.Message
                });
            }
        }

        //Getitem
        [HttpGet("GetItem")]
        public async Task<IActionResult> GetItem(
        [FromQuery] int CompId, [FromQuery] int ScheduleId, [FromQuery] int CustId, [FromQuery] int HeadingId, [FromQuery] int SubHeadId)
        {
            try
            {
                // Call service method
                var result = await _ScheduleReportService.GetItemAsync(
                    CompId, ScheduleId, CustId, HeadingId, SubHeadId);

                // Check for no data
                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No schedule items found.",
                        data = (object)null
                    });
                }

                // Return success
                return Ok(new
                {
                    statusCode = 200,
                    message = "Schedule items fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving schedule items.",
                    error = ex.Message
                });
            }
        }

        ////GetSummaryReportForPandL(Income)
        //[HttpGet("GetSummaryPnL(Income)")]
        //public async Task<IActionResult> GetSummaryPnLIncome([FromQuery] int CompId, [FromQuery] SummaryPnLIncome p)
        //{
        //    try
        //    {
        //        // Minimal validation
        //        if (p.YearID <= 0 || p.CustID <= 0 || p.ScheduleTypeID <= 0)
        //        {
        //            return BadRequest(new
        //            {
        //                statusCode = 400,
        //                message = "YearID, CustID, and ScheduleTypeID are required and must be greater than zero.",
        //                data = (object)null
        //            });
        //        }

        //        var result = await _ScheduleReportService.GetSummaryPnLIncomeAsync(CompId, p);

        //        if (result == null || !result.Any())
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "No summary P&L data found.",
        //                data = (object)null
        //            });
        //        }

        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Summary P&L data fetched successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while retrieving Summary P&L data.",
        //            error = ex.Message
        //        });
        //    }
        //}

        ////GetSummaryReportForPandL(Expenses)
        //[HttpGet("GetSummaryPnL(Expenses)")]
        //public async Task<IActionResult> GetSummaryPnLExpenses([FromQuery] int CompId, [FromQuery] SummaryPnLExpenses dto)
        //{
        //    try
        //    {
        //        // Minimal validation
        //        if (dto.YearID <= 0 || dto.CustID <= 0 || dto.ScheduleTypeID <= 0)
        //        {
        //            return BadRequest(new
        //            {
        //                statusCode = 400,
        //                message = "YearID, CustID, and ScheduleTypeID are required and must be greater than zero.",
        //                data = (object)null
        //            });
        //        }

        //        var result = await _ScheduleReportService.GetSummaryPnLExpensesAsync(CompId, dto);

        //        if (result == null || !result.Any())
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "No summary P&L data found.",
        //                data = (object)null
        //            });
        //        }

        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Summary P&L data fetched successfully.",
        //            data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            statusCode = 500,
        //            message = "An error occurred while retrieving Summary P&L data.",
        //            error = ex.Message
        //        });
        //    }
        //}


        //GetSummaryReportForPandL
        [HttpGet("GetSummaryPnL")]
        public async Task<IActionResult> GetSummaryReportPnL([FromQuery] int CompId, [FromQuery] SummaryReportPnL dto)
        {
            try
            {
                // Minimal validation
                if (dto.YearID <= 0 || dto.CustID <= 0)
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "YearID, CustID, and ScheduleTypeID are required and must be greater than zero.",
                        data = (object)null
                    });
                }

                var result = await _ScheduleReportService.GetReportSummaryPnLAsync(CompId, dto);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No summary P&L data found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Summary P&L data fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving Summary P&L data.",
                    error = ex.Message
                });
            }
        }

        //GetSummaryReportForBalanceSheet
        [HttpGet("GetSummaryBalanceSheet")]
        public async Task<IActionResult> GetSummaryReportBalanceSheet([FromQuery] int CompId, [FromQuery] SummaryReportBalanceSheet dto)
        {
            try
            {
                // Minimal validation
                if (dto.YearID <= 0 || dto.CustID <= 0)
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "YearID, CustID, and ScheduleTypeID are required and must be greater than zero.",
                        data = (object)null
                    });
                }

                var result = await _ScheduleReportService.GetReportSummaryBalanceSheetAsync(CompId, dto);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No summary P&L data found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Summary P&L data fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving Summary P&L data.",
                    error = ex.Message
                });
            }
        }

        //GetDetailedreportPandL
        [HttpGet("GetDetailedReport")]
        public async Task<IActionResult> GetDetailedReport([FromQuery] DetailedReportParams p)
        {
            try
            {
                var result = await _ScheduleReportService.GetDetailedReportAsync(p);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No data found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Data fetched successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving the data.",
                    error = ex.Message
                });
            }
        }
    }
}

