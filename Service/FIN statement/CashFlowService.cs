using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TracePca.Data;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.CashFlowDto;

namespace TracePca.Service.FIN_statement
{
    public class CashFlowService : CashFlowInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CashFlowService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
        }

        //        //SaveCashFlow(Category 1)
        //        public async Task<(int UpdateOrSave, int Oper)> SaveCashFlowCategory1Async(int companyId, CashFlowCategory1 obj)
        //        {
        //            // ✅ Step 1: Get DB name from session
        //            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //            if (string.IsNullOrEmpty(dbName))
        //                throw new Exception("CustomerCode is missing in session. Please log in again.");

        //            // ✅ Step 2: Get the connection string
        //            var connectionString = _configuration.GetConnectionString(dbName);

        //            // ✅ Step 3: Use SqlConnection
        //            using var connection = new SqlConnection(connectionString);
        //            await connection.OpenAsync();

        //            {
        //                var parameters = new DynamicParameters();

        //                parameters.Add("@ACF_pkid", obj.ACF_pkid, DbType.Int32);
        //                parameters.Add("@ACF_Description", obj.ACF_Description, DbType.String, size: 5000);
        //                parameters.Add("@ACF_Custid", obj.ACF_Custid, DbType.Int32);
        //                parameters.Add("@ACF_Branchid", obj.ACF_Branchid, DbType.Int32);
        //                parameters.Add("@ACF_Current_Amount", obj.ACF_Current_Amount, DbType.Double);
        //                parameters.Add("@ACF_Prev_Amount", obj.ACF_Prev_Amount, DbType.Double);
        //                parameters.Add("@ACF_Status", obj.ACF_Status, DbType.String, size: 1);
        //                parameters.Add("@ACF_Crby", obj.ACF_Crby, DbType.Int32);
        //                parameters.Add("@ACF_Updatedby", obj.ACF_Updatedby, DbType.Int32);
        //                parameters.Add("@ACF_Compid", companyId, DbType.Int32);
        //                parameters.Add("@ACF_Ipaddress", obj.ACF_Ipaddress, DbType.String);
        //                parameters.Add("@ACF_Catagary", obj.ACF_Catagary, DbType.Int32);
        //                parameters.Add("@ACF_Yearid", obj.ACF_Yearid, DbType.Int32);
        //                parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
        //                parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //                await connection.ExecuteAsync(
        //                    "spAcc_Cashflow",
        //                    parameters,
        //                    commandType: CommandType.StoredProcedure
        //                );

        //                int updateOrSave = parameters.Get<int>("@iUpdateOrSave");
        //                int oper = parameters.Get<int>("@iOper");

        //                return (updateOrSave, oper);
        //            }
        //        }

        //        //SaveCashFlow(Category 2)
        //        public async Task<int> SaveCashFlowCategory2Async(CashFlowCategory2 dto)
        //        {
        //            // ✅ Step 1: Get DB name from session
        //            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //            if (string.IsNullOrEmpty(dbName))
        //                throw new Exception("CustomerCode is missing in session. Please log in again.");

        //            // ✅ Step 2: Get connection string
        //            var connectionString = _configuration.GetConnectionString(dbName);

        //            using var connection = new SqlConnection(connectionString);
        //            await connection.OpenAsync();

        //            if (dto.ACF_pkid != 0)
        //            {
        //                // ✅ Update existing record
        //                var updateQuery = @"
        //UPDATE Acc_CashFlow
        //SET ACF_Description   = @Description,
        //    ACF_Current_Amount = @CurrentAmount,
        //    ACF_Prev_Amount    = @PrevAmount,
        //    ACF_Status         = @Status,
        //    ACF_Updatedby      = @UserId,
        //    ACF_Ipaddress      = @IpAddress,
        //    ACF_Catagary       = @Category,
        //    ACF_yearid         = @YearId
        //WHERE ACF_pkid = @Id
        //  AND ACF_Custid = @CustomerId
        //  AND ACF_Branchid = @BranchId
        //  AND ACF_Compid = @CompanyId;";

        //                await connection.ExecuteAsync(updateQuery, new
        //                {
        //                    Id = dto.ACF_pkid,
        //                    Description = dto.ACF_Description,
        //                    CurrentAmount = dto.ACF_Current_Amount,
        //                    PrevAmount = dto.ACF_Prev_Amount,
        //                    Status = dto.ACF_Status ?? "U",
        //                    UserId = dto.ACF_Updatedby,
        //                    IpAddress = dto.ACF_Ipaddress ?? string.Empty,
        //                    Category = dto.ACF_Catagary,
        //                    YearId = dto.ACF_yearid,
        //                    CustomerId = dto.ACF_Custid,
        //                    BranchId = dto.ACF_Branchid,
        //                    CompanyId = dto.ACF_Compid
        //                });

        //                return dto.ACF_pkid;
        //            }
        //            else
        //            {
        //                // ✅ Insert new record (manual ID generation)
        //                var insertQuery = @"
        //DECLARE @NextId INT;
        //SELECT @NextId = ISNULL(MAX(ACF_pkid), 0) + 1 FROM Acc_CashFlow;

        //INSERT INTO Acc_CashFlow
        //(ACF_pkid, ACF_Custid, ACF_Branchid, ACF_Description, ACF_Current_Amount, ACF_Prev_Amount,
        // ACF_Status, ACF_Crby, ACF_Updatedby, ACF_Compid, ACF_Ipaddress, ACF_Catagary, ACF_yearid)
        //VALUES
        //(@NextId, @CustomerId, @BranchId, @Description, @CurrentAmount, @PrevAmount,
        // 'U', @UserId, @UserId, @CompanyId, @IpAddress, @Category, @YearId);

        //SELECT @NextId;";

        //                var newId = await connection.ExecuteScalarAsync<int>(insertQuery, new
        //                {
        //                    CustomerId = dto.ACF_Custid,
        //                    BranchId = dto.ACF_Branchid,
        //                    Description = dto.ACF_Description,
        //                    CurrentAmount = dto.ACF_Current_Amount,
        //                    PrevAmount = dto.ACF_Prev_Amount,
        //                    UserId = dto.ACF_Crby,
        //                    CompanyId = dto.ACF_Compid,
        //                    IpAddress = dto.ACF_Ipaddress ?? string.Empty,
        //                    Category = dto.ACF_Catagary,
        //                    YearId = dto.ACF_yearid
        //                });

        //                return newId;
        //            }
        //        }

        //        //SaveCashFlow(Category 3)
        //        public async Task<(int UpdateOrSave, int Oper)> SaveCashFlowCategory3Async(int companyId, CashFlowCategory3 obj)
        //        {
        //            // ✅ Step 1: Get DB name from session
        //            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //            if (string.IsNullOrEmpty(dbName))
        //                throw new Exception("CustomerCode is missing in session. Please log in again.");

        //            // ✅ Step 2: Get the connection string
        //            var connectionString = _configuration.GetConnectionString(dbName);

        //            // ✅ Step 3: Use SqlConnection
        //            using var connection = new SqlConnection(connectionString);
        //            await connection.OpenAsync();

        //            {
        //                var parameters = new DynamicParameters();

        //                parameters.Add("@ACF_pkid", obj.ACF_pkid, DbType.Int32);
        //                parameters.Add("@ACF_Description", obj.ACF_Description, DbType.String, size: 5000);
        //                parameters.Add("@ACF_Custid", obj.ACF_Custid, DbType.Int32);
        //                parameters.Add("@ACF_Branchid", obj.ACF_Branchid, DbType.Int32);
        //                parameters.Add("@ACF_Current_Amount", obj.ACF_Current_Amount, DbType.Double);
        //                parameters.Add("@ACF_Prev_Amount", obj.ACF_Prev_Amount, DbType.Double);
        //                parameters.Add("@ACF_Status", obj.ACF_Status, DbType.String, size: 1);
        //                parameters.Add("@ACF_Crby", obj.ACF_Crby, DbType.Int32);
        //                parameters.Add("@ACF_Updatedby", obj.ACF_Updatedby, DbType.Int32);
        //                parameters.Add("@ACF_Compid", companyId, DbType.Int32);
        //                parameters.Add("@ACF_Ipaddress", obj.ACF_Ipaddress, DbType.String);
        //                parameters.Add("@ACF_Catagary", obj.ACF_Catagary, DbType.Int32);
        //                parameters.Add("@ACF_Yearid", obj.ACF_Yearid, DbType.Int32);
        //                parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
        //                parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //                await connection.ExecuteAsync(
        //                    "spAcc_Cashflow",
        //                    parameters,
        //                    commandType: CommandType.StoredProcedure
        //                );

        //                int updateOrSave = parameters.Get<int>("@iUpdateOrSave");
        //                int oper = parameters.Get<int>("@iOper");

        //                return (updateOrSave, oper);
        //            }
        //        }

        //        //SaveCashFlow(Category 4)
        //        public async Task<(int UpdateOrSave, int Oper)> SaveCashFlowCategory4Async(int companyId, CashFlowCategory4 obj)
        //        {
        //            // ✅ Step 1: Get DB name from session
        //            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //            if (string.IsNullOrEmpty(dbName))
        //                throw new Exception("CustomerCode is missing in session. Please log in again.");

        //            // ✅ Step 2: Get the connection string
        //            var connectionString = _configuration.GetConnectionString(dbName);

        //            // ✅ Step 3: Use SqlConnection
        //            using var connection = new SqlConnection(connectionString);
        //            await connection.OpenAsync();

        //            {
        //                var parameters = new DynamicParameters();

        //                parameters.Add("@ACF_pkid", obj.ACF_pkid, DbType.Int32);
        //                parameters.Add("@ACF_Description", obj.ACF_Description, DbType.String, size: 5000);
        //                parameters.Add("@ACF_Custid", obj.ACF_Custid, DbType.Int32);
        //                parameters.Add("@ACF_Branchid", obj.ACF_Branchid, DbType.Int32);
        //                parameters.Add("@ACF_Current_Amount", obj.ACF_Current_Amount, DbType.Double);
        //                parameters.Add("@ACF_Prev_Amount", obj.ACF_Prev_Amount, DbType.Double);
        //                parameters.Add("@ACF_Status", obj.ACF_Status, DbType.String, size: 1);
        //                parameters.Add("@ACF_Crby", obj.ACF_Crby, DbType.Int32);
        //                parameters.Add("@ACF_Updatedby", obj.ACF_Updatedby, DbType.Int32);
        //                parameters.Add("@ACF_Compid", companyId, DbType.Int32);
        //                parameters.Add("@ACF_Ipaddress", obj.ACF_Ipaddress, DbType.String);
        //                parameters.Add("@ACF_Catagary", obj.ACF_Catagary, DbType.Int32);
        //                parameters.Add("@ACF_Yearid", obj.ACF_Yearid, DbType.Int32);
        //                parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
        //                parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //                await connection.ExecuteAsync(
        //                    "spAcc_Cashflow",
        //                    parameters,
        //                    commandType: CommandType.StoredProcedure
        //                );

        //                int updateOrSave = parameters.Get<int>("@iUpdateOrSave");
        //                int oper = parameters.Get<int>("@iOper");

        //                return (updateOrSave, oper);
        //            }
        //        }

        //        //SaveCashFlow(Category 5)
        //        public async Task<int> SaveCashFlowCategory5Async(CashFlowCategory5 dto)
        //        {
        //            // ✅ Step 1: Get DB name from session
        //            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //            if (string.IsNullOrEmpty(dbName))
        //                throw new Exception("CustomerCode is missing in session. Please log in again.");

        //            // ✅ Step 2: Get connection string
        //            var connectionString = _configuration.GetConnectionString(dbName);

        //            using var connection = new SqlConnection(connectionString);
        //            await connection.OpenAsync();

        //            if (dto.ACF_pkid != 0)
        //            {
        //                // ✅ Update existing record
        //                var updateQuery = @"
        //UPDATE Acc_CashFlow
        //SET ACF_Description   = @Description,
        //    ACF_Current_Amount = @CurrentAmount,
        //    ACF_Prev_Amount    = @PrevAmount,
        //    ACF_Status         = @Status,
        //    ACF_Updatedby      = @UserId,
        //    ACF_Ipaddress      = @IpAddress,
        //    ACF_Catagary       = @Category,
        //    ACF_yearid         = @YearId
        //WHERE ACF_pkid = @Id
        //  AND ACF_Custid = @CustomerId
        //  AND ACF_Branchid = @BranchId
        //  AND ACF_Compid = @CompanyId;";

        //                await connection.ExecuteAsync(updateQuery, new
        //                {
        //                    Id = dto.ACF_pkid,
        //                    Description = dto.ACF_Description,
        //                    CurrentAmount = dto.ACF_Current_Amount,
        //                    PrevAmount = dto.ACF_Prev_Amount,
        //                    Status = dto.ACF_Status ?? "U",
        //                    UserId = dto.ACF_Updatedby,
        //                    IpAddress = dto.ACF_Ipaddress ?? string.Empty,
        //                    Category = dto.ACF_Catagary,
        //                    YearId = dto.ACF_yearid,
        //                    CustomerId = dto.ACF_Custid,
        //                    BranchId = dto.ACF_Branchid,
        //                    CompanyId = dto.ACF_Compid
        //                });

        //                return dto.ACF_pkid;
        //            }
        //            else
        //            {
        //                // ✅ Insert new record (manual ID generation)
        //                var insertQuery = @"
        //DECLARE @NextId INT;
        //SELECT @NextId = ISNULL(MAX(ACF_pkid), 0) + 1 FROM Acc_CashFlow;

        //INSERT INTO Acc_CashFlow
        //(ACF_pkid, ACF_Custid, ACF_Branchid, ACF_Description, ACF_Current_Amount, ACF_Prev_Amount,
        // ACF_Status, ACF_Crby, ACF_Updatedby, ACF_Compid, ACF_Ipaddress, ACF_Catagary, ACF_yearid)
        //VALUES
        //(@NextId, @CustomerId, @BranchId, @Description, @CurrentAmount, @PrevAmount,
        // 'U', @UserId, @UserId, @CompanyId, @IpAddress, @Category, @YearId);

        //SELECT @NextId;";

        //                var newId = await connection.ExecuteScalarAsync<int>(insertQuery, new
        //                {
        //                    CustomerId = dto.ACF_Custid,
        //                    BranchId = dto.ACF_Branchid,
        //                    Description = dto.ACF_Description,
        //                    CurrentAmount = dto.ACF_Current_Amount,
        //                    PrevAmount = dto.ACF_Prev_Amount,
        //                    UserId = dto.ACF_Crby,
        //                    CompanyId = dto.ACF_Compid,
        //                    IpAddress = dto.ACF_Ipaddress ?? string.Empty,
        //                    Category = dto.ACF_Catagary,
        //                    YearId = dto.ACF_yearid
        //                });

        //                return newId;
        //            }
        //        }


        //SaveCashFlow(Category 1)
        public async Task<List<int>> SaveCashFlowCategory1Async(List<CashFlowCategory1> dtos)
        {
            if (dtos == null || dtos.Count == 0)
                throw new ArgumentException("No cash flow items provided.");

            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ✅ Step 3: Define the standard (mandatory) particulars
            var standardParticulars = new List<string>
    {
        "A.Cash flow from operating activities",
        "Adjustment for:",
        "Bad Debts",
        "Depreciation and amortisation",
        "Expense on employee stock option scheme",
        "Finance Costs",
        "Net Profit / (Loss) before extraordinary items and tax",
        "Provision for impairment of fixed assets and intangibles"
    };

            var resultIds = new List<int>();

            foreach (var dto in dtos)
            {
                bool isStandard = standardParticulars.Contains(dto.ACF_Description ?? string.Empty);

                // ✅ Step 4: Handle deletion (only allowed for user-added items)
                if (dto.IsDeleted)
                {
                    if (isStandard)
                        throw new InvalidOperationException($"'{dto.ACF_Description}' is a standard item and cannot be deleted.");

                    var deleteQuery = @"DELETE FROM Acc_CashFlow WHERE ACF_pkid = @ACF_pkid;";
                    await connection.ExecuteAsync(deleteQuery, new { dto.ACF_pkid });
                    resultIds.Add(dto.ACF_pkid);
                    continue;
                }

                // ✅ Step 5: Update existing record
                if (dto.ACF_pkid != 0)
                {
                    if (isStandard)
                    {
                        // 🟡 Standard Particular — Only Prev Year editable
                        var updateQuery = @"
        UPDATE Acc_CashFlow
        SET 
            ACF_Prev_Amount   = @ACF_Prev_Amount,
            ACF_Updatedby     = @ACF_Updatedby,
            ACF_Ipaddress     = @ACF_Ipaddress
        WHERE 
            ACF_pkid = @ACF_pkid
            AND ACF_Custid = @ACF_Custid
            AND ACF_Compid = @ACF_Compid
            AND ACF_Yearid = @ACF_Yearid;";

                        await connection.ExecuteAsync(updateQuery, new
                        {
                            dto.ACF_pkid,
                            dto.ACF_Prev_Amount,
                            dto.ACF_Updatedby,
                            dto.ACF_Ipaddress,
                            dto.ACF_Custid,
                            dto.ACF_Compid,
                            dto.ACF_yearid
                        });
                    }
                    else
                    {
                        // 🟢 User-added Particular — Allow edit of description and previous year only
                        var updateQuery = @"
        UPDATE Acc_CashFlow
        SET 
            ACF_Description   = @ACF_Description,
            ACF_Prev_Amount   = @ACF_Prev_Amount,
            ACF_Status        = @ACF_Status,
            ACF_Updatedby     = @ACF_Updatedby,
            ACF_Ipaddress     = @ACF_Ipaddress
        WHERE 
            ACF_pkid = @ACF_pkid
            AND ACF_Custid = @ACF_Custid
            AND ACF_Compid = @ACF_Compid
            AND ACF_Yearid = @ACF_yearid;";

                        await connection.ExecuteAsync(updateQuery, new
                        {
                            dto.ACF_pkid,
                            dto.ACF_Description,
                            dto.ACF_Prev_Amount,
                            dto.ACF_Status,
                            dto.ACF_Updatedby,
                            dto.ACF_Ipaddress,
                            dto.ACF_Custid,
                            dto.ACF_Compid,
                            dto.ACF_yearid
                        });
                    }

                    resultIds.Add(dto.ACF_pkid);
                }
                else
                {
                    // ✅ Step 6: Insert new record (only for user-added particulars)
                    var insertQuery = @"
    DECLARE @NextId INT;
    SELECT @NextId = ISNULL(MAX(ACF_pkid), 0) + 1 FROM Acc_CashFlow;

    INSERT INTO Acc_CashFlow
    (ACF_pkid, ACF_Description, ACF_Custid, ACF_Branchid, ACF_Current_Amount, ACF_Prev_Amount, 
    ACF_Status, ACF_Crby, ACF_Compid, ACF_Ipaddress, ACF_Catagary, ACF_Yearid )
    VALUES
    (@NextId, @ACF_Description, @ACF_Custid, @ACF_Branchid, @ACF_Current_Amount, @ACF_Prev_Amount, @ACF_Status, 
    @ACF_Crby, @ACF_Compid, @ACF_Ipaddress, @ACF_Catagary, @ACF_Yearid);

    SELECT @NextId;";

                    var newId = await connection.ExecuteScalarAsync<int>(insertQuery, new
                    {
                        dto.ACF_Description,
                        dto.ACF_Custid,
                        dto.ACF_Branchid,
                        dto.ACF_Current_Amount,
                        dto.ACF_Prev_Amount,
                        dto.ACF_Status,
                        dto.ACF_Crby,
                        dto.ACF_Compid,
                        dto.ACF_Ipaddress,
                        dto.ACF_Catagary,
                        dto.ACF_yearid
                    });

                    resultIds.Add(newId);
                }
            }

            return resultIds;
        }
    }
}
