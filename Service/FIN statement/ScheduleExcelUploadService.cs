using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Globalization;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleExcelUploadDto;
using static TracePca.Service.FIN_statement.ScheduleExcelUploadService;

namespace TracePca.Service.FIN_statement
{
    public class ScheduleExcelUploadService : ScheduleExcelUploadInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public ScheduleExcelUploadService(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
        }

        //DownloadUploadableExcelAndTemplate
        public Dto.FIN_Statement.ScheduleExcelUploadDto.FileDownloadResult GetExcelTemplate()
        {
            var filePath = "C:\\Users\\SSD\\Desktop\\TracePa\\tracepa-dotnet-core\\SampleExcels\\SampleTrailBalExcel.xlsx";

            if (!File.Exists(filePath))
                return new Dto.FIN_Statement.ScheduleExcelUploadDto.FileDownloadResult();

            var bytes = File.ReadAllBytes(filePath);
            var fileName = "SampleScheduleTempl3teExcel3.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return new Dto.FIN_Statement.ScheduleExcelUploadDto.FileDownloadResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        //GetCustomersName
        public async Task<IEnumerable<Dto.FIN_Statement.ScheduleExcelUploadDto.CustDto>> GetCustomerNameAsync(int CompId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            Cust_Id,
            Cust_Name 
        FROM SAD_CUSTOMER_MASTER
        WHERE cust_Compid = @CompID";

            await connection.OpenAsync();

            return await connection.QueryAsync<Dto.FIN_Statement.ScheduleExcelUploadDto.CustDto>(query, new { CompID = CompId });
        }

        //GetFinancialYear
        public async Task<IEnumerable<Dto.FIN_Statement.ScheduleExcelUploadDto.FinancialYearDto>> GetFinancialYearAsync(int CompId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            YMS_YEARID AS Year_Id,
            YMS_ID As Id
        FROM YEAR_MASTER 
        WHERE YMS_FROMDATE < DATEADD(year, +1, GETDATE()) 
          AND YMS_CompId = @CompID 
        ORDER BY YMS_ID DESC";

            await connection.OpenAsync();

            return await connection.QueryAsync<Dto.FIN_Statement.ScheduleExcelUploadDto.FinancialYearDto>(query, new { CompID = CompId });
        }

        //GetDuration
        public async Task<int?> GetCustomerDurationIdAsync(int compId, int custId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var query = "SELECT Cust_DurtnId FROM SAD_CUSTOMER_MASTER WHERE CUST_CompID = @CompId AND CUST_ID = @CustId";

            var parameters = new { CompId = compId, CustId = custId };
            var result = await connection.QueryFirstOrDefaultAsync<int?>(query, parameters);

            return result;
        }

        //GetBranchName
        public async Task<IEnumerable<Dto.FIN_Statement.ScheduleExcelUploadDto.CustBranchDto>> GetBranchNameAsync(int CompId, int CustId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            Mas_Id AS Branchid, 
            Mas_Description AS BranchName 
        FROM SAD_CUST_LOCATION 
        WHERE Mas_CompID = @compId AND Mas_CustID = @custId";

            await connection.OpenAsync();

            return await connection.QueryAsync<Dto.FIN_Statement.ScheduleExcelUploadDto.CustBranchDto>(query, new { CompId, CustId });
        }

        //SaveOpeningBalance
        public async Task<List<int>> SaveOpeningBalanceAsync(List<OpeningBalanceDto> dtos)
        {
            var resultIds = new List<int>();

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection3"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var dto in dtos)
                {
                    int iPKId = dto.ATBU_ID;
                    int updateOrSave, oper;

                    // --- Save Main Upload (spAcc_TrailBalance_Upload) ---
                    using (var uploadCommand = new SqlCommand("spAcc_TrailBalance_Upload", connection, transaction))
                    {
                        uploadCommand.CommandType = CommandType.StoredProcedure;

                        uploadCommand.Parameters.AddWithValue("@ATBU_ID", dto.ATBU_ID);
                        uploadCommand.Parameters.AddWithValue("@ATBU_CODE", dto.ATBU_CODE ?? string.Empty);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Description", dto.ATBU_Description ?? string.Empty);
                        uploadCommand.Parameters.AddWithValue("@ATBU_CustId", dto.ATBU_CustId);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Opening_Debit_Amount", dto.ATBU_Opening_Debit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Opening_Credit_Amount", dto.ATBU_Opening_Credit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_TR_Debit_Amount", dto.ATBU_TR_Debit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_TR_Credit_Amount", dto.ATBU_TR_Credit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Closing_Debit_Amount", dto.ATBU_Closing_Debit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Closing_Credit_Amount", dto.ATBU_Closing_Credit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_DELFLG", dto.ATBU_DELFLG ?? "A");
                        uploadCommand.Parameters.AddWithValue("@ATBU_CRBY", dto.ATBU_CRBY);
                        uploadCommand.Parameters.AddWithValue("@ATBU_STATUS", dto.ATBU_STATUS ?? "C");
                        uploadCommand.Parameters.AddWithValue("@ATBU_UPDATEDBY", dto.ATBU_UPDATEDBY);
                        uploadCommand.Parameters.AddWithValue("@ATBU_IPAddress", dto.ATBU_IPAddress ?? string.Empty);
                        uploadCommand.Parameters.AddWithValue("@ATBU_CompId", dto.ATBU_CompId);
                        uploadCommand.Parameters.AddWithValue("@ATBU_YEARId", dto.ATBU_YEARId);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Branchid", dto.ATBU_Branchid);
                        uploadCommand.Parameters.AddWithValue("@ATBU_QuarterId", dto.ATBU_QuarterId);

                        var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        uploadCommand.Parameters.Add(updateOrSaveParam);
                        uploadCommand.Parameters.Add(operParam);

                        await uploadCommand.ExecuteNonQueryAsync();

                        updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                        oper = (int)(operParam.Value ?? 0);
                    }

                    resultIds.Add(oper);

                    if (dto.ATBU_ID == 0)
                    {
                        // --- Save Upload Details (spAcc_TrailBalance_Upload_Details) ---
                        using (var detailsCommand = new SqlCommand("spAcc_TrailBalance_Upload_Details", connection, transaction))
                        {
                            detailsCommand.CommandType = CommandType.StoredProcedure;

                            detailsCommand.Parameters.AddWithValue("@ATBUD_ID", dto.ATBUD_ID);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Masid", oper);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_CODE", dto.ATBUD_CODE ?? string.Empty);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Description", dto.ATBUD_Description ?? string.Empty);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_CustId", dto.ATBUD_CustId);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_SChedule_Type", dto.ATBUD_SChedule_Type);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Branchid", dto.ATBUD_Branchid);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_QuarterId", dto.ATBUD_QuarterId);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Company_Type", dto.ATBUD_Company_Type);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Headingid", dto.ATBUD_Headingid);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Subheading", dto.ATBUD_Subheading);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_itemid", dto.ATBUD_itemid);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Subitemid", dto.ATBUD_Subitemid);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_DELFLG", dto.ATBUD_DELFLG ?? "A");
                            detailsCommand.Parameters.AddWithValue("@ATBUD_CRBY", dto.ATBUD_CRBY);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_UPDATEDBY", dto.ATBUD_UPDATEDBY);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_STATUS", dto.ATBUD_STATUS ?? "C");
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Progress", dto.ATBUD_Progress ?? "Uploaded");
                            detailsCommand.Parameters.AddWithValue("@ATBUD_IPAddress", dto.ATBUD_IPAddress ?? string.Empty);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_CompId", dto.ATBUD_CompId);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_YEARId", dto.ATBUD_YEARId);

                            var detailOutput1 = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var detailOutput2 = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            detailsCommand.Parameters.Add(detailOutput1);
                            detailsCommand.Parameters.Add(detailOutput2);

                            await detailsCommand.ExecuteNonQueryAsync();
                        }
                    }
                }

                transaction.Commit();
                return resultIds;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error while saving Trail Balance data: " + ex.Message, ex);
            }
        }

        //SaveTrailBalance
        public async Task<List<int>> SaveTrailBalanceAsync(List<TrailBalanceDto> dtos)
        {
            var resultIds = new List<int>();

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection3"));
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var dto in dtos)
                {
                    int iPKId = dto.ATBU_ID;
                    int updateOrSave, oper;

                    // --- Save Main Upload (spAcc_TrailBalance_Upload) ---
                    using (var uploadCommand = new SqlCommand("spAcc_TrailBalance_Upload", connection, transaction))
                    {
                        uploadCommand.CommandType = CommandType.StoredProcedure;

                        uploadCommand.Parameters.AddWithValue("@ATBU_ID", dto.ATBU_ID);
                        uploadCommand.Parameters.AddWithValue("@ATBU_CODE", dto.ATBU_CODE ?? string.Empty);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Description", dto.ATBU_Description ?? string.Empty);
                        uploadCommand.Parameters.AddWithValue("@ATBU_CustId", dto.ATBU_CustId);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Opening_Debit_Amount", dto.ATBU_Opening_Debit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Opening_Credit_Amount", dto.ATBU_Opening_Credit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_TR_Debit_Amount", dto.ATBU_TR_Debit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_TR_Credit_Amount", dto.ATBU_TR_Credit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Closing_Debit_Amount", dto.ATBU_Closing_Debit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Closing_Credit_Amount", dto.ATBU_Closing_Credit_Amount);
                        uploadCommand.Parameters.AddWithValue("@ATBU_DELFLG", dto.ATBU_DELFLG ?? "A");
                        uploadCommand.Parameters.AddWithValue("@ATBU_CRBY", dto.ATBU_CRBY);
                        uploadCommand.Parameters.AddWithValue("@ATBU_STATUS", dto.ATBU_STATUS ?? "C");
                        uploadCommand.Parameters.AddWithValue("@ATBU_UPDATEDBY", dto.ATBU_UPDATEDBY);
                        uploadCommand.Parameters.AddWithValue("@ATBU_IPAddress", dto.ATBU_IPAddress ?? string.Empty);
                        uploadCommand.Parameters.AddWithValue("@ATBU_CompId", dto.ATBU_CompId);
                        uploadCommand.Parameters.AddWithValue("@ATBU_YEARId", dto.ATBU_YEARId);
                        uploadCommand.Parameters.AddWithValue("@ATBU_Branchid", dto.ATBU_Branchid);
                        uploadCommand.Parameters.AddWithValue("@ATBU_QuarterId", dto.ATBU_QuarterId);

                        var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var operParam = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        uploadCommand.Parameters.Add(updateOrSaveParam);
                        uploadCommand.Parameters.Add(operParam);

                        await uploadCommand.ExecuteNonQueryAsync();

                        updateOrSave = (int)(updateOrSaveParam.Value ?? 0);
                        oper = (int)(operParam.Value ?? 0);
                    }

                    resultIds.Add(oper);

                    if (dto.ATBU_ID == 0)
                    {
                        // --- Save Upload Details (spAcc_TrailBalance_Upload_Details) ---
                        using (var detailsCommand = new SqlCommand("spAcc_TrailBalance_Upload_Details", connection, transaction))
                        {
                            detailsCommand.CommandType = CommandType.StoredProcedure;

                            detailsCommand.Parameters.AddWithValue("@ATBUD_ID", dto.ATBUD_ID);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Masid", oper);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_CODE", dto.ATBUD_CODE ?? string.Empty);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Description", dto.ATBUD_Description ?? string.Empty);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_CustId", dto.ATBUD_CustId);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_SChedule_Type", dto.ATBUD_SChedule_Type);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Branchid", dto.ATBUD_Branchid);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_QuarterId", dto.ATBUD_QuarterId);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Company_Type", dto.ATBUD_Company_Type);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Headingid", dto.ATBUD_Headingid);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Subheading", dto.ATBUD_Subheading);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_itemid", dto.ATBUD_itemid);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Subitemid", dto.ATBUD_Subitemid);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_DELFLG", dto.ATBUD_DELFLG ?? "A");
                            detailsCommand.Parameters.AddWithValue("@ATBUD_CRBY", dto.ATBUD_CRBY);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_UPDATEDBY", dto.ATBUD_UPDATEDBY);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_STATUS", dto.ATBUD_STATUS ?? "C");
                            detailsCommand.Parameters.AddWithValue("@ATBUD_Progress", dto.ATBUD_Progress ?? "Uploaded");
                            detailsCommand.Parameters.AddWithValue("@ATBUD_IPAddress", dto.ATBUD_IPAddress ?? string.Empty);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_CompId", dto.ATBUD_CompId);
                            detailsCommand.Parameters.AddWithValue("@ATBUD_YEARId", dto.ATBUD_YEARId);

                            var detailOutput1 = new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            var detailOutput2 = new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            detailsCommand.Parameters.Add(detailOutput1);
                            detailsCommand.Parameters.Add(detailOutput2);

                            await detailsCommand.ExecuteNonQueryAsync();
                        }
                    }
                }

                transaction.Commit();
                return resultIds;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error while saving Trail Balance data: " + ex.Message, ex);
            }
        }
    }
}

