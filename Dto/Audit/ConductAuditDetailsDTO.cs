using System.Text.Json.Serialization;

namespace TracePca.Dto.Audit
{
    public class ConductAuditDetailsDTO
    {
        public int? SAC_ID { get; set; }
        public int? SAC_SA_ID { get; set; }
        public int? SAC_CheckPointID { get; set; }
        public string? SAC_CheckPointName { get; set; }
        public string? SAC_HeadingName { get; set; }
        public string? SAC_AssertionName { get; set; }
        public int? SAC_Mandatory { get; set; }
        public string? SAC_MandatoryName { get; set; }
        public int? SAC_Annexure { get; set; }
        public string? SAC_AnnexureName { get; set; }
        public string? SAC_Remarks { get; set; }
        public string? SAC_ReviewerRemarks { get; set; }
        public int? SAC_Status { get; set; }
        public string? SAC_TestResult { get; set; }
        public string? SAC_TestResultName { get; set; }
        public int? SAC_ConductedBy { get; set; }
        public string? SAC_ConductedByName { get; set; }
        public DateTime? SAC_LastUpdatedOn { get; set; }
        public int? SAC_AttachID { get; set; }
        public string? SAC_IPAddress { get; set; }
        public int? SAC_CompID { get; set; }
        public int? SAC_WorkpaperID { get; set; }
        public string? SAC_WorkpaperNo { get; set; }
        public string? SAC_WorkpaperRef { get; set; }
    }

    public class ConductAuditWorkPaperDetailsDTO
    {
        public int? SSW_ID { get; set; } = 0;
        public int? SSW_SA_ID { get; set; } = 0;
        public string? SSW_WorkpaperNo { get; set; }
        public string? SSW_WorkpaperRef { get; set; }
        public string? SSW_TypeOfTest { get; set; }
        public string? SSW_TypeOfTestName { get; set; }
        public string? SSW_Observation { get; set; }
        public string? SSW_Conclusion { get; set; }
        public int SSW_Status { get; set; }
        public string? SSW_StatusName { get; set; }
        public int SSW_AttachID { get; set; }
        public int SSW_AttachCount { get; set; }
        public int SSW_CrBy { get; set; }
        public string? SSW_CrByName { get; set; }
        public DateTime SSW_CrOn { get; set; } = DateTime.Now;
        public int SSW_UpdatedBy { get; set; }
        public string? SSW_UpdatedByName { get; set; }
        public DateTime? SSW_UpdatedOn { get; set; }
        public string? SSW_IPAddress { get; set; }
        public int? SSW_CompID { get; set; }
        public int? SSW_ExceededMateriality { get; set; }
        public string? SSW_ExceededMaterialityName { get; set; }
        public decimal? SSW_AuditorHoursSpent { get; set; }
        public string? SSW_NotesSteps { get; set; }
        public string? SSW_CriticalAuditMatter { get; set; }
        public string? SSW_ReviewerComments { get; set; }
        public int? SSW_ReviewedBy { get; set; }
        public string? SSW_ReviewedByName { get; set; }
        public DateTime? SSW_ReviewedOn { get; set; }
        public int? SSW_WPCheckListID { get; set; }
        public int? SSW_DRLID { get; set; }
        public int? SSW_DRLAttachmentID { get; set; }
        public int? SSW_DRLAttachmentCount { get; set; }
    }
}
