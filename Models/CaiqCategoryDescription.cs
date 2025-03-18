using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CaiqCategoryDescription
{
    public int? CcdPkid { get; set; }

    public int? CcdYearId { get; set; }

    public int? CcdAuditId { get; set; }

    public int? CcdFactorId { get; set; }

    public int? CcdCategoryId { get; set; }

    public int? CcdDescriptorId { get; set; }

    public string? CcdName { get; set; }

    public string? CcdDesc { get; set; }

    public int? CcdDescValue { get; set; }

    public string? CcdFlag { get; set; }

    public string? CcdStatus { get; set; }

    public int? CcdCrBy { get; set; }

    public DateTime? CcdCrOn { get; set; }

    public int? CcdUpdatedBy { get; set; }

    public DateTime? CcdUpdatedOn { get; set; }

    public int? CcdApprovedBy { get; set; }

    public DateTime? CcdApprovedOn { get; set; }

    public int? CcdRecallBy { get; set; }

    public DateTime? CcdRecallOn { get; set; }

    public int? CcdDeletedBy { get; set; }

    public DateTime? CcdDeletedOn { get; set; }

    public string? CcdIpaddress { get; set; }

    public int? CcdCompId { get; set; }
}
