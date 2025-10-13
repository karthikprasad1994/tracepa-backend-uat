using static TracePca.Dto.FIN_Statement.CashFlowDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface CashFlowInterface
    {

        ////SaveCashFlow(Category 1)
        //Task<(int UpdateOrSave, int Oper)> SaveCashFlowCategory1Async(int companyId, CashFlowCategory1 obj);

        ////SaveCashFlow(Category 2)
        //Task<int> SaveCashFlowCategory2Async(CashFlowCategory2 dto);

        ////SaveCashFlow(Category 3)
        //Task<(int UpdateOrSave, int Oper)> SaveCashFlowCategory3Async(int companyId, CashFlowCategory3 obj);

        ////SaveCashFlow(Category 4)
        //Task<(int UpdateOrSave, int Oper)> SaveCashFlowCategory4Async(int companyId, CashFlowCategory4 obj);

        ////SaveCashFlow(Category 5)
        //Task<int> SaveCashFlowCategory5Async(CashFlowCategory5 dto);

        //SaveCashFlow(Category 1)
        Task<List<int>> SaveCashFlowCategory1Async(List<CashFlowCategory1> dtos);
    }
}
