using TracePca.Dto.Audit;

namespace TracePca.Dto.Master
{
    public class ReportTypeDTO
    {
        public int RTM_Id { get; set; }
        public string RTM_ReportTypeName { get; set; }
    }
    public class ReportContentDTO
    {
        public int RCM_Id { get; set; }
        public int RCM_ReportId { get; set; }
        public string RCM_Heading { get; set; }
        public string RCM_Description { get; set; }
    }
    public class ReportContentSaveDTO
    {
        public int RCM_Id { get; set; }
        public int RCM_ReportId { get; set; }
        public string RCM_ReportName { get; set; }        
        public string RCM_Heading { get; set; }
        public string RCM_Description { get; set; }
        public int UserId { get; set; }
        public int CompId { get; set; }
        public string IpAddress { get; set; }
        public int YearId { get; set; }
    }

    public class ReportTemplateSortOrderSaveDTO
    {
        public int TEM_Yearid { get; set; }
        public int TEM_FunctionId { get; set; }
        public string TEM_ContentId { get; set; }
        public string TEM_SortOrder { get; set; }
        public int UserId { get; set; }
        public int CompId { get; set; }
        public string IpAddress { get; set; }
    }
}
