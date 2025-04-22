namespace TracePca.Dto
{
    public class AssetMasterRequestDto
    {
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public List<int> AmIds { get; set; }
    }
}
