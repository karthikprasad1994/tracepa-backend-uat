using TracePca.Dto.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleAccountingRatioDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleAccountingRatioInterface
    {
        //
        Task<AccountingRatioResult> LoadAccRatioAsync(int yearId, int customerId, int branchId);
        Task<ScheduleAccountingRatioDto.AccountingRatioResult> LoadAccRatioAsync(int yearId, int customerId, int branchId);

    }
}
