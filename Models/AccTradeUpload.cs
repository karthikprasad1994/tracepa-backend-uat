using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccTradeUpload
{
    public int? AtuId { get; set; }

    public string AtuName { get; set; } = null!;

    public int? AtuCustId { get; set; }

    public decimal? AtuLessThanSixMonth { get; set; }

    public decimal? AtuMoreThanSixMonth { get; set; }

    public decimal? AtuOneYear { get; set; }

    public decimal? AtuTwoYear { get; set; }

    public decimal? AtuThreeYear { get; set; }

    public decimal? AtuMoreThan { get; set; }

    public decimal? AtuTotalAmount { get; set; }

    public DateTime? AtuCron { get; set; }

    public int? AtuCrby { get; set; }

    public int? AtuApprovedby { get; set; }

    public DateTime? AtuApprovedon { get; set; }

    public int? AtuUpdatedby { get; set; }

    public DateTime? AtuUpdatedon { get; set; }

    public string? AtuIpaddress { get; set; }

    public int? AtuYearid { get; set; }

    public int? AtuBranchid { get; set; }

    public int? AtuCategory { get; set; }

    public int? AtuOtherType { get; set; }
}
