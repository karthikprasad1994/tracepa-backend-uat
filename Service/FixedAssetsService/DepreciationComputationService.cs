using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Data;
using TracePca.Interface.FixedAssetsInterface;
using static TracePca.Dto.FixedAssets.DepreciationComputationDto;


namespace TracePca.Service.FixedAssetsService
{
    public class DepreciationComputationService : DepreciationComputationInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IWebHostEnvironment _env;
        private readonly SqlConnection _db;

        public DepreciationComputationService(Trdmyus1Context dbcontext, IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        //methodofDepreciation
        public async Task<List<DepreciationDto>> LoadDepreciationCompSLMAsync(
    string sNameSpace,
    int compId,
    int yearId,
    int noOfDays,
    int tNoOfDays,
    int duration,
    DateTime startDate,
    DateTime endDate,
    int custId,
    int method)
        {
            var result = new List<DepreciationDto>();

            // 1️⃣ Get all assets
            var sql = $@"
            SELECT DISTINCT AFAA_TrType, AFAA_Id, AFAA_ItemDescription,
                            AFAA_FYAmount, AFAA_AssetType, AFAA_AssetAmount,
                            AFAA_ItemCode, AFAA_ItemType, AFAA_Location,
                            AFAA_Division, AFAA_Department, AFAA_Bay
            FROM Acc_FixedAssetAdditionDel
            WHERE AFAA_CompID = @CompId
              AND AFAA_CustId = @CustId
              AND AFAA_YearID <= @YearId
              AND AFAA_DelFlag <> 'D'
            ORDER BY AFAA_ItemType";

            var assets = await _db.QueryAsync(sql, new { CompId = compId, CustId = custId, YearId = yearId });

            foreach (var asset in assets)
            {
                var dto = new DepreciationDto
                {
                    AssetClassID = asset.AFAA_AssetType,
                    AssetID = asset.AFAA_ItemType,
                    AssetType = await _db.ExecuteScalarAsync<string>(
                        "SELECT AM_Description FROM Acc_AssetMaster WHERE AM_ID = @AssetClassID AND AM_CompID = @CompId AND AM_CustId = @CustId",
                        new { AssetClassID = asset.AFAA_AssetType, CompId = compId, CustId = custId }
                    ),
                    Item = await _db.ExecuteScalarAsync<string>(
                        "SELECT AFAM_ItemDescription FROM Acc_FixedAssetMaster WHERE AFAM_ID = @AssetID AND AFAM_CustId = @CustId",
                        new { AssetID = asset.AFAA_ItemType, CustId = custId }
                    ),
                    LocationID = asset.AFAA_Location,
                    DivisionID = asset.AFAA_Division,
                    DepartmentID = asset.AFAA_Department,
                    BayID = asset.AFAA_Bay,
                    Location = await _db.ExecuteScalarAsync<string>(
                        "SELECT LS_Description FROM Acc_AssetLocationSetup WHERE LS_ID = @Id AND LS_CompID = @CompId AND LS_CustId = @CustId",
                        new { Id = asset.AFAA_Location, CompId = compId, CustId = custId }
                    ),
                    Division = await _db.ExecuteScalarAsync<string>(
                        "SELECT LS_Description FROM Acc_AssetLocationSetup WHERE LS_ID = @Id AND LS_CompID = @CompId AND LS_CustId = @CustId",
                        new { Id = asset.AFAA_Division, CompId = compId, CustId = custId }
                    ),
                    Department = await _db.ExecuteScalarAsync<string>(
                        "SELECT LS_Description FROM Acc_AssetLocationSetup WHERE LS_ID = @Id AND LS_CompID = @CompId AND LS_CustId = @CustId",
                        new { Id = asset.AFAA_Department, CompId = compId, CustId = custId }
                    ),
                    Bay = await _db.ExecuteScalarAsync<string>(
                        "SELECT LS_Description FROM Acc_AssetLocationSetup WHERE LS_ID = @Id AND LS_CompID = @CompId AND LS_CustId = @CustId",
                        new { Id = asset.AFAA_Bay, CompId = compId, CustId = custId }
                    ),
                    PurchaseDate = asset.PurchaseDate ?? startDate,
                    TrType = asset.AFAA_TrType
                };

                // 2️⃣ Calculate Days
                if (dto.PurchaseDate.HasValue)
                {
                    if (dto.PurchaseDate > startDate)
                    {
                        dto.NoOfDays = (dto.PurchaseDate <= endDate)
                            ? (endDate - dto.PurchaseDate.Value).Days
                            : 0;
                    }
                    else
                    {
                        dto.NoOfDays = noOfDays;
                    }
                }
                else
                {
                    dto.NoOfDays = 0;
                }

                // 3️⃣ Residual Value
                dto.Rsdulvalue = await _db.ExecuteScalarAsync<decimal>(
                    "SELECT AM_ResidualValue FROM Acc_AssetMaster WHERE AM_ID = @AssetClassID AND AM_CompID = @CompId AND AM_CustId = @CustId",
                    new { AssetClassID = dto.AssetClassID, CompId = compId, CustId = custId }
                );

                // 4️⃣ Check previous freeze ledger (implement GetPreviousYrFreezeLedgerCount)
                int freezeCount = GetPreviousYrFreezeLedgerCount(sNameSpace, compId, yearId, dto.AssetClassID, dto.AssetID, custId,
                    dto.LocationID, dto.DivisionID, dto.DepartmentID, dto.BayID, method);

                // 5️⃣ Fill Old/New Asset (implement FillOldAssetAsync & FillNewAssetAsync)
                if (freezeCount > 0)
                    await FillOldAssetAsync(dto, asset, sNameSpace, compId, custId, yearId, method);
                else
                    await FillNewAssetAsync(dto, asset, sNameSpace, compId, custId);

                result.Add(dto);
            }

            return result;
        }

        // Placeholder methods (implement according to your logic)
        private int GetPreviousYrFreezeLedgerCount(string ns, int compId, int yearId, int assetClassId, int assetId, int custId,
            int locationId, int divisionId, int departmentId, int bayId, int method) => 0;

        private Task FillOldAssetAsync(DepreciationDto dto, dynamic asset, string ns, int compId, int custId, int yearId, int method)
            => Task.CompletedTask;

        private Task FillNewAssetAsync(DepreciationDto dto, dynamic asset, string ns, int compId, int custId)
            => Task.CompletedTask;
    }
}

