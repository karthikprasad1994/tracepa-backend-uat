using Dapper;
using Microsoft.Data.SqlClient;
using TracePca.Dto;
using TracePca.Interface.FixedAssetsInterface;

namespace TracePca.Service.FixedAssetsService
{
    public class AssetRepo : IAssetRepository
    {
        private readonly IConfiguration _configuration;

        public AssetRepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task InsertAssetsAsync(List<AssetExcelUploadDto> assets)
        {
            var sql = @"INSERT INTO Acc_FixedAssetMaster 
                    (AssetCode, LocationId, DivisionId, DepartmentId, BayId, Quantity, UnitsOfMeasurement, AssetAge, CreatedOn)
                    VALUES 
                    (@AssetCode, @LocationId, @DivisionId, @DepartmentId, @BayId, @Quantity, @UnitsOfMeasurement, @AssetAge, @CreatedOn)";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                // Insert all assets in batch using ExecuteAsync
                await connection.ExecuteAsync(sql, assets); // Dapper will map each AssetExcelUploadDto in the list to the parameters
            }
        }
    }
}

