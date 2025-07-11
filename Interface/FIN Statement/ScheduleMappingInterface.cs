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

        //SaveTrailbalnceDetails
        Task<int[]> SaveTrailBalanceDetailsAsync(int CompId, List<TrailBalanceDetailsDto> dtos);

        //UpdateTrailBalnce
        Task<List<int>> UpdateTrailBalanceAsync(List<UpdateTrailBalanceDto> dtos);

        //LoadSubHeadingByHeadingDto
        Task<IEnumerable<LoadSubHeadingByHeadingDto>> GetSubHeadingsByHeadingIdAsync(int headingId, int orgType);

        //LoadItemBySubHeadingDto
        Task<IEnumerable<LoadItemBySubHeadingDto>> GetItemsBySubHeadingIdAsync(int subHeadingId, int orgType);

        //LoadSubItemByItemDto
        Task<IEnumerable<LoadSubItemByItemDto>> GetSubItemsByItemIdAsync(int itemId, int orgType);

        //GetPreviousLoadId
        Task<(int? HeadingId, int? SubHeadingId, int? ItemId)> GetPreviousLoadIdAsync(
  int? subItemId = null, int? itemId = null, int? subHeadingId = null);
    }
}
