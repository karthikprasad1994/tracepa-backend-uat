using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditTimeCostBudgetMaster
{
    public int? AtcbPkid { get; set; }

    public int? AtcbYearId { get; set; }

    public int? AtcbAuditCodeId { get; set; }

    public string? AtcbType { get; set; }

    public int? AtcbTaskProcessId { get; set; }

    public int? AtcbTotalDays { get; set; }

    public int? AtcbTotalHours { get; set; }

    public double? AtcbTotalCost { get; set; }

    public string? AtcbDelFlag { get; set; }

    public string? AtcbStatus { get; set; }

    public int? AtcbCreatedby { get; set; }

    public DateTime? AtcbCreatedon { get; set; }

    public int? AtcbUpdatedby { get; set; }

    public DateTime? AtcbUpdatedOn { get; set; }

    public int? AtcbSubmittedby { get; set; }

    public DateTime? AtcbSubmittedOn { get; set; }

    public string? AtcbIpaddress { get; set; }

    public int? AtcbCompId { get; set; }
}
