using System.Data;
using static TracePca.Dto.FIN_Statement.ScheduleNoteDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleNoteInterface
    {

        //GetSubHeadingname(Notes For SubHeading)
        Task<IEnumerable<SubHeadingNoteDto>> GetSubHeadingDetailsAsync(int CompId, int CustId);

        //SaveOrUpdateSubHeadingNotes(Notes For SubHeading)
        //Task<int[]> SaveSubHeadindNotesAsync(SubHeadingNotesDto dto);

        //SaveOrUpdateSubHeadingNotes(Notes For SubHeading)
        Task<List<SaveSubheadingDto>> SaveNotesUsingExistingSubHeadingAsync(List<SaveSubheadingDto> subheadingDtos);

        //LoadGrid(Notes For SubHeading)
        Task<List<SubheadingNoteLoadDto>> LoadSubheadingNotesAsync(int compId, int yearId, int custId);

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
        //SaveAuthorisedShareCapital(Particulars)
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
        Task<int> SavebEquityShareCapitalAsync(EquityShareCapitalDto dto);

        //Save(b)PreferenceShareCapital
        Task<int> SavebPreferenceShareCapitalAsync(PreferenceShareCapitalDto dto);

        //Save(c)Terms/rights attached to equity shares
        Task<int> SaveTermsToEquityShareAsync(TermsToEquityShareeDto dto);

        //Save(d)Terms/Rights attached to preference shares
        Task<int> SaveTermsToPreferenceShareAsync(TermsToPreferenceShareDto dto);

        //Save(e)Nameofthesharholder
        Task<int> SaveeNameofthesharholderAsync(NameofthesharholderDto dto);

        //Save(e)PreferenceShares
        Task<int> SaveePreferenceSharesAsync(ePreferenceSharesDto dto);

        //Save(f)SharesAllotted
        Task<int> SavefSharesAllottedAsync(FSahresAllottedDto dto);

        //SaveEquityShares(Promoter name)
        Task<int> SaveEquitySharesPromoterNameAsync(SaveEquitySharesPromoterNameDto dto);

        //SavePreferenceShares(Promoter name)
        Task<int> SavePreferenceSharesPromoterNameAsync(SavePreferenceSharesPromoterNameDto dto);

        //SaveFootNote
        Task<int> SaveFootNoteAsync(FootNoteDto dto);

        //GetFirstNote
        Task<IEnumerable<FirstNoteDto>> GetFirstNoteAsync(int compId, string category, int custId, int YearId);

        //GetSecondNoteById
        Task<IEnumerable<SecondNoteDto>> GetSecondNoteByIdAsync(int compId, string category, int custId, int YearId);

        //GetDescriptionNoteById
        Task<IEnumerable<DescriptionNoteDto>> GetDescriptionNoteAsync(int compId, string category, int custId, int YearId);

        //GetThirdNote
        Task<IEnumerable<ThirdNoteDto>> GetThirdNoteAsync(int compId, string category, int custId, int YearId);

        //GetFourthNote
        Task<IEnumerable<FourthNoteDto>> GetFourthNoteAsync(int compId, string category, int custd, int YearId);

        //DeleteFirstNote
        Task<int> DeleteSchedFirstNoteDetailsAsync(int id, int customerId, int compId, int yearId);

        //DeleteThirdNote
        Task<int> DeleteSchedThirdNoteDetailsAsync(int id, int customerId, int compId, int yearId);

        //DeleteFourthNote
        Task<int> DeleteSchedFourthNoteDetailsAsync(int id, int customerId, int compId, int yearId);

        //DownloadScheduleNoteExcel
        ScheduleNoteFileDownloadResult GetNoteExcelTemplate();

        //DownloadScheduleNotePDF
        ScheduleNotePDFDownloadResult GetNotePDFTemplate();

        //DownloadScheduleNotePDFTemplate
        Task<byte[]> GenerateScheduleNotePdfAsync(int compId, int custId, string financialYear);
    }
}
