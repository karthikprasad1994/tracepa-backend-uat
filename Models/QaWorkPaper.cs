using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class QaWorkPaper
{
    public int? QawPkid { get; set; }

    public int? QawYearId { get; set; }

    public int? QawCustId { get; set; }

    public int? QawAuditCode { get; set; }

    public int? QawFunctionId { get; set; }

    public int? QawSubFunctionId { get; set; }

    public int? QawProcessId { get; set; }

    public int? QawSubProcessId { get; set; }

    public int? QawRiskId { get; set; }

    public int? QawControlId { get; set; }

    public int? QawChecksId { get; set; }

    public string? QawWorkPaperNo { get; set; }

    public int? QawTypeofTestId { get; set; }

    public int? QawConclusionId { get; set; }

    public string? QawWorkPaperDone { get; set; }

    public string? QawAuditorObservationName { get; set; }

    public string? QawNote { get; set; }

    public string? QawAuditeeResponseName { get; set; }

    public string? QawResponse { get; set; }

    public string? QawAuditorRemarks { get; set; }

    public string? QawReviewerRemarks { get; set; }

    public int? QawOpenCloseStatus { get; set; }

    public int? QawAttachId { get; set; }

    public int? QawCrBy { get; set; }

    public DateTime? QawCrOn { get; set; }

    public int? QawUpdatedBy { get; set; }

    public DateTime? QawUpdatedOn { get; set; }

    public int? QawSubmittedBy { get; set; }

    public DateTime? QawSubmittedOn { get; set; }

    public int? QawReviewedBy { get; set; }

    public DateTime? QawReviewedOn { get; set; }

    public string? QawStatus { get; set; }

    public string? QawIpaddress { get; set; }

    public int? QawCompId { get; set; }

    public int? QawPgedetailId { get; set; }
}
