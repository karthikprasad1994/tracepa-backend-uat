using static TracePca.Dto.FIN_Statement.ScheduleNoteDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleNoteInterface
    {
        //GetCustomersName
        Task<IEnumerable<CustDto>> GetCustomerNameAsync(int icompId);

        //GetFinancialYear
        Task<IEnumerable<FinancialYearDto>> GetFinancialYearAsync(int icompId);

        //GetSubHeadingname(Notes For SubHeading)
        Task<IEnumerable<SubHeadingNoteDto>> GetSubHeadingDetailsAsync(int customerId, int subHeadingId);

        //SaveOrUpdateSubHeadingNotes(Notes For SubHeading)
        Task<int[]> SaveSubHeadindNotesAsync(SubHeadingNotesDto dto);

        //GetBranch(Notes For Ledger)
        Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(int compId, int custId);

        //GetLedger(Notes For Ledger)
        Task<IEnumerable<LedgerIndividualDto>> GetLedgerIndividualDetailsAsync(int customerId, int subHeadingId);

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
