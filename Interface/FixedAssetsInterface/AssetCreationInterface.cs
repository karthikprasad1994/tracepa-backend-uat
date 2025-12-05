using static TracePca.Dto.FixedAssets.AssetCreationDto;
//using static TracePca.Dto.FixedAssets.AssetMasterdto;

namespace TracePca.Interface.FixedAssetsInterface
{
    public interface AssetCreationInterface
    {
        //LoadAssetClass
        Task<IEnumerable<AssetTypeDto>> LoadAssetTypeAsync(int compId, int custId);

        //New
        Task<int> AddNewAssetAsync(NewDto asset);

        //Search
        Task<IEnumerable<AssetRegisterDto>> LoadAssetRegisterAsync(int compId, int assetTypeId, int yearId, int custId);
    }
}
