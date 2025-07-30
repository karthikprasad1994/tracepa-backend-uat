using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static TracePca.Dto.FIN_Statement.JournalEntryDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface JournalEntryInterface
    {

        //GetJournalEntryInformation
        Task<IEnumerable<JournalEntryInformationDto>> GetJournalEntryInformationAsync(int CompId, int UserId, string Status, int CustId, int YearId, int BranchId, string DateFormat, int DurationId);

        //GetJEType 
        Task<IEnumerable<JETypeDto>> GetJETypeAsync(int CompId, string Type);

        //GetHeadOfAccounts
        Task<IEnumerable<DescheadDto>> LoadDescheadAsync(int compId, int custId, int yearId, int branchId, int durationId);

        //GetGeneralLedger
        Task<IEnumerable<SubGlDto>> LoadSubGLDetailsAsync(int compId, int custId);

        //SaveTransactionDetails
        Task<int[]> SaveJournalEntryWithTransactionsAsync(List<SaveJournalEntryWithTransactionsDto> dtos);
    }
}
