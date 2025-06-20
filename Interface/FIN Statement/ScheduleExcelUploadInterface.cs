using static TracePca.Dto.FIN_Statement.ScheduleExcelUploadDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleExcelUploadInterface
    {
        //DownloadUploadableExcelAndTemplate
        FileDownloadResult GetExcelTemplate();

        //GetCustomersName
        Task<IEnumerable<CustDto>> GetCustomerNameAsync(int icompId);

        //GetFinancialYear
        Task<IEnumerable<FinancialYearDto>> GetFinancialYearAsync(int icompId);

        //GetDuration
        Task<int?> GetCustomerDurationIdAsync(int compId, int custId);
        
        //GetBranchName
        Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(int compId, int custId);

        //SaveAllInformation
        Task<int[]> SaveAllInformationAsync(UploadExcelRequestDto request);
    }
}
