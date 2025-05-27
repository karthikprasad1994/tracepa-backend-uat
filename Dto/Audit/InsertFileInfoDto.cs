namespace TracePca.Dto.Audit
{
    public class InsertFileInfoDto
    {
        public int YearId { get; set; }
        public int AuditId { get; set; }
        public int CustomerId { get; set; }
        public string EmailId { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
        public string TimlineToRespondOn { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public int CompId { get; set; }
    }
}

