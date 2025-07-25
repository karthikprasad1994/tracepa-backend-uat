using static TracePca.Dto.FIN_Statement.ScheduleNoteDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleNoteInterface
    {

        //GetSubHeadingname(Notes For SubHeading)
        Task<IEnumerable<SubHeadingNoteDto>> GetSubHeadingDetailsAsync(string DBName, int CustomerId, int SubHeadingId);

        //SaveOrUpdateSubHeadingNotes(Notes For SubHeading)
        Task<int[]> SaveSubHeadindNotesAsync(string DBName, SubHeadingNotesDto dto);

        //GetBranch(Notes For Ledger)
        Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(string DBName, int CompId, int CustId);

        //GetLedger(Notes For Ledger)
        Task<IEnumerable<LedgerIndividualDto>> GetLedgerIndividualDetailsAsync(string DBName, int CustomerId, int SubHeadingId);

        //SaveOrUpdateLedger(Notes For Ledger)
        Task<int[]> SaveLedgerDetailsAsync(string DBName, SubHeadingLedgerNoteDto dto);

        //DownloadNotesExcel
        ExcelFileDownloadResult GetExcelTemplate();

        //DownloadNotesPdf
        PdfFileDownloadResult GetPdfTemplate();

        //SaveOrUpdate
        Task<int> SaveFirstScheduleNoteDetailsAsync(string DBName, FirstScheduleNoteDto dto);
    }
}
