namespace TracePca.Dto.Audit
{
    public class CustomerInvoiceDto
    {
        public string CustName { get; set; }
        public string CustAddress { get; set; }
        public string CustCityPin { get; set; }
        public string CustState { get; set; }
        public string CustEmail { get; set; }
        public string CustTelephone { get; set; }
        public string CustPAN { get; set; } = "";  // Always empty in original code
        public string CustGSTIN { get; set; }
    }
}
