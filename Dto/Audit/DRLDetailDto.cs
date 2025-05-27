using System.Text.Json.Serialization;

namespace TracePca.Dto.Audit
{
    public class DRLDetailDto
    {
        
        public int DRLID { get; set; }
        public int CheckPointID { get; set; }
        public string CheckPoint { get; set; }
        public string DocumentRequestedList { get; set; }
        public int DocumentRequestedListID { get; set; }
        public string DocumentRequestedType { get; set; }
        public int DocumentRequestedTypeID { get; set; }
        public string EmailID { get; set; }
        public string RequestedOn { get; set; }
        public string TimlinetoResOn { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public string ReceivedComments { get; set; }
        public string ReceivedOn { get; set; }
        public int AttachID { get; set; }
        public int ReportType { get; set; }
    }
}
