namespace TracePca.Dto.Audit
{
    public class EdtAttachmentsDto
    {
        public int DRL_ID { get; set; }         // The DRL Log ID
        public string AttachmentPath { get; set; }  // Path to the attachment
        public string UploadedBy { get; set; }   // The user who uploaded the attachment
        public DateTime UploadedDate { get; set; } // The date when the attachment was uploaded
    }
}
