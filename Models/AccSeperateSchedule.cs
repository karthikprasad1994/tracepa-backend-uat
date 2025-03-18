using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccSeperateSchedule
{
    public int? SsPkid { get; set; }

    public int? SsFinancialYear { get; set; }

    public int? SsCustId { get; set; }

    public int? SsOrgtype { get; set; }

    public int? SsGroup { get; set; }

    public string? SsParticulars { get; set; }

    public decimal? SsValues { get; set; }

    public DateTime? SsDate { get; set; }

    public string? SsStatus { get; set; }

    public string? SsDelflag { get; set; }

    public int? SsCrBy { get; set; }

    public DateTime? SsCrOn { get; set; }

    public int? SsUpdatedBy { get; set; }

    public DateTime? SsUpdatedOn { get; set; }

    public int? SsApprovedby { get; set; }

    public DateTime? SsApprovedOn { get; set; }

    public string? SsIpaddress { get; set; }

    public int? SsCompId { get; set; }
}
