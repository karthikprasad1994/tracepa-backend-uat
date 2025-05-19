using TracePca.Dto.Audit;
using TracePca.Dto.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleMappingDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleMappingInterface
    {
        //GetCustomersName
        Task<IEnumerable<CustDto>> GetCustomerNameAsync(int icompId);

        //GetFinancialYear
        Task<IEnumerable<FinancialYearDto>> GetFinancialYearAsync(int icompId);

        //GetDuration
        Task<IEnumerable<CustDurationDto>> GetDurationAsync(int compId, int custId);

        //GetBranchName
        Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(int compId, int custId);

        Task<List<ScheduleHeadingDto>> GetScheduleHeadingsAsync(int compId, int custId, int ScheduleTypeId);
        Task<List<ScheduleHeadingDto>> GetSchedulesubHeadingsAsync(int compId, int custId, int ScheduleTypeId);
        Task<List<ScheduleHeadingDto>> GetScheduleItemAsync(int compId, int custId, int ScheduleTypeId);
        Task<List<ScheduleHeadingDto>> GetScheduleSubItemAsync(int compId, int custId, int ScheduleTypeId);
        Task<int> SaveTrailBalanceExcelUploadAsync(string sAC, TrailBalanceUploadDto dto, int userId);
        Task<int> SaveTrailBalanceDetailAsync(string dbName, TrailBalanceUploadDetailDto dto, int userId);

    }
}
