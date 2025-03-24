using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("GRACe_RiskControlMatrix")]
public partial class GraceRiskControlMatrix
{
    [Column("GG_PKID")]
    public int? GgPkid { get; set; }

    [Column("GG_Risk")]
    public int? GgRisk { get; set; }

    [Column("GG_Controls")]
    public int? GgControls { get; set; }

    [Column("GG_RiskControlScore")]
    public int? GgRiskControlScore { get; set; }

    [Column("GG_CompID")]
    public int? GgCompId { get; set; }
}
