using System.Text.Json.Serialization;
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

    public class LocalAttachmentDto
    {
        public List<IFormFile>? Files { get; set; } = new();

        public string ModuleName { get; set; } = string.Empty;
        public string? AccessCodeDirectory { get; set; } = string.Empty;

        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public int AttachmentId { get; set; } = 0;

        public int ADRLId { get; set; }
        public int AuditNo { get; set; }
        public int CustomerId { get; set; }

        public string RequestedOn { get; set; } = string.Empty;
        public string? TimelineToRespondOn { get; set; } = string.Empty;
        public List<string> EmailIds { get; set; } = new();
        public string? Comments { get; set; } = string.Empty;

        public int ReportType { get; set; }

        public string IPAddress { get; set; } = string.Empty;
        public int YearId { get; set; }
        public int RequestedListId { get; set; }

        public int SendeMailFlag { get; set; }
    }


}
