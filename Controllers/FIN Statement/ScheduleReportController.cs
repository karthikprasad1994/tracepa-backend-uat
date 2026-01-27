using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.Audit;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.Audit;
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

                var result = await _ScheduleReportService.GetSubHeadingAsync(CompId, ScheduleId, CustId, HeadingId);

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
        public async Task<IActionResult> GetItem([FromQuery] int CompId, [FromQuery] int ScheduleId, [FromQuery] int CustId, [FromQuery] int HeadingId, [FromQuery] int SubHeadId)
        {
            try
            {
                // Call service method
                var result = await _ScheduleReportService.GetItemAsync(CompId, ScheduleId, CustId, HeadingId, SubHeadId);

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

        //GetDetailedReportPandL
        [HttpGet("GetDetailedReportPandL")]
        public async Task<IActionResult> GetDetailedReportPandL([FromQuery] int CompId, [FromQuery] DetailedReportPandL dto)
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

                var result = await _ScheduleReportService.GetDetailedReportPandLAsync(CompId, dto);

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

        //GetDetailedReportBalanceSheet
        [HttpGet("GetDetailedreportBalanceSheet")]
        public async Task<IActionResult> GetDetailedReportBalanceSheet([FromQuery] int CompId, [FromQuery] DetailedReportBalanceSheet dto)
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

                var result = await _ScheduleReportService.GetDetailedReportBalanceSheetAsync(CompId, dto);

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

        [HttpPost("GetScheduleReportAllDetails")]
        public async Task<IActionResult> GetScheduleReport([FromBody] ScheduleReportRequestDto request)
        {
            var result = await _ScheduleReportService.GetScheduleReportDetailsAsync(request);
            return Ok(result);
        }
        [HttpPost("GetOrgTypeAndDirectors")]
        public async Task<IActionResult> GetOrgTypeAndMembers([FromBody] OrgTypeRequestDto request)
        {
            if (request.CustomerId <= 0 || request.CompanyId <= 0)
                return BadRequest("Invalid input.");

            var result = await _ScheduleReportService.GetOrgTypeAndMembersAsync(request.CustomerId, request.CompanyId);
            return Ok(result);
        }
        [HttpGet("GetCompanyDetails")]
        public async Task<IActionResult> GetCompanyDetails(int compId)
        {
            var result = await _ScheduleReportService.LoadCompanyDetailsAsync(compId);
            return Ok(result);
        }

        //UpdatePnL
        [HttpPost("UpdatePnL")]
        public async Task<IActionResult> UpdatePnLAsync([FromBody] UpdatePnlRequestDto request)
        {
            try
            {


                var result = await _ScheduleReportService.UpdatePnLAsync(request.PnLAmount, request.CompId, request.CustId, request.UserId, request.YearId, request.BranchId, request.DurationId);

                return Ok(new
                {
                    statusCode = result ? 200 : 400,
                    message = result ? "PnL updated successfully." : "Failed to update PnL.",
                    success = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while updating PnL.",
                    error = ex.Message
                });
            }
        }
        [HttpPost("SavePartner")]
        public async Task<IActionResult> SaveCustomerStatutoryPartner([FromBody] StatutoryPartnerDto partnerDto)
        {
            var result = await _ScheduleReportService.SaveCustomerStatutoryPartnerAsync(partnerDto);

            return Ok(new
            {
                Success = true,
                UpdateOrSave = result.iUpdateOrSave,
                Operation = result.iOper
            });
        }
        [HttpPost("savestatutorydirector")]
        public async Task<IActionResult> SaveCustomerStatutoryDirector([FromBody] StatutoryDirectorDto directorDto)
        {
         
            var result = await _ScheduleReportService.SaveCustomerStatutoryDirectorAsync(directorDto);

            return Ok(new
            {
                Success = true,
                iUpdateOrSave = result.iUpdateOrSave,
                iOper = result.iOper
            });
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int directorId)
        {
            if (directorId <= 0)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = "Invalid Director ID"
                });
            }

            try
            {
                var result = await _ScheduleReportService.GetDirectorByIdAsync(directorId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "Director not found"
                    });
                }

                return Ok(new
                {
                    status = 200,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "Internal server error",
                    details = ex.Message
                });
            }
        }
        [HttpGet("get-customer-amount-settings")]
        public async Task<IActionResult> GetCustomerAmountSettings(int customerId)
        {
            var result = await _ScheduleReportService.GetCustomerAmountSettingsAsync(customerId);

            if (result == null)
                return NotFound("Customer not found");

            return Ok(result);
        }

        //SaveFinancialStatement
        //[HttpPost("SaveOrUpdateEngagementPlan")]
        //public async Task<IActionResult> SaveOrUpdateFinancialStatement( [FromBody] SREngagementPlanDetailsDTO dto)
        //{
        //    try
        //    {
        //        if (dto == null)
        //        {
        //            return BadRequest(new
        //            {
        //                StatusCode = 400,
        //                Message = "Invalid request data."
        //            });
        //        }

        //        var loeId = await _ScheduleReportService.SaveOrUpdateFinancialStatementAsync(dto);

        //        return Ok(new
        //        {
        //            StatusCode = 200,
        //            Message = dto.LOE_Id > 0
        //                ? "Engagement plan updated successfully."
        //                : "Engagement plan saved successfully.",
        //            Data = new
        //            {
        //                LOE_Id = loeId
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            StatusCode = 500,
        //            Message = "An error occurred while saving engagement plan data.",
        //            Error = ex.Message
        //        });
        //    }
        //}

        //[HttpPost("save-or-update")]
        //public async Task<IActionResult> SaveOrUpdateFinancialStatement([FromBody] FinancialStatementDTO dto)
        //{
        //    if (dto == null)
        //    {
        //        return BadRequest(new
        //        {
        //            status = 400,
        //            message = "Invalid request data"
        //        });
        //    }

        //    try
        //    {
        //        // Call the service to save or update LOE templates
        //        int loeId = await _ScheduleReportService.SaveOrUpdateFinancialStatementAsync(dto);

        //        return Ok(new
        //        {
        //            status = 200,
        //            message = "Engagement plan saved successfully",
        //            data = new
        //            {
        //                LOE_Id = loeId
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            status = 500,
        //            message = "Internal server error",
        //            details = ex.Message
        //        });
        //    }
        //}

        [HttpPost("save-or-update")]
        public async Task<IActionResult> SaveOrUpdateLOE([FromBody] SaveLOEDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = "Invalid request data"
                });
            }
            try
            {
                int loeId = await _ScheduleReportService.SaveOrUpdateLOEAsync(dto);

                return Ok(new
                {
                    status = 200,
                    message = "Engagement plan saved successfully",
                    data = new
                    {
                        LOE_Id = loeId
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "Internal server error",
                    details = ex.Message
                });
            }
        }

        [HttpGet("net-income-zero")]
        public async Task<ActionResult<bool>> IsNetIncomeZero(
      int yearId,
      int custId,
      int branchId, int duration)
        {
            bool result = await _ScheduleReportService
                .IsNetIncomeZeroAsync(yearId, custId, branchId, duration);

            return Ok(result);
        }
    }
}
