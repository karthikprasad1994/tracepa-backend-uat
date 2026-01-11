using TracePca.Dto.Audit;
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

        Task<CashFlowCategory1Result> LoadCashFlowCategory2Async(int customerId, int yearId, int branchId, List<UserAdjustmentInput>? userAdjustments);

        Task<CashFlowCategory1Result> LoadCashFlowCategory3Async(int customerId, int yearId, int branchId, List<UserAdjustmentInput>? userAdjustments);
        Task<CashFlowCategory1Result> LoadCashFlowCategory4Async(int customerId, int yearId, int branchId, List<UserAdjustmentInput>? userAdjustments);

        Task<CashFlowCategory1Result> LoadCashFlowCategory5Async(int customerId, int yearId, int branchId, List<UserAdjustmentInput>? userAdjustments);

        Task<IEnumerable<CashflowClientDto>> LoadCashFlowClientAsync(int compId);

        Task<IEnumerable<CashflowBranchDto>> LoadBranchDetailsAsync(int ClientID, int compId);

        Task<IEnumerable<CashflowFinacialYearDto>> LoadFinacialYearAsync(int compId);

        Task<int> SaveCashFlowAsync(CashFlowAddDto cashFlow);

        //Task<IEnumerable<CashFlowOperatingSystemDto>> GetCashFlowDetails(

        //int customerId,
        //int branchId,
        //int category,
        //int financialYearId, int companyId);
    }
}
