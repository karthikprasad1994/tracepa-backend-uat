using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("LOE_Resources")]
public partial class LoeResource
{
    [Column("LOER_ID")]
    public int? LoerId { get; set; }

    [Column("LOER_LOEID")]
    public int? LoerLoeid { get; set; }

    [Column("LOER_CategoryID")]
    public int? LoerCategoryId { get; set; }

    [Column("LOER_NoResources")]
    public short? LoerNoResources { get; set; }

    [Column("LOER_ChargesPerDay")]
    public int? LoerChargesPerDay { get; set; }

    [Column("LOER_CategoryName")]
    [StringLength(100)]
    [Unicode(false)]
    public string? LoerCategoryName { get; set; }

    [Column("LOER_NoDays")]
    public int? LoerNoDays { get; set; }

    [Column("LOER_ResTotal")]
    public int? LoerResTotal { get; set; }

    [Column("LOER_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? LoerDelflag { get; set; }

    [Column("LOER_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? LoerStatus { get; set; }

    [Column("LOER_CrBy")]
    public int? LoerCrBy { get; set; }

    [Column("LOER_CrOn", TypeName = "datetime")]
    public DateTime? LoerCrOn { get; set; }

    [Column("LOER_UpdatedBy")]
    public int? LoerUpdatedBy { get; set; }

    [Column("LOER_UpdatedOn", TypeName = "datetime")]
    public DateTime? LoerUpdatedOn { get; set; }

    [Column("LOER_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? LoerIpaddress { get; set; }

    [Column("LOER_CompID")]
    public int? LoerCompId { get; set; }
}
