namespace TracePca.Dto.Audit
{
    public class CustomerAuditDropdownDto
    {
        public IEnumerable<CustomerDto> CustomerLoes { get; set; }
        public IEnumerable<AuditTypeDto> AuditTypes { get; set; }
    }
}
