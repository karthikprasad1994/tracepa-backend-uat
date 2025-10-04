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
        public async Task<IEnumerable<ContentManagementDto>> GetMaterialityDescriptionAsync(int CompId, string cmmCategory, int YearId, int CustId)
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
        SELECT  cmm.cmm_ID as cmm_ID,cmm.cmm_Desc as cmm_Desc, ISNULL(lm.lm_levelofrisk, 0) AS lm_levelofrisk,
    ISNULL(lm.lm_weightage, 0) AS lm_weightage, lm_Id
        FROM Content_Management_Master cmm
LEFT JOIN Ledger_Materiality_Master lm  ON lm.lm_MaterialityId = cmm.cmm_ID AND lm.lm_CustId = @iCustId  AND lm.lm_FinancialYearId = @iYearId
        WHERE cmm.CMM_CompID = @CompID
          AND cmm.cmm_Category = 'MT'
        ORDER by cmm.cmm_ID";

            return await connection.QueryAsync<ContentManagementDto>(
                query, new { CompID = CompId, Category = cmmCategory, iYearId = YearId, iCustId = CustId }
            );
        }

        //SaveOrUpdateLedgerMaterialityMaster
        public async Task<List<int[]>> SaveOrUpdateLedgerMaterialityAsync(IEnumerable<LedgerMaterialityMasterDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                throw new ArgumentException("No data provided to save or update.");

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            var results = new List<int[]>();

            try
            {
                foreach (var dto in dtos)
                {
                    if (dto.lm_ID > 0)
                    {
                        // 🔹 UPDATE existing record
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

                        // Add result [2 = updated, 1 = success]
                        results.Add(new int[] { 2, dto.lm_ID });
                    }
                    else
                    {
                        // 🔹 INSERT using stored procedure
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
                        parameters.Add("@lm_RecallBy", dto.lm_RecallBy);
                        parameters.Add("@lm_RecallOn", dto.lm_RecallOn);
                        parameters.Add("@lm_IPAddress", dto.lm_IPAddress ?? "");
                        parameters.Add("@lm_CompID", dto.lm_CompID);
                        parameters.Add("@lm_CrBy", dto.lm_CrBy);
                        parameters.Add("@lm_CrOn", dto.lm_CrOn);

                        parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                        parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        await connection.ExecuteAsync(
                            "spLedger_Materiality_Master",
                            parameters,
                            commandType: CommandType.StoredProcedure,
                            transaction: transaction
                        );
                        results.Add(new int[]
                        {
                    parameters.Get<int>("@iUpdateOrSave"),
                    parameters.Get<int>("@iOper")
                        });
                    }
                }
                transaction.Commit();
                return results;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //GenerateIDButtonForContentMaterialityMaster
        public async Task<string> GenerateAndInsertContentForMTAsync(int compId, string description)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ✅ Step 4: Fetch MaxID from Content_Management_Master for MT
            var maxIdQuery = @"
        SELECT ISNULL(MAX(cmm_ID), 0) + 1 
        FROM Content_Management_Master 
        WHERE Cmm_CompID = @CompID";

            int maxId = await connection.ExecuteScalarAsync<int>(maxIdQuery, new { CompID = compId });

            // ✅ Step 5: Build Code for MT only
            string newCode = $"MT_{maxId}";

            // ✅ Step 6: Insert new record into database
            var insertQuery = @"
        INSERT INTO Content_Management_Master
            (cmm_ID, cmm_Code, cmm_Desc, Cmm_CompID, cmm_Delflag)
        VALUES
            (@Id, @Code, @Desc, @CompID, @Delflag)";

            await connection.ExecuteAsync(insertQuery, new
            {
                Id = maxId,
                Code = newCode,
                Desc = description,
                CompID = compId,
                Delflag = "N" // default value
            });

            // ✅ Step 7: Return the newly generated code
            return newCode;
        }
    }
}
