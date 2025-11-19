using TracePca.Dto.Audit;
using TracePca.Dto.TaskManagement;

namespace TracePca.Interface.TaskManagement
{
    public interface TaskInvoiceAndReportInterface
    {
        Task<TaskDropDownListDataDTO> LoadAllDDLDataAsync(int compId);
        Task<CompanyInvoiceDetailsDto> GetCompanyInvoiceDetailsAsync(int compId, int companyId);
    }
}