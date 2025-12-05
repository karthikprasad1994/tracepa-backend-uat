using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Dto.FixedAssets;
using TracePca.Interface.FixedAssetsInterface;
using static TracePca.Dto.FixedAssets.AssetSetupDto;

namespace TracePca.Service.FixedAssetsService
{
    public class AssetSetupService : AssetSetupInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IWebHostEnvironment _env;
        private readonly SqlConnection _db;

        public AssetSetupService(Trdmyus1Context dbcontext, IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        //Location
        public async Task<IEnumerable<LocationDto>> GetLocationAsync(int compId, int CustId)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
                     SELECT 
                        LS_ID AS LocationId,
                    LS_Description AS LocationName
                   FROM Acc_AssetLocationSetup
                   WHERE 
                   LS_LevelCode = 0
                   AND LS_CompID = @CompId
                   AND LS_CustId = @CustId
                   ORDER BY LS_ID DESC";

            return await connection.QueryAsync<LocationDto>(query, new { CompId = compId, CustId = CustId });
        }

        //LoadDivision
        public async Task<IEnumerable<DivisionDto>> LoadDivisionAsync(int compId, int parentId, int custId)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string query;

            if (parentId == 0)
            {
                query = @"
                SELECT 
                    LS_ID AS DivisionId,
                    LS_Description AS DivisionName
                FROM Acc_AssetLocationSetup
                WHERE 
                    LS_LevelCode = 1
                    AND LS_CompID = @CompId
                    AND LS_CustId = @CustId
                ORDER BY LS_ID DESC";
            }
            else
            {
                query = @"
                SELECT 
                    LS_ID AS DivisionId,
                    LS_Description AS DivisionName
                FROM Acc_AssetLocationSetup
                WHERE 
                    LS_ParentID IN (@ParentId)
                    AND LS_LevelCode = 1
                    AND LS_CompID = @CompId
                    AND LS_CustId = @CustId
                ORDER BY LS_ID DESC";
            }

            return await connection.QueryAsync<DivisionDto>(query, new { CompId = compId, ParentId = parentId, CustId = custId });
        }


        //LoadDepartment
        public async Task<IEnumerable<DepartmentDto>> LoadDepartmentAsync(int compId, int parentId, int custId)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string query;

            if (parentId == 0)
            {
                query = @"
                SELECT 
                    LS_ID AS DepartmentId,
                    LS_Description AS DepartmentName
                FROM Acc_AssetLocationSetup
                WHERE 
                    LS_LevelCode = 2
                    AND LS_CompID = @CompId
                    AND LS_CustId = @CustId
                ORDER BY LS_ID DESC";
            }
            else
            {
                query = @"
                SELECT 
                    LS_ID AS DepartmentId,
                    LS_Description AS DepartmentName
                FROM Acc_AssetLocationSetup
                WHERE 
                    LS_LevelCode = 2
                    AND LS_ParentID IN (@ParentId)
                    AND LS_CompID = @CompId
                    AND LS_CustId = @CustId
                ORDER BY LS_ID DESC";
            }

            return await connection.QueryAsync<DepartmentDto>(query, new { CompId = compId, ParentId = parentId, CustId = custId });
        }



        //LoadBay
        public async Task<IEnumerable<BayDto>> LoadBayAsync(int compId, int parentId, int custId)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Step 3: Build query
            var query = @"
        SELECT 
            LS_ID AS BayiId,
            LS_Description AS BayiName
        FROM Acc_AssetLocationSetup
        WHERE 
            LS_LevelCode = 3
            AND LS_CompID = @CompId
            AND LS_CustId = @CustId";

            if (parentId != 0)
            {
                query += @"
            AND LS_ParentID IN (@ParentId)";
            }

            query += @"
        ORDER BY LS_ID DESC";

            return await connection.QueryAsync<BayDto>(query, new { CompId = compId, ParentId = parentId, CustId = custId });
        }


        //LoadHeading
        public async Task<IEnumerable<HeadingDto>> LoadHeadingAsync(int compId, int custId)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT 
            AM_ID AS HeadingId,
            AM_Description AS HeadingName
        FROM Acc_AssetMaster
        WHERE 
            AM_LevelCode = 0
            AND AM_CompID = @CompId
            AND AM_CustId = @CustId
        ORDER BY AM_ID DESC";

            return await connection.QueryAsync<HeadingDto>(query, new { CompId = compId, CustId = custId });
        }


        //LoadSubHeading
        public async Task<IEnumerable<SubHeadingDto>> LoadSubHeadingAsync(int compId, int parentId, int custId)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Base query
            var query = @"
        SELECT 
            AM_ID AS SubHeadingId,
            AM_Description AS SubHeadingName
        FROM Acc_AssetMaster
        WHERE 
            AM_LevelCode = 1
            AND AM_CompID = @CompId
            AND AM_CustId = @CustId";

            // Add parent filter only when parentId != 0
            if (parentId != 0)
            {
                query += @"
            AND AM_ParentID = @ParentId";
            }

            query += @"
        ORDER BY AM_ID DESC";

            return await connection.QueryAsync<SubHeadingDto>(query, new { CompId = compId, ParentId = parentId, CustId = custId });
        }


        //AssetClassUnderSubHeading(LoadItem)
        public async Task<IEnumerable<ItemDto>> LoadItemsAsync(int compId, int parentId, int custId)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string query;

            if (parentId == 0)
            {
                query = @"
            SELECT 
                AM_ID AS ItemId,
                AM_Description AS ItemName
            FROM Acc_AssetMaster
            WHERE 
                AM_LevelCode = 2
                AND AM_CompID = @CompId
                AND AM_CustId = @CustId
            ORDER BY AM_ID DESC";
            }
            else
            {
                query = @"
            SELECT 
                AM_ID AS ItemId,
                AM_Description AS ItemName
            FROM Acc_AssetMaster
            WHERE 
                AM_ParentID = @ParentId
                AND AM_LevelCode = 2
                AND AM_CompID = @CompId
                AND AM_CustId = @CustId
            ORDER BY AM_ID DESC";
            }

            return await connection.QueryAsync<ItemDto>(query, new { CompId = compId, ParentId = parentId, CustId = custId });
        }

        //SaveAsset
        public async Task<int[]> SaveAssetAsync(AssetMasterDto asset)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using (var transaction = connection.BeginTransaction())
            {
                    using (var cmd = new SqlCommand("spAcc_AssetMaster", connection, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // 🔽 INPUT PARAMETERS
                        cmd.Parameters.AddWithValue("@AM_ID", asset.AM_ID);
                        cmd.Parameters.AddWithValue("@AM_Description", asset.AM_Description ?? string.Empty);
                        cmd.Parameters.AddWithValue("@AM_Code", asset.AM_Code ?? string.Empty);
                        cmd.Parameters.AddWithValue("@AM_LevelCode", asset.AM_LevelCode);
                        cmd.Parameters.AddWithValue("@AM_ParentID", asset.AM_ParentID);
                        cmd.Parameters.AddWithValue("@AM_WDVITAct", asset.AM_WDVITAct);
                        cmd.Parameters.AddWithValue("@AM_ITRate", asset.AM_ITRate ?? string.Empty);
                        cmd.Parameters.AddWithValue("@AM_ResidualValue", asset.AM_ResidualValue);
                        cmd.Parameters.AddWithValue("@AM_CreatedBy", asset.AM_CreatedBy);
                        cmd.Parameters.AddWithValue("@AM_CreatedOn", asset.AM_CreatedOn);
                        cmd.Parameters.AddWithValue("@AM_UpdatedBy", asset.AM_UpdatedBy);
                        cmd.Parameters.AddWithValue("@AM_UpdatedOn", asset.AM_UpdatedOn);
                        cmd.Parameters.AddWithValue("@AM_DelFlag", asset.AM_DelFlag ?? "A");
                        cmd.Parameters.AddWithValue("@AM_Status", asset.AM_Status ?? "W");
                        cmd.Parameters.AddWithValue("@AM_YearID", asset.AM_YearID);
                        cmd.Parameters.AddWithValue("@AM_CompID", asset.AM_CompID);
                        cmd.Parameters.AddWithValue("@AM_CustId", asset.AM_CustId);
                        cmd.Parameters.AddWithValue("@AM_ApprovedBy", asset.AM_ApprovedBy);
                        cmd.Parameters.AddWithValue("@AM_ApprovedOn", asset.AM_ApprovedOn);
                        cmd.Parameters.AddWithValue("@AM_Opeartion", asset.AM_Opeartion ?? "S");
                        cmd.Parameters.AddWithValue("@AM_IPAddress", asset.AM_IPAddress ?? string.Empty);

                        // 🔽 OUTPUT PARAMETERS
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

                        // EXECUTE
                        await cmd.ExecuteNonQueryAsync();

                        transaction.Commit();

                        // RETURN OUTPUT VALUES
                        return new int[]
                        {
                    (int)(updateOrSaveParam.Value ?? 0),
                    (int)(operParam.Value ?? 0)
                        };
                    }
                
            }
        }

        //EditLocation
        public async Task<IEnumerable<LocationEditDto>> LoadLocationForEditAsync(int compId, int locationId)
        {
            // 1. Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            // 2. Get connection string
            string connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);

            // SQL to load Location Name & Code
            var query = @"
        SELECT 
            LS_ID AS LocationId,
            LS_Description AS LocationName,
            LS_DescCode AS LocationCode
        FROM Acc_AssetLocationSetup
        WHERE LS_ID = @LocationId
        AND LS_CompID = @CompId";

            // Execute Query
            //var result = await connection.QueryFirstOrDefaultAsync<LocationEditDto>(sql,
            //    new { LocationId = locationId, CompId = compId });

            //return (IEnumerable<LocationEditDto>)result; // returns null if not found
            return await connection.QueryAsync<LocationEditDto>(query, new { LocationId = locationId, CompId = compId });

        }

        //EditDivision
        public async Task<DivisionEditDto> LoadDivisionForEditAsync(int compId, int DivisionId)
        {
            // 1. Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            // 2. Get connection string
            string connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);

            // SQL to load Location Name & Code
            string sql = @"
        SELECT 
            LS_ID AS DivisionId,
            LS_Description AS DicisionName,
            LS_DescCode AS DivisionCode
        FROM Acc_AssetLocationSetup
        WHERE LS_ID = @DivisionId
        AND LS_CompID = @CompId";

            // Execute Query
            var result = await connection.QueryFirstOrDefaultAsync<DivisionEditDto>(sql,
                new { LocationId = DivisionId, CompId = compId });

            return result; // returns null if not found
        }

        //EditDepartment
        public async Task<DepartmentEditDto> LoadDepartmentForEditAsync(int compId, int DepartmentId)
        {
            // 1. Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            // 2. Get connection string
            string connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);

            // SQL to load Location Name & Code
            string sql = @"
        SELECT 
            LS_ID AS DepartmentId,
            LS_Description AS DepartmentName,
            LS_DescCode AS DepartmentCode
        FROM Acc_AssetLocationSetup
        WHERE LS_ID = @DepartmentId
        AND LS_CompID = @CompId";

            // Execute Query
            var result = await connection.QueryFirstOrDefaultAsync<DepartmentEditDto>(sql,
                new { LocationId = DepartmentId, CompId = compId });

            return result; // returns null if not found
        }


       

    }
}
