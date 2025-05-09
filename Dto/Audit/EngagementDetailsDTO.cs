using System.Text.Json.Serialization;

namespace TracePca.Dto.Audit
{
    public class EngagementDetailsDTO
    {
        public int? LOE_Id { get; set; } = 0;
        public int LOE_YearId { get; set; }
        public int LOE_CustomerId { get; set; }
        public int LOE_ServiceTypeId { get; set; }
        public int LOE_FunctionId { get; set; }        
        public string? LOE_NatureOfService { get; set; }
        public int LOE_Total { get; set; }
        public string? LOE_Name { get; set; }
        public int LOE_Frequency { get; set; }
        public DateTime? LOE_CrOn { get; set; } = DateTime.Now;
        public int LOE_CrBy { get; set; }
        public DateTime? LOE_UpdatedOn { get; set; }
        public int? LOE_UpdatedBy { get; set; }
        public DateTime? LOE_ApprovedBy { get; set; }
        public int? LOE_ApprovedON { get; set; }
        [JsonIgnore]
        public string? LOE_Delflag { get; set; } = "A";
        [JsonIgnore]
        public string? LOE_Status { get; set; } = "C";
        public string LOE_IPAddress { get; set; }
        public int LOE_CompID { get; set; }

        public int? LOET_Id { get; set; } = 0;
        public string? LOET_ScopeOfWork { get; set; }
        public string LOET_Frequency { get; set; }
        public int LOET_ProfessionalFees { get; set; } = 0;
        public int? LOE_AttachID { get; set; }
        public List<EngagementTemplateDetailsDTO> EngagementTemplateDetails { get; set; } = new List<EngagementTemplateDetailsDTO>();
        public List<EngagementAdditionalFeesDTO> EngagementAdditionalFees { get; set; } = new List<EngagementAdditionalFeesDTO>();
    }

    public class EngagementTemplateDetailsDTO
    {
        public int? LTD_ID { get; set; } = 0;
        public int? LTD_LOE_ID { get; set; } = 0;
        public int LTD_ReportTypeID { get; set; }
        public int LTD_HeadingID { get; set; }
        public string? LTD_Heading { get; set; }
        public string? LTD_Decription { get; set; }
        public string? LTD_FormName { get; set; } = "LOE";
        public int LTD_CrBy { get; set; }
        public DateTime LTD_CrOn { get; set; }
        public string LTD_IPAddress { get; set; }
        public int LTD_CompID { get; set; }
        public int? LTD_UpdatedBy { get; set; }
        public DateTime? LTD_UpdatedOn { get; set; }
    }

    public class EngagementAdditionalFeesDTO
    {
        public int? LAF_ID { get; set; } = 0;
        public int? LAF_LOEID { get; set; } = 0;
        public int LAF_OtherExpensesID { get; set; }
        public int LAF_Charges { get; set; }
        public string? LAF_OtherExpensesName { get; set; }
        [JsonIgnore]
        public string? LAF_Delflag { get; set; } = "A";
        [JsonIgnore]
        public string? LAF_Status { get; set; } = "A";
        public int LAF_CrBy { get; set; }
        public DateTime LAF_CrOn { get; set; }
        public int? LAF_UpdatedBy { get; set; }
        public DateTime? LAF_UpdatedOn { get; set; }
        public string LAF_IPAddress { get; set; }
        public int LAF_CompID { get; set; }
    }

    public class ReportTypeDetails
    {
        public int RCM_Id { get; set; }
        public string? RCM_ReportName { get; set; }
        public string? RCM_Heading { get; set; }
        public string? RCM_Description { get; set; }
    }
}

