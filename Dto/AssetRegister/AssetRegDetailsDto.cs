namespace TracePca.Dto.AssetRegister
{
    public class AssetRegDetailsDto
    {
        public int CustomerId { get; set; }
        public int Id { get; set;}
        public int LocationId { get; set; } 
        public string CustomerName { get; set; }
        public int YearId { get; set; }
        public string FinancialYear { get; set; }
        public string AssetClassName { get; set; }
        public int AssetId { get; set; }
        public string AssetCode { get; set; }
        public string AssetNo { get; set; }
        public string AssetDescription { get; set; }
        public decimal Quantity { get; set; }
        public string UnitOfMeasurement { get; set; }
        public int UnitOfMeasurementId { get; set; }
        public int UsefulLife { get; set; }
        public DateTime PutToUseDate { get; set; }


    }
}
