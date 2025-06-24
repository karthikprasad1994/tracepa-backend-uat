namespace TracePca.Dto.Audit
{
    public class DRLLogDto
    {
        public int Id { get; set; }
        public int AttachId { get; set; }
        public string? Status { get; set; }
        public int YearId { get; set; }
        public int AuditNo { get; set; }
        public int CustomerId { get; set; }
        public int RequestedListId { get; set; }
        public int RequestedTypeId { get; set; }
        public DateTime RequestedOn { get; set; }
        public DateTime TimelineToRespond { get; set; }
        public string EmailIds { get; set; }
       // public List<string> EmailIds { get; set; }
        public string Comments { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public string IPAddress { get; set; }
        public int CompanyId { get; set; }
        public int ReportType { get; set; }
    }
}
