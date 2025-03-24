using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_IssueTracker_Details")]
public partial class AuditIssueTrackerDetail
{
    [Column("AIT_PKID")]
    public int? AitPkid { get; set; }

    [Column("AIT_YearID")]
    public int? AitYearId { get; set; }

    [Column("AIT_CustID")]
    public int? AitCustId { get; set; }

    [Column("AIT_AuditCode")]
    public int? AitAuditCode { get; set; }

    [Column("AIT_WorkPaperID")]
    public int? AitWorkPaperId { get; set; }

    [Column("AIT_FunctionID")]
    public int? AitFunctionId { get; set; }

    [Column("AIT_SubFunctionID")]
    public int? AitSubFunctionId { get; set; }

    [Column("AIT_ProcessID")]
    public int? AitProcessId { get; set; }

    [Column("AIT_SubProcessID")]
    public int? AitSubProcessId { get; set; }

    [Column("AIT_RiskID")]
    public int? AitRiskId { get; set; }

    [Column("AIT_ControlID")]
    public int? AitControlId { get; set; }

    [Column("AIT_CheckID")]
    public int? AitCheckId { get; set; }

    [Column("AIT_IssueJobNo")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AitIssueJobNo { get; set; }

    [Column("AIT_SeverityID")]
    public int? AitSeverityId { get; set; }

    [Column("AIT_RiskCategoryID")]
    public int? AitRiskCategoryId { get; set; }

    [Column("AIT_IssueName")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AitIssueName { get; set; }

    [Column("AIT_Criteria")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AitCriteria { get; set; }

    [Column("AIT_Condition")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AitCondition { get; set; }

    [Column("AIT_Details")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AitDetails { get; set; }

    [Column("AIT_Impact")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AitImpact { get; set; }

    [Column("AIT_RootCause")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AitRootCause { get; set; }

    [Column("AIT_SuggestedRemedies")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AitSuggestedRemedies { get; set; }

    [Column("AIT_OpenCloseStatus")]
    public int? AitOpenCloseStatus { get; set; }

    [Column("AIT_AuditorRemarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AitAuditorRemarks { get; set; }

    [Column("AIT_ReviewerRemarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AitReviewerRemarks { get; set; }

    [Column("AIT_CreatedBy")]
    public int? AitCreatedBy { get; set; }

    [Column("AIT_CreatedOn", TypeName = "datetime")]
    public DateTime? AitCreatedOn { get; set; }

    [Column("AIT_UpdatedBy")]
    public int? AitUpdatedBy { get; set; }

    [Column("AIT_UpdatedOn", TypeName = "datetime")]
    public DateTime? AitUpdatedOn { get; set; }

    [Column("AIT_ReviewedBy")]
    public int? AitReviewedBy { get; set; }

    [Column("AIT_ReviewedOn", TypeName = "datetime")]
    public DateTime? AitReviewedOn { get; set; }

    [Column("AIT_AttachID")]
    public int? AitAttachId { get; set; }

    [Column("AIT_Status")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AitStatus { get; set; }

    [Column("AIT_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AitIpaddress { get; set; }

    [Column("AIT_CompID")]
    public int? AitCompId { get; set; }

    [Column("AIT_IssueNameID")]
    public int? AitIssueNameId { get; set; }

    [Column("AIT_PGEDetailId")]
    public int? AitPgedetailId { get; set; }
}
