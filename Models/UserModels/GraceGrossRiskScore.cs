using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("GRACe_GrossRiskScore")]
public partial class GraceGrossRiskScore
{
    [Column("GG_PKID")]
    public int? GgPkid { get; set; }

    [Column("GG_Impact")]
    public int? GgImpact { get; set; }

    [Column("GG_Likelihood")]
    public int? GgLikelihood { get; set; }

    [Column("GG_RiskScore")]
    public int? GgRiskScore { get; set; }

    [Column("GG_CompID")]
    public int? GgCompId { get; set; }
}
