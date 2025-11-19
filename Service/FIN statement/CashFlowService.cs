using Dapper;
using DocumentFormat.OpenXml.InkML;
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

        //DeleteCashFlowCategoryWise
        public async Task DeleteCashflowCategory1Async(int compId, int pkId, int custId, int Category)
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

            // ✅ Step 4: Check if the row is one of the first 7 fixed rows
            var fixedParticulars = new[]
            {
                "Finance Costs",
                "Expense on employee stock option scheme",
                "Bad Debts",
                "Provision for impairment of fixed assets and intangibles",
                "Depreciation and amortisation",
                "Adjustment for:",
                "Net Profit / (Loss) before extraordinary items and tax",
                "A.Cash flow from operating activities"
            };

            var rowParticular = await connection.QueryFirstOrDefaultAsync<string>(@"
            SELECT ACF_Description
            FROM Acc_Cashflow 
            WHERE ACF_Catagary = @Category
               AND ACF_pkid = @PkId
               AND ACF_Custid = @CustId 
               AND ACF_Compid = @CompId",
                new { PkId = pkId, CustId = custId, CompId = compId, Category = Category });

            if (rowParticular == null)
                throw new Exception("Row not found.");

            if (fixedParticulars.Contains(rowParticular))
                throw new Exception($"Deletion not allowed for '{rowParticular}'.");

            // ✅ Step 5: Delete the row
            var sql = @"
            DELETE FROM Acc_Cashflow
            WHERE ACF_Catagary = @Category
               AND ACF_pkid = @PkId
               AND ACF_Custid = @CustId
               AND ACF_Compid = @CompId";

            await connection.ExecuteAsync(sql, new { PkId = pkId, CustId = custId, CompId = compId, Category = Category });
        }

        //GetCashFlowID(SearchButton)
        public async Task<int> GetCashFlowParticularsIdAsync(int compId, string description, int custId, int branchId)
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

            // ✅ Step 4: Parameterized query to get the pkid
            var sql = @"
            SELECT ACF_pkid 
            FROM Acc_Cashflow 
            WHERE ACF_Description = @Description
               AND ACF_Custid = @CustId
               AND ACF_BranchId = @BranchId
               AND ACF_Compid = @CompId";

            var pkid = await connection.QueryFirstOrDefaultAsync<int?>(sql, new
            {
                Description = description,
                CustId = custId,
                BranchId = branchId,
                CompId = compId
            });

            if (!pkid.HasValue)
                throw new Exception("Cashflow row not found.");

            return pkid.Value;
        }

        //GetCashFlowForAllCategory
        public async Task<IEnumerable<CashFlowForAllCategoryDto>> GetCashFlowForAllCategoryAsync(int compId, int custId, int yearId, int branchId, int category)
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
            SELECT 
               ACF_pkid,
               ACF_Custid,
               ACF_Description, 
               ACF_Current_Amount, 
               ACF_Prev_Amount
            FROM Acc_CashFlow
            WHERE ACF_Catagary = @Category
               AND ACF_Compid = @CompId
               AND ACF_Custid = @CustId
               AND ACF_yearid = @YearId
               AND ACF_Branchid = @BranchId";

            return await connection.QueryAsync<CashFlowForAllCategoryDto>(sql, new
            {
                Category = category,
                CompId = compId,
                CustId = custId,
                YearId = yearId,
                BranchId = branchId
            });
        }

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

                // ✅ Step 4: Check if same description already exists
                var existingId = await connection.ExecuteScalarAsync<int?>(@"
            SELECT TOP 1 ACF_pkid 
            FROM Acc_CashFlow 
            WHERE ACF_Description = @ACF_Description
              AND ACF_Custid = @ACF_Custid
              AND ACF_Compid = @ACF_Compid
              AND ACF_Yearid = @ACF_Yearid
              AND ACF_Catagary = @ACF_Catagary",
                    new
                    {
                        dto.ACF_Description,
                        dto.ACF_Custid,
                        dto.ACF_Compid,
                        dto.ACF_yearid,
                        dto.ACF_Catagary
                    });

                // ✅ Prevent duplicate inserts for both standard & user-defined items
                if (existingId.HasValue && dto.ACF_pkid == 0)
                {
                    throw new Exception($"Cash flow record '{dto.ACF_Description}' already exists.");
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
                AND ACF_Yearid = @ACF_Yearid
                AND ACF_Catagary = @ACF_Catagary;";

                        await connection.ExecuteAsync(updateQuery, new
                        {
                            dto.ACF_pkid,
                            dto.ACF_Prev_Amount,
                            dto.ACF_Updatedby,
                            dto.ACF_Ipaddress,
                            dto.ACF_Custid,
                            dto.ACF_Compid,
                            dto.ACF_yearid,
                            dto.ACF_Catagary
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
                AND ACF_Yearid = @ACF_yearid
                AND ACF_Catagary = @ACF_Catagary;";

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
                            dto.ACF_yearid,
                            dto.ACF_Catagary
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

        //SaveCashFlow(Category3)
        public async Task<List<int>> SaveCashFlowCategory3Async(List<CashFlowCategory3> dtos)
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

            var resultIds = new List<int>();

            foreach (var dto in dtos)
            {
                // ✅ Step 3: Update existing record
                if (dto.ACF_pkid != 0)
                {
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
    AND ACF_Yearid = @ACF_yearid
    AND ACF_Catagary = @ACF_Catagary;";

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
                        dto.ACF_yearid,
                        dto.ACF_Catagary
                    });

                    resultIds.Add(dto.ACF_pkid);
                }
                else
                {
                    // ✅ Step 4: Insert new record
                    var insertQuery = @"
DECLARE @NextId INT;
SELECT @NextId = ISNULL(MAX(ACF_pkid), 0) + 1 FROM Acc_CashFlow;

INSERT INTO Acc_CashFlow
(ACF_pkid, ACF_Description, ACF_Custid, ACF_Branchid, ACF_Current_Amount, ACF_Prev_Amount, 
 ACF_Status, ACF_Crby, ACF_Compid, ACF_Ipaddress, ACF_Catagary, ACF_Yearid)
VALUES
(@NextId, @ACF_Description, @ACF_Custid, @ACF_Branchid, @ACF_Current_Amount, @ACF_Prev_Amount, 
 @ACF_Status, @ACF_Crby, @ACF_Compid, @ACF_Ipaddress, @ACF_Catagary, @ACF_Yearid);

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

        //SaveCashFlow(Category4)
        public async Task<List<int>> SaveCashFlowCategory4Async(List<CashFlowCategory4> dtos)
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
                "Proceeds from issue of equity shares",
                "Share application money received / (refunded)",
                "Increase / (Decrease) in Long Term Borrowings ",
                "Increase / (Decrease) in Short Term Borrowings ",
                "Interest Received on deposits/Income tax refund",
                "Insurance claims received it refund",
                "NDividend Income",
                "Finance costs"
            };

            var resultIds = new List<int>();

            foreach (var dto in dtos)
            {
                bool isStandard = standardParticulars.Contains(dto.ACF_Description ?? string.Empty);

                // ✅ Step 4: Check if same description already exists
                var existingId = await connection.ExecuteScalarAsync<int?>(@"
            SELECT TOP 1 ACF_pkid 
            FROM Acc_CashFlow 
            WHERE ACF_Description = @ACF_Description
              AND ACF_Custid = @ACF_Custid
              AND ACF_Compid = @ACF_Compid
              AND ACF_Yearid = @ACF_Yearid
              AND ACF_Catagary = @ACF_Catagary",
                    new
                    {
                        dto.ACF_Description,
                        dto.ACF_Custid,
                        dto.ACF_Compid,
                        dto.ACF_yearid,
                        dto.ACF_Catagary
                    });

                // ✅ Prevent duplicate inserts for both standard & user-defined items
                if (existingId.HasValue && dto.ACF_pkid == 0)
                {
                    throw new Exception($"Cash flow record '{dto.ACF_Description}' already exists.");
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
                AND ACF_Yearid = @ACF_Yearid
                AND ACF_Catagary = @ACF_Catagary;";

                        await connection.ExecuteAsync(updateQuery, new
                        {
                            dto.ACF_pkid,
                            dto.ACF_Prev_Amount,
                            dto.ACF_Updatedby,
                            dto.ACF_Ipaddress,
                            dto.ACF_Custid,
                            dto.ACF_Compid,
                            dto.ACF_yearid,
                            dto.ACF_Catagary
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
                AND ACF_Yearid = @ACF_yearid
                AND ACF_Catagary = @ACF_Catagary;";

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
                            dto.ACF_yearid,
                            dto.ACF_Catagary
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

        //SaveCashFlow(Category2)
        public async Task<List<int>> SaveCashFlowCategory2Async(List<CashFlowCategory2> dtos)
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
                "Changes in working capital:",
                "Adjustments for (increase) / decrease in operating assets:",
                "Inventories",
                "Trade receivables",
                "Short-term loans and advances",
                "Long-term loans and advances",
                "Other current Assets",
                "Adjustments for increase / (decrease) in operating liabilities:",
                "Trade Payables",
                "Other current liabilities",
                "Short-term provisions",
                "Operating profit / (loss) after working capital changes",
                "Cash flow from extraordinary items/prior period items - Sale of Fixed Asset",
                "Cash generated from operations"
            };

            var resultIds = new List<int>();

            foreach (var dto in dtos)
            {
                bool isStandard = standardParticulars.Contains(dto.ACF_Description ?? string.Empty);

                // ✅ Step 4: Check if same description already exists
                var existingId = await connection.ExecuteScalarAsync<int?>(@"
            SELECT TOP 1 ACF_pkid 
            FROM Acc_CashFlow 
            WHERE ACF_Description = @ACF_Description
              AND ACF_Custid = @ACF_Custid
              AND ACF_Compid = @ACF_Compid
              AND ACF_Yearid = @ACF_Yearid
              AND ACF_Catagary = @ACF_Catagary",
                    new
                    {
                        dto.ACF_Description,
                        dto.ACF_Custid,
                        dto.ACF_Compid,
                        dto.ACF_yearid,
                        dto.ACF_Catagary
                    });

                // ✅ Prevent duplicate inserts for both standard & user-defined items
                if (existingId.HasValue && dto.ACF_pkid == 0)
                {
                    throw new Exception($"Cash flow record '{dto.ACF_Description}' already exists.");
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
                AND ACF_Yearid = @ACF_Yearid
                AND ACF_Catagary = @ACF_Catagary;";

                        await connection.ExecuteAsync(updateQuery, new
                        {
                            dto.ACF_pkid,
                            dto.ACF_Prev_Amount,
                            dto.ACF_Updatedby,
                            dto.ACF_Ipaddress,
                            dto.ACF_Custid,
                            dto.ACF_Compid,
                            dto.ACF_yearid,
                            dto.ACF_Catagary
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
                AND ACF_Yearid = @ACF_yearid
                AND ACF_Catagary = @ACF_Catagary;";

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
                            dto.ACF_yearid,
                            dto.ACF_Catagary
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

        //SaveCashFlow(Category5)
        public async Task<List<int>> SaveCashFlowCategory5Async(List<CashFlowCategory5> dtos)
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
                 "Net increase / (decrease) in Cash and cash equivalents (A+B+C)",
                 "Cash and cash equivalents at begining of the year",
                 "Cash and cash equivalents at Closing of the year",
                 "Reconciliation of Cash and cash equivalents with the Balance Sheet:",
                 "Cash and cash equivalents as per Balance Sheet",
                 "Less: Bank balances not considered as Cash and cash equivalents as defined in AS 3 Cash Flow Statements (give details)",
                 "Net Cash and cash equivalents (as defined in AS 3 Cash Flow Statements)",
                 "Add: Current investments considered as part of Cash and cash equivalents (as defined in AS 3 Cash Flow Statements) (Refer Note < br/> (ii) to Note 16 Current investments)",
                 "* Comprises:",
                 "(a) Cash on hand",
                 "(b) Balances with banks - in current accounts",
                 "(b) Balances with banks - Fixed Deposits"

            };

            var resultIds = new List<int>();

            foreach (var dto in dtos)
            {
                bool isStandard = standardParticulars.Contains(dto.ACF_Description ?? string.Empty);

                // ✅ Step 4: Check if same description already exists
                var existingId = await connection.ExecuteScalarAsync<int?>(@"
            SELECT TOP 1 ACF_pkid 
            FROM Acc_CashFlow 
            WHERE ACF_Description = @ACF_Description
              AND ACF_Custid = @ACF_Custid
              AND ACF_Compid = @ACF_Compid
              AND ACF_Yearid = @ACF_Yearid
              AND ACF_Catagary = @ACF_Catagary",
                    new
                    {
                        dto.ACF_Description,
                        dto.ACF_Custid,
                        dto.ACF_Compid,
                        dto.ACF_yearid,
                        dto.ACF_Catagary
                    });

                // ✅ Prevent duplicate inserts for both standard & user-defined items
                if (existingId.HasValue && dto.ACF_pkid == 0)
                {
                    throw new Exception($"Cash flow record '{dto.ACF_Description}' already exists.");
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
                AND ACF_Yearid = @ACF_Yearid
                AND ACF_Catagary = @ACF_Catagary;";

                        await connection.ExecuteAsync(updateQuery, new
                        {
                            dto.ACF_pkid,
                            dto.ACF_Prev_Amount,
                            dto.ACF_Updatedby,
                            dto.ACF_Ipaddress,
                            dto.ACF_Custid,
                            dto.ACF_Compid,
                            dto.ACF_yearid,
                            dto.ACF_Catagary
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
                AND ACF_Yearid = @ACF_yearid
                AND ACF_Catagary = @ACF_Catagary;";

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
                            dto.ACF_yearid,
                            dto.ACF_Catagary
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

