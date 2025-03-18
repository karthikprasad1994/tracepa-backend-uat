using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadUserEmpProfExperiance
{
    public int? SupPkid { get; set; }

    public int? SupUserEmpId { get; set; }

    public string? SupAssignment { get; set; }

    public string? SupReportingTo { get; set; }

    public int? SupFrom { get; set; }

    public int? SupTo { get; set; }

    public double? SupSalaryPerAnnum { get; set; }

    public string? SupPosition { get; set; }

    public string? SupRemarks { get; set; }

    public int? SupAttachId { get; set; }

    public int? SupCrBy { get; set; }

    public DateTime? SupCrOn { get; set; }

    public int? SupUpdatedBy { get; set; }

    public DateTime? SupUpdatedOn { get; set; }

    public string? SupIpaddress { get; set; }

    public int? SupCompId { get; set; }
}
