namespace TracePca.Dto.Audit
{
    public class AttachmentDto
    {
        public int SrNo { get; set; }
        public int AtchID { get; set; }
        public int DrlId { get; set; }
        public string FName { get; set; }
        public string FDescription { get; set; }
        public int CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string FileSize { get; set; }
        public string Extention { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
}
