namespace TracePca.Dto.AssetRegister
{
    public class AssetDetailsDto
    {
        public string Afam_AssetCode { get; set; }
        public string AssetTypeName { get; set; }
        public string AFAM_Code { get; set; }
        public string Afam_Description { get; set; }
        public DateTime? Afam_CommissionDate { get; set; }
        public int Afam_Quantity { get; set; }
        public int Afam_AssetAge { get; set; }
        public string Afam_Status { get; set; } = "Not Available"; // Default
        public string Afam_TRStatus { get; set; } = "Not Available";

        public string UnitOfMeasurement { get; set; }  // This will now hold the CMM_DESC

    }
}
