using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TracePca.Interface.FIN_Statement;
using TracePca.Service.FIN_statement;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.FIN_Statement
{
    [Route("api/[controller]")]
    [ApiController]
    public class AbnormalitiesController : ControllerBase
    {
        private AbnormalitiesInterface _AbnormalitiesInterface;
        private AbnormalitiesInterface _AbnormalitiesService;
        private readonly IWebHostEnvironment _env;

        public AbnormalitiesController(AbnormalitiesInterface AbnormalitiesInterface, IWebHostEnvironment env)
        {
            _AbnormalitiesInterface = AbnormalitiesInterface;
            _AbnormalitiesService = AbnormalitiesInterface;
            _env = env;
        }

        //GetAbnormalTransactions
        //[HttpGet("abnormal")]
        //public async Task<IActionResult> GetAbnormalTransactions([FromQuery] int iCustId, [FromQuery] int iBranchId, [FromQuery] int iYearID, [FromQuery] int iAbnormalType, [FromQuery] decimal dAmount)
        //{
        //    try
        //    {
        //        var result = await _AbnormalitiesService.GetAbnormalTransactionsAsync(iCustId, iBranchId, iYearID, iAbnormalType, dAmount);

        //        if (result == null || !result.Any())
        //        {
        //            return NotFound(new
        //            {
        //                StatusCode = 404,
        //                Message = "No abnormal transactions found.",
        //                Data = new List<object>()
        //            });
        //        }

        //        return Ok(new
        //        {
        //            StatusCode = 200,
        //            Message = "Abnormal transactions retrieved successfully.",
        //            Data = result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            StatusCode = 500,
        //            Message = "An error occurred while fetching abnormal transactions.",
        //            Error = ex.Message
        //        });
        //    }
        //}

        //Type1
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAbnormalTransactions1(
            [FromQuery] int iCustId,
            [FromQuery] int iBranchId,
            [FromQuery] int iYearID,
            [FromQuery] decimal dAmount)
        {
            try
            {
                var transactions = await _AbnormalitiesService.GetAllAbnormalTransactions1Async(
                    iCustId, iBranchId, iYearID, dAmount);

                return Ok(new
                {
                    statusCode = 200,
                    message = "Abnormal transactions retrieved successfully.",
                    data = transactions
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "An error occurred while fetching abnormal transactions.",
                    error = ex.Message
                });
            }
        }

        //Type2
        [HttpGet("GetAbnormalTransactions")]
        public async Task<IActionResult> GetAbnormalTransactions2(
           int customerId,
           int branchId,
           int yearId,
           decimal amount)
        {
            try
            {
                var abnormalTransactions = await _AbnormalitiesService
                    .GetAbnormalTransactions2Async(customerId, branchId, yearId, amount);

                if (abnormalTransactions == null || !abnormalTransactions.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No abnormal transactions found for the specified filters.",
                        Data = new object[] { }
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Abnormal transactions retrieved successfully.",
                    Data = abnormalTransactions
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching abnormal transactions.",
                    Error = ex.Message
                });
            }
        }

        //Type3
        [HttpGet("GetAbnormalTransactions3")]
        public async Task<IActionResult> GetAbnormalTransactions3(
        int iCustId,
        int iBranchId,
        int iYearID,
        int iAbnormalType,
        decimal dAmount)
        {
            try
            {
                var data = await _AbnormalitiesService.GetAbnormalTransactions3Async(
                    iCustId,
                    iBranchId,
                    iYearID,
                    iAbnormalType,
                    dAmount
                );

                if (data == null || !data.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No abnormal transactions found for the given criteria.",
                        Data = new List<object>()
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Abnormal transactions retrieved successfully.",
                    Data = data
                });
            }
            catch (SqlException sqlEx)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "A database error occurred while fetching abnormal transactions.",
                    Error = sqlEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching abnormal transactions.",
                    Error = ex.Message
                });
            }
        }
    }
}

