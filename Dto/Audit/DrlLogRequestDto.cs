namespace TracePca.Dto.Audit
{
    public class DrlLogRequestDto
    {
        public int YearId { get; set; }
        public int CustomerId { get; set; }
        public int AuditNo { get; set; }
        public int DocumentRequestedListId { get; set; }
        public string RequestedOn { get; set; }
        public string RespondDate { get; set; }
        public string Comment { get; set; }
        public int UserId { get; set; }
        public string IPAddress { get; set; }
        public int CompanyId { get; set; }
        public int? AttachId { get; set; }
        public int? DocId { get; set; }
    }
}
