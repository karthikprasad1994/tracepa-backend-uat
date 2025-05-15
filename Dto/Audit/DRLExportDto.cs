namespace TracePca.Dto.Audit
{
    public class DRLExportDto
    {
        public int DRL_ID { get; set; }          // The DRL Log ID
        public string ExportedBy { get; set; }   // User who is exporting the log
        public string ExportRemarks { get; set; } // Remarks for the export
        public string ExportedToEmail { get; set; } // Email address to send the export to
    }
}
