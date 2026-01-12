using DocumentFormat.OpenXml.Bibliography;
using System.Text.Json.Serialization;

namespace TracePca.Dto.Audit
{
    public class AuditCompletionDTO
    {
        public int SAC_CustID { get; set; }
        public int SAC_YearID { get; set; }
        public int SAC_AuditID { get; set; }
        public int SAC_CreatedBy { get; set; }
        public DateTime? SAC_CreatedOn { get; set; } = DateTime.Now;
        public int? SAC_UpdatedBy { get; set; }
        public DateTime? SAC_UpdatedOn { get; set; }
        public int SAC_CompID { get; set; }
        public string? SAC_IPAddress { get; set; }
        public List<AuditCompletionSubPointDetailsDTO> AuditCompletionSubPointDetails { get; set; } = new List<AuditCompletionSubPointDetailsDTO>();
        public List<AuditCompletionTemplateDetailsDTO> AuditCompletionTemplateDetails { get; set; } = new List<AuditCompletionTemplateDetailsDTO>();
    }

    public class AuditCompletionSingleDTO
    {
        public int SAC_CustID { get; set; }
        public int SAC_YearID { get; set; }
        public int SAC_AuditID { get; set; }
        public int SAC_CreatedBy { get; set; }
        public DateTime? SAC_CreatedOn { get; set; } = DateTime.Now;
        public int? SAC_UpdatedBy { get; set; }
        public DateTime? SAC_UpdatedOn { get; set; }
        public int SAC_CompID { get; set; }
        public string? SAC_IPAddress { get; set; }
        public AuditCompletionSubPointDetailsDTO AuditCompletionSubPointDetails { get; set; } = new AuditCompletionSubPointDetailsDTO();
    }

    public class AuditCompletionSubPointDetailsDTO
    {
        public int SAC_ID { get; set; }
        public string? SAC_CheckPointName { get; set; }
        public int SAC_CheckPointId { get; set; }
        public int SAC_SubPointId { get; set; }
        public string? SAC_SubPointName { get; set; }
        public string? SAC_Remarks { get; set; }
        public int SAC_WorkPaperId { get; set; }
        public string? SAC_WorkPaperName { get; set; }
        public int SAC_AttachmentId { get; set; }
    }

    public class AuditCompletionTemplateDetailsDTO
    {
        public int? LTD_ID { get; set; } = 0;
        public int? LTD_LOE_ID { get; set; } = 0;
        public int LTD_ReportTypeID { get; set; }
        public int LTD_HeadingID { get; set; }
        public string? LTD_Heading { get; set; }
        public string? LTD_Decription { get; set; }
        public string? LTD_FormName { get; set; } = "AC";
        public int LTD_CrBy { get; set; }
        public DateTime? LTD_CrOn { get; set; } = DateTime.Now;
        public string LTD_IPAddress { get; set; }
        public int LTD_CompID { get; set; }
        public int? LTD_UpdatedBy { get; set; }
        public DateTime? LTD_UpdatedOn { get; set; }
    }

    public class AuditSignedByUDINRequestDTO
    {
        public int SA_ID { get; set; }
        public int SA_SignedBy { get; set; }
        public string? SA_UDIN { get; set; }
        public DateTime SA_UDINdate { get; set; }
        public int SA_CompID { get; set; }
    }

    public class AuditArchiveDTO
    {
        public int SA_ID { get; set; }
        public int? SA_RetentionPeriod { get; set; }
        public int? SA_ForCompleteAudit { get; set; }
        public DateTime? SA_ExpiryDate { get; set; }
        public int SA_CompID { get; set; }
        public int? SA_UserID { get; set; } = 0;
    }

    public class AuditReportCustInfoAuditeeDetailDTO
    {
        public int SlNo { get; set; }
        public string Particulars { get; set; }
        public string Details { get; set; }
    }

    public class ConductAuditWorkPaperDTO
    {
        public string WorkpaperNo { get; set; }
        public string WorkpaperRef { get; set; }
        public string Notes { get; set; }
        public string Deviations { get; set; }
        public string CriticalAuditMatter { get; set; }
        public string Conclusion { get; set; }        
        public string TypeOfTest { get; set; }
        public string ExceededMateriality { get; set; }
        public string AuditorHoursSpent { get; set; }
        public string Status { get; set; }
        public string AttachmentCount { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ReviewedBy { get; set; }
        public string ReviewedOn { get; set; }
    }

    public class ConductAuditReportDetailDTO
    {
        public int SlNo { get; set; }
        public string Heading { get; set; }
        public string CheckPoints { get; set; }
        public string Comments { get; set; }
        public string Mandatory { get; set; }
        public string TestResult { get; set; }
        public string Annexure { get; set; }
        public string WorkpaperRef { get; set; }        
        public string ConductedBy { get; set; }
        public string ConductedOn { get; set; }
    }

    public class ConductAuditObservationDTO
    {
        public string SrNo { get; set; }
        public string CheckPoint { get; set; }
        public string Observations { get; set; }
    }

    public class ConductAuditRemarksReportDTO
    {
        public string SrNo { get; set; }
        public string CheckPoint { get; set; }
        public string Observations { get; set; }
        public DateTime? Date { get; set; }
        public string RemarksBy { get; set; }
        public string RemarksByRole { get; set; }
        public string ClientRemarks { get; set; }
    }

    public class CommunicationWithClientTemplateReportDetailsDTO
    {
        public string? LTD_Heading { get; set; }
        public string? LTD_Decription { get; set; }
    }

    public class StandardAuditAllAttachmentsDTO
    {
        public List<AttachmentGroupDTO> AuditPlanAttachments { get; set; } = new();
        public List<AttachmentGroupDTO> AuditScheduleAttachments { get; set; } = new();
        public List<AttachmentGroupDTO> BeginningAuditAttachments { get; set; } = new();
        public List<AttachmentGroupDTO> NearEndAuditAttachments { get; set; } = new();
        public List<AttachmentGroupDTO> DuringAuditAttachments { get; set; } = new();
        public List<AttachmentGroupDTO> WorkpaperAttachments { get; set; } = new();
        public List<AttachmentGroupDTO> ConductAuditAttachments { get; set; } = new();
        public List<AttachmentGroupDTO> AuditCompletionSubCheckpointAttachments { get; set; } = new();
        public List<AttachmentGroupDTO> AccountFinalisationAttachments { get; set; } = new();        
    }

    public class AttachmentGroupDTO
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public List<AttachmentDetailsDTO> Attachments { get; set; }
    }
}
