using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditTimeSheet
{
    public int? TsPkid { get; set; }

    public int? TsAuditCodeId { get; set; }

    public int? TsFunId { get; set; }

    public int? TsCustId { get; set; }

    public int? TsTaskId { get; set; }

    public string? TsTaskType { get; set; }

    public int? TsUserId { get; set; }

    public DateTime? TsDate { get; set; }

    public string? TsComments { get; set; }

    public decimal? TsHours { get; set; }

    public string? TsIsApproved { get; set; }

    public string? TsApproverRemarks { get; set; }

    public int? TsDescid { get; set; }

    public string? TsDelflg { get; set; }

    public string? TsStatus { get; set; }

    public DateTime? TsCron { get; set; }

    public int? TsCrby { get; set; }

    public DateTime? TsUpdatedOn { get; set; }

    public int? TsUpdatedBy { get; set; }

    public int? TsApprovedby { get; set; }

    public DateTime? TsApprovedon { get; set; }

    public string? TsIpaddress { get; set; }

    public int? TsYearId { get; set; }

    public int? TsCompId { get; set; }
}
