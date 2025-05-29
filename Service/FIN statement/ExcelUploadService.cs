using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using TracePca.Data;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ExcelUploadDto;
namespace TracePca.Service.FIN_statement
{
    public class ExcelUploadService : ExcelUploadInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public ExcelUploadService(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
        }

        //DownloadUploadableExcelAndTemplate
        public FileDownloadResult GetExcelTemplate()
        {
            var filePath = "C:\\Users\\SSD\\Desktop\\TracePa\\tracepa-dotnet-core\\SampleExcels\\SampleTrailBalExcel.xlsx";

            if (!File.Exists(filePath))
                return new FileDownloadResult();

            var bytes = File.ReadAllBytes(filePath);
            var fileName = "SampleScheduleTempl3teExcel3.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return new FileDownloadResult
            {
                FileBytes = bytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        //GetCustomersName
        public async Task<IEnumerable<CustDto>> GetCustomerNameAsync(int icompId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            Cust_Id,
            Cust_Name 
        FROM SAD_CUSTOMER_MASTER
        WHERE cust_Compid = @CompID";

            await connection.OpenAsync();

            return await connection.QueryAsync<CustDto>(query, new { CompID = icompId });
        }

        //GetFinancialYear
        public async Task<IEnumerable<FinancialYearDto>> GetFinancialYearAsync(int icompId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            YMS_YEARID,
            YMS_ID 
        FROM YEAR_MASTER 
        WHERE YMS_FROMDATE < DATEADD(year, +1, GETDATE()) 
          AND YMS_CompId = @CompID 
        ORDER BY YMS_ID DESC";

            await connection.OpenAsync();

            return await connection.QueryAsync<FinancialYearDto>(query, new { CompID = icompId });
        }

        //GetDuration
        public async Task<IEnumerable<CustDurationDto>> GetDurationAsync(int compId, int custId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            ISNULL(Cust_DurtnId, 0) AS Cust_DurtnId  
        FROM SAD_CUSTOMER_MASTER 
        WHERE Cust_CompID = @compId AND cust_id = @custId";

            await connection.OpenAsync();

            return await connection.QueryAsync<CustDurationDto>(query, new { compId, custId });
        }

        //GetBranchName
        public async Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(int compId, int custId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
        SELECT 
            Mas_Id AS Branchid, 
            Mas_Description AS BranchName 
        FROM SAD_CUST_LOCATION 
        WHERE Mas_CompID = @compId AND Mas_CustID = @custId";

            await connection.OpenAsync();

            return await connection.QueryAsync<CustBranchDto>(query, new { compId, custId });
        }

        //SaveAllInformation
        public async Task<int[]> SaveAllInformationAsync(UploadExcelRequestDto request)
        {
            var results = new List<int>();

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var row in request.ExcelRows)
                {
                    int accountHeadId = row.AccountHead?.Trim().ToUpper() switch
                    {
                        "ASSETS" => 1,
                        "INCOME" => 1,
                        "EXPENSES" => 2,
                        "CAPITAL AND LIABILITIES" => 2,
                        _ => 0
                    };

                    int headingId = await GetOrSaveHeadingAsync(request, row, accountHeadId, connection, transaction);
                    int subHeadingId = await GetOrSaveSubHeadingAsync(request, row, headingId, connection, transaction);
                    int itemId = await GetOrSaveItemAsync(request, row, headingId, subHeadingId, connection, transaction);
                    int subItemId = await GetOrSaveSubItemAsync(request, row, headingId, subHeadingId, itemId, connection, transaction);

                    await SaveScheduleAsync(request, row, headingId, subHeadingId, itemId, subItemId, connection, transaction);

                    int masId = await SaveTrailBalanceUploadAsync(request, row, connection, transaction);
                    await SaveTrailBalanceUploadDetailsAsync(request, row, masId, connection, transaction);

                    results.Add(masId); // Collecting upload master ID
                }

                transaction.Commit();
            }
            catch
            { 
                transaction.Rollback();
                throw;
            }
            return results.ToArray();
        }
        //ScheduleHeading
        private async Task<int> GetOrSaveHeadingAsync(UploadExcelRequestDto request, ExcelRowDto row, int accountHeadId, SqlConnection conn, SqlTransaction tran)
        {
            // Step 1: Check if heading already exists
            string checkQuery = @"
SELECT TOP 1 ASH_ID
FROM ACC_ScheduleHeading
WHERE ASH_Name = @Heading 
  AND ASH_CompId = @CompId
  AND ASH_YEARId = @FinancialYearId
  AND Ash_scheduletype = @ScheduleType
  AND Ash_Orgtype = @CustomerId";

            using var cmd = new SqlCommand(checkQuery, conn, tran);
            cmd.Parameters.AddWithValue("@CompId", request.CompId);
            cmd.Parameters.AddWithValue("@FinancialYearId", request.FinancialYearId);
            cmd.Parameters.AddWithValue("@Heading", row.Heading ?? string.Empty);
            cmd.Parameters.AddWithValue("@ScheduleType", request.ScheduleType);
            cmd.Parameters.AddWithValue("@CustomerId", request.CustomerId);

            var headingId = (int?)await cmd.ExecuteScalarAsync() ?? 0;
            if (headingId > 0)
                return headingId;

            // Step 2: Save new heading using stored procedure
            using var saveCmd = new SqlCommand("spACC_ScheduleHeading", conn, tran)
            {
                CommandType = CommandType.StoredProcedure
            };

            saveCmd.Parameters.AddWithValue("@ASH_ID", 0);
            saveCmd.Parameters.AddWithValue("@ASH_Name", row.Heading ?? string.Empty);
            saveCmd.Parameters.AddWithValue("@ASH_DELFLG", "N");
            saveCmd.Parameters.AddWithValue("@ASH_CRBY", request.UserId);
            saveCmd.Parameters.AddWithValue("@ASH_STATUS", "A");
            saveCmd.Parameters.AddWithValue("@ASH_UPDATEDBY", 0);
            saveCmd.Parameters.AddWithValue("@ASH_IPAddress", "0.0.0.0"); // or use actual IP
            saveCmd.Parameters.AddWithValue("@ASH_CompId", request.CompId);
            saveCmd.Parameters.AddWithValue("@ASH_YEARId", request.FinancialYearId);
            saveCmd.Parameters.AddWithValue("@Ash_scheduletype", request.ScheduleType);
            saveCmd.Parameters.AddWithValue("@Ash_Orgtype", request.CustomerId);
            saveCmd.Parameters.AddWithValue("@ASH_Notes", accountHeadId);

            saveCmd.Parameters.Add("@iUpdateOrSave", SqlDbType.Int).Direction = ParameterDirection.Output;
            saveCmd.Parameters.Add("@iOper", SqlDbType.Int).Direction = ParameterDirection.Output;

            var newHeadingId = (int)(await saveCmd.ExecuteScalarAsync() ?? 0);
            return newHeadingId;
        }
        //ScheduleSub-Heading
        private async Task<int> GetOrSaveSubHeadingAsync(
    UploadExcelRequestDto request,
    ExcelRowDto row,
    int headingId,
    SqlConnection connection,
    SqlTransaction transaction)
        {
            // Check if SubHeading already exists
            string checkQuery = @"
SELECT TOP 1 ASSH_ID
FROM ACC_ScheduleSubHeading
WHERE ASSH_Name = @SubHeading
  AND ASSH_CompId = @CompId
  AND ASSH_YEARId = @FinancialYearId
  AND ASSH_scheduletype = @ScheduleType
  AND ASSH_Orgtype = @CustomerId
  AND ASSH_HeadingID = @HeadingId";

            using (var checkCmd = new SqlCommand(checkQuery, connection, transaction))
            {
                checkCmd.Parameters.AddWithValue("@CompId", request.CompId);
                checkCmd.Parameters.AddWithValue("@FinancialYearId", request.FinancialYearId);
                checkCmd.Parameters.AddWithValue("@SubHeading", row.SubHeading ?? string.Empty);
                checkCmd.Parameters.AddWithValue("@ScheduleType", request.ScheduleType);
                checkCmd.Parameters.AddWithValue("@CustomerId", request.CustomerId);
                checkCmd.Parameters.AddWithValue("@HeadingId", headingId);

                var existingIdObj = await checkCmd.ExecuteScalarAsync();
                int existingId = existingIdObj != null ? Convert.ToInt32(existingIdObj) : 0;

                if (existingId > 0)
                {
                    return existingId;
                }
            }

            // Insert new SubHeading using stored procedure that returns the ID via SELECT
            using (var saveCmd = new SqlCommand("spACC_ScheduleSubHeading", connection, transaction))
            {
                saveCmd.CommandType = CommandType.StoredProcedure;

                saveCmd.Parameters.Add(new SqlParameter("@ASSH_ID", SqlDbType.Int) { Value = 0 });
                saveCmd.Parameters.Add(new SqlParameter("@ASSH_Name", SqlDbType.VarChar, 5000) { Value = row.SubHeading ?? string.Empty });
                saveCmd.Parameters.Add(new SqlParameter("@ASSH_HeadingID", SqlDbType.Int) { Value = headingId });
                saveCmd.Parameters.Add(new SqlParameter("@ASSH_DELFLG", SqlDbType.VarChar, 1) { Value = "N" });
                saveCmd.Parameters.Add(new SqlParameter("@ASSH_CRBY", SqlDbType.Int) { Value = request.UserId });
                saveCmd.Parameters.Add(new SqlParameter("@ASSH_STATUS", SqlDbType.VarChar, 2) { Value = "A" });
                saveCmd.Parameters.Add(new SqlParameter("@ASSH_UPDATEDBY", SqlDbType.Int) { Value = 0 });
                saveCmd.Parameters.Add(new SqlParameter("@ASSH_IPAddress", SqlDbType.VarChar, 25) { Value = request.IpAddress ?? "0.0.0.0" });
                saveCmd.Parameters.Add(new SqlParameter("@ASSH_CompId", SqlDbType.Int) { Value = request.CompId });
                saveCmd.Parameters.Add(new SqlParameter("@ASSH_YEARId", SqlDbType.Int) { Value = request.FinancialYearId });
                saveCmd.Parameters.Add(new SqlParameter("@ASSH_Notes", SqlDbType.Int) { Value = headingId });
                saveCmd.Parameters.Add(new SqlParameter("@ASSH_scheduletype", SqlDbType.Int) { Value = request.ScheduleType });
                saveCmd.Parameters.Add(new SqlParameter("@ASSH_Orgtype", SqlDbType.Int) { Value = request.CustomerId });

                // OUTPUT parameters
                saveCmd.Parameters.Add(new SqlParameter("@iUpdateOrSave", SqlDbType.Int) { Direction = ParameterDirection.Output });
                saveCmd.Parameters.Add(new SqlParameter("@iOper", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await saveCmd.ExecuteNonQueryAsync();
                return Convert.ToInt32(saveCmd.Parameters["@iOper"].Value);
            }
        }
        //ScheduleItem
        private async Task<int> GetOrSaveItemAsync(
    UploadExcelRequestDto request,
    ExcelRowDto row,
    int headingId,
    int subHeadingId,
    SqlConnection connection,
    SqlTransaction transaction)
        {
            // 1. Check if Item already exists
            string checkQuery = @"
SELECT TOP 1 ASI_ID
FROM ACC_ScheduleItems
WHERE ASI_Name = @Item
  AND ASI_CompId = @CompId
  AND ASI_YEARId = @FinancialYearId
  AND ASI_scheduletype = @ScheduleType
  AND ASI_Orgtype = @CustomerId
  AND ASI_HeadingID = @HeadingId
  AND ASI_SubHeadingID = @SubHeadingId";

            using (var checkCmd = new SqlCommand(checkQuery, connection, transaction))
            {
                checkCmd.Parameters.AddWithValue("@Item", row.Item ?? string.Empty);
                checkCmd.Parameters.AddWithValue("@CompId", request.CompId);
                checkCmd.Parameters.AddWithValue("@FinancialYearId", request.FinancialYearId);
                checkCmd.Parameters.AddWithValue("@ScheduleType", request.ScheduleType);
                checkCmd.Parameters.AddWithValue("@CustomerId", request.CustomerId);
                checkCmd.Parameters.AddWithValue("@HeadingId", headingId);
                checkCmd.Parameters.AddWithValue("@SubHeadingId", subHeadingId);

                var existingIdObj = await checkCmd.ExecuteScalarAsync();
                int existingId = existingIdObj != null ? Convert.ToInt32(existingIdObj) : 0;

                if (existingId > 0)
                {
                    return existingId;
                }
            }
            // 2. Insert new Item using stored procedure and return ID
            using (var saveCmd = new SqlCommand("spACC_ScheduleItems", connection, transaction))
            {
                saveCmd.CommandType = CommandType.StoredProcedure;

                saveCmd.Parameters.AddWithValue("@ASI_ID", 0); // Insert indicator
                saveCmd.Parameters.AddWithValue("@ASI_Name", row.Item ?? string.Empty);
                saveCmd.Parameters.AddWithValue("@ASI_HeadingID", headingId);
                saveCmd.Parameters.AddWithValue("@ASI_SubHeadingID", subHeadingId);
                saveCmd.Parameters.AddWithValue("@ASI_DELFLG", "N");
                saveCmd.Parameters.AddWithValue("@ASI_CRBY", request.UserId);
                saveCmd.Parameters.AddWithValue("@ASI_STATUS", "A");
                saveCmd.Parameters.AddWithValue("@ASI_IPAddress", request.IpAddress ?? "0.0.0.0"); // ✅ Corrected name here
                saveCmd.Parameters.AddWithValue("@ASI_CompId", request.CompId);
                saveCmd.Parameters.AddWithValue("@ASI_YEARId", request.FinancialYearId);
                saveCmd.Parameters.AddWithValue("@ASI_scheduletype", request.ScheduleType);
                saveCmd.Parameters.AddWithValue("@ASI_Orgtype", request.CustomerId);

                saveCmd.Parameters.Add("@iUpdateOrSave", SqlDbType.Int).Direction = ParameterDirection.Output;
                saveCmd.Parameters.Add("@iOper", SqlDbType.Int).Direction = ParameterDirection.Output;

                await saveCmd.ExecuteNonQueryAsync();

                return Convert.ToInt32(saveCmd.Parameters["@iOper"].Value);
            }
        }
        //ScheduleSub-Item
        private async Task<int> GetOrSaveSubItemAsync(
     UploadExcelRequestDto request,
     ExcelRowDto row,
     int headingId,
     int subHeadingId,
     int itemId,
     SqlConnection connection,
     SqlTransaction transaction)
        {
            // Check if SubItem already exists
            string checkQuery = @"
SELECT TOP 1 ASSI_ID
FROM ACC_ScheduleSubItems
WHERE ASSI_CompId = @CompId
  AND ASSI_YEARId = @FinancialYearId
  AND ASSI_Name = @SubItem
  AND ASSI_scheduletype = @ScheduleType
  AND ASSI_Orgtype = @CustomerId
  AND ASSI_HeadingID = @HeadingId
  AND ASSI_SubHeadingID = @SubHeadingId
  AND ASSI_ItemsID = @ItemId";

            using (var checkCmd = new SqlCommand(checkQuery, connection, transaction))
            {
                checkCmd.Parameters.AddWithValue("@CompId", request.CompId);
                checkCmd.Parameters.AddWithValue("@FinancialYearId", request.FinancialYearId);
                checkCmd.Parameters.AddWithValue("@SubItem", row.SubItem ?? string.Empty);
                checkCmd.Parameters.AddWithValue("@ScheduleType", request.ScheduleType);
                checkCmd.Parameters.AddWithValue("@CustomerId", request.CustomerId);
                checkCmd.Parameters.AddWithValue("@HeadingId", headingId);
                checkCmd.Parameters.AddWithValue("@SubHeadingId", subHeadingId);
                checkCmd.Parameters.AddWithValue("@ItemId", itemId);

                var existingIdObj = await checkCmd.ExecuteScalarAsync();
                int existingId = existingIdObj != null ? Convert.ToInt32(existingIdObj) : 0;

                if (existingId > 0)
                {
                    return existingId;
                }
            }
            // Insert new SubItem using stored procedure, expect returned ASSI_ID via SELECT
            using (var saveCmd = new SqlCommand("spACC_ScheduleSubItems", connection, transaction))
            {
                saveCmd.CommandType = CommandType.StoredProcedure;

                saveCmd.Parameters.AddWithValue("@ASSI_ID", 0); // Indicates insert
                saveCmd.Parameters.AddWithValue("@ASSI_Name", row.SubItem ?? string.Empty);
                saveCmd.Parameters.AddWithValue("@ASSI_HeadingID", headingId);
                saveCmd.Parameters.AddWithValue("@ASSI_SubHeadingID", subHeadingId);
                saveCmd.Parameters.AddWithValue("@ASSI_ItemsID", itemId);
                saveCmd.Parameters.AddWithValue("@ASSI_DELFLG", "N");
                saveCmd.Parameters.AddWithValue("@ASSI_CRBY", request.UserId);
                saveCmd.Parameters.AddWithValue("@ASSI_STATUS", "A");
                saveCmd.Parameters.AddWithValue("@ASSI_UPDATEDBY", 0);
                saveCmd.Parameters.AddWithValue("@ASSI_IPAddress", "0.0.0.0"); // or use actual IP
                saveCmd.Parameters.AddWithValue("@ASSI_CompId", request.CompId);
                saveCmd.Parameters.AddWithValue("@ASSI_YEARId", request.FinancialYearId);
                saveCmd.Parameters.AddWithValue("@ASSI_scheduletype", request.ScheduleType);
                saveCmd.Parameters.AddWithValue("@ASSI_Orgtype", request.CustomerId);

                saveCmd.Parameters.Add("@iUpdateOrSave", SqlDbType.Int).Direction = ParameterDirection.Output;
                saveCmd.Parameters.Add("@iOper", SqlDbType.Int).Direction = ParameterDirection.Output;

                var insertedId = (int)(await saveCmd.ExecuteScalarAsync() ?? 0);
                return insertedId;
            }
        }
        //Schedule
        private async Task SaveScheduleAsync(
    UploadExcelRequestDto request,
    ExcelRowDto row,
    int HeadingId,
    int SubHeadingId,
    int ItemId,
    int SubItemId,
    SqlConnection connection,
    SqlTransaction transaction)
        {
            string query = @"
IF EXISTS (
    SELECT 1 FROM Acc_TrailBalance_Upload_Details 
    WHERE ATBUD_Headingid = @HeadingId
      AND ATBUD_Subheading = @SubHeadingId
      AND ATBUD_itemid = @ItemId
      AND ATBUD_SubItemId = @SubItemId
      AND ATBUD_CompId = @CompId
      AND ATBUD_YEARId = @YearId
      AND ATBUD_CustId = @CustomerId
      AND ATBUD_SChedule_Type = @ScheduleType
      AND ATBUD_QuarterId = @QuarterId
)
BEGIN
    UPDATE Acc_TrailBalance_Upload_Details 
    SET ATBUD_Description = @ATBUD_Description,
        ATBUD_Code = @ATBUD_CODE,
        ATBUD_ID = @ATBUD_ID,
        ATBUD_IPAddress = @ATBUD_IPAddress
    WHERE ATBUD_Headingid = @HeadingId
      AND ATBUD_Subheading = @SubHeadingId
      AND ATBUD_itemid = @ItemId
      AND ATBUD_SubItemId = @SubItemId
      AND ATBUD_CompId = @CompId
      AND ATBUD_YEARId = @YearId
      AND ATBUD_CustId = @CustomerId
      AND ATBUD_SChedule_Type = @ScheduleType
      AND ATBUD_QuarterId = @QuarterId;
END
ELSE
BEGIN
    INSERT INTO Acc_TrailBalance_Upload_Details 
    (
        ATBUD_Headingid, ATBUD_Subheading, ATBUD_itemid, ATBUD_SubItemId,
        ATBUD_Description, ATBUD_CODE, ATBUD_CompId, ATBUD_YEARId, ATBUD_CustId, ATBUD_SChedule_Type, ATBUD_QuarterId,
        ATBUD_ID, ATBUD_IPAddress
    )
    VALUES
    (
        @ATBUD_Headingid, @ATBUD_Subheading, @ATBUD_itemid, @ATBUD_SubItemId,
        @ATBUD_Description, @ATBUD_CODE, @ATBUD_CompId, @ATBUD_YEARId, @ATBUD_CustId, @ATBUD_SChedule_Type, @ATBUD_QuarterId,
        @ATBUD_ID, @ATBUD_IPAddress
    );
END";

            using var cmd = new SqlCommand(query, connection, transaction);

            cmd.Parameters.AddWithValue("@HeadingId", HeadingId);
            cmd.Parameters.AddWithValue("@SubHeadingId", SubHeadingId);
            cmd.Parameters.AddWithValue("@ItemId", ItemId);
            cmd.Parameters.AddWithValue("@SubItemId", SubItemId);
            cmd.Parameters.AddWithValue("@CompId", request.CompId);
            cmd.Parameters.AddWithValue("@YearId", request.FinancialYearId);
            cmd.Parameters.AddWithValue("@CustomerId", request.CustomerId);
            cmd.Parameters.AddWithValue("@ScheduleType", request.ScheduleType);
            cmd.Parameters.AddWithValue("@QuarterId", request.QuarterId);

            cmd.Parameters.AddWithValue("@ATBUD_Headingid", HeadingId);
            cmd.Parameters.AddWithValue("@ATBUD_Subheading", SubHeadingId);
            cmd.Parameters.AddWithValue("@ATBUD_itemid", ItemId);
            cmd.Parameters.AddWithValue("@ATBUD_SubItemId", SubItemId);
            cmd.Parameters.AddWithValue("@ATBUD_Description", row.Description ?? string.Empty);
            cmd.Parameters.AddWithValue("@ATBUD_CODE", row.DescriptionCode ?? string.Empty);
            cmd.Parameters.AddWithValue("@ATBUD_CompId", request.CompId);
            cmd.Parameters.AddWithValue("@ATBUD_YEARId", request.FinancialYearId);
            cmd.Parameters.AddWithValue("@ATBUD_CustId", request.CustomerId);
            cmd.Parameters.AddWithValue("@ATBUD_SChedule_Type", request.ScheduleType);
            cmd.Parameters.AddWithValue("@ATBUD_QuarterId", request.QuarterId);
            cmd.Parameters.AddWithValue("@ATBUD_ID", request.UserId);
            cmd.Parameters.AddWithValue("@ATBUD_IPAddress", request.IpAddress ?? string.Empty);

            await cmd.ExecuteNonQueryAsync();
        }
        //SaveTrailBalanceUpload
        private async Task<int> SaveTrailBalanceUploadAsync(
     UploadExcelRequestDto request,
     ExcelRowDto row,
     SqlConnection connection,
     SqlTransaction transaction)
        {
            using var cmd = new SqlCommand("spAcc_TrailBalance_Upload", connection, transaction)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Required input parameters
            cmd.Parameters.AddWithValue("@ATBU_ID", 0);
            cmd.Parameters.AddWithValue("@ATBU_CODE", row.DescriptionCode ?? string.Empty);
            cmd.Parameters.AddWithValue("@ATBU_Description", row.Description ?? string.Empty);
            cmd.Parameters.AddWithValue("@ATBU_CustId", request.CustomerId);
            cmd.Parameters.AddWithValue("@ATBU_Opening_Debit_Amount", row.OpeningDebit);
            cmd.Parameters.AddWithValue("@ATBU_Opening_Credit_Amount", row.OpeningCredit);
            cmd.Parameters.AddWithValue("@ATBU_TR_Debit_Amount", row.TrDebit);
            cmd.Parameters.AddWithValue("@ATBU_TR_Credit_Amount", row.TrCredit);
            cmd.Parameters.AddWithValue("@ATBU_Closing_Debit_Amount", row.ClosingDebit);
            cmd.Parameters.AddWithValue("@ATBU_Closing_Credit_Amount", row.ClosingCredit);
            cmd.Parameters.AddWithValue("@ATBU_DELFLG", "A");
            cmd.Parameters.AddWithValue("@ATBU_CRBY", request.UserId);
            cmd.Parameters.AddWithValue("@ATBU_STATUS", "C");
            cmd.Parameters.AddWithValue("@ATBU_UPDATEDBY", request.UserId);
            cmd.Parameters.AddWithValue("@ATBU_IPAddress", request.IpAddress ?? string.Empty);
            cmd.Parameters.AddWithValue("@ATBU_CompId", request.CompId);
            cmd.Parameters.AddWithValue("@ATBU_YEARId", request.FinancialYearId);
            cmd.Parameters.AddWithValue("@ATBU_Branchid", request.BranchId);
            cmd.Parameters.AddWithValue("@ATBU_QuarterId", request.QuarterId);

            // Add the missing input parameter
            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(updateOrSaveParam);

            // Add the output parameter BEFORE execution
            var outputParam = new SqlParameter("@iOper", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputParam);

            // Execute
            await cmd.ExecuteNonQueryAsync();

            return (int)outputParam.Value!;
        }
        //SaveTrailBalanceUploadDetails
        private async Task<int> SaveTrailBalanceUploadDetailsAsync(
    UploadExcelRequestDto request,
    ExcelRowDto row,
    int masId,
    SqlConnection connection,
    SqlTransaction transaction)
        {
            using var cmd = new SqlCommand("spAcc_TrailBalance_Upload_Details", connection, transaction)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@ATBUD_ID", 0);
            cmd.Parameters.AddWithValue("@ATBUD_Masid", masId);
            cmd.Parameters.AddWithValue("@ATBUD_CODE", row.DescriptionCode ?? string.Empty);
            cmd.Parameters.AddWithValue("@ATBUD_Description", row.Description ?? string.Empty);
            cmd.Parameters.AddWithValue("@ATBUD_CustId", request.CustomerId);
            cmd.Parameters.AddWithValue("@ATBUD_SChedule_Type", request.ScheduleType);
            cmd.Parameters.AddWithValue("@ATBUD_Branchid", request.BranchId);
            cmd.Parameters.AddWithValue("@ATBUD_QuarterId", request.QuarterId);
            cmd.Parameters.AddWithValue("@ATBUD_Company_Type", request.CustomerId);
            cmd.Parameters.AddWithValue("@ATBUD_Headingid", row.HeadingId);
            cmd.Parameters.AddWithValue("@ATBUD_Subheading", row.SubHeadingId);
            cmd.Parameters.AddWithValue("@ATBUD_itemid", row.ItemId);
            cmd.Parameters.AddWithValue("@ATBUD_SubItemId", row.SubItemId);
            cmd.Parameters.AddWithValue("@ATBUD_DELFLG", "A");            // Always "A"
            cmd.Parameters.AddWithValue("@ATBUD_CRBY", request.UserId);
            cmd.Parameters.AddWithValue("@ATBUD_STATUS", "C");            // Always "C"
            cmd.Parameters.AddWithValue("@ATBUD_Progress", "Uploaded");   // Always "Uploaded"
            cmd.Parameters.AddWithValue("@ATBUD_UPDATEDBY", request.UserId);
            cmd.Parameters.AddWithValue("@ATBUD_IPAddress", request.IpAddress ?? string.Empty);
            cmd.Parameters.AddWithValue("@ATBUD_CompId", request.CompId);
            cmd.Parameters.AddWithValue("@ATBUD_YEARId", request.FinancialYearId);

            // Add the missing input parameter
            var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(updateOrSaveParam);

            // Add the output parameter BEFORE execution
            var outputParam = new SqlParameter("@iOper", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputParam);

            // Execute
            await cmd.ExecuteNonQueryAsync();

            return (int)outputParam.Value!;
        }

    }
}
