using System.Data;
using Dapper;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Commons.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleNoteDto;

namespace TracePca.Service.FIN_statement
{
    public class ScheduleNoteService : ScheduleNoteInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ScheduleNoteService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
        }

        //GetSubHeadingname(Notes For SubHeading)
        public async Task<IEnumerable<SubHeadingNoteDto>> GetSubHeadingDetailsAsync(int CustomerId, int SubHeadingId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            var query = @"
SELECT 
    ASHN_Description AS Description,
    ASHN_ID 
FROM ACC_SubHeadingNoteDesc 
LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = ASHN_SubHeadingId 
WHERE ASHN_CustomerId = @CustomerId 
  AND ASHN_SubHeadingId = @SubHeadingId";

            return await connection.QueryAsync<SubHeadingNoteDto>(query, new
            {
                CustomerId = CustomerId,
                SubHeadingId = SubHeadingId
            });
        }

        //SaveOrUpdateSubHeadingNotes(Notes For SubHeading)
        public async Task<int[]> SaveSubHeadindNotesAsync(SubHeadingNotesDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand("spACC_SubHeadingNoteDesc", connection, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@ASHN_ID", dto.ASHN_ID);
                            cmd.Parameters.AddWithValue("@ASHN_SubHeadingId", dto.ASHN_SubHeadingId);
                            cmd.Parameters.AddWithValue("@ASHN_CustomerId", dto.ASHN_CustomerId);
                            cmd.Parameters.AddWithValue("@ASHN_Description", dto.ASHN_Description ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASHN_DelFlag", dto.ASHN_DelFlag ?? "A");
                            cmd.Parameters.AddWithValue("@ASHN_Status", dto.ASHN_Status ?? "W");
                            cmd.Parameters.AddWithValue("@ASHN_Operation", dto.ASHN_Operation ?? "S");
                            cmd.Parameters.AddWithValue("@ASHN_CreatedBy", dto.ASHN_CreatedBy);
                            cmd.Parameters.AddWithValue("@ASHN_CreatedOn", dto.ASHN_CreatedOn);
                            cmd.Parameters.AddWithValue("@ASHN_CompID", dto.ASHN_CompID);
                            cmd.Parameters.AddWithValue("@ASHN_YearID", dto.ASHN_YearID);
                            cmd.Parameters.AddWithValue("@ASHN_IPAddress", dto.ASHN_IPAddress ?? string.Empty);

                            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };
                            var operParam = new SqlParameter("@iOper", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };

                            cmd.Parameters.Add(updateOrSaveParam);
                            cmd.Parameters.Add(operParam);

                            await cmd.ExecuteNonQueryAsync();

                            transaction.Commit();

                            return new int[]
                            {
                        (int)(updateOrSaveParam.Value ?? 0),
                        (int)(operParam.Value ?? 0)
                            };
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        //GetBranch(Notes For Ledger)
        public async Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(int CompId, int CustId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            var query = @"
        SELECT 
            Mas_Id AS Branchid, 
            Mas_Description AS BranchName 
        FROM SAD_CUST_LOCATION 
        WHERE Mas_CompID = @compId AND Mas_CustID = @custId";

            return await connection.QueryAsync<CustBranchDto>(query, new { CompId, CustId });
        }

        //GetLedger(Notes For Ledger)
        public async Task<IEnumerable<LedgerIndividualDto>> GetLedgerIndividualDetailsAsync(int CustomerId, int SubHeadingId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            var query = @"
SELECT 
    ASHL_Description AS Description, 
    ASHL_ID 
FROM ACC_SubHeadingLedgerDesc 
LEFT JOIN Acc_TrailBalance_Upload_Details a ON a.ATBUD_ID = ASHL_SubHeadingId 
WHERE ASHL_CustomerId = @CustomerId 
  AND ASHL_SubHeadingId = @SubHeadingId";
            return await connection.QueryAsync<LedgerIndividualDto>(query, new
            {
                CustomerId = CustomerId,
                SubHeadingId = SubHeadingId
            });
        }

        //SaveOrUpdateLedger(Notes For Ledger)
        public async Task<int[]> SaveLedgerDetailsAsync(SubHeadingLedgerNoteDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand("spACC_SubHeadingLedgerDesc", connection, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@ASHL_ID", dto.ASHL_ID);
                            cmd.Parameters.AddWithValue("@ASHL_SubHeadingId", dto.ASHL_SubHeadingId);
                            cmd.Parameters.AddWithValue("@ASHL_CustomerId", dto.ASHL_CustomerId);
                            cmd.Parameters.AddWithValue("@ASHL_BranchId", dto.ASHL_BranchId);
                            cmd.Parameters.AddWithValue("@ASHL_Description", dto.ASHL_Description ?? string.Empty);
                            cmd.Parameters.AddWithValue("@ASHL_DelFlag", dto.ASHL_DelFlag ?? "A");
                            cmd.Parameters.AddWithValue("@ASHL_Status", dto.ASHL_Status ?? "W");
                            cmd.Parameters.AddWithValue("@ASHL_Operation", dto.ASHL_Operation ?? "S");
                            cmd.Parameters.AddWithValue("@ASHL_CreatedBy", dto.ASHL_CreatedBy);
                            cmd.Parameters.AddWithValue("@ASHL_CreatedOn", dto.ASHL_CreatedOn);
                            cmd.Parameters.AddWithValue("@ASHL_CompID", dto.ASHL_CompID);
                            cmd.Parameters.AddWithValue("@ASHL_YearID", dto.ASHL_YearID);
                            cmd.Parameters.AddWithValue("@ASHL_IPAddress", dto.ASHL_IPAddress ?? string.Empty);

                            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };
                            var operParam = new SqlParameter("@iOper", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };

                            cmd.Parameters.Add(updateOrSaveParam);
                            cmd.Parameters.Add(operParam);

                            await cmd.ExecuteNonQueryAsync();
                            transaction.Commit();

                            return new int[]
                            {
                        (int)(updateOrSaveParam.Value ?? 0),
                        (int)(operParam.Value ?? 0)
                            };
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        //DownloadNotesExcel
        public ExcelFileDownloadResult GetExcelTemplate()
        {
            var filePath = "C:\\Users\\SSD\\Desktop\\TracePa\\tracepa-dotnet-core\\SampleExcels\\ScheduleNote Cust1-2024-2025.xls";

            if (!File.Exists(filePath))
                return new ExcelFileDownloadResult();

            var bytes = File.ReadAllBytes(filePath);
            var fileName = "ScheduleNote Cust1-2024-2025.xls";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return new ExcelFileDownloadResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        //DownloadNotesPdf
        public PdfFileDownloadResult GetPdfTemplate()
        {
            var filePath = "C:\\Users\\SSD\\Desktop\\TracePa\\tracepa-dotnet-core\\SamplePdfs\\ScheduleNote Cust1-2024-2025.pdf";

            if (!File.Exists(filePath))
                return new PdfFileDownloadResult();

            var bytes = File.ReadAllBytes(filePath);
            var fileName = "ScheduleNoteTemplate.pdf";
            var contentType = "application/pdf";

            return new PdfFileDownloadResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        //SaveOrUpdate
        public async Task<int> SaveFirstScheduleNoteDetailsAsync(FirstScheduleNoteDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (dto.SNF_ID != 0)
            {
                const string updateQuery = @"
UPDATE ScheduleNote_First 
SET 
    SNF_Description = @SNF_Description,
    SNF_CYear_Amount = @SNF_CYear_Amount,
    SNF_PYear_Amount = @SNF_PYear_Amount
WHERE 
    SNF_ID = @SNF_ID AND 
    SNF_Category = @SNF_Category AND 
    SNF_CustId = @SNF_CustId AND 
    SNF_YEARId = @SNF_YEARId";

                await connection.ExecuteAsync(updateQuery, dto);
                return dto.SNF_ID;
            }
            else
            {
                const string maxIdQuery = "SELECT ISNULL(MAX(SNF_ID) + 1, 1) FROM ScheduleNote_First";
                var newId = await connection.ExecuteScalarAsync<int>(maxIdQuery);

                const string insertQuery = @"
INSERT INTO ScheduleNote_First (
    SNF_ID, SNF_CustId, SNF_Description, SNF_Category,
    SNF_CYear_Amount, SNF_PYear_Amount, SNF_YEARId, SNF_CompId,
    SNF_STATUS, SNF_DELFLAG, SNF_CRON, SNF_CRBY, SNF_IPAddress
) VALUES (
    @SNF_ID, @SNF_CustId, @SNF_Description, @SNF_Category,
    @SNF_CYear_Amount, @SNF_PYear_Amount, @SNF_YEARId, @SNF_CompId,
    @SNF_STATUS, @SNF_DELFLAG, @SNF_CRON, @SNF_CRBY, @SNF_IPAddress
)";

                dto.SNF_ID = newId;
                await connection.ExecuteAsync(insertQuery, dto);
                return newId;
            }
        }


        // --PreDefinied Notes //
        //SaveAuthorisedShareCapital(Particulars)
        public async Task<int> SaveAuthorisedShareCapitalAsync(AuthorisedShareCapitalDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (dto.Id != 0)
            {
                // ✅ Update existing record
                var updateQuery = @"
                UPDATE ScheduleNote_First 
                SET SNF_Description = @Description,
                    SNF_CYear_Amount = @CurrentYearAmount,
                    SNF_PYear_Amount = @PreviousYearAmount,
                    SNF_CRBY = @UserId,
                    SNF_CRON = GETDATE()
                WHERE SNF_ID = @Id 
                  AND SNF_Category = 'AU'
                  AND SNF_CustId = @CustomerId 
                  AND SNF_YearId = @FinancialYearId";

                await connection.ExecuteAsync(updateQuery, dto);
                return dto.Id;
            }
            else
            {
                // ✅ Insert new record
                var insertQuery = @"
                DECLARE @NextId INT;
                SELECT @NextId = ISNULL(MAX(SNF_ID), 0) + 1 FROM ScheduleNote_First;

                INSERT INTO ScheduleNote_First
                (SNF_ID, SNF_CustId, SNF_Description, SNF_Category, SNF_CYear_Amount, SNF_PYear_Amount, 
                 SNF_YearId, SNF_CompId, SNF_Status, SNF_DelFlag, SNF_CRON, SNF_CRBY, SNF_IPAddress)
                VALUES
                (@NextId, @CustomerId, @Description, 'AU', @CurrentYearAmount, @PreviousYearAmount,
                 @FinancialYearId, @CompanyId, 'W', 'X', GETDATE(), @UserId, @IpAddress);

                SELECT @NextId;";


                var newId = await connection.ExecuteScalarAsync<int>(insertQuery, dto);
                return newId;
            }
        }

        //SaveIssuedSubscribedandFullyPaidupShareCapital
        public async Task<int> SaveIssuedSubscribedandFullyPaidupShareCapitalAsync(IssuedSubscribedandFullyPaidupShareCapitalAsyncDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (dto.Id != 0)
            {
                // ✅ Update existing record
                var updateQuery = @"
                UPDATE ScheduleNote_First 
                SET SNF_Description = @Description,
                    SNF_CYear_Amount = @CurrentYearAmount,
                    SNF_PYear_Amount = @PreviousYearAmount,
                    SNF_CRBY = @UserId,
                    SNF_CRON = GETDATE()
                WHERE SNF_ID = @Id 
                  AND SNF_Category = 'IS'
                  AND SNF_CustId = @CustomerId 
                  AND SNF_YearId = @FinancialYearId";

                await connection.ExecuteAsync(updateQuery, dto);
                return dto.Id;
            }
            else
            {
                // ✅ Insert new record
                var insertQuery = @"
                DECLARE @NextId INT;
                SELECT @NextId = ISNULL(MAX(SNF_ID), 0) + 1 FROM ScheduleNote_First;

                INSERT INTO ScheduleNote_First
                (SNF_ID, SNF_CustId, SNF_Description, SNF_Category, SNF_CYear_Amount, SNF_PYear_Amount, 
                 SNF_YearId, SNF_CompId, SNF_Status, SNF_DelFlag, SNF_CRON, SNF_CRBY, SNF_IPAddress)
                VALUES
                (@NextId, @CustomerId, @Description, 'IS', @CurrentYearAmount, @PreviousYearAmount,
                 @FinancialYearId, @CompanyId, 'W', 'X', GETDATE(), @UserId, @IpAddress);

                SELECT @NextId;";


                var newId = await connection.ExecuteScalarAsync<int>(insertQuery, dto);
                return newId;
            }
        }

        //Save(A)Issued
        public async Task<int> SaveIssuedAsync(IssuedDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (dto.Id != 0)
            {
                // ✅ Update existing record
                var updateQuery = @"
                UPDATE ScheduleNote_First 
                SET SNF_Description = @Description,
                    SNF_CYear_Amount = @CurrentYearAmount,
                    SNF_PYear_Amount = @PreviousYearAmount,
                    SNF_CRBY = @UserId,
                    SNF_CRON = GETDATE()
                WHERE SNF_ID = @Id 
                  AND SNF_Category = 'AI'
                  AND SNF_CustId = @CustomerId 
                  AND SNF_YearId = @FinancialYearId";

                await connection.ExecuteAsync(updateQuery, dto);
                return dto.Id;
            }
            else
            {
                // ✅ Insert new record
                var insertQuery = @"
                DECLARE @NextId INT;
                SELECT @NextId = ISNULL(MAX(SNF_ID), 0) + 1 FROM ScheduleNote_First;

                INSERT INTO ScheduleNote_First
                (SNF_ID, SNF_CustId, SNF_Description, SNF_Category, SNF_CYear_Amount, SNF_PYear_Amount, 
                 SNF_YearId, SNF_CompId, SNF_Status, SNF_DelFlag, SNF_CRON, SNF_CRBY, SNF_IPAddress)
                VALUES
                (@NextId, @CustomerId, @Description, 'AI', @CurrentYearAmount, @PreviousYearAmount,
                 @FinancialYearId, @CompanyId, 'W', 'X', GETDATE(), @UserId, @IpAddress);

                SELECT @NextId;";


                var newId = await connection.ExecuteScalarAsync<int>(insertQuery, dto);
                return newId;
            }
        }

        //Save(B)SubscribedandPaid-up
        public async Task<int> SaveSubscribedandPaidupAsync(SubscribedandPaidupDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (dto.Id != 0)
            {
                // ✅ Update existing record
                var updateQuery = @"
                UPDATE ScheduleNote_First 
                SET SNF_Description = @Description,
                    SNF_CYear_Amount = @CurrentYearAmount,
                    SNF_PYear_Amount = @PreviousYearAmount,
                    SNF_CRBY = @UserId,
                    SNF_CRON = GETDATE()
                WHERE SNF_ID = @Id 
                  AND SNF_Category = 'BS'
                  AND SNF_CustId = @CustomerId 
                  AND SNF_YearId = @FinancialYearId";

                await connection.ExecuteAsync(updateQuery, dto);
                return dto.Id;
            }
            else
            {
                // ✅ Insert new record
                var insertQuery = @"
                DECLARE @NextId INT;
                SELECT @NextId = ISNULL(MAX(SNF_ID), 0) + 1 FROM ScheduleNote_First;

                INSERT INTO ScheduleNote_First
                (SNF_ID, SNF_CustId, SNF_Description, SNF_Category, SNF_CYear_Amount, SNF_PYear_Amount, 
                 SNF_YearId, SNF_CompId, SNF_Status, SNF_DelFlag, SNF_CRON, SNF_CRBY, SNF_IPAddress)
                VALUES
                (@NextId, @CustomerId, @Description, 'BS', @CurrentYearAmount, @PreviousYearAmount,
                 @FinancialYearId, @CompanyId, 'W', 'X', GETDATE(), @UserId, @IpAddress);

                SELECT @NextId;";


                var newId = await connection.ExecuteScalarAsync<int>(insertQuery, dto);
                return newId;
            }
        }

        //SaveCallsUnpaid
        public async Task<int> SaveCallsUnpaidAsync(CallsUnpaidDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (dto.Id != 0)
            {
                // ✅ Update existing record
                var updateQuery = @"
                UPDATE ScheduleNote_First 
                SET SNF_Description = @Description,
                    SNF_CYear_Amount = @CurrentYearAmount,
                    SNF_PYear_Amount = @PreviousYearAmount,
                    SNF_CRBY = @UserId,
                    SNF_CRON = GETDATE()
                WHERE SNF_ID = @Id 
                  AND SNF_Category = 'CC'
                  AND SNF_CustId = @CustomerId 
                  AND SNF_YearId = @FinancialYearId";

                await connection.ExecuteAsync(updateQuery, dto);
                return dto.Id;
            }
            else
            {
                // ✅ Insert new record
                var insertQuery = @"
                DECLARE @NextId INT;
                SELECT @NextId = ISNULL(MAX(SNF_ID), 0) + 1 FROM ScheduleNote_First;

                INSERT INTO ScheduleNote_First
                (SNF_ID, SNF_CustId, SNF_Description, SNF_Category, SNF_CYear_Amount, SNF_PYear_Amount, 
                 SNF_YearId, SNF_CompId, SNF_Status, SNF_DelFlag, SNF_CRON, SNF_CRBY, SNF_IPAddress)
                VALUES
                (@NextId, @CustomerId, @Description, 'CC', @CurrentYearAmount, @PreviousYearAmount,
                 @FinancialYearId, @CompanyId, 'W', 'X', GETDATE(), @UserId, @IpAddress);

                SELECT @NextId;";


                var newId = await connection.ExecuteScalarAsync<int>(insertQuery, dto);
                return newId;
            }
        }

        //SaveForfeitedShares
        public async Task<int> SaveForfeitedSharesAsync(ForfeitedSharesDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (dto.Id != 0)
            {
                // ✅ Update existing record
                var updateQuery = @"
                UPDATE ScheduleNote_First 
                SET SNF_Description = @Description,
                    SNF_CYear_Amount = @CurrentYearAmount,
                    SNF_PYear_Amount = @PreviousYearAmount,
                    SNF_CRBY = @UserId,
                    SNF_CRON = GETDATE()
                WHERE SNF_ID = @Id 
                  AND SNF_Category = 'FD'
                  AND SNF_CustId = @CustomerId 
                  AND SNF_YearId = @FinancialYearId";

                await connection.ExecuteAsync(updateQuery, dto);
                return dto.Id;
            }
            else
            {
                // ✅ Insert new record
                var insertQuery = @"
                DECLARE @NextId INT;
                SELECT @NextId = ISNULL(MAX(SNF_ID), 0) + 1 FROM ScheduleNote_First;

                INSERT INTO ScheduleNote_First
                (SNF_ID, SNF_CustId, SNF_Description, SNF_Category, SNF_CYear_Amount, SNF_PYear_Amount, 
                 SNF_YearId, SNF_CompId, SNF_Status, SNF_DelFlag, SNF_CRON, SNF_CRBY, SNF_IPAddress)
                VALUES
                (@NextId, @CustomerId, @Description, 'FD', @CurrentYearAmount, @PreviousYearAmount,
                 @FinancialYearId, @CompanyId, 'W', 'X', GETDATE(), @UserId, @IpAddress);

                SELECT @NextId;";


                var newId = await connection.ExecuteScalarAsync<int>(insertQuery, dto);
                return newId;
            }
        }

        //Save(i)EquityShares
        public async Task<int> SaveEquitySharesAsync(EquitySharesDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (dto.SNS_ID != 0)
            {
                // ✅ Delete existing record (to maintain only one per customer-year-category)
                var deleteQuery = @"
        DELETE FROM ScheduleNote_Second
        WHERE SNS_CustId = @CustomerId
          AND SNS_YEARId = @FinancialYearId
          AND SNS_Category = 'SF'";
                await connection.ExecuteAsync(deleteQuery, new { CustomerId = dto.SNS_CustId, FinancialYearId = dto.SNS_YearID });
            }

            // ✅ Insert new record
            var insertQuery = @"
    DECLARE @NextId INT;
    SELECT @NextId = ISNULL(MAX(SNS_ID), 0) + 1 FROM ScheduleNote_Second;

    INSERT INTO ScheduleNote_Second
    (SNS_ID, SNS_CustId, SNS_Description, SNS_Category,
     SNS_CYear_BegShares, SNS_CYear_BegAmount, SNS_PYear_BegShares, SNS_PYear_BegAmount,
     SNS_CYear_AddShares, SNS_CYear_AddAmount, SNS_PYear_AddShares, SNS_PYear_AddAmount,
     SNS_CYear_EndShares, SNS_CYear_EndAmount, SNS_PYear_EndShares, SNS_PYear_EndAmount,
     SNS_YearId, SNS_CompId, SNS_Status, SNS_DelFlag, SNS_CRON, SNS_CRBY, SNS_IPAddress)
    VALUES
    (@NextId, @SNS_CustId, @SNS_Description, 'SF',
     @SNS_CYear_BegShares, @SNS_CYear_BegAmount, @SNS_PYear_BegShares, @SNS_PYear_BegAmount,
     @SNS_CYear_AddShares, @SNS_CYear_AddAmount, @SNS_PYear_AddShares, @SNS_PYear_AddAmount,
     @SNS_CYear_EndShares, @SNS_CYear_EndAmount, @SNS_PYear_EndShares, @SNS_PYear_EndAmount,
     @SNS_YearID, @SNS_CompID, 'W', 'X', GETDATE(), @SNS_CrBy, @SNS_IPAddress);

    SELECT @NextId;";

            var newId = await connection.ExecuteScalarAsync<int>(insertQuery, dto);
            return newId;
        }

        //Save(ii)PreferenceShares
        public async Task<int> SavePreferenceSharesAsync(PreferenceSharesDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (dto.SNS_ID != 0)
            {
                // ✅ Delete existing record (to maintain only one per customer-year-category)
                var deleteQuery = @"
        DELETE FROM ScheduleNote_Second
        WHERE SNS_CustId = @CustomerId
          AND SNS_YEARId = @FinancialYearId
          AND SNS_Category = 'SS'";
                await connection.ExecuteAsync(deleteQuery, new { CustomerId = dto.SNS_CustId, FinancialYearId = dto.SNS_YearID });
            }

            // ✅ Insert new record
            var insertQuery = @"
    DECLARE @NextId INT;
    SELECT @NextId = ISNULL(MAX(SNS_ID), 0) + 1 FROM ScheduleNote_Second;

    INSERT INTO ScheduleNote_Second
    (SNS_ID, SNS_CustId, SNS_Description, SNS_Category,
     SNS_CYear_BegShares, SNS_CYear_BegAmount, SNS_PYear_BegShares, SNS_PYear_BegAmount,
     SNS_CYear_AddShares, SNS_CYear_AddAmount, SNS_PYear_AddShares, SNS_PYear_AddAmount,
     SNS_CYear_EndShares, SNS_CYear_EndAmount, SNS_PYear_EndShares, SNS_PYear_EndAmount,
     SNS_YearId, SNS_CompId, SNS_Status, SNS_DelFlag, SNS_CRON, SNS_CRBY, SNS_IPAddress)
    VALUES
    (@NextId, @SNS_CustId, @SNS_Description, 'SS',
     @SNS_CYear_BegShares, @SNS_CYear_BegAmount, @SNS_PYear_BegShares, @SNS_PYear_BegAmount,
     @SNS_CYear_AddShares, @SNS_CYear_AddAmount, @SNS_PYear_AddShares, @SNS_PYear_AddAmount,
     @SNS_CYear_EndShares, @SNS_CYear_EndAmount, @SNS_PYear_EndShares, @SNS_PYear_EndAmount,
     @SNS_YearID, @SNS_CompID, 'W', 'X', GETDATE(), @SNS_CrBy, @SNS_IPAddress);

    SELECT @NextId;";

            var newId = await connection.ExecuteScalarAsync<int>(insertQuery, dto);
            return newId;
        }

        //Save(iii)EquityShares
        public async Task<int> SaveiiiEquitySharesAsync(iiiEquitySharesDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (dto.SNS_ID != 0)
            {
                // ✅ Delete existing record (to maintain only one per customer-year-category)
                var deleteQuery = @"
        DELETE FROM ScheduleNote_Second
        WHERE SNS_CustId = @CustomerId
          AND SNS_YEARId = @FinancialYearId
          AND SNS_Category = 'ST'";
                await connection.ExecuteAsync(deleteQuery, new { CustomerId = dto.SNS_CustId, FinancialYearId = dto.SNS_YearID });
            }

            // ✅ Insert new record
            var insertQuery = @"
    DECLARE @NextId INT;
    SELECT @NextId = ISNULL(MAX(SNS_ID), 0) + 1 FROM ScheduleNote_Second;

    INSERT INTO ScheduleNote_Second
    (SNS_ID, SNS_CustId, SNS_Description, SNS_Category,
     SNS_CYear_BegShares, SNS_CYear_BegAmount, SNS_PYear_BegShares, SNS_PYear_BegAmount,
     SNS_CYear_AddShares, SNS_CYear_AddAmount, SNS_PYear_AddShares, SNS_PYear_AddAmount,
     SNS_CYear_EndShares, SNS_CYear_EndAmount, SNS_PYear_EndShares, SNS_PYear_EndAmount,
     SNS_YearId, SNS_CompId, SNS_Status, SNS_DelFlag, SNS_CRON, SNS_CRBY, SNS_IPAddress)
    VALUES
    (@NextId, @SNS_CustId, @SNS_Description, 'ST',
     @SNS_CYear_BegShares, @SNS_CYear_BegAmount, @SNS_PYear_BegShares, @SNS_PYear_BegAmount,
     @SNS_CYear_AddShares, @SNS_CYear_AddAmount, @SNS_PYear_AddShares, @SNS_PYear_AddAmount,
     @SNS_CYear_EndShares, @SNS_CYear_EndAmount, @SNS_PYear_EndShares, @SNS_PYear_EndAmount,
     @SNS_YearID, @SNS_CompID, 'W', 'X', GETDATE(), @SNS_CrBy, @SNS_IPAddress);

    SELECT @NextId;";

            var newId = await connection.ExecuteScalarAsync<int>(insertQuery, dto);
            return newId;
        }

        //Save(iv)PreferenceShares
        public async Task<int> SaveivPreferenceSharesAsync(ivPreferenceSharesDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (dto.SNS_ID != 0)
            {
                // ✅ Delete existing record (to maintain only one per customer-year-category)
                var deleteQuery = @"
        DELETE FROM ScheduleNote_Second
        WHERE SNS_CustId = @CustomerId
          AND SNS_YEARId = @FinancialYearId
          AND SNS_Category = 'SV'";
                await connection.ExecuteAsync(deleteQuery, new { CustomerId = dto.SNS_CustId, FinancialYearId = dto.SNS_YearID });
            }

            // ✅ Insert new record
            var insertQuery = @"
    DECLARE @NextId INT;
    SELECT @NextId = ISNULL(MAX(SNS_ID), 0) + 1 FROM ScheduleNote_Second;

    INSERT INTO ScheduleNote_Second
    (SNS_ID, SNS_CustId, SNS_Description, SNS_Category,
     SNS_CYear_BegShares, SNS_CYear_BegAmount, SNS_PYear_BegShares, SNS_PYear_BegAmount,
     SNS_CYear_AddShares, SNS_CYear_AddAmount, SNS_PYear_AddShares, SNS_PYear_AddAmount,
     SNS_CYear_EndShares, SNS_CYear_EndAmount, SNS_PYear_EndShares, SNS_PYear_EndAmount,
     SNS_YearId, SNS_CompId, SNS_Status, SNS_DelFlag, SNS_CRON, SNS_CRBY, SNS_IPAddress)
    VALUES
    (@NextId, @SNS_CustId, @SNS_Description, 'SV',
     @SNS_CYear_BegShares, @SNS_CYear_BegAmount, @SNS_PYear_BegShares, @SNS_PYear_BegAmount,
     @SNS_CYear_AddShares, @SNS_CYear_AddAmount, @SNS_PYear_AddShares, @SNS_PYear_AddAmount,
     @SNS_CYear_EndShares, @SNS_CYear_EndAmount, @SNS_PYear_EndShares, @SNS_PYear_EndAmount,
     @SNS_YearID, @SNS_CompID, 'W', 'X', GETDATE(), @SNS_CrBy, @SNS_IPAddress);

    SELECT @NextId;";

            var newId = await connection.ExecuteScalarAsync<int>(insertQuery, dto);
            return newId;
        }

        //Save(b)EquityShareCapital
        public async Task<int> SavebEquityShareCapitalAsync(EquityShareCapitalDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (dto.Id != 0)
            {
                // ✅ Update existing record (category hardcoded to 'TBE')
                var updateQuery = @"
        UPDATE ScheduleNote_Third 
        SET SNT_Description   = @Description,
            SNT_CYear_Shares  = @CYearShares,
            SNT_CYear_Amount  = @CYearAmount,
            SNT_PYear_Shares  = @PYearShares,
            SNT_PYear_Amount  = @PYearAmount,
            SNT_CRBY          = @UserId,
            SNT_CRON          = GETDATE()
        WHERE SNT_ID = @Id 
          AND SNT_Category = 'TBE'
          AND SNT_CustId = @CustomerId 
          AND SNT_YearId = @FinancialYearId;";

                await connection.ExecuteAsync(updateQuery, new
                {
                    Id = dto.Id,
                    Description = dto.Description,
                    CYearShares = dto.CYearShares,
                    CYearAmount = dto.CYearAmount,
                    PYearShares = dto.PYearShares,
                    PYearAmount = dto.PYearAmount,
                    CustomerId = dto.CustomerId,
                    FinancialYearId = dto.FinancialYearId,
                    UserId = dto.UserId
                });

                return dto.Id;
            }
            else
            {
                // ✅ Insert new record (manual ID generation - category/status/delflg hardcoded)
                var insertQuery = @"
        DECLARE @NextId INT;
        SELECT @NextId = ISNULL(MAX(SNT_ID), 0) + 1 FROM ScheduleNote_Third;

        INSERT INTO ScheduleNote_Third
        (SNT_ID, SNT_CustId, SNT_Description, SNT_Category, 
         SNT_CYear_Shares, SNT_CYear_Amount, SNT_PYear_Shares, SNT_PYear_Amount,
         SNT_YearId, SNT_CompId, SNT_Status, SNT_DelFlag, SNT_CRON, SNT_CRBY, SNT_IPAddress)
        VALUES
        (@NextId, @CustomerId, @Description, 'TBE',
         @CYearShares, @CYearAmount, @PYearShares, @PYearAmount,
         @FinancialYearId, @CompanyId, 'W', 'X', GETDATE(), @UserId, @IpAddress);

        SELECT @NextId;";

                var newId = await connection.ExecuteScalarAsync<int>(insertQuery, new
                {
                    CustomerId = dto.CustomerId,
                    Description = dto.Description,
                    CYearShares = dto.CYearShares,
                    CYearAmount = dto.CYearAmount,
                    PYearShares = dto.PYearShares,
                    PYearAmount = dto.PYearAmount,
                    FinancialYearId = dto.FinancialYearId,
                    CompanyId = dto.CompanyId,
                    UserId = dto.UserId,
                    IpAddress = dto.IpAddress ?? string.Empty
                });
                return newId;
            }
        }

        //Save(b)PreferenceShareCapital
        public async Task<int> SavebPreferenceShareCapitalAsync(PreferenceShareCapitalDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (dto.Id != 0)
            {
                // ✅ Update existing record (category hardcoded to 'TBE')
                var updateQuery = @"
        UPDATE ScheduleNote_Third 
        SET SNT_Description   = @Description,
            SNT_CYear_Shares  = @CYearShares,
            SNT_CYear_Amount  = @CYearAmount,
            SNT_PYear_Shares  = @PYearShares,
            SNT_PYear_Amount  = @PYearAmount,
            SNT_CRBY          = @UserId,
            SNT_CRON          = GETDATE()
        WHERE SNT_ID = @Id 
          AND SNT_Category = 'TBp'
          AND SNT_CustId = @CustomerId 
          AND SNT_YearId = @FinancialYearId;";

                await connection.ExecuteAsync(updateQuery, new
                {
                    Id = dto.Id,
                    Description = dto.Description,
                    CYearShares = dto.CYearShares,
                    CYearAmount = dto.CYearAmount,
                    PYearShares = dto.PYearShares,
                    PYearAmount = dto.PYearAmount,
                    CustomerId = dto.CustomerId,
                    FinancialYearId = dto.FinancialYearId,
                    UserId = dto.UserId
                });

                return dto.Id;
            }
            else
            {
                // ✅ Insert new record (manual ID generation - category/status/delflg hardcoded)
                var insertQuery = @"
        DECLARE @NextId INT;
        SELECT @NextId = ISNULL(MAX(SNT_ID), 0) + 1 FROM ScheduleNote_Third;

        INSERT INTO ScheduleNote_Third
        (SNT_ID, SNT_CustId, SNT_Description, SNT_Category, 
         SNT_CYear_Shares, SNT_CYear_Amount, SNT_PYear_Shares, SNT_PYear_Amount,
         SNT_YearId, SNT_CompId, SNT_Status, SNT_DelFlag, SNT_CRON, SNT_CRBY, SNT_IPAddress)
        VALUES
        (@NextId, @CustomerId, @Description, 'TBp',
         @CYearShares, @CYearAmount, @PYearShares, @PYearAmount,
         @FinancialYearId, @CompanyId, 'W', 'X', GETDATE(), @UserId, @IpAddress);

        SELECT @NextId;";

                var newId = await connection.ExecuteScalarAsync<int>(insertQuery, new
                {
                    CustomerId = dto.CustomerId,
                    Description = dto.Description,
                    CYearShares = dto.CYearShares,
                    CYearAmount = dto.CYearAmount,
                    PYearShares = dto.PYearShares,
                    PYearAmount = dto.PYearAmount,
                    FinancialYearId = dto.FinancialYearId,
                    CompanyId = dto.CompanyId,
                    UserId = dto.UserId,
                    IpAddress = dto.IpAddress ?? string.Empty
                });
                return newId;
            }
        }

        //Save(c)Terms/rights attached to equity shares
        public async Task<int> SaveTermsToEquityShareAsync(TermsToEquityShareeDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ✅ Step 3: Delete existing rows for Customer + Year + Category
            var deleteQuery = @"
        DELETE FROM ScheduleNote_Desc
        WHERE SND_CustId = @CustomerId
          AND SND_YearId = @FinancialYearId
          AND SND_Category = 'cEquity';";

            await connection.ExecuteAsync(deleteQuery, new
            {
                CustomerId = dto.CustomerId,
                FinancialYearId = dto.FinancialYearId
            });

            // ✅ Step 4: Generate next ID
            var nextIdQuery = "SELECT ISNULL(MAX(SND_ID), 0) + 1 FROM ScheduleNote_Desc";
            var nextId = await connection.ExecuteScalarAsync<int>(nextIdQuery);

            // ✅ Step 5: Insert new record (Category, Status, DelFlag hardcoded)
            var insertQuery = @"
        INSERT INTO ScheduleNote_Desc
        (SND_ID, SND_CustId, SND_Description, SND_Category,
         SND_YearId, SND_CompId, SND_Status, SND_DelFlag,
         SND_CRON, SND_CRBY, SND_IPAddress)
        VALUES
        (@NextId, @CustomerId, @Description, 'cEquity',
         @FinancialYearId, @CompanyId, 'W', 'X',
         GETDATE(), @UserId, @IpAddress);

        SELECT @NextId;";

            var newId = await connection.ExecuteScalarAsync<int>(insertQuery, new
            {
                NextId = nextId,
                CustomerId = dto.CustomerId,
                Description = dto.Description,
                FinancialYearId = dto.FinancialYearId,
                CompanyId = dto.CompanyId,
                UserId = dto.UserId,
                IpAddress = dto.IpAddress ?? string.Empty
            });

            return newId;
        }

        //Save(d)Terms/Rights attached to preference shares
        public async Task<int> SaveTermsToPreferenceShareAsync(TermsToPreferenceShareDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ✅ Step 3: Delete existing rows for Customer + Year + Category
            var deleteQuery = @"
        DELETE FROM ScheduleNote_Desc
        WHERE SND_CustId = @CustomerId
          AND SND_YearId = @FinancialYearId
          AND SND_Category = 'dPref';";

            await connection.ExecuteAsync(deleteQuery, new
            {
                CustomerId = dto.CustomerId,
                FinancialYearId = dto.FinancialYearId
            });

            // ✅ Step 4: Generate next ID
            var nextIdQuery = "SELECT ISNULL(MAX(SND_ID), 0) + 1 FROM ScheduleNote_Desc";
            var nextId = await connection.ExecuteScalarAsync<int>(nextIdQuery);

            // ✅ Step 5: Insert new record (Category, Status, DelFlag hardcoded)
            var insertQuery = @"
        INSERT INTO ScheduleNote_Desc
        (SND_ID, SND_CustId, SND_Description, SND_Category,
         SND_YearId, SND_CompId, SND_Status, SND_DelFlag,
         SND_CRON, SND_CRBY, SND_IPAddress)
        VALUES
        (@NextId, @CustomerId, @Description, 'dPref',
         @FinancialYearId, @CompanyId, 'W', 'X',
         GETDATE(), @UserId, @IpAddress);

        SELECT @NextId;";

            var newId = await connection.ExecuteScalarAsync<int>(insertQuery, new
            {
                NextId = nextId,
                CustomerId = dto.CustomerId,
                Description = dto.Description,
                FinancialYearId = dto.FinancialYearId,
                CompanyId = dto.CompanyId,
                UserId = dto.UserId,
                IpAddress = dto.IpAddress ?? string.Empty
            });

            return newId;
        }

        //Save(e)Nameofthesharholder
        public async Task<int> SaveeNameofthesharholderAsync(NameofthesharholderDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (dto.Id != 0)
            {
                // ✅ Update existing record (category hardcoded to 'TBE')
                var updateQuery = @"
        UPDATE ScheduleNote_Third 
        SET SNT_Description   = @Description,
            SNT_CYear_Shares  = @CYearShares,
            SNT_CYear_Amount  = @CYearAmount,
            SNT_PYear_Shares  = @PYearShares,
            SNT_PYear_Amount  = @PYearAmount,
            SNT_CRBY          = @UserId,
            SNT_CRON          = GETDATE()
        WHERE SNT_ID = @Id 
          AND SNT_Category = 'TEE'
          AND SNT_CustId = @CustomerId 
          AND SNT_YearId = @FinancialYearId;";

                await connection.ExecuteAsync(updateQuery, new
                {
                    Id = dto.Id,
                    Description = dto.Description,
                    CYearShares = dto.CYearShares,
                    CYearAmount = dto.CYearAmount,
                    PYearShares = dto.PYearShares,
                    PYearAmount = dto.PYearAmount,
                    CustomerId = dto.CustomerId,
                    FinancialYearId = dto.FinancialYearId,
                    UserId = dto.UserId
                });

                return dto.Id;
            }
            else
            {
                // ✅ Insert new record (manual ID generation - category/status/delflg hardcoded)
                var insertQuery = @"
        DECLARE @NextId INT;
        SELECT @NextId = ISNULL(MAX(SNT_ID), 0) + 1 FROM ScheduleNote_Third;

        INSERT INTO ScheduleNote_Third
        (SNT_ID, SNT_CustId, SNT_Description, SNT_Category, 
         SNT_CYear_Shares, SNT_CYear_Amount, SNT_PYear_Shares, SNT_PYear_Amount,
         SNT_YearId, SNT_CompId, SNT_Status, SNT_DelFlag, SNT_CRON, SNT_CRBY, SNT_IPAddress)
        VALUES
        (@NextId, @CustomerId, @Description, 'TEE',
         @CYearShares, @CYearAmount, @PYearShares, @PYearAmount,
         @FinancialYearId, @CompanyId, 'W', 'X', GETDATE(), @UserId, @IpAddress);

        SELECT @NextId;";

                var newId = await connection.ExecuteScalarAsync<int>(insertQuery, new
                {
                    CustomerId = dto.CustomerId,
                    Description = dto.Description,
                    CYearShares = dto.CYearShares,
                    CYearAmount = dto.CYearAmount,
                    PYearShares = dto.PYearShares,
                    PYearAmount = dto.PYearAmount,
                    FinancialYearId = dto.FinancialYearId,
                    CompanyId = dto.CompanyId,
                    UserId = dto.UserId,
                    IpAddress = dto.IpAddress ?? string.Empty
                });
                return newId;
            }
        }

        //Save(e)PreferenceShares
        public async Task<int> SaveePreferenceSharesAsync(ePreferenceSharesDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (dto.Id != 0)
            {
                // ✅ Update existing record (category hardcoded to 'TBE')
                var updateQuery = @"
        UPDATE ScheduleNote_Third 
        SET SNT_Description   = @Description,
            SNT_CYear_Shares  = @CYearShares,
            SNT_CYear_Amount  = @CYearAmount,
            SNT_PYear_Shares  = @PYearShares,
            SNT_PYear_Amount  = @PYearAmount,
            SNT_CRBY          = @UserId,
            SNT_CRON          = GETDATE()
        WHERE SNT_ID = @Id 
          AND SNT_Category = 'TEP'
          AND SNT_CustId = @CustomerId 
          AND SNT_YearId = @FinancialYearId;";

                await connection.ExecuteAsync(updateQuery, new
                {
                    Id = dto.Id,
                    Description = dto.Description,
                    CYearShares = dto.CYearShares,
                    CYearAmount = dto.CYearAmount,
                    PYearShares = dto.PYearShares,
                    PYearAmount = dto.PYearAmount,
                    CustomerId = dto.CustomerId,
                    FinancialYearId = dto.FinancialYearId,
                    UserId = dto.UserId
                });

                return dto.Id;
            }
            else
            {
                // ✅ Insert new record (manual ID generation - category/status/delflg hardcoded)
                var insertQuery = @"
        DECLARE @NextId INT;
        SELECT @NextId = ISNULL(MAX(SNT_ID), 0) + 1 FROM ScheduleNote_Third;

        INSERT INTO ScheduleNote_Third
        (SNT_ID, SNT_CustId, SNT_Description, SNT_Category, 
         SNT_CYear_Shares, SNT_CYear_Amount, SNT_PYear_Shares, SNT_PYear_Amount,
         SNT_YearId, SNT_CompId, SNT_Status, SNT_DelFlag, SNT_CRON, SNT_CRBY, SNT_IPAddress)
        VALUES
        (@NextId, @CustomerId, @Description, 'TEP',
         @CYearShares, @CYearAmount, @PYearShares, @PYearAmount,
         @FinancialYearId, @CompanyId, 'W', 'X', GETDATE(), @UserId, @IpAddress);

        SELECT @NextId;";

                var newId = await connection.ExecuteScalarAsync<int>(insertQuery, new
                {
                    CustomerId = dto.CustomerId,
                    Description = dto.Description,
                    CYearShares = dto.CYearShares,
                    CYearAmount = dto.CYearAmount,
                    PYearShares = dto.PYearShares,
                    PYearAmount = dto.PYearAmount,
                    FinancialYearId = dto.FinancialYearId,
                    CompanyId = dto.CompanyId,
                    UserId = dto.UserId,
                    IpAddress = dto.IpAddress ?? string.Empty
                });
                return newId;
            }
        }

        //Save(f)SharesAllotted
        public async Task<int> SavefSharesAllottedAsync(FSahresAllottedDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ✅ Step 3: Delete existing rows for Customer + Year + Category
            var deleteQuery = @"
        DELETE FROM ScheduleNote_Desc
        WHERE SND_CustId = @CustomerId
          AND SND_YearId = @FinancialYearId
          AND SND_Category = 'fShares';";

            await connection.ExecuteAsync(deleteQuery, new
            {
                CustomerId = dto.CustomerId,
                FinancialYearId = dto.FinancialYearId
            });

            // ✅ Step 4: Generate next ID
            var nextIdQuery = "SELECT ISNULL(MAX(SND_ID), 0) + 1 FROM ScheduleNote_Desc";
            var nextId = await connection.ExecuteScalarAsync<int>(nextIdQuery);

            // ✅ Step 5: Insert new record (Category, Status, DelFlag hardcoded)
            var insertQuery = @"
        INSERT INTO ScheduleNote_Desc
        (SND_ID, SND_CustId, SND_Description, SND_Category,
         SND_YearId, SND_CompId, SND_Status, SND_DelFlag,
         SND_CRON, SND_CRBY, SND_IPAddress)
        VALUES
        (@NextId, @CustomerId, @Description, 'fShares',
         @FinancialYearId, @CompanyId, 'W', 'X',
         GETDATE(), @UserId, @IpAddress);

        SELECT @NextId;";

            var newId = await connection.ExecuteScalarAsync<int>(insertQuery, new
            {
                NextId = nextId,
                CustomerId = dto.CustomerId,
                Description = dto.Description,
                FinancialYearId = dto.FinancialYearId,
                CompanyId = dto.CompanyId,
                UserId = dto.UserId,
                IpAddress = dto.IpAddress ?? string.Empty
            });
            return newId;
        }

        //SaveEquityShares(Promoter name)
        public async Task<int> SaveEquitySharesPromoterNameAsync(SaveEquitySharesPromoterNameDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (dto.Id != 0)
            {
                // ✅ Update existing record
                var updateQuery = @"
            UPDATE ScheduleNote_Fourth
            SET SNFT_Description  = @Description,
                SNFT_NumShares    = @NumShares,
                SNFT_TotalShares  = @TotalShares,
                SNFT_ChangedShares = @ChangedShares,
                SNFT_CRBY         = @UserId,
                SNFT_CRON         = GETDATE()
            WHERE SNFT_ID = @Id
              AND SNFT_Category = 'FSC'
              AND SNFT_CustId = @CustomerId
              AND SNFT_YearId = @FinancialYearId;";

                await connection.ExecuteAsync(updateQuery, new
                {
                    Id = dto.Id,
                    Description = dto.Description,
                    NumShares = dto.NumShares,
                    TotalShares = dto.TotalShares,
                    ChangedShares = dto.ChangedShares,
                    CustomerId = dto.CustomerId,
                    FinancialYearId = dto.FinancialYearId,
                    UserId = dto.UserId
                });

                return dto.Id;
            }
            else
            {
                // ✅ Insert new record (manual ID generation)
                var insertQuery = @"
            DECLARE @NextId INT;
            SELECT @NextId = ISNULL(MAX(SNFT_ID), 0) + 1 FROM ScheduleNote_Fourth;

            INSERT INTO ScheduleNote_Fourth
            (SNFT_ID, SNFT_CustId, SNFT_Description, SNFT_Category,
             SNFT_NumShares, SNFT_TotalShares, SNFT_ChangedShares,
             SNFT_YearId, SNFT_CompId,
             SNFT_Status, SNFT_DelFlag, SNFT_CRON, SNFT_CRBY, SNFT_IPAddress)
            VALUES
            (@NextId, @CustomerId, @Description, 'FSC',
             @NumShares, @TotalShares, @ChangedShares,
             @FinancialYearId, @CompanyId,
             'W', 'X', GETDATE(), @UserId, @IpAddress);

            SELECT @NextId;";

                var newId = await connection.ExecuteScalarAsync<int>(insertQuery, new
                {
                    CustomerId = dto.CustomerId,
                    Description = dto.Description,
                    NumShares = dto.NumShares,
                    TotalShares = dto.TotalShares,
                    ChangedShares = dto.ChangedShares,
                    FinancialYearId = dto.FinancialYearId,
                    CompanyId = dto.CompanyId,
                    UserId = dto.UserId,
                    IpAddress = dto.IpAddress ?? string.Empty
                });

                return newId;
            }
        }

        //SavePreferenceShares(Promoter name)
        public async Task<int> SavePreferenceSharesPromoterNameAsync(SavePreferenceSharesPromoterNameDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            if (dto.Id != 0)
            {
                // ✅ Update existing record
                var updateQuery = @"
            UPDATE ScheduleNote_Fourth
            SET SNFT_Description  = @Description,
                SNFT_NumShares    = @NumShares,
                SNFT_TotalShares  = @TotalShares,
                SNFT_ChangedShares = @ChangedShares,
                SNFT_CRBY         = @UserId,
                SNFT_CRON         = GETDATE()
            WHERE SNFT_ID = @Id
              AND SNFT_Category = 'FSP'
              AND SNFT_CustId = @CustomerId
              AND SNFT_YearId = @FinancialYearId;";

                await connection.ExecuteAsync(updateQuery, new
                {
                    Id = dto.Id,
                    Description = dto.Description,
                    NumShares = dto.NumShares,
                    TotalShares = dto.TotalShares,
                    ChangedShares = dto.ChangedShares,
                    CustomerId = dto.CustomerId,
                    FinancialYearId = dto.FinancialYearId,
                    UserId = dto.UserId
                });

                return dto.Id;
            }
            else
            {
                // ✅ Insert new record (manual ID generation)
                var insertQuery = @"
            DECLARE @NextId INT;
            SELECT @NextId = ISNULL(MAX(SNFT_ID), 0) + 1 FROM ScheduleNote_Fourth;

            INSERT INTO ScheduleNote_Fourth
            (SNFT_ID, SNFT_CustId, SNFT_Description, SNFT_Category,
             SNFT_NumShares, SNFT_TotalShares, SNFT_ChangedShares,
             SNFT_YearId, SNFT_CompId,
             SNFT_Status, SNFT_DelFlag, SNFT_CRON, SNFT_CRBY, SNFT_IPAddress)
            VALUES
            (@NextId, @CustomerId, @Description, 'FSP',
             @NumShares, @TotalShares, @ChangedShares,
             @FinancialYearId, @CompanyId,
             'W', 'X', GETDATE(), @UserId, @IpAddress);

            SELECT @NextId;";

                var newId = await connection.ExecuteScalarAsync<int>(insertQuery, new
                {
                    CustomerId = dto.CustomerId,
                    Description = dto.Description,
                    NumShares = dto.NumShares,
                    TotalShares = dto.TotalShares,
                    ChangedShares = dto.ChangedShares,
                    FinancialYearId = dto.FinancialYearId,
                    CompanyId = dto.CompanyId,
                    UserId = dto.UserId,
                    IpAddress = dto.IpAddress ?? string.Empty
                });

                return newId;
            }
        }

        //SaveFootNote
        public async Task<int> SaveFootNoteAsync(FootNoteDto dto)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ✅ Step 3: Delete existing rows for Customer + Year + Category
            var deleteQuery = @"
        DELETE FROM ScheduleNote_Desc
        WHERE SND_CustId = @CustomerId
          AND SND_YearId = @FinancialYearId
          AND SND_Category = 'footNote';";

            await connection.ExecuteAsync(deleteQuery, new
            {
                CustomerId = dto.CustomerId,
                FinancialYearId = dto.FinancialYearId
            });

            // ✅ Step 4: Generate next ID
            var nextIdQuery = "SELECT ISNULL(MAX(SND_ID), 0) + 1 FROM ScheduleNote_Desc";
            var nextId = await connection.ExecuteScalarAsync<int>(nextIdQuery);

            // ✅ Step 5: Insert new record (Category, Status, DelFlag hardcoded)
            var insertQuery = @"
        INSERT INTO ScheduleNote_Desc
        (SND_ID, SND_CustId, SND_Description, SND_Category,
         SND_YearId, SND_CompId, SND_Status, SND_DelFlag,
         SND_CRON, SND_CRBY, SND_IPAddress)
        VALUES
        (@NextId, @CustomerId, @Description, 'footNote',
         @FinancialYearId, @CompanyId, 'W', 'X',
         GETDATE(), @UserId, @IpAddress);

        SELECT @NextId;";

            var newId = await connection.ExecuteScalarAsync<int>(insertQuery, new
            {
                NextId = nextId,
                CustomerId = dto.CustomerId,
                Description = dto.Description,
                FinancialYearId = dto.FinancialYearId,
                CompanyId = dto.CompanyId,
                UserId = dto.UserId,
                IpAddress = dto.IpAddress ?? string.Empty
            });
            return newId;
        }

        //GetFirstNote
        public async Task<IEnumerable<FirstNoteDto>> GetFirstNoteAsync(int compId, string category, int custId, int YearId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT 
            SNF_ID, 
            SNF_CustId,
            SNF_Description, 
            SNF_CYear_Amount, 
            SNF_PYear_Amount
        FROM ScheduleNote_First
        WHERE SNF_Category = @Category
          AND SNF_CompId = @CompId
          AND SNF_CustId = @CustId
          AND SNF_YEARId = @YearId
          AND SNT_DELFLAG = 'X'";

            return await connection.QueryAsync<FirstNoteDto>(query, new
            {
                Category = category,
                CompId = compId,
                CustId = custId,
                YearId = YearId
            });
        }

        //GetSecondNoteById
        public async Task<IEnumerable<SecondNoteDto>> GetSecondNoteByIdAsync(int compId, string category, int custId, int YearId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT 
            SNS_ID, 
            SNS_CustId,
            SNS_Description, 
            SNS_CYear_BegShares, SNS_CYear_BegAmount, SNS_PYear_BegShares, SNS_PYear_BegAmount
            SNS_CYear_AddShares, SNS_CYear_AddAmount, SNS_PYear_AddShares, SNS_PYear_AddAmount,
            SNS_CYear_EndShares, SNS_CYear_EndAmount, SNS_PYear_EndShares, SNS_PYear_EndAmount
        FROM ScheduleNote_Second
        WHERE SNS_Category = @Category
          AND SNS_CompId = @CompId
          AND SNS_CustId = @CustId
          AND SNS_YEARId = @YearId";
             
            return await connection.QueryAsync<SecondNoteDto>(query, new
            {
                Category = category,
                CompId = compId,
                CustId = custId,
                YearId = YearId
            });
        }

        //GetDescriptionNoteById
        public async Task<IEnumerable<DescriptionNoteDto>> GetDescriptionNoteAsync(int compId, string category, int custId, int YearId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT 
            SND_ID, 
            SND_CustId,
            SND_Description 
        FROM ScheduleNote_Desc
        WHERE SND_Category = @Category
          AND SND_CompId = @CompId
          AND SND_CustId = @CustId
          AND SND_YEARId = @YearId";

            return await connection.QueryAsync<DescriptionNoteDto>(query, new
            {
                Category = category,
                CompId = compId,
                CustId = custId,
                YearId = YearId
            });
        }

        //GetThirdNote
        public async Task<IEnumerable<ThirdNoteDto>> GetThirdNoteAsync(int compId, string category, int custId, int YearId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT 
            SNT_ID, 
            SNT_CustId,
            SNT_Description, 
            SNT_CYear_Amount, 
            SNT_PYear_Amount
        FROM ScheduleNote_Third
        WHERE SNT_Category = @Category
          AND SNT_CompId = @CompId
          AND SNT_CustId = @CustId
          AND SNT_YEARId = @YearId
          AND SNT_DELFLAG='X'";

            return await connection.QueryAsync<ThirdNoteDto>(query, new
            {
                Category = category,
                CompId = compId,
                CustId = custId,
                YearId = YearId
            });
        }

        //GetFourthNote
        public async Task<IEnumerable<FourthNoteDto>> GetFourthNoteAsync(int compId, string category, int custId, int YearId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT 
            SNFT_ID,
            SNFT_CustId, 
            SNFT_Description, 
            SNFT_NumShares,
            SNFT_TotalShares,
            SNFT_ChangedShares
        FROM ScheduleNote_Fourth
        WHERE SNFT_Category = @Category
          AND SNFT_CompId = @CompId
          AND SNFT_CustId = @CustId
          AND SNFT_YEARId = @YearId
          AND SNT_DELFLAG='X'";

            return await connection.QueryAsync<FourthNoteDto>(query, new
            {
                Category = category,
                CompId = compId,
                CustId = custId,
                YearId = YearId
            });
        }

        //DeleteFirstNote
        public async Task<int> DeleteSchedFirstNoteDetailsAsync(int id, int customerId, int compId, int yearId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Open connection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ✅ Step 4: Soft Delete (set DelFlag & Status to 'D')
            var query = @"
        UPDATE ScheduleNote_First
        SET SNF_DelFlag = 'D',
            SNF_Status  = 'D'
        WHERE SNF_ID     = @Id
          AND SNF_CustId = @CustomerId
          AND SNF_CompId = @CompId
          AND SNF_YearId = @YearId;";

            return await connection.ExecuteAsync(query, new
            {
                Id = id,
                CustomerId = customerId,
                CompId = compId,
                YearId = yearId
            });
        }

        //DeleteThirdNote
        public async Task<int> DeleteSchedThirdNoteDetailsAsync(int id, int customerId, int compId, int yearId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Open connection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ✅ Step 4: Soft delete (mark as deleted instead of removing)
            var query = @"
        UPDATE ScheduleNote_Third
        SET SNT_DelFlag = 'D',
            SNT_Status  = 'D'
        WHERE SNT_ID     = @Id
          AND SNT_CustId = @CustomerId
          AND SNT_CompId = @CompId
          AND SNT_YearId = @YearId;";

            return await connection.ExecuteAsync(query, new
            {
                Id = id,
                CustomerId = customerId,
                CompId = compId,
                YearId = yearId
            });
        }

        //DeleteFourthNote
        public async Task<int> DeleteSchedFourthNoteDetailsAsync(int id, int customerId, int compId, int yearId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Open SQL connection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ✅ Step 4: Perform soft delete
            var query = @"
        UPDATE ScheduleNote_Fourth
        SET SNFT_DelFlag = 'D',
            SNFT_Status  = 'D'
        WHERE SNFT_ID     = @Id
          AND SNFT_CustId = @CustomerId
          AND SNFT_CompId = @CompId
          AND SNFT_YearId = @YearId;";

            return await connection.ExecuteAsync(query, new
            {
                Id = id,
                CustomerId = customerId,
                CompId = compId,
                YearId = yearId
            });
        }

        //DownloadScheduleNoteExcel
        public ScheduleNoteFileDownloadResult GetNoteExcelTemplate()
        {
            var filePath = "C:\\Users\\SSD\\Desktop\\TracePa\\tracepa-dotnet-core - Copy\\SampleExcels\\ScheduleNoteExcel.xls";

            if (!File.Exists(filePath))
                return new ScheduleNoteFileDownloadResult();

            var bytes = File.ReadAllBytes(filePath);
            var fileName = "ScheduleNote.xls";   // ✅ keep .xls
            var contentType = "application/vnd.ms-excel"; // ✅ correct for .xls

            return new ScheduleNoteFileDownloadResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        //DownloadScheduleNotePDF
        public ScheduleNotePDFDownloadResult GetNotePDFTemplate()
        {
            var filePath = "C:\\Users\\SSD\\Desktop\\TracePa\\tracepa-dotnet-core - Copy\\SamplePdfs\\ScheduleNotePDF.pdf";

            if (!File.Exists(filePath))
                return new ScheduleNotePDFDownloadResult();

            var bytes = File.ReadAllBytes(filePath);
            var fileName = "ScheduleNotePDF.pdf";   // ✅ keep .xls
            var contentType = "application/vnd.ms-excel"; // ✅ correct for .xls

            return new ScheduleNotePDFDownloadResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        //DownloadScheduleNotePDFTemplate
        public async Task<Dictionary<string, DataTable>> GetScheduleNoteReportDataAsync(int companyId, int customerId, int financialYearId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var datasets = new Dictionary<string, DataTable>();

            // 🔹 First group - ScheduleNote_First
            string[] firstCategories = { "AU", "IS", "AI", "BS", "CC", "FD" };
            for (int i = 0; i < firstCategories.Length; i++)
            {
                var dt = await GetDataTableAsync(connection, @"
                SELECT SNF_Description, SNF_CYear_Amount, SNF_PYear_Amount 
                FROM ScheduleNote_First
                WHERE SNF_CompId = @CompanyId
                  AND SNF_CustId = @CustomerId
                  AND SNF_YearId = @FinancialYearId
                  AND SNF_Category = @Category
                  AND SNF_DelFlag <> 'X'
                ORDER BY SNF_ID;",
                    new { CompanyId = companyId, CustomerId = customerId, FinancialYearId = financialYearId, Category = firstCategories[i] });

                datasets.Add($"DataSet{i + 1}", dt);
            }

            // 🔹 Second group - ScheduleNote_Second
            string[] secondCategories = { "SF", "SS", "ST", "SV" };
            for (int i = 0; i < secondCategories.Length; i++)
            {
                var dt = await GetDataTableAsync(connection, @"
                SELECT * 
                FROM ScheduleNote_Second
                WHERE SNS_CompId = @CompanyId
                  AND SNS_CustId = @CustomerId
                  AND SNS_YearId = @FinancialYearId
                  AND SNS_Category = @Category
                  AND SNS_DelFlag <> 'X'
                ORDER BY SNS_ID;",
                    new { CompanyId = companyId, CustomerId = customerId, FinancialYearId = financialYearId, Category = secondCategories[i] });

                datasets.Add($"ScheduleNote_Second{i + 1}", dt);
            }

            // 🔹 Third group - ScheduleNote_Third
            string[] thirdCategories = { "TBE", "TBP", "TEE" };
            for (int i = 0; i < thirdCategories.Length; i++)
            {
                var dt = await GetDataTableAsync(connection, @"
                SELECT SNT_Description,SNT_CYear_Shares,SNT_CYear_Amount,SNT_PYear_Shares, SNT_PYear_Amount 
                FROM ScheduleNote_Third
                WHERE SNT_CompId = @CompanyId
                  AND SNT_CustId = @CustomerId
                  AND SNT_YearId = @FinancialYearId
                  AND SNT_Category = @Category
                  AND SNT_DelFlag <> 'X'
                ORDER BY SNT_ID;",
                    new { CompanyId = companyId, CustomerId = customerId, FinancialYearId = financialYearId, Category = thirdCategories[i] });

                datasets.Add($"ScheduleNote_Third{i + 1}", dt);
            }

            // 🔹 Description group - ScheduleNote_Desc
            string[] descCategories = { "cEquity", "dPref", "fShares", "footNote" };
            for (int i = 0; i < descCategories.Length; i++)
            {
                var dt = await GetDataTableAsync(connection, @"
                SELECT SND_Description 
                FROM ScheduleNote_Desc
                WHERE SND_CompId = @CompanyId
                  AND SND_CustId = @CustomerId
                  AND SND_YearId = @FinancialYearId
                  AND SND_Category = @Category
                  AND SND_DelFlag <> 'X'
                ORDER BY SND_ID;",
                    new { CompanyId = companyId, CustomerId = customerId, FinancialYearId = financialYearId, Category = descCategories[i] });

                datasets.Add($"ScheduleNote_Desc{i + 1}", dt);
            }

            // 🔹 Fourth group - ScheduleNote_Fourth
            string[] fourthCategories = { "FSC", "FSP" };
            for (int i = 0; i < fourthCategories.Length; i++)
            {
                var dt = await GetDataTableAsync(connection, @"
                SELECT SNFT_Description,SNFT_NumShares,SNFT_TotalShares,SNFT_ChangedShares 
                FROM ScheduleNote_Fourth
                WHERE SNFT_CompId = @CompanyId
                  AND SNFT_CustId = @CustomerId
                  AND SNFT_YearId = @FinancialYearId
                  AND SNFT_Category = @Category
                  AND SNFT_DelFlag <> 'D'
                ORDER BY SNFT_ID;",
                    new { CompanyId = companyId, CustomerId = customerId, FinancialYearId = financialYearId, Category = fourthCategories[i] });

                datasets.Add($"ScheduleNote_Fourth{i + 1}", dt);
            }

            return datasets;
        }
        private async Task<DataTable> GetDataTableAsync(SqlConnection connection, string query, object parameters)
        {
            var reader = await connection.ExecuteReaderAsync(query, parameters);
            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }
    }
}


