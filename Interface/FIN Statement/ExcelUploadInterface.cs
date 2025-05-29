using static TracePca.Dto.FIN_Statement.ExcelUploadDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ExcelUploadInterface
    {
        //DownloadUploadableExcelAndTemplate
        FileDownloadResult GetExcelTemplate();

        //GetCustomersName
        Task<IEnumerable<CustDto>> GetCustomerNameAsync(int icompId);

        //GetFinancialYear
        Task<IEnumerable<FinancialYearDto>> GetFinancialYearAsync(int icompId);

        //GetDuration
        Task<IEnumerable<CustDurationDto>> GetDurationAsync(int compId, int custId);

        //GetBranchName
        Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(int compId, int custId);

        //SaveAllInformation
        Task<int[]> SaveAllInformationAsync(UploadExcelRequestDto request);
    }
}
