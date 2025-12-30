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

        ////Location
        //public async Task<IEnumerable<LocationDto>> GetLocationAsync(int compId, int CustId)
        //{
        //    // Step 1: Get DB name from session
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    // Step 2: Get connection string dynamically
        //    var connectionString = _configuration.GetConnectionString(dbName);

        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    var query = @"
        //             SELECT 
        //                LS_ID AS LocationId,
        //            LS_Description AS LocationName
        //           FROM Acc_AssetLocationSetup
        //           WHERE 
        //           LS_LevelCode = 0
        //           AND LS_CompID = @CompId
        //           AND LS_CustId = @CustId
        //           ORDER BY LS_ID DESC";

        //    return await connection.QueryAsync<LocationDto>(query, new { CompId = compId, CustId = CustId });
        //}

        ////LoadDivision
        //public async Task<IEnumerable<DivisionDto>> LoadDivisionAsync(int compId, int parentId, int custId)
        //{
        //    // Step 1: Get DB name from session
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    // Step 2: Get connection string dynamically
        //    var connectionString = _configuration.GetConnectionString(dbName);

        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    string query;

        //    if (parentId == 0)
        //    {
        //        query = @"
        //        SELECT 
        //            LS_ID AS DivisionId,
        //            LS_Description AS DivisionName
        //        FROM Acc_AssetLocationSetup
        //        WHERE 
        //            LS_LevelCode = 1
        //            AND LS_CompID = @CompId
        //            AND LS_CustId = @CustId
        //        ORDER BY LS_ID DESC";
        //    }
        //    else
        //    {
        //        query = @"
        //        SELECT 
        //            LS_ID AS DivisionId,
        //            LS_Description AS DivisionName
        //        FROM Acc_AssetLocationSetup
        //        WHERE 
        //            LS_ParentID IN (@ParentId)
        //            AND LS_LevelCode = 1
        //            AND LS_CompID = @CompId
        //            AND LS_CustId = @CustId
        //        ORDER BY LS_ID DESC";
        //    }

        //    return await connection.QueryAsync<DivisionDto>(query, new { CompId = compId, ParentId = parentId, CustId = custId });
        //}


        ////LoadDepartment
        //public async Task<IEnumerable<DepartmentDto>> LoadDepartmentAsync(int compId, int parentId, int custId)
        //{
        //    // Step 1: Get DB name from session
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    // Step 2: Get connection string dynamically
        //    var connectionString = _configuration.GetConnectionString(dbName);

        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    string query;

        //    if (parentId == 0)
        //    {
        //        query = @"
        //        SELECT 
        //            LS_ID AS DepartmentId,
        //            LS_Description AS DepartmentName
        //        FROM Acc_AssetLocationSetup
        //        WHERE 
        //            LS_LevelCode = 2
        //            AND LS_CompID = @CompId
        //            AND LS_CustId = @CustId
        //        ORDER BY LS_ID DESC";
        //    }
        //    else
        //    {
        //        query = @"
        //        SELECT 
        //            LS_ID AS DepartmentId,
        //            LS_Description AS DepartmentName
        //        FROM Acc_AssetLocationSetup
        //        WHERE 
        //            LS_LevelCode = 2
        //            AND LS_ParentID IN (@ParentId)
        //            AND LS_CompID = @CompId
        //            AND LS_CustId = @CustId
        //        ORDER BY LS_ID DESC";
        //    }

        //    return await connection.QueryAsync<DepartmentDto>(query, new { CompId = compId, ParentId = parentId, CustId = custId });
        //}



        ////LoadBay
        //public async Task<IEnumerable<BayDto>> LoadBayAsync(int compId, int parentId, int custId)
        //{
        //    // Step 1: Get DB name from session
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    // Step 2: Get connection string dynamically
        //    var connectionString = _configuration.GetConnectionString(dbName);

        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    // Step 3: Build query
        //    var query = @"
        //SELECT 
        //    LS_ID AS BayiId,
        //    LS_Description AS BayiName
        //FROM Acc_AssetLocationSetup
        //WHERE 
        //    LS_LevelCode = 3
        //    AND LS_CompID = @CompId
        //    AND LS_CustId = @CustId";

        //    if (parentId != 0)
        //    {
        //        query += @"
        //    AND LS_ParentID IN (@ParentId)";
        //    }

        //    query += @"
        //ORDER BY LS_ID DESC";

        //    return await connection.QueryAsync<BayDto>(query, new { CompId = compId, ParentId = parentId, CustId = custId });
        //}


        ////LoadHeading
        //public async Task<IEnumerable<HeadingDto>> LoadHeadingAsync(int compId, int custId)
        //{
        //    // Step 1: Get DB name from session
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    // Step 2: Get connection string dynamically
        //    var connectionString = _configuration.GetConnectionString(dbName);

        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    var query = @"
        //SELECT 
        //    AM_ID AS HeadingId,
        //    AM_Description AS HeadingName
        //FROM Acc_AssetMaster
        //WHERE 
        //    AM_LevelCode = 0
        //    AND AM_CompID = @CompId
        //    AND AM_CustId = @CustId
        //ORDER BY AM_ID DESC";

        //    return await connection.QueryAsync<HeadingDto>(query, new { CompId = compId, CustId = custId });
        //}


        ////LoadSubHeading
        //public async Task<IEnumerable<SubHeadingDto>> LoadSubHeadingAsync(int compId, int parentId, int custId)
        //{
        //    // Step 1: Get DB name from session
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    // Step 2: Get connection string dynamically
        //    var connectionString = _configuration.GetConnectionString(dbName);

        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    // Base query
        //    var query = @"
        //SELECT 
        //    AM_ID AS SubHeadingId,
        //    AM_Description AS SubHeadingName
        //FROM Acc_AssetMaster
        //WHERE 
        //    AM_LevelCode = 1
        //    AND AM_CompID = @CompId
        //    AND AM_CustId = @CustId";

        //    // Add parent filter only when parentId != 0
        //    if (parentId != 0)
        //    {
        //        query += @"
        //    AND AM_ParentID = @ParentId";
        //    }

        //    query += @"
        //ORDER BY AM_ID DESC";

        //    return await connection.QueryAsync<SubHeadingDto>(query, new { CompId = compId, ParentId = parentId, CustId = custId });
        //}


        ////AssetClassUnderSubHeading(LoadItem)
        //public async Task<IEnumerable<ItemDto>> LoadItemsAsync(int compId, int parentId, int custId)
        //{
        //    // Step 1: Get DB name from session
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    // Step 2: Get connection string dynamically
        //    var connectionString = _configuration.GetConnectionString(dbName);

        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    string query;

        //    if (parentId == 0)
        //    {
        //        query = @"
        //    SELECT 
        //        AM_ID AS ItemId,
        //        AM_Description AS ItemName
        //    FROM Acc_AssetMaster
        //    WHERE 
        //        AM_LevelCode = 2
        //        AND AM_CompID = @CompId
        //        AND AM_CustId = @CustId
        //    ORDER BY AM_ID DESC";
        //    }
        //    else
        //    {
        //        query = @"
        //    SELECT 
        //        AM_ID AS ItemId,
        //        AM_Description AS ItemName
        //    FROM Acc_AssetMaster
        //    WHERE 
        //        AM_ParentID = @ParentId
        //        AND AM_LevelCode = 2
        //        AND AM_CompID = @CompId
        //        AND AM_CustId = @CustId
        //    ORDER BY AM_ID DESC";
        //    }

        //    return await connection.QueryAsync<ItemDto>(query, new { CompId = compId, ParentId = parentId, CustId = custId });
        //}

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

        // //EditLocation
        // public async Task<IEnumerable<LocationEditDto>> LoadLocationForEditAsync(int compId, int locationId)
        // {
        //     // 1. Get DB name from session
        //     string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //     if (string.IsNullOrEmpty(dbName))
        //         throw new Exception("CustomerCode is missing in session.");

        //     // 2. Get connection string
        //     string connectionString = _configuration.GetConnectionString(dbName);

        //     using var connection = new SqlConnection(connectionString);

        //     // SQL to load Location Name & Code
        //     var query = @"
        // SELECT 
        //     LS_ID AS LocationId,
        //     LS_Description AS LocationName,
        //     LS_DescCode AS LocationCode
        // FROM Acc_AssetLocationSetup
        // WHERE LS_ID = @LocationId
        // AND LS_CompID = @CompId";

        //     // Execute Query
        //     //var result = await connection.QueryFirstOrDefaultAsync<LocationEditDto>(sql,
        //     //    new { LocationId = locationId, CompId = compId });

        //     //return (IEnumerable<LocationEditDto>)result; // returns null if not found
        //     return await connection.QueryAsync<LocationEditDto>(query, new { LocationId = locationId, CompId = compId });
        // }

        // //EditDivision
        // public async Task<IEnumerable<DivisionEditDto>> LoadDivisionForEditAsync(int compId, int DivisionId)
        // {
        //     // 1. Get DB name from session
        //     string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //     if (string.IsNullOrEmpty(dbName))
        //         throw new Exception("CustomerCode is missing in session.");

        //     // 2. Get connection string
        //     string connectionString = _configuration.GetConnectionString(dbName);

        //     using var connection = new SqlConnection(connectionString);

        //     // SQL to load Location Name & Code
        //     var query = @"
        // SELECT 
        //     LS_ID AS DivisionId,
        //     LS_Description AS DivisionName,
        //     LS_DescCode AS DivisionCode
        // FROM Acc_AssetLocationSetup
        // WHERE LS_ID = @DivisionId
        // AND LS_CompID = @CompId";

        //     return await connection.QueryAsync<DivisionEditDto>(query, new { DivisionId = DivisionId, CompId = compId });
        // }


        // //EditDepartment
        // public async Task<IEnumerable<DepartmentEditDto>> LoadDepartmentForEditAsync(int compId, int DepartmentId)
        // {
        //     // 1. Get DB name from session
        //     string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //     if (string.IsNullOrEmpty(dbName))
        //         throw new Exception("CustomerCode is missing in session.");

        //     // 2. Get connection string
        //     string connectionString = _configuration.GetConnectionString(dbName);

        //     using var connection = new SqlConnection(connectionString);

        //     // SQL to load Location Name & Code
        //     var query = @"
        // SELECT 
        //     LS_ID AS DepartmentId,
        //     LS_Description AS DepartmentName,
        //     LS_DescCode AS DepartmentCode
        // FROM Acc_AssetLocationSetup
        // WHERE LS_ID = @DepartmentId
        // AND LS_CompID = @CompId";

        //     return await connection.QueryAsync<DepartmentEditDto>(query, new { DepartmentId = DepartmentId, CompId = compId });
        // }

        // //EditBay
        // public async Task<IEnumerable<BayEditDto>> LoadBayForEditAsync(int compId, int bayId)
        // {
        //     // 1. Get DB name from session
        //     string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //     if (string.IsNullOrEmpty(dbName))
        //         throw new Exception("CustomerCode is missing in session.");

        //     // 2. Get connection string
        //     string connectionString = _configuration.GetConnectionString(dbName);

        //     using var connection = new SqlConnection(connectionString);

        //     // 3. SQL Query (same style as your Location code)
        //     var query = @"
        // SELECT 
        //     LS_ID AS BayId,
        //     LS_Description AS BayName,
        //     LS_DescCode AS BayCode
        // FROM Acc_AssetLocationSetup
        // WHERE LS_ID = @BayId
        // AND LS_CompID = @CompId";

        //     // 4. Execute in same format
        //     return await connection.QueryAsync<BayEditDto>(query,
        //         new { BayId = bayId, CompId = compId });
        // }

        // //EditHeading
        // public async Task<IEnumerable<HeadingEditDto>> HeadingForEditAsync(int compId, int HeadingId)
        // {
        //     // 1. Get DB name from session
        //     string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //     if (string.IsNullOrEmpty(dbName))
        //         throw new Exception("CustomerCode is missing in session.");

        //     // 2. Get connection string
        //     string connectionString = _configuration.GetConnectionString(dbName);

        //     using var connection = new SqlConnection(connectionString);

        //     // SQL to load Location Name & Code
        //     var query = @"
        // SELECT 
        //     AM_ID AS HeadingId,
        //     AM_Description AS HeadingName
        // FROM Acc_AssetMaster
        // WHERE AM_ID = @HeadingId
        // AND AM_CompID = @CompId";

        //     return await connection.QueryAsync<HeadingEditDto>(query, new { HeadingId = HeadingId, CompId = compId });
        // }

        //// EditSubHeading
        // public async Task<IEnumerable<SubHeadingEditDto>> SubHeadingForEditAsync(int compId, int SubHeadingId)
        // {
        //     // 1. Get DB name from session
        //     string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //     if (string.IsNullOrEmpty(dbName))
        //         throw new Exception("CustomerCode is missing in session.");

        //     // 2. Get connection string
        //     string connectionString = _configuration.GetConnectionString(dbName);

        //     using var connection = new SqlConnection(connectionString);

        //     // SQL to load Location Name & Code
        //     var query = @"
        // SELECT 
        //     AM_ID AS SubHeadingId,
        //     AM_Description AS SubHeadingName
        // FROM Acc_AssetMaster
        // WHERE AM_ID = @SubHeadingId
        // AND AM_CompID = @CompId";

        //     return await connection.QueryAsync<SubHeadingEditDto>(query, new { SubHeadingId = SubHeadingId, CompId = compId });
        // }

        // ////EditAssetClassUnderSubHeading(LoadItem)
        // public async Task<IEnumerable<ItemEditDto>> ItemForEditAsync(int compId, int ItemId)
        // {
        //     // 1. Get DB name from session
        //     string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //     if (string.IsNullOrEmpty(dbName))
        //         throw new Exception("CustomerCode is missing in session.");

        //     // 2. Get connection string
        //     string connectionString = _configuration.GetConnectionString(dbName);

        //     using var connection = new SqlConnection(connectionString);

        //     // SQL to load Location Name & Code
        //     var query = @"
        // SELECT 
        //     AM_ID AS ItemId,
        //     AM_Description AS ItemName
        // FROM Acc_AssetMaster
        // WHERE AM_ID = @ItemId
        // AND AM_CompID = @CompId";

        //     return await connection.QueryAsync<ItemEditDto>(query, new { ItemId = ItemId, CompId = compId });
        // }

        //SaveLocation
        public async Task<int[]> SaveLocationSetupAsync(LocationSetupDto dto)
        {
            // 1. Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            // 2. Get connection string
            string connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                var parameters = new DynamicParameters();

                // ➤ INPUT PARAMETERS (same order as VB.NET)
                parameters.Add("@LS_ID", dto.LS_ID);
                parameters.Add("@LS_Description", dto.LS_Description);
                parameters.Add("@LS_DescCode", dto.LS_DescCode);
                parameters.Add("@LS_Code", dto.LS_Code);
                parameters.Add("@LS_LevelCode", dto.LS_LevelCode);
                parameters.Add("@LS_ParentID", dto.LS_ParentID);
                parameters.Add("@LS_CreatedBy", dto.LS_CreatedBy);
                parameters.Add("@LS_CreatedOn", dto.LS_CreatedOn);
                parameters.Add("@LS_UpdatedBy", dto.LS_UpdatedBy);
                parameters.Add("@LS_UpdatedOn", dto.LS_UpdatedOn);
                parameters.Add("@LS_DelFlag", dto.LS_DelFlag);
                parameters.Add("@LS_Status", dto.LS_Status);
                parameters.Add("@LS_YearID", dto.LS_YearID);
                parameters.Add("@LS_CompID", dto.LS_CompID);
                parameters.Add("@LS_CustId", dto.LS_CustId);
                parameters.Add("@LS_ApprovedBy", dto.LS_ApprovedBy);
                parameters.Add("@LS_ApprovedOn", dto.LS_ApprovedOn);
                parameters.Add("@LS_Opeartion", dto.LS_Opeartion);
                parameters.Add("@LS_IPAddress", dto.LS_IPAddress);

                // ➤ OUTPUT PARAMETERS
                parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // EXECUTE
                await connection.ExecuteAsync(
                    "spAcc_AssetLocationSetup",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    transaction: transaction
                );

                transaction.Commit();

                // RETURN OUTPUT VALUES
                return new int[]
                {
             parameters.Get<int>("@iUpdateOrSave"),
             parameters.Get<int>("@iOper")
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //SaveDivision
        public async Task<int[]> SaveDivisionAsync(SaveDivisionDto dto)
        {
            // 1. Read DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            // 2. Get connection string
            string connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            var parameters = new DynamicParameters();

            // INPUT PARAMETERS (Same as VB.NET)
            parameters.Add("@LS_ID", dto.LS_ID);
            parameters.Add("@LS_Description", dto.LS_Description);
            parameters.Add("@LS_DescCode", dto.LS_DescCode);
            parameters.Add("@LS_Code", dto.LS_Code);
            parameters.Add("@LS_LevelCode", dto.LS_LevelCode);
            parameters.Add("@LS_ParentID", dto.LS_ParentID);
            parameters.Add("@LS_CreatedBy", dto.LS_CreatedBy);
            parameters.Add("@LS_CreatedOn", dto.LS_CreatedOn);
            parameters.Add("@LS_UpdatedBy", dto.LS_UpdatedBy);
            parameters.Add("@LS_UpdatedOn", dto.LS_UpdatedOn);
            parameters.Add("@LS_DelFlag", dto.LS_DelFlag);
            parameters.Add("@LS_Status", dto.LS_Status);
            parameters.Add("@LS_YearID", dto.LS_YearID);
            parameters.Add("@LS_CompID", dto.LS_CompID);
            parameters.Add("@LS_CustId", dto.LS_CustId);
            parameters.Add("@LS_ApprovedBy", dto.LS_ApprovedBy);
            parameters.Add("@LS_ApprovedOn", dto.LS_ApprovedOn);
            parameters.Add("@LS_Opeartion", dto.LS_Opeartion);
            parameters.Add("@LS_IPAddress", dto.LS_IPAddress);

            // OUTPUT parameters
            parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

            // EXECUTE stored procedure
            await connection.ExecuteAsync(
                "spAcc_AssetLocationSetup",
                parameters,
                commandType: CommandType.StoredProcedure,
                transaction: transaction
            );

            transaction.Commit();

            // RETURN OUTPUT VALUES (same as VB.NET Arr)
            return new int[]
            {
             parameters.Get<int>("@iUpdateOrSave"),
             parameters.Get<int>("@iOper")
            };
        }

        //SaveDepartment
        public async Task<int[]> SaveDepartmentAsync(SaveDepartmentDto dto)
        {
            // 1. Read DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            // 2. Get connection string
            string connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            var parameters = new DynamicParameters();

            // INPUT PARAMETERS
            parameters.Add("@LS_ID", dto.LS_ID);
            parameters.Add("@LS_Description", dto.LS_Description);
            parameters.Add("@LS_DescCode", dto.LS_DescCode);
            parameters.Add("@LS_Code", dto.LS_Code);
            parameters.Add("@LS_LevelCode", dto.LS_LevelCode);
            parameters.Add("@LS_ParentID", dto.LS_ParentID);
            parameters.Add("@LS_CreatedBy", dto.LS_CreatedBy);
            parameters.Add("@LS_CreatedOn", dto.LS_CreatedOn);
            parameters.Add("@LS_UpdatedBy", dto.LS_UpdatedBy);
            parameters.Add("@LS_UpdatedOn", dto.LS_UpdatedOn);
            parameters.Add("@LS_DelFlag", dto.LS_DelFlag);
            parameters.Add("@LS_Status", dto.LS_Status);
            parameters.Add("@LS_YearID", dto.LS_YearID);
            parameters.Add("@LS_CompID", dto.LS_CompID);
            parameters.Add("@LS_CustId", dto.LS_CustId);
            parameters.Add("@LS_ApprovedBy", dto.LS_ApprovedBy);
            parameters.Add("@LS_ApprovedOn", dto.LS_ApprovedOn);
            parameters.Add("@LS_Opeartion", dto.LS_Opeartion);
            parameters.Add("@LS_IPAddress", dto.LS_IPAddress);

            // OUTPUT PARAMETERS
            parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

            // EXECUTE STORED PROCEDURE
            await connection.ExecuteAsync(
                "spAcc_AssetLocationSetup",
                parameters,
                commandType: CommandType.StoredProcedure,
                transaction: transaction
            );

            transaction.Commit();

            // RETURN OUTPUT VALUES
            return new int[]
            {
             parameters.Get<int>("@iUpdateOrSave"),
             parameters.Get<int>("@iOper")
            };
        }

        //SaveBay
        public async Task<int[]> SaveBayAsync(SaveBayDto dto)
        {
            // 1. Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            // 2. Get connection string
            string connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                var parameters = new DynamicParameters();

                // INPUT PARAMETERS
                parameters.Add("@LS_ID", dto.LS_ID);
                parameters.Add("@LS_Description", dto.LS_Description);
                parameters.Add("@LS_DescCode", dto.LS_DescCode);
                parameters.Add("@LS_Code", dto.LS_Code);
                parameters.Add("@LS_LevelCode", dto.LS_LevelCode);
                parameters.Add("@LS_ParentID", dto.LS_ParentID);
                parameters.Add("@LS_CreatedBy", dto.LS_CreatedBy);
                parameters.Add("@LS_CreatedOn", dto.LS_CreatedOn);
                parameters.Add("@LS_UpdatedBy", dto.LS_UpdatedBy);
                parameters.Add("@LS_UpdatedOn", dto.LS_UpdatedOn);
                parameters.Add("@LS_DelFlag", dto.LS_DelFlag);
                parameters.Add("@LS_Status", dto.LS_Status);
                parameters.Add("@LS_YearID", dto.LS_YearID);
                parameters.Add("@LS_CompID", dto.LS_CompID);
                parameters.Add("@LS_CustId", dto.LS_CustId);
                parameters.Add("@LS_ApprovedBy", dto.LS_ApprovedBy);
                parameters.Add("@LS_ApprovedOn", dto.LS_ApprovedOn);
                parameters.Add("@LS_Opeartion", dto.LS_Opeartion);
                parameters.Add("@LS_IPAddress", dto.LS_IPAddress);

                // OUTPUT PARAMETERS
                parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // EXECUTE STORED PROCEDURE
                await connection.ExecuteAsync(
                    "spAcc_AssetLocationSetup",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    transaction: transaction
                );

                transaction.Commit();

                // RETURN OUTPUT VALUES
                return new int[]
                {
             parameters.Get<int>("@iUpdateOrSave"),
             parameters.Get<int>("@iOper")
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //SaveHeading
        public async Task<int[]> SaveHeadingAsync(SaveHeadingDto dto)
        {
            // 1. Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            // 2. Get connection string
            string connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            var parameters = new DynamicParameters();

            // INPUT PARAMETERS (converted from VB)
            parameters.Add("@AM_ID", dto.AM_ID);
            parameters.Add("@AM_Description", dto.AM_Description);
            parameters.Add("@AM_Code", dto.AM_Code);
            parameters.Add("@AM_LevelCode", dto.AM_LevelCode);
            parameters.Add("@AM_ParentID", dto.AM_ParentID);
            parameters.Add("@AM_WDVITAct", dto.AM_WDVITAct);
            parameters.Add("@AM_ITRate", dto.AM_ITRate);
            parameters.Add("@AM_ResidualValue", dto.AM_ResidualValue);
            parameters.Add("@AM_CreatedBy", dto.AM_CreatedBy);
            parameters.Add("@AM_CreatedOn", dto.AM_CreatedOn);
            parameters.Add("@AM_UpdatedBy", dto.AM_UpdatedBy);
            parameters.Add("@AM_UpdatedOn", dto.AM_UpdatedOn);
            parameters.Add("@AM_DelFlag", dto.AM_DelFlag);
            parameters.Add("@AM_Status", dto.AM_Status);
            parameters.Add("@AM_YearID", dto.AM_YearID);
            parameters.Add("@AM_CompID", dto.AM_CompID);
            parameters.Add("@AM_CustId", dto.AM_CustId);
            parameters.Add("@AM_ApprovedBy", dto.AM_ApprovedBy);
            parameters.Add("@AM_ApprovedOn", dto.AM_ApprovedOn);
            parameters.Add("@AM_Opeartion", dto.AM_Opeartion);
            parameters.Add("@AM_IPAddress", dto.AM_IPAddress);

            // OUTPUT PARAMETERS
            parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

            // EXECUTE STORED PROCEDURE
            await connection.ExecuteAsync(
                "spAcc_AssetMaster",
                parameters,
                commandType: CommandType.StoredProcedure,
                transaction: transaction
            );

            transaction.Commit();

            // RETURN OUTPUT VALUES
            return new int[]
            {
             parameters.Get<int>("@iUpdateOrSave"),
             parameters.Get<int>("@iOper")
            };
        }

        //SaveSubHeading
        public async Task<int[]> SaveSubHeadingAsync(SaveSubHeadingDto dto)
        {
            // 1. Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            // 2. Get connection string
            string connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            var parameters = new DynamicParameters();

            // ==== INPUT PARAMETERS (same as VB.NET) ====

            parameters.Add("@AM_ID", dto.AM_ID);
            parameters.Add("@AM_Description", dto.AM_Description);
            parameters.Add("@AM_Code", dto.AM_Code);
            parameters.Add("@AM_LevelCode", dto.AM_LevelCode);
            parameters.Add("@AM_ParentID", dto.AM_ParentID);
            parameters.Add("@AM_WDVITAct", dto.AM_WDVITAct);
            parameters.Add("@AM_ITRate", dto.AM_ITRate);
            parameters.Add("@AM_ResidualValue", dto.AM_ResidualValue);
            parameters.Add("@AM_CreatedBy", dto.AM_CreatedBy);
            parameters.Add("@AM_CreatedOn", dto.AM_CreatedOn);
            parameters.Add("@AM_UpdatedBy", dto.AM_UpdatedBy);
            parameters.Add("@AM_UpdatedOn", dto.AM_UpdatedOn);
            parameters.Add("@AM_DelFlag", dto.AM_DelFlag);
            parameters.Add("@AM_Status", dto.AM_Status);
            parameters.Add("@AM_YearID", dto.AM_YearID);
            parameters.Add("@AM_CompID", dto.AM_CompID);
            parameters.Add("@AM_CustId", dto.AM_CustId);
            parameters.Add("@AM_ApprovedBy", dto.AM_ApprovedBy);
            parameters.Add("@AM_ApprovedOn", dto.AM_ApprovedOn);
            parameters.Add("@AM_Opeartion", dto.AM_Opeartion);
            parameters.Add("@AM_IPAddress", dto.AM_IPAddress);

            // ==== OUTPUT PARAMETERS ====
            parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

            // ==== EXECUTE SP ====
            await connection.ExecuteAsync(
                "spAcc_AssetMaster",
                parameters,
                transaction: transaction,
                commandType: CommandType.StoredProcedure
            );

            transaction.Commit();

            // ==== RETURN OUTPUT VALUES ====
            return new int[] { parameters.Get<int>("@iUpdateOrSave"), parameters.Get<int>("@iOper") };
        }

        //SaveAssetClassUnderSubHeading
        public async Task<string[]> SaveAssetClassAsync(SaveAssetClassDto asset)
        {
            // Step 1: Get DB from Session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            // Step 2: Get dynamic connection string
            string connectionString = _configuration.GetConnectionString(dbName);

            using (var connection = new SqlConnection(connectionString))
            {
                var parameters = new DynamicParameters();

                parameters.Add("@AM_ID", asset.AM_ID);
                parameters.Add("@AM_Description", asset.AM_Description ?? "");
                parameters.Add("@AM_Code", asset.AM_Code ?? "");
                parameters.Add("@AM_LevelCode", asset.AM_LevelCode);
                parameters.Add("@AM_ParentID", asset.AM_ParentID);
                parameters.Add("@AM_WDVITAct", asset.AM_WDVITAct);
                parameters.Add("@AM_ITRate", asset.AM_ITRate);
                parameters.Add("@AM_ResidualValue", asset.AM_ResidualValue);
                parameters.Add("@AM_CreatedBy", asset.AM_CreatedBy);
                parameters.Add("@AM_CreatedOn", asset.AM_CreatedOn);
                parameters.Add("@AM_UpdatedBy", asset.AM_UpdatedBy);
                parameters.Add("@AM_UpdatedOn", asset.AM_UpdatedOn);
                parameters.Add("@AM_DelFlag", asset.AM_DelFlag ?? "A");
                parameters.Add("@AM_Status", asset.AM_Status);
                parameters.Add("@AM_YearID", asset.AM_YearID);
                parameters.Add("@AM_CompID", asset.AM_CompID);
                parameters.Add("@AM_CustId", asset.AM_CustId);
                parameters.Add("@AM_ApprovedBy", asset.AM_ApprovedBy);
                parameters.Add("@AM_ApprovedOn", asset.AM_ApprovedOn);
                parameters.Add("@AM_Opeartion", asset.AM_Opeartion ?? "S");
                parameters.Add("@AM_IPAddress", asset.AM_IPAddress ?? "");

                // OUTPUT PARAMETERS
                parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // EXECUTE
                await connection.ExecuteAsync(
                    "spAcc_AssetMaster",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                // RETURN ARRAY (same as VB)
                return new string[]
                {
             parameters.Get<int>("@iUpdateOrSave").ToString(),
             parameters.Get<int>("@iOper").ToString()
                };
            }
        }






    }
}
