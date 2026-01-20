using System.Reflection;
using Microsoft.Reporting.WebForms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.WebForms;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.ScheduleFormatDto;
using static TracePca.Dto.FIN_Statement.ScheduleNoteDto;
using System.Data;
using QuestPDF.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleNoteController : ControllerBase
    {
        private ScheduleNoteInterface _ScheduleNoteInterface;
        private ScheduleNoteInterface _ScheduleNoteService;
        private readonly IWebHostEnvironment _env;

        public ScheduleNoteController(ScheduleNoteInterface ScheduleNoteInterface, IWebHostEnvironment env)
        {
            _ScheduleNoteInterface = ScheduleNoteInterface;
            _ScheduleNoteService = ScheduleNoteInterface;
            _env = env;
        }

        //GetSubHeadingname(Notes For SubHeading)
        [HttpGet("SubHeading-NotesForSubHeading")]
        public async Task<IActionResult> GetSubHeadingNotes(int CompId, int CustId, int YearId)
        {
            try
            {
                var notes = await _ScheduleNoteService.GetSubHeadingDetailsAsync(CompId, CustId, YearId);

                if (notes == null || !notes.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No notes found for the specified subheading and customer.",
                        Data = new List<object>()
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Notes retrieved successfully.",
                    Data = notes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching subheading note details.",
                    Error = ex.Message
                });
            }
        }

        //SaveOrUpdateSubHeadingNotes(Notes For SubHeading)
        //[HttpPost("SaveOrUpdateScheduleFormatHeading")]
        //public async Task<IActionResult> aveSubHeadindNotes([FromBody] SubHeadingNotesDto dto)
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
        //        bool isUpdate = dto.ASHN_ID > 0;

        //        var result = await _ScheduleNoteService.SaveSubHeadindNotesAsync(dto);

        //        string successMessage = isUpdate
        //            ? "Schedule Heading successfully updated."
        //            : "Schedule Heading successfully created.";

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

        //SaveOrUpdateSubHeadingNotes(Notes For SubHeading)
        [HttpPost("SaveSubheadingWithNotes")]
        public async Task<IActionResult> SaveSubheadingWithNotes([FromBody] List<SubheadingDto> subheadingDtos)
        {
            try
            {
                if (subheadingDtos == null || subheadingDtos.Count == 0)
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        message = "No subheading data provided.",
                        data = (object)null
                    });
                }

                // Call service to save subheadings + notes
                var savedSubheadings = await _ScheduleNoteService.SaveSubheadingWithNotesAsync(subheadingDtos);

                // Return response in your desired format
                return Ok(new
                {
                    statusCode = 200,
                    message = "Subheadings saved successfully.",
                    data = savedSubheadings
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while saving subheadings with notes.",
                    error = ex.Message
                });
            }
        }

        //LoadGrid(Notes For SubHeading)
        [HttpGet("LoadSubheadingNotes")]
        public async Task<IActionResult> LoadSubheadingNotes([FromQuery] int compId, [FromQuery] int yearId, [FromQuery] int custId)
        {
            try
            {
                var notes = await _ScheduleNoteService.LoadSubheadingNotesAsync(compId, yearId, custId);

                if (notes == null || notes.Count == 0)
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No subheading notes found for the specified company and year.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Subheading notes loaded successfully.",
                    data = notes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while loading subheading notes.",
                    error = ex.Message
                });
            }
        }


        //GetBranch(Notes For Ledger)
        [HttpGet("GetBranchNameNotesForLedger")]
        public async Task<IActionResult> GetBranchName([FromQuery] int CompId, [FromQuery] int CustId)
        {
            try
            {
                var result = await _ScheduleNoteService.GetBranchNameAsync(CompId, CustId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Branch types found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Branch types loaded successfully.",
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

        //GetLedger(Notes For Ledger)
        [HttpGet("GetLedgerNotesForLedger")]
        public async Task<IActionResult> GetLedgerIndividualDetails([FromQuery] int CustomerId, [FromQuery] int SubHeadingId)
        {
            try
            {
                var result = await _ScheduleNoteService.GetLedgerIndividualDetailsAsync(CustomerId, SubHeadingId);

                if (result != null && result.Any())
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        Message = "Ledger individual details fetched successfully.",
                        Data = result
                    });
                }

                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "No ledger individual details found.",
                    Data = Enumerable.Empty<LedgerIndividualDto>()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching ledger individual details.",
                    Error = ex.Message
                });
            }
        }

        //SaveOrUpdateLedger(Notes For Ledger)
        [HttpPost("SaveOrUpdateLedgerDetails")]
        public async Task<IActionResult> SaveLedgerDetails([FromBody] SubHeadingLedgerNoteDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Invalid input: DTO is null",
                    Data = (object)null
                });
            }

            try
            {
                bool isUpdate = dto.ASHL_ID > 0;

                var result = await _ScheduleNoteService.SaveLedgerDetailsAsync(dto);

                string successMessage = isUpdate
                    ? "Ledger details successfully updated."
                    : "Ledger details successfully created.";

                return Ok(new
                {
                    Status = 200,
                    Message = successMessage,
                    Data = new
                    {
                        UpdateOrSave = result[0],
                        Oper = result[1],
                        IsUpdate = isUpdate
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        //DownloadNotesExcel
        [HttpGet("DownloadableExcelFile")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public IActionResult DownloadExcelTemplate()
        {
            var result = _ScheduleNoteService.GetExcelTemplate();

            if (result.FileBytes == null)
                return NotFound("File not found.");

            return File(result.FileBytes, result.ContentType, result.FileName);
        }

        //DownloadNotesPdf
        [HttpGet("DownloadablePdfFile")]
        [Produces("application/pdf")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public IActionResult DownloadPdfTemplate()
        {
            var result = _ScheduleNoteService.GetPdfTemplate();

            if (result.FileBytes == null)
                return NotFound("File not found.");

            return File(result.FileBytes, result.ContentType, result.FileName);
        }

        //SaveOrUpdate
        [HttpPost("SaveFirstScheduleNote")]
        public async Task<IActionResult> SaveFirstScheduleNoteAsync([FromBody] FirstScheduleNoteDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.SNF_Description))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Description is required.",
                        Data = (int?)null
                    });
                }

                var noteId = await _ScheduleNoteService.SaveFirstScheduleNoteDetailsAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.SNF_ID == 0 ? "Note saved successfully." : "Note updated successfully.",
                    Data = noteId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving the schedule note.",
                    Error = ex.Message
                });
            }
        }

        // --PreDefinied Notes //
        //SaveAuthorisedShareCapital(Particulars)
        [HttpPost("SaveAuthorisedShareCapital")]
        public async Task<IActionResult> SaveAuthorisedShareCapital([FromBody] AuthorisedShareCapitalDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Description))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Description is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SaveAuthorisedShareCapitalAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.Id == 0 ? "Authorised Share Capital saved successfully." : "Authorised Share Capital updated successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving Authorised Share Capital.",
                    Error = ex.Message
                });
            }
        }

        //SaveIssuedSubscribedandFullyPaidupShareCapital
        [HttpPost("SaveIssuedSubscribedandFullyPaidupShareCapital")]
        public async Task<IActionResult> SaveIssuedSubscribedandFullyPaidupShareCapital([FromBody] IssuedSubscribedandFullyPaidupShareCapitalAsyncDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Description))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Description is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SaveIssuedSubscribedandFullyPaidupShareCapitalAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.Id == 0 ? "Issued Subscribed and Fully Paidup Share Capital saved successfully." : "Issued Subscribed and Fully Paidup Share Capital updated successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving Issued Subscribed and Fully Paidup Share Capital.",
                    Error = ex.Message
                });
            }
        }

        //Save(A)Issued
        [HttpPost("SaveIssued")]
        public async Task<IActionResult> SaveIssued([FromBody] IssuedDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Description))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Description is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SaveIssuedAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.Id == 0 ? "Issued saved successfully." : "Issued updated successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving Issued.",
                    Error = ex.Message
                });
            }
        }

        //Save(B)SubscribedandPaid-up
        [HttpPost("SaveSubscribedandPaid-up")]
        public async Task<IActionResult> SaveSubscribedandPaidup([FromBody] SubscribedandPaidupDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Description))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Description is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SaveSubscribedandPaidupAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.Id == 0 ? "Subscribed and Paid-up saved successfully." : "Subscribed and Paid-up updated successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving Subscribed and Paid-up.",
                    Error = ex.Message
                });
            }
        }

        //SaveCallsUnpaid
        [HttpPost("SaveCallsUnpaid")]
        public async Task<IActionResult> SaveCallsUnpaid([FromBody] CallsUnpaidDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Description))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Description is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SaveCallsUnpaidAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.Id == 0 ? "Calls Unpaid saved successfully." : "Calls Unpaid updated successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving Calls Unpaid.",
                    Error = ex.Message
                });
            }
        }

        //SaveForfeitedShares
        [HttpPost("SaveForfeitedShares")]
        public async Task<IActionResult> SaveForfeitedShares([FromBody] ForfeitedSharesDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Description))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Description is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SaveForfeitedSharesAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.Id == 0 ? "Forfeited Shares saved successfully." : "Forfeited Shares updated successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving Forfeited Shares.",
                    Error = ex.Message
                });
            }
        }

        //Save(i)EquityShares
        [HttpPost("Save(i)EquityShares")]
        public async Task<IActionResult> SaveEquityShares([FromBody] EquitySharesDto dto)
        {
            try
            {
                // ✅ Validation example (optional, you can add more)
                if (dto.SNS_CustId == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "CustomerId is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SaveEquitySharesAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.SNS_ID == 0 ? "Equity Shares saved successfully." : "Equity Shares delete successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving Equity Shares.",
                    Error = ex.Message
                });
            }
        }

        //Save(ii)PreferenceShares
        [HttpPost("Save(i)PreferenceShares")]
        public async Task<IActionResult> SavePreferenceShares([FromBody] PreferenceSharesDto dto)
        {
            try
            {
                // ✅ Validation example (optional, you can add more)
                if (dto.SNS_CustId == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "CustomerId is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SavePreferenceSharesAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.SNS_ID == 0 ? "Preference Shares saved successfully." : "Preference Shares delete successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving Preference Shares.",
                    Error = ex.Message
                });
            }
        }

        //Save(iii)EquityShares
        [HttpPost("Save(iii)EquityShares")]
        public async Task<IActionResult> SaveiiiEquityShares([FromBody] iiiEquitySharesDto dto)
        {
            try
            {
                // ✅ Validation example (optional, you can add more)
                if (dto.SNS_CustId == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "CustomerId is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SaveiiiEquitySharesAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.SNS_ID == 0 ? "(iii)Equity Shares saved successfully." : "(iii)Equity Shares delete successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving (iii)Equity Shares.",
                    Error = ex.Message
                });
            }
        }

        //Save(iv)PreferenceShares
        [HttpPost("Save(iv)PreferenceShares")]
        public async Task<IActionResult> SaveivPreferenceShares([FromBody] ivPreferenceSharesDto dto)
        {
            try
            {
                // ✅ Validation example (optional, you can add more)
                if (dto.SNS_CustId == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "CustomerId is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SaveivPreferenceSharesAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.SNS_ID == 0 ? "(iii)Equity Shares saved successfully." : "(iii)Equity Shares delete successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving (iii)Equity Shares.",
                    Error = ex.Message
                });
            }
        }

        // Save(b)EquityShareCapital
        [HttpPost("Save(b)EquityShareCapital")]
        public async Task<IActionResult> SavebEquityShareCapital([FromBody] EquityShareCapitalDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Description))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Description is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SavebEquityShareCapitalAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.Id == 0
                        ? "Equity Share Capital saved successfully."
                        : "Equity Share Capital updated successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving Equity Share Capital.",
                    Error = ex.Message
                });
            }
        }

        //Save(b)PreferenceShareCapital
        [HttpPost("Save(b)PreferenceShareCapital")]
        public async Task<IActionResult> SavebPreferenceShareCapital([FromBody] PreferenceShareCapitalDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Description))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Description is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SavebPreferenceShareCapitalAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.Id == 0
                        ? "Preference Share Capital saved successfully."
                        : "Preference Share Capital  updated successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving Preference Share Capital .",
                    Error = ex.Message
                });
            }
        }

        //Save(c)Terms/rights attached to equity shares
        [HttpPost("Save(c)TermsToEquityShare")]
        public async Task<IActionResult> SaveTermsToEquityShare([FromBody] TermsToEquityShareeDto dto)
        {
            try
            {
                // ✅ Validation (optional, can extend with more rules)
                if (dto.CustomerId == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "CustomerId is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SaveTermsToEquityShareAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.Id == 0
                        ? "Description for cEquity Share saved successfully."
                        : "Description for cEquity share deleted successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving description for cEquity Share.",
                    Error = ex.Message
                });
            }
        }

        //Save(d)Terms/Rights attached to preference shares
        [HttpPost("Save(d)TermsToPreferenceShare")]
        public async Task<IActionResult> SaveTermsToPreferenceShare([FromBody] TermsToPreferenceShareDto dto)
        {
            try
            {
                // ✅ Validation (optional, can extend with more rules)
                if (dto.CustomerId == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "CustomerId is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SaveTermsToPreferenceShareAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.Id == 0
                        ? "Description for dPreference Share saved successfully."
                        : "Description for dPreference share deleted successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving description for dPreference Share.",
                    Error = ex.Message
                });
            }
        }

        //Save(e)Nameofthesharholder
        [HttpPost("Save(e)Nameofthesharholder")]
        public async Task<IActionResult> SaveNameofthesharholder([FromBody] NameofthesharholderDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Description))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Description is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SaveeNameofthesharholderAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.Id == 0
                        ? "Name of the sharholder saved successfully."
                        : "Name of the sharholder  updated successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving Name of the sharholder  .",
                    Error = ex.Message
                });
            }
        }

        //Save(e)PreferenceShares
        [HttpPost("Save(e)PreferenceShares")]
        public async Task<IActionResult> SaveePreferenceShares([FromBody] ePreferenceSharesDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Description))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Description is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SaveePreferenceSharesAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.Id == 0
                        ? "Preference Shares saved successfully."
                        : "Preference Shares updated successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving Preference Shares .",
                    Error = ex.Message
                });
            }
        }

        //Save(f)SharesAllotted
        [HttpPost("Save(f)SharesAllotted")]
        public async Task<IActionResult> SavefSharesAllotted([FromBody] FSahresAllottedDto dto)
        {
            try
            {
                // ✅ Validation (optional, can extend with more rules)
                if (dto.CustomerId == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "CustomerId is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SavefSharesAllottedAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.Id == 0
                        ? "Shares Allotted saved successfully."
                        : "Shares Allotted deleted successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving description for Shares Allotted.",
                    Error = ex.Message
                });
            }
        }

        //SaveEquityShares(Promoter name)
        [HttpPost("SaveEquityShares(Promoter name)")]
        public async Task<IActionResult> SaveEquitySharesPromoterName([FromBody] SaveEquitySharesPromoterNameDto dto)
        {
            try
            {
                // ✅ Validation
                if (dto.CustomerId == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "CustomerId is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SaveEquitySharesPromoterNameAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.Id == 0
                        ? "Equity Shares(FSC) saved successfully."
                        : "Equity Shares(FSC) updated successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving Equity Shares(FSC).",
                    Error = ex.Message
                });
            }
        }

        //SavePreferenceShares(Promoter name)
        [HttpPost("SavePreferenceShares(Promoter name)")]
        public async Task<IActionResult> SavePreferenceSharesPromoterName([FromBody] SavePreferenceSharesPromoterNameDto dto)
        {
            try
            {
                // ✅ Validation
                if (dto.CustomerId == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "CustomerId is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SavePreferenceSharesPromoterNameAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.Id == 0
                        ? "Preference Shares(FSC) saved successfully."
                        : "Preference Shares(FSC) updated successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving Preference Shares(FSC).",
                    Error = ex.Message
                });
            }
        }

        //SaveFootNote
        [HttpPost("SaveFootNote")]
        public async Task<IActionResult> SaveFootNote([FromBody] FootNoteDto dto)
        {
            try
            {
                // ✅ Validation (optional, can extend with more rules)
                if (dto.CustomerId == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "CustomerId is required.",
                        Data = (int?)null
                    });
                }

                var id = await _ScheduleNoteService.SaveFootNoteAsync(dto);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = dto.Id == 0
                        ? "Foot Note saved successfully."
                        : "Foot Note deleted successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while saving description for Foot Note.",
                    Error = ex.Message
                });
            }
        }

        //GetFirstNote
        [HttpGet("GetFirstNote")]
        public async Task<IActionResult> GetFirstNote(int compId, string category, int custId, int YearId)
        {
            try
            {
                var result = await _ScheduleNoteService.GetFirstNoteAsync(compId, category, custId, YearId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = $"No records found in ScheduleNote_First for category '{category}' and companyId '{compId}'.",
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
                    Message = "An error occurred while fetching ScheduleNote_First records.",
                    Error = ex.Message
                });
            }
        }

        //GetSecondNoteById
        [HttpGet("GetSecondNote")]
        public async Task<IActionResult> GetSecondNote(int compId, string category, int custId, int yearId)
        {
            try
            {
                var result = await _ScheduleNoteService.GetSecondNoteByIdAsync(compId, category, custId, yearId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = $"No records found in ScheduleNote_First for category '{category}' and companyId '{compId}'.",
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
                    Message = "An error occurred while fetching ScheduleNote_First records.",
                    Error = ex.Message
                });
            }
        }

        //GetDescriptionNoteById
        [HttpGet("GetDescriptionNoteById")]
        public async Task<IActionResult> GetDescriptionNote([FromQuery] int compId, [FromQuery] string category, [FromQuery] int custId, [FromQuery] int yearId)
        {
            try
            {
                var result = await _ScheduleNoteService.GetDescriptionNoteAsync(compId, category, custId, yearId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = $"No records found in ScheduleNote_Desc for category '{category}' and companyId '{compId}'.",
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
                    Message = "An error occurred while fetching ScheduleNote_Desc records.",
                    Error = ex.Message
                });
            }
        }

        //GetThirdNote
        [HttpGet("GetThirdNote")]
        public async Task<IActionResult> GetThirdNote(int compId, string category, int custId, int YearId)
        {
            try
            {
                var result = await _ScheduleNoteService.GetThirdNoteAsync(compId, category, custId, YearId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = $"No records found in ScheduleNote_Third for category '{category}' and companyId '{compId}'.",
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
                    Message = "An error occurred while fetching ScheduleNote_Third records.",
                    Error = ex.Message
                });
            }
        }

        //GetFourthNote
        [HttpGet("GetFourthNote")]
        public async Task<IActionResult> GetFourthNote(int compId, string category, int custId, int YearId)
        {
            try
            {
                var result = await _ScheduleNoteService.GetFourthNoteAsync(compId, category, custId, YearId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = $"No records found in ScheduleNote_Fourth for category '{category}' and companyId '{compId}'.",
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
                    Message = "An error occurred while fetching ScheduleNote_Fourth records.",
                    Error = ex.Message
                });
            }
        }

        //DeleteFirstNote
        [HttpPost("DeleteSchedFirstNoteDetails")]
        public async Task<IActionResult> DeleteSchedFirstNoteDetails([FromBody] DeleteFirstNoteDto dto)
        {
            try
            {
                if (dto.Id == 0 || dto.CustomerId == 0 || dto.CompId == 0 || dto.YearId == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Id, CustomerId, CompId, and YearId are required.",
                        Data = (object)null
                    });
                }

                var rowsAffected = await _ScheduleNoteService.DeleteSchedFirstNoteDetailsAsync(
                    dto.Id, dto.CustomerId, dto.CompId, dto.YearId);

                if (rowsAffected == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No matching record found to delete.",
                        Data = (object)null
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "ScheduleNote_First record soft-deleted successfully.",
                    Data = rowsAffected
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while deleting ScheduleNote_First record.",
                    Error = ex.Message
                });
            }
        }

        //DeleteThirdNote
        [HttpPost("DeleteSchedThirdNoteDetails")]
        public async Task<IActionResult> DeleteSchedThirdNoteDetails([FromBody] DeleteThirdNoteDto dto)
        {
            try
            {
                if (dto.Id == 0 || dto.CustomerId == 0 || dto.CompId == 0 || dto.YearId == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Id, CustomerId, CompId, and YearId are required.",
                        Data = (object)null
                    });
                }

                var rowsAffected = await _ScheduleNoteService.DeleteSchedThirdNoteDetailsAsync(
                    dto.Id, dto.CustomerId, dto.CompId, dto.YearId);

                if (rowsAffected == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No matching record found to delete in ScheduleNote_Third.",
                        Data = (object)null
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "ScheduleNote_Third record soft-deleted successfully.",
                    Data = rowsAffected
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while deleting ScheduleNote_Third record.",
                    Error = ex.Message
                });
            }
        }

        //DeleteFourthNote
        [HttpPost("DeleteSchedFourthNoteDetails")]
        public async Task<IActionResult> DeleteSchedFourthNoteDetails([FromBody] DeleteFourthNoteDto dto)
        {
            try
            {
                // ✅ Validation
                if (dto.Id == 0 || dto.CustomerId == 0 || dto.CompId == 0 || dto.YearId == 0)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Id, CustomerId, CompId, and YearId are required.",
                        Data = (object)null
                    });
                }

                // ✅ Call service
                var rowsAffected = await _ScheduleNoteService.DeleteSchedFourthNoteDetailsAsync(
                    dto.Id, dto.CustomerId, dto.CompId, dto.YearId);

                if (rowsAffected == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No matching record found to delete in ScheduleNote_Fourth.",
                        Data = (object)null
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "ScheduleNote_Fourth record soft-deleted successfully.",
                    Data = rowsAffected
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while deleting ScheduleNote_Fourth record.",
                    Error = ex.Message
                });
            }
        }

        //DownloadScheduleNoteExcel
        [HttpGet("DownloadableScheduleNoteExcelFile")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public IActionResult DownloadScheduleNoteExcelTemplate()

        {
            var result = _ScheduleNoteService.GetNoteExcelTemplate();

            if (result.FileBytes == null)
                return NotFound("File not found.");

            return File(result.FileBytes, result.ContentType, result.FileName);
        }

        //DownloadScheduleNotePDF
        [HttpGet("DownloadableScheduleNotePDFFile")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public IActionResult DownloadableScheduleNotePDFFile()

        {
            var result = _ScheduleNoteService.GetNotePDFTemplate();

            if (result.FileBytes == null)
                return NotFound("File not found.");

            return File(result.FileBytes, result.ContentType, result.FileName);
        }

        //DownloadScheduleNotePDFTemplate
        [HttpGet("DownloadPdf")]
        public async Task<IActionResult> GenerateScheduleNotePdf(int compId, int custId, string financialYear)
        {
            try
            {
                var pdfBytes = await _ScheduleNoteService.GenerateScheduleNotePdfAsync(compId, custId, financialYear);

                if (pdfBytes == null || pdfBytes.Length == 0)
                    return NotFound("PDF generation failed.");

                // Return PDF file
                return File(pdfBytes, "application/pdf", $"ScheduleNote_{custId}_{financialYear}.pdf");
            }
            catch (Exception ex)
            {
                // Log error if needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

