using System.Data;
using Dapper;
using Microsoft.Reporting.NETCore;
using QuestPDF.Helpers;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Commons.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using TracePca.Data;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleNoteDto;
using DocumentFormat.OpenXml.Bibliography;

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
        public async Task<IEnumerable<SubHeadingNoteDto>> GetSubHeadingDetailsAsync(int CompId, int CustId)
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
              ASSH_ID AS ASSH_ID,
              ASSH_Name AS ASSH_Name
            FROM ACC_ScheduleSubHeading 
            WHERE Assh_Orgtype = @CustId
               AND ASSH_CompId = @CompId";


            return await connection.QueryAsync<SubHeadingNoteDto>(query, new
            {
                CompId = CompId,
                CustId = CustId
            });
        }

        //SaveOrUpdateSubHeadingNotes(Notes For SubHeading)
        //public async Task<int[]> SaveSubHeadindNotesAsync(SubHeadingNotesDto dto)
        //{
        //    // ✅ Step 1: Get DB name from session
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    // ✅ Step 2: Get the connection string
        //    var connectionString = _configuration.GetConnectionString(dbName);

        //    // ✅ Step 3: Use SqlConnection
        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();
        //    {
        //        using (var transaction = connection.BeginTransaction())
        //        {
        //            try
        //            {
        //                using (var cmd = new SqlCommand("spACC_SubHeadingNoteDesc", connection, transaction))
        //                {
        //                    cmd.CommandType = CommandType.StoredProcedure;

        //                    cmd.Parameters.AddWithValue("@ASHN_ID", dto.ASHN_ID);
        //                    cmd.Parameters.AddWithValue("@ASHN_SubHeadingId", dto.ASHN_SubHeadingId);
        //                    cmd.Parameters.AddWithValue("@ASHN_CustomerId", dto.ASHN_CustomerId);
        //                    cmd.Parameters.AddWithValue("@ASHN_Description", dto.ASHN_Description ?? string.Empty);
        //                    cmd.Parameters.AddWithValue("@ASHN_DelFlag", dto.ASHN_DelFlag ?? "A");
        //                    cmd.Parameters.AddWithValue("@ASHN_Status", dto.ASHN_Status ?? "W");
        //                    cmd.Parameters.AddWithValue("@ASHN_Operation", dto.ASHN_Operation ?? "S");
        //                    cmd.Parameters.AddWithValue("@ASHN_CreatedBy", dto.ASHN_CreatedBy);
        //                    cmd.Parameters.AddWithValue("@ASHN_CreatedOn", dto.ASHN_CreatedOn);
        //                    cmd.Parameters.AddWithValue("@ASHN_CompID", dto.ASHN_CompID);
        //                    cmd.Parameters.AddWithValue("@ASHN_YearID", dto.ASHN_YearID);
        //                    cmd.Parameters.AddWithValue("@ASHN_IPAddress", dto.ASHN_IPAddress ?? string.Empty);

        //                    var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
        //                    {
        //                        Direction = ParameterDirection.Output
        //                    };
        //                    var operParam = new SqlParameter("@iOper", SqlDbType.Int)
        //                    {
        //                        Direction = ParameterDirection.Output
        //                    };

        //                    cmd.Parameters.Add(updateOrSaveParam);
        //                    cmd.Parameters.Add(operParam);

        //                    await cmd.ExecuteNonQueryAsync();

        //                    transaction.Commit();

        //                    return new int[]
        //                    {
        //                (int)(updateOrSaveParam.Value ?? 0),
        //                (int)(operParam.Value ?? 0)
        //                    };
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                transaction.Rollback();
        //                throw;
        //            }
        //        }
        //    }
        //}


        //SaveOrUpdateSubHeadingNotes(Notes For SubHeading)
        public async Task<List<SaveSubheadingDto>> SaveNotesUsingExistingSubHeadingAsync( List<SaveSubheadingDto> subheadingDtos)
        {
            // 1️⃣ Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var subheading in subheadingDtos)
                {
                    if (subheading.assH_ID <= 0)
                    {
                        string fetchSubHeadingSql = @"
                    SELECT ASSH_ID
                    FROM ACC_ScheduleSubHeading
                    WHERE ASSH_Name = @Name
                      AND ASSH_HeadingID = @HeadingID
                      AND ASSH_CompId = @CompId
                      AND ASSH_YEARId = @YearId
                      AND ASSH_DELFLG = 'N'";

                        subheading.assH_ID = await connection.ExecuteScalarAsync<int>(
                            fetchSubHeadingSql,
                            new
                            {
                                Name = subheading.assH_Name,
                                HeadingID = subheading.assH_HeadingID,
                                CompId = subheading.assH_CompId,
                                YearId = subheading.assH_YEARId
                            },
                            transaction
                        );

                        if (subheading.assH_ID <= 0)
                            throw new Exception($"SubHeading not found: {subheading.assH_Name}");
                    }

                    foreach (var note in subheading.notes)
                    {
                        using var cmdNote = new SqlCommand(
                            "spACC_SubHeadingNoteDesc",
                            connection,
                            transaction);

                        cmdNote.CommandType = CommandType.StoredProcedure;

                        cmdNote.Parameters.AddWithValue("@ASHN_ID", note.ashN_ID);
                        cmdNote.Parameters.AddWithValue("@ASHN_SubHeadingId", subheading.assH_ID); // 🔑 KEY POINT
                        cmdNote.Parameters.AddWithValue("@ASHN_CustomerId", note.ashN_CustomerId);
                        cmdNote.Parameters.AddWithValue("@ASHN_Description", note.ashN_Description ?? string.Empty);
                        cmdNote.Parameters.AddWithValue("@ASHN_DelFlag", note.ashN_DelFlag ?? "N");
                        cmdNote.Parameters.AddWithValue("@ASHN_Status", note.ashN_Status ?? "C");
                        cmdNote.Parameters.AddWithValue("@ASHN_Operation", note.ashN_Operation ?? "SAVE");
                        cmdNote.Parameters.AddWithValue("@ASHN_CreatedBy", note.ashN_CreatedBy);
                        cmdNote.Parameters.AddWithValue("@ASHN_CreatedOn",
                            note.ashN_CreatedOn == default ? DateTime.Now : note.ashN_CreatedOn);
                        cmdNote.Parameters.AddWithValue("@ASHN_CompID", note.ashN_CompID);
                        cmdNote.Parameters.AddWithValue("@ASHN_YearID", note.ashN_YearID);
                        cmdNote.Parameters.AddWithValue("@ASHN_IPAddress", note.ashN_IPAddress ?? "127.0.0.1");

                        var iUpdateOrSave = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };

                        var iOper = new SqlParameter("@iOper", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };

                        cmdNote.Parameters.Add(iUpdateOrSave);
                        cmdNote.Parameters.Add(iOper);

                        await cmdNote.ExecuteNonQueryAsync();

                        // Update note ID
                        note.ashN_ID = Convert.ToInt32(iOper.Value);
                    }
                }

                transaction.Commit();
                return subheadingDtos;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //LoadGrid(Notes For SubHeading)
        public async Task<List<SubheadingNoteLoadDto>> LoadSubheadingNotesAsync(int compId, int yearId, int custId)
        {
            // Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string sql = @"
        SELECT 
            sh.ASSH_ID AS SubHeadingId,
            sh.ASSH_Name AS SubHeadingName,
            n.ASHN_ID AS NoteId,
            n.ASHN_Description AS Description
        FROM ACC_SubHeadingNoteDesc n
        INNER JOIN ACC_ScheduleSubHeading sh
            ON n.ASHN_SubHeadingId = sh.ASSH_ID
        WHERE n.ASHN_CompID = @CompId
          AND n.ASHN_YearID = @YearId
          AND n.ASHN_CustomerId = @CustId
        ORDER BY sh.ASSH_Name, n.ASHN_ID";

            var notes = await connection.QueryAsync<SubheadingNoteLoadDto>(sql, new { CompId = compId, YearId = yearId, CustId = custId });

            return notes.ToList();
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
          AND SNF_DELFLAG = 'X'";

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
SNT_CYear_Shares,SNT_PYear_Shares,
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
          AND SNFT_DELFLAG='X'";

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
        public async Task<byte[]> GenerateScheduleNotePdfAsync(int compId, int custId, string financialYear)
        {
            try
            {
                // Get Customer Name
                var customerName = await GetCustomerNameAsync(custId);

                // Fetch datasets safely
                var datasets = new Dictionary<string, DataTable>
                {
                    ["DataSet1"] = await GetScheduleNoteFirstAsync(compId, custId, financialYear, "AU") ?? new DataTable(),
                    ["DataSet2"] = await GetScheduleNoteFirstAsync(compId, custId, financialYear, "IS") ?? new DataTable(),
                    ["DataSet3"] = await GetScheduleNoteFirstAsync(compId, custId, financialYear, "AI") ?? new DataTable(),
                    ["DataSet4"] = await GetScheduleNoteFirstAsync(compId, custId, financialYear, "BS") ?? new DataTable(),
                    ["DataSet5"] = await GetScheduleNoteFirstAsync(compId, custId, financialYear, "CC") ?? new DataTable(),
                    ["DataSet6"] = await GetScheduleNoteFirstAsync(compId, custId, financialYear, "FD") ?? new DataTable(),

                    ["ScheduleNote_Second1"] = await GetScheduleNoteSecondAsync(compId, custId, financialYear, "SF") ?? new DataTable(),
                    ["ScheduleNote_Second2"] = await GetScheduleNoteSecondAsync(compId, custId, financialYear, "SS") ?? new DataTable(),
                    ["ScheduleNote_Second3"] = await GetScheduleNoteSecondAsync(compId, custId, financialYear, "ST") ?? new DataTable(),
                    ["ScheduleNote_Second4"] = await GetScheduleNoteSecondAsync(compId, custId, financialYear, "SV") ?? new DataTable(),

                    ["ScheduleNote_Third1"] = await GetScheduleNoteThirdAsync(compId, custId, financialYear, "TBE") ?? new DataTable(),
                    ["ScheduleNote_Third2"] = await GetScheduleNoteThirdAsync(compId, custId, financialYear, "TBP") ?? new DataTable(),
                    ["ScheduleNote_Desc"] = await GetScheduleNoteDescAsync(compId, custId, financialYear, "cEquity") ?? new DataTable(),
                    ["ScheduleNote_Desc1"] = await GetScheduleNoteDescAsync(compId, custId, financialYear, "dPref") ?? new DataTable(),
                    ["ScheduleNote_Third3"] = await GetScheduleNoteThirdAsync(compId, custId, financialYear, "TEE") ?? new DataTable(),
                    ["ScheduleNote_Third4"] = await GetScheduleNoteThirdAsync(compId, custId, financialYear, "TBP") ?? new DataTable(),
                    ["ScheduleNote_Desc2"] = await GetScheduleNoteDescAsync(compId, custId, financialYear, "fShares") ?? new DataTable(),
                    ["ScheduleNote_Fourth"] = await GetScheduleNoteFourthAsync(compId, custId, financialYear, "FSC") ?? new DataTable(),
                    ["ScheduleNote_Fourth1"] = await GetScheduleNoteFourthAsync(compId, custId, financialYear, "FSP") ?? new DataTable(),
                    ["ScheduleNote_Desc3"] = await GetScheduleNoteDescAsync(compId, custId, financialYear, "footNote") ?? new DataTable(),
                };

                // 🔎 Debug: print dataset info before adding to report
                foreach (var ds in datasets)
                {
                    Console.WriteLine($"Dataset: {ds.Key}, Rows: {ds.Value.Rows.Count}");
                    foreach (DataColumn col in ds.Value.Columns)
                    {
                        Console.WriteLine($"   Column: {col.ColumnName} ({col.DataType.Name})");
                    }
                }

                // Load RDLC report
                var rdlcPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Report", "rptSchduleNote.rdlc");
                if (!File.Exists(rdlcPath))
                    throw new FileNotFoundException("RDLC file not found", rdlcPath);

                var report = new LocalReport { ReportPath = rdlcPath };

                // Add datasets to report
                foreach (var ds in datasets)
                {
                    report.DataSources.Add(new ReportDataSource(ds.Key, ds.Value));
                }

                // Add parameters
                var paramList = new List<ReportParameter>
        {
            new ReportParameter("Customer", customerName ?? custId.ToString()),
            new ReportParameter("FYear", financialYear)
        };

                if (int.TryParse(financialYear, out var fy))
                {
                    paramList.Add(new ReportParameter("CurrentYear", fy.ToString()));
                    paramList.Add(new ReportParameter("PreviousYear", (fy - 1).ToString()));
                }

                report.SetParameters(paramList);

                // Render PDF
                return report.Render("PDF");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error generating PDF: " + ex);
                throw; // rethrow so you can see full stack trace
            }
        }
        private async Task<string> GetCustomerNameAsync(int custId)
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

            var sql = "select CUST_NAME as customerName from SAD_CUSTOMER_MASTER where CUST_ID = @CustId";
            
            return await connection.ExecuteScalarAsync<string>(sql, new { CustId = custId });
        }
        private async Task<DataTable> GetScheduleNoteFirstAsync(int compId, int custId, string financialYear, string category)
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

            string sql = @"SELECT SNF_Description, SNF_CYear_Amount, SNF_PYear_Amount 
                           FROM ScheduleNote_First
                           WHERE SNF_CustID=@CustId AND SNF_CompId=@CompId 
                             AND SNF_YEARId=@financialYear AND SNF_DELFLAG='X' 
                             AND SNF_Category=@Category ORDER BY SNF_ID";

            var list = await connection.QueryAsync<dynamic>(sql, new { CustId = custId, CompId = compId, financialYear = financialYear, Category = category });

            var dt = new DataTable();
            dt.Columns.Add("SNF_Description");
            dt.Columns.Add("SNF_CYear_Amount");
            dt.Columns.Add("SNF_PYear_Amount");

            decimal totalCurr = 0, totalPrev = 0;
            foreach (var item in list)
            {
                var row = dt.NewRow();
                row["SNF_Description"] = item.SNF_Description;
                row["SNF_CYear_Amount"] = Convert.ToDecimal(item.SNF_CYear_Amount).ToString("#,##0.00");
                row["SNF_PYear_Amount"] = Convert.ToDecimal(item.SNF_PYear_Amount).ToString("#,##0.00");
                totalCurr += item.SNF_CYear_Amount;
                totalPrev += item.SNF_PYear_Amount;
                dt.Rows.Add(row);
            }

            if (dt.Rows.Count > 0)
            {
                var totalRow = dt.NewRow();
                totalRow["SNF_Description"] = "<b>Total</b>";
                totalRow["SNF_CYear_Amount"] = "<b>" + totalCurr.ToString("#,##0.00") + "</b>";
                totalRow["SNF_PYear_Amount"] = "<b>" + totalPrev.ToString("#,##0.00") + "</b>";
                dt.Rows.Add(totalRow);
            }

            return dt;
        }
        private async Task<DataTable> GetScheduleNoteSecondAsync(int compId, int custId, string financialYear, string category)
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

            string sql = @"SELECT * FROM ScheduleNote_Second
                           WHERE SNS_CustId=@CustId AND SNS_CompId=@CompId 
                             AND SNS_YEARId=@financialYear AND SNS_DELFLAG='X' 
                             AND SNS_Category=@Category ORDER BY SNS_ID";

            var list = await connection.QueryAsync<dynamic>(sql, new { CustId = custId, CompId = compId, financialYear = financialYear, Category = category });

            var dt = new DataTable();
            string[] cols = new string[] { "SNS_Description", "SNS_CYear_BegShares", "SNS_CYear_BegAmount", "SNS_PYear_BegShares", "SNS_PYear_BegAmount",
                                          "SNS_CYear_AddShares", "SNS_CYear_AddAmount", "SNS_PYear_AddShares", "SNS_PYear_AddAmount",
                                          "SNS_CYear_EndShares", "SNS_CYear_EndAmount", "SNS_PYear_EndShares", "SNS_PYear_EndAmount"};
            foreach (var c in cols) dt.Columns.Add(c);

            foreach (var item in list)
            {
                var row = dt.NewRow();
                foreach (var c in cols)
                {
                    if (((IDictionary<string, object>)item).ContainsKey(c))
                    {
                        var value = ((IDictionary<string, object>)item)[c];
                        if (value != null && decimal.TryParse(value.ToString(), out decimal parsed))
                        {
                            row[c] = parsed.ToString("#,##0.00");
                        }
                        else
                        {
                            row[c] = value?.ToString() ?? ""; // keep original string if not numeric
                        }
                    }
                    else
                    {
                        row[c] = "";
                    }
                }
                dt.Rows.Add(row);
            }

            return dt;
        }
        private async Task<DataTable> GetScheduleNoteThirdAsync(int compId, int custId, string financialYear, string category)
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

            string sql = @"SELECT SNT_Description, SNT_CYear_Shares, SNT_CYear_Amount, SNT_PYear_Shares, SNT_PYear_Amount
                           FROM ScheduleNote_Third
                           WHERE SNT_CustId=@CustId AND SNT_CompId=@CompId 
                             AND SNT_YEARId=@financialYear AND SNT_DELFLAG='X' 
                             AND SNT_Category=@Category ORDER BY SNT_ID";

            var list = await connection.QueryAsync<dynamic>(sql, new { CustId = custId, CompId = compId, financialYear = financialYear, Category = category });

            var dt = new DataTable();
            dt.Columns.Add("SNT_Description");
            dt.Columns.Add("SNT_CYear_Shares");
            dt.Columns.Add("SNT_CYear_Amount");
            dt.Columns.Add("SNT_PYear_Shares");
            dt.Columns.Add("SNT_PYear_Amount");

            foreach (var item in list)
            {
                var row = dt.NewRow();
                row["SNT_Description"] = item.SNT_Description;
                row["SNT_CYear_Shares"] = Convert.ToDecimal(item.SNT_CYear_Shares).ToString("#,##0.00");
                row["SNT_CYear_Amount"] = Convert.ToDecimal(item.SNT_CYear_Amount).ToString("#,##0.00");
                row["SNT_PYear_Shares"] = Convert.ToDecimal(item.SNT_PYear_Shares).ToString("#,##0.00");
                row["SNT_PYear_Amount"] = Convert.ToDecimal(item.SNT_PYear_Amount).ToString("#,##0.00");
                dt.Rows.Add(row);
            }
            return dt;
        }
        private async Task<DataTable> GetScheduleNoteDescAsync(int compId, int custId, string financialYear, string category)
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

            string sql = @"SELECT SND_Description FROM ScheduleNote_Desc
                           WHERE SND_CustId=@CustId AND SND_CompId=@CompId 
                             AND SND_YEARId=@financialYear AND SND_DELFLAG='X' 
                             AND SND_Category=@Category ORDER BY SND_ID";

            var list = await connection.QueryAsync<dynamic>(sql, new { CustId = custId, CompId = compId, financialYear = financialYear, Category = category });

            var dt = new DataTable();
            dt.Columns.Add("SND_Description");

            foreach (var item in list)
            {
                var row = dt.NewRow();
                row["SND_Description"] = item.SND_Description;
                dt.Rows.Add(row);
            }
            return dt;
        }
        private async Task<DataTable> GetScheduleNoteFourthAsync(int compId, int custId, string financialYear, string category)
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

            string sql = @"SELECT SNFT_Description, SNFT_NumShares, SNFT_TotalShares, SNFT_ChangedShares 
                           FROM ScheduleNote_Fourth
                           WHERE SNFT_CustId=@CustId AND SNFT_CompId=@CompId 
                             AND SNFT_YEARId=@financialYear AND SNFT_DELFLAG='X' 
                             AND SNFT_Category=@Category ORDER BY SNFT_ID";

            var list = await connection.QueryAsync<dynamic>(sql, new { CustId = custId, CompId = compId, financialYear = financialYear, Category = category });

            var dt = new DataTable();
            dt.Columns.Add("SNFT_Description");
            dt.Columns.Add("SNFT_NumShares");
            dt.Columns.Add("SNFT_TotalShares");
            dt.Columns.Add("SNFT_ChangedShares");

            foreach (var item in list)
            {
                var row = dt.NewRow();
                row["SNFT_Description"] = item.SNFT_Description;
                row["SNFT_NumShares"] = Convert.ToDecimal(item.SNFT_NumShares).ToString("#,##0.00");
                row["SNFT_TotalShares"] = Convert.ToDecimal(item.SNFT_TotalShares).ToString("#,##0.00");
                row["SNFT_ChangedShares"] = Convert.ToDecimal(item.SNFT_ChangedShares).ToString("#,##0.00");
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}






