using static TracePca.Dto.FIN_Statement.SelectedPartiesDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface SelectedPartiesInterface
    {

        //GetSelectedParties
        Task<IEnumerable<LoadTrailBalanceDto>> GetTrailBalanceAsync(int custId, int financialYearId, int branchId);

        //UpdateSelectedPartiesStatus
        Task<int> UpdateTrailBalanceStatusAsync(List<UpdateTrailBalanceStatusDto> dtoList);

        //GetJETransactionDetails
        Task<IEnumerable<JournalEntryWithTrailBalanceDto>> GetJournalEntryWithTrailBalanceAsync(int custId, int yearId, int branchId);

        //UpdateJESeqReferenceNum
        Task<int> UpdateJournalEntrySeqRefAsync(List<UpdateJournalEntrySeqRefDto> dtoList);
    }
}
