namespace TracePca.Dto.Audit
{
    public class SarAttachmentsDto
    {
        public int DRL_ID { get; set; }          // The DRL Log ID
        public string Remarks { get; set; }      // Remarks for the attachment
        public string AddedBy { get; set; }      // The user who added the remarks
        public DateTime AddedDate { get; set; }  // The date when the remarks were added
    }
}
