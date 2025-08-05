using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.ScheduleMastersDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleMastersController : ControllerBase
    {
        private ScheduleMastersInterface _ScheduleMastersService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public ScheduleMastersController(Trdmyus1Context dbcontext, IConfiguration configuration,ScheduleMastersInterface ScheduleMastersInterface, IHttpContextAccessor httpContextAccessor)
        {
            
            _ScheduleMastersService = ScheduleMastersInterface;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _dbcontext = dbcontext;

        }

        //GetCustomersName
        [HttpGet("GetCustomersName")]
        public async Task<IActionResult> GetCustomerName([FromQuery] int CompId)
        {

            try
            {
                var result = await _ScheduleMastersService.GetCustomerNameAsync(CompId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Customer name found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Customer name loaded successfully.",
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
                var durationId = await _ScheduleMastersService.GetCustomerDurationIdAsync(CompId, CustId);

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

        //GetFinancialYear
        [HttpGet("GetFinancialYear")]
        public async Task<IActionResult> GetFinancialYear([FromQuery] int CompId)
        {
            try
            {
                var result = await _ScheduleMastersService.GetFinancialYearAsync(CompId);

                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "No Financial year found.",
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


        //GetBranchName
        [HttpGet("GetBranchName")]
        public async Task<IActionResult> GetBranchName([FromQuery] int CompId, [FromQuery] int CustId)
        {
            try
            {
                var result = await _ScheduleMastersService.GetBranchNameAsync(CompId, CustId);

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
                var result = await _ScheduleMastersService.GetScheduleHeadingAsync(CompId, CustId, ScheduleTypeId);

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
                var result = await _ScheduleMastersService.GetScheduleItemAsync(CompId, CustId, ScheduleTypeId);

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
                var result = await _ScheduleMastersService.GetScheduleItemAsync(CompId, CustId, ScheduleTypeId);

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
                var result = await _ScheduleMastersService.GetScheduleSubItemAsync(CompId, CustId, ScheduleTypeId);

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

        //GetCustomerOrgType
        [HttpGet("GetCustomerOrgType")]
        public async Task<IActionResult> GetCustomerOrgType([FromQuery] int CustId, [FromQuery] int CompId)
        {
            try
            {
                var result = await _ScheduleMastersService.GetCustomerOrgTypeAsync(CustId, CompId);

                if (string.IsNullOrWhiteSpace(result))
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = "Customer organization type not found.",
                        data = (string)null
                    });
                }

                return Ok(new
                {
                    statusCode = 200,
                    message = "Customer organization type retrieved successfully.",
                    data = new
                    {
                        orgType = result
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while retrieving the organization type.",
                    error = ex.Message
                });
            }
        }


        [HttpPost("save-location")]
        public async Task<IActionResult> SaveCustomerLocation([FromBody] CustomerLocationDto dto)
        {
            if (dto.MasCustID == 0)
                return BadRequest("Customer must be selected");

            string resultMessage;
            string dbName;

            try
            {
                // ✅ Step 1: Get DB name from session
                 dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

                if (string.IsNullOrEmpty(dbName))
                    throw new Exception("CustomerCode is missing in session. Please log in again.");

                // ✅ Step 2: Get the connection string
                var connectionString = _configuration.GetConnectionString(dbName);
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var cmd = new SqlCommand("spSAD_CUST_LOCATION", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add input parameters
                cmd.Parameters.AddWithValue("@Mas_Id", dto.MasId);
                cmd.Parameters.AddWithValue("@Mas_code", dto.MasCode ?? "");
                cmd.Parameters.AddWithValue("@Mas_Description", dto.MasDescription ?? "");
                cmd.Parameters.AddWithValue("@Mas_DelFlag", dto.MasDelFlag ?? "A");
                cmd.Parameters.AddWithValue("@Mas_CustID", dto.MasCustID);
                cmd.Parameters.AddWithValue("@Mas_Loc_Address", dto.MasLocAddress ?? "");
                cmd.Parameters.AddWithValue("@Mas_Contact_Person", dto.MasContactPerson ?? "");
                cmd.Parameters.AddWithValue("@Mas_Contact_MobileNo", dto.MasContactMobileNo ?? "");
                cmd.Parameters.AddWithValue("@Mas_Contact_LandLineNo", dto.MasContactLandLineNo ?? "");
                cmd.Parameters.AddWithValue("@Mas_Contact_Email", dto.MasContactEmail ?? "");
                cmd.Parameters.AddWithValue("@mas_Designation", dto.MasDesignation ?? "");
                cmd.Parameters.AddWithValue("@Mas_CRBY", dto.MasCRBY);
                cmd.Parameters.AddWithValue("@Mas_UpdatedBy", dto.MasUpdatedBy);
                cmd.Parameters.AddWithValue("@Mas_STATUS", dto.MasStatus ?? "A");
                cmd.Parameters.AddWithValue("@Mas_IPAddress", dto.MasIPAddress ?? "");
                cmd.Parameters.AddWithValue("@Mas_CompID", dto.MasCompID);

                // Add output parameters
                var outputSave = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var outputOper = new SqlParameter("@iOper", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputSave);
                cmd.Parameters.Add(outputOper);

                await cmd.ExecuteNonQueryAsync();
                await connection.CloseAsync();

                int resultCode = Convert.ToInt32(outputSave.Value);
                resultMessage = resultCode switch
                {
                    3 => "Successfully Saved",
                    2 => "Successfully Updated",
                    _ => "Unknown Result"
                };

                // Call logic to update location IDs
                await UpdateCustomerLocationIds(dbName, dto.MasCompID, dto.MasCustID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }

            return Ok(new { status = resultMessage });
        }

        private async Task UpdateCustomerLocationIds(string dbName, int compId, int custId)
        {
            var connString = _configuration.GetConnectionString(dbName);
            var locationIds = new StringBuilder();

            using var con = new SqlConnection(connString);

            // Fetch location IDs
            using var getCmd = new SqlCommand("SELECT Mas_Id FROM SAD_CUST_LOCATION WHERE Mas_CustID = @custId AND Mas_CompID = @compId", con);
            getCmd.Parameters.AddWithValue("@custId", custId);
            getCmd.Parameters.AddWithValue("@compId", compId);

            await con.OpenAsync();
            using var reader = await getCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                locationIds.Append(reader["Mas_Id"].ToString()).Append(',');
            }
            await con.CloseAsync();

            string locationIdString = locationIds.ToString().TrimEnd(',');

            // Update master table with CSV of IDs
            using var updateCmd = new SqlCommand("UPDATE SAD_CUSTOMER_MASTER SET Cust_LocationID = @locIds WHERE Cust_ID = @custId", con);
            updateCmd.Parameters.AddWithValue("@locIds", locationIdString);
            updateCmd.Parameters.AddWithValue("@custId", custId);

            await con.OpenAsync();
            await updateCmd.ExecuteNonQueryAsync();
            await con.CloseAsync();
        }


    }
}
