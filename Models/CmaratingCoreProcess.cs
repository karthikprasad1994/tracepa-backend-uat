using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CmaratingCoreProcess
{
    public int? CmacrId { get; set; }

    public int? CmacrYearId { get; set; }

    public float? CmacrStartValue { get; set; }

    public float? CmacrEndValue { get; set; }

    public string? CmacrName { get; set; }

    public string? CmacrColor { get; set; }

    public string? CmacrDesc { get; set; }

    public string? CmacrFlag { get; set; }

    public string? CmacrStatus { get; set; }

    public int? CmacrCrBy { get; set; }

    public DateTime? CmacrCrOn { get; set; }

    public int? CmacrUpdatedBy { get; set; }

    public DateTime? CmacrUpdatedOn { get; set; }

    public int? CmacrApprovedBy { get; set; }

    public DateTime? CmacrApprovedOn { get; set; }

    public int? CmacrRecallBy { get; set; }

    public DateTime? CmacrRecallOn { get; set; }

    public int? CmacrDeletedBy { get; set; }

    public DateTime? CmacrDeletedOn { get; set; }

    public string? CmacrIpaddress { get; set; }

    public int? CmacrCompId { get; set; }
}
