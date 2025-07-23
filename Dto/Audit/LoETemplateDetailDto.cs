namespace TracePca.Dto.Audit
{
    public class LoETemplateDetailDto
    {
        public int AuditNo { get; set; }
        public int CabType { get; set; }
        public int HeadingId { get; set; }
        public string Heading { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public string IpAddress { get; set; }
    }
}
