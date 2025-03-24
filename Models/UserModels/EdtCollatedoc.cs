using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("edt_collatedoc")]
public partial class EdtCollatedoc
{
    [Column("CLD_COLLATENO", TypeName = "numeric(5, 0)")]
    public decimal? CldCollateno { get; set; }

    [Column("CLD_DOCID", TypeName = "numeric(5, 0)")]
    public decimal? CldDocid { get; set; }

    [Column("CLD_PAGEID", TypeName = "numeric(5, 0)")]
    public decimal? CldPageid { get; set; }
}
