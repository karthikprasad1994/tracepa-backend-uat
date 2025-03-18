using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CrpaRating
{
    public int? CratPkid { get; set; }

    public int? CratYearId { get; set; }

    public int? CratAuditId { get; set; }

    public float? CratStartValue { get; set; }

    public float? CratEndValue { get; set; }

    public string? CratDesc { get; set; }

    public string? CratName { get; set; }

    public string? CratColor { get; set; }

    public string? CratFlag { get; set; }

    public string? CratStatus { get; set; }

    public int? CratCrBy { get; set; }

    public DateTime? CratCrOn { get; set; }

    public int? CratUpdatedBy { get; set; }

    public DateTime? CratUpdatedOn { get; set; }

    public int? CratApprovedBy { get; set; }

    public DateTime? CratApprovedOn { get; set; }

    public int? CratRecallBy { get; set; }

    public DateTime? CratRecallOn { get; set; }

    public int? CratDeletedBy { get; set; }

    public DateTime? CratDeletedOn { get; set; }

    public string? CratIpaddress { get; set; }

    public int? CratCompId { get; set; }
}
