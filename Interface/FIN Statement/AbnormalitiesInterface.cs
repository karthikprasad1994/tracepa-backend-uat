using static TracePca.Dto.FIN_Statement.AbnormalitiesDto;
using static TracePca.Dto.FIN_Statement.SelectedPartiesDto;
namespace TracePca.Interface.FIN_Statement
{
    public interface AbnormalitiesInterface
    {

        //GetAbnormalTransactions
        Task<IEnumerable<AbnormalTransactionsDto>> GetAbnormalTransactionsAsync(int iCustId, int iBranchId, int iYearID, int iAbnormalType, string sAmount);

        //UpdateAEStatus
        Task<int> UpdateJournalEntrySeqRefAsync(List<UpdateJournalEntrySeqRef1Dto> dtoList);
    }
}
