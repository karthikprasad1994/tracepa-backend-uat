namespace TracePca.Dto.Audit
{
    public class LoETemplateDetailInputDto
    {
        public int LoeTemplateId { get; set; }
        public int ReportTypeId { get; set; }
        public int HeadingId { get; set; }
        public string Heading { get; set; }
        public string Description { get; set; }
        public string FormName { get; set; }
        public int UserId { get; set; }
        public string IpAddress { get; set; }
        public int CompanyId { get; set; }
    }
}
