using TracePca.Dto.Audit;
using TracePca.Dto.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleMappingDto;
using static TracePca.Dto.FIN_Statement.ScheduleMappingDto.TrailBalanceUploadDto;
namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleMappingInterface
    {
        //GetCustomersName
        Task<IEnumerable<CustDto>> GetCustomerNameAsync(int icompId);

        //GetFinancialYear
        Task<IEnumerable<FinancialYearDto>> GetFinancialYearAsync(int icompId);

        //GetDuration
        Task<IEnumerable<CustDurationDto>> GetDurationAsync(int compId, int custId);

        //GetBranchName
        Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(int compId, int custId);

        //GetScheduleHeading
        Task<IEnumerable<ScheduleHeadingDto>> GetScheduleHeadingAsync(int compId, int custId, int scheduleTypeId);

        //GetScheduleSub-Heading
        Task<IEnumerable<ScheduleSubHeadingDto>> GetScheduleSubHeadingAsync(int compId, int custId, int scheduleTypeId);

        //GetScheduleItem
        Task<IEnumerable<ScheduleItemDto>> GetScheduleItemAsync(int compId, int custId, int scheduleTypeId);

        //GetScheduleSub-Item
        Task<IEnumerable<ScheduleSubItemDto>> GetScheduleSubItemAsync(int compId, int custId, int scheduleTypeId);

        //SaveOrUpdateTrialBalanceUpload
        Task<int[]> SaveTrailBalanceUploadAsync(int iCompId, TrailBalanceUploadDto dto);

        //SaveOrUpdateTrialBalanceUploadDetails
        Task<int[]> SaveTrailBalanceUploadDetailsAsync(int iCompId, TrailBalanceUploadDetailsDto dto);

        //GetTotalAmount
        Task<IEnumerable<CustCOASummaryDto>> GetCustCOAMasterDetailsAsync(int compId, int custId, int yearId, int branchId, int durationId);

        //GetTrialBalance(Grid)
        Task<IEnumerable<CustCOADetailsDto>> GetCustCOADetailsAsync(
        int compId, int custId, int yearId, int scheduleTypeId, int unmapped, int branchId, int durationId);

        //SaveScheduleTemplate
        Task<int[]> UploadTrialBalanceExcelAsync(int companyId, AccTrailBalanceUploadBatchDto dto);

        //UploadExcelFile
        //Task<ExcelUploadResultDto> UploadScheduleExcelAsync(IFormFile file, int clientId, int branchId, int yearId, int quarter, string accessCode, int accessCodeId, string username);

        //FreezeForPreviousDuration
        Task<int[]> FreezePreviousYearTrialBalanceAsync(FreezePreviousDurationRequestDto input);

        //FreezeForNextDuration
        Task<int[]> FreezeNextDurationrialBalanceAsync(FreezeNextDurationRequestDto input);

        //DownloadUploadableExcelAndTemplate
        FileDownloadResult GetExcelTemplate();
    }
}
