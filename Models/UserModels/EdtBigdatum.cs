using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_BIGDATA")]
public partial class EdtBigdatum
{
    [Column("BDT_BASENAME", TypeName = "numeric(10, 0)")]
    public decimal? BdtBasename { get; set; }

    [Column("BDT_BIGDATA", TypeName = "image")]
    public byte[]? BdtBigdata { get; set; }

    [Column("BDT_SIZE")]
    public double? BdtSize { get; set; }
}
