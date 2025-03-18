using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditAnnualPlan
{
    public int? AapPkid { get; set; }

    public int? AapYearId { get; set; }

    public int? AapMonthId { get; set; }

    public int? AapCustId { get; set; }

    public int? AapFunId { get; set; }

    public string? AapResourceId { get; set; }

    public int? AapCrby { get; set; }

    public DateTime? AapCron { get; set; }

    public int? AapUpdatedby { get; set; }

    public DateTime? AapUpdatedOn { get; set; }

    public string? AapIpaddress { get; set; }

    public int? AapCompId { get; set; }

    public string? AapComments { get; set; }
}
