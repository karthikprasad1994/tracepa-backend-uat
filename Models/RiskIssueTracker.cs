using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class RiskIssueTracker
{
    public int? RitPkid { get; set; }

    public string? RitSource { get; set; }

    public string? RitIssueNo { get; set; }

    public string? RitReferenceNo { get; set; }

    public int? RitAsgNo { get; set; }

    public int? RitFinancialYear { get; set; }

    public int? RitCustId { get; set; }

    public int? RitFunId { get; set; }

    public int? RitSubFunId { get; set; }

    public string? RitIssueHeading { get; set; }

    public string? RitIssueDesc { get; set; }

    public int? RitRiskId { get; set; }

    public int? RitRiskTypeId { get; set; }

    public int? RitControlId { get; set; }

    public string? RitActualLoss { get; set; }

    public string? RitProbableLoss { get; set; }

    public string? RitActionPlan { get; set; }

    public DateTime? RitTargetDate { get; set; }

    public int? RitOpenCloseStatus { get; set; }

    public int? RitManagerResponsible { get; set; }

    public int? RitIndividualResponsible { get; set; }

    public string? RitRemaks { get; set; }

    public int? RitAttchId { get; set; }

    public int? RitCrBy { get; set; }

    public DateTime? RitCrOn { get; set; }

    public int? RitUpdatedBy { get; set; }

    public DateTime? RitUpdatedOn { get; set; }

    public int? RitSubmittedBy { get; set; }

    public DateTime? RitSubmittedOn { get; set; }

    public string? RitStatus { get; set; }

    public string? RitIpaddress { get; set; }

    public int? RitCompId { get; set; }

    public int? RitPgedetailId { get; set; }
}
