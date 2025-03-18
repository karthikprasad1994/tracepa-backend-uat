using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditWorkPaperHistory
{
    public int? AwphPkid { get; set; }

    public int? AwphWpid { get; set; }

    public int? AwphAuditId { get; set; }

    public int? AwphCustId { get; set; }

    public int? AwphFunctionId { get; set; }

    public string? AwphReviewerRemarks { get; set; }

    public string? AwphAuditorRemarks { get; set; }

    public int? AwphRrcrBy { get; set; }

    public DateTime? AwphRrcrOn { get; set; }

    public int? AwphArcrBy { get; set; }

    public DateTime? AwphArcrOn { get; set; }

    public string? AwphIpaddress { get; set; }

    public int? AwphCompId { get; set; }
}
