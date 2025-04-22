using Microsoft.AspNetCore.Mvc;
using TracePca.Dto;
using TracePca.Interface;
using TracePca.Models.UserModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetMasterController : ControllerBase
    {
        private AssetInterface _AssetInterface;
        // GET: api/<AssetController>

        public AssetMasterController(AssetInterface AssetInterface)
        {
            _AssetInterface = AssetInterface;

        }


        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers([FromQuery] int companyId)
        {
            var customers = await _AssetInterface.GetCustomersAsync(companyId);
            return Ok(customers);
        }

        [HttpGet("LoadYears")]
        public async Task<IActionResult> LoadYears([FromQuery] int companyId)
        {
            var result = await _AssetInterface.LoadYearsAsync(companyId);
            return Ok(result);
        }


        [HttpGet("GetFixedAssetTypes")]
        public async Task<IActionResult> GetFixedAssetTypes([FromQuery] int companyId, [FromQuery] int customerId)
        {
            var result = await _AssetInterface.LoadFixedAssetTypesAsync(companyId, customerId);

            return Ok(result); // You said: "whatever passed in response" — this returns it directly
        }


        [HttpGet("GenerateTransactionNo")]
        public async Task<IActionResult> GenerateTransactionNo([FromQuery] int compId, [FromQuery] int yearId, [FromQuery] int custId)
        {
            string transactionNo = await _AssetInterface.GenerateTransactionNoAsync(compId, yearId, custId);
            return Ok(new { TransactionNo = transactionNo });
        }

        [HttpGet("units-of-measure")]
        public async Task<IActionResult> GetUnitsOfMeasure(int companyId)
        {
            var result = await _AssetInterface.LoadUnitsOfMeasureAsync(companyId);
            return Ok(result);
        }

        [HttpGet("LoadCurrency")]
        public async Task<IActionResult> LoadCurrency([FromQuery] string sNameSpace, [FromQuery] int iCompID)
        {
            try
            {
                var currencytype = await _AssetInterface.LoadCurrencyAsync(sNameSpace, iCompID);
                return Ok(new
                {
                    statusCode = 200,
                    message = "Currency code generated successfully",
                    data = new
                    {
                        currencytype = currencytype
                    }
                });
            
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpGet("GenerateEmployeeCode")]
        public async Task<IActionResult> GenerateEmployeeCode()
        {
            var employeeCode = await _AssetInterface.GetNextEmployeeCodeAsync();

            return Ok(new
            {
                statusCode = 200,
                message = "Asset code generated successfully",
                data = new
                {
                    employeeCode = employeeCode
                }
            });
        }

        [HttpGet("GetSuppliers")]
        public async Task<IActionResult> GetSuppliers(int companyId, int supplierId = 0)
        {
            var suppliers = await _AssetInterface.LoadExistingSuppliersAsync(companyId, supplierId);
            return Ok(new
            {
                status = 200,
                message = "Suppliers fetched successfully",
                data = suppliers
            });
        }

        [HttpPost("InsertAsset")]
        public async Task<IActionResult> InsertAsset([FromBody] AddAssetDto assetDto)
        {
            

            var assetId = await _AssetInterface.InsertAssetAsync(assetDto);
            return Ok(new { message = "Asset saved successfully", assetId });
        }
        
        [HttpPost("SaveSupplier")]
        public async Task<IActionResult> SaveSupplier([FromBody] AddSupplierDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Supplier data is required.");

                int resultId = await _AssetInterface.SaveSupplierAsync(dto);

                string message = dto.SupId.HasValue && dto.SupId.Value > 0
                    ? "Supplier updated successfully"
                    : "Supplier inserted successfully";

                return Ok(new
                {
                    Message = message,
                    SupplierId = resultId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while saving supplier: {ex.Message}");
            }
        }



        [HttpPost("GetSheetNames")]
        public async Task<IActionResult> GetSheetNames(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var sheetNames = await _AssetInterface.GetSheetNamesAsync(file);
            return Ok(sheetNames);
        }


        [HttpPost("UploadAndProcessExcel")]
        public async Task<IActionResult> UploadAndProcessExcel(IFormFile file, [FromForm] string sheetName, int customerId, int financialYearId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { StatusCode = 400, Message = "No file uploaded." });
            }

            try
            {
                var result = await _AssetInterface.UploadAndProcessExcel(file, sheetName, customerId, financialYearId);

                // Check if the result indicates success or failure
                if (result.Contains("successfully"))
                {
                    return Ok(new { StatusCode = 200, Message = result });
                }
                else
                {
                    return BadRequest(new { StatusCode = 400, Message = result });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"Error processing the file: {ex.Message}" });
            }
        }


        [HttpPost("ValidateExcelFormat")]
        public async Task<IActionResult> ValidateExcelFormat(IFormFile file, string sheetName)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (string.IsNullOrWhiteSpace(sheetName))
            {
                return BadRequest("No sheet name selected.");
            }

            try
            {
                // Validate and process the file with the selected sheet name
                var (validAssets, validationErrors) = await _AssetInterface.ValidateExcelFormatAsync(file, sheetName);

                if (validationErrors.Any())
                {
                    return BadRequest(new { errors = validationErrors });
                }

                return Ok(new { validAssets, message = "File validated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error validating the file: {ex.Message}");
            }
        }





        // GET api/<AssetController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AssetController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AssetController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AssetController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
