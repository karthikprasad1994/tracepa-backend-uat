using TracePca.Dto.Audit;
namespace TracePca.Dto.TaskManagement
{
    public class TaskDropDownListDataDTO
    {
        public DropDownListData? CurrentYear { get; set; }
        public List<DropDownListData>? YearList { get; set; }
        public List<DropDownListData>? ClientList { get; set; }
        public List<DropDownListData>? TaskList { get; set; }
        public List<DropDownListData>? PartnerList { get; set; }
        public List<DropDownListData>? TeamMemberList { get; set; }
        public List<DropDownListData>? ProjectList { get; set; }
        public List<DropDownListData>? FrequencyList { get; set; }
        public List<DropDownListData>? WorkStatusList { get; set; }
        public List<DropDownListData>? CompanyList { get; set; }
        public List<DropDownListData>? TaxListTask { get; set; }
    }
}
