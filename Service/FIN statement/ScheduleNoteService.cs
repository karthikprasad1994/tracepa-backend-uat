using System.Data;
using Dapper;
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

        public ScheduleNoteService(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
        }

        //GetSubHeadingname(Notes For SubHeading)
        public async Task<IEnumerable<SubHeadingNoteDto>> GetSubHeadingDetailsAsync(int CustomerId, int SubHeadingId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
SELECT 
    ASHN_Description AS Description,
    ASHN_ID 
FROM ACC_SubHeadingNoteDesc 
LEFT JOIN ACC_ScheduleSubHeading a ON a.ASSH_ID = ASHN_SubHeadingId 
WHERE ASHN_CustomerId = @CustomerId 
  AND ASHN_SubHeadingId = @SubHeadingId";

            await connection.OpenAsync();

            return await connection.QueryAsync<SubHeadingNoteDto>(query, new
            {
                CustomerId = CustomerId,
                SubHeadingId = SubHeadingId
            });
        }

        //SaveOrUpdateSubHeadingNotes(Notes For SubHeading)
        public async Task<int[]> SaveSubHeadindNotesAsync(SubHeadingNotesDto dto)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
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
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            Mas_Id AS Branchid, 
            Mas_Description AS BranchName 
        FROM SAD_CUST_LOCATION 
        WHERE Mas_CompID = @compId AND Mas_CustID = @custId";

            await connection.OpenAsync();

            return await connection.QueryAsync<CustBranchDto>(query, new { CompId, CustId });
        }

        //GetLedger(Notes For Ledger)
        public async Task<IEnumerable<LedgerIndividualDto>> GetLedgerIndividualDetailsAsync(int CustomerId, int SubHeadingId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
SELECT 
    ASHL_Description AS Description, 
    ASHL_ID 
FROM ACC_SubHeadingLedgerDesc 
LEFT JOIN Acc_TrailBalance_Upload_Details a ON a.ATBUD_ID = ASHL_SubHeadingId 
WHERE ASHL_CustomerId = @CustomerId 
  AND ASHL_SubHeadingId = @SubHeadingId";

            await connection.OpenAsync();

            return await connection.QueryAsync<LedgerIndividualDto>(query, new
            {
                CustomerId = CustomerId,
                SubHeadingId = SubHeadingId
            });
        }

        //SaveOrUpdateLedger(Notes For Ledger)
        public async Task<int[]> SaveLedgerDetailsAsync(SubHeadingLedgerNoteDto dto)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
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
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
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
    }
}
