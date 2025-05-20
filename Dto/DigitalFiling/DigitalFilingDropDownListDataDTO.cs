namespace TracePca.Dto.DigitalFiling
{
    public class DigitalFilingDropDownListDataDTO
    {
        public List<DFDropDownListData> DepartmentList { get; set; }
        public List<DFDropDownListData> CabinetUserPermissionList { get; set; }
    }

    public class DFDropDownListData
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class UpdateCabinetStatusRequestDTO
    {
        public int CompId { get; set; }
        public int UserId { get; set; }
        public int CabinetNode { get; set; }
        public string StatusCode { get; set; } = string.Empty;
        public string Flag { get; set; } = string.Empty;
        public int CabinetId { get; set; } = 0;
    }
}
