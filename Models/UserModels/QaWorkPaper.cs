using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("QA_WorkPaper")]
public partial class QaWorkPaper
{
    [Column("QAW_PKID")]
    public int? QawPkid { get; set; }

    [Column("QAW_YearID")]
    public int? QawYearId { get; set; }

    [Column("QAW_CustID")]
    public int? QawCustId { get; set; }

    [Column("QAW_AuditCode")]
    public int? QawAuditCode { get; set; }

    [Column("QAW_FunctionID")]
    public int? QawFunctionId { get; set; }

    [Column("QAW_SubFunctionID")]
    public int? QawSubFunctionId { get; set; }

    [Column("QAW_ProcessID")]
    public int? QawProcessId { get; set; }

    [Column("QAW_SubProcessID")]
    public int? QawSubProcessId { get; set; }

    [Column("QAW_RiskID")]
    public int? QawRiskId { get; set; }

    [Column("QAW_ControlID")]
    public int? QawControlId { get; set; }

    [Column("QAW_ChecksID")]
    public int? QawChecksId { get; set; }

    [Column("QAW_WorkPaperNo")]
    [StringLength(500)]
    [Unicode(false)]
    public string? QawWorkPaperNo { get; set; }

    [Column("QAW_TypeofTestID")]
    public int? QawTypeofTestId { get; set; }

    [Column("QAW_ConclusionID")]
    public int? QawConclusionId { get; set; }

    [Column("QAW_WorkPaperDone")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? QawWorkPaperDone { get; set; }

    [Column("QAW_AuditorObservationName")]
    [StringLength(500)]
    [Unicode(false)]
    public string? QawAuditorObservationName { get; set; }

    [Column("QAW_Note")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? QawNote { get; set; }

    [Column("QAW_AuditeeResponseName")]
    [StringLength(500)]
    [Unicode(false)]
    public string? QawAuditeeResponseName { get; set; }

    [Column("QAW_Response")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? QawResponse { get; set; }

    [Column("QAW_AuditorRemarks")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? QawAuditorRemarks { get; set; }

    [Column("QAW_ReviewerRemarks")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? QawReviewerRemarks { get; set; }

    [Column("QAW_OpenCloseStatus")]
    public int? QawOpenCloseStatus { get; set; }

    [Column("QAW_AttachID")]
    public int? QawAttachId { get; set; }

    [Column("QAW_CrBy")]
    public int? QawCrBy { get; set; }

    [Column("QAW_CrOn", TypeName = "datetime")]
    public DateTime? QawCrOn { get; set; }

    [Column("QAW_UpdatedBy")]
    public int? QawUpdatedBy { get; set; }

    [Column("QAW_UpdatedOn", TypeName = "datetime")]
    public DateTime? QawUpdatedOn { get; set; }

    [Column("QAW_SubmittedBy")]
    public int? QawSubmittedBy { get; set; }

    [Column("QAW_SubmittedOn", TypeName = "datetime")]
    public DateTime? QawSubmittedOn { get; set; }

    [Column("QAW_ReviewedBy")]
    public int? QawReviewedBy { get; set; }

    [Column("QAW_ReviewedOn", TypeName = "datetime")]
    public DateTime? QawReviewedOn { get; set; }

    [Column("QAW_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? QawStatus { get; set; }

    [Column("QAW_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? QawIpaddress { get; set; }

    [Column("QAW_CompID")]
    public int? QawCompId { get; set; }

    [Column("QAW_PGEDetailId")]
    public int? QawPgedetailId { get; set; }
}
