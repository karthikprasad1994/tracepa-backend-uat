using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditWorkPaper
{
    public int? AwpPkid { get; set; }

    public int? AwpYearId { get; set; }

    public int? AwpCustId { get; set; }

    public int? AwpAuditCode { get; set; }

    public int? AwpFunctionId { get; set; }

    public int? AwpSubFunctionId { get; set; }

    public int? AwpProcessId { get; set; }

    public int? AwpSubProcessId { get; set; }

    public int? AwpRiskId { get; set; }

    public int? AwpControlId { get; set; }

    public int? AwpChecksId { get; set; }

    public string? AwpWorkPaperNo { get; set; }

    public int? AwpTypeofTestId { get; set; }

    public int? AwpConclusionId { get; set; }

    public string? AwpWorkPaperDone { get; set; }

    public string? AwpAuditorObservationName { get; set; }

    public string? AwpNote { get; set; }

    public string? AwpAuditeeResponseName { get; set; }

    public string? AwpResponse { get; set; }

    public string? AwpAuditorRemarks { get; set; }

    public string? AwpReviewerRemarks { get; set; }

    public int? AwpOpenCloseStatus { get; set; }

    public int? AwpAttachId { get; set; }

    public int? AwpCrBy { get; set; }

    public DateTime? AwpCrOn { get; set; }

    public int? AwpUpdatedBy { get; set; }

    public DateTime? AwpUpdatedOn { get; set; }

    public int? AwpSubmittedBy { get; set; }

    public DateTime? AwpSubmittedOn { get; set; }

    public int? AwpReviewedBy { get; set; }

    public DateTime? AwpReviewedOn { get; set; }

    public string? AwpStatus { get; set; }

    public string? AwpIpaddress { get; set; }

    public int? AwpCompId { get; set; }

    public int? AwpPgedetailId { get; set; }
}
