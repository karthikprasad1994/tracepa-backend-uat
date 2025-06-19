using System.Text.Json.Serialization;

namespace TracePca.Dto.Audit
{
    public class WorkpaperDto
    {
        
        public int? WorkpaperId { get; set; }
        public int AuditId { get; set; }
        public string WorkpaperRef { get; set; }
        public int TypeOfTestId { get; set; }
        public int? CheckListId { get; set; }
       
        public int? DocumentRequestId { get; set; }
        public int ExceededMaterialityId { get; set; }
        public decimal? AuditorHoursSpent { get; set; }
        public string Observation { get; set; }
        public string NotesSteps { get; set; }
        public string CriticalAuditMatter { get; set; }
        public string Conclusion { get; set; }
        public int StatusId { get; set; }
        public int AttachId { get; set; }
        public int CreatedBy { get; set; }
        public string IPAddress { get; set; }
        public int CompanyId { get; set; }
        
        //public int CustomerAccessCodeId { get; set; }
    }
}
