using Microsoft.AspNetCore.Mvc;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.ScheduleFormatDto;
using static TracePca.Dto.FIN_Statement.ScheduleNoteDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleNoteController : ControllerBase
    {
        private ScheduleNoteInterface _ScheduleNoteInterface;
        private ScheduleNoteInterface _ScheduleNoteService;

        public ScheduleNoteController(ScheduleNoteInterface ScheduleNoteInterface)
        {
            _ScheduleNoteInterface = ScheduleNoteInterface;
            _ScheduleNoteService = ScheduleNoteInterface;
        }

        //GetSubHeadingname(Notes For SubHeading)
        [HttpGet("SubHeading-NotesForSubHeading")]
        public async Task<IActionResult> GetSubHeadingNotes(int CustomerId, int SubHeadingId)
        {
            try
            {
                var notes = await _ScheduleNoteService.GetSubHeadingDetailsAsync(CustomerId, SubHeadingId);

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
        [HttpPost("SaveOrUpdateScheduleFormatHeading")]
        public async Task<IActionResult> aveSubHeadindNotes([FromBody] SubHeadingNotesDto dto)
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
                bool isUpdate = dto.ASHN_ID > 0;

                var result = await _ScheduleNoteService.SaveSubHeadindNotesAsync(dto);

                string successMessage = isUpdate
                    ? "Schedule Heading successfully updated."
                    : "Schedule Heading successfully created.";

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

    }
}
