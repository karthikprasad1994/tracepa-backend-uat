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
using TracePca.Dto.FixedAssets;
using TracePca.Helpers;
using TracePca.Interface;
using TracePca.Interface.FixedAssetsInterface;
using TracePca.Models;
using TracePca.Models.UserModels;
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
    }
}







