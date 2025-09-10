using DocumentFormat.OpenXml.Math;

namespace TracePca.Dto.CustomerMaster
{
    public class CreateCustomerMasterDto
    {
        public int? CustomerId { get; set; }
        public int? CompanyId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string? CompanyUrl { get; set; }
        public string? CompanyEmail { get; set; }
        public string? GroupName { get; set; }
        public string BoardofDirectors { get; set; }
        public int GroupIndividual { get; set; }
        public int GroupId { get; set; }
        public List<int> ServiceTypeId { get; set; }


        public int OrganizationTypeId { get; set; }
        public int IndustryTypeId { get; set; }
        public int ManagementTypeId { get; set; }
        public string CINNO { get; set; }
        public int FinancialYearId { get; set; }
        public DateTime? CommitmentDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
