using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("sad_grplvl_members")]
public partial class SadGrplvlMember
{
    [Column("Gld_GrpLvlId")]
    public short? GldGrpLvlId { get; set; }

    [Column("Gld_UserId")]
    public short? GldUserId { get; set; }

    [Column("Gld_CrDate", TypeName = "smalldatetime")]
    public DateTime? GldCrDate { get; set; }

    [Column("Gld_FromDate", TypeName = "datetime")]
    public DateTime? GldFromDate { get; set; }

    [Column("Gld_ToDate", TypeName = "datetime")]
    public DateTime? GldToDate { get; set; }

    [Column("Gld_CrBy")]
    public short? GldCrBy { get; set; }

    [Column("Gld_GrpLvlPosn")]
    public short? GldGrpLvlPosn { get; set; }
}
