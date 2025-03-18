using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditAssignmentSubTaskMaster
{
    public int? AmId { get; set; }

    public string? AmCode { get; set; }

    public string? AmName { get; set; }

    public int? AmAuditAssignmentId { get; set; }

    public int? AmBillingTypeId { get; set; }

    public string? AmDesc { get; set; }

    public string? AmDelflg { get; set; }

    public DateTime? AmCron { get; set; }

    public int? AmCrby { get; set; }

    public int? AmApprovedby { get; set; }

    public DateTime? AmApprovedon { get; set; }

    public string? AmStatus { get; set; }

    public int? AmUpdatedby { get; set; }

    public DateTime? AmUpdatedon { get; set; }

    public int? AmDeletedby { get; set; }

    public DateTime? AmDeletedon { get; set; }

    public int? AmRecallby { get; set; }

    public DateTime? AmRecallon { get; set; }

    public string? AmIpaddress { get; set; }

    public int? AmCompId { get; set; }
}
