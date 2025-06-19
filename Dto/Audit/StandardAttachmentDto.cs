namespace TracePca.Dto.Audit
{
    public class StandardAttachmentDto
    {
        public int SrNo { get; set; }
        public int AtchID { get; set; }
        public string FName { get; set; }
        public string FDescription { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string FileSize { get; set; }
        public string Extention { get; set; }
        public int Type { get; set; }
    }
}
