using static TracePca.Dto.FIN_Statement.ScheduleAccountingRatioDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleAccountingRatioInterface
    {

        //GetAccoutingRatio
        Task<AssetsLiabilitiesDto> GetAccountingRatioAsync(int yearId, int customerId);
    }
}
