using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CaiqDescriptor
{
    public int? CdPkid { get; set; }

    public int? CdYearId { get; set; }

    public int? CdAuditId { get; set; }

    public string? CdName { get; set; }

    public string? CdDesc { get; set; }

    public string? CdRange { get; set; }

    public string? CdFlag { get; set; }

    public string? CdStatus { get; set; }

    public int? CdCrBy { get; set; }

    public DateTime? CdCrOn { get; set; }

    public int? CdUpdatedBy { get; set; }

    public DateTime? CdUpdatedOn { get; set; }

    public int? CdApprovedBy { get; set; }

    public DateTime? CdApprovedOn { get; set; }

    public int? CdRecallBy { get; set; }

    public DateTime? CdRecallOn { get; set; }

    public int? CdDeletedBy { get; set; }

    public DateTime? CdDeletedOn { get; set; }

    public string? CdIpaddress { get; set; }

    public int? CdCompId { get; set; }
}
