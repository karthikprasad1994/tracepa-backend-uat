using static TracePca.Dto.FIN_Statement.ScheduleMastersDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleMastersInterface
    {
        //GetCustomersName
        Task<IEnumerable<CustDto>> GetCustomerNameAsync(string DBName, int CompId);

        //GetDuration
        Task<int?> GetCustomerDurationIdAsync(string DBName, int CompId, int CustId);

        //GetFinancialYear
        Task<IEnumerable<FinancialYearDto>> GetFinancialYearAsync(string DBName, int CompId);

        //GetBranchName
        Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(string DBName, int CompId, int CustId);

        //GetScheduleHeading
        Task<IEnumerable<ScheduleHeadingDto>> GetScheduleHeadingAsync(string DBName, int CompId, int CustId, int ScheduleTypeId);

        //GetScheduleSubHeading
        Task<IEnumerable<ScheduleSubHeadingDto>> GetScheduleSubHeadingAsync(string DBName, int CompId, int CustId, int ScheduleTypeId);

        //GetScheduleItem
        Task<IEnumerable<ScheduleItemDto>> GetScheduleItemAsync(string DBName, int CompId, int CustId, int CcheduleTypeId);

        //GetScheduleSubItem
        Task<IEnumerable<ScheduleSubItemDto>> GetScheduleSubItemAsync(string DBName, int CompId, int CustId, int ScheduleTypeId);

        //GetCustomerOrgType
        Task<string> GetCustomerOrgTypeAsync(string DBName, int CustId, int CompId);
    }
}
