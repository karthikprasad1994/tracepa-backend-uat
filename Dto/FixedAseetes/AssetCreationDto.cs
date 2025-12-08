namespace TracePca.Dto.FixedAssets
{
    public class AssetCreationDto
    {
        //LoadAssetClass
        public class AssetTypeDto
        {
            public int AssetTypeId { get; set; }
            public string AssetTypeName { get; set; }
        }

        //New
        public class NewDto
        {
            public string AssetName { get; set; }
            public string AssetCode { get; set; }
            public int CreatedBy { get; set; }
            public int CompId { get; set; }
            public int CustId { get; set; }
        }

        //Search
        public class AssetRegisterRaw
        {
            public int Id { get; set; }
            public int AssetId { get; set; }
            public string? AssetCode { get; set; }
            public string? AssetDescription { get; set; }
            public string? ItemCode { get; set; }
            public string? ItemDescription { get; set; }
            public DateTime? CommissionDate { get; set; }
            public int? Qty { get; set; }
            public int? AssetAge { get; set; }
            public string? CurrentStatus { get; set; }
            public string? TRStatus { get; set; }
            public string? Location { get; set; }
            public string? Division { get; set; }
            public string? Department { get; set; }
            public string? Bay { get; set; }
        }

        public class AssetRegisterDto
        {
            public int SLNo { get; set; }
            public int ID { get; set; }
            public int AssetID { get; set; }
            public string AssetCode { get; set; }
            public string AssetDescription { get; set; }
            public string ItemCode { get; set; }
            public string ItemDescription { get; set; }
            public string DateCommission { get; set; }
            public int Qty { get; set; }
            public int AssetAge { get; set; }
            public string CurrentStatus { get; set; }
            public string TRStatus { get; set; }
            public string Location { get; set; }
            public string Division { get; set; }
            public string Department { get; set; }
            public string Bay { get; set; }
        }


    }
}
