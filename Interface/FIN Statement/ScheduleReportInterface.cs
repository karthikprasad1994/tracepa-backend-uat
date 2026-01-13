using TracePca.Dto.Audit;
using TracePca.Dto.FIN_Statement;
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
        Task<IEnumerable<SubHeadingDto>> GetSubHeadingAsync(int CompId, int ScheduleId, int CustId, int HeadingId);

        //GetItem
        Task<IEnumerable<ItemDto>> GetItemAsync(int CompId, int ScheduleId, int CustId, int HeadingId, int SubHeadId);

        //GetSummaryReportForPandL
        Task<IEnumerable<SummaryReportPnLRow>> GetReportSummaryPnLAsync(int CompId, SummaryReportPnL p);

        //GetSummaryReportForBalanceSheet
        Task<IEnumerable<SummaryReportBalanceSheetRow>> GetReportSummaryBalanceSheetAsync(int CompId, SummaryReportBalanceSheet p);

        //GetDetailedReportPandL
        Task<IEnumerable<DetailedReportPandLRow>> GetDetailedReportPandLAsync(int CompId, DetailedReportPandL p);

        //GetDetailedReportBalanceSheet
        Task<IEnumerable<DetailedReportBalanceSheetRow>> GetDetailedReportBalanceSheetAsync(int CompId, DetailedReportBalanceSheet p);

        Task<ScheduleReportResponseDto> GetScheduleReportDetailsAsync(ScheduleReportRequestDto request);
        Task<OrgTypeResponseDto> GetOrgTypeAndMembersAsync(int customerId, int companyId);
        Task<List<CompanyDto>> LoadCompanyDetailsAsync(int compId);

        //UpdatePnL
        Task<bool> UpdatePnLAsync(string pnlAmount, int compId, int custId, int userId, int yearId, string branchId, int durationId);
        Task<(int iUpdateOrSave, int iOper)> SaveCustomerStatutoryPartnerAsync(StatutoryPartnerDto partnerDto);
        Task<(int iUpdateOrSave, int iOper)> SaveCustomerStatutoryDirectorAsync(StatutoryDirectorDto directorDto);

        Task<DirectorDto> GetDirectorByIdAsync(int directorId);
        Task<CustomerAmountSettingsDto> GetCustomerAmountSettingsAsync(int customerId);

        //SaveFinancialStatement
        Task<int> SaveOrUpdateFinancialStatementAsync(SREngagementPlanDetailsDTO dto);
    }
}
                         