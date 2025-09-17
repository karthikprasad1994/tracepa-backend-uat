using static TracePca.Dto.FIN_Statement.ScheduleNoteDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleNoteInterface
    {

        //GetSubHeadingname(Notes For SubHeading)
        Task<IEnumerable<SubHeadingNoteDto>> GetSubHeadingDetailsAsync(int CustomerId, int SubHeadingId);

        //SaveOrUpdateSubHeadingNotes(Notes For SubHeading)
        Task<int[]> SaveSubHeadindNotesAsync(SubHeadingNotesDto dto);

        //GetBranch(Notes For Ledger)
        Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(int CompId, int CustId);

        //GetLedger(Notes For Ledger)
        Task<IEnumerable<LedgerIndividualDto>> GetLedgerIndividualDetailsAsync(int CustomerId, int SubHeadingId);

        //SaveOrUpdateLedger(Notes For Ledger)
        Task<int[]> SaveLedgerDetailsAsync(SubHeadingLedgerNoteDto dto);

        //DownloadNotesExcel
        ExcelFileDownloadResult GetExcelTemplate();

        //DownloadNotesPdf
        PdfFileDownloadResult GetPdfTemplate();

        //SaveOrUpdate
        Task<int> SaveFirstScheduleNoteDetailsAsync(FirstScheduleNoteDto dto);

        // --PreDefinied Notes //
        //SaveShareCapital(Particulars)
        Task<int> SaveAuthorisedShareCapitalAsync(AuthorisedShareCapitalDto dto);

        //SaveIssuedSubscribedandFullyPaidupShareCapital
        Task<int> SaveIssuedSubscribedandFullyPaidupShareCapitalAsync(IssuedSubscribedandFullyPaidupShareCapitalAsyncDto dto);

        //Save(A)Issued
        Task<int> SaveIssuedAsync(IssuedDto dto);

        //Save(B)SubscribedandPaid-up
        Task<int> SaveSubscribedandPaidupAsync(SubscribedandPaidupDto dto);

        //SaveCallsUnpaid
        Task<int> SaveCallsUnpaidAsync(CallsUnpaidDto dto);

        //SaveForfeitedShares
        Task<int> SaveForfeitedSharesAsync(ForfeitedSharesDto dto);

        //Save(i)EquityShares
        Task<int> SaveEquitySharesAsync(EquitySharesDto dto);

        //Save(ii)PreferenceShares
        Task<int> SavePreferenceSharesAsync(PreferenceSharesDto dto);

        //Save(iii)EquityShares
        Task<int> SaveiiiEquitySharesAsync(iiiEquitySharesDto dto);

        //Save(iv)PreferenceShares
        Task<int> SaveivPreferenceSharesAsync(ivPreferenceSharesDto dto);

        //Save(b)EquityShareCapital
        Task<int> SaveThirdScheduleNoteDetailsAsync(EquityShareCapitalDto dto);
    }
}
