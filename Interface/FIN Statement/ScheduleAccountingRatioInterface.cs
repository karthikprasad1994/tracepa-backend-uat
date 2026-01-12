using TracePca.Dto.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleAccountingRatioDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleAccountingRatioInterface
    {
        Task<ScheduleAccountingRatioDto.AccountingRatioResult> LoadAccRatioAsync(int yearId, int customerId);
    }
}
