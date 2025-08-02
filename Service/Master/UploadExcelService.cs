using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using TracePca.Data;
using TracePca.Interface.Master;
using static TracePca.Dto.Masters.UploadExcelDto;

namespace TracePca.Service.Master
{
    public class UploadExcelService : UploadExcelInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly SqlConnection _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UploadExcelService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
        }

        //ValidateClientDetails
        public async Task<IEnumerable<CustomerValidationResult>> ValidateCustomerExcelAsync(IFormFile file)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Setup Excel
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var results = new List<CustomerValidationResult>();
            var customerNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var emails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];

            if (worksheet == null)
                throw new Exception("Excel file does not contain a valid worksheet.");

            var mandatoryFields = new[]
            {
        "Customer Name", "Organisation Type", "Address", "City", "E-Mail", "Mobile No",
        "Business Reltn. Start Date", "Industry Type", "Professional Services Offered 1",
        "Location Name 1", "Contact Person 1", "Address 1"
    };

            var headers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                var header = worksheet.Cells[1, col].Text.Trim().Replace("*", "");
                if (!string.IsNullOrEmpty(header))
                    headers[header] = col;
            }

            var missingColumns = mandatoryFields.Where(field => !headers.ContainsKey(field)).ToList();
            if (missingColumns.Any())
            {
                throw new Exception("Missing mandatory columns: " + string.Join(", ", missingColumns));
            }

            int customerCounter = 1;

            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var result = new CustomerValidationResult
                {
                    RowNumber = row,
                    MissingFields = new List<string>(),
                    Data = new Dictionary<string, string>()
                };

                foreach (var column in mandatoryFields)
                {
                    string value = worksheet.Cells[row, headers[column]].Text.Trim();
                    result.Data[column] = value;

                    if (string.IsNullOrWhiteSpace(value))
                        result.MissingFields.Add(column);

                    if (column == "Customer Name")
                        result.CustomerName = value;

                    if (column == "E-Mail")
                        result.Email = value;
                }

                // Check for duplicates
                string nameKey = result.CustomerName?.ToLowerInvariant() ?? "";
                string emailKey = result.Email?.ToLowerInvariant() ?? "";

                bool isDuplicate = false;
                if (!string.IsNullOrEmpty(nameKey) && !customerNames.Add(nameKey))
                    isDuplicate = true;
                if (!string.IsNullOrEmpty(emailKey) && !emails.Add(emailKey))
                    isDuplicate = true;

                result.IsDuplicate = isDuplicate;

                if (!result.IsDuplicate && !result.MissingFields.Any())
                {
                    result.GeneratedCustomerId = $"CUST{customerCounter:D3}";
                    customerCounter++;
                }

                results.Add(result);
            }
            return results;
        }
    }
}
