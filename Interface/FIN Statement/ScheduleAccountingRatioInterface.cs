using TracePca.Dto.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleAccountingRatioDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleAccountingRatioInterface
    {
<<<<<<< HEAD
        //
        Task<AccountingRatioResult> LoadAccRatioAsync(int yearId, int customerId, int branchId);
=======
        Task<ScheduleAccountingRatioDto.AccountingRatioResult> LoadAccRatioAsync(int yearId, int customerId, int branchId);
>>>>>>> 2ed7780949550b8113b9d4ee2a732733d8fcb143
    }
}
