using System.Text.Json.Serialization;

namespace TracePca.Dto.CustomerMaster
{
    public class CustomerDetailsDto
    {
        public int CustId { get; set; }
        public string CustName { get; set; }
        public string Status { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerCode { get; set; }
       public string CommitmentDate { get; set; }
        public int IndustrytypeId { get; set; }
        public int OrganizationtypeId { get; set; }
        public string CompanyUrl { get; set; }
        public string GroupName { get; set; }
        public int GroupIndividual { get; set; }
        public int ManagementTypeId { get; set; }   // depending on DB type, could be int
        public string CINNO { get; set; }
        public List<int> ServiceTypeId { get; set; }
        [JsonIgnore]
        public string ServiceTypeIdCsv { get; set; } // stored as CSV in DB
        public string BoardOfDirectors { get; set; }
        public int FinancialYearId { get; set; }

   }

}
