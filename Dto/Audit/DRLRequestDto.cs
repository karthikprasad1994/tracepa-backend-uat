namespace TracePca.Dto.Audit
{
    public class DRLRequestDto
    {
        public string ReferenceNumber { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }

        public int ExportType { get; set; } // 1 = Beginning, 2 = Nearing completion
        public int FinancialYearId { get; set; }
        public int CustomerId { get; set; }
        public int AuditNo { get; set; }
        public string RequestedOn { get; set; }
        public string TimelineToRespond { get; set; }
        public string EmailIds { get; set; }
        public string Comments { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public string IPAddress { get; set; }
        public int CompanyId { get; set; }
        public int ReportType { get; set; }

        public List<DRLTemplateItem> TemplateItems { get; set; }
    }

    public class DRLTemplateItem
    {
        public string Heading { get; set; }
        public string Description { get; set; }
    }
}

