using System;
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using TracePca.Data;

//using TracePca.Dto;

//using TracePca.Dto;
using TracePca.Dto.FixedAssets;



//using TracePca.Dto.DigitalFilling;
using TracePca.Helpers;
using TracePca.Interface;
using TracePca.Interface.FixedAssetsInterface;
using TracePca.Models;
using TracePca.Models.UserModels;
//using static TracePca.Dto.FIN_Statement.ScheduleReportDto;
//using static TracePca.Dto.FIN_Statement.ScheduleReportDto;
using static TracePca.Dto.FixedAssets.AssetMasterdto;
using static TracePca.Service.FixedAssetsService.AssetMasterService;



namespace TracePca.Service.FixedAssetsService
{
    public class AssetMasterService : AssetMasterInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IWebHostEnvironment _env;
        private readonly SqlConnection _db;

        public AssetMasterService(Trdmyus1Context dbcontext, IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        //LoadCustomer
        public async Task<IEnumerable<CustDto>> LoadCustomerAsync(int CompId)
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
                CUST_ID AS Cust_Id,
                CUST_NAME AS Cust_Name
            FROM SAD_CUSTOMER_MASTER
            WHERE CUST_DELFLG <> 'D' 
              AND CUST_CompID = @CompId";

            return await connection.QueryAsync<CustDto>(query, new { CompId });
        }

        //LoadStatus
        public async Task<IEnumerable<StatusDto>> LoadStatusAsync(int compId, string Name)
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
                CUST_STATUS AS Status
            FROM SAD_CUSTOMER_MASTER
            WHERE CUST_CompID = @CompId
            AND CUST_NAME = @Name";

            return await connection.QueryAsync<StatusDto>(query, new { CompId = compId, Name = Name });

        }

        //FinancialYear
        public async Task<IEnumerable<YearDto>> GetYearsAsync(int compId)
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
                 YMS_YEARID,
               YMS_ID
               FROM YEAR_MASTER
               WHERE 
               YMS_FROMDATE < DATEADD(YEAR, 1, GETDATE())
               AND YMS_CompId = @CompId
               ORDER BY LEFT(YMS_ID, 4) DESC";
            return await connection.QueryAsync<YearDto>(query, new { CompId = compId });
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
        //        // Step 1: Get DB name from session
        //        string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //        if (string.IsNullOrEmpty(dbName))
        //            throw new Exception("CustomerCode is missing in session. Please log in again.");

        //        // Step 2: Get connection string dynamically
        //        var connectionString = _configuration.GetConnectionString(dbName);

        //        using var connection = new SqlConnection(connectionString);
        //        await connection.OpenAsync();

        //        string query;

        //        if (parentId == 0)
        //        {
        //            query = @"
        //        SELECT 
        //            LS_ID AS DivisionId,
        //            LS_Description AS DivisionName
        //        FROM Acc_AssetLocationSetup
        //        WHERE 
        //            LS_LevelCode = 1
        //            AND LS_CompID = @CompId
        //            AND LS_CustId = @CustId
        //        ORDER BY LS_ID DESC";
        //        }
        //        else
        //        {
        //            query = @"
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
        //        }

        //        return await connection.QueryAsync<DivisionDto>( query,new { CompId = compId, ParentId = parentId, CustId = custId } );
        //    }


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

        //    return await connection.QueryAsync<HeadingDto>(query, new { CompId = compId,CustId = custId});
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

        //    return await connection.QueryAsync<SubHeadingDto>(query, new{ CompId = compId, ParentId = parentId, CustId = custId });
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

        ////SaveAsset
        //public async Task<int[]> SaveAssetAsync(AssetMasterDto asset)
        //{
        //    // ✅ Step 1: Get DB name from session
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    // ✅ Step 2: Get connection string dynamically
        //    var connectionString = _configuration.GetConnectionString(dbName);

        //    // ✅ Step 3: Use SqlConnection
        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    using (var transaction = connection.BeginTransaction())
        //    {
        //        try
        //        {
        //            using (var cmd = new SqlCommand("spAcc_AssetMaster", connection, transaction))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;

        //                // 🔽 INPUT PARAMETERS
        //                cmd.Parameters.AddWithValue("@AM_ID", asset.AM_ID);
        //                cmd.Parameters.AddWithValue("@AM_Description", asset.AM_Description ?? string.Empty);
        //                cmd.Parameters.AddWithValue("@AM_Code", asset.AM_Code ?? string.Empty);
        //                cmd.Parameters.AddWithValue("@AM_LevelCode", asset.AM_LevelCode);
        //                cmd.Parameters.AddWithValue("@AM_ParentID", asset.AM_ParentID);
        //                cmd.Parameters.AddWithValue("@AM_WDVITAct", asset.AM_WDVITAct);
        //                cmd.Parameters.AddWithValue("@AM_ITRate", asset.AM_ITRate ?? string.Empty);
        //                cmd.Parameters.AddWithValue("@AM_ResidualValue", asset.AM_ResidualValue);
        //                cmd.Parameters.AddWithValue("@AM_CreatedBy", asset.AM_CreatedBy);
        //                cmd.Parameters.AddWithValue("@AM_CreatedOn", asset.AM_CreatedOn);
        //                cmd.Parameters.AddWithValue("@AM_UpdatedBy", asset.AM_UpdatedBy);
        //                cmd.Parameters.AddWithValue("@AM_UpdatedOn", asset.AM_UpdatedOn);
        //                cmd.Parameters.AddWithValue("@AM_DelFlag", asset.AM_DelFlag ?? "A");
        //                cmd.Parameters.AddWithValue("@AM_Status", asset.AM_Status ?? "W");
        //                cmd.Parameters.AddWithValue("@AM_YearID", asset.AM_YearID);
        //                cmd.Parameters.AddWithValue("@AM_CompID", asset.AM_CompID);
        //                cmd.Parameters.AddWithValue("@AM_CustId", asset.AM_CustId);
        //                cmd.Parameters.AddWithValue("@AM_ApprovedBy", asset.AM_ApprovedBy);
        //                cmd.Parameters.AddWithValue("@AM_ApprovedOn", asset.AM_ApprovedOn);
        //                cmd.Parameters.AddWithValue("@AM_Opeartion", asset.AM_Opeartion ?? "S");
        //                cmd.Parameters.AddWithValue("@AM_IPAddress", asset.AM_IPAddress ?? string.Empty);

        //                // 🔽 OUTPUT PARAMETERS
        //                var updateOrSaveParam = new SqlParameter("@iUpdateOrSave", SqlDbType.Int)
        //                {
        //                    Direction = ParameterDirection.Output
        //                };
        //                var operParam = new SqlParameter("@iOper", SqlDbType.Int)
        //                {
        //                    Direction = ParameterDirection.Output
        //                };

        //                cmd.Parameters.Add(updateOrSaveParam);
        //                cmd.Parameters.Add(operParam);

        //                // EXECUTE
        //                await cmd.ExecuteNonQueryAsync();

        //                transaction.Commit();

        //                // RETURN OUTPUT VALUES
        //                return new int[]
        //                {
        //            (int)(updateOrSaveParam.Value ?? 0),
        //            (int)(operParam.Value ?? 0)
        //                };
        //            }
        //        }
        //        catch
        //        {
        //            transaction.Rollback();
        //            throw;
        //        }
        //    }
        //}




        ////public async Task<(int UpdateOrSave, int Oper)> SaveAssetAsync(LocationSetupDto model)
        ////{
        ////    // Step 1: Get DB name from session
        ////    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        ////    if (string.IsNullOrEmpty(dbName))
        ////        throw new Exception("CustomerCode is missing in session. Please log in again.");

        ////    // Step 2: Get connection string dynamically
        ////    var connectionString = _configuration.GetConnectionString(dbName);

        ////    using var connection = new SqlConnection(connectionString);
        ////    await connection.OpenAsync();

        ////    var parameters = new DynamicParameters();

        ////    // Input parameters
        ////    parameters.Add("@LS_ID", model.LS_ID);
        ////    parameters.Add("@LS_Description", model.LS_Description);
        ////    parameters.Add("@LS_DescCode", model.LS_DescCode);
        ////    parameters.Add("@LS_Code", model.LS_Code);
        ////    parameters.Add("@LS_LevelCode", model.LS_LevelCode);
        ////    parameters.Add("@LS_ParentID", model.LS_ParentID);
        ////    parameters.Add("@LS_CreatedBy", model.LS_CreatedBy);
        ////    parameters.Add("@LS_CreatedOn", model.LS_CreatedOn);
        ////    parameters.Add("@LS_UpdatedBy", model.LS_UpdatedBy);
        ////    parameters.Add("@LS_UpdatedOn", model.LS_UpdatedOn);
        ////    parameters.Add("@LS_DelFlag", model.LS_DelFlag);
        ////    parameters.Add("@LS_Status", model.LS_Status);
        ////    parameters.Add("@LS_YearID", model.LS_YearID);
        ////    parameters.Add("@LS_CompID", model.LS_CompID);
        ////    parameters.Add("@LS_CustId", model.LS_CustId);
        ////    parameters.Add("@LS_ApprovedBy", model.LS_ApprovedBy);
        ////    parameters.Add("@LS_ApprovedOn", model.LS_ApprovedOn);
        ////    parameters.Add("@LS_Opeartion", model.LS_Opeartion);
        ////    parameters.Add("@LS_IPAddress", model.LS_IPAddress);

        ////    // OUTPUT parameters
        ////    parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
        ////    parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

        ////    // Execute Stored Procedure
        ////    await connection.ExecuteAsync(
        ////        "spAcc_AssetLocationSetup",
        ////        parameters,
        ////        commandType: CommandType.StoredProcedure
        ////    );

        ////    // Return OUTPUT values
        ////    return ( UpdateOrSave: parameters.Get<int>("@iUpdateOrSave"), Oper: parameters.Get<int>("@iOper") );
        ////}


        ////SaveLocationn
        //public async Task<int> SaveLocationAsync(AddLocationnDto locationDto)
        //{
        //    // Step 1: Get DB name from session
        //    string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //    if (string.IsNullOrEmpty(dbName))
        //        throw new Exception("CustomerCode is missing in session. Please log in again.");

        //    // Step 2: Get dynamic connection string
        //    var connectionString = _configuration.GetConnectionString(dbName);

        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    using var transaction = connection.BeginTransaction();

        //        // ---------------------------
        //        // UPDATE BLOCK
        //        // ---------------------------
        //        if (locationDto.Id.HasValue && locationDto.Id > 0)
        //        {
        //            string updateQuery = @"
        //        UPDATE Acc_AssetLocationSetup
        //        SET 
        //            LS_Description = @Name, 
        //            LS_DescCode   = @Code, 
        //            LS_UpdatedBy  = @CreatedBy, 
        //            LS_UpdatedOn  = GETDATE()
        //        WHERE 
        //            LS_ID = @Id
        //            AND LS_CustId = @CustomerId";

        //            await connection.ExecuteAsync(updateQuery, new
        //            {
        //                locationDto.Id,
        //                locationDto.Name,
        //                locationDto.Code,
        //                locationDto.CreatedBy,
        //                locationDto.CustomerId
        //            }, transaction);

        //            await transaction.CommitAsync();
        //            return locationDto.Id.Value;
        //        }

        //        // ---------------------------
        //        // INSERT BLOCK
        //        // ---------------------------

        //        // Generating new LS_ID manually
        //        string maxIdQuery = @"SELECT ISNULL(MAX(LS_ID), 0) FROM Acc_AssetLocationSetup WITH (UPDLOCK, HOLDLOCK)";
        //        int maxId = await connection.ExecuteScalarAsync<int>(maxIdQuery, transaction: transaction);
        //        int newId = maxId + 1;

        //        string insertQuery = @"
        //    INSERT INTO Acc_AssetLocationSetup
        //    (
        //        LS_ID, LS_Description, LS_DescCode, LS_Code, LS_LevelCode, LS_ParentID,
        //        LS_CreatedBy, LS_CreatedOn, LS_UpdatedBy, LS_UpdatedOn,
        //        LS_ApprovedBy, LS_ApprovedOn,
        //        LS_DelFlag, LS_Status, LS_YearID, LS_CompID, LS_Opeartion, LS_IPAddress, LS_CustId
        //    )
        //    VALUES
        //    (
        //        @NewId, @Name, @Code, 
        //        0, 0, @ParentID,
        //        @CreatedBy, GETDATE(), @CreatedBy, GETDATE(),
        //        @CreatedBy, GETDATE(),
        //        'X', 'W', @YearID, @CompanyId, 'C', @IPAddress, @CustomerId
        //    )";

        //        await connection.ExecuteAsync(insertQuery, new
        //        {
        //            NewId = newId,
        //            locationDto.Name,
        //            locationDto.Code,
        //            locationDto.ParentID,
        //            locationDto.CreatedBy,
        //            locationDto.IPAddress,
        //            locationDto.YearID,
        //            locationDto.CompanyId,
        //            locationDto.CustomerId
        //        }, transaction);

        //        await transaction.CommitAsync();
        //        return newId;
        //}

        ////SaveDivision
        //    public async Task<int> SaveDivisionAsync(AddDivisionnDto divisionDto)
        //    {
        //        // ✅ Step 1: Get DB name from Session
        //        string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //        if (string.IsNullOrEmpty(dbName))
        //            throw new Exception("CustomerCode is missing in session. Please log in again.");

        //        // ✅ Step 2: Dynamic Connection String
        //        using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));

        //        // 🔍 Check Duplicate (same Name + Customer)
        //        var existsQuery = @"
        //    SELECT COUNT(1)
        //    FROM Acc_AssetLocationSetup
        //    WHERE LS_Description = @Name
        //      AND LS_CustId = @CustomerId
        //      AND (@Id IS NULL OR LS_ID <> @Id)";

        //        var exists = await connection.ExecuteScalarAsync<int>(existsQuery, divisionDto);

        //        if (exists > 0)
        //            return -1;   // ❗ Duplicate Division

        //        // -----------------------------------
        //        //  ✅ UPDATE CASE
        //        // -----------------------------------
        //        if (divisionDto.Id.HasValue && divisionDto.Id.Value > 0)
        //        {
        //            string recordExistsQuery = @"
        //        SELECT COUNT(1)
        //        FROM Acc_AssetLocationSetup
        //        WHERE LS_ID = @Id AND LS_CustId = @CustomerId";

        //            var recordExists = await connection.ExecuteScalarAsync<int>(recordExistsQuery, new
        //            {
        //                Id = divisionDto.Id,
        //                divisionDto.CustomerId
        //            });

        //            if (recordExists > 0)
        //            {
        //                // 🔁 Update record
        //                string updateQuery = @"
        //            UPDATE Acc_AssetLocationSetup
        //            SET 
        //                LS_Description = @Name,
        //                LS_DescCode = @Code,
        //                LS_UpdatedBy = @CreatedBy,
        //                LS_UpdatedOn = GETDATE()
        //            WHERE LS_ID = @Id AND LS_CustId = @CustomerId";

        //                await connection.ExecuteAsync(updateQuery, new
        //                {
        //                    Id = divisionDto.Id,
        //                    divisionDto.Name,
        //                    divisionDto.Code,
        //                    divisionDto.CreatedBy,
        //                    divisionDto.CustomerId
        //                });

        //                return divisionDto.Id.Value;     // Return updated ID
        //            }
        //        }

        //        // -----------------------------------
        //        //  ➕ INSERT NEW RECORD
        //        // -----------------------------------
        //        string maxIdQuery = "SELECT ISNULL(MAX(LS_ID), 0) FROM Acc_AssetLocationSetup";
        //        int newId = (await connection.ExecuteScalarAsync<int>(maxIdQuery)) + 1;

        //        string insertQuery = @"
        //    INSERT INTO Acc_AssetLocationSetup 
        //    (
        //        LS_ID, LS_Description, LS_DescCode, LS_Code, LS_LevelCode, LS_ParentID,
        //        LS_CreatedBy, LS_CreatedOn, LS_UpdatedBy, LS_UpdatedOn, 
        //        LS_ApprovedBy, LS_ApprovedOn, 
        //        LS_DelFlag, LS_Status, LS_YearID, LS_CompID, 
        //        LS_Opeartion, LS_IPAddress, LS_CustId
        //    )
        //    VALUES
        //    (
        //        @NewId, @Name, @Code, 0, 1, 0,
        //        @CreatedBy, GETDATE(), @CreatedBy, GETDATE(),
        //        @CreatedBy, GETDATE(),
        //        'X', 'W', @YearID, @CompanyId,
        //        'C', @IPAddress, @CustomerId
        //    )";

        //        await connection.ExecuteAsync(insertQuery, new
        //        {
        //            NewId = newId,
        //            divisionDto.Name,
        //            divisionDto.Code,
        //            divisionDto.CreatedBy,
        //            divisionDto.IPAddress,
        //            divisionDto.YearID,
        //            CompanyId = divisionDto.CompanyId,
        //            divisionDto.CustomerId
        //        });

        //        return newId;  // New inserted ID
        //    }

        ////SaveDepartment
        //    public async Task<int> SaveDepartmentAsync(AddDepartmenttDto departmentDto)
        //    {
        //        // ✅ Step 1: Get DB name from session
        //        string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //        if (string.IsNullOrEmpty(dbName))
        //            throw new Exception("CustomerCode is missing in session. Please log in again.");

        //        // ✅ Step 2: Get dynamic connection string
        //        using var connection = new SqlConnection(_configuration.GetConnectionString(dbName));
        //        await connection.OpenAsync();

        //        // 🔍 Step 3: Check duplicate department for the same customer & division
        //        var existsQuery = @"
        //    SELECT COUNT(1) 
        //    FROM Acc_AssetLocationSetup 
        //    WHERE LS_Description = @Name
        //      AND LS_CustId = @CustomerId
        //      AND LS_ParentID = @DivisionId";

        //        var exists = await connection.ExecuteScalarAsync<int>(existsQuery, departmentDto);
        //        if (exists > 0)
        //            return -1; // ❌ Duplicate Department Found

        //        // -----------------------------------
        //        // ✅ UPDATE CASE
        //        // -----------------------------------
        //        if (departmentDto.Id.HasValue && departmentDto.Id.Value > 0)
        //        {
        //            string updateQuery = @"
        //        UPDATE Acc_AssetLocationSetup
        //        SET LS_Description = @Name,
        //            LS_DescCode = @Code,
        //            LS_UpdatedBy = @CreatedBy,
        //            LS_UpdatedOn = GETDATE()
        //        WHERE LS_ID = @Id 
        //          AND LS_CustId = @CustomerId";

        //            await connection.ExecuteAsync(updateQuery, departmentDto);
        //            return departmentDto.Id.Value;
        //        }

        //        // -----------------------------------
        //        // ➕ INSERT CASE
        //        // -----------------------------------
        //        string maxIdQuery = "SELECT ISNULL(MAX(LS_ID), 0) FROM Acc_AssetLocationSetup";
        //        int newDepartmentId = (await connection.ExecuteScalarAsync<int>(maxIdQuery)) + 1;

        //        string insertQuery = @"
        //    INSERT INTO Acc_AssetLocationSetup
        //    (
        //        LS_ID, LS_Description, LS_DescCode, LS_Code, LS_LevelCode, LS_ParentID,
        //        LS_CreatedBy, LS_CreatedOn, LS_UpdatedBy, LS_UpdatedOn,
        //        LS_ApprovedBy, LS_ApprovedOn,
        //        LS_DelFlag, LS_Status, LS_YearID, LS_CompID,
        //        LS_Opeartion, LS_IPAddress, LS_CustId
        //    )
        //    VALUES
        //    (
        //        @NewId, @Name, @Code, 0, 2, @DivisionId,
        //        @CreatedBy, GETDATE(), @CreatedBy, GETDATE(),
        //        @CreatedBy, GETDATE(),
        //        'X', 'W', @YearID, @CompanyId,
        //        'C', @IPAddress, @CustomerId
        //    )";

        //        await connection.ExecuteAsync(insertQuery, new
        //        {
        //            NewId = newDepartmentId,
        //            departmentDto.Name,
        //            departmentDto.Code,
        //            departmentDto.CreatedBy,
        //            departmentDto.YearID,
        //            departmentDto.CompanyId,
        //            departmentDto.IPAddress,
        //            departmentDto.CustomerId,
        //            departmentDto.DivisionId
        //        });

        //        return newDepartmentId;
        //    }

        //SaveBay

     }
}







