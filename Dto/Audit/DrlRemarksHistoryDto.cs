namespace TracePca.Dto.Audit
{
    public class DrlRemarksHistoryDto
    {
        public int SarId { get; set; }
        public string Role { get; set; }
        public string RemarksBy { get; set; }
        public string Remarks { get; set; }
        public string Date { get; set; }           // formatted as "dd-MM-yyyy hh:mm:ss tt"
        public string Timeline { get; set; }
        public string Comments { get; set; }
    }
}
