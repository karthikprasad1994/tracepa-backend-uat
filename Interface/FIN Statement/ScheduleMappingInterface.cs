using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.Audit;
using TracePca.Dto.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleMappingDto;
namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleMappingInterface
    {

        //GetScheduleHeading
        Task<IEnumerable<ScheduleHeadingDto>> GetScheduleHeadingAsync(string DBName, int CompId, int CustId, int ScheduleTypeId);

        //GetScheduleSub-Heading
        Task<IEnumerable<ScheduleSubHeadingDto>> GetScheduleSubHeadingAsync(string DBName, int CompId, int CustId, int ScheduleTypeId);

        //GetScheduleItem
        Task<IEnumerable<ScheduleItemDto>> GetScheduleItemAsync(string DBName, int CompId, int CustId, int CcheduleTypeId);

        //GetScheduleSub-Item
        Task<IEnumerable<ScheduleSubItemDto>> GetScheduleSubItemAsync(string DBName, int CompId, int CustId, int ScheduleTypeId);

        //GetTotalAmount
        Task<IEnumerable<CustCOASummaryDto>> GetCustCOAMasterDetailsAsync(string DBName, int CompId, int CustId, int YearId, int BranchId, int DurationId);

        //GetTrialBalance(Grid)
        Task<IEnumerable<CustCOADetailsDto>> GetCustCOADetailsAsync(string DBName,
        int CompId, int CustId, int YearId, int ScheduleTypeId, int Unmapped, int BranchId, int DurationId);

        //FreezeForPreviousDuration
        Task<int[]> FreezePreviousYearTrialBalanceAsync(string DBName, FreezePreviousDurationRequestDto input);

        //FreezeForNextDuration
        Task<int[]> FreezeNextDurationrialBalanceAsync(string DBName, FreezeNextDurationRequestDto input);

        //DownloadUploadableExcelAndTemplate
        FileDownloadResult GetExcelTemplate();

        //SaveTrailbalnceDetails
        Task<int[]> SaveTrailBalanceDetailsAsync(string DBName, int CompId, List<TrailBalanceDetailsDto> dtos);

        //UpdateTrailBalnce
        Task<List<int>> UpdateTrailBalanceAsync(string DBName, List<UpdateTrailBalanceDto> dtos);

        //LoadSubHeadingByHeadingDto
        Task<IEnumerable<LoadSubHeadingByHeadingDto>> GetSubHeadingsByHeadingIdAsync(string DBName, int headingId, int orgType);

        //LoadItemBySubHeadingDto
        Task<IEnumerable<LoadItemBySubHeadingDto>> GetItemsBySubHeadingIdAsync(string DBName, int subHeadingId, int orgType);

        //LoadSubItemByItemDto
        Task<IEnumerable<LoadSubItemByItemDto>> GetSubItemsByItemIdAsync(string DBName, int itemId, int orgType);

        //GetPreviousLoadId
        Task<(int? HeadingId, int? SubHeadingId, int? ItemId)> GetPreviousLoadIdAsync(string DBName,
  int? subItemId = null, int? itemId = null, int? subHeadingId = null);
    }
}
