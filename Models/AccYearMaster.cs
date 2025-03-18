using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccYearMaster
{
    public int YmsId { get; set; }

    public DateTime? YmsFromdate { get; set; }

    public DateTime? YmsTodate { get; set; }

    public int? YmsCrby { get; set; }

    public DateTime? YmsCron { get; set; }

    public string? YmsFreezeYear { get; set; }

    public int? YmsFromYear { get; set; }

    public int? YmsToYear { get; set; }

    public int? YmsCompId { get; set; }

    public string? YmsStatus { get; set; }

    public int? YmsDeletedBy { get; set; }

    public int? YmsReCalledBy { get; set; }

    public string? YmsDelFlag { get; set; }

    public int? YmsUpdatedBy { get; set; }

    public DateTime? YmsUpdatedOn { get; set; }

    public string? YmsOperation { get; set; }

    public string? YmsIpaddress { get; set; }

    public int? YmsApprovedBy { get; set; }

    public DateTime? YmsApprovedOn { get; set; }

    public int? YmsDefault { get; set; }
}
