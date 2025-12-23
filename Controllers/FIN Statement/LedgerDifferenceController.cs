using Microsoft.AspNetCore.Mvc;
using System.Data;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.LedgerDifferenceDto;
using static TracePca.Dto.FIN_Statement.SelectedPartiesDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class LedgerDifferenceController : ControllerBase
    {
        private ILedgerDifferenceInterface _LedgerDifferenceService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public LedgerDifferenceController(ILedgerDifferenceInterface LedgerDifferenceInterface, Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _LedgerDifferenceService = LedgerDifferenceInterface;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _dbcontext = dbcontext;
        }

        //GetDescriptionWiseDetails
        [HttpGet("GetDescriptionWiseDetails")]
        public async Task<IActionResult> GetDescriptionWiseDetails(int compId, int custId, int branchId, int yearId, int typeId)
        {
            try
            {
                var result = await _LedgerDifferenceService.GetDescriptionWiseDetailsAsync(compId, custId, branchId, yearId, typeId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No records found."
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Data fetched successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching description-wise details.",
                    Error = ex.Message
                });
            }
        }

        //UpdateDescriptionWiseDetailsStatus
        [HttpPost("UpdateDescriptionWiseDetailsStatus")]
        public async Task<IActionResult> UpdateTrailBalanceStatusBulk([FromBody] List<UpdateDescriptionWiseDetailsStatusDto> dtoList)
        {
            if (dtoList == null || dtoList.Count == 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "No records provided for update."
                });
            }

            try
            {
                var updatedCount = await _LedgerDifferenceService.UpdateTrailBalanceStatusAsync(dtoList);

                if (updatedCount == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No Trail Balance records found to update."
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = $"{updatedCount} Trail Balance record(s) updated successfully.",
                    UpdatedCount = updatedCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while updating Trail Balance statuses.",
                    Error = ex.Message
                });
            }
        }

        //GetDescriptionDetails
        [HttpGet("GetDescriptionDetails")]
        public async Task<IActionResult> GetAccountDetails(int compId, int custId, int branchId, int yearId, int typeId, int pkId)
        {
            try
            {
                var result = await _LedgerDifferenceService.GetAccountDetailsAsync(compId, custId, branchId, yearId, typeId, pkId);

                // If no data found return empty list rather than NotFound()
                if (result == null || !result.Any())
                {
                    return Ok(new
                    {
                        StatusCode = 404,
                        Message = "No records found.",
                        Data = new List<object>() // empty array
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Data fetched successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching description-by-accountwise details.",
                    Error = ex.Message
                });
            }
        }

        //GetCustomerTBGrid
        //[HttpGet("GetCustomerTBGrid")]
        //    public async Task<IActionResult> GetCustCOAMasterDetailsCustomer(
        //[FromQuery] int compId,[FromQuery] int custId,[FromQuery] int yearId,[FromQuery] int scheduleTypeId,[FromQuery] int unmapped,[FromQuery] int branchId)
        //    {
        //        try
        //        {
        //            var result = await _LedgerDifferenceService
        //                .GetCustCOAMasterDetailsCustomerAsync(
        //                    compId, custId, yearId, scheduleTypeId, unmapped, branchId
        //                );

        //            if (result == null)
        //            {
        //                return Ok(new
        //                {
        //                    statusCode = 404,
        //                    message = "Customer Record not found.",
        //                });
        //            }

        //            return Ok(new
        //            {
        //                statusCode = 200,
        //                message = "Customer COA details loaded successfully.",
        //                data = result
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            return StatusCode(StatusCodes.Status500InternalServerError, new
        //            {
        //                statusCode = 500,
        //                message = ex.Message
        //            });
        //        }
        //    }
        //[HttpGet("GetCustomerTrailBalance")]
        //public async Task<IActionResult> GetCustomerTBGrid([FromQuery] int CompId,[FromQuery] int custId,[FromQuery] int yearId,[FromQuery] int branchId)
        //{
        //    try
        //    {
        //        var data = await _LedgerDifferenceService.GetCustomerTBGridAsync(CompId, custId, yearId, branchId);
        //        if (data == null || !data.Any())
        //        {
        //            return NotFound(new
        //            {
        //                statusCode = 404,
        //                message = "Customer record not found.",
        //                data = new List<object>()
        //            });
        //        }
        //        return Ok(new
        //        {
        //            statusCode = 200,
        //            message = "Customer Trail Balance loaded successfully",
        //            data
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, new
        //        {
        //            statusCode = 500,
        //            message = ex.Message
        //        });
        //    }
        //}
        [HttpPost("get-cust-coa")]
        public async Task<IActionResult> GetCustCoa([FromBody] CustCoaRequestDto request)
        {
            if (request == null)
                return BadRequest(new { success = false, message = "Invalid request" });

            try
            {
                var ds = await _LedgerDifferenceService
                    .GetCustCOAMasterDetailsCustomerAsync(request);

                if (ds == null || ds.Tables.Count == 0)
                    return NotFound(new { success = false, message = "No data found" });

                // ✅ Convert DataSet → serializable object
                var result = ds.Tables.Cast<DataTable>()
                    .Select(t => t.AsEnumerable()
                        .Select(r => t.Columns.Cast<DataColumn>()
                            .ToDictionary(c => c.ColumnName, c => r[c])))
                    .ToList();

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error"
                });
            }
        }



        //UpdateCustomerTBDelFlg
        [HttpPost("UpdateCustomerTBDelFlg")]
        public async Task<IActionResult> UpdateCustomerTrailBalanceStatus([FromBody] List<UpdateCustomerTrailBalanceStatusDto> dtoList)
        {
            if (dtoList == null || dtoList.Count == 0)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "No records provided for update."
                });
            }

            try
            {
                var updatedCount = await _LedgerDifferenceService.UpdateCustomerTrailBalanceStatusAsync(dtoList);

                if (updatedCount == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No Customer Trail Balance records found to update."
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = $"{updatedCount} Customer Trail Balance record(s) updated successfully.",
                    UpdatedCount = updatedCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while updating Customer Trail Balance statuses.",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("getFlaggedCustTrailBal")]
        public async Task<IActionResult> GetCustCoa([FromBody] CustCoaRequestFlaggedDto request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            var ds = await _LedgerDifferenceService.GetCustCoaAsync(request);

            var systemData = ToDictionaryList(ds.Tables["SystemData"]);
            var customerData = ToDictionaryList(ds.Tables["CustomerUpload"]);

            var mergedData = MergeByDescription(systemData, customerData);

            return Ok(new
            {
                success = true,
                data = mergedData
            });
        }
        private static List<Dictionary<string, object>> ToDictionaryList(DataTable table)
        {
            return table.AsEnumerable()
                .Select(row =>
                    table.Columns.Cast<DataColumn>()
                        .ToDictionary(col => col.ColumnName, col => row[col])
                ).ToList();
        }

        private static List<Dictionary<string, object>> MergeByDescription(
     List<Dictionary<string, object>> systemData,
     List<Dictionary<string, object>> customerData)
        {
            var result = new List<Dictionary<string, object>>();

            // 🔹 System lookup (secondary)
            var systemLookup = systemData.ToDictionary(
                x => x["DescriptionCode"]?.ToString() ?? string.Empty,
                x => x
            );

            // 🔹 Customer is PRIMARY
            foreach (var cust in customerData)
            {
                var desc = cust["DescriptionCode"]?.ToString() ?? string.Empty;

                // start with customer row
                var merged = new Dictionary<string, object>(cust);

                // merge system values if exists
                if (systemLookup.TryGetValue(desc, out var sys))
                {
                    foreach (var kv in sys)
                    {
                        if (!merged.ContainsKey(kv.Key))
                            merged[kv.Key] = kv.Value;
                    }
                }

                result.Add(merged);
            }

            return result;
        }


    }
}
