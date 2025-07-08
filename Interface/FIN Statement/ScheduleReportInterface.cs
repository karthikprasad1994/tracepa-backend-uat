using static TracePca.Dto.FIN_Statement.ScheduleReportDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleReportInterface
    {
        //GetCustomersName
        Task<IEnumerable<CustDto>> GetCustomerNameAsync(int CompId);

        //GetFinancialYear
        Task<IEnumerable<FinancialYearDto>> GetFinancialYearAsync(int CcompId);

        //GetBranchName
        Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(int CompId, int CustId);

        //GetCompanyName
        Task<IEnumerable<CompanyDetailsDto>> GetCompanyNameAsync(int CompId);

        //GetPartners
        Task<IEnumerable<PartnersDto>> LoadCustomerPartnersAsync(int CompId, int DetailsId);

        //GetSubHeading
        Task<IEnumerable<SubHeadingDto>> GetSubHeadingAsync(
       int CompId, int ScheduleId, int CustId, int HeadingId);

        //GetItem
        Task<IEnumerable<ItemDto>> GetItemAsync(
        int CompId, int ScheduleId, int CustId, int HeadingId, int SubHeadId);

        //GetDateFormat
        Task<string> GetDateFormatSelectionAsync(int CompanyId, string ConfigKey);
    }
}
                         