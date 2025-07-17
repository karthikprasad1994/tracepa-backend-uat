using static TracePca.Dto.FIN_Statement.ScheduleReportDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleReportInterface
    {

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

        ////GetSummaryReportForPandL(Income)
        //Task<IEnumerable<SummaryPnLRowIncome>> GetSummaryPnLIncomeAsync(int CompId, SummaryPnLIncome p);

        ////GetSummaryReportForPandL(Expenses)
        //Task<IEnumerable<SummaryPnLRowExpenses>> GetSummaryPnLExpensesAsync(int CompId, SummaryPnLExpenses dto);

        //GetSummaryReportForPandL(Income and Expenses)
        Task<IEnumerable<SummaryReportPnLRow>> GetReportSummaryPnLAsync(int CompId, SummaryReportPnL p);

        //GetSummaryReportForBalanceSheet(Income and Expenses)
        Task<IEnumerable<SummaryReportBalanceSheetRow>> GetReportSummaryBalanceSheetAsync(int CompId, SummaryReportBalanceSheet p);
    }
}
                         