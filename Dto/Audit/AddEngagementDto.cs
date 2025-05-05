using System.Text.Json.Serialization;

namespace TracePca.Dto.Audit
{
    public class AddEngagementDto
    {

        // ===================== LOE Table Fields =====================
        [JsonIgnore]
          public int? LoeId { get; set; }
            public int? LoeYearId { get; set; }
            public int? LoeCustomerId { get; set; }
            public int? LoeServiceTypeId { get; set; } // cmmId
            public string? LoeNatureOfService { get; set; }
           // public string? LoeMilestones { get; set; }
            public DateTime? LoeTimeSchedule { get; set; }
            public DateTime? LoeReportDueDate { get; set; }
          //  public int? LoeRembFilingFee { get; set; }
            public DateTime? LoeCrOn { get; set; }
            public int? LoeTotal { get; set; }
            public string? LoeName { get; set; }
            public int? LoeFrequency { get; set; }
           // public int? LoeFunctionId { get; set; }
           // public string? LoeSubFunctionId { get; set; }
          //  public DateTime? LoeUpdatedOn { get; set; }
          //  public int? LoeUpdatedBy { get; set; }
         //   public int? LoeApprovedby { get; set; }
         //   public DateTime? LoeApprovedon { get; set; }
          //  public string? LoeDelflag { get; set; }
            public string? LoeIpaddress { get; set; }
            public int? LoeCompId { get; set; }

            // ===================== LOE_Template Table Fields =====================
            public int LOET_Id { get; set; }
           // public int LOET_LOEID { get; set; }
            //public int LOET_CustomerId { get; set; }
           // public int LOET_FunctionId { get; set; }
           // public int LOE_CompID { get; set; }
            //public string LOET_Frequency { get; set; }
            //public string LOET_IPAddress { get; set; }
            //public string? LoeScopeOfWork { get; set; }
           // public string? LoeDeliverable { get; set; }
           // public int? LoeProfessionalFees { get; set; }
           // public int? LoeApprovedBy { get; set; }
           // public int? LoeCrBy { get; set; }
            public int? LoeAttachId { get; set; }

            // ===================== LOE_AdditionalFees Table Fields =====================
            public int FeesId { get; set; }
        [JsonIgnore] // 👈 This hides the property from appearing in request/response JSON
        public int ExpensesId { get; set; }

        [JsonIgnore]
        
        public string? FeeName { get; set; }


        // ... add any other fields used by related tables
    }

    }

