using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditIssueTrackerDetail
{
    public int? AitPkid { get; set; }

    public int? AitYearId { get; set; }

    public int? AitCustId { get; set; }

    public int? AitAuditCode { get; set; }

    public int? AitWorkPaperId { get; set; }

    public int? AitFunctionId { get; set; }

    public int? AitSubFunctionId { get; set; }

    public int? AitProcessId { get; set; }

    public int? AitSubProcessId { get; set; }

    public int? AitRiskId { get; set; }

    public int? AitControlId { get; set; }

    public int? AitCheckId { get; set; }

    public string? AitIssueJobNo { get; set; }

    public int? AitSeverityId { get; set; }

    public int? AitRiskCategoryId { get; set; }

    public string? AitIssueName { get; set; }

    public string? AitCriteria { get; set; }

    public string? AitCondition { get; set; }

    public string? AitDetails { get; set; }

    public string? AitImpact { get; set; }

    public string? AitRootCause { get; set; }

    public string? AitSuggestedRemedies { get; set; }

    public int? AitOpenCloseStatus { get; set; }

    public string? AitAuditorRemarks { get; set; }

    public string? AitReviewerRemarks { get; set; }

    public int? AitCreatedBy { get; set; }

    public DateTime? AitCreatedOn { get; set; }

    public int? AitUpdatedBy { get; set; }

    public DateTime? AitUpdatedOn { get; set; }

    public int? AitReviewedBy { get; set; }

    public DateTime? AitReviewedOn { get; set; }

    public int? AitAttachId { get; set; }

    public string? AitStatus { get; set; }

    public string? AitIpaddress { get; set; }

    public int? AitCompId { get; set; }

    public int? AitIssueNameId { get; set; }

    public int? AitPgedetailId { get; set; }
}
