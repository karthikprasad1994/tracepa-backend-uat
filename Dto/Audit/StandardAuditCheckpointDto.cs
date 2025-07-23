namespace TracePca.Dto.Audit
{
    public class StandardAuditCheckpointDto
    {
        public int AuditId { get; set; }
        public int SrNo { get; set; }
        public int ConductAuditCheckPointPKId { get; set; }
        public int CheckPointID { get; set; }
        public string Heading { get; set; }
        public string CheckPoint { get; set; }
        public string Assertions { get; set; }
        public string Remarks { get; set; }
        public string Mandatory { get; set; }
        public string TestResult { get; set; }
        public string ReviewerRemarks { get; set; }
        public int AttachmentID { get; set; }
        public string Annexure { get; set; }
        public string ConductedBy { get; set; }
        public string LastUpdatedOn { get; set; }
        public int WorkpaperId { get; set; }
        public string WorkpaperNo { get; set; }
        public string WorkpaperRef { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int DRLCount { get; set; }
    }
}
