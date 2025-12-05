namespace TracePca.Dto.FixedAssets
{
    public class DepreciationComputationDto
    {
        //MethodofDepreciation
        public class DepreciationDto
        {
            public int AssetClassID { get; set; }
            public int AssetID { get; set; }
            public string AssetType { get; set; }
            public string AssetCode { get; set; }
            public string Location { get; set; }
            public string Division { get; set; }
            public string Department { get; set; }
            public string Bay { get; set; }
            public int LocationID { get; set; }
            public int DivisionID { get; set; }
            public int DepartmentID { get; set; }
            public int BayID { get; set; }
            public DateTime? PurchaseDate { get; set; }
            public int TrType { get; set; }
            public int NoOfDays { get; set; }
            public string Item { get; set; }
            public decimal OrignalCost { get; set; }
            public decimal Rsdulvalue { get; set; }
            public decimal SalvageValue { get; set; }
            public int AssetAge { get; set; }
            public string DepreciationRate { get; set; }
            public decimal AddtnAmt { get; set; }
            public decimal OPBForYR { get; set; }
            public decimal DepreciationforFY { get; set; }
            public decimal wrtnvalue { get; set; }
        }

    }
}
