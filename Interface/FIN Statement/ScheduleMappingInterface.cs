using Microsoft.AspNetCore.Mvc;
using TracePca.Dto.Audit;
using TracePca.Dto.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleMappingDto;
namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleMappingInterface
    {

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
        Task<IEnumerable<CustCOADetailsDto>> GetCustCOADetailsAsync(int CompId, int CustId, int YearId, int ScheduleTypeId, int Unmapped, int BranchId, int DurationId);

        //FreezeForPreviousDuration
        Task<int[]> FreezePreviousDurationTrialBalanceAsync(List<FreezePreviousYearTrialBalanceDto> inputList);

        //FreezeForNextDuration
        Task<int[]> FreezeNextDurationTrialBalanceAsync(List<FreezeNextYearTrialBalanceDto> inputList);

        //DownloadUploadableExcelAndTemplate
        FileDownloadResult GetExcelTemplate();

        //CheckTrailBalanceRecordExists
        Task<bool> CheckTrailBalanceRecordExistsAsync(int CompId, int CustId, int YearId, int BranchId, int QuarterId);

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
        Task<(int? HeadingId, int? SubHeadingId, int? ItemId)> GetPreviousLoadIdAsync(int? subItemId = null, int? itemId = null, int? subHeadingId = null);

        //UpdateNetIncome
        Task<bool> UpdateNetIncomeAsync(int compId, int custId, int userId, int yearId, string branchId, int durationId);

        //SaveMappingTransactionDetails
        Task<int[]> SaveMappingTransactionDetailsAsync(SaveMappingTransactionDetailsDto dto);

        //GetCustomerTrailBalance
        Task<IEnumerable<CustomerCOADto>> GetCustomerTBAsync(int compId, int yearId, int custId, int orgType);

        //GetVODTotalGrid
        Task<CustCOATrialBalanceResult> GetCustCOAMasterDetailsCustomerAsync(int compId, int custId, int yearId, int scheduleTypeId, int unmapped, int branchId);
    }
}
