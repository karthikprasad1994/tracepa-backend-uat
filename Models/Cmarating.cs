using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class Cmarating
{
    public int? CmarId { get; set; }

    public int? CmarYearId { get; set; }

    public float? CmarStartValue { get; set; }

    public float? CmarEndValue { get; set; }

    public string? CmarDesc { get; set; }

    public string? CmarName { get; set; }

    public string? CmarColor { get; set; }

    public string? CmarFlag { get; set; }

    public string? CmarStatus { get; set; }

    public int? CmarCrBy { get; set; }

    public DateTime? CmarCrOn { get; set; }

    public int? CmarUpdatedBy { get; set; }

    public DateTime? CmarUpdatedOn { get; set; }

    public int? CmarApprovedBy { get; set; }

    public DateTime? CmarApprovedOn { get; set; }

    public int? CmarRecallBy { get; set; }

    public DateTime? CmarRecallOn { get; set; }

    public int? CmarDeletedBy { get; set; }

    public DateTime? CmarDeletedOn { get; set; }

    public string? CmarIpaddress { get; set; }

    public int? CmarCompId { get; set; }
}
