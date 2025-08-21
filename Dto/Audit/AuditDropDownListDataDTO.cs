namespace TracePca.Dto.Audit
{
    public class AuditDropDownListDataDTO
    {
        public DropDownListData CurrentYear { get; set; }
        public List<DropDownListData> CustomerList { get; set; }
        public List<AuditDropDownListData> ExistingAuditNoList { get; set; }
        public List<DropDownListData> ReportTypeList { get; set; }
        public List<DropDownListData> AuditTypeList { get; set; }
        public List<DropDownListData> FeeTypeList { get; set; }
        public List<LOEDropDownListData> ExistingEngagementPlanNames { get; set; }
        public List<DropDownListData> AuditWorkpaperList { get; set; }
        public List<DropDownListData> AuditCompletionCheckPointList { get; set; }
        public List<DropDownListData> AuditClosureCheckPointList { get; set; }
        public List<DropDownListData> SignedByList { get; set; }
        public List<DropDownListData> CustomerUserList { get; set; }
        public List<DropDownListData> WorkpaperCheckList { get; set; }
        public List<DropDownListData> TypeofTestList { get; set; }
        public List<DropDownListData> ExceededMaterialityList { get; set; }
        public List<DropDownListData> WorkPaperStatusList { get; set; }
        public List<DropDownListData> TestResultList { get; set; }
        public List<DropDownListData> AuditCheckPointHeadingList { get; set; }
        public List<DropDownListData> DRLWithAttachmentsList { get; set; }        
    }
    public class DropDownListData
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class AuditDropDownListData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int isPartner { get; set; }
        public int isReviewer { get; set; }
        public int isAuditor { get; set; }
        public int Status { get; set; }
        public int AuditFrameworkId { get; set; }
        public int IsArchived { get; set; }
    }

    public class LOEDropDownListData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int AuditTypeID { get; set; }
    }
}
