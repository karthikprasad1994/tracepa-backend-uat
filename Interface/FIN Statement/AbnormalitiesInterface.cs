using static TracePca.Dto.FIN_Statement.AbnormalitiesDto;
using static TracePca.Dto.FIN_Statement.SelectedPartiesDto;
namespace TracePca.Interface.FIN_Statement
{
    public interface AbnormalitiesInterface
    {

        //GetAbnormalTransactions
        Task<PagedResult<AbnormalTransactionsDto>> GetAbnormalTransactionsAsync(
              int iCustId,
              int iBranchId,
              int iYearID,
              int iAbnormalType,
              decimal amount,                 // ✅ FIX: was string
              string searchTerm = "",
              int pageNumber = 1,
              int pageSize = 10,
              string sortColumn = "AJTB_CreatedOn",
              string sortDirection = "DESC");

        //UpdateAEStatus
        Task<int> UpdateJournalEntrySeqRefAsync(List<UpdateJournalEntrySeqRef1Dto> dtoList);
        Task<byte[]> DownloadAbnormalTransactionsExcelAsync(
    int iCustId,
    int iBranchId,
    int iYearID,
    int iAbnormalType,
    decimal sAmount,
    string searchTerm = "");
        Task<byte[]> DownloadAbnormalTransactionsExcelAsync(
    int iCustId,
    int iBranchId,
    int iYearID,
    int iAbnormalType,
    string sAmount,
    string searchTerm = "");
        Task<List<JeTransactionDetailDto>> GetJeTransactionDetailsAsync(int masId);

    }
}
