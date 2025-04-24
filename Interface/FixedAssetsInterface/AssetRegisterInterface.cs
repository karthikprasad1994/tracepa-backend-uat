using TracePca.Dto.AssetRegister;

namespace TracePca.Interface.FixedAssetsInterface
{
    public interface AssetRegisterInterface
    {
        Task<IEnumerable<AssetDetailsDto>> GetAssetDetailsAsync(int customerId, int assetClassId, int financialYearId);
        Task<AssetRegDetailsDto> GetAssetRegDetailsAsync(int assetId);
        Task UpdateAssetDetailsAsync(int afamId, AssetUpdateDto updateDto);

    }
}
