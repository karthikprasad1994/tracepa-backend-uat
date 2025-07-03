namespace TracePca.Dto.Audit
{
    public class ReportHistoryCommentsDto
    {
        public int CustId { get; set; }
        public int AuditId { get; set; }
        public int RequestId { get; set; }
        public int ReportType { get; set; }
    }
}
