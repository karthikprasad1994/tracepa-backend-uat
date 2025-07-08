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

        //SaveOpeningBalance
        Task<List<int>> SaveOpeningBalanceAsync(List<OpeningBalanceDto> dtos);

        //SaveTrailBalance
        Task<List<int>> SaveTrailBalanceAsync(List<TrailBalanceDto> dtos);
    }
}
