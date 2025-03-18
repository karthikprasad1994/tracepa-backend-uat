using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CaiqFactor
{
    public int? CfPkid { get; set; }

    public int? CfYearId { get; set; }

    public int? CfAuditId { get; set; }

    public string? CfName { get; set; }

    public string? CfDesc { get; set; }

    public string? CfFlag { get; set; }

    public string? CfStatus { get; set; }

    public int? CfCrBy { get; set; }

    public DateTime? CfCrOn { get; set; }

    public int? CfUpdatedBy { get; set; }

    public DateTime? CfUpdatedOn { get; set; }

    public int? CfApprovedBy { get; set; }

    public DateTime? CfApprovedOn { get; set; }

    public int? CfRecallBy { get; set; }

    public DateTime? CfRecallOn { get; set; }

    public int? CfDeletedBy { get; set; }

    public DateTime? CfDeletedOn { get; set; }

    public string? CfIpaddress { get; set; }

    public int? CfCompId { get; set; }
}
