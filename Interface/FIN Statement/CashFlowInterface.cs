using TracePca.Dto.FIN_Statement;
using static TracePca.Dto.FIN_Statement.CashFlowDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface CashFlowInterface
    {
        //DeleteCashFlowCategoryWise
        Task DeleteCashflowCategory1Async(int compId, int pkId, int custId, int Category);

        //GetCashFlowID(SearchButton)
        Task<int> GetCashFlowParticularsIdAsync(int compId, string description, int custId, int branchId);

        //GetCashFlowForAllCategory
        Task<IEnumerable<CashFlowForAllCategoryDto>> GetCashFlowForAllCategoryAsync(int compId, int custId, int yearId, int branchId, int category);

        //SaveCashFlow(Category 1)
        Task<CashFlowCategory1Result> LoadCashFlowCategory1Async(int customerId, int yearId, int branchId, List<UserAdjustmentInput>? userAdjustments);
    }
}
