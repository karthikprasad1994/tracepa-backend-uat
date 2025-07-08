using static TracePca.Dto.FIN_Statement.ScheduleNoteDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleNoteInterface
    {
        //GetCustomersName
        Task<IEnumerable<CustDto>> GetCustomerNameAsync(int CompId);

        //GetFinancialYear
        Task<IEnumerable<FinancialYearDto>> GetFinancialYearAsync(int CompId);

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
    }
}
