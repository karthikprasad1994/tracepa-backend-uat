namespace TracePca.Dto.Audit
{
    public class UpdateScheduleCheckPointDto
    {
        public int SACId { get; set; }
        public int AuditId { get; set; }
        public int UserId { get; set; }
        public string Remarks { get; set; }
        public int Annexure { get; set; }
      
        public int ConductAuditCheckPointId { get; set; }
        public int CheckPointId { get; set; }
        public int CompanyId { get; set; }
        public int RemarksType { get; set; }
    }
}
