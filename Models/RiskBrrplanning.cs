using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class RiskBrrplanning
{
    public int BrrpPkid { get; set; }

    public int? BrrpYearId { get; set; }

    public int? BrrpCustId { get; set; }

    public int? BrrpSalesUnitCode { get; set; }

    public int? BrrpBranchId { get; set; }

    public int? BrrpRegionId { get; set; }

    public int? BrrpZoneId { get; set; }

    public int? BrrpRiskScore { get; set; }

    public int? BrrpBrrratingId { get; set; }

    public int? BrrpBcmratingId { get; set; }

    public int? BrrpIaratingId { get; set; }

    public int? BrrpGrossControlScore { get; set; }

    public int? BrrpNetScore { get; set; }

    public int? BrrpAaplan { get; set; }

    public string? BrrpRemarks { get; set; }

    public string? BrrpStatus { get; set; }

    public string? BrrpDelFlag { get; set; }

    public int? BrrpCrBy { get; set; }

    public DateTime? BrrpCrOn { get; set; }

    public int? BrrpUpdatedBy { get; set; }

    public DateTime? BrrpUpdatedOn { get; set; }

    public int? BrrpSubmittedBy { get; set; }

    public DateTime? BrrpSubmittedOn { get; set; }

    public string? BrrpIpaddress { get; set; }

    public int? BrrpCompId { get; set; }
}
