using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using TracePca.Dto.Audit;

namespace TracePca.Interface.Audit
{
    public interface AuditCompletionInterface
    {
        Task<AuditDropDownListDataDTO> LoadAllAuditDDLDataAsync(int compId);
        Task<IEnumerable<ReportTypeDetailsDTO>> GetReportTypeDetails(int compId);
        Task<AuditDropDownListDataDTO> LoadAuditNoDDLAsync(int compId, int yearId, int custId, int userId);
        Task<AuditDropDownListDataDTO> LoadAuditWorkPaperDDLAsync(int compId, int auditId);
        Task<AuditCompletionDTO> GetAuditCompletionDetailsByIdAsync(int compId, int auditId);
        Task<List<AuditCompletionSubPointDetailsDTO>> GetAuditCompletionSubPointDetailsAsync(int compId, int auditId, int checkPointId);
        Task<List<AuditCompletionSubPointDetailsDTO>> GetAuditClosureSubPointDetailsAsync(int compId, int auditId, int checkPointId);
        Task<int> SaveOrUpdateAuditCompletionSubPointDataAsync(AuditCompletionSingleDTO dto);
        Task<int> SaveOrUpdateAuditCompletionDataAsync(AuditCompletionDTO dto);        
        Task<int> UpdateSignedByUDINInAuditAsync(AuditSignedByUDINRequestDTO dto);
        Task<int> CheckCAEIndependentAuditorsReportSavedAsync(int compId, int auditId);
        Task<AuditSignedByUDINRequestDTO> GetSignedByUDINInAuditAsync(int compId, int auditId);
        Task<(byte[] fileBytes, string contentType, string fileName)> GenerateAndDownloadReportAsync(int compId, int auditId, string format);
        Task<string> GenerateReportAndGetURLPathAsync(int compId, int auditId, string format);
        Task<string> GenerateACSubPointsReportAndGetURLPathAsync(int compId, int auditId, int userId, string format);
        Task<List<ConductAuditReportDetailDTO>> GetConductAuditReportAsync(int compId, int auditId);
        Task<List<ConductAuditRemarksReportDTO>> GetConductAuditRemarksReportAsync(int compId, int auditId);        
        Task<List<ConductAuditObservationDTO>> GetConductAuditObservationsAsync(int compId, int auditId);
        Task<List<ConductAuditWorkPaperDTO>> LoadConductAuditWorkPapersAsync(int compId, int auditId);
        Task<StandardAuditAllAttachmentsDTO> LoadAllAuditAttachmentsByAuditIdAsync(int compId, int auditId);
        Task<(bool, string)> DownloadAllAuditAttachmentsByAuditIdAsync(int compId, int auditId, int userId, string ipAddress);
    }
}
