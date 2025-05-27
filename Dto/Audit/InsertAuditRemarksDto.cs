namespace TracePca.Dto.Audit
{
    public class InsertAuditRemarksDto
    {
        
        
            public int YearId { get; set; }
            public int AuditId { get; set; }
            public int FunctionId { get; set; }
            public int CustomerId { get; set; }
            public int RequestedListId { get; set; }
            public int RequestedTypeId { get; set; }
            public string RequestedOn { get; set; } = string.Empty;
            public string TimelineToRespondOn { get; set; } = string.Empty;
            public string EmailId { get; set; } = string.Empty;
            public string Remark { get; set; } = string.Empty;
            public int UserId { get; set; }
            public int UpdatedBy { get; set; }
            public string IpAddress { get; set; } = string.Empty;
            public int CompId { get; set; }

            // RemarksHistory-specific
            public string CheckPointIds { get; set; } = string.Empty;
            public int CustRegAccessCodeId { get; set; }
           public int AttachId { get; set; }
            public int MasId { get; set; }
            public int DrlId { get; set; }
        }


    }

