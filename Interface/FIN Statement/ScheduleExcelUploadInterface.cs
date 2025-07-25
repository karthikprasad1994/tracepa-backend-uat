using TracePca.Service.FIN_statement;
using static TracePca.Dto.FIN_Statement.ScheduleExcelUploadDto;



namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleExcelUploadInterface
    {
        //DownloadUploadableExcelAndTemplate
        FileDownloadResult GetExcelTemplate();

        //SaveScheduleTemplate(P and L)
        Task<List<int>> SaveSchedulePandLAsync(string DBName, int CompId, List<ScheduleTemplatePandLDto> dtos);

        //SaveScheduleTemplate(Balance Sheet)
        Task<List<int>> SaveScheduleBalanceSheetAsync(string DBName, int CompId, List<ScheduleTemplateBalanceSheetDto> dtos);

        //SaveScheduleTemplate
        Task<List<int>> SaveScheduleTemplateAsync(string DBName, int CompId, List<ScheduleTemplateDto> dtos);

        //SaveOpeningBalance
        Task<List<int>> SaveOpeningBalanceAsync(string DBName, int CompId, List<OpeningBalanceDto> dtos);

        //SaveTrailBalance
        Task<int[]> SaveTrailBalanceDetailsAsync(string DBName, int CompId, List<TrailBalanceDto> dtos);

        //SaveClientTrailBalance
        Task<List<int>> ClientTrailBalanceAsync(string DBName, int CompId, List<ClientTrailBalance> items);

        //SaveJournalEntry
        Task<List<int>> SaveCompleteTrailBalanceAsync(string DBName, int CompId, List<TrailBalanceCompositeModel> models);
    }
}
