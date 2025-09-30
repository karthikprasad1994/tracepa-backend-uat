using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.LedgerMaterialityDto;
using static TracePca.Dto.FIN_Statement.ScheduleMastersDto;

namespace TracePca.Service.FIN_statement
{
    public class LedgerMaterialityService : LedgerMaterialityInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LedgerMaterialityService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        //GetMaterialityDescription
        public async Task<IEnumerable<ContentManagementDto>> GetMaterialityDescriptionAsync(int CompId, string cmmCategory)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT
              cmm_ID,
              cmm_Desc
        FROM Content_Management_Master
        WHERE CMM_CompID = @CompID
          AND cmm_Category = 'MT'
        ORDER by cmm_ID";

            return await connection.QueryAsync<ContentManagementDto>(
                query, new { CompID = CompId, Category = cmmCategory }
            );
        }

        //SaveOrUpdateLedgerMaterialityMaster
        public async Task<int[]> SaveOrUpdateLedgerMaterialityAsync(LedgerMaterialityMasterDto dto)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                if (dto.lm_ID > 0)
                {
                    // 🔹 Direct UPDATE if record exists
                    var updateQuery = @"
                UPDATE Ledger_Materiality_Master
                SET lm_LevelOfRisk = @lm_LevelOfRisk,
                    lm_Weightage = @lm_Weightage,
                    lm_Delflag = @lm_Delflag,
                    lm_Status = @lm_Status,
                    lm_UpdatedBy = @lm_UpdatedBy,
                    lm_UpdatedOn = GETDATE(),
                    lm_ApprovedBy = @lm_ApprovedBy,
                    lm_ApprovedOn = GETDATE(),
                    lm_DeletedBy = @lm_DeletedBy,
                    lm_DeletedOn = GETDATE(),
                    lm_RecallBy = @lm_RecallBy,
                    lm_RecallOn = GETDATE(),
                    lm_IPAddress = @lm_IPAddress
                WHERE lm_ID = @lm_ID AND lm_CompID = @lm_CompID";

                    await connection.ExecuteAsync(updateQuery, dto, transaction);
                    transaction.Commit();

                    // return [2 = updated, 1 = success]
                    return new int[] { 2, dto.lm_ID };
                }
                else
                {
                    // 🔹 Use stored procedure for INSERT
                    var parameters = new DynamicParameters();
                    parameters.Add("@lm_ID", dto.lm_ID);
                    parameters.Add("@lm_MaterialityId", dto.lm_MaterialityId);
                    parameters.Add("@lm_CustId", dto.lm_CustId);
                    parameters.Add("@lm_FinancialYearId", dto.lm_FinancialYearId);
                    parameters.Add("@lm_Branch", dto.lm_Branch);
                    parameters.Add("@lm_LevelOfRisk", dto.lm_LevelOfRisk);
                    parameters.Add("@lm_Weightage", dto.lm_Weightage);
                    parameters.Add("@lm_Delflag", dto.lm_Delflag ?? "");
                    parameters.Add("@lm_Status", dto.lm_Status ?? "");
                    parameters.Add("@lm_UpdatedBy", dto.lm_UpdatedBy);
                    parameters.Add("@lm_UpdatedOn", dto.lm_UpdatedOn);
                    parameters.Add("@lm_ApprovedBy", dto.lm_ApprovedBy);
                    parameters.Add("@lm_ApprovedOn", dto.lm_ApprovedOn);
                    parameters.Add("@lm_DeletedBy", dto.lm_DeletedBy);
                    parameters.Add("@lm_DeletedOn", dto.lm_DeletedOn);
                    parameters.Add("@lm_RecallBy", dto.lm_RecallBy );
                    parameters.Add("@lm_RecallOn", dto.lm_RecallOn);
                    parameters.Add("@lm_IPAddress", dto.lm_IPAddress ?? "");
                    parameters.Add("@lm_CompID", dto.lm_CompID);
                    parameters.Add("@lm_CrBy", dto.lm_CrBy);
                    parameters.Add("@lm_CrOn", dto.lm_CrOn);

                    // Output parameters
                    parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync(
                        "spLedger_Materiality_Master",
                        parameters,
                        commandType: CommandType.StoredProcedure,
                        transaction: transaction
                    );

                    transaction.Commit();

                    return new int[]
                    {
                parameters.Get<int>("@iUpdateOrSave"),
                parameters.Get<int>("@iOper")
                    };
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //GetLedgerMaterialityMaster
        public async Task<IEnumerable<GetLedgerMaterialityMasterDto>> GetLedgerMaterialityAsync(int compId, int lm_ID)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT 
            lm_ID, lm_MaterialityId, lm_CustId, lm_FinancialYearId, lm_Branch, lm_LevelOfRisk, lm_Weightage
        FROM Ledger_Materiality_Master
        WHERE lm_CompID = @CompID
        AND (@LmId IS NULL OR lm_ID = @LmId)
        ORDER BY lm_ID DESC";

            return await connection.QueryAsync<GetLedgerMaterialityMasterDto>(query, new { CompID = compId, LmId = lm_ID });
        }
    }
}
