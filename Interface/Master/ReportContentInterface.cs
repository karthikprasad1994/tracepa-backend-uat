using TracePca.Dto.Master;
namespace TracePca.Interface.Master
{
    public interface ReportTemplateInterface
    {
        Task<(bool Success, string Message, List<ReportTypeDTO> Data)> GetReportTypesByFunctionAsync(int functionId, int compId);
        Task<(bool Success, string Message, List<ReportContentDTO> Data)> GetReportContentByReportTypeAsync(int reportTypeId, int compId);
        Task<(bool Success, string Message)> SaveOrUpdateReportContentAsync(ReportContentSaveDTO dto);
        Task<(bool Success, string Message)> SaveOrUpdateReportTemplateSortOrderAsync(ReportTemplateSortOrderSaveDTO dto);
    }
}
