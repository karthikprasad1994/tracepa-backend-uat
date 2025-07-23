namespace TracePca.Dto.DigitalFiling
{
    public class DigitalFilingDropDownListDataDTO
    {
        public List<DFDropDownListData> DepartmentList { get; set; }
        public List<DFDropDownListData> StatusList { get; set; }
        public List<DFDropDownListData> PermissionLevelList { get; set; }
        public List<DFDropDownListData> PermissionOptionsList { get; set; }
        public List<DFDropDownListData> CabinetUserPermissionList { get; set; }
        public List<DFDropDownListData> SubCabinetList { get; set; }
    }

    public class DFDropDownListData
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
