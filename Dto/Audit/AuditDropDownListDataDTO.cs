namespace TracePca.Dto.Audit
{
    public class AuditDropDownListDataDTO
    {
        public DropDownListData CurrentYear { get; set; }
        public List<DropDownListData> CustomerList { get; set; }
        public List<DropDownListData> ExistingAuditNoList { get; set; }
        public List<DropDownListData> ReportTypeList { get; set; }
        public List<DropDownListData> AuditTypeList { get; set; }
        public List<DropDownListData> FeeTypeList { get; set; }
        public List<LOEDropDownListData> ExistingEngagementPlanNames { get; set; }
        public List<DropDownListData> AuditWorkpaperList { get; set; }
        public List<DropDownListData> AuditCompletionCheckPointList { get; set; }
        public List<DropDownListData> SignedByList { get; set; }
    }
    public class DropDownListData
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class LOEDropDownListData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int AuditTypeID { get; set; }

    }
}
