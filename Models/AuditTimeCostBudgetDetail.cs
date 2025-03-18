using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditTimeCostBudgetDetail
{
    public int? AtcdPkid { get; set; }

    public int? AtcdAtcbid { get; set; }

    public int? AtcdYearId { get; set; }

    public string? AtcdType { get; set; }

    public int? AtcdTaskProcessId { get; set; }

    public int? AtcdAuditCodeId { get; set; }

    public int? AtcdUserId { get; set; }

    public int? AtcdHours { get; set; }

    public int? AtcdHoursPerDay { get; set; }

    public int? AtcdDays { get; set; }

    public double? AtcdCost { get; set; }

    public double? AtcdCostPerDay { get; set; }

    public string? AtcdIpaddress { get; set; }

    public int? AtcdCompId { get; set; }
}
