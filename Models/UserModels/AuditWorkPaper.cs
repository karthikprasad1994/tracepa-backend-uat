using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_WorkPaper")]
public partial class AuditWorkPaper
{
    [Column("AWP_PKID")]
    public int? AwpPkid { get; set; }

    [Column("AWP_YearID")]
    public int? AwpYearId { get; set; }

    [Column("AWP_CustID")]
    public int? AwpCustId { get; set; }

    [Column("AWP_AuditCode")]
    public int? AwpAuditCode { get; set; }

    [Column("AWP_FunctionID")]
    public int? AwpFunctionId { get; set; }

    [Column("AWP_SubFunctionID")]
    public int? AwpSubFunctionId { get; set; }

    [Column("AWP_ProcessID")]
    public int? AwpProcessId { get; set; }

    [Column("AWP_SubProcessID")]
    public int? AwpSubProcessId { get; set; }

    [Column("AWP_RiskID")]
    public int? AwpRiskId { get; set; }

    [Column("AWP_ControlID")]
    public int? AwpControlId { get; set; }

    [Column("AWP_ChecksID")]
    public int? AwpChecksId { get; set; }

    [Column("AWP_WorkPaperNo")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AwpWorkPaperNo { get; set; }

    [Column("AWP_TypeofTestID")]
    public int? AwpTypeofTestId { get; set; }

    [Column("AWP_ConclusionID")]
    public int? AwpConclusionId { get; set; }

    [Column("AWP_WorkPaperDone")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AwpWorkPaperDone { get; set; }

    [Column("AWP_AuditorObservationName")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AwpAuditorObservationName { get; set; }

    [Column("AWP_Note")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AwpNote { get; set; }

    [Column("AWP_AuditeeResponseName")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AwpAuditeeResponseName { get; set; }

    [Column("AWP_Response")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AwpResponse { get; set; }

    [Column("AWP_AuditorRemarks")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AwpAuditorRemarks { get; set; }

    [Column("AWP_ReviewerRemarks")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AwpReviewerRemarks { get; set; }

    [Column("AWP_OpenCloseStatus")]
    public int? AwpOpenCloseStatus { get; set; }

    [Column("AWP_AttachID")]
    public int? AwpAttachId { get; set; }

    [Column("AWP_CrBy")]
    public int? AwpCrBy { get; set; }

    [Column("AWP_CrOn", TypeName = "datetime")]
    public DateTime? AwpCrOn { get; set; }

    [Column("AWP_UpdatedBy")]
    public int? AwpUpdatedBy { get; set; }

    [Column("AWP_UpdatedOn", TypeName = "datetime")]
    public DateTime? AwpUpdatedOn { get; set; }

    [Column("AWP_SubmittedBy")]
    public int? AwpSubmittedBy { get; set; }

    [Column("AWP_SubmittedOn", TypeName = "datetime")]
    public DateTime? AwpSubmittedOn { get; set; }

    [Column("AWP_ReviewedBy")]
    public int? AwpReviewedBy { get; set; }

    [Column("AWP_ReviewedOn", TypeName = "datetime")]
    public DateTime? AwpReviewedOn { get; set; }

    [Column("AWP_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AwpStatus { get; set; }

    [Column("AWP_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AwpIpaddress { get; set; }

    [Column("AWP_CompID")]
    public int? AwpCompId { get; set; }

    [Column("AWP_PGEDetailId")]
    public int? AwpPgedetailId { get; set; }
}
