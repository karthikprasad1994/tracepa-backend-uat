using static TracePca.Dto.FIN_Statement.LedgerDifferenceDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface LedgerDifferenceInterface
    {
        //GetHeadWiseDetails
        Task<(IEnumerable<HeadWiseDetailsDto> ScheduleType3, IEnumerable<HeadWiseDetailsDto> ScheduleType4)>
      GetHeadWiseDetailsAsync(int compId, int custId, int branchId, int yearId, int durationId);

        //GetAccountWiseDetails
        Task<(IEnumerable<AccountWiseDetailsDto> ScheduleType3, IEnumerable<AccountWiseDetailsDto> ScheduleType4)>
       GetAccountWiseDetailsAsync(int compId, int custId, int branchId, int yearId, int durationId);
    }
}
