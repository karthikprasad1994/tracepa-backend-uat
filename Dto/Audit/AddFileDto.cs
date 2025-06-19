using System.Text.Json.Serialization;

namespace TracePca.Dto.Audit
{
    public class AddFileDto 
    {
        public int CustomerId { get; set; }
        public int AuditId { get; set; }
        public int AtchId { get; set; }

       // public int DocId { get; set; }

      //   public int ReportId { get; set; } 
        // public string FilePath { get; set; }
        // public string FileName { get; set; }
        public IFormFile? File { get; set; }
        public int UserId { get; set; }
        public string EmailId { get; set; }
        public string IpAddress { get; set; }
        public int YearId { get; set; }
        [JsonIgnore]
        public string ExportType { get; set; }

        public DateTime? RequestedOn { get; set; }
        public DateTime? RespondTime { get; set; }

        public int  ReportType { get; set; }
        [JsonIgnore]
        public string DocumentName { get; set; }
        public string? Remark { get; set; }
        public string Type { get; set; }

        public string Status { get; set; }


        public int AuditScheduleId { get; set; }
        public int SubProcessId { get; set; }
        public int CompId { get; set; }
        public int DrlId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        
        public int AdrlId { get; set; }



    }
}
