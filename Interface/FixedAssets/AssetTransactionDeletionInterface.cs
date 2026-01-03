using static TracePca.Dto.FixedAssets.AssetTransactionDeletionnDto;


namespace TracePca.Interface.FixedAssetsInterface
{
    public interface AssetTransactionDeletionInterface
    {
        //Deletee
        Task<(int UpdateOrSave, int Oper)> SaveFixedAssetDeletionnAsync(AssetDeletionDto dto, AaudittDto audit);
    }
}
