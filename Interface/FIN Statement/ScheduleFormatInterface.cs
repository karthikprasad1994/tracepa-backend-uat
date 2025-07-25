using static TracePca.Dto.FIN_Statement.ScheduleExcelUploadDto;
using static TracePca.Dto.FIN_Statement.ScheduleFormatDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleFormatInterface
    {

        //GetScheduleHeading
        Task<IEnumerable<ScheduleHeadingDto>> GetScheduleFormatHeadingAsync(
           string DBName, int CompId, int ScheduleId, int CustId, int AccHead);

        //GetScheduleSubHeading
        Task<IEnumerable<ScheduleSubHeadingDto>> GetScheduleFormatSubHeadingAsync(
          string DBName, int CompId, int ScheduleId, int CustId, int HeadingId);

        //GetScheduleItem
        Task<IEnumerable<ScheduleItemDto>> GetScheduleFormatItemsAsync(
           string DBName, int CompId, int ScheduleId, int CustId, int HeadingId, int SubHeadId);

        //GetScheduleSubitems
        Task<IEnumerable<ScheduleSubItemDto>> GetScheduleFormatSubItemsAsync(
           string DBName, int CompId, int ScheduleId, int CustId, int HeadingId, int SubHeadId, int ItemId);

        //GetScheduleTemplate
        Task<IEnumerable<ScheduleFormatTemplateDto>> GetScheduleTemplateAsync(
        string DBName, int CompId, int ScheduleId, int CustId, int AccHead);

        //DeleteScheduleTemplate(Grid)
        Task<bool> DeleteScheduleTemplateAsync(
           string DBName, int CompId, int ScheduleType, int CustId, int SelectedValue, int MainId);

        //SaveScheduleHeading
        Task<int[]> SaveScheduleHeadingAndTemplateAsync(string DBName, int CompId, SaveScheduleHeadingDto dto);

        //SaveScheduleSubHeading
        Task<int[]> SaveScheduleSubHeadingAndTemplateAsync(string DBName, int CompId, SaveScheduleSubHeadingDto dto);

        //SaveScheduleItem
        Task<int[]> SaveScheduleItemAndTemplateAsync(string DBName, int CompId, SaveScheduleItemDto dto);

        //SaveScheduleSubItem
        Task<int[]> SaveScheduleSubItemAndTemplateAsync(string DBName, int CompId, SaveScheduleSubItemDto dto);

        //SaveOrUpdateScheduleHeadingAlias
        Task<int[]> SaveScheduleHeadingAliasAsync(string DBName, ScheduleHeadingAliasDto dto);

        //GetScheduleTemplateCount
        Task<IEnumerable<ScheduleTemplateCountDto>> GetScheduleFormatItemsAsync(string DBName, int CustId, int CompId);

        //SaveScheduleTemplate(P and L)
        Task<List<int>> SaveScheduleTemplateAsync(string DBName, int CompId, List<ScheduleTemplate> dtos);
    }
}
