namespace TracePca.Dto.Audit
{
    public class GenerateDRLReportRequestDto
    {
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public int ReportTypeId { get; set; }
        public int ExportType { get; set; } // 1 or 2
        public int YearId { get; set; }
        public int AuditNo { get; set; }
        public int AttachId { get; set; }
        public string Status { get; set; }
        public int RequestedTypeId { get; set; }
        public DateTime TimelineToRespond { get; set; }
        public string EmailIds { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string IPAddress { get; set; }
        public string Format { get; set; } // "pdf" or "word"
        public int ReportType { get; set; }
    }
}
