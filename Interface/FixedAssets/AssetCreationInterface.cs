//using TracePca.Dto;
using static TracePca.Dto.FixedAssets.AssetCreationDto;
//using static TracePca.Dto.FixedAssets.AssetMasterdto;

namespace TracePca.Interface.FixedAssetsInterface
{
    public interface AssetCreationInterface
    {
        //DownloadExcel
        AssetMasterResult GetAssetMasterExcelTemplate();

        //LoadAssetClass
        Task<IEnumerable<AssetTypeDto>> LoadAssetTypeAsync(int compId, int custId);

        //New
        Task<int> AddNewAssetAsync(NewDto asset);

        //Search
        Task<IEnumerable<AssetRegisterDto>> LoadAssetRegisterAsync(int compId, int assetTypeId, int yearId, int custId);

        //LoadUnitsofMeasurement
        Task<IEnumerable<UnitMeasureDto>> LoadUnitsOfMeasureAsync(int compId);

        //LoadSuplierName
        Task<IEnumerable<SupplierDto>> LoadExistingSupplierAsync(int compId, int supplierId);

        //EditSuplierName
        Task<IEnumerable<EditSupplierDetailsDto>> EditSupplierDetailsAsync(int compId, int supplierId);

        //SaveSuplierDetails
        Task<(int iUpdateOrSave, int iOper)> SaveSupplierDetailsAsync(SaveSupplierDto model);

        //SaveAsset
        //Task<(int iUpdateOrSave, int iOper)> SaveFixedAssetAsync(SaveFixedAssetDto model);
        //Task SaveGRACeFormOperationsAsync(GRACeFormOperationDto model);


        //  Task<int[]> SaveFixedAssetWithAuditAsync(SaveFixedAssetDto asset, GRACeFormOperationDto audit);

        //------------

        Task<int> SaveFixedAssetAsync(FixedAssetDto asset, AuditDto audit);

    }
}
