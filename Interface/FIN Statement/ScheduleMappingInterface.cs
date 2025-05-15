using TracePca.Dto.Audit;
using TracePca.Dto.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleCustDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleMappingInterface
    {
        Task<List<CustDto>> LoadCustomers(int compId);
        Task<List<FinancialYearDto>> LoadFinancialYear(int compId);
        Task<List<CustBranchDto>> LoadBranches(int compId, int custId);
        Task<List<CustDurationDto>> LoadDuration(int compId, int custId);
        Task<List<ScheduleHeadingDto>> GetScheduleHeadingsAsync(int compId, int custId, int ScheduleTypeId);
        Task<List<ScheduleHeadingDto>> GetSchedulesubHeadingsAsync(int compId, int custId, int ScheduleTypeId);
        Task<List<ScheduleHeadingDto>> GetScheduleItemAsync(int compId, int custId, int ScheduleTypeId);
        Task<List<ScheduleHeadingDto>> GetScheduleSubItemAsync(int compId, int custId, int ScheduleTypeId);                    
        Task<int> SaveTrailBalanceExcelUploadAsync(string sAC, TrailBalanceUploadDto dto, int userId);      
        Task<int> SaveTrailBalanceDetailAsync(string dbName, TrailBalanceUploadDetailDto dto, int userId);

        //SaveScheduleFormatHeading
        Task<int[]> SaveScheduleHeadingAndTemplateAsync(int iCompId, SaveScheduleFormatHeadingDto dto);

    }
}
