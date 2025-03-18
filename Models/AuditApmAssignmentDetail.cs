using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditApmAssignmentDetail
{
    public int? AapmId { get; set; }

    public int? AapmYearId { get; set; }

    public int? AapmAuditCodeId { get; set; }

    public int? AapmCustId { get; set; }

    public int? AapmFunctionId { get; set; }

    public int? AapmAuditTaskId { get; set; }

    public string? AapmAuditTaskType { get; set; }

    public DateTime? AapmPstartDate { get; set; }

    public DateTime? AapmPendDate { get; set; }

    public string? AapmResource { get; set; }

    public int? AapmCrBy { get; set; }

    public DateTime? AapmCrOn { get; set; }

    public int? AapmUpdatedBy { get; set; }

    public DateTime? AapmUpdatedOn { get; set; }

    public string? AapmIpaddress { get; set; }

    public int? AapmCompId { get; set; }
}
