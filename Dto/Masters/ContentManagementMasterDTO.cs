using TracePca.Dto.Audit;

namespace TracePca.Dto
{
    public class ContentManagementMasterDTO
    {
        public int? CMM_ID { get; set; }
        public string? CMM_Code { get; set; }
        public string? CMM_Desc { get; set; }
        public string? CMM_Category { get; set; }
        public string? CMS_Remarks { get; set; }
        public int? CMS_KeyComponent { get; set; }
        public string? CMS_Module { get; set; }
        public string? CMM_Delflag { get; set; }
        public string? CMM_Status { get; set; }
        public int? CMM_UpdatedBy { get; set; }
        public DateTime? CMM_UpdatedOn { get; set; }
        public int? CMM_ApprovedBy { get; set; }
        public DateTime? CMM_ApprovedOn { get; set; }
        public int? CMM_DeletedBy { get; set; }
        public DateTime? CMM_DeletedOn { get; set; }
        public int? CMM_RecallBy { get; set; }
        public DateTime? CMM_RecallOn { get; set; }
        public string? CMM_IPAddress { get; set; }
        public int? CMM_CompID { get; set; }
        public int? CMM_RiskCategory { get; set; }
        public int? CMM_CrBy { get; set; }
        public DateTime? CMM_CrOn { get; set; }
        public decimal? CMM_Rate { get; set; }
        public string? CMM_Act { get; set; }
        public string? CMM_HSNSAC { get; set; }
        public int? CMM_AudrptType { get; set; }
    }

    public class MasterDropDownListDataDTO
    {
        public List<MasterDropDownListData> MasterList { get; set; }
        public List<DropDownListData> AuditFrameworkList { get; set; }
    }

    public class MasterDropDownListData
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }

    public class UpdateStatusRequest
    {
        public List<int> Ids { get; set; } = new();
        public string Action { get; set; } = string.Empty;
        public int CompId { get; set; }
        public int UserId { get; set; }
        public string IpAddress { get; set; } = string.Empty;
    }

    public class AuditTypeChecklistMasterDTO
    {
        public int? ACM_ID { get; set; }
        public string? ACM_Code { get; set; }
        public int ACM_AuditTypeID { get; set; }
        public string? ACM_Heading { get; set; }
        public string? ACM_Checkpoint { get; set; }
        public string? ACM_DELFLG { get; set; }
        public DateTime? ACM_CRON { get; set; }
        public int? ACM_CRBY { get; set; }
        public int? ACM_APPROVEDBY { get; set; }
        public DateTime? ACM_APPROVEDON { get; set; }
        public string? ACM_STATUS { get; set; }
        public int? ACM_UPDATEDBY { get; set; }
        public DateTime? ACM_UPDATEDON { get; set; }
        public int? ACM_DELETEDBY { get; set; }
        public DateTime? ACM_DELETEDON { get; set; }
        public int? ACM_RECALLBY { get; set; }
        public DateTime? ACM_RECALLON { get; set; }
        public string? ACM_IPAddress { get; set; }
        public int ACM_CompId { get; set; }
        public string? ACM_Assertions { get; set; }
    }

    public class AssignmentTaskChecklistMasterDTO
    {
        public int? ACM_ID { get; set; }
        public string? ACM_Code { get; set; }
        public int ACM_AssignmentTaskID { get; set; }
        public string? ACM_Heading { get; set; }
        public string? ACM_Checkpoint { get; set; }
        public int ACM_BillingType { get; set; }
        public string? ACM_DELFLG { get; set; }
        public DateTime? ACM_CRON { get; set; }
        public int? ACM_CRBY { get; set; }
        public int? ACM_APPROVEDBY { get; set; }
        public DateTime? ACM_APPROVEDON { get; set; }
        public string? ACM_STATUS { get; set; }
        public int? ACM_UPDATEDBY { get; set; }
        public DateTime? ACM_UPDATEDON { get; set; }
        public int? ACM_DELETEDBY { get; set; }
        public DateTime? ACM_DELETEDON { get; set; }
        public int? ACM_RECALLBY { get; set; }
        public DateTime? ACM_RECALLON { get; set; }
        public string? ACM_IPAddress { get; set; }
        public int ACM_CompId { get; set; }
    }

    public class AuditCompletionSubPointMasterDTO
    {
        public int? ASM_ID { get; set; }
        public string? ASM_Code { get; set; }
        public int ASM_CheckpointID { get; set; }
        public string? ASM_SubPoint { get; set; }
        public string? ASM_Remarks { get; set; }
        public string? ASM_DELFLG { get; set; }
        public DateTime? ASM_CRON { get; set; }
        public int? ASM_CRBY { get; set; }
        public int? ASM_APPROVEDBY { get; set; }
        public DateTime? ASM_APPROVEDON { get; set; }
        public string? ASM_STATUS { get; set; }
        public int? ASM_UPDATEDBY { get; set; }
        public DateTime? ASM_UPDATEDON { get; set; }
        public int? ASM_DELETEDBY { get; set; }
        public DateTime? ASM_DELETEDON { get; set; }
        public int? ASM_RECALLBY { get; set; }
        public DateTime? ASM_RECALLON { get; set; }
        public string? ASM_IPAddress { get; set; }
        public int ASM_CompId { get; set; }
    }
}
