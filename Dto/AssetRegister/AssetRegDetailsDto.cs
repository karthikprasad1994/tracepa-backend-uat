namespace TracePca.Dto.AssetRegister
{
    public class AssetRegDetailsDto
    {
        public string CustomerName { get; set; }
        public int FinancialYear { get; set; }
        public string AssetClassName { get; set; }
        public string AssetCode { get; set; }
        public string AssetNo { get; set; }
        public string AssetDescription { get; set; }
        public int Quantity { get; set; }
        public string UnitOfMeasurement { get; set; }
        public int UsefulLife { get; set; }
        public DateTime PutToUseDate { get; set; }

      
    }
}
