using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CaiqFactorCategory
{
    public int? CfcPkid { get; set; }

    public int? CfcYearId { get; set; }

    public int? CfcAuditId { get; set; }

    public int? CfcFactorId { get; set; }

    public string? CfcName { get; set; }

    public string? CfcDesc { get; set; }

    public string? CfcFlag { get; set; }

    public string? CfcStatus { get; set; }

    public int? CfcCrBy { get; set; }

    public DateTime? CfcCrOn { get; set; }

    public int? CfcUpdatedBy { get; set; }

    public DateTime? CfcUpdatedOn { get; set; }

    public int? CfcApprovedBy { get; set; }

    public DateTime? CfcApprovedOn { get; set; }

    public int? CfcRecallBy { get; set; }

    public DateTime? CfcRecallOn { get; set; }

    public int? CfcDeletedBy { get; set; }

    public DateTime? CfcDeletedOn { get; set; }

    public string? CfcIpaddress { get; set; }

    public int? CfcCompId { get; set; }
}
