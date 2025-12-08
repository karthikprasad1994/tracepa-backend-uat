using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FixedAssetsInterface;
using static TracePca.Dto.FixedAssets.AssetCreationDto;
//using static TracePca.Dto.FixedAssets.AssetMasterdto;

namespace TracePca.Service.FixedAssetsService
{
    public class AssetCreationService : AssetCreationInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IWebHostEnvironment _env;
        private readonly SqlConnection _db;

        public AssetCreationService(Trdmyus1Context dbcontext, IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        //LoadAssetClass
        public async Task<IEnumerable<AssetTypeDto>> LoadAssetTypeAsync(int compId, int custId)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string query = @"
        SELECT 
            AM_ID AS AssetTypeId,
            AM_Description AS AssetTypeName
        FROM Acc_AssetMaster
        WHERE 
            AM_LevelCode = 2
            AND AM_DelFlag = 'A'
            AND AM_CompID = @CompId
            AND AM_CustID = @CustId
        ORDER BY AM_ID DESC";

            return await connection.QueryAsync<AssetTypeDto>(query, new
            {
                CompId = compId,
                CustId = custId
            });
        }

        //New
        public async Task<int> AddNewAssetAsync(NewDto asset)
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
        INSERT INTO AssetMaster
        (
            AssetName, 
            AssetCode, 
            CreatedBy, 
            CompId, 
            CustId
        )
        VALUES
        (
            @AssetName,
            @AssetCode,
            @CreatedBy,
            @CompId,
            @CustId
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
    ";

            var parameters = new
            {
                AssetName = asset.AssetName,
                AssetCode = asset.AssetCode,
                CreatedBy = asset.CreatedBy,
                CompId = asset.CompId,
                CustId = asset.CustId
            };

            return await connection.ExecuteScalarAsync<int>(query, parameters);
        }

        //Search
        public async Task<IEnumerable<AssetRegisterDto>> LoadAssetRegisterAsync(int compId, int assetTypeId, int yearId, int custId)
        {
            // Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // Step 2: Get connection string dynamically
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string query = @"
        SELECT 
            F.AFAM_ID AS Id,
            F.AFAM_AssetType AS AssetId,
            F.AFAM_AssetCode AS AssetCode,
            AM.AM_Description AS AssetDescription,
            F.AFAM_ItemCode AS ItemCode,
            F.AFAM_ItemDescription AS ItemDescription,
            F.AFAM_CommissionDate AS CommissionDate,
            F.AFAM_Quantity AS Qty,
            F.AFAM_AssetAge AS AssetAge,
            F.AFAM_Status AS CurrentStatus,
            F.AFAM_TRStatus AS TRStatus,
            LS1.LS_Description AS Location,
            LS2.LS_Description AS Division,
            LS3.LS_Description AS Department,
            LS4.LS_Description AS Bay
        FROM Acc_FixedAssetMaster F
        LEFT JOIN Acc_AssetMaster AM 
            ON AM.AM_ID = F.AFAM_AssetType 
            AND AM.AM_CompID = @CompId 
            AND AM.AM_CustId = @CustId
        LEFT JOIN Acc_AssetLocationSetup LS1 
            ON LS1.LS_ID = F.AFAM_Location 
            AND LS1.LS_CompID = @CompId 
            AND LS1.LS_CustId = @CustId
        LEFT JOIN Acc_AssetLocationSetup LS2 
            ON LS2.LS_ID = F.AFAM_Division 
            AND LS2.LS_CompID = @CompId 
            AND LS2.LS_CustId = @CustId 
            AND LS2.LS_LevelCode = 1
        LEFT JOIN Acc_AssetLocationSetup LS3 
            ON LS3.LS_ID = F.AFAM_Department 
            AND LS3.LS_CompID = @CompId 
            AND LS3.LS_CustId = @CustId 
            AND LS3.LS_LevelCode = 2
        LEFT JOIN Acc_AssetLocationSetup LS4 
            ON LS4.LS_ID = F.AFAM_Bay 
            AND LS4.LS_CompID = @CompId 
            AND LS4.LS_CustId = @CustId 
            AND LS4.LS_LevelCode = 3
        WHERE 
            F.AFAM_CompID = @CompId
            AND F.AFAM_CustID = @CustId
            AND F.AFAM_YearID = @YearId
            AND F.AFAM_DelFlag <> 'T'
            AND F.AFAM_Status <> 'T'
            " + (assetTypeId != 0 ? " AND F.AFAM_AssetType = @AssetTypeId " : "") + @"
        ORDER BY F.AFAM_ID";

            var rawResults = await connection.QueryAsync<AssetRegisterRaw>(query, new
            {
                CompId = compId,
                CustId = custId,
                YearId = yearId,
                AssetTypeId = assetTypeId
            });

            int counter = 1;

            // final result mapping
            return rawResults.Select(r => new AssetRegisterDto
            {
                SLNo = counter++,
                ID = r.Id,
                AssetID = r.AssetId,
                AssetCode = r.AssetCode ?? "0",
                AssetDescription = r.AssetDescription ?? "",
                ItemCode = r.ItemCode ?? "0",
                ItemDescription = r.ItemDescription ?? "",
                DateCommission =
                    r.CommissionDate?.ToString("dd/MM/yyyy") == "01/01/1900"
                    ? ""
                    : r.CommissionDate?.ToString("dd/MM/yyyy"),
                Qty = r.Qty ?? 0,
                AssetAge = r.AssetAge ?? 0,
                CurrentStatus = r.CurrentStatus == "W" ? "Waiting For Approval" : "Approved",
                TRStatus = r.TRStatus ?? "",
                Location = r.Location ?? "",
                Division = r.Division ?? "",
                Department = r.Department ?? "",
                Bay = r.Bay ?? ""
            });
        }


    }
}
