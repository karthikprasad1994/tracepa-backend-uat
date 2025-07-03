using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.Audit;
using TracePca.Interface.FIN_Statement;

using TracePca.Service.FIN_statement;

using static TracePca.Dto.FIN_Statement.ScheduleMappingDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleMappingController : ControllerBase
    {
        private ScheduleMappingInterface _ScheduleMappingInterface;
        private ScheduleMappingInterface _ScheduleMappingService;

        public ScheduleMappingController(ScheduleMappingInterface ScheduleMappingInterface)
        {
            _ScheduleMappingInterface = ScheduleMappingInterface;
            _ScheduleMappingService = ScheduleMappingInterface;
        }

        //GetCustomersName
        [HttpGet("GetCustomersName")]
        public async Task<IActionResult> GetCustomerName([FromQuery] int CompId)
        {

            try
            {
                var result = await _ScheduleMappingService.GetCustomerNameAsync(CompId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No company types found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Company types loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching company types.",
                    error = ex.Message
                });
            }
        }

        //GetFinancialYear
        [HttpGet("GetFinancialYear")]
        public async Task<IActionResult> GetFinancialYear([FromQuery] int CompId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetFinancialYearAsync(CompId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Financial year types found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Financial year loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching company types.",
                    error = ex.Message
                });
            }
        }

        //GetDuration
        [HttpGet("GetCustomerDurationId")]
        public async Task<IActionResult> GetCustomerDurationId([FromQuery] int CompId, [FromQuery] int CustId)
        {
            try
            {
                var durationId = await _ScheduleMappingService.GetCustomerDurationIdAsync(CompId, CustId);

                if (durationId == null)
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "Duration ID not found for the provided Company ID and Customer ID.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Duration ID retrieved successfully.",
                    data = new { Cust_DurtnId = durationId }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving the duration ID.",
                    error = ex.Message
                });
            }
        }


        //GetBranchName
        [HttpGet("GetBranchName")]
        public async Task<IActionResult> GetBranchName([FromQuery] int CompId, [FromQuery] int CustId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetBranchNameAsync(CompId, CustId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Branch name found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Branch name loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching company types.",
                    error = ex.Message
                });
            }
        }

        //GetScheduleHeading
        [HttpGet("GetScheduleHeading")]
        public async Task<IActionResult> GetScheduleHeadings(int CompId, int CustId, int ScheduleTypeId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetScheduleHeadingAsync(CompId, CustId, ScheduleTypeId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Heading types found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Heading types loaded successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching company types.",
                    error = ex.Message
                });
            }
        }

        //GetScheduleSub-Heading
        [HttpGet("GetScheduleSubHeading")]
        public async Task<IActionResult> GetScheduleSubHeading(int CompId, int CustId, int ScheduleTypeId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetScheduleItemAsync(CompId, CustId, ScheduleTypeId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Sub-Heading items found.",
                        data = new List<object>()
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Sub-Heading retrieved successfully.",
                    data = result.Select(item => new
                    {
                        id = item.ASI_ID,
                        name = item.ASI_Name
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving schedule items.",
                    error = ex.Message
                });
            }
        }

        //GetScheduleItem
        [HttpGet("GetScheduleItem")]
        public async Task<IActionResult> GetScheduleItem(int CompId, int CustId, int ScheduleTypeId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetScheduleItemAsync(CompId, CustId, ScheduleTypeId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No schedule items found.",
                        data = new List<object>()
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Schedule items retrieved successfully.",
                    data = result.Select(item => new
                    {
                        id = item.ASI_ID,
                        name = item.ASI_Name
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving schedule items.",
                    error = ex.Message
                });
            }
        }

        //GetScheduleSub-Item
        [HttpGet("GetScheduleSubItem")]
        public async Task<IActionResult> GetScheduleSubItem(int CompId, int CustId, int ScheduleTypeId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetScheduleSubItemAsync(CompId, CustId, ScheduleTypeId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No schedule sub items found.",
                        data = new List<object>()
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Schedule sub items retrieved successfully.",
                    data = result.Select(item => new
                    {
                        id = item.ASSI_ID,
                        name = item.ASSI_Name
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving schedule items.",
                    error = ex.Message
                });
            }
        }

        ////SaveOrUpdateTrialBalanceUpload
        //[HttpPost("SaveOrUpdateTrailBalanceUpload")]
        //public async Task<IActionResult> SaveTrailBalanceUploadAsync([FromQuery] int iCompId, [FromBody] TrailBalanceUploadDto dto)
        //{
        //    if (dto == null)
        //    {
        //        return BadRequest(new
        //        {
        //            Status = 400,
        //            Message = "Invalid input: DTO is null",
        //            Data = (object)null
        //        });
        //    }

        //    try
        //    {
        //        bool isUpdate = dto.ATBU_ID > 0;

        //        var result = await _ScheduleMappingService.SaveTrailBalanceUploadAsync(iCompId, dto);

        //        string successMessage = isUpdate
        //            ? "Trail balance upload details successfully updated."
        //            : "Trail balance upload details successfully created.";

        //        return Ok(new
        //        {
        //            Status = 200,
        //            Message = successMessage,
        //            Data = new
        //            {
        //                UpdateOrSave = result[0],
        //                Oper = result[1],
        //                IsUpdate = isUpdate
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            Status = 500,
        //            Message = "An error occurred while processing your request.",
        //            Error = ex.Message,
        //            InnerException = ex.InnerException?.Message
        //        });
        //    }
        //}

        ////SaveOrUpdateTrialBalanceUploadDetails
        //[HttpPost("SaveOrUpdateTrailBalanceUploadDetails")]
        //public async Task<IActionResult> SaveTrailBalanceUploadDetailsAsync([FromQuery] int iCompId, [FromBody] TrailBalanceUploadDetailsDto dto)
        //{
        //    if (dto == null)
        //    {
        //        return BadRequest(new
        //        {
        //            Status = 400,
        //            Message = "Invalid input: DTO is null",
        //            Data = (object)null
        //        });
        //    }

        //    try
        //    {
        //        bool isUpdate = dto.ATBUD_ID > 0;

        //        var result = await _ScheduleMappingService.SaveTrailBalanceUploadDetailsAsync(iCompId, dto);

        //        string successMessage = isUpdate
        //            ? "Trail balance upload details successfully updated."
        //            : "Trail balance upload details successfully created.";

        //        return Ok(new
        //        {
        //            Status = 200,
        //            Message = successMessage,
        //            Data = new
        //            {
        //                UpdateOrSave = result[0],
        //                Oper = result[1],
        //                IsUpdate = isUpdate
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            Status = 500,
        //            Message = "An error occurred while processing your request.",
        //            Error = ex.Message,
        //            InnerException = ex.InnerException?.Message
        //        });
        //    }
        //}

        //GetTotalAmount
        [HttpGet("GetTotalAmount")]
        public async Task<IActionResult> GetCustCOAMasterDetails(
        int CompId, int CustId, int YearId, int BranchId, int DurationId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetCustCOAMasterDetailsAsync(CompId, CustId, YearId, BranchId, DurationId);

                if (result != null && result.Any())
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Data fetched successfully",
                        data = result
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No data found",
                        data = Enumerable.Empty<CustCOASummaryDto>()
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching data",
                    error = ex.Message
                });
            }
        }

        //GetTrialBalance(Grid)
        [HttpGet("GetTrailBalance(Grid)")]
        public async Task<IActionResult> GetCustCOADetails(
    int CompId, int CustId, int YearId, int ScheduleTypeId, int Unmapped, int BranchId, int DurationId)
        {
            try
            {
                var result = await _ScheduleMappingService.GetCustCOADetailsAsync(CompId, CustId, YearId, ScheduleTypeId, Unmapped, BranchId, DurationId);

                if (result != null && result.Any())
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        message = "Data fetched successfully",
                        data = result
                    });
                }

                return NotFound(new
                {
                    statusCode = 404,
                    message = "No data found",
                    data = Enumerable.Empty<CustCOADetailsDto>()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching data",
                    error = ex.Message
                });
            }
        }

        //SaveScheduleTemplate
        [HttpPost("SaveScheduleTemplate")]
        public async Task<IActionResult> UploadTrialBalance([FromQuery] int CompanyId, [FromBody] AccTrailBalanceUploadBatchDto dto)
        {
            try
            {
                var resultIds = await _ScheduleMappingService.UploadTrialBalanceExcelAsync(CompanyId, dto);

                return Ok(new
                {
                    status = 200,
                    message = "Schedule Template uploaded successfully.",
                    data = new
                    {
                        ResultIds = resultIds
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while uploading trial balance.",
                    error = ex.Message
                });
            }
        }

        //UploadExcelFile
        //[HttpPost("UploadExcelFile")]
        //public async Task<IActionResult> UploadExcelFile(
        //[FromForm] IFormFile file,
        //[FromForm] int clientId,
        //[FromForm] int branchId,
        //[FromForm] int yearId,
        //[FromForm] int quarter,
        //[FromForm] string accessCode,
        //[FromForm] int accessCodeId,
        //[FromForm] string username)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest(new
        //        {
        //            status = 400,
        //            message = "No file uploaded or file is empty.",
        //            data = (object)null
        //        });
        //    }

        //    try
        //    {
        //        var result = await _ScheduleMappingService.UploadScheduleExcelAsync(
        //            file, clientId, branchId, yearId, quarter, accessCode, accessCodeId, username
        //        );

        //        return Ok(new
        //        {
        //            status = 200,
        //            message = result.Message,
        //            data = new
        //            {
        //                sheetNames = result.SheetNames,
        //                isExistingData = result.IsExistingData
        //            }
        //        });
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(new
        //        {
        //            status = 400,
        //            message = ex.Message,
        //            data = (object)null
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            status = 500,
        //            message = "An error occurred while uploading the file.",
        //            error = ex.Message
        //        });
        //    }
        //}

        //FreezeForPreviousDuration
        [HttpPost("FreezePreviousDurationTrialBalance")]
        public async Task<IActionResult> FreezePreviousYearTrialBalance([FromBody] FreezePreviousDurationRequestDto input)
        {
            try
            {
                var detailIds = await _ScheduleMappingService.FreezePreviousYearTrialBalanceAsync(input);

                return Ok(new
                {
                    status = 200,
                    message = "Previous year trial balance frozen successfully.",
                    data = new
                    {
                        DetailIds = detailIds
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while freezing previous year trial balance.",
                    error = ex.Message
                });
            }
        }

        //FreezrForNextDuration
        [HttpPost("FreezeNextDuratiionrialBalance")]
        public async Task<IActionResult> FreezeNextDurationTrialBalance([FromBody] FreezeNextDurationRequestDto input)
        {
            try
            {
                var detailIds = await _ScheduleMappingService.FreezeNextDurationrialBalanceAsync(input);

                return Ok(new
                {
                    status = 200,
                    message = "Previous year trial balance frozen successfully.",
                    data = new
                    {
                        DetailIds = detailIds
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while freezing previous year trial balance.",
                    error = ex.Message
                });
            }
        }

        //DownloadUploadableExcelAndTemplate
        [HttpGet("DownloadableExcelFile")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public IActionResult DownloadExcelTemplate()
        {
            var result = _ScheduleMappingService.GetExcelTemplate();

            if (result.FileBytes == null)
                return NotFound("File not found.");

            return File(result.FileBytes, result.ContentType, result.FileName);
        }

        //SaveTrailBalanceDetails
        [HttpPost("SaveTrailBalanceDetails")]
        public async Task<IActionResult> SaveTrailBalanceDetails([FromQuery] int CompId, [FromBody] TrailBalanceDetailsDto HeaderDto)
        {
            try
            {
                var resultIds = await _ScheduleMappingService.SaveTrailBalanceDetailsAsync(CompId, HeaderDto);

                return Ok(new
                {
                    status = 200,
                    message = "Trail Balance uploaded successfully.",
                    data = new
                    {
                        ResultIds = resultIds
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "An error occurred while uploading trial balance.",
                    error = ex.Message
                });
            }
        }
    }
}




