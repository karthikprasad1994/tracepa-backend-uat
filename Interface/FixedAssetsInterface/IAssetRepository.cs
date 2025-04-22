using TracePca.Dto;

namespace TracePca.Interface.FixedAssetsInterface
{
    public interface IAssetRepository
    {
        //Task InsertAssetAsync(Acc_FixedAssetMaster asset); // Keep this method for single insertion if needed
        Task InsertAssetsAsync(List<AssetExcelUploadDto> assets);
    }
}
