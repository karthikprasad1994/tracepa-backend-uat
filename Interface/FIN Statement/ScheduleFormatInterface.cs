using static TracePca.Dto.FIN_Statement.ScheduleFormatDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface ScheduleFormatInterface
    {
        //GetSecheduleFormat-ClientName
        Task<IEnumerable<ScheduleFormatClientDto>> GetScheduleFormatClientAsync(int iCompId);

        //GetScheduleFormat-Heading
        Task<IEnumerable<ScheduleFormatHeadingDto>> GetScheduleFormatHeadingAsync(
            int iCompId, int iScheduleId, int iCustId, int iAccHead);

        //GetScheduleFormat-SubHeading
        Task<IEnumerable<ScheduleFormatSubHeadingDto>> GetScheduleFormatSubHeadingAsync(
            int iCompId, int iScheduleId, int iCustId, int iHeadingId);

        //GetScheduleFormat-ItemUnderSubHeading
        Task<IEnumerable<SFItemUnderSubHeadingDto>> GetScheduleFormatItemsAsync(
            int iCompId, int iScheduleId, int iCustId, int iHeadingId, int iSubHeadId);

        //GetScheduleFormat-SubitemsUnderItems
        Task<IEnumerable<SFSubItemsUnderItemsDto>> GetScheduleFormatSubItemsAsync(
            int iCompId, int iScheduleId, int iCustId, int iHeadingId, int iSubHeadId, int iItemId);

        //GetScheduleTemplate
        Task<IEnumerable<ScheduleFormatTemplateDto>> GetScheduleTemplateAsync(
        int iCompId, int iScheduleId, int iCustId, int iAccHead);

        //ScheduleFormatDeleteScheduleTemplate(Grid)
        Task<bool> DeleteScheduleTemplateAsync(
            int iCompId, int iScheduleType, int iCustId, int iSelectedValue, int iMainId);

        //SaveScheduleFormatHeading
        Task<int[]> SaveScheduleHeadingAndTemplateAsync(int iCompId, SaveScheduleFormatHeadingDto dto);

        //SaveScheduleFormatSub-HeadingAndTemplate
        Task<int[]> SaveScheduleSubHeadingAndTemplateAsync(int iCompId, SaveScheduleFormatSub_HeaddingDto dto);

        //SaveScheduleFormatItemsAndTemplate
        Task<int[]> SaveScheduleItemAndTemplateAsync(int iCompId, SaveScheduleFormatItemDto dto);

        //SaveScheduleFormatSub-ItemAndHeading
        Task<int[]> SaveScheduleSubItemAndTemplateAsync(int iCompId, SaveScheduleFormatSub_ItemDto dto);

        //DeleteScheduleTemplate
        Task<bool> DeleteInformationAsync(int iCompId, int iScheduleType, int iCustId, int iSelectedValue, int iMainId);

        //SaveOrUpdateScheduleHeadingAlias
        Task<int[]> SaveScheduleHeadingAliasAsync(ScheduleHeadingAliasDto dto);
    }
}
