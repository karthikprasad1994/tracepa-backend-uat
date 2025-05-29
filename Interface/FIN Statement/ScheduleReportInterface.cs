using static TracePca.Dto.FIN_Statement.ScheduleReportDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleReportInterface
    {
        //GetCustomersName
        Task<IEnumerable<CustDto>> GetCustomerNameAsync(int icompId);

        //GetFinancialYear
        Task<IEnumerable<FinancialYearDto>> GetFinancialYearAsync(int icompId);

        //GetBranchName
        Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(int compId, int custId);

        //GetCompanyName
        Task<IEnumerable<CompanyDetailsDto>> GetCompanyNameAsync(int iCompId);

        //GetPartners
        Task<IEnumerable<PartnersDto>> LoadCustomerPartnersAsync(int compId, int detailsId);

        //GetSubHeading
        Task<IEnumerable<SubHeadingDto>> GetSubHeadingAsync(
       int iCompId, int iScheduleId, int iCustId, int iHeadingId);

        //GetItem
        Task<IEnumerable<ItemDto>> GetItemAsync(
        int iCompId, int iScheduleId, int iCustId, int iHeadingId, int iSubHeadId);

        //GetDateFormat
        Task<string> GetDateFormatSelectionAsync(int companyId, string configKey);

        //LoadButton
        Task<IEnumerable<ReportDto>> GenerateReportAsync(int reportType, int scheduleTypeId, int accountId, int customerId, int yearId);
    }
}
