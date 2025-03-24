using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("GRACe_GrossControlScore")]
public partial class GraceGrossControlScore
{
    [Column("GG_PKID")]
    public int? GgPkid { get; set; }

    [Column("GG_DE")]
    public int? GgDe { get; set; }

    [Column("GG_OE")]
    public int? GgOe { get; set; }

    [Column("GG_ControlScore")]
    public int? GgControlScore { get; set; }

    [Column("GG_CompID")]
    public int? GgCompId { get; set; }
}
