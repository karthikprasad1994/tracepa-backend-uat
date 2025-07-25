using static TracePca.Dto.FIN_Statement.ScheduleReportDto;


namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleReportInterface
    {

        //GetCompanyName
        Task<IEnumerable<CompanyDetailsDto>> GetCompanyNameAsync(string DBName, int CompId);

        //GetPartners
        Task<IEnumerable<PartnersDto>> LoadCustomerPartnersAsync(string DBName, int CompId, int DetailsId);

        //GetSubHeading
        Task<IEnumerable<SubHeadingDto>> GetSubHeadingAsync(string DBName,
       int CompId, int ScheduleId, int CustId, int HeadingId);

        //GetItem
        Task<IEnumerable<ItemDto>> GetItemAsync(string DBName,
        int CompId, int ScheduleId, int CustId, int HeadingId, int SubHeadId);

        ////GetSummaryReportForPandL(Income)
        //Task<IEnumerable<SummaryPnLRowIncome>> GetSummaryPnLIncomeAsync(int CompId, SummaryPnLIncome p);

        ////GetSummaryReportForPandL(Expenses)
        //Task<IEnumerable<SummaryPnLRowExpenses>> GetSummaryPnLExpensesAsync(int CompId, SummaryPnLExpenses dto);

        //GetSummaryReportForPandL
        Task<IEnumerable<SummaryReportPnLRow>> GetReportSummaryPnLAsync(string DBName, int CompId, SummaryReportPnL p);

        //GetSummaryReportForBalanceSheet
        Task<IEnumerable<SummaryReportBalanceSheetRow>> GetReportSummaryBalanceSheetAsync(string DBName, int CompId, SummaryReportBalanceSheet p);

        //GetDetailedReportPandL
        Task<IEnumerable<DetailedReportPandLRow>> GetDetailedReportPandLAsync(string DBName, int CompId, DetailedReportPandL p);

    }
}
                         