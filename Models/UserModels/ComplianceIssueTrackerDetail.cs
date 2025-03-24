using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Compliance_IssueTracker_details")]
public partial class ComplianceIssueTrackerDetail
{
    [Column("CIT_PKID")]
    public int? CitPkid { get; set; }

    [Column("CIT_YearID")]
    public int? CitYearId { get; set; }

    [Column("CIT_CustomerID")]
    public int? CitCustomerId { get; set; }

    [Column("CIT_ComplianceCodeID")]
    public int? CitComplianceCodeId { get; set; }

    [Column("CIT_ChecklistID")]
    public int? CitChecklistId { get; set; }

    [Column("CIT_IssueJobNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CitIssueJobNo { get; set; }

    [Column("CIT_FunctionID")]
    public int? CitFunctionId { get; set; }

    [Column("CIT_SubFunctionID")]
    public int? CitSubFunctionId { get; set; }

    [Column("CIT_ProcessID")]
    public int? CitProcessId { get; set; }

    [Column("CIT_SubProcessID")]
    public int? CitSubProcessId { get; set; }

    [Column("CIT_RiskID")]
    public int? CitRiskId { get; set; }

    [Column("CIT_ControlID")]
    public int? CitControlId { get; set; }

    [Column("CIT_CheckID")]
    public int? CitCheckId { get; set; }

    [Column("CIT_IssueHeading")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? CitIssueHeading { get; set; }

    [Column("CIT_IssueDetails")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? CitIssueDetails { get; set; }

    [Column("CIT_Impact")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CitImpact { get; set; }

    [Column("CIT_ActionPlan")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? CitActionPlan { get; set; }

    [Column("CIT_IssueRatingID")]
    public int? CitIssueRatingId { get; set; }

    [Column("CIT_ActualLoss")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CitActualLoss { get; set; }

    [Column("CIT_ProbableLoss")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CitProbableLoss { get; set; }

    [Column("CIT_TargetDate", TypeName = "datetime")]
    public DateTime? CitTargetDate { get; set; }

    [Column("CIT_ResponsibleFunctionID")]
    public int? CitResponsibleFunctionId { get; set; }

    [Column("CIT_FunctionManagerID")]
    public int? CitFunctionManagerId { get; set; }

    [Column("CIT_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? CitRemarks { get; set; }

    [Column("CIT_AttachID")]
    public int? CitAttachId { get; set; }

    [Column("CIT_IssueStatus")]
    [StringLength(5)]
    [Unicode(false)]
    public string? CitIssueStatus { get; set; }

    [Column("CIT_CreatedBy")]
    public int? CitCreatedBy { get; set; }

    [Column("CIT_CreatedOn", TypeName = "datetime")]
    public DateTime? CitCreatedOn { get; set; }

    [Column("CIT_UpdatedBy")]
    public int? CitUpdatedBy { get; set; }

    [Column("CIT_UpdatedOn", TypeName = "datetime")]
    public DateTime? CitUpdatedOn { get; set; }

    [Column("CIT_SubmittedBy")]
    public int? CitSubmittedBy { get; set; }

    [Column("CIT_SubmittedOn", TypeName = "datetime")]
    public DateTime? CitSubmittedOn { get; set; }

    [Column("CIT_Status")]
    [StringLength(15)]
    [Unicode(false)]
    public string? CitStatus { get; set; }

    [Column("CIT_CompID")]
    public int? CitCompId { get; set; }

    [Column("CIT_IPAddress")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CitIpaddress { get; set; }

    [Column("CIT_RiskTypeID")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CitRiskTypeId { get; set; }

    [Column("CIT_PGEDetailId")]
    public int? CitPgedetailId { get; set; }
}
