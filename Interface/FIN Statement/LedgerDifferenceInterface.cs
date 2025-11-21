using static TracePca.Dto.FIN_Statement.LedgerDifferenceDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface LedgerDifferenceInterface
    {

        //GetDescriptionWiseDetails
        Task<IEnumerable<DescriptionWiseDetailsDto>> GetDescriptionWiseDetailsAsync(int compId, int custId, int branchId, int yearId, int typeId);

        //UpdateDescriptionWiseDetailsStatus
        Task<int> UpdateTrailBalanceStatusAsync(List<UpdateDescriptionWiseDetailsStatusDto> dtoList);

        //GetDescriptionDetails
        Task<IEnumerable<DescriptionDetailsDto>> GetDescriptionDetailsAsync(int compId, int custId, int branchId, int yearId, int typeId, int pkId);
    }
}
