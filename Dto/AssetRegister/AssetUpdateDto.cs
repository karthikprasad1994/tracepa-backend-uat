namespace TracePca.Dto.AssetRegister
{
    public class AssetUpdateDto
    {
        public string CustomerName { get; set; }         // For mapping to CUST_ID
        public string UnitOfMeasurement { get; set; }
        public string AssetCode { get; set; }
        // For mapping to CMM_ID
        public int YMSID { get; set; }                   // Used directly to get YMS_YEARID
        public string AssetClassName { get; set; }
        public string Afam_ItemCode { get; set; }
        public string Afam_ItemDesc { get; set; }
        public decimal? Afam_PurchaseAmount { get; set; }
        public DateTime? Afam_PurchaseDate { get; set; }
        public string Afam_PolicyNo { get; set; }
        public decimal? Afam_Amount { get; set; }
        public int? Afam_Location { get; set; }
        public int? Afam_Division { get; set; }
        public int? Afam_Department { get; set; }
        public int? Afam_Bay { get; set; }
        public string Afam_EmployeeName { get; set; }
        public string Afam_EmployeeCode { get; set; }
        public string Afam_BrokerName { get; set; }
        public string Afam_CompanyName { get; set; }
        public string Afam_SupplierName { get; set; }
        public string Afam_ContactPerson { get; set; }
        public string Afam_Address { get; set; }
        public string Afam_Phone { get; set; }
        public string Afam_Fax { get; set; }
        public string Afam_EmailID { get; set; }
        public string Afam_Unit { get; set; }
        public string Afam_UpdatedBy { get; set; }
    }
}
