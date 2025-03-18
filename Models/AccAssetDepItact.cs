using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccAssetDepItact
{
    public int? AditactId { get; set; }

    public int? AditactAssetClassId { get; set; }

    public decimal? AditactRateofDep { get; set; }

    public decimal? AditactOpbforYr { get; set; }

    public decimal? AditactDepreciationforFy { get; set; }

    public decimal? AditactWrittenDownValue { get; set; }

    public decimal? AditactBfrQtrAmount { get; set; }

    public decimal? AditactBfrQtrDep { get; set; }

    public decimal? AditactAftQtrAmount { get; set; }

    public decimal? AditactAftQtrDep { get; set; }

    public decimal? AditactDelAmount { get; set; }

    public int? AditactCreatedBy { get; set; }

    public DateTime? AditactCreatedOn { get; set; }

    public int? AditactUpdatedBy { get; set; }

    public DateTime? AditactUpdatedOn { get; set; }

    public int? AditactApprovedBy { get; set; }

    public DateTime? AditactApprovedOn { get; set; }

    public string? AditactDelFlag { get; set; }

    public string? AditactStatus { get; set; }

    public int? AditactYearId { get; set; }

    public int? AditactCompId { get; set; }

    public int? AditactCustId { get; set; }

    public string? AditactOpeartion { get; set; }

    public string? AditactIpaddress { get; set; }

    public decimal? AditactInitAmt { get; set; }
}
