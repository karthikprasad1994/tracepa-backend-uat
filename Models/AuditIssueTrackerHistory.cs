using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditIssueTrackerHistory
{
    public int? AithPkid { get; set; }

    public int? AithIssuePkid { get; set; }

    public int? AithAuditId { get; set; }

    public int? AithCustId { get; set; }

    public int? AithFunctionId { get; set; }

    public string? AithReviewerRemarks { get; set; }

    public string? AithAuditorRemarks { get; set; }

    public int? AithRrcrBy { get; set; }

    public DateTime? AithRrcrOn { get; set; }

    public int? AithArcrBy { get; set; }

    public DateTime? AithArcrOn { get; set; }

    public string? AithIpaddress { get; set; }

    public int? AithCompId { get; set; }
}
