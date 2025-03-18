using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditCostBudgetDetail
{
    public int? CbdId { get; set; }

    public int? CbdYearId { get; set; }

    public int? CbdAuditCodeId { get; set; }

    public int? CbdDescId { get; set; }

    public int? CbdUserId { get; set; }

    public double? CbdPerHead { get; set; }

    public int? CbdCrBy { get; set; }

    public DateTime? CbdCrOn { get; set; }

    public int? CbdUpdateBy { get; set; }

    public DateTime? CbdUpdatedOn { get; set; }

    public int? CbdApprovedBy { get; set; }

    public DateTime? CbdApprovedOn { get; set; }

    public string? CbdIpaddress { get; set; }

    public string? CbdStatus { get; set; }

    public int? CbdCompId { get; set; }
}
