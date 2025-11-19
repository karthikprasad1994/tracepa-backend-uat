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
        Task<List<int>> SaveCashFlowCategory1Async(List<CashFlowCategory1> dtos);

        //SaveCashFlow(Category3)
        Task<List<int>> SaveCashFlowCategory3Async(List<CashFlowCategory3> dtos);

        //SaveCashFlow(Category4)
        Task<List<int>> SaveCashFlowCategory4Async(List<CashFlowCategory4> dtos);

        //SaveCashFlow(Category2)
        Task<List<int>> SaveCashFlowCategory2Async(List<CashFlowCategory2> dtos);

        //SaveCashFlow(Category5)
        Task<List<int>> SaveCashFlowCategory5Async(List<CashFlowCategory5> dtos);
    }
}
