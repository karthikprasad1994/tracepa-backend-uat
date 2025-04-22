using TracePca.Data.CustomerRegistration;
using TracePca.Data;
using TracePca.Interface.FixedAssetsInterface;
using TracePca.Dto.AssetRegister;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace TracePca.Service.FixedAssetsService
{
    public class AssetRegister : AssetRegisterInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly CustomerRegistrationContext _customerRegistrationContext;
        private readonly IConfiguration _configuration;

        public AssetRegister(Trdmyus1Context dbContext, CustomerRegistrationContext customerDbContext, IConfiguration configuration)
        {
            _dbcontext = dbContext;
            _customerRegistrationContext = customerDbContext;
            _configuration = configuration;
        }


        public async Task<IEnumerable<AssetDetailsDto>> GetAssetDetailsAsync(int customerId, int assetClassId, int financialYearId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"
        SELECT 
            afm.AFAM_AssetCode,
            afm.AFAM_Code,
            am.AM_Description AS AssetTypeName,
            afm.AFAM_Description,
            afm.AFAM_CommissionDate,
            afm.AFAM_Quantity,
            afm.AFAM_AssetAge,
            afm.AFAM_Status,
            afm.AFAM_TRStatus
        FROM Acc_FixedAssetMaster afm
        INNER JOIN Acc_AssetMaster am ON afm.AFAM_AssetType = am.AM_ID
        WHERE afm.AFAM_CustId = @CustomerId
          AND afm.AFAM_AssetType = @AssetClassId
          AND afm.AFAM_YearId = @FinancialYearId";

            var result = await connection.QueryAsync<AssetDetailsDto>(query, new
            {
                CustomerId = customerId,
                AssetClassId = assetClassId,
                FinancialYearId = financialYearId
            });

            return result;
        }

    }
}

