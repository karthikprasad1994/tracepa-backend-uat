using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditDrllog
{
    public int? AdrlId { get; set; }

    public int? AdrlYearId { get; set; }

    public int? AdrlAuditNo { get; set; }

    public int? AdrlFunId { get; set; }

    public int? AdrlCustId { get; set; }

    public int? AdrlRequestedListId { get; set; }

    public int? AdrlRequestedTypeId { get; set; }

    public string? AdrlRequestedOn { get; set; }

    public string? AdrlEmailId { get; set; }

    public string? AdrlComments { get; set; }

    public string? AdrlReceivedComments { get; set; }

    public string? AdrlReceivedOn { get; set; }

    public int? AdrlLogStatus { get; set; }

    public string? AdrlStatus { get; set; }

    public int? AdrlAttachId { get; set; }

    public int? AdrlCrBy { get; set; }

    public DateTime? AdrlCrOn { get; set; }

    public int? AdrlUpdatedBy { get; set; }

    public DateTime? AdrlUpdatedOn { get; set; }

    public string? AdrlIpaddress { get; set; }

    public int? AdrlCompId { get; set; }

    public string? AdrlTimlinetoResOn { get; set; }

    public int? AdrlAttchDocId { get; set; }

    public int? AdrlReportType { get; set; }
}
