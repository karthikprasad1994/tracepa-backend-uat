namespace TracePca.Dto
{
    public class AddSubHeadingDto
    {
        public int? Id { get; set; }  // Null for new entry, ID for update
        public string Name { get; set; }
        public decimal WDVITAct { get; set; } = 0;
        public decimal ITRate { get; set; } = 0;
        public decimal ResidualValue { get; set; } = 0;
        public int CreatedBy { get; set; }
        public int YearID { get; set; }
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public string IPAddress { get; set; }

        public int ParentId { get; set; }
    }
}
