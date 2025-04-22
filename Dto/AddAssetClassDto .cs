namespace TracePca.Dto
{
    public class AddAssetClassDto
    {
        public int? Id { get; set; }  // Null for new entry, ID for update
        public string Name { get; set; }
        public decimal WDVITAct { get; set; } 
        public decimal ITRate { get; set; } 
        public decimal ResidualValue { get; set; } 
        public decimal OriginalCost { get; set; }
        public int CreatedBy { get; set; }
        public int YearID { get; set; }
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public string IPAddress { get; set; }

        public int ParentId { get; set; }
    }
}
