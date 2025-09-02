namespace TracePca.Dto.Email
{
    public class CommonEmailDto
    {
        public List<string> ToEmails { get; set; } = new();
        public List<string> CcEmails { get; set; } = new();
        public string EmailType { get; set; } // "OTP", "AuditLifecycle", "DuringAudit"
        public Dictionary<string, string> Parameters { get; set; } = new(); // dynamic data
    }
}
