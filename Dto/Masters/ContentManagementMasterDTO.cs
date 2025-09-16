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

    public class UpdateStatusRequest
    {
        public List<int> Ids { get; set; } = new();
        public string Action { get; set; } = string.Empty; // approve, revert, delete
        public int CompId { get; set; }
        public int UpdatedBy { get; set; }
        public string IpAddress { get; set; } = string.Empty;
    }
}
