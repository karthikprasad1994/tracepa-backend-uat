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
        public string SA_UDIN { get; set; }
        public DateTime SA_UDINdate { get; set; }
        public int SA_CompID { get; set; }
    }
}
