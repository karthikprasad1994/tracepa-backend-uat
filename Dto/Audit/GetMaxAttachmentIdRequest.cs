namespace TracePca.Dto.Audit
{
    public class GetMaxAttachmentIdRequest
    {
        public int CustomerId { get; set; }
        public int AuditId { get; set; }
       // public int YearId { get; set; }
        public int ReportTypeId { get; set; }
       // public int ExportType { get; set; }
    }
}

