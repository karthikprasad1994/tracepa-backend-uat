using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccAssetLocationSetup
{
    public int? LsId { get; set; }

    public string? LsDescription { get; set; }

    public string? LsDescCode { get; set; }

    public string? LsCode { get; set; }

    public int? LsLevelCode { get; set; }

    public int? LsParentId { get; set; }

    public int? LsCreatedBy { get; set; }

    public DateTime? LsCreatedOn { get; set; }

    public int? LsUpdatedBy { get; set; }

    public DateTime? LsUpdatedOn { get; set; }

    public string? LsDelFlag { get; set; }

    public string? LsStatus { get; set; }

    public int? LsYearId { get; set; }

    public int? LsCompId { get; set; }

    public int? LsCustId { get; set; }

    public int? LsApprovedBy { get; set; }

    public DateTime? LsApprovedOn { get; set; }

    public string? LsOpeartion { get; set; }

    public string? LsIpaddress { get; set; }
}
