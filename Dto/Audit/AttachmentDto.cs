namespace TracePca.Dto.Audit
{
    public class AttachmentDto
    {
        public int SrNo { get; set; }

        public int PkId { get; set; }
        public int AtchID { get; set; }
        public int DrlId { get; set; }
        public string FName { get; set; }

        public string ReportName { get; set; }

        public string FDescription { get; set; }
        public int CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string FileSize { get; set; }
        public string Extention { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

        public int? ReportType { get; set; }

        public string AttchStatus { get; set; }
    }
    public class GetDocumentPathRequestDto
    {
        public int CompanyId { get; set; }
        public string Module { get; set; }
        public string UserId { get; set; }
        public int AttachId { get; set; }
        public int AttachDocId { get; set; }
    }

    public class AuditFrameworkIdDto
    {
        public int SA_AuditFrameworkId { get; set; }
    }

}
