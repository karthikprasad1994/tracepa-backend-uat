namespace TracePca.Dto.Audit
{
    public class CustomerDataDto
    {
        public CustomerInvoiceDto Customer { get; set; }
        public List<DRLTemplateItem> TemplateSections { get; set; }
    }
}
