using static TracePca.Dto.FIN_Statement.ScheduleAccountingRatioDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleAccountingRatioInterface
    {

        //GetAccoutingRatio
        Task<Ratio1Dto> GetAccountingRatioAsync(int yearId, int customerId);

        //GetAccoutingRatio2
        Task<Ratio2Dto> GetBorrowingsVsShareholdersAsync(int yearId, int customerId, int branchId);
    }
}
