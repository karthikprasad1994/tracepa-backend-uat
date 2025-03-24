using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("LOE_ReAmbersment")]
public partial class LoeReAmbersment
{
    [Column("LAR_ID")]
    public int? LarId { get; set; }

    [Column("LAR_LOEID")]
    public int? LarLoeid { get; set; }

    [Column("LAR_ReambersmentID")]
    public int? LarReambersmentId { get; set; }

    [Column("LAR_Charges")]
    public int? LarCharges { get; set; }

    [Column("LAR_ReambName")]
    [StringLength(100)]
    [Unicode(false)]
    public string? LarReambName { get; set; }

    [Column("LAR_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? LarDelflag { get; set; }

    [Column("LAR_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? LarStatus { get; set; }

    [Column("LAR_CrBy")]
    public int? LarCrBy { get; set; }

    [Column("LAR_CrOn", TypeName = "datetime")]
    public DateTime? LarCrOn { get; set; }

    [Column("LAR_UpdatedBy")]
    public int? LarUpdatedBy { get; set; }

    [Column("LAR_UpdatedOn", TypeName = "datetime")]
    public DateTime? LarUpdatedOn { get; set; }

    [Column("LAR_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? LarIpaddress { get; set; }

    [Column("LAR_CompID")]
    public int? LarCompId { get; set; }
}
