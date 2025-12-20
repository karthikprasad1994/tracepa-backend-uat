using static TracePca.Dto.FixedAssets.AssetTransactionAdditionDto;

namespace TracePca.Interface.FixedAssetsInterface
{
    public interface AssetTransactionAdditionInterface
    {
        //LoadCustomer
        Task<IEnumerable<CustDto>> LoadCustomerAsync(int CompId);

        //LoadStatus
        Task<IEnumerable<StatusDto>> LoadStatusAsync(int compId, string Name);

        //FinancialYear
        Task<IEnumerable<YearDto>> GetYearsAsync(int compId);


    }
}
