using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_RRF_PlanningSchecduling_Details")]
public partial class RiskRrfPlanningSchecdulingDetail
{
    [Column("RPD_PKID")]
    public int? RpdPkid { get; set; }

    [Column("RPD_AsgNo")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RpdAsgNo { get; set; }

    [Column("RPD_RefNO")]
    [StringLength(500)]
    [Unicode(false)]
    public string? RpdRefNo { get; set; }

    [Column("RPD_CustID")]
    public int? RpdCustId { get; set; }

    [Column("RPD_FunID")]
    public int? RpdFunId { get; set; }

    [Column("RPD_SubFunID")]
    public int? RpdSubFunId { get; set; }

    [Column("RPD_Title")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? RpdTitle { get; set; }

    [Column("RPD_Scope")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? RpdScope { get; set; }

    [Column("RPD_ScheduleStartDate", TypeName = "datetime")]
    public DateTime? RpdScheduleStartDate { get; set; }

    [Column("RPD_ScheduleClosure", TypeName = "datetime")]
    public DateTime? RpdScheduleClosure { get; set; }

    [Column("RPD_ReviewerTypeID")]
    public int? RpdReviewerTypeId { get; set; }

    [Column("RPD_ReviewerID")]
    public int? RpdReviewerId { get; set; }

    [Column("RPD_CrBy")]
    public int? RpdCrBy { get; set; }

    [Column("RPD_CrOn", TypeName = "datetime")]
    public DateTime? RpdCrOn { get; set; }

    [Column("RPD_UpdatedBy")]
    public int? RpdUpdatedBy { get; set; }

    [Column("RPD_UpdatedOn", TypeName = "datetime")]
    public DateTime? RpdUpdatedOn { get; set; }

    [Column("RPD_SubmittedBy")]
    public int? RpdSubmittedBy { get; set; }

    [Column("RPD_SubmittedOn", TypeName = "datetime")]
    public DateTime? RpdSubmittedOn { get; set; }

    [Column("RPD_IPaddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RpdIpaddress { get; set; }

    [Column("RPD_ConductingActualStartDate", TypeName = "datetime")]
    public DateTime? RpdConductingActualStartDate { get; set; }

    [Column("RPD_ConductingActualClosure", TypeName = "datetime")]
    public DateTime? RpdConductingActualClosure { get; set; }

    [Column("RPD_ConductingRemarks")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? RpdConductingRemarks { get; set; }

    [Column("RPD_ConductingCrBy")]
    public int? RpdConductingCrBy { get; set; }

    [Column("RPD_ConductingCrOn", TypeName = "datetime")]
    public DateTime? RpdConductingCrOn { get; set; }

    [Column("RPD_ConductingUpdatedBy")]
    public int? RpdConductingUpdatedBy { get; set; }

    [Column("RPD_ConductingUpdatedOn", TypeName = "datetime")]
    public DateTime? RpdConductingUpdatedOn { get; set; }

    [Column("RPD_ConductingSubmittedBy")]
    public int? RpdConductingSubmittedBy { get; set; }

    [Column("RPD_ConductingSubmittedOn", TypeName = "datetime")]
    public DateTime? RpdConductingSubmittedOn { get; set; }

    [Column("RPD_ConductingIPaddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RpdConductingIpaddress { get; set; }

    [Column("RPD_CompID")]
    public int? RpdCompId { get; set; }

    [Column("RPD_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RpdStatus { get; set; }

    [Column("RPD_YearID")]
    public int? RpdYearId { get; set; }

    [Column("RPD_PlanningAttachID")]
    public int? RpdPlanningAttachId { get; set; }

    [Column("RPD_ConductAttachID")]
    public int? RpdConductAttachId { get; set; }

    [Column("RPD_ConductingStatus")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RpdConductingStatus { get; set; }

    [Column("RPD_ConductingRRStatus")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RpdConductingRrstatus { get; set; }

    [Column("RPD_PGEDetailId")]
    public int? RpdPgedetailId { get; set; }

    [Column("RPD_ConductPGEDetailId")]
    public int? RpdConductPgedetailId { get; set; }
}
