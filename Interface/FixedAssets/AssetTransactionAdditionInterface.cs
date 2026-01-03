using TracePca.Dto.Audit;
using static TracePca.Dto.FixedAssets.AssetCreationDto;
using static TracePca.Dto.FixedAssets.AssetTransactionAdditionDto;

namespace TracePca.Interface.FixedAssetsInterface
{
    public interface AssetTransactionAdditionInterface
    {
        ////LoadCustomer
        //Task<IEnumerable<CustDto>> LoadCustomerAsync(int CompId);

        ////LoadStatus
        //Task<IEnumerable<StatusDto>> LoadStatusAsync(int compId, string Name);

        ////FinancialYear
        //Task<IEnumerable<YearDto>> GetYearsAsync(int compId);

        //AddDetails
        //Task<List<AssetAdditionDetailsDto>> AddAssetDetailsAsync(AddAssetDetailsRequest request);

        //LoadAsset
        Task<IEnumerable<AssettTypeDto>> LoadFxdAssetTypeAsync(int compId, int custId);
        //SaveTransactionAddition
        Task<int> SaveTransactionAssetAndAuditAsync(ClsAssetTransactionAdditionDto asset, TransactionAuditDto audit);

        //LoadVoucherNo(transactionno)
        Task<IEnumerable<AssetTransactionDto>> ExistingTransactionNoAsync(int compId, int yearId, int custId);

        //ExcelUpload
        AssetAdditionResult GetAssetAdditionExcelTemplate();

        //SaveDetails
        Task<(int UpdateOrSave, int Oper)> SaveFixedAssetAsync( ClsAssetOpeningBalExcelUpload header,ClsAssetTransactionAddition details,AuditLogDto audit);
    }
}
