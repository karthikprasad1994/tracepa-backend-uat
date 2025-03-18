using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccAssetDepreciationPrev
{
    public int? AdepId { get; set; }

    public int? AdepAssetId { get; set; }

    public string? AdepItem { get; set; }

    public decimal? AdepRateofDep { get; set; }

    public decimal? AdepOpbforYr { get; set; }

    public decimal? AdepDepreciationforFy { get; set; }

    public decimal? AdepWrittenDownValue { get; set; }

    public DateTime? AdepClosingDate { get; set; }

    public int? AdepCreatedBy { get; set; }

    public DateTime? AdepCreatedOn { get; set; }

    public int? AdepUpdatedBy { get; set; }

    public DateTime? AdepUpdatedOn { get; set; }

    public int? AdepApprovedBy { get; set; }

    public DateTime? AdepApprovedOn { get; set; }

    public string? AdepDelFlag { get; set; }

    public string? AdepStatus { get; set; }

    public int? AdepYearId { get; set; }

    public int? AdepCompId { get; set; }

    public int? AdepCustId { get; set; }

    public int? AdepLocation { get; set; }

    public int? AdepDivision { get; set; }

    public int? AdepDepartment { get; set; }

    public int? AdepBay { get; set; }

    public int? AdepTransType { get; set; }

    public int? AdepMethod { get; set; }

    public string? AdepOpeartion { get; set; }

    public string? AdepIpaddress { get; set; }
}
