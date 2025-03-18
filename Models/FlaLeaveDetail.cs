using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class FlaLeaveDetail
{
    public int? LpeId { get; set; }

    public int? LpeEmpid { get; set; }

    public int? LpeYearId { get; set; }

    public DateTime? LpeFromdate { get; set; }

    public DateTime? LpeTodate { get; set; }

    public int? LpeDays { get; set; }

    public string? LpePurpose { get; set; }

    public string? LpeDelFlag { get; set; }

    public string? LpeStatus { get; set; }

    public int? LpeCrBy { get; set; }

    public DateTime? LpeCrOn { get; set; }

    public int? LpeUpdatedBy { get; set; }

    public DateTime? LpeUpdatedOn { get; set; }

    public string? LpeApprove { get; set; }

    public string? LpeApprovedDetails { get; set; }

    public string? LpeIpaddress { get; set; }

    public int? LpeCompId { get; set; }
}
