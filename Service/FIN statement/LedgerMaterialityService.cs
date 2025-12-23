using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Runtime.ConstrainedExecution;
using TracePca.Data;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.LedgerDifferenceDto;
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
        WHERE cmm.CMM_CompID = @CompID AND cmm.cmm_Category = 'MT'        
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

        //SaveOrUpdateContentMateriality
        public async Task<string> SaveOrUpdateContentForMTAsync(int? id, int compId, string description, string remarks, string Category)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ✅ Step 3: Check if record exists
            var existing = await connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(*) 
          FROM Content_Management_Master 
          WHERE cmm_ID = @Id AND Cmm_CompID = @CompID",
                new { Id = id, CompID = compId }
            );

            string newCode = string.Empty;

            if (existing > 0)
            {
                // ✅ Step 4A: UPDATE existing record
                var updateQuery = @"
        UPDATE Content_Management_Master
        SET cmm_Desc = @Desc,
            cms_Remarks = @Remarks,
            cmm_Delflag = @Delflag,
            CMM_Status = @Status,
            cmm_Category = @Category
        WHERE cmm_ID = @Id AND Cmm_CompID = @CompID";

                await connection.ExecuteAsync(updateQuery, new
                {
                    Id = id,
                    Desc = description,
                    Remarks = remarks,
                    Delflag = "A",
                    Status = "A",
                    CompID = compId,
                    Category = Category // ✅ Added missing parameter
                });

                newCode = $"MT_{id}";
            }
            else
            {
                var maxIdQuery = @"
        SELECT ISNULL(MAX(cmm_ID), 0) + 1 
        FROM Content_Management_Master 
        WHERE Cmm_CompID = @CompID";

                int newId = await connection.ExecuteScalarAsync<int>(maxIdQuery, new { CompID = compId });

                newCode = $"MT_{newId}";

                var insertQuery = @"
        INSERT INTO Content_Management_Master
            (cmm_ID, cmm_Code, cmm_Desc, Cmm_CompID, cms_Remarks, cmm_Delflag, CMM_Status, cmm_Category)
        VALUES
            (@Id, @Code, @Desc, @CompID, @Remarks, @Delflag, @Status, @Category)";

                await connection.ExecuteAsync(insertQuery, new
                {
                    Id = newId,
                    Code = newCode,
                    Desc = description,
                    CompID = compId,
                    Remarks = remarks,
                    Delflag = "A",
                    Status = "A",
                    Category = Category
                });
            }
            return newCode;
        }

        //GetMaterialityId
        public async Task<IEnumerable<GetMaterialityIdDto>> GetMaterialityIdAsync(int CompId, int Id)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Query using Dapper
            const string query = @"
        SELECT 
            cmm_ID,
            cmm_Desc, 
            cms_Remarks, 
            cmm_Code
        FROM Content_Management_Master
        WHERE cmm_CompID = @CompId
          AND cmm_ID = @Id
          AND cmm_Category = 'MT'";

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryAsync<GetMaterialityIdDto>(
                query,
                new { CompId, Id} // ✅ parameter names match the SQL variables
            );

            return result;
        }


        //DeleteMaterialityById
        public async Task<int> DeleteMaterialityByIdAsync(int Id)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"DELETE FROM Content_Management_Master WHERE cmm_ID = @Id";

            int rowsAffected = await connection.ExecuteAsync(query, new { Id });

            return rowsAffected; // returns number of deleted rows
        }

        //LoadDescription
        public async Task<IEnumerable<LoadDescriptionDto>> LoadDescriptionAsync(int compId, string category)
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

            // ✅ Step 4: Parameterized query to get cashflow data by category
            var sql = @"
            SELECT cmm_ID, cmm_Code, cmm_Desc, cms_Remarks
            FROM Content_Management_Master
            WHERE cmm_Category = @Category
                  AND CMM_CompID = @CompId";

            return await connection.QueryAsync<LoadDescriptionDto>(sql, new
            {
                Category = category,
                CompId = compId
            });
        }

        //public async Task<IEnumerable<DescriptionDetailsDto>> GetMaterialityBasisAsync(int compId, int custId, int branchId, int yearId, int typeId)
        //{
        //    {
        //        // ✅ Step 1: Get DB name from session
        //        string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //        if (string.IsNullOrEmpty(dbName))
        //            throw new Exception("CustomerCode is missing in session. Please log in again.");

        //        // ✅ Step 2: Get the connection string
        //        var connectionString = _configuration.GetConnectionString(dbName);

        //        // ✅ Step 3: Use SqlConnection
        //        using var connection = new SqlConnection(connectionString);
        //        await connection.OpenAsync();
        //        using var tran = connection.BeginTransaction();
        //        {
        //            string sql = string.Empty;
        //            if (typeId == 1)    // 1. Profit before tax
        //            {
        //                int headIncomeId = await GetHeadingId(connection, tran, custId, "Income");
        //                int headExpenseId = await GetHeadingId(connection, tran, custId, "Expenses");

        //                var dtIncome = await GetHeadingAmt(connection, tran, yearId, custId, 3, headIncomeId);
        //                var dtExpense = await GetHeadingAmt(connection, tran, yearId, custId, 3, headExpenseId);

        //                decimal profitCur = dtIncome.Dc1 - dtExpense.Dc1;
        //                decimal profitPrev = dtIncome.DP1 - dtExpense.DP1;



        //            }
        //            else if (typeId == 2)   // 2.Revenue from operation
        //            {
        //                int headId = await GetHeadingId(connection, tran, custId, "I Revenue from operations");

        //                var dtDetails = await GetHeadingAmt(connection, tran, yearId, custId, 3, headId);

        //                decimal profitCur = dtDetails.Dc1 - dtDetails.Dc1;                     
        //            }
        //            else if (typeId == 3)  //   3.Total asset
        //            {
        //                int headIncomeId = await GetHeadingId(connection, tran, custId, "Income");
        //                int headExpenseId = await GetHeadingId(connection, tran, custId, "Expenses");
        //                var dtIncome = await GetHeadingAmt(connection, tran, yearId, custId, 3, headIncomeId);
        //                var dtExpense = await GetHeadingAmt(connection, tran, yearId, custId, 3, headExpenseId);

        //                decimal profitCur = dtIncome.Dc1 - dtExpense.Dc1;
        //                decimal profitPrev = dtIncome.DP1 - dtExpense.DP1;
        //            }
        //            else if (typeId == 4)  //4.Total expenses
        //            {
        //                int headIncomeId = await GetHeadingId(connection, tran, custId, "Income");
        //                int headExpenseId = await GetHeadingId(connection, tran, custId, "Expenses");

        //                var dtIncome = await GetHeadingAmt(connection, tran, yearId, custId, 3, headIncomeId);
        //                var dtExpense = await GetHeadingAmt(connection, tran, yearId, custId, 3, headExpenseId);

        //                decimal profitCur = dtIncome.Dc1 - dtExpense.Dc1;
        //                decimal profitPrev = dtIncome.DP1 - dtExpense.DP1;
        //            }
        //            else if (typeId == 5) //5.Networth(Equity shares)
        //            {
        //                int headIncomeId = await GetHeadingId(connection, tran, custId, "Income");
        //                int headExpenseId = await GetHeadingId(connection, tran, custId, "Expenses");

        //                var dtIncome = await GetHeadingAmt(connection, tran, yearId, custId, 3, headIncomeId);
        //                var dtExpense = await GetHeadingAmt(connection, tran, yearId, custId, 3, headExpenseId);

        //                decimal profitCur = dtIncome.Dc1 - dtExpense.Dc1;
        //                decimal profitPrev = dtIncome.DP1 - dtExpense.DP1;
        //            }
        //            var result = await connection.QueryAsync<DescriptionDetailsDto>(sql, new
        //            {
        //                CustId = custId,
        //                BranchId = branchId,
        //                YearId = yearId,
        //                PrevYearId = yearId - 1                        
        //            });
        //            return result;


        //        }
        //    }
        //}

        public async Task<MaterialityBasisGridDto> GetMaterialityBasisAsync(
         int compId, int custId, int branchId, int yearId, int typeId)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var tran = connection.BeginTransaction();

            try
            {
                // 1️⃣ Get Org Type
                string orgTypeQuery = @"
            SELECT cmm_Desc
            FROM SAD_CUSTOMER_MASTER scm
            LEFT JOIN Content_Management_Master cmm
                ON cmm.cmm_id = scm.CUST_ORGTYPEID
            WHERE scm.CUST_ID = @CustomerId";

                string orgTypeName;

                using (var cmd = new SqlCommand(orgTypeQuery, connection, tran))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", custId);
                    var result = await cmd.ExecuteScalarAsync();
                    orgTypeName = result?.ToString() ?? "Unknown";
                }

                int headIncomeId, headExpenseId, subRevenueId, subPpeId,AssetIId,Asset2Id;

                if (orgTypeName == "Private Limited")
                {
                    headIncomeId = await GetHeadingId(connection, tran, custId, "Income");
                    headExpenseId = await GetHeadingId(connection, tran, custId, "Expenses");
                    subRevenueId = await GetSubHeadingId(connection, tran, custId, "I Revenue from operations");
                    subPpeId = await GetSubHeadingId(connection, tran, custId, "(a) (i) Property, Plant and Equipment");
                }
                else
                {
                    headIncomeId = await GetHeadingId(connection, tran, custId, "I Revenue");
                    headExpenseId = await GetHeadingId(connection, tran, custId, "II Expenses");
                    subRevenueId = await GetSubHeadingId(connection, tran, custId, "(i) Revenue from operations (net)");
                    subPpeId = await GetSubHeadingId(connection, tran, custId, "(a)  Property Plant and Equipment");
                }

                // 2️⃣ Income / Expense
                var income = await GetHeadingAmt(connection, tran, yearId, custId, 3, headIncomeId);
                var expense = await GetHeadingAmt(connection, tran, yearId, custId, 3, headExpenseId);

                // 3️⃣ Revenue
                var revenueAmt = await GetSubHeadingAmt(connection, tran, yearId, custId, 3, subRevenueId);

                decimal cyRevenue = revenueAmt.Dc1;
                decimal pyRevenue = revenueAmt.DP1;

                decimal revenuePercent =
                    pyRevenue == 0 ? 0 :
                    Math.Round(((cyRevenue - pyRevenue) / pyRevenue) * 100, 0, MidpointRounding.AwayFromZero);

                // 4️⃣ PBT
                decimal currentPBT = income.Dc1 - expense.Dc1;
                decimal previousPBT = income.DP1 - expense.DP1;

                decimal pbtPercent =
                    previousPBT == 0 ? 0 :
                    Math.Round(((currentPBT - previousPBT) / previousPBT) * 100, 0, MidpointRounding.AwayFromZero);

                decimal pbtOnRevenue =
                    cyRevenue == 0 ? 0 :
                    Math.Round((currentPBT / cyRevenue) * 100, 0, MidpointRounding.AwayFromZero);

                // 5️⃣ PPE
                var ppeAmt = await GetSubHeadingAmt(connection, tran, yearId, custId, 4, subPpeId);

                decimal ppePercent =
                    ppeAmt.DP1 == 0 ? 0 :
                    Math.Round(((ppeAmt.Dc1 - ppeAmt.DP1) / ppeAmt.DP1) * 100, 0, MidpointRounding.AwayFromZero);

                // 6️⃣ Total Assets
                AssetIId = await GetHeadingId(connection, tran, custId, "1 Non-Current Assets");
                Asset2Id = await GetHeadingId(connection, tran, custId, "2 Current Assets");

                var asset1Amt= await GetHeadingAmt(connection, tran, yearId, custId, 4, AssetIId);
                var asset2Amt = await GetHeadingAmt(connection, tran, yearId, custId, 4, Asset2Id);
                decimal currentasset = asset1Amt.Dc1 + asset2Amt.Dc1;
                decimal previousasset = asset1Amt.DP1 + asset2Amt.DP1;
                //var assetAmt = await GetTotalAssets(connection, tran, yearId, custId, subPpeId);
                var assetAmt = 0;
                decimal assetPercent =
                    assetAmt == 0 ? 0 :
                    Math.Round(((currentasset - previousasset) / previousasset) * 100, 0, MidpointRounding.AwayFromZero);

                tran.Commit();

                return new MaterialityBasisGridDto
                {
                    cyRevenue = cyRevenue,
                    pyRevenue = pyRevenue,
                    RevenuepercentChange = revenuePercent,

                    currentPBT = currentPBT,
                    previousPBT = previousPBT,
                    PBTpercentage = pbtPercent,
                    percentageRevenue = pbtOnRevenue,

                    currentPPE = ppeAmt.Dc1,
                    previoustPPE = ppeAmt.DP1,
                    PPEPercantage = ppePercent,

                    currentAsset = currentasset,
                    previoustAsset = previousasset,
                    AssetPercantage = assetPercent
                };
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        private async Task<int> GetHeadingId(SqlConnection conn, SqlTransaction tran, int customerId, string headName)
        {
            const string sql = @"SELECT ISNULL(ASH_ID,0) FROM ACC_ScheduleHeading WHERE ASH_Name = @HeadName AND ASH_OrgType = @CustId";
            return await conn.ExecuteScalarAsync<int>(sql, new { HeadName = headName, CustId = customerId }, tran);
        }

        private async Task<int> GetSubHeadingId(SqlConnection conn, SqlTransaction tran, int customerId, string subHeadName)
        {
            const string sql = @"SELECT ISNULL(ASSH_ID,0) FROM ACC_SchedulesubHeading WHERE ASSH_Name = @SubHeadName AND ASSH_OrgType = @CustId";
            return await conn.ExecuteScalarAsync<int>(sql, new { SubHeadName = subHeadName, CustId = customerId }, tran);
        }

        private async Task<ScheduleAccountingRatioDto.HeadingAmount> GetHeadingAmt(SqlConnection conn, SqlTransaction tran,
            int yearId, int customerId, int schedType, int headingId)
        {
            if (headingId == 0) return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = 0m, DP1 = 0m };

            // Ensure SQL names match your schema — adapted for safety
            var sql = @"
                SELECT 
                  ABS(ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount),0)) AS Dc1,
                  ABS(ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount),0)) AS DP1
                FROM Acc_TrailBalance_Upload_Details ud
                LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YearId = @YearId AND d.ATBU_CustId = @CustomerId and ud.atbud_yearid=@YearId
                LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YearId = @PrevYear AND e.ATBU_CustId = @CustomerId and ud.atbud_yearid=@PrevYear
                WHERE ud.ATBUD_Schedule_Type = @SchedType AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_HeadingId = @HeadingId 
            ";

            var row = await conn.QueryFirstOrDefaultAsync(sql, new { YearId = yearId, PrevYear = yearId - 1, CustomerId = customerId, SchedType = schedType, HeadingId = headingId }, tran);

            if (row == null) return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = 0m, DP1 = 0m };

            decimal dc1 = row.Dc1 == null ? 0m : Convert.ToDecimal(row.Dc1);
            decimal dp1 = row.DP1 == null ? 0m : Convert.ToDecimal(row.DP1);
            decimal percentChange = 0;
            if (dc1 > 0 && dp1 > 0)
            {
                 percentChange = +(dc1 - dp1) / dp1;
            }         
            return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = dc1, DP1 = dp1, PercentChange = percentChange };
            }

        private async Task<ScheduleAccountingRatioDto.HeadingAmount> GetSubHeadingAmt(SqlConnection conn, SqlTransaction tran,
            int yearId, int customerId, int schedType, int subHeadingId)
        {
            if (subHeadingId == 0) return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = 0m, DP1 = 0m };

            var sql = @"
                SELECT 
                  ABS(ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount),0)) AS Dc1,
                  ABS(ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount),0)) AS DP1
                FROM Acc_TrailBalance_Upload_Details ud
                LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YearId = @YearId AND d.ATBU_CustId = @CustomerId and ud.atbud_yearid=@YearId
                LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YearId = @PrevYear AND e.ATBU_CustId = @CustomerId and ud.atbud_yearid=@PrevYear
                WHERE ud.ATBUD_Schedule_Type = @SchedType AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_SubHeading = @SubHeadingId 
            ";

            var row = await conn.QueryFirstOrDefaultAsync(sql, new { YearId = yearId, PrevYear = yearId - 1, CustomerId = customerId, SchedType = schedType, SubHeadingId = subHeadingId }, tran);

            if (row == null) return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = 0m, DP1 = 0m };

            decimal dc1 = row.Dc1 == null ? 0m : Convert.ToDecimal(row.Dc1);
            decimal dp1 = row.DP1 == null ? 0m : Convert.ToDecimal(row.DP1);

            return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = dc1, DP1 = dp1 };
        }
        private async Task<decimal> GetPandLFinalAmt(SqlConnection conn, SqlTransaction tran, int yearId, int customerId, int branchId)
        {
            const string sql = @" SELECT ISNULL(SUM(ATBU_Closing_TotalDebit_Amount - ATBU_Closing_TotalCredit_Amount), 0)
                FROM Acc_TrailBalance_Upload
                WHERE ATBU_Description = 'Net Income' AND ATBU_YearId = @YearId AND ATBU_CustId = @CustomerId AND ATBU_BranchId = @BranchId ";
            var value = await conn.ExecuteScalarAsync<decimal?>(sql, new { YearId = yearId, CustomerId = customerId, BranchId = branchId }, tran);
            return value ?? 0m;
        }
        private async Task<ScheduleAccountingRatioDto.HeadingAmount> GetTotalAssets(SqlConnection conn, SqlTransaction tran, int yearId, int customerId, int branchId)
        {
            var sql = @"
                SELECT 
                  ABS(ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount),0)) AS Dc1,
                  ABS(ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount),0)) AS DP1
                FROM Acc_TrailBalance_Upload_Details ud
                LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YearId = @YearId AND d.ATBU_CustId = @CustomerId and ud.atbud_yearid=@YearId
                LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YearId = @PrevYear AND e.ATBU_CustId = @CustomerId and ud.atbud_yearid=@PrevYear
left join ACC_ScheduleTemplates on AST_Companytype=ud.atbud_custid and AST_Schedule_type=4 and AST_AccHeadId=1  and AST_Companytype=@CustomerId               
                WHERE ud.ATBUD_Schedule_Type = @SchedType AND ud.ATBUD_CustId = @CustomerId  ";
            var row = await conn.QueryFirstOrDefaultAsync(sql, new { YearId = yearId, PrevYear = yearId - 1, CustomerId = customerId, SchedType = 4, }, tran);

            if (row == null) return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = 0m, DP1 = 0m };

            decimal dc1 = row.Dc1 == null ? 0m : Convert.ToDecimal(row.Dc1);
            decimal dp1 = row.DP1 == null ? 0m : Convert.ToDecimal(row.DP1);

            return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = dc1, DP1 = dp1 };
        }
    }
}
