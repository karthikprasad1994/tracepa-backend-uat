using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CrpaValueRating
{
    public int? CvrId { get; set; }

    public int? CvrYearId { get; set; }

    public string? CvrPoint { get; set; }

    public int? CvrAuditId { get; set; }

    public string? CvrName { get; set; }

    public string? CvrDesc { get; set; }

    public string? CvrFlag { get; set; }

    public string? CvrStatus { get; set; }

    public int? CvrCrBy { get; set; }

    public DateTime? CvrCrOn { get; set; }

    public int? CvrUpdatedBy { get; set; }

    public DateTime? CvrUpdatedOn { get; set; }

    public int? CvrApprovedBy { get; set; }

    public DateTime? CvrApprovedOn { get; set; }

    public int? CvrRecallBy { get; set; }

    public DateTime? CvrRecallOn { get; set; }

    public int? CvrDeletedBy { get; set; }

    public DateTime? CvrDeletedOn { get; set; }

    public string? CvrIpaddress { get; set; }

    public int? CvrCompId { get; set; }
}
