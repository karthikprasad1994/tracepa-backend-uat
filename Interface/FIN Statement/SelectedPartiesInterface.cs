using static TracePca.Dto.FIN_Statement.SelectedPartiesDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface SelectedPartiesInterface
    {

        //GetSelectedParties
        Task<IEnumerable<LoadTrailBalanceDto>> GetTrailBalanceAsync(int custId, int financialYearId, int branchId);

        //UpdateSelectedPartiesStatus
        Task<int> UpdateTrailBalanceStatusAsync(UpdateTrailBalanceStatusDto dto);

    }
}
