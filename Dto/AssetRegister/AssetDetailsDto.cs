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
        public string Afam_Status { get; set; }
        public string? TR_Status { get; set; }
    }
}
