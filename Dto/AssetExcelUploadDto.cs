namespace TracePca.Dto
{
    public class AssetExcelUploadDto
    {
        public int AmId { get; set; }
        public string Location { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Bay { get; set; }
        public string AssetClassname { get; set; }
        public string Unitsname { get; set; }
        public string AssetCode { get; set; }  // To be auto-generated
        public string AfamDescription { get; set; }  // To be auto-generated
        public string AfamItemDesc { get; set; }  // To be auto-generated

        public int Quantity { get; set; }  // Filled by user in Excel
        public string UnitsOfMeasurement { get; set; }  // Filled by user in Excel
        public int AssetAge { get; set; }  // Filled by user in Excel
        public DateOnly CommissionDate { get; set; }  // Filled by user in Excel

        // These IDs are fetched automatically based on name
        public int LocationId { get; set; }
        public int DivisionId { get; set; }
        public int DepartmentId { get; set; }
        public int BayId { get; set; }
        public int AssetCodeId { get; set; }
        public int CustomerId { get; set; }
        public int FinancialYearId { get; set; }
        public string TransactionNo { get; set; }
        public string AmDescription { get; set; }

        public int UnitsOfMeasurementId { get; set; }

    }
}
