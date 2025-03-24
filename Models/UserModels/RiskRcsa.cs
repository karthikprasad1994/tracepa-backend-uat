using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_RCSA")]
public partial class RiskRcsa
{
    [Column("RCSA_PKID")]
    public int? RcsaPkid { get; set; }

    [Column("RCSA_AsgNo")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RcsaAsgNo { get; set; }

    [Column("RCSA_FinancialYear")]
    public int? RcsaFinancialYear { get; set; }

    [Column("RCSA_CustID")]
    public int? RcsaCustId { get; set; }

    [Column("RCSA_FunID")]
    public int? RcsaFunId { get; set; }

    [Column("RCSA_OwnerID")]
    public int? RcsaOwnerId { get; set; }

    [Column("RCSA_TargetDate", TypeName = "datetime")]
    public DateTime? RcsaTargetDate { get; set; }

    [Column("RCSA_ActionPlan")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? RcsaActionPlan { get; set; }

    [Column("RCSA_FactorIncrease")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? RcsaFactorIncrease { get; set; }

    [Column("RCSA_FactorDecrease")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? RcsaFactorDecrease { get; set; }

    [Column("RCSA_Comments")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? RcsaComments { get; set; }

    [Column("RCSA_NetScore")]
    public float? RcsaNetScore { get; set; }

    [Column("RCSA_CrBy")]
    public int? RcsaCrBy { get; set; }

    [Column("RCSA_CrOn", TypeName = "datetime")]
    public DateTime? RcsaCrOn { get; set; }

    [Column("RCSA_RUpdatedBy")]
    public int? RcsaRupdatedBy { get; set; }

    [Column("RCSA_RUpdatedOn", TypeName = "datetime")]
    public DateTime? RcsaRupdatedOn { get; set; }

    [Column("RCSA_RSubmittedBy")]
    public int? RcsaRsubmittedBy { get; set; }

    [Column("RCSA_RSubmittedOn", TypeName = "datetime")]
    public DateTime? RcsaRsubmittedOn { get; set; }

    [Column("RCSA_BUpdatedBy")]
    public int? RcsaBupdatedBy { get; set; }

    [Column("RCSA_BUpdatedOn", TypeName = "datetime")]
    public DateTime? RcsaBupdatedOn { get; set; }

    [Column("RCSA_BSubmittedBy")]
    public int? RcsaBsubmittedBy { get; set; }

    [Column("RCSA_BSubmittedOn", TypeName = "datetime")]
    public DateTime? RcsaBsubmittedOn { get; set; }

    [Column("RCSA_ReAssignBy")]
    public int? RcsaReAssignBy { get; set; }

    [Column("RCSA_ReAssignOn", TypeName = "datetime")]
    public DateTime? RcsaReAssignOn { get; set; }

    [Column("RCSA_ApprovedBy")]
    public int? RcsaApprovedBy { get; set; }

    [Column("RCSA_ApprovedOn", TypeName = "datetime")]
    public DateTime? RcsaApprovedOn { get; set; }

    [Column("RCSA_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RcsaStatus { get; set; }

    [Column("RCSA_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RcsaIpaddress { get; set; }

    [Column("RCSA_CompID")]
    public int? RcsaCompId { get; set; }
}
