// Services/BulkJournalEntryService.cs
using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using JournalEntryUploadAPI.Services;
using JournalEntryUploadAPI.Models.DTOs;
using Dapper;
using TracePca.Interface.FIN_Statement;

namespace JournalEntryUploadAPI.Services
{
    public class BulkJournalEntryService : IBulkJournalEntryService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BulkJournalEntryService> _logger;
        private readonly Dictionary<string, int> _accountCache = new();
        private readonly Dictionary<string, int> _voucherTypeCache = new();

        public BulkJournalEntryService(IConfiguration configuration, ILogger<BulkJournalEntryService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

      














        public async Task<UploadResponseDto> BulkUploadJournalEntriesAsync(IFormFile file, JournalEntryUploadDto request)
        {
            var response = new UploadResponseDto
            {
                TransactionId = Guid.NewGuid().ToString(),
                StartTime = DateTime.UtcNow
            };

            var stopwatch = Stopwatch.StartNew();

            try
                {
                _logger.LogInformation($"Bulk upload started: {file.FileName}, Size: {file.Length}");

                // Read Excel file
                var journalEntries = await ReadExcelFileAsync(file);
                response.TotalRecords = journalEntries.Count;

                // Create DataTable for TVP
                var dataTable = CreateDataTable(journalEntries);

                // Call stored procedure
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("usp_BulkUploadJournalEntriesTVP", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 300; // 5 minutes timeout

                        // Add parameters
                        command.Parameters.AddWithValue("@CustomerId", request.CustomerId);
                        command.Parameters.AddWithValue("@FinancialYearId", request.FinancialYearId);
                        command.Parameters.AddWithValue("@BranchId", request.BranchId);
                        command.Parameters.AddWithValue("@DurationId", request.DurationId);
                        command.Parameters.AddWithValue("@AccessCodeId", request.AccessCodeId);
                        command.Parameters.AddWithValue("@UserId", request.UserId);
                        command.Parameters.AddWithValue("@IpAddress", request.IpAddress);

                        // Add TVP parameter
                        var tvpParam = command.Parameters.AddWithValue("@JournalEntries", dataTable);
                        tvpParam.SqlDbType = SqlDbType.Structured;
                        tvpParam.TypeName = "dbo.JournalEntryType";

                        // Add output parameters
                        var processedParam = new SqlParameter("@ProcessedRecords", SqlDbType.Int);
                        processedParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(processedParam);

                        var failedParam = new SqlParameter("@FailedRecords", SqlDbType.Int);
                        failedParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(failedParam);

                        var errorParam = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, -1);
                        errorParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(errorParam);

                        // Execute stored procedure
                        await command.ExecuteNonQueryAsync();

                        // Get results
                        response.ProcessedRecords = (int)processedParam.Value;
                        response.FailedRecords = (int)failedParam.Value;
                        response.Success = response.FailedRecords == 0;

                        if (errorParam.Value != DBNull.Value)
                        {
                            response.Errors = ((string)errorParam.Value)?.Split(Environment.NewLine).ToList() ?? new List<string>();
                        }

                        stopwatch.Stop();
                        response.ProcessingTimeInSeconds = stopwatch.Elapsed.TotalSeconds;
                        response.RecordsPerSecond = response.ProcessedRecords / stopwatch.Elapsed.TotalSeconds;
                        response.EndTime = DateTime.UtcNow;
                        response.Message = response.Success ?
                            $"Successfully processed {response.ProcessedRecords} records in {response.ProcessingTimeInSeconds:F2} seconds" :
                            $"Processed {response.ProcessedRecords} records, {response.FailedRecords} failed in {response.ProcessingTimeInSeconds:F2} seconds";
                    }
                }

                _logger.LogInformation($"Bulk upload completed: {response.ProcessedRecords} records in {response.ProcessingTimeInSeconds:F2}s");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk upload failed");
                response.Success = false;
                response.Errors.Add($"Upload failed: {ex.Message}");
                return response;
            }
        }

        private async Task<List<JournalEntryRecordDto>> ReadExcelFileAsync(IFormFile file)
        {
            var entries = new List<JournalEntryRecordDto>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var totalRows = worksheet.Dimension.Rows;

                    for (int row = 2; row <= totalRows; row++) // Skip header
                    {
                        var entry = new JournalEntryRecordDto
                        {
                            RowIndex = row,

                            // ❌ Excel has no column 0 → FIXED
                            Date = GetCellValue(worksheet, row, 1),

                            // Particulars / Party Name
                            Name = GetCellValue(worksheet, row, 2),

                            // Voucher Type
                            Type = GetCellValue(worksheet, row, 4),

                            // Voucher Number
                            Number = GetCellValue(worksheet, row, 5),

                            // Optional fields (safe mapping)
                            SrNo = row.ToString(),
                            TransactionNumber = GetCellValue(worksheet, row, 5),
                            Adjustment = GetCellValue(worksheet, row, 3),

                            // Narration
                            Memo = GetCellValue(worksheet, row, 8),

                            // Account (may be empty on main row)
                            Account = GetCellValue(worksheet, row, 2),

                            // Amounts (non-nullable decimal → default 0)
                            Debit = ParseDecimal(GetCellValue(worksheet, row, 6)),
                            Credit = ParseDecimal(GetCellValue(worksheet, row, 7))
                        };

                        entries.Add(entry);
                    }
                }
            }

            return entries;
        }

        private DataTable CreateDataTable(List<JournalEntryRecordDto> entries)
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add("SrNo", typeof(string));
            dataTable.Columns.Add("Trans", typeof(string));
            dataTable.Columns.Add("Type", typeof(string));
            dataTable.Columns.Add("Date", typeof(string));
            dataTable.Columns.Add("Num", typeof(string));
            dataTable.Columns.Add("Adj", typeof(string));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Memo", typeof(string));
            dataTable.Columns.Add("Account", typeof(string));
            dataTable.Columns.Add("Debit", typeof(decimal));
            dataTable.Columns.Add("Credit", typeof(decimal));

            foreach (var entry in entries)
            {
                try
                {
                    var row = dataTable.NewRow();

                    row["SrNo"] = entry.SrNo ?? string.Empty;
                    row["Trans"] = entry.TransactionNumber ?? string.Empty;
                    row["Type"] = entry.Type ?? string.Empty;
                    row["Date"] = entry.Date ?? string.Empty;
                    row["Num"] = entry.Number ?? string.Empty;
                    row["Adj"] = entry.Adjustment ?? string.Empty;
                    row["Name"] = entry.Name ?? string.Empty;
                    row["Memo"] = entry.Memo ?? string.Empty;
                    row["Account"] = entry.Account ?? string.Empty;

                    // ✅ ALWAYS decimal (never DBNull)
                    row["Debit"] = entry.Debit;
                    row["Credit"] = entry.Credit;

                    if (!IsRowEmpty(row))
                    {
                        dataTable.Rows.Add(row);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding entry {entry.SrNo}: {ex.Message}");
                }
            }

            return dataTable;
        }


        private bool IsRowEmpty(DataRow row)
        {
            return string.IsNullOrWhiteSpace(row["Date"]?.ToString()) &&
                   string.IsNullOrWhiteSpace(row["Name"]?.ToString()) &&
                   string.IsNullOrWhiteSpace(row["Account"]?.ToString()) &&
                   (decimal)row["Debit"] == 0m &&
                   (decimal)row["Credit"] == 0m;
        }


        public async Task<ValidationResultDto> ValidateJournalEntriesAsync(IFormFile file, JournalEntryUploadDto request)
        {
            var result = new ValidationResultDto();

            try
            {
                // Read Excel file
                var journalEntries = await ReadExcelFileAsync(file);
                result.TotalRecords = journalEntries.Count;

                // Create DataTable for TVP
                var dataTable = CreateDataTable(journalEntries);

                // Call validation stored procedure
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("usp_ValidateJournalEntries", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add TVP parameter
                        var tvpParam = command.Parameters.AddWithValue("@JournalEntries", dataTable);
                        tvpParam.SqlDbType = SqlDbType.Structured;
                        tvpParam.TypeName = "dbo.JournalEntryType";

                        command.Parameters.AddWithValue("@AccessCodeId", request.AccessCodeId);

                        var validationResultParam = new SqlParameter("@ValidationResult", SqlDbType.Xml);
                        validationResultParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(validationResultParam);

                        await command.ExecuteNonQueryAsync();

                        // Parse XML result
                        if (validationResultParam.Value != DBNull.Value)
                        {
                            var xmlResult = validationResultParam.Value.ToString();
                            // Parse XML and populate ValidationResultDto
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Validation failed");
            }

            return result;
        }

        public async Task<UploadHistoryDto> GetUploadHistoryAsync(int customerId, DateTime? startDate, DateTime? endDate, int pageNumber = 1, int pageSize = 50)
        {
            var history = new UploadHistoryDto();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("usp_GetUploadHistory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CustomerId", customerId);
                    command.Parameters.AddWithValue("@StartDate", startDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EndDate", endDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PageNumber", pageNumber);
                    command.Parameters.AddWithValue("@PageSize", pageSize);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            history.Uploads.Add(new UploadHistoryItemDto
                            {
                                UploadId = reader.GetInt32(0),
                                CustomerId = reader.GetInt32(1),
                                FinancialYearId = reader.GetInt32(2),
                                TotalRecords = reader.GetInt32(3),
                                ProcessedRecords = reader.GetInt32(4),
                                FailedRecords = reader.GetInt32(5),
                                DurationMs = reader.GetInt32(6),
                                CreatedDate = reader.GetDateTime(7),
                                Status = reader.GetString(8),
                                ErrorMessage = reader.IsDBNull(9) ? null : reader.GetString(9)
                            });
                        }

                        if (await reader.NextResultAsync() && await reader.ReadAsync())
                        {
                            history.TotalCount = reader.GetInt32(0);
                        }
                    }
                }
            }

            return history;
        }

        private string GetCellValue(ExcelWorksheet worksheet, int row, int col)
        {
            return worksheet.Cells[row, col].Text.Trim();
        }


        private decimal ParseDecimal(string value)
        {
            decimal.TryParse(value, out var result);
            return result;
        }


    }
}