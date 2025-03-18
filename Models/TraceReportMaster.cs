using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class TraceReportMaster
{
    public int? TrmId { get; set; }

    public string? TrmHeaderName { get; set; }

    public string? TrmDescription { get; set; }

    public int? TrmParent { get; set; }

    public int? TrmSubParent { get; set; }

    public int? TrmHead { get; set; }

    public int? TrmRptId { get; set; }

    public int? TrmIndId { get; set; }

    public int? TrmCustId { get; set; }
}
