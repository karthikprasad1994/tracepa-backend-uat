using static TracePca.Dto.FIN_Statement.ScheduleExcelUploadDto;
using static TracePca.Dto.FIN_Statement.ScheduleFormatDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleFormatInterface
    {

        //GetScheduleHeading
        Task<IEnumerable<ScheduleHeadingDto>> GetScheduleFormatHeadingAsync(int CompId, int ScheduleId, int CustId, int AccHead);

        //GetScheduleSubHeading
        Task<IEnumerable<ScheduleSubHeadingDto>> GetScheduleFormatSubHeadingAsync(int CompId, int ScheduleId, int CustId, int HeadingId);

        //GetScheduleItem
        Task<IEnumerable<ScheduleItemDto>> GetScheduleFormatItemsAsync(int CompId, int ScheduleId, int CustId, int HeadingId, int SubHeadId);

        //GetScheduleSubitems
        Task<IEnumerable<ScheduleSubItemDto>> GetScheduleFormatSubItemsAsync(int CompId, int ScheduleId, int CustId, int HeadingId, int SubHeadId, int ItemId);

        //GetScheduleTemplate
        Task<IEnumerable<ScheduleFormatTemplateDto>> GetScheduleTemplateAsync(int CompId, int ScheduleId, int CustId, int AccHead);

        //DeleteScheduleTemplate(Grid)
        Task<bool> DeleteScheduleTemplateAsync(int CompId, int ScheduleType, int CustId, int SelectedValue, int MainId);

        //SaveScheduleHeading
        Task<int[]> SaveScheduleHeadingAndTemplateAsync(int CompId, SaveScheduleHeadingDto dto);

        //SaveScheduleSubHeading
        Task<int[]> SaveScheduleSubHeadingAndTemplateAsync(int CompId, SaveScheduleSubHeadingDto dto);

        //SaveScheduleItem
        Task<int[]> SaveScheduleItemAndTemplateAsync(int CompId, SaveScheduleItemDto dto);

        //SaveScheduleSubItem
        Task<int[]> SaveScheduleSubItemAndTemplateAsync(int CompId, SaveScheduleSubItemDto dto);

        //SaveOrUpdateScheduleHeadingAlias
        Task<int[]> SaveScheduleHeadingAliasAsync(int CompId, ScheduleHeadingAliasDto dto);

        //GetScheduleTemplateCount
        Task<IEnumerable<ScheduleTemplateCountDto>> GetScheduleFormatItemsAsync(int CustId, int CompId);

        //SaveScheduleTemplate(P and L)
        Task<List<int>> SaveScheduleTemplateAsync(int CompId, List<ScheduleTemplate> dtos);

        //GetGridViewAlias
        Task<IEnumerable<AliasDto>> LoadGridView1gridAsync(int CompId, int CustId, string lblText, int SelectedVal);
        Task<CustomerDetailsDto> GetCustomerDetailsAsync(int custId, int compId);

    }
}
