namespace TracePca.Dto.Audit
{
    public class DropDownDataDto
    {
        public List<ReportTypeDto> ReportTypes { get; set; }
        public List<AuditTypeDto> AuditTypes { get; set; }
        public List<AuditSummaryDto> CustomerDetails { get; set; }
        public List<AuditSummaryDto> AuditNoDetails { get; set; }
    }
}
