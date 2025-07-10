using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.ScheduleExcelUploadDto;


namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleExcelUploadInterface
    {
        //DownloadUploadableExcelAndTemplate
        FileDownloadResult GetExcelTemplate();

        //GetCustomersName
        Task<IEnumerable<CustDto>> GetCustomerNameAsync(int CompId);

        //GetFinancialYear
        Task<IEnumerable<FinancialYearDto>> GetFinancialYearAsync(int CompId);

        //GetDuration
        Task<int?> GetCustomerDurationIdAsync(int CompId, int CustId);
        
        //GetBranchName
        Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(int CompId, int CustId);

        //SaveScheduleTemplate(P and L)
        Task<List<int>> SaveSchedulePandLAsync(List<ScheduleTemplatePandLDto> dtos);

        //SaveScheduleTemplate(Balance Sheet)
        Task<List<int>> SaveScheduleBalanceSheetAsync(List<ScheduleTemplateBalanceSheetDto> dtos);

        //SaveOpeningBalance
        Task<List<int>> SaveOpeningBalanceAsync(List<OpeningBalanceDto> dtos);

        //SaveTrailBalance
        Task<List<int>> SaveTrailBalanceAsync(List<TrailBalanceDto> dtos);

        //SaveClientTrailBalance
        Task<List<int>> ClientTrailBalanceAsync(List<ClientTrailBalance> items);

        //SaveJournalEntry
        Task<List<int>> SaveCompleteTrailBalanceAsync(List<TrailBalanceCompositeModel> models);
    }
}
