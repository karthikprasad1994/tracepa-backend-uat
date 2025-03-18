using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccProfitAndLossAmount
{
    public int AccPnLPkid { get; set; }

    public int AccPnLCustid { get; set; }

    public string? AccPnLAmount { get; set; }

    public int? AccPnLCrBy { get; set; }

    public DateTime? AccPnLCrOn { get; set; }

    public int? AccPnLFlag { get; set; }

    public int? AccPnLYearid { get; set; }

    public string? AccPnLBranchId { get; set; }

    public int? AccPnLDurtnId { get; set; }
}
