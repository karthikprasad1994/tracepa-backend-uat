using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_KCC_PlanningSchecduling_Details")]
public partial class RiskKccPlanningSchecdulingDetail
{
    [Column("KCC_PKID")]
    public int? KccPkid { get; set; }

    [Column("KCC_CustID")]
    public int? KccCustId { get; set; }

    [Column("KCC_AsgNo")]
    [StringLength(25)]
    [Unicode(false)]
    public string? KccAsgNo { get; set; }

    [Column("KCC_RiskReportReferenceNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? KccRiskReportReferenceNo { get; set; }

    [Column("KCC_FunID")]
    public int? KccFunId { get; set; }

    [Column("KCC_SubFunID")]
    public int? KccSubFunId { get; set; }

    [Column("KCC_YearID")]
    public int? KccYearId { get; set; }

    [Column("KCC_Title")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? KccTitle { get; set; }

    [Column("KCC_Scope")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? KccScope { get; set; }

    [Column("KCC_ScheduleStartDate", TypeName = "datetime")]
    public DateTime? KccScheduleStartDate { get; set; }

    [Column("KCC_ScheduleClosure", TypeName = "datetime")]
    public DateTime? KccScheduleClosure { get; set; }

    [Column("KCC_ReviewerTypeID")]
    public int? KccReviewerTypeId { get; set; }

    [Column("KCC_ReviewerID")]
    public int? KccReviewerId { get; set; }

    [Column("KCC_CrBy")]
    public int? KccCrBy { get; set; }

    [Column("KCC_CrOn", TypeName = "datetime")]
    public DateTime? KccCrOn { get; set; }

    [Column("KCC_UpdatedBy")]
    public int? KccUpdatedBy { get; set; }

    [Column("KCC_UpdatedOn", TypeName = "datetime")]
    public DateTime? KccUpdatedOn { get; set; }

    [Column("KCC_SubmittedBy")]
    public int? KccSubmittedBy { get; set; }

    [Column("KCC_SubmittedOn", TypeName = "datetime")]
    public DateTime? KccSubmittedOn { get; set; }

    [Column("KCC_IPaddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? KccIpaddress { get; set; }

    [Column("KCC_ConductingActualStartDate", TypeName = "datetime")]
    public DateTime? KccConductingActualStartDate { get; set; }

    [Column("KCC_ConductingActualClosure", TypeName = "datetime")]
    public DateTime? KccConductingActualClosure { get; set; }

    [Column("KCC_ConductingRemarks")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? KccConductingRemarks { get; set; }

    [Column("KCC_ConductingCrBy")]
    public int? KccConductingCrBy { get; set; }

    [Column("KCC_ConductingCrOn", TypeName = "datetime")]
    public DateTime? KccConductingCrOn { get; set; }

    [Column("KCC_ConductingUpdatedBy")]
    public int? KccConductingUpdatedBy { get; set; }

    [Column("KCC_ConductingUpdatedOn", TypeName = "datetime")]
    public DateTime? KccConductingUpdatedOn { get; set; }

    [Column("KCC_ConductingSubmittedBy")]
    public int? KccConductingSubmittedBy { get; set; }

    [Column("KCC_ConductingSubmittedOn", TypeName = "datetime")]
    public DateTime? KccConductingSubmittedOn { get; set; }

    [Column("KCC_ConductingIPaddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? KccConductingIpaddress { get; set; }

    [Column("KCC_CompID")]
    public int? KccCompId { get; set; }

    [Column("KCC_ConductingStatus")]
    [StringLength(25)]
    [Unicode(false)]
    public string? KccConductingStatus { get; set; }

    [Column("KCC_ConductingKCCStatus")]
    [StringLength(25)]
    [Unicode(false)]
    public string? KccConductingKccstatus { get; set; }

    [Column("KCC_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? KccStatus { get; set; }

    [Column("KCC_PlanningAttachID")]
    public int? KccPlanningAttachId { get; set; }

    [Column("KCC_ConductAttachID")]
    public int? KccConductAttachId { get; set; }

    [Column("KCC_PlanningPGEDetailId")]
    public int? KccPlanningPgedetailId { get; set; }

    [Column("KCC_ConductPGEDetailId")]
    public int? KccConductPgedetailId { get; set; }
}
