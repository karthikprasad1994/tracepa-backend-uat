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
        Task<int> SaveFixedAssetAsync(FixedAssetDto asset, AuditDto audit);

        //Generete
        Task<string> GenerateAssetCodeAsync(int compId, int custId, int locationId, int divisionId, int departmentId, int bayId, string assetCode);

        //UploadAssetCreationExcel
        Task<List<string>> UploadAssetExcelAsync(
            int compId, int custId, int yearId, int userId,
            IFormFile file);

    }
}
