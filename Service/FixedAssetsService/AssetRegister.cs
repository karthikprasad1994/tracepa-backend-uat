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
            afm.AFAM_ID,
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

        public async Task<AssetRegDetailsDto> GetAssetRegDetailsAsync(int assetId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"
    SELECT 
        cm.CUST_ID AS CustomerId,
        cm.CUST_NAME AS CustomerName,
        fy.YMS_YEARID AS YearId,
        fy.YMS_ID AS FinancialYear,
        am.AM_Description AS AssetClassName,
        am.AM_ID AS AssetId,
       
        
        afm.AFAM_Code AS AssetCode,
        afm.AFAM_AssetCode AS AssetNo,
        afm.AFAM_ID AS Id,
        afm.AFAM_Location AS LocationId,
        afm.AFAM_Description AS AssetDescription,
        afm.AFAM_Quantity AS Quantity,
        cmm.CMM_DESC AS UnitOfMeasurement, 
        cmm.CMM_ID AS UnitOfMeasurementId,
        afm.AFAM_AssetAge AS UsefulLife,
        afm.AFAM_CommissionDate AS PutToUseDate
    FROM Acc_FixedAssetMaster afm
    INNER JOIN Acc_AssetMaster am ON afm.AFAM_AssetType = am.AM_ID
    INNER JOIN YEAR_MASTER fy ON afm.AFAM_YearId = fy.YMS_YEARID
    INNER JOIN SAD_CUSTOMER_MASTER cm ON afm.AFAM_CustId = cm.CUST_ID
    LEFT JOIN Content_Management_Master cmm ON afm.AFAM_Unit = cmm.CMM_ID
    WHERE afm.AFAM_ID = @AssetId;";

            var result = await connection.QueryFirstOrDefaultAsync<AssetRegDetailsDto>(query, new { AssetId = assetId });

            return result;
        }


        public async Task<bool> UpdateAssetAsync(int afamId, AssetUpdateDto dto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = @"
UPDATE Acc_FixedAssetMaster SET
    Attribute1 = @Attribute1,
    Attribute2 = @Attribute2,
    Attribute3 = @Attribute3,
    AFAM_AssetType = @AssetClassId,
    AFAM_Location = @AfamLocation,
    AFAM_Division = @AfamDivision,
    AFAM_Department = @AfamDepartment,
    AFAM_Bay = @AfamBay,
    AFAM_EmployeeName = @AfamEmployeeName,
    AFAM_EmployeeCode = @AfamEmployeeCode,
    AFAM_ItemCode = @AfamItemCode,
    AFAM_WrntyDesc = @AfamWrntyDesc,
    AFAM_ContactPerson = @AfamContactPrsn,
    AfAM_CompanyName = @AfamCompanyName,
    AFAM_Date = @AfamDate,
    AFAM_ToDate = @AfamToDate,
    AFAM_Contprsn = @AfamContprsn,
    AFAM_LToWhom = @AfamLtoWhom,
    AFAM_Address = @AfamAddress,
    AFAM_LAmount = @AfamLamount,
    AFAM_LAggriNo = @AfamLaggriNo,
    AFAM_LDate = @AfamLdate,
    AFAM_LExchDate = @AfamLexchDate,
    AFAM_LCurrencyType = @AfamLcurrencyType,
    AFAM_PolicyNo = @AfamPolicyNo,
    AfAM_Amount = @AfamAmount,
    AfAM_BrokerName = @AfamBrokerName,
    AFAM_AMCCompanyName = @AfamAmcCompanyName,
    AFAM_AMCFrmDate = @AfamAmcFrmDate,
    AFAM_AssetAge = @AfamAssetAge,
    AFAM_Remark = @AfamRemark,
    AFAM_AMCTo = @AfamAmcTo
WHERE AFAM_ID = @AfamId;";

            var parameters = new DynamicParameters(dto);
            parameters.Add("AfamId", afamId);

            var rowsAffected = await connection.ExecuteAsync(query, parameters);

            return rowsAffected > 0;
        }







    }
}

