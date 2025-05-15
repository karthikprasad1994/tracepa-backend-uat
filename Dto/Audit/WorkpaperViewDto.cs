namespace TracePca.Dto.Audit
{
    public class WorkpaperViewDto
    {
        public int PKID { get; set; }
        public string WorkpaperNo { get; set; }
        public string WorkpaperRef { get; set; }
        public string Observation { get; set; }
        public string Conclusion { get; set; }
        public string ReviewerComments { get; set; }
        public string TypeOfTest { get; set; }
        public string Status { get; set; }
        public int AttachID { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ReviewedBy { get; set; }
        public string ReviewedOn { get; set; }
    }
}
