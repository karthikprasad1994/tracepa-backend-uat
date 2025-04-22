using TracePca.Dto;

namespace TracePca.Helpers
{
    public static class AssetHelpers
    {
        public static string GenerateAssetCode(string lastAssetCode)
        {
            var newAssetCode = $"FAR_{(int.Parse(lastAssetCode.Split('_')[1]) + 1):D3}";
            return newAssetCode;
        }


        public static string GenerateAfamItemDesc(AssetExcelUploadDto asset)
        {
            return $"{asset.TransactionNo} - {asset.AmDescription}";
        }
    }
}
