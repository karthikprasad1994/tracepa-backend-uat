using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccCashflow
{
    public int AcfPkid { get; set; }

    public string AcfDescription { get; set; } = null!;

    public int? AcfCustid { get; set; }

    public decimal? AcfCurrentAmount { get; set; }

    public decimal? AcfPrevAmount { get; set; }

    public string? AcfStatus { get; set; }

    public int? AcfCrby { get; set; }

    public DateTime? AcfCron { get; set; }

    public int? AcfUpdatedby { get; set; }

    public DateTime? AcfUpdatedOn { get; set; }

    public int? AcfCompid { get; set; }

    public string? AcfIpaddress { get; set; }

    public int? AcfCatagary { get; set; }

    public int? AcfBranchid { get; set; }

    public int? AcfYearid { get; set; }
}
