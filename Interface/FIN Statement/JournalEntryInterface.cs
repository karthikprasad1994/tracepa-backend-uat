using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static TracePca.Dto.FIN_Statement.JournalEntryDto;
using static TracePca.Dto.FIN_Statement.ScheduleExcelUploadDto;

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
        //Task<int> ApproveJournalEntriesAsync(ApproveRequestDto dto);
        Task<JERecordDto?> GetJERecordAsync(int jeId, int compId);
        Task<List<TransactionDetailDto>> LoadTransactionDetailsAsync(int companyId, int yearId, int custId, int jeId, int branchId, int durationId);
        Task<(int UpdateOrSave, int Oper, int FinalId, string FinalCode)> SaveOrUpdateAsync(AdminMasterDto dto);

        //GetJETypeDropDown
        Task<IEnumerable<JeTypeDto>> GetJETypeListAsync(int CompId);

        //GetJETypeDropDownDetails
        Task<IEnumerable<JETypeDropDownDetailsDto>> GetJETypeDropDownDetailsAsync(int compId, int custId, int yearId, int BranchId, int jetype);

        //SaveJEType
        Task<string> SaveOrUpdateContentForJEAsync(int? id, int compId, string description, string remarks, string Category);

        Task<JournalEntryProcessResponse> ProcessJournalEntriesAsync(JournalEntryUploadDto uploadDto);
        Task<JournalEntryProcessResponse> ValidateAndProcessExcelFileAsync(Stream fileStream, JournalEntryUploadDto uploadDto);
        Task<BatchProcessResult> ProcessBatchAsync(List<JournalEntryRowDto> batch, int customerId, int financialYearId,
            int branchId, int durationId, int accessCodeId, int userId, string ipAddress);
        Task<List<JournalEntryRowDto>> ReadExcelFileAsync(Stream fileStream);
        Task<int?> GetContentMasterIdAsync(string description, int compId);

    }
}
