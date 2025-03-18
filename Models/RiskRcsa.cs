using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class RiskRcsa
{
    public int? RcsaPkid { get; set; }

    public string? RcsaAsgNo { get; set; }

    public int? RcsaFinancialYear { get; set; }

    public int? RcsaCustId { get; set; }

    public int? RcsaFunId { get; set; }

    public int? RcsaOwnerId { get; set; }

    public DateTime? RcsaTargetDate { get; set; }

    public string? RcsaActionPlan { get; set; }

    public string? RcsaFactorIncrease { get; set; }

    public string? RcsaFactorDecrease { get; set; }

    public string? RcsaComments { get; set; }

    public float? RcsaNetScore { get; set; }

    public int? RcsaCrBy { get; set; }

    public DateTime? RcsaCrOn { get; set; }

    public int? RcsaRupdatedBy { get; set; }

    public DateTime? RcsaRupdatedOn { get; set; }

    public int? RcsaRsubmittedBy { get; set; }

    public DateTime? RcsaRsubmittedOn { get; set; }

    public int? RcsaBupdatedBy { get; set; }

    public DateTime? RcsaBupdatedOn { get; set; }

    public int? RcsaBsubmittedBy { get; set; }

    public DateTime? RcsaBsubmittedOn { get; set; }

    public int? RcsaReAssignBy { get; set; }

    public DateTime? RcsaReAssignOn { get; set; }

    public int? RcsaApprovedBy { get; set; }

    public DateTime? RcsaApprovedOn { get; set; }

    public string? RcsaStatus { get; set; }

    public string? RcsaIpaddress { get; set; }

    public int? RcsaCompId { get; set; }
}
