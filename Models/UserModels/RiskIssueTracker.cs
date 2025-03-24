using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_IssueTracker")]
public partial class RiskIssueTracker
{
    [Column("RIT_PKID")]
    public int? RitPkid { get; set; }

    [Column("RIT_Source")]
    [StringLength(100)]
    [Unicode(false)]
    public string? RitSource { get; set; }

    [Column("RIT_IssueNo")]
    [StringLength(100)]
    [Unicode(false)]
    public string? RitIssueNo { get; set; }

    [Column("RIT_ReferenceNo")]
    [StringLength(100)]
    [Unicode(false)]
    public string? RitReferenceNo { get; set; }

    [Column("RIT_AsgNo")]
    public int? RitAsgNo { get; set; }

    [Column("RIT_FinancialYear")]
    public int? RitFinancialYear { get; set; }

    [Column("RIT_CustID")]
    public int? RitCustId { get; set; }

    [Column("RIT_FunID")]
    public int? RitFunId { get; set; }

    [Column("RIT_SubFunID")]
    public int? RitSubFunId { get; set; }

    [Column("RIT_IssueHeading")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? RitIssueHeading { get; set; }

    [Column("RIT_Issue_Desc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? RitIssueDesc { get; set; }

    [Column("RIT_RiskID")]
    public int? RitRiskId { get; set; }

    [Column("RIT_RiskTypeID")]
    public int? RitRiskTypeId { get; set; }

    [Column("RIT_ControlID")]
    public int? RitControlId { get; set; }

    [Column("RIT_ActualLoss")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? RitActualLoss { get; set; }

    [Column("RIT_ProbableLoss")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? RitProbableLoss { get; set; }

    [Column("RIT_ActionPlan")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? RitActionPlan { get; set; }

    [Column("RIT_TargetDate", TypeName = "datetime")]
    public DateTime? RitTargetDate { get; set; }

    [Column("RIT_OpenCloseStatus")]
    public int? RitOpenCloseStatus { get; set; }

    [Column("RIT_ManagerResponsible")]
    public int? RitManagerResponsible { get; set; }

    [Column("RIT_IndividualResponsible")]
    public int? RitIndividualResponsible { get; set; }

    [Column("RIT_Remaks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? RitRemaks { get; set; }

    [Column("RIT_AttchID")]
    public int? RitAttchId { get; set; }

    [Column("RIT_CrBy")]
    public int? RitCrBy { get; set; }

    [Column("RIT_CrOn", TypeName = "datetime")]
    public DateTime? RitCrOn { get; set; }

    [Column("RIT_UpdatedBy")]
    public int? RitUpdatedBy { get; set; }

    [Column("RIT_UpdatedOn", TypeName = "datetime")]
    public DateTime? RitUpdatedOn { get; set; }

    [Column("RIT_SubmittedBy")]
    public int? RitSubmittedBy { get; set; }

    [Column("RIT_SubmittedOn", TypeName = "datetime")]
    public DateTime? RitSubmittedOn { get; set; }

    [Column("RIT_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RitStatus { get; set; }

    [Column("RIT_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RitIpaddress { get; set; }

    [Column("RIT_CompID")]
    public int? RitCompId { get; set; }

    [Column("RIT_PGEDetailId")]
    public int? RitPgedetailId { get; set; }
}
