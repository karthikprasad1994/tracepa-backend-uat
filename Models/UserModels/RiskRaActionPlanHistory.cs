using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_RA_ActionPlan_History")]
public partial class RiskRaActionPlanHistory
{
    [Column("RAH_PKID")]
    public int? RahPkid { get; set; }

    [Column("RAH_RAPKID")]
    public int? RahRapkid { get; set; }

    [Column("RAH_CUSTID")]
    public int? RahCustid { get; set; }

    [Column("RAH_FUNID")]
    public int? RahFunid { get; set; }

    [Column("RAH_FinancialYear")]
    public int? RahFinancialYear { get; set; }

    [Column("RAH_FactorIncrease")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? RahFactorIncrease { get; set; }

    [Column("RAH_FactorDecrease")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? RahFactorDecrease { get; set; }

    [Column("RAH_ActionPlan")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? RahActionPlan { get; set; }

    [Column("RAH_TargetDate", TypeName = "datetime")]
    public DateTime? RahTargetDate { get; set; }

    [Column("RAH_CrBy")]
    public int? RahCrBy { get; set; }

    [Column("RAH_CrOn", TypeName = "datetime")]
    public DateTime? RahCrOn { get; set; }

    [Column("RAH_CompID")]
    public int? RahCompId { get; set; }
}
