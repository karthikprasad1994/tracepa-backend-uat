using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static TracePca.Dto.FIN_Statement.JournalEntryDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface JournalEntryInterface
    {

        //GetJournalEntryInformation
        Task<IEnumerable<JournalEntryInformationDto>> GetJournalEntryInformationAsync(int CompId, int UserId, string Status, int CustId, int YearId, int BranchId, string DateFormat, int DurationId);

        //GetExistingJournalVouchers
        Task<IEnumerable<JournalEntryVoucherDto>> LoadExistingVoucherNosAsync(int compId, int yearId, int partyId, int branchId);

        //GetJEType 
        Task<IEnumerable<GeneralMasterJETypeDto>> LoadGeneralMastersAsync(int compId, string type);

        //GetHeadOfAccounts
        Task<IEnumerable<DescheadDto>> LoadDescheadAsync(int compId, int custId, int yearId, int branchId, int durationId);

        //GetGeneralLedger
        Task<IEnumerable<SubGlDto>> LoadSubGLDetailsAsync(int compId, int custId);

        //SaveOrUpdateTransactionDetails
        Task<int[]> SaveJournalEntryWithTransactionsAsync(List<SaveJournalEntryWithTransactionsDto> dtos);

        //SaveGeneralLedger
        Task<int[]> SaveGeneralLedgerAsync(int CompId, List<GeneralLedgerDto> dtos);

        //ActivateJE
        Task<int> ActivateJournalEntriesAsync(ActivateRequestDto dto);

        //DeActivateJE
        Task<int> ApproveJournalEntriesAsync(ApproveRequestDto dto);
        Task<JERecordDto?> GetJERecordAsync(int jeId, int compId);

    }
}
