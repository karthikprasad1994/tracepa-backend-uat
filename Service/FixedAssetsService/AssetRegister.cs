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

        public async Task<AssetRegDetailsDto> GetAssetRegDetailsAsync(int assetId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"
    SELECT 
        cm.CUST_NAME AS CustomerName,
        fy.YMS_ID AS FinancialYear,
        am.AM_Description AS AssetClassName,
        afm.AFAM_Code AS AssetCode,
        afm.AFAM_AssetCode AS AssetNo,
        afm.AFAM_Description AS AssetDescription,
        afm.AFAM_Quantity AS Quantity,
        cmm.CMM_DESC AS UnitOfMeasurement, -- ✨ updated here
        afm.AFAM_AssetAge AS UsefulLife,
        afm.AFAM_CommissionDate AS PutToUseDate
    FROM Acc_FixedAssetMaster afm
    INNER JOIN Acc_AssetMaster am ON afm.AFAM_AssetType = am.AM_ID
    INNER JOIN YEAR_MASTER fy ON afm.AFAM_YearId = fy.YMS_YEARID
    INNER JOIN SAD_CUSTOMER_MASTER cm ON afm.AFAM_CustId = cm.CUST_ID
    LEFT JOIN Content_Management_Master cmm ON afm.AFAM_Unit = cmm.CMM_ID -- ✨ join added
    WHERE afm.AFAM_ID = @AssetId;";

            var result = await connection.QueryFirstOrDefaultAsync<AssetRegDetailsDto>(query, new { AssetId = assetId });

            return result;
        }

        public async Task UpdateAssetDetailsAsync(int afamId, AssetUpdateDto updateDto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Lookup IDs from master tables
                var customerIdQuery = "SELECT CUST_ID FROM SAD_CUSTOMER_MASTER WHERE CUST_NAME = @CustomerName";
                var customerId = await connection.ExecuteScalarAsync<int?>(customerIdQuery, new { CustomerName = updateDto.CustomerName }, transaction);

                var unitOfMeasurementIdQuery = "SELECT CMM_ID FROM Content_Management_Master WHERE CMM_DESC = @UnitOfMeasurement";
                var unitOfMeasurementId = await connection.ExecuteScalarAsync<int?>(unitOfMeasurementIdQuery, new { UnitOfMeasurement = updateDto.UnitOfMeasurement }, transaction);

                var financialYearIdQuery = "SELECT YMS_YEARID FROM YEAR_MASTER WHERE YMS_ID = @YMSID";
                var financialYearId = await connection.ExecuteScalarAsync<int?>(financialYearIdQuery, new { YMSID = updateDto.YMSID }, transaction);

                var assetClassIdQuery = "SELECT AM_ID FROM Acc_AssetMaster WHERE AM_Description = @AssetClassName";
                var assetClassId = await connection.ExecuteScalarAsync<int?>(assetClassIdQuery, new { AssetClassName = updateDto.AssetClassName }, transaction);

                string updateQuery = @"
            UPDATE Acc_FixedAssetMaster
            SET 
                AFAM_AssetCode = @Afam_AssetCode,
                AFAM_Description = @Afam_Description,
                AFAM_Quantity = @Afam_Quantity,
                AFAM_AssetAge = @Afam_AssetAge,
                AFAM_CommissionDate = @Afam_CommissionDate,
                AFAM_Status = @Afam_Status,
                TR_Status = @TR_Status,
                UnitOfMeasurement = @UnitOfMeasurementId,
                Afam_CustomerId = @CustomerId,
                Afam_YearId = @FinancialYearId,
                Afam_AssetClassId = @AssetClassId,

                Afam_ItemCode = @Afam_ItemCode,
                Afam_ItemDesc = @Afam_ItemDesc,
                Afam_PurchaseAmount = @Afam_PurchaseAmount,
                Afam_PurchaseDate = @Afam_PurchaseDate,
                Afam_PolicyNo = @Afam_PolicyNo,
                Afam_Amount = @Afam_Amount,
                Afam_Location = @Afam_Location,
                Afam_Division = @Afam_Division,
                Afam_Department = @Afam_Department,
                Afam_Bay = @Afam_Bay,
                Afam_EmployeeName = @Afam_EmployeeName,
                Afam_EmployeeCode = @Afam_EmployeeCode,
                Afam_BrokerName = @Afam_BrokerName,
                Afam_CompanyName = @Afam_CompanyName,
                Afam_SupplierName = @Afam_SupplierName,
                Afam_ContactPerson = @Afam_ContactPerson,
                Afam_Address = @Afam_Address,
                Afam_Phone = @Afam_Phone,
                Afam_Fax = @Afam_Fax,
                Afam_EmailID = @Afam_EmailID,
                Afam_Unit = @Afam_Unit,
                Afam_UpdatedBy = @Afam_UpdatedBy
            WHERE AFAM_ID = @AfamId;
        ";

                await connection.ExecuteAsync(updateQuery, new
                {
                   
                   
                    UnitOfMeasurementId = unitOfMeasurementId,
                    CustomerId = customerId,
                    FinancialYearId = financialYearId,
                    AssetClassId = assetClassId,
                    Afam_AssetCode = updateDto.AssetCode,
                    Afam_ItemCode = updateDto.Afam_ItemCode,
                    Afam_ItemDesc = updateDto.Afam_ItemDesc,
                    Afam_PurchaseAmount = updateDto.Afam_PurchaseAmount,
                    Afam_PurchaseDate = updateDto.Afam_PurchaseDate,
                    Afam_PolicyNo = updateDto.Afam_PolicyNo,
                    Afam_Amount = updateDto.Afam_Amount,
                    Afam_Location = updateDto.Afam_Location,
                    Afam_Division = updateDto.Afam_Division,
                    Afam_Department = updateDto.Afam_Department,
                    Afam_Bay = updateDto.Afam_Bay,
                    Afam_EmployeeName = updateDto.Afam_EmployeeName,
                    Afam_EmployeeCode = updateDto.Afam_EmployeeCode,
                    Afam_BrokerName = updateDto.Afam_BrokerName,
                    Afam_CompanyName = updateDto.Afam_CompanyName,
                    Afam_SupplierName = updateDto.Afam_SupplierName,
                    Afam_ContactPerson = updateDto.Afam_ContactPerson,
                    Afam_Address = updateDto.Afam_Address,
                    Afam_Phone = updateDto.Afam_Phone,
                    Afam_Fax = updateDto.Afam_Fax,
                    Afam_EmailID = updateDto.Afam_EmailID,
                    Afam_Unit = updateDto.Afam_Unit,
                    Afam_UpdatedBy = updateDto.Afam_UpdatedBy
                }, transaction);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }





    }
}

