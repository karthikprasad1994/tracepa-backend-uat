using TracePca.Dto.Audit;

namespace TracePca.Dto
{
    public class SaveAuditDataRequest
    {
       
        public string Module { get; set; } = string.Empty;
        public string Form { get; set; } = string.Empty;
        public string Event { get; set; } = string.Empty;
        public int MasterId { get; set; }
        public string MasterName { get; set; } = string.Empty;
        public int SubMasterId { get; set; }
        public string SubMasterName { get; set; } = string.Empty;
        public int AttachId { get; set; }
        public int DrlLogId { get; set; }
    }
}
