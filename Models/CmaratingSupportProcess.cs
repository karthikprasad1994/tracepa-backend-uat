using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CmaratingSupportProcess
{
    public int? CmasrId { get; set; }

    public int? CmasrYearId { get; set; }

    public float? CmasrStartValue { get; set; }

    public float? CmasrEndValue { get; set; }

    public string? CmasrDesc { get; set; }

    public string? CmasrName { get; set; }

    public string? CmasrColor { get; set; }

    public string? CmasrFlag { get; set; }

    public string? CmasrStatus { get; set; }

    public int? CmasrCrBy { get; set; }

    public DateTime? CmasrCrOn { get; set; }

    public int? CmasrUpdatedBy { get; set; }

    public DateTime? CmasrUpdatedOn { get; set; }

    public int? CmasrApprovedBy { get; set; }

    public DateTime? CmasrApprovedOn { get; set; }

    public int? CmasrRecallBy { get; set; }

    public DateTime? CmasrRecallOn { get; set; }

    public int? CmasrDeletedBy { get; set; }

    public DateTime? CmasrDeletedOn { get; set; }

    public string? CmasrIpaddress { get; set; }

    public int? CmasrCompId { get; set; }
}
