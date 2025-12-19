using System.Data;
using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.ScheduleExcelUploadDto;



namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleExcelUploadInterface
    {
        //DownloadUploadableExcelAndTemplate
        FileDownloadResult GetExcelTemplate();

        //SaveScheduleTemplate(P and L)
        Task<List<int>> SaveSchedulePandLAsync(int CompId, List<ScheduleTemplatePandLDto> dtos);

        //SaveScheduleTemplate(Balance Sheet)
        Task<List<int>> SaveScheduleBalanceSheetAsync(int CompId, List<ScheduleTemplateBalanceSheetDto> dtos);

        //SaveScheduleTemplate
        Task<List<int>> SaveScheduleTemplateAsync(int CompId, List<ScheduleTemplateDto> dtos);

        //SaveOpeningBalance
        Task<List<int>> SaveOpeningBalanceAsync(int CompId, List<OpeningBalanceDto> dtos);

        //SaveTrailBalance
        Task<int[]> SaveTrailBalanceDetailsAsync(int CompId, List<TrailBalanceDto> dtos);

        //SaveClientTrailBalance
        Task<List<int>> ClientTrailBalanceAsync(int CompId, List<ClientTrailBalance> items);

        //SaveJournalEntry
        Task<List<int>> SaveCompleteTrailBalanceAsync(
            int CompId,
            List<TrailBalanceCompositeModel> models,
            IFormFile excelFile,
            string sheetName);

        //UploadCustomerTrialBalance
        Task<string> UploadCustomerTrialBalanceExcelAsync(
            IFormFile excelFile,
            int customerId,
            int yearId,
            int branchId,
            int quarterId,
            int companyId,
            int userId);
    }
}
