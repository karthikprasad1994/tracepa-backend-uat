using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccLedgerHeadingFactor
{
    public int AlhfId { get; set; }

    public int? AlhfCustId { get; set; }

    public int? AlhfBranchid { get; set; }

    public int? AlhfYearid { get; set; }

    public int? AlhfSchedule { get; set; }

    public int? AlhfHeadingId { get; set; }

    public int? AlhfRiskId { get; set; }

    public int? AlhfMaterialId { get; set; }

    public DateTime? AlhfCron { get; set; }

    public int? AlhfCrby { get; set; }
}
