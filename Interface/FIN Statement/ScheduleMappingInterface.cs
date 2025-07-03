using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.Audit;
using TracePca.Dto.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleMappingDto;
namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleMappingInterface
    {
        //GetCustomersName
        Task<IEnumerable<CustDto>> GetCustomerNameAsync(int CompId);

        //GetFinancialYear
        Task<IEnumerable<FinancialYearDto>> GetFinancialYearAsync(int CompId);

        //GetDuration
        Task<int?> GetCustomerDurationIdAsync(int CompId, int CustId);

        //GetBranchName
        Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(int CompId, int CustId);

        //GetScheduleHeading
        Task<IEnumerable<ScheduleHeadingDto>> GetScheduleHeadingAsync(int CompId, int CustId, int ScheduleTypeId);

        //GetScheduleSub-Heading
        Task<IEnumerable<ScheduleSubHeadingDto>> GetScheduleSubHeadingAsync(int CompId, int CustId, int ScheduleTypeId);

        //GetScheduleItem
        Task<IEnumerable<ScheduleItemDto>> GetScheduleItemAsync(int CompId, int CustId, int CcheduleTypeId);

        //GetScheduleSub-Item
        Task<IEnumerable<ScheduleSubItemDto>> GetScheduleSubItemAsync(int CompId, int CustId, int ScheduleTypeId);

        ////SaveOrUpdateTrialBalanceUpload
        //Task<int[]> SaveTrailBalanceUploadAsync(int iCompId, TrailBalanceUploadDto dto);

        ////SaveOrUpdateTrialBalanceUploadDetails
        //Task<int[]> SaveTrailBalanceUploadDetailsAsync(int iCompId, TrailBalanceUploadDetailsDto dto);

        //GetTotalAmount
        Task<IEnumerable<CustCOASummaryDto>> GetCustCOAMasterDetailsAsync(int CompId, int CustId, int YearId, int BranchId, int DurationId);

        //GetTrialBalance(Grid)
        Task<IEnumerable<CustCOADetailsDto>> GetCustCOADetailsAsync(
        int CompId, int CustId, int YearId, int ScheduleTypeId, int Unmapped, int BranchId, int DurationId);

        //SaveScheduleTemplate
        Task<int[]> UploadTrialBalanceExcelAsync(int CompanyId, AccTrailBalanceUploadBatchDto dto);

        //UploadExcelFile
        //Task<ExcelUploadResultDto> UploadScheduleExcelAsync(IFormFile file, int clientId, int branchId, int yearId, int quarter, string accessCode, int accessCodeId, string username);

        //FreezeForPreviousDuration
        Task<int[]> FreezePreviousYearTrialBalanceAsync(FreezePreviousDurationRequestDto input);

        //FreezeForNextDuration
        Task<int[]> FreezeNextDurationrialBalanceAsync(FreezeNextDurationRequestDto input);

        //DownloadUploadableExcelAndTemplate
        FileDownloadResult GetExcelTemplate();

        //SaveTrailBalanceDetails
        Task<int[]> SaveTrailBalanceDetailsAsync(int CompId, TrailBalanceDetailsDto HeaderDto);
    }
}
